using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Diagnostics;
using System.Collections.ObjectModel;
using OxyPlot;

namespace TransferFunctionLib
{
    /// <summary>
    /// Klasa elementu analizujcego czestotliwosciowo transmitancje
    /// </summary>
    internal class FrequencyAnalizer
    {
        /// <summary>
        /// Wspolczynniki licznika transmitancji po aproksymacji Pade
        /// </summary>
        private Double[] nomFactors;

        /// <summary>
        /// Wspolczynniki mianownika transmitancji po aproksymacji Pade
        /// </summary>
        private Double[] denFactors;

        /// <summary>
        /// Wspolczynniki licznika czesci rzeczywsitej
        /// </summary>
        private Double[] realNomFrequencyFactors;

        /// <summary>
        /// Wspolczynniki licznika czesci urojonej
        /// </summary>
        private Double[] imgNomFrequencyFactors;

        /// <summary>
        /// Wspolczynniki mianownika czesci urojonej i rzeczywistej
        /// </summary>
        private Double[] denFrequencyFactors;

        /// <summary>
        /// Wspolczynniki rownania na wyznaczenie zapasu fazy
        /// </summary>
        private Double[] phaseMarginFactors;

        private Double[,] phaseRanges= null;
        /// <summary>
        /// Zakresy przejscia fazy przez oś Rez po prawej stronie - zakresy przeliczenia atan
        /// </summary>
        private Double[,] PhaseRanges
        {
            get
            {
                if(phaseRanges == null)
                {
                    phaseRanges = CalculatePhaseRanges();
                }

                return phaseRanges;
            }
        }

        private ZeroPoint[] imgZeroPoints = null;
        /// <summary>
        /// Punkty dla ktorych czesc urojona jest rowna 0
        /// Posortowane pod wzgledem czestotliwosci
        /// </summary>
        public ZeroPoint[] ImgZeroPoints
        {
            get
            {
                if (imgZeroPoints == null)
                {
                    imgZeroPoints = GetImgZeroPoints();
                }

                return imgZeroPoints;
            }

        }

        private Margin[] gainMargins = null;
        /// <summary>
        /// Zapasy modulu
        /// Posortowane pod wzgledem czestotliwosci
        /// </summary>
        public Margin[] GainMargins
        {
            get
            {
                if (gainMargins == null)
                {
                    gainMargins = GetGainMargins();
                }

                return gainMargins;
            }

        }

        private Margin gainMargin = null;
        /// <summary>
        /// Zapas modułu
        /// </summary>
        public Margin GainMargin
        {
            get
            {
                if(gainMargin == null)
                {
                    gainMargin = CalculateGainMargin();
                }

                return gainMargin;
            }
            private set
            {

            }
        }

        private Margin[] phaseMargins = null;
        /// <summary>
        /// Zapasy fazy
        /// Posortowane pod wzgledem czestotliwosci
        /// </summary>
        public Margin[] PhaseMargins
        {
            get
            {
                if (phaseMargins == null)
                {
                    phaseMargins = GetPhaseMargins();
                }

                return phaseMargins;

            }
        }

        private Margin phaseMargin = null;
        /// <summary>
        /// Zapas fazy
        /// </summary>
        public Margin PhaseMargin
        {
            get
            {
                if(phaseMargin == null)
                {
                    phaseMargin = CalculatePhaseMargin();
                }

                return phaseMargin;
            }

            private set
            { 
            
            }
        }
        
        /// <summary>
        /// Metoda wyznaczajaca zapas modułu
        /// </summary>
        /// <returns>
        /// Zapas modułu
        /// </returns>
        private Margin CalculateGainMargin()
        {
            //Pobranie najmniejszego z zapasow modulu jako modulu glownego
            if(GainMargins.Length == 0)
            {
                return null;
            }

            else
            {
                return (from margin in GainMargins
                       where margin.Value == GainMargins.Min((x) => (x.Value))
                       select margin).First();
            }
        }

        /// <summary>
        /// Metoda pobierajaca minimalny zapas fazy
        /// </summary>
        /// <returns>
        /// Minimalny zapas fazy
        /// </returns>
        private Margin CalculatePhaseMargin()
        {
            //Pobranie najmniejszego (co do modulu - zapasy fazy moga miec dwa znaki) zapasu fazy
            if (PhaseMargins.Length == 0)
            {
                return null;
            }

            Double minPhaseMarginValue = (from margin in PhaseMargins
                                          select margin).Min((phaseMargin) => (Math.Abs(phaseMargin.Value)));

            return (from margin in PhaseMargins
                    where Math.Abs(margin.Value) == minPhaseMarginValue
                    select margin).First();
        }

        private Double[] automaticDisplayRanges = null;
        /// <summary>
        /// Automatycznie wzynaczany zakres wyznaczania charakterystyk
        /// </summary>
        public Double[] AutomaticDisplayRanges
        {
            get
            {
                if(automaticDisplayRanges == null)
                {
                    automaticDisplayRanges = CalculateAutomaticRange();
                }

                return automaticDisplayRanges;
            }
        }

        private FrequencyPoint pointForOmegaZero = null;
        /// <summary>
        /// Punkt charakterysytki dla zerowej czestotliwosci
        /// </summary>
        public FrequencyPoint PointForOmegaZero
        {
            get
            {
                if(pointForOmegaZero == null)
                {
                    pointForOmegaZero = GetPointForOmegaZero();

                }
                return pointForOmegaZero;
            }
        }

        private FrequencyPoint pointForOmegaInf = null;
        /// <summary>
        /// Punkt charakterysytki dla nieskonczonej czestotliwosci
        /// </summary>
        public FrequencyPoint PointForOmegaInf
        {
            get
            {
                if(pointForOmegaInf == null)
                {
                    pointForOmegaInf = GetPointForOmegaInf();
                }

                return pointForOmegaInf;
            }
        }
        
