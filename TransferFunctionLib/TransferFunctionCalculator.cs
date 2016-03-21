using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using System.Diagnostics;

namespace TransferFunctionLib
{
    /// <summary>
    /// Typ wyliczeniowy reprezentujacy metode konwersji transmitancji
    /// </summary>
    public enum TransferFunctionConversionType
    {
        Goodwin,Tustin
    }

    /// <summary>
    /// Klasa reprezentujaca kalkulator transmitancji
    /// </summary>
    internal class TransferFunctionCalculator
    {
        /// <summary>
        /// tablica przechowujaca wspolczynniki dwumianow newtona - aby nie trzeba bylo ich kilka razy obliczac
        /// 
        /// | ( 0 / 0 ) ... ( 0 / i ) ... ( 0 / n ) |
        /// |    ...    ...    ...    ... (  ...    |
        /// | ( j / 0 ) ... ( j / i ) ... ( j / n ) |
        /// |    ...    ...    ...    ...    ...    |
        /// | ( n / 0 ) ... ( n / i ) ... ( n / n ) |
        /// 
        /// </summary>
        protected Double[,] newtonArray;

        /// <summary>
        /// Metoda tworzaca tablice dwumianow Newtona
        /// </summary>
        /// <param name="n">
        /// Maksymalny rzad dla ktorego ma zostac wyznaczona tablica dwumianow
        /// </param>
        private void CreateNewtonArray(int n)
        {
            newtonArray = new Double[n + 1, n + 1];

            //Wykorzystanie wzoru:
            //
            //  | n |   | n |    n - k
            //  |   | = |   | * -------
            //  |k+1|   | k |    k + 1

            for (int i = 0; i <= n; i++)
            {
                for (int j = i; j <= n; j++)
                {
                    if (i == 0)
                    {
                        newtonArray[i, j] = 1;
                    }
                    else
                    {
                        newtonArray[i, j] = newtonArray[i - 1, j] * (Convert.ToDouble(j - i + 1) / Convert.ToDouble(i));
                    }
                }
            }
        }

        /// <summary>
        /// Metoda wyznaczajaca tabele alfa => (1 - z^-1) ^ alfa
        /// </summary>
        /// <param name="alpha">
        /// Wspolczynnik do ktorego podnoszony jest (1 - z^-1)
        /// </param>
        /// <returns>
        /// Tabela wartosci wspolczynnikow:
        /// [ a0 a1 a2 ... ]
        /// gdzie a0 przypada dla potegi z^0 a1 z^1 itd...
        /// </returns>
        private Double[] CalculateAlphaTable(int alpha)
        {
            Double[] alphaArray = new Double[alpha+1];

            //Jezeli potega jest parzysta - wspolczynnik jest dodatni
            //W przeciwnym przypadku ujemny
            for(int i=0; i<=alpha; i++)
            {
                if(i%2==1)
                {
                    //newtonArray pobiera wartosc symbolu Newtona => newtonArray[k,n]
                    alphaArray[i] = -newtonArray[i, alpha];
                }
                else
                {
                    alphaArray[i] = newtonArray[i, alpha];
                }
            }

            return alphaArray;
        }

        /// <summary>
        /// Metoda wyznaczajaca tabele beta => (1 + z^-1) ^ beta
        /// </summary>
        /// <param name="beta">
        /// Wspolczynnik do ktorego podnoszony jest (1 + z^-1)
        /// </param>
        /// <returns>
        /// Tabela wartosci wspolczynnikow:
        /// [ a0 a1 a2 ... ]
        /// gdzie a0 przypada dla potegi z^0 a1 z^1 itd...
        /// </returns>
        private Double[] CalculateBetaTable(int beta)
        {
            Double[] betaArray = new Double[beta +1];

            for (int i = 0; i <= beta; i++)
            {
                //newtonArray pobiera wartosc symbolu Newtona => newtonArray[k,n]
                betaArray[i] = newtonArray[i, beta];
            }

            return betaArray;
        }