        /// <summary>
        /// Stopien aproksymacji Pade
        /// </summary>
        private int padeRank;

        /// <summary>
        /// Konstruktor elementu analizujcego czestotliwosciowo transmitancje
        /// </summary>
        /// <param name="transferFunction">
        /// Transmitancja rozpatrywana
        /// </param>
        /// <param name="padeRank">
        /// Rzad aproksymacji pade
        /// </param>
        public FrequencyAnalizer(ContinousTransferFunction transferFunction, int padeRank = 5)
        {
            //Inicjalizacja rzedu aproksymacji pade
            this.padeRank = padeRank;

            //aproksymacja pade podanej transmitancji
            ContinousTransferFunction padeApproximatedTransferFunction = TransferFunctionCalculator.ConnectTransferFunctions(transferFunction, TransferFunctionCalculator.PadeApproximation(transferFunction.timeDelay, padeRank));
            
            //Ustawienie mianownika i licznika transmitancji po aproksymacji
            this.nomFactors = padeApproximatedTransferFunction.nomFactors;
            this.denFactors = padeApproximatedTransferFunction.denFactors;

            //Ustawienie wspolczynnikow licznikow i mianownika czesci transmitancji rzeczywistej i urojonej
            this.realNomFrequencyFactors = CalculateRealNomFrequencyFactors();
            this.imgNomFrequencyFactors = CalculateImgNomFrequencyFactors();
            this.denFrequencyFactors = CalculateDenFrequencyFactors();

            //Ustawienie wspolczynnikow do wyznaczania zapasu fazy
            this.phaseMarginFactors = CalculatePhaseMarginFactors();

        }

        /// <summary>
        /// Metoda wyznaczajaca wspolczynniki licznika czesci rzeczywistej
        /// </summary>
        /// <returns>
        /// Wspolczynniki licznika czesci rzeczywistej
        /// </returns>
        private Double[] CalculateRealNomFrequencyFactors()
        {
            //Pogrupowanie w oddzielne kolekcje wspolczynnikow rzeczywistych i urojonych licznika
            Double[] nomRealFactors = new Double[nomFactors.Length];
            Double[] nomImgFactors = new Double[nomFactors.Length];

            for (int i = 0; i < nomFactors.Length; i++)
            {
                if (i % 2 == 0)
                {
                    nomRealFactors[i] = Complex.Pow(Complex.ImaginaryOne, i).Real * nomFactors[i];
                }
                else
                {
                    nomImgFactors[i] = Complex.Pow(Complex.ImaginaryOne, i - 1).Real * nomFactors[i];
                }
            }

            //Pogrupowanie w oddzielne kolekcje wspolczynnikow rzeczywistych i urojonych miankownika
            Double[] denRealFactors = new Double[denFactors.Length];
            Double[] denImgFactors = new Double[denFactors.Length];

            for (int i = 0; i < denFactors.Length; i++)
            {
                if (i % 2 == 0)
                {
                    denRealFactors[i] = Complex.Pow(Complex.ImaginaryOne, i).Real * denFactors[i];
                }
                else
                {
                    denImgFactors[i] = Complex.Pow(Complex.ImaginaryOne, i - 1).Real * denFactors[i];
                }
            }

            //[A] + [B]j    ([A][C] + [B][D]) + j([B][C]-[A][D])
            //---------- = ---------------------------------------
            //[C] + [D]j                [C][C] + [D][D]

            //Stworzenie wektorow AC i BD
            Double[] ACMultiplication = TransferFunctionCalculator.CrossFactors(denRealFactors, nomRealFactors);
            Double[] BDMultiplication = TransferFunctionCalculator.CrossFactors(denImgFactors, nomImgFactors);

            //Zsumowanie wektorow ( nalezy sprawdzic ktory jest dluzszy zeby poprawnie je dodac )
            if (ACMultiplication.Length > BDMultiplication.Length)
            {
                for (int i = 0; i < BDMultiplication.Length; i++)
                {
                    ACMultiplication[i] += BDMultiplication[i];
                }

                return ACMultiplication;
            }
            else
            {
                for (int i = 0; i < ACMultiplication.Length; i++)
                {
                    BDMultiplication[i] += ACMultiplication[i];
                }

                return BDMultiplication;
            }
        }

        /// <summary>
        /// Metoda wyznaczajaca wspolczynniki licznika czesci urojonej
        /// </summary>
        /// <returns>
        /// Wspolczynniki licznika czesci urojonej
        /// </returns>
        private Double[] CalculateImgNomFrequencyFactors()
        {
            //Pogrupowanie w oddzielne kolekcje wspolczynnikow rzeczywistych i urojonych licznika
            Double[] nomRealFactor = new Double[nomFactors.Length];
            Double[] nomImgFactor = new Double[nomFactors.Length];

            for (int i = 0; i < nomFactors.Length; i++)
            {
                if (i % 2 == 0)
                {
                    nomRealFactor[i] = Complex.Pow(Complex.ImaginaryOne, i).Real * nomFactors[i];
                }
                else
                {
                    nomImgFactor[i] = Complex.Pow(Complex.ImaginaryOne, i - 1).Real * nomFactors[i];
                }
            }

            //Pogrupowanie w oddzielne kolekcje wspolczynnikow rzeczywistych i urojonych miankownika
            Double[] denRealFactors = new Double[denFactors.Length];
            Double[] denImgFactors = new Double[denFactors.Length];

            for (int i = 0; i < denFactors.Length; i++)
            {
                if (i % 2 == 0)
                {
                    denRealFactors[i] = Complex.Pow(Complex.ImaginaryOne, i).Real * denFactors[i];
                }
                else
                {
                    denImgFactors[i] = Complex.Pow(Complex.ImaginaryOne, i - 1).Real * denFactors[i];
                }
            }

            //[A] + [B]j    ([A][C] + [B][D]) + j([B][C]-[A][D])
            //---------- = ---------------------------------------
            //[C] + [D]j                [C][C] + [D][D]

            //Stworzenie wektorow BC i AD
            Double[] BCMultiplication = TransferFunctionCalculator.CrossFactors(nomImgFactor, denRealFactors);
            Double[] ADMultiplication = TransferFunctionCalculator.CrossFactors(denImgFactors, nomRealFactor);

            for (int i = 0; i < ADMultiplication.Length; i++)
            {
                ADMultiplication[i] *= -1;
            }

            //Zsumowanie wektorow ( nalezy sprawdzic ktory jest dluzszy zeby poprawnie je dodac )
            if (BCMultiplication.Length > ADMultiplication.Length)
            {
                for (int i = 0; i < ADMultiplication.Length; i++)
                {
                    BCMultiplication[i] += ADMultiplication[i];
                }

                return BCMultiplication;
            }
            else
            {
                for (int i = 0; i < BCMultiplication.Length; i++)
                {
                    ADMultiplication[i] += BCMultiplication[i];
                }

                return ADMultiplication;
            }
        }

        /// <summary>
        /// Metoda wyznaczajaca wspolczynniki mianownika czesci rzeczywistej i urojonej
        /// </summary>
        /// <returns>
        /// Wspolczynniki mianownika czesci rzeczywistej i urojonej
        /// </returns>
        private Double[] CalculateDenFrequencyFactors()
        {
            //Pogrupowanie w oddzielne kolekcje wspolczynnikow rzeczywistych i urojonych licznika
            Double[] nomRealFactor = new Double[denFactors.Length];
            Double[] nomImgFactor = new Double[denFactors.Length];

            for (int i = 0; i < denFactors.Length; i++)
            {
                if (i % 2 == 0)
                {
                    nomRealFactor[i] = Complex.Pow(Complex.ImaginaryOne, i).Real * denFactors[i];
                }
                else
                {
                    nomImgFactor[i] = Complex.Pow(Complex.ImaginaryOne, i - 1).Real * denFactors[i];
                }
            }

            //[A] + [B]j    ([A][C] + [B][D]) + j([B][C]-[A][D])
            //---------- = ---------------------------------------
            //[C] + [D]j                [C][C] + [D][D]

            //Stworzenie wektorow CC i DD
            Double[] CCMultiplication = TransferFunctionCalculator.CrossFactors(nomRealFactor, nomRealFactor);
            Double[] DDMultiplication = TransferFunctionCalculator.CrossFactors(nomImgFactor, nomImgFactor);
            
            //Zsumowanie wektorow ( nalezy sprawdzic ktory jest dluzszy zeby poprawnie je dodac )
            if (CCMultiplication.Length > DDMultiplication.Length)
            {
                for (int i = 0; i < DDMultiplication.Length; i++)
                {
                    CCMultiplication[i] += DDMultiplication[i];
                }

                return CCMultiplication;
            }
            else
            {
                for (int i = 0; i < CCMultiplication.Length; i++)
                {
                    DDMultiplication[i] += CCMultiplication[i];
                }

                return DDMultiplication;
            }
        }

        /// <summary>
        /// Metoda wyznaczajaca wspolczynniki licznika wielomianu okreslejacego warunek na miejsce zerowe zapasu fazy
        /// </summary>
        /// <returns>
        /// Wspolczynniki licznika wielomianu okreslejacego warunek na miejsce zerowe zapasu fazy
        /// </returns>
        private Double[] CalculatePhaseMarginFactors()
        {
            //Sqrt(Rez^2 + Imz^2) = 1 => Rez^2 + Imz^2 = 1:
            //          
            //     [RN]^2                     [IN]^2                [D]^2         [RN]^2 + [IN]^2 - [D]^2
            //--------------    +      ------------------    -    -------- =  -------------------------------
            //     [D]^2                      [D]^2                 [D]^2                  [D]^2

            //Wyznaczenie podniesionych do kwadratu powyzszych wektorow wspolczynnikow
            Double[] RN2Factors = TransferFunctionCalculator.CrossFactors(realNomFrequencyFactors, realNomFrequencyFactors);

            Double[] IN2Factors = TransferFunctionCalculator.CrossFactors(imgNomFrequencyFactors, imgNomFrequencyFactors);

            Double[] denCrossedFactors = TransferFunctionCalculator.CrossFactors(denFrequencyFactors, denFrequencyFactors);

            //Wyznaczenie ktory z nich jest najdlozszy
            List<int> lengths = new List<int>();

            lengths.Add(RN2Factors.Length);
            lengths.Add(IN2Factors.Length);
            lengths.Add(denCrossedFactors.Length);

            int length = lengths.Max();

            //Wektor wyjsciowych wspolczynnikow bedzie dlugosci najdluzszego z nich
            Double[] factors = new Double[length];

            for (int i = 0; i < factors.Length; i++)
            {
                factors[i] = 0.0;

                //Sprawdzenie czy dlugosc kolejnych tablic wspolczynnikow pozwala na ich dodanie do wyliczanego wspolczynnika
                if (i < RN2Factors.Length)
                {
                    factors[i] += RN2Factors[i];
                }

                if (i < IN2Factors.Length)
                {
                    factors[i] += IN2Factors[i];
                }

                if (i < denCrossedFactors.Length)
                {
                    factors[i] -= denCrossedFactors[i];
                }
            }

            return factors;
        }