        /// <summary>
        /// Metoda wyznaczajaca wspolczynniki pomnozonych tablic alfa i beta gdzie:
        /// alfa to tablica wspoczynnikow (1 - z^-1) ^ alfa
        /// alfa to tablica wspoczynnikow (1 + z^-1) ^ beta
        /// </summary>
        /// <param name="alpha">
        /// Wspolczynnik (1 - z^-1)
        /// </param>
        /// <param name="beta">
        /// Wspolczynnik (1 + z^-1)
        /// </param>
        /// <returns>
        /// Tabela wartosci wspolczynnikow:
        /// [ a0 a1 a2 ... ]
        /// gdzie a0 przypada dla potegi z^0 a1 z^1 itd...
        /// </returns>
        private Double[] CalculateBinomialTheoremMultiplication(int alpha, int beta)
        {
            //laczna dlugosc tablicy jest rowna sumie wspolczynnikow
            int n = alpha + beta;

            //Tworze tablice symboli Newtona
            CreateNewtonArray(n);

            Double[] factorArray = new Double[n + 1];

            Double[] alphaArray = CalculateAlphaTable(alpha);
            Double[] betaArray = CalculateBetaTable(beta);
           
            //To jaka jest potega przy wspolczynniku odpowiada w zaleznosci od sumy indeksow oby tablic alfa i beta
            for( int i=0; i<= alpha; i++)
            {
                for( int j=0; j<= beta; j++)
                {
                    factorArray[i + j] += alphaArray[i] * betaArray[j];
                }
            }

            return factorArray;
        }

        /// <summary>
        /// Wyznaczenie wspolczynnikow dwumianu Newtona
        /// </summary>
        /// <param name="n">
        /// Rzad dwumianu
        /// </param>
        /// <returns>
        /// Tabela wspolczynnikow
        /// </returns>
        private Double[] CalculateBinomialTheorem(int n)
        {
            CreateNewtonArray(n);

            return CalculateAlphaTable(n);
        }

        /// <summary>
        /// Metoda Tustina dyskretyzacji transmitancji ciagłej
        /// </summary>
        /// <param name="continousFactors">
        /// Wspolczynniki transmitancji ciągłej
        /// </param>
        /// <param name="n">
        /// Rzad transmitancji ciaglej
        /// </param>
        /// <param name="sampleTime">
        /// Czas probkowania
        /// </param>
        /// <returns>
        /// Wspolczynniki przekonwertowanej transmitancji
        /// </returns>
        private Double[] CalculateDiscreteFactorsTustin(Double[] continousFactors, int n ,Double sampleTime)
        {
            
            Double[] discreteFactors = new Double[n+1];

            for( int i=0; i<continousFactors.Length; i++)
            {
                //kazdy z czynnikow musi byc przemnozony przez Tp^i
                Double factor = continousFactors[i]*Math.Pow((2/sampleTime),i);

                //wyznaczenie wspolczynnikow mnozenia (1-z^-1)^i * (1-z^-1)^(n-i)
                Double[] factorArray = CalculateBinomialTheoremMultiplication(i,n-i);

                //Wymnozenie kazdego w tych wspolczynnikow przez obliczony wczesniej mnoznik
                for( int j=0; j<=n; j++)
                {
                    discreteFactors[j] += factor * factorArray[j];
                }
            }

            return discreteFactors;
        }

        /// <summary>
        /// Metoda Goodwina dyskretyzacji transmitancji ciagłej
        /// </summary>
        /// <param name="continousFactors">
        /// Wspolczynniki transmitancji ciągłej
        /// </param>
        /// <param name="n">
        /// Rzad transmitancji ciaglej
        /// </param>
        /// <param name="sampleTime">
        /// Czas probkowania</param>
        /// <returns>
        /// Wspolczynniki przekonwertowanej transmitancji
        /// </returns>
        private Double[] CalculateDiscreteFactorsGoodwin(Double[] continousFactors, int n, Double sampleTime)
        {
            Double[] discreteFactors = new Double[n + 1];

            for (int i = 0; i < continousFactors.Length; i++)
            {
                //wyznaczenie wspolczynnikow mnozenia (1-z^-1)^i
                Double[] factorArray = CalculateBinomialTheorem(i);

                //Przemnozenie kazdego z tych wspolczynnikow przez wyspolczynnik transmitancji ciaglej i 1/Tp^i
                for (int j = 0; j < factorArray.Length; j++)
                {
                    discreteFactors[j] += continousFactors[i]*(factorArray[j] * Math.Pow((1 / sampleTime), i));
                }
            }

            return discreteFactors;
        }