        /// <summary>
        /// Wyznaczanie zakresow w ktorych atan okreslajacy fazę jest cyklicznie zmieniany - punktow w ktorych img=0 i rez<=0
        /// </summary>
        /// <returns>
        /// Zakresy w ktorych atan okreslajacy fazę jest cyklicznie zmieniany:
        /// zakres[0,x] - poczatek zakresu
        /// zakres[1,x] - koniec zakresu
        /// zakres[2,x] - wartosc ktora reprezentuje dany przedzial - wykorzystywana do wyznaczania wartosci fazy
        /// </returns>
        private Double[,] CalculatePhaseRanges()
        {
            //Zakresow bedzie zawsze o 1 wiecej
            Double[,] ranges = new Double[3, GainMargins.Length + 1];
            
            //Jezeli nie ma zadnych zapasow modulu - jest tylko jeden duzy przedzial - od minimalnej do maksymalnej wartosci jaka moze posiadac double
            if (GainMargins.Length == 0)
            {
                ranges[0, 0] = Double.MinValue;
                ranges[1, 0] = Double.MaxValue;
                ranges[2, 0] = 0;

                return ranges;
            }

            //Jezeli elementow jest wiecej tworzone jest n+1 zakresow
            for (int i = 0; i < ranges.GetLength(1); i++)
            {
                //Pierwszy w przedzialow wyliczany jest od minimalnej wartosci do pierwszego miejsca zapasu modulu
                if (i == 0)
                {
                    ranges[0, 0] = Double.MinValue;
                    ranges[1, 0] = GainMargins[0].Frequency;
                    ranges[2, 0] = 0;

                    continue;
                }

                //Ostatni przedzial jest od zapasu modulu dla najwyzszej czestotliwosci do maksymalnej liczby Double
                if (i == ranges.GetLength(1) - 1)
                {

                    ranges[0, i] = GainMargins[i - 1].Frequency;
                    ranges[1, i] = Double.MaxValue;

                    //Wartosc ktora ma byc wykorzystywana w procesie wyznaczania zapasu fazy jest:
                    //Wieksza niz poprzednia jezeli zapas modulu przechodzi z imz- do imz+
                    //Ujemna jezeli zapas modulu przechodzi z imz+ do imz-
                    if (GainMargins[i - 1].Direction)
                    {
                        ranges[2, i] = ranges[2, i - 1] - 1;
                    }
                    else
                    {
                        ranges[2, i] = ranges[2, i - 1] + 1;
                    }
                    continue;
                }

                //Kazdy inny przedzial jest od czestotliwosci poprzedniego zapasu modulu do czestotliwosci aktualnego
                ranges[0, i] = ranges[1, i - 1];
                ranges[1, i] = GainMargins[i].Frequency;

                if (GainMargins[i - 1].Direction)
                {
                    ranges[2, i] = ranges[2, i - 1] - 1;
                }
                else
                {
                    ranges[2, i] = ranges[2, i - 1] + 1;
                }
            }

            return ranges;
        }

        /// <summary>
        /// Metoda wyznaczajaca automatyczny zakres wyswietlania czestotliwosci:
        /// od 10 razy wiekszej czestotliwosci niz najwieksza czestotliwosc zapasu modulu lub fazy
        /// do 10 razy mniejszej czestotlwiosc niz najmniejsza czestotliwosc zapasu modulu lub fazy
        /// Jezeli nie ma zapasu fazy i modulu - czestotlwiosci 1E-8 do 1E8
        /// </summary>
        /// <returns>
        /// Zakres czestotlwiosci do wyswietlania
        /// </returns>
        private Double[] CalculateAutomaticRange()
        {
            //Zakres jest przechowywany w tablicy dwoch elementow
            Double[] range = new Double[2];

            //Jezeli nie ma zadnych zapasow fazy lub stabilnosci - automatyczne wyswietlanie jest ustawione na 1E-8 do 1E8
            if (ImgZeroPoints.Length == 0 && PhaseMargins.Length == 0)
            {
                range[0] = 1E-8;
                range[1] = 1E8;

                return range;
            }


            //Wyznaczenie najwiekszych i najmniejszych wartosci dla zapasow modulu
            Double maxGainFrequency = Double.MinValue;
            Double minGainFrequency = Double.MaxValue;

            if (ImgZeroPoints.Length != 0)
            {
                maxGainFrequency = (from margin in ImgZeroPoints
                                    select margin.Frequency).Max();

                minGainFrequency = (from margin in ImgZeroPoints
                                    select margin.Frequency).Min();
            }

            //Wyznaczenie najwiekszych i najmniejszych wartosci dla zapasow fazy
            Double maxPhaseFrequency = Double.MinValue;
            Double minPhaseFrequency = Double.MaxValue;

            if (PhaseMargins.Length != 0)
            {
                maxPhaseFrequency = (from margin in PhaseMargins
                                     select margin.Frequency).Max();

                minPhaseFrequency = (from margin in PhaseMargins
                                     select margin.Frequency).Min();
            }

            //Wyznaczenie mniejszej z dwoch minmalnych czestotliwosci
            Double minRange = minGainFrequency < minPhaseFrequency ? minGainFrequency : minPhaseFrequency;

            //Wyznaczanie wiekszej z dwoch maksylanych czestotliwosci
            Double maxRange = maxGainFrequency > maxPhaseFrequency ? maxGainFrequency : maxPhaseFrequency;

            //Jezeli minimalna czestotliwosc to 0 - nalezy ja zmienic na wartosc niezerowa
            if (minRange == 0)
            {
                minRange = 1E-7;
            }

            //Jezeli maksymalna czestotliwosc to 0 - nalezy ja zmienic na wartosc niezerowa
            if (maxRange == 0)
            {
                maxRange = 1E7;
            }

            range[0] = minRange / 10;
            range[1] = maxRange * 10;

            return range;
        }

        /// <summary>
        /// Wyznaczenie wartosci fazy
        /// </summary>
        /// <param name="value">
        /// Wartosc fazy dla punktu
        /// </param>
        /// <param name="frequency">
        /// Czestotliwosc
        /// </param>
        /// <returns>
        /// Wartosc fazy
        /// </returns>
        private Double CalculatePhase(Complex value, Double frequency)
        {
            //Pobranie wartosci reprezentujacej przedzial czestotliwosci
            Double valueRange = ValueForPhaseRange(frequency);

            //Przeliczenie Wartosci fazy punktu na rzeczywista wartosc fazy
            if (valueRange == 0)
            {
                return ConvertRadiusToDegree(value.Phase);
            }
            else if (valueRange < 0)
            {
                return ConvertRadiusToDegree((2 * valueRange + 1) * Math.PI + value.Phase - Math.PI);
            }
            else
            {
                return ConvertRadiusToDegree((2 * valueRange + 1) * Math.PI + value.Phase - Math.PI);
            }
        }

        /// <summary>
        /// Metoda pobierajaca punkty w ktorych czesc urojona rowna jest 0
        /// </summary>
        /// <returns>
        /// Punkty w ktorych czesc urojona jest rowna 0
        /// </returns>
        private ZeroPoint[] GetImgZeroPoints()
        {
            //Sprawdzenie czy wszystkie wspolczynniki wykorzystywane do liczenia nie sa zerami
            int numberOfEmptyFactors = (from factor in imgNomFrequencyFactors
                                        where factor == 0
                                        select factor).Count();

            if (numberOfEmptyFactors == imgNomFrequencyFactors.Length)
            {
                return new ZeroPoint[0];
            }

            //Pobranie miejsc zerowych licznika czesci urojonej
            RootFinder rootFinder = new RootFinder(imgNomFrequencyFactors);
            Double[] omegaRoots = (from root in rootFinder.GetRealRoots()
                                   select root).Distinct().ToArray();

            List<ZeroPoint> zeros = new List<ZeroPoint>();

            //Dla kazdego znalezionego miejsca zerowego stworzenie obiektu zapasu fazy
            for (int i = 0; i < omegaRoots.Length; i++)
            {
                if (omegaRoots[i] > 0)
                {
                    //Jezeli rowniez mianownik jest rowny 0 - taki punkt nie brany jest pod uwage
                    if (Math.Abs(TransferFunctionCalculator.Horner(omegaRoots[i], denFrequencyFactors)) == 0)
                    {
                        continue;
                    }
                    //Pobranie roznicy miedzy punktem troche nad miejscem zerowym i pod
                    Double delta = (TransferFunctionCalculator.Horner(omegaRoots[i] + rootFinder.Accuracy, imgNomFrequencyFactors) / TransferFunctionCalculator.Horner(omegaRoots[i] + rootFinder.Accuracy, denFrequencyFactors)) - (TransferFunctionCalculator.Horner(omegaRoots[i] - rootFinder.Accuracy, imgNomFrequencyFactors) / TransferFunctionCalculator.Horner(omegaRoots[i] - rootFinder.Accuracy, denFrequencyFactors));

                    //Stworzenie punktu miejsca zerowego
                    Complex marginPoint = new Complex(TransferFunctionCalculator.Horner(omegaRoots[i], realNomFrequencyFactors) / TransferFunctionCalculator.Horner(omegaRoots[i], denFrequencyFactors), TransferFunctionCalculator.Horner(omegaRoots[i], imgNomFrequencyFactors) / TransferFunctionCalculator.Horner(omegaRoots[i], denFrequencyFactors));

                    if (zeros.FindAll((a) => (Math.Abs(a.Frequency - omegaRoots[i]) < rootFinder.Accuracy)).Count == 0)
                    {
                        zeros.Add(new ZeroPoint(marginPoint, omegaRoots[i], (delta > 0)));
                    }
                }
            }

            //Zwrocenie kolekcji wyznaczonych punktow posortowanych wzgledem rosnacej czestotliwosci
            return (from zero in zeros
                    orderby zero.Frequency ascending
                    select zero).ToArray();
        }

        /// <summary>
        /// Metoda pobierajca zapasy modulu
        /// </summary>
        /// <returns>
        /// kolekcja zapasow modulu
        /// </returns>
        private Margin[] GetGainMargins()
        {
            //Pobranie punktow ktore ktore maja miejsca mniejsza od zera czesc rzeczywista
            ZeroPoint[] marginPoints = (from point in ImgZeroPoints
                                        where point.Point.Real <= 0
                                        select point).ToArray();
            
            //Stworzenie z tych elementow zapasow modulu
            Margin[] margins = new Margin[marginPoints.Length];

            for (int i = 0; i < marginPoints.Length; i++)
            {
                margins[i] = new Margin(marginPoints[i].Point, marginPoints[i].Frequency, marginPoints[i].Direction, 20 * Math.Log10(1 / marginPoints[i].Point.Magnitude), -20 * Math.Log10(1 / marginPoints[i].Point.Magnitude));
            }

            return margins;
        }