        /// <summary>
        /// Przeksztalcenie odwrotne Tustina
        /// </summary>
        /// <param name="discreteFactors">
        /// Wspoczynniki transmitancji dyskretnej
        /// </param>
        /// <param name="n">
        /// Rzad transmitancji dyskretnej
        /// </param>
        /// <param name="sampleTime">
        /// Czas probkowania
        /// </param>
        /// <returns>
        /// Tablica wspolczynnikow transmitancji ciaglej
        /// </returns>
        private Double[] CalculateContinousFactorsTustin(Double[] discreteFactors, int n, Double sampleTime)
        {
            Double[] continousFactors = new Double[n + 1];

            for( int i=0; i<discreteFactors.Length; i++)
            {
                //Wyznaczenie wspolczynnikow wyrazenia (1-s)^(-i)*(1+s)^(n-i)
                Double[] factorArray = CalculateBinomialTheoremMultiplication(i, n - i);

                for (int j = 0; j <= n; j++)
                {
                    //Przemnozenie kazdego ze wspolczynnikow przez wspoczynniki wyrazenia powyzej, 2^(n-i) oraz Tp^j
                    continousFactors[j] += discreteFactors[i] * factorArray[j] * Math.Pow(sampleTime, j) * Math.Pow(2, n - j);
                }
            }

            return continousFactors;
        }

        /// <summary>
        /// Przeksztalcenie odwrotne Goodwina
        /// </summary>
        /// <param name="discreteFactors">
        /// Wspoczynniki transmitancji dyskretnej
        /// </param>
        /// <param name="n">
        /// Rzad transmitancji dyskretnej
        /// </param>
        /// <param name="sampleTime">
        /// Czas probkowania
        /// </param>
        /// <returns>
        /// Tablica wspolczynnikow transmitancji ciaglej
        /// </returns>
        private Double[] CalculateContinousFactorsGoodwin(Double[] discreteFactors, int n, Double sampleTime)
        {
            Double[] continousFactors = new Double[n + 1];

            for (int i = 0; i < discreteFactors.Length; i++)
            {
                //Wyznaczenie wspolczynnikow (1-s)^i
                Double[] factorArray = CalculateBinomialTheorem(i);

                for (int j = 0; j < factorArray.Length; j++)
                {
                    continousFactors[j] += discreteFactors[i] * (factorArray[j] * Math.Pow((sampleTime), j));
                }
            }

            return continousFactors;
        }

        /// <summary>
        /// Metoda przeksztalcajaca transmitancje ciagla na dyskretna
        /// </summary>
        /// <param name="transferFunction">
        /// Transmitancja ciagla
        /// </param>
        /// <param name="sampleTime">
        /// Czas probkowania
        /// </param>
        /// <param name="conversionType">
        /// Typ konwersji (Goodwin, Tustin)
        /// </param>
        /// <returns>
        /// Transmitancja dyskretna
        /// </returns>
        public DiscreteTransferFunction ConvertToDiscreteTransferFunction(ContinousTransferFunction transferFunction, Double sampleTime, TransferFunctionConversionType conversionType = TransferFunctionConversionType.Goodwin)
        {
            //Wyznaczenie rzedu transmitancji
            int n = transferFunction.denFactors.Length-1;

            Double[] numdArray = null;
            Double[] dendArray = null; 

            switch(conversionType)
            {
                    //przeksztalcenie licznika i mianownika metoda Goodwina
                case TransferFunctionConversionType.Goodwin:
                    {
                        numdArray = CalculateDiscreteFactorsGoodwin(transferFunction.nomFactors, n, sampleTime);
                        dendArray = CalculateDiscreteFactorsGoodwin(transferFunction.denFactors, n, sampleTime);
                        break;
                    }

                    //Przeksztalcenie licznika i mianownika metoda Tustina
                case TransferFunctionConversionType.Tustin:
                    {
                        numdArray = CalculateDiscreteFactorsTustin(transferFunction.nomFactors, n, sampleTime);
                        dendArray = CalculateDiscreteFactorsTustin(transferFunction.denFactors, n, sampleTime);
                        break;
                    }
            }

            //Wyznaczenie dyskretnego czasu opoznienia
            int discreteTimeDelay = Convert.ToInt32(Math.Round(transferFunction.timeDelay / sampleTime));

            return new DiscreteTransferFunction(numdArray, dendArray, discreteTimeDelay,sampleTime);
        }