        /// <summary>
        /// Pobranie zapasow fazy
        /// </summary>
        /// <returns>
        /// Kolekcja zapasow fazy
        /// </returns>
        private Margin[] GetPhaseMargins()
        {
            //Sprawdzenie czy wszystkie wspolczynniki wykorzystywane do liczenia nie sa zerami
            int numberOfEmptyFactors = (from factor in phaseMarginFactors
                                        where factor == 0
                                        select factor).Count();

            if(numberOfEmptyFactors == phaseMarginFactors.Length)
            {
                return new Margin[0];
            }

            //Pobranie miejsc zerowych funkcji zapasu fazy
            RootFinder phaseRoots = new RootFinder(phaseMarginFactors);
            //W przypadku kilkukrotnych miejsc zerowych - pobieramy tylko te ktore sie nie powtarzaja
            Double[] omegaRoots = (from root in phaseRoots.GetRealRoots()
                                      select root).Distinct().ToArray();
            
            //Stworzenie punktow 
            List<Margin> margins = new List<Margin>();

            for (int i = 0; i < omegaRoots.Length; i++)
            {
                if (omegaRoots[i] > 0)
                {
                    if (TransferFunctionCalculator.Horner(omegaRoots[i], denFrequencyFactors) == 0)
                    {
                        continue;
                    }

                    Complex point = new Complex(TransferFunctionCalculator.Horner(omegaRoots[i], realNomFrequencyFactors) / TransferFunctionCalculator.Horner(omegaRoots[i], denFrequencyFactors), TransferFunctionCalculator.Horner(omegaRoots[i], imgNomFrequencyFactors) / TransferFunctionCalculator.Horner(omegaRoots[i], denFrequencyFactors));
                    Double delta = Complex.Abs(new Complex(TransferFunctionCalculator.Horner(omegaRoots[i] + phaseRoots.Accuracy, realNomFrequencyFactors) / TransferFunctionCalculator.Horner(omegaRoots[i] + phaseRoots.Accuracy, denFrequencyFactors), TransferFunctionCalculator.Horner(omegaRoots[i] + phaseRoots.Accuracy, imgNomFrequencyFactors) / TransferFunctionCalculator.Horner(omegaRoots[i] + phaseRoots.Accuracy, denFrequencyFactors))) - Complex.Abs(new Complex(TransferFunctionCalculator.Horner(omegaRoots[i] - phaseRoots.Accuracy, realNomFrequencyFactors) / TransferFunctionCalculator.Horner(omegaRoots[i] - phaseRoots.Accuracy, denFrequencyFactors), TransferFunctionCalculator.Horner(omegaRoots[i] - phaseRoots.Accuracy, imgNomFrequencyFactors) / TransferFunctionCalculator.Horner(omegaRoots[i] - phaseRoots.Accuracy, denFrequencyFactors)));

                    if (margins.FindAll((a) => (Math.Abs(a.Frequency - omegaRoots[i]) < phaseRoots.Accuracy)).Count == 0)
                    {
                        margins.Add(new Margin(point, omegaRoots[i], (delta > 0), CalculatePhaseMargin(point.Phase), CalculatePhase(point, omegaRoots[i])));
                    }

                }
            }

            //Posortowanie punktow wzgledem czestotliwosci
            return  (from margin in margins
                    orderby margin.Frequency ascending
                    select margin).ToArray();
        }

        /// <summary>
        /// Metoda zwracajaca wartosc wielomianu dla zerowej czestotliwosci
        /// </summary>
        /// <param name="denominator">
        /// Wielomian mianownika
        /// </param>
        /// <param name="nominator">
        /// Wielomian licznika
        /// </param>
        /// <returns>
        /// Wartosc wielomianu dla zerowej czestotliwosci
        /// </returns>
        private static Double GetValueForOmegaZero(Double[] denominator, Double[] nominator)
        {
            //Pobranie rzedu licznika
            int length = nominator.Length;

            for(int i=0; i<length; i++)
            {
                //Jezeli pierwszy element licznika jest rozny od 0 - latwo obliczyc wartosc dla omega = 0 - jest to iloraz pierwszego wspolczynnika liczniak i mianownika
                if(denominator[i]!=0)
                {
                    return nominator[i] / denominator[i];
                }
                else
                {
                    //Jezeli pierwszy element licznika jest rozny od 0 a mianownika rowny 0 - wartosc bedzie nieskonczona lub -nieskonczona
                    if (nominator[i] != 0)
                    {
                        if (nominator[i] < 0)
                        {
                            return Double.NegativeInfinity;
                        }
                        else
                        {
                            return Double.PositiveInfinity;
                        }
                    }
                }
            }

            //W reszcie przypadkow ktore nie powinny wystapic - blad
            return Double.NaN;
        }

        /// <summary>
        /// Metoda zwracajaca wartosc wielomianu dla nieskonczonej czestotliwosci
        /// </summary>
        /// <param name="denominator">
        /// Wielomian mianownika
        /// </param>
        /// <param name="nominator">
        /// Wielomian licznika
        /// </param>
        /// <returns>
        /// Wartosc wielomianu dla nieskonczonej czestotliwosci
        /// </returns>
        private static Double GetValueForOmegaInf(Double[] denominator, Double[] nominator)
        {
            //Pobranie rzedu licznika
            int length = denominator.Length;

            for (int i = length - 1; i >= 0; i--)
            {
                if (denominator[i] != 0)
                {
                    if (i > nominator.Length - 1)
                    {
                        return 0;
                    }
                    else
                    {
                        if (nominator[i] != 0)
                        {
                            return nominator[i] / denominator[i];
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
                else
                {
                    if (i < nominator.Length - 1)
                    {
                        if (nominator[i] > 0)
                        {
                            return Double.PositiveInfinity;
                        }

                        if (nominator[i] < 0)
                        {
                            return Double.NegativeInfinity;
                        }
                    }
                }
            }

            return Double.NaN;
        }

        /// <summary>
        /// Metoda zwracajaca punkt dla czestotliwosci nieskonczonej
        /// </summary>
        /// <returns>
        /// Punkt dla czestotlwiosci nieskonczonej
        /// </returns>
        private FrequencyPoint GetPointForOmegaInf()
        {
            return new FrequencyPoint(GetValueForOmegaInf(denFrequencyFactors, realNomFrequencyFactors), GetValueForOmegaInf(denFrequencyFactors, imgNomFrequencyFactors), Double.PositiveInfinity);
        }

        /// <summary>
        /// Metoda zwracajaca punkt dla czestotliwosci zerowej
        /// </summary>
        /// <returns>
        /// Punkt dla czestotlwiosci zerowej
        /// </returns>
        private FrequencyPoint GetPointForOmegaZero()
        {
            return new FrequencyPoint(GetValueForOmegaZero(denFrequencyFactors, realNomFrequencyFactors), GetValueForOmegaZero(denFrequencyFactors, imgNomFrequencyFactors), 0);
        }

        /// <summary>
        /// Metoda zwracajaca wartosc dla zakresu fazy 
        /// </summary>
        /// <param name="frequency">
        /// Czestotlwiosc
        /// </param>
        /// <returns>
        /// Wartosc do wyliczenia fazy
        /// </returns>
        private Double ValueForPhaseRange(Double frequency)
        {
            for (int i = 0; i < PhaseRanges.Length; i++)
            {
                if (frequency >= PhaseRanges[0, i] && frequency <= PhaseRanges[1, i])
                {
                    return PhaseRanges[2, i];
                }
            }

            throw new InvalidProgramException("Frequency outside all ranges");
        }

        /// <summary>
        /// Wyznaczenie punktow charakterystyki Nyquista
        /// </summary>
        /// <returns>
        /// Punkty charakterystyki Nyquista
        /// </returns>
        public FrequencyPoint[] GetNyquistPlot()
        {
            return GetNyquistPlot(AutomaticDisplayRanges[0], AutomaticDisplayRanges[1], 500);
        }

        /// <summary>
        /// Wyznaczenie punktow charakterystyki Nyquista
        /// </summary>
        /// <param name="omegaStart">
        /// Czestotliwosc poczatkowa
        /// </param>
        /// <param name="omegaStop">
        /// Czestotliwosc koncowa
        /// </param>
        /// <param name="numberOfPoints">
        /// Liczba punktow
        /// </param>
        /// <returns>
        /// Punkty charakterystyki Nyquista
        /// </returns>
        public FrequencyPoint[] GetNyquistPlot(Double omegaStart, Double omegaStop, Int64 numberOfPoints = 500)
        {
            //punktow jest tyle ile zmienna numberOfPoints
            FrequencyPoint[] nyquistPlot = new FrequencyPoint[numberOfPoints];

            //Wyznaczenie zakresu poczatkowego i koncowego - wyznaczane punkty sa w sposob logarytmiczny
            Double start = Math.Log10(omegaStart);
            Double stop = Math.Log10(omegaStop);

            Double resolution = (stop - start) / numberOfPoints;

            Double omega = 0;
            
            //Wyznaczanie wartosci charakterystyki nyquista
            for (Int64 i = 0; i < numberOfPoints; i++)
            {
                omega = Math.Pow(10, start + i * resolution);

                Complex value = new Complex(TransferFunctionCalculator.Horner(omega, realNomFrequencyFactors) / TransferFunctionCalculator.Horner(omega, denFrequencyFactors), TransferFunctionCalculator.Horner(omega, imgNomFrequencyFactors) / TransferFunctionCalculator.Horner(omega, denFrequencyFactors));

                FrequencyPoint point = new FrequencyPoint(value, omega);

                nyquistPlot[i] = point;
            }

            return nyquistPlot;
        }

        /// <summary>
        /// Wyznaczenie punktow charakterystyki Nyquista
        /// </summary>
        /// <param name="observableCollection">
        /// Kolekcja punktow charakterystyki w postaci komaptybilnej z mechanizmem wiazania danych WPF
        /// </param>
        /// <param name="omegaStart">
        /// Czestotliwosc poczatkowa
        /// </param>
        /// <param name="omegaStop">
        /// Czestotliwosc koncowa
        /// </param>
        /// <param name="numberOfPoints">
        /// Liczba punktow
        /// </param>
        public void GetNyquistPlotObservable(FrequencyPointsObservableCollection observableCollection, Double omegaStart, Double omegaStop, Int64 numberOfPoints = 2000)
        {
            //Wyczyszczenie kolekcji punktow kompatybilnej z WPF
            observableCollection.FrequencyPoints.Clear();

            //Wyznaczenie zakresu poczatkowego i koncowego - wyznaczane punkty sa w sposob logarytmiczny
            Double start = Math.Log10(omegaStart);
            Double stop = Math.Log10(omegaStop);

            Double resolution = (stop - start) / numberOfPoints;

            Double omega = 0;

            //Wyznaczanie wartosci charakterystyki nyquista
            for (Int64 i = 0; i < numberOfPoints; i++)
            {
                omega = Math.Pow(10, start + i * resolution);

                Complex value = new Complex(TransferFunctionCalculator.Horner(omega, realNomFrequencyFactors) / TransferFunctionCalculator.Horner(omega, denFrequencyFactors), TransferFunctionCalculator.Horner(omega, imgNomFrequencyFactors) / TransferFunctionCalculator.Horner(omega, denFrequencyFactors));

                //Dodanie nowej wartosci do kolekcji punktow kompatybilnych z WPF
                observableCollection.FrequencyPoints.Add(new DataPoint(value.Real,value.Imaginary));
            }

        }

        /// <summary>
        /// Wyznaczenie punktow charakterystyki Nyquista
        /// </summary>
        /// <param name="observableCollection">
        /// Kolekcja punktow charakterystyki w postaci komaptybilnej z mechanizmem wiazania danych WPF
        /// </param>
        public void GetNyquistPlotObservable(FrequencyPointsObservableCollection observableCollection)
        {
            GetNyquistPlotObservable(observableCollection,AutomaticDisplayRanges[0], AutomaticDisplayRanges[1], 1000);
        }

        /// <summary>
        /// Wyznaczenie punktow charakterystyki Nyquista
        /// </summary>
        /// <param name="omegaStart">
        /// Czestotliwosc poczatkowa
        /// </param>
        /// <param name="omegaStop">
        /// Czestotliwosc koncowa
        /// </param>
        /// <param name="numberOfPoints">
        /// Liczba punktow
        /// </param>
        /// <returns>
        /// Punkty charakterystyki Nyquista
        /// </returns>
        public BodePoint[] GetBodePlot()
        {
            return GetBodePlot(AutomaticDisplayRanges[0], AutomaticDisplayRanges[1], 5000);
        }

        /// <summary>
        /// Wyznaczenie punktow charakterystyki Bodego
        /// </summary>
        /// <param name="omegaStart">
        /// Czestotliwosc poczatkowa
        /// </param>
        /// <param name="omegaStop">
        /// Czestotliwosc koncowa
        /// </param>
        /// <param name="numberOfPoints">
        /// Liczba punktow
        /// </param>
        /// <returns>
        /// Punkty charakterystyki Bodego
        /// </returns>
        public BodePoint[] GetBodePlot(Double omegaStart, Double omegaStop, Int64 numberOfPoints = 5000)
        {
            //punktow jest tyle ile zmienna numberOfPoints
            BodePoint[] nyquistPlot = new BodePoint[numberOfPoints];

            //Wyznaczenie zakresu poczatkowego i koncowego - wyznaczane punkty sa w sposob logarytmiczny
            Double start = Math.Log10(omegaStart);
            Double stop = Math.Log10(omegaStop);

            Double resolution = (stop - start) / numberOfPoints;

            Double omega = 0;

            //Wyznaczanie wartosci charakterystyki bodego
            for (Int64 i = 0; i < numberOfPoints; i++)
            {
                omega = Math.Pow(10, start + i * resolution);

                Complex value = new Complex(TransferFunctionCalculator.Horner(omega, realNomFrequencyFactors) / TransferFunctionCalculator.Horner(omega, denFrequencyFactors), TransferFunctionCalculator.Horner(omega, imgNomFrequencyFactors) / TransferFunctionCalculator.Horner(omega, denFrequencyFactors));

                BodePoint point = new BodePoint(value, omega, -20 * Math.Log10(1 / value.Magnitude), CalculatePhase(value, omega));

                nyquistPlot[i] = point;
            }

            return nyquistPlot;
        }

        /// <summary>
        /// Wyznaczenie punktow charakterystyki Bodego
        /// </summary>
        /// <param name="observableCollection">
        /// Kolekcja punktow charakterystyki w postaci komaptybilnej z mechanizmem wiazania danych WPF
        /// </param>
        /// <param name="omegaStart">
        /// Czestotliwosc poczatkowa
        /// </param>
        /// <param name="omegaStop">
        /// Czestotliwosc koncowa
        /// </param>
        /// <param name="numberOfPoints">
        /// Liczba punktow
        /// </param>
        public void GetBodePlotObservable(BodePointsDataPoints observableCollection, Double omegaStart, Double omegaStop, Int64 numberOfPoints = 5000)
        {
            //Wyzerowanie punktow wzmocnienia i fazy
            observableCollection.MarginBodePoints.Clear();
            observableCollection.PhaseBodePoints.Clear();

            //Wyznaczenie zakresu poczatkowego i koncowego - wyznaczane punkty sa w sposob logarytmiczny
            Double start = Math.Log10(omegaStart);
            Double stop = Math.Log10(omegaStop);

            Double resolution = (stop - start) / numberOfPoints;

            Double omega = 0;

            //Wyznaczanie wartosci charakterystyki nyquista
            for (Int64 i = 0; i < numberOfPoints; i++)
            {
                omega = Math.Pow(10, start + i * resolution);

                Complex value = new Complex(TransferFunctionCalculator.Horner(omega, realNomFrequencyFactors) / TransferFunctionCalculator.Horner(omega, denFrequencyFactors), TransferFunctionCalculator.Horner(omega, imgNomFrequencyFactors) / TransferFunctionCalculator.Horner(omega, denFrequencyFactors));

                //Dodanie dwoch nowych punktow wzmocnienia i fazy - punktow bodego
                observableCollection.MarginBodePoints.Add(new DataPoint(omega, -20 * Math.Log10(1 / value.Magnitude)));
                observableCollection.PhaseBodePoints.Add(new DataPoint(omega, CalculatePhase(value, omega)));
            }

        }

        /// <summary>
        /// Wyznaczenie punktow charakterystyki Bodego
        /// </summary>
        /// <param name="observableCollection">
        /// Kolekcja punktow charakterystyki w postaci komaptybilnej z mechanizmem wiazania danych WPF
        /// </param>
        public void GetBodePlotObservable(BodePointsDataPoints observableCollection)
        {
            GetBodePlotObservable(observableCollection, AutomaticDisplayRanges[0], AutomaticDisplayRanges[1], 5000);
        }

        /// <summary>
        /// Metoda wyliczajaca zapas fazy na podstawie wartosci kata - kat musi byc od -pi do pi
        /// </summary>
        /// <param name="phase">
        /// kąt - musi byc od -pi do pi
        /// </param>
        /// <returns>
        /// zapas fazy ( w stopniach)
        /// </returns>
        private Double CalculatePhaseMargin(Double phase)
        {
            if(phase < 0)
            {
                return ConvertRadiusToDegree(Math.PI + phase);
            }
            else
            {
                return ConvertRadiusToDegree(-Math.PI + phase);
            }
        }

        /// <summary>
        /// Metoda dokonujaca konwersji
        /// </summary>
        /// <param name="radiusAngle">
        /// Kąt w radianach
        /// </param>
        /// <returns>
        /// Kąt w stopniach
        /// </returns>
        private Double ConvertRadiusToDegree(Double radiusAngle)
        {
            return radiusAngle*180/Math.PI;
        }

    }
}