        /// <summary>
        /// Metoda przeksztalcajaca transmitancje dyskretna na ciagla
        /// </summary>
        /// <param name="transferFunction">
        /// Transmitancja dyskretna
        /// </param>
        /// <param name="conversionType">
        /// Typ konwersji (Goodwin, Tustin)
        /// </param>
        /// <returns>
        /// Transmitancja ciagla
        /// </returns>
        public ContinousTransferFunction ConvertToContinuesTransferFunction(DiscreteTransferFunction transferFunction, TransferFunctionConversionType conversionType = TransferFunctionConversionType.Goodwin)
        {
            int n = transferFunction.denFactors.Length-1;

            Double[] numcArray = null;
            Double[] dencArray = null; 

            switch(conversionType)
            {
                case TransferFunctionConversionType.Goodwin:
                    {
                        numcArray = CalculateContinousFactorsGoodwin(transferFunction.nomFactors, n, transferFunction.sampleTime);
                        dencArray = CalculateContinousFactorsGoodwin(transferFunction.denFactors, n, transferFunction.sampleTime);
                        break;
                    }
                case TransferFunctionConversionType.Tustin:
                    {
                        numcArray = CalculateContinousFactorsTustin(transferFunction.nomFactors, n, transferFunction.sampleTime);
                        dencArray = CalculateContinousFactorsTustin(transferFunction.denFactors, n, transferFunction.sampleTime);
                        break;
                    }
            }
            Double continousTimeDelay = Convert.ToDouble(transferFunction.timeDelay) * transferFunction.sampleTime;
            return new ContinousTransferFunction(numcArray,dencArray,continousTimeDelay);
        }

        /// <summary>
        /// Metoda mnozaca przez siebie tablice dwoch wspolczynnikow (kazdy przez kazdy - kolejnosc w tabeli oznacza jego rzad)
        /// </summary>
        /// <param name="firstFactors">
        /// Pierwsza tablica wspolczynnikow
        /// </param>
        /// <param name="secondFactors">
        /// Druga tablica wspolczynnikow
        /// </param>
        /// <returns>
        /// Zwrocona tablica przemnozonych wspolczynnikow
        /// </returns>
        public static Double[] CrossFactors(Double[] firstFactors, Double[] secondFactors)
        {
            Double[] crossedFactors = new Double[firstFactors.Length + secondFactors.Length];

            for(int i=0; i<firstFactors.Length; i++)
            {
                for (int j = 0; j < secondFactors.Length; j++)
                {
                    crossedFactors[i + j] += firstFactors[i] * secondFactors[j];
                }
            }
            return crossedFactors;
        }

        /// <summary>
        /// Polaczenie dwoch transmitancji ciaglych
        /// </summary>
        /// <param name="firstTransferFunction">
        /// Pierwsza transmitancja ciagla
        /// </param>
        /// <param name="secondTransferFunction">
        /// Druga transmitancja ciagla
        /// </param>
        /// <returns>
        /// Polaczona transmitancja ciagla
        /// </returns>
        public static ContinousTransferFunction ConnectTransferFunctions(ContinousTransferFunction firstTransferFunction, ContinousTransferFunction secondTransferFunction)
        {
            //Przemnozenie wspolycznnikow licznikow i mianownikow
            Double[] newNomFactors = CrossFactors(firstTransferFunction.nomFactors, secondTransferFunction.nomFactors);
            Double[] newDenFactors = CrossFactors(firstTransferFunction.denFactors, secondTransferFunction.denFactors);

            //Dodanie czasow opoznienia
            Double newTimeDelay = firstTransferFunction.timeDelay + secondTransferFunction.timeDelay;
            
            //Zwrocenie wynikowej transmitancji
            return new ContinousTransferFunction(newNomFactors, newDenFactors, newTimeDelay);
        }

        /// <summary>
        /// Polaczenie dwoch transmitancji dyskretnych
        /// </summary>
        /// <param name="firstTransferFunction">
        /// Pierwsza transmitancja dyskretna
        /// </param>
        /// <param name="secondTransferFunction">
        /// Druga transmitancja dyskretna
        /// </param>
        /// <returns>
        /// Polaczona transmitancja dyskretna
        /// </returns>
        public static DiscreteTransferFunction ConnectTransferFunctions(DiscreteTransferFunction firstTransferFunction, DiscreteTransferFunction secondTransferFunction)
        {
            //Nie mozna laczyc transmitancji o roznych czasach probkowania
            if(firstTransferFunction.sampleTime!=secondTransferFunction.sampleTime)
            {
                throw new InvalidOperationException("Transfer function must have the same sample time");
            }

            //Przemnozenie wspolycznnikow licznikow i mianownikow transmitancji
            Double[] newNomFactors = CrossFactors(firstTransferFunction.nomFactors, secondTransferFunction.nomFactors);
            Double[] newDenFactors = CrossFactors(firstTransferFunction.denFactors, secondTransferFunction.denFactors);

            //Dodanie czasow opoznienia
            Int32 newTimeDelay = firstTransferFunction.timeDelay + secondTransferFunction.timeDelay;

            return new DiscreteTransferFunction(newNomFactors, newDenFactors, newTimeDelay,firstTransferFunction.sampleTime);
        }

        /// <summary>
        /// Wyznaczenie wspolczynnikow licznika aproksymacji Pade
        /// </summary>
        /// <param name="n">
        /// Rzad aproksymacji
        /// </param>
        /// <param name="T0">
        /// Wspolczynnik opoznienia
        /// </param>
        /// <returns>
        /// Wspolczynniki licznika wielomianu Pade
        /// </returns>
        private static Double[] GetPadeNominator(int n, Double T0)
        {
            Double[] padeNominator = new Double[n+1];
   
            for(int i=0;i<padeNominator.Length;i++)
            {
                padeNominator[i] = Math.Pow(-T0, i) * Factorial(2 * n - i) / (Factorial(i) * Factorial(n - i));
            }

            return padeNominator;
        }

        /// <summary>
        /// Metoda przesuwajca wspolczynniki transmitancji dysrektnej o podane opóznienie
        /// </summary>
        /// <param name="factors">
        /// Wspolczynniki transmitancji dyskretnej
        /// </param>
        /// <param name="move">
        /// Czas opoznienia
        /// </param>
        /// <returns>
        /// Przesuniete wspolczynniki
        /// </returns>
        public static Double[] MoveFactors(Double[] factors, Int32 move)
        {
            Double[] newFactors = new Double[move+factors.Length];
            
            for(int i=0; i<factors.Length;i++)
            {
                newFactors[i+move] = factors[i];
            }

            return newFactors;
        }

        /// <summary>
        /// Wyznaczenie wspolczynnikow mianownika aproksymacji Pade
        /// </summary>
        /// <param name="n">
        /// Rzad aproksymacji
        /// </param>
        /// <param name="T0">
        /// Wspolczynnik opoznienia
        /// </param>
        /// <returns>
        /// Wspolczynniki mianownika wielomianu Pade
        /// </returns>
        private static Double[] GetPadeDenominator(int n, Double T0)
        {
            Double[] padeNominator = new Double[n + 1];
            for (int i = 0; i < padeNominator.Length; i++)
            {
                padeNominator[i] = Math.Pow(T0, i) * Factorial(2 * n - i) / (Factorial(i) * Factorial(n - i));
            }

            return padeNominator;
        }

        /// <summary>
        /// Aproksymacja Pade czlonu opoznienia
        /// </summary>
        /// <param name="timeDelay">
        /// Czas opoznienia
        /// </param>
        /// <param name="rank">
        /// Rzad aproksymacji Pade
        /// </param>
        /// <returns>
        /// Transmitancja ciagla czlonu opozniajacacego
        /// </returns>
        public static ContinousTransferFunction PadeApproximation(Double timeDelay,int rank)
        {
            return new ContinousTransferFunction(GetPadeNominator(rank, timeDelay), GetPadeDenominator(rank, timeDelay), 0);
        }

        /// <summary>
        /// Silnia
        /// </summary>
        /// <param name="n">
        /// Argument silni
        /// </param>
        /// <returns>
        /// Wartosc silni
        /// </returns>
        private static Double Factorial(int n)
        {
            int result = 1;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        /// <summary>
        /// Scehmat Hornera pozwalajacy na szybsze i lepiej uwarunkownane numeryczne wyznaczenie wartosci wielomianu w punkcie
        /// </summary>
        /// <param name="x">
        /// Punkt wielomianu
        /// </param>
        /// <param name="factors">
        /// Wspolczynniki wielomianu
        /// </param>
        /// <returns>
        /// Wartosc wielomianu w tym punkcie
        /// </returns>
        public static Double Horner(Double x, Double[] factors)
        {
            Double value = 0;

            for (int i = factors.Length - 1; i >= 0; i--)
            {
                value = value * x + factors[i];
            }

            return value;
        }

        /// <summary>
        /// Scehmat Hornera pozwalajacy na szybsze i lepiej uwarunkownane numeryczne wyznaczenie wartosci wielomianu w punkcie
        /// </summary>
        /// <param name="x">
        /// Punkt wielomianu
        /// </param>
        /// <param name="factors">
        /// Wspolczynniki wielomianu
        /// </param>
        /// <returns>
        /// Wartosc wielomianu w tym punkcie
        /// </returns>
        public static Complex Horner(Complex x, Complex[] factors)
        {
            Complex value = 0;

            for (int i = factors.Length - 1; i >= 0; i--)
            {
                value = value * x + factors[i];
            }

            return value;
        }

    }
}
