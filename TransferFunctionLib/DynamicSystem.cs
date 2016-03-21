using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Xml.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace TransferFunctionLib
{
    /// <summary>
    /// Zdarzenie zgłaszane gdy wspolczynniki transmitacji dyskretnej ulegają zmianie
    /// </summary>
    /// <param name="sender">
    /// Obiekt którego transmitancja dyskretna ulega zmianie
    /// </param>
    /// <param name="eventArgument">
    /// Argument metody obslugi zdarzenia
    /// </param>
    public delegate void DiscreteTransferFunctionChanged(object sender, DiscreteTransferFunctionChangedEventArg eventArgument);

    /// <summary>
    /// Argument zdarzenia wywoływanego za każdym gdy zmiennej przypisywana jest wartosc
    /// </summary>
    public class DiscreteTransferFunctionChangedEventArg : EventArgs
    {
        /// <summary>
        /// Nowe wspolczynniki licznika transmitancji dyskretnej
        /// </summary>
        public Double[] nominator
        {
            get;
            private set;
        }

        /// <summary>
        /// Nowe wspolczynniki mianownika transmitancji ciągłej
        /// </summary>
        public Double[] denomiantor
        {
            get;
            private set;
        }

        /// <summary>
        /// Dyskretne opoznienie transportowe
        /// </summary>
        public Double timeDelay
        {
            get;
            private set;
        }

        /// <summary>
        /// Konstruktor klasy argumentu zdarzenia zmiany transmitancji ciągłej
        /// </summary>
        /// <param name="nominator">
        /// Wspolczynniki licznika transmitancji dyskretnej
        /// </param>
        /// <param name="denominator">
        /// Wspolczynniki mianownika transmitancji dyskretnej
        /// </param>
        /// <param name="timeDelay">
        /// Dyskretne opóźnienie transportowe
        /// </param>
        public DiscreteTransferFunctionChangedEventArg(Double[] nominator, Double[] denominator, Double timeDelay)
        {
            this.nominator = nominator;
            this.denomiantor = denomiantor;
            this.timeDelay = timeDelay;
        }

    }

    /// <summary>
    /// Zdarzenie zgłaszane gdy wspolczynniki transmitacji ciaglej ulegają zmianie
    /// </summary>
    /// <param name="sender">
    /// Obiekt którego transmitancja ciagla ulega zmianie
    /// </param>
    /// <param name="eventArgument">
    /// Argument metody obslugi zdarzenia
    /// </param>
    public delegate void ContinousTransferFunctionChanged(object sender, ContinousTransferFunctionChangedEventArg eventArgument);

    /// <summary>
    /// Argument zdarzenia wywoływanego za każdym gdy zmiennej przypisywana jest wartosc
    /// </summary>
    public class ContinousTransferFunctionChangedEventArg : EventArgs
    {
        /// <summary>
        /// Nowe wspolczynniki licznika transmitancji ciaglej
        /// </summary>
        public Double[] nominator
        {
            get;
            private set;
        }

        /// <summary>
        /// Nowe wspolczynniki mianownika transmitancji ciągłej
        /// </summary>
        public Double[] denomiantor
        {
            get;
            private set;
        }

        /// <summary>
        /// Opoznienie transportowe
        /// </summary>
        public Double timeDelay
        {
            get;
            private set;
        }

        /// <summary>
        /// Konstruktor klasy argumentu zdarzenia zmiany transmitancji ciągłej
        /// </summary>
        /// <param name="nominator">
        /// Wspolczynniki licznika transmitancji ciągłej
        /// </param>
        /// <param name="denominator">
        /// Wspolczynniki mianownika transmitancji ciągłej
        /// </param>
        /// <param name="timeDelay">
        /// Opóźnienie transportowe
        /// </param>
        public ContinousTransferFunctionChangedEventArg(Double[] nominator, Double[] denominator, Double timeDelay)
        {
            this.nominator = nominator;
            this.denomiantor = denomiantor;
            this.timeDelay = timeDelay;
        }

    }

    /// <summary>
    /// Klasa reprezentujaca uklad dynamiczny
    /// </summary>
    public class DynamicSystem
    {
        /// <summary>
        /// Poczatkowy zakres czestotlwiosci automatycznej
        /// </summary>
        public Double OmegaFrom
        {
            get
            {
                return frequencyAnalizer.AutomaticDisplayRanges[0];
            }

        }

        /// <summary>
        /// Koncowy zakres czestotlwiosci automatycznej
        /// </summary>
        public Double OmegaTo
        {
            get
            {
                return frequencyAnalizer.AutomaticDisplayRanges[1];
            }

        }

        /// <summary>
        /// Metoda zwracajaca lańcuch znaków reprezentujacy wielomian mianownika transmitancji dyskretnej
        /// </summary>
        public String DiscreteDenominatorString
        {
            get
            {
                return TransferFunctionDisplay.ConvertDiscreteFactorsToString(DiscreteTransferFunction.denFactors);
            }

            private set
            {

            }
        }

        /// <summary>
        /// Metoda zwracajaca lańcuch znaków reprezentujacy wielomian licznika transmitancji dyskretnej
        /// </summary>
        public String DiscreteNominatorString
        {
            get
            {
                return TransferFunctionDisplay.ConvertDiscreteFactorsToString(DiscreteTransferFunction.nomFactors);
            }

            private set
            {

            }
        }

        /// <summary>
        /// Metoda zwracajaca lańcuch znaków reprezentujacy wielomian mianownika transmitancji ciągłej
        /// </summary>
        public String ContinousDenominatorString
        {
            get
            {
                return TransferFunctionDisplay.ConvertContinuesFactorsToString(ContinousTransferFunction.denFactors);
            }

            private set
            {

            }
        }

        /// <summary>
        /// Metoda zwracajaca lańcuch znaków reprezentujacy wielomian licznika transmitancji ciągłej
        /// </summary>
        public String ContinousNominatorString
        {
            get
            {
                return TransferFunctionDisplay.ConvertContinuesFactorsToString(ContinousTransferFunction.nomFactors);
            }

            private set
            {

            }
        }

        /// <summary>
        /// Metoda zwracajaca lańcuch znaków reprezentujacy wielomian mianownika transmitancji dyskretnej
        /// </summary>
        public String DiscreteDenominatorVectorString
        {
            get
            {
                return TransferFunctionDisplay.FactorsToVectorString(DiscreteTransferFunction.denFactors);
            }

            private set
            {

            }
        }

        /// <summary>
        /// Metoda zwracajaca lańcuch znaków reprezentujacy wielomian licznika transmitancji dyskretnej
        /// </summary>
        public String DiscreteNominatorVectorString
        {
            get
            {
                return TransferFunctionDisplay.FactorsToVectorString(DiscreteTransferFunction.nomFactors);
            }

            private set
            {

            }
        }

        /// <summary>
        /// Metoda zwracajaca lańcuch znaków reprezentujacy wielomian mianownika transmitancji ciągłej
        /// </summary>
        public String ContinousDenominatorVectorString
        {
            get
            {
                return TransferFunctionDisplay.FactorsToVectorString(ContinousTransferFunction.denFactors);
            }

            private set
            {

            }
        }

        /// <summary>
        /// Metoda zwracajaca lańcuch znaków reprezentujacy wielomian licznika transmitancji ciągłej
        /// </summary>
        public String ContinousNominatorVectorString
        {
            get
            {
                return TransferFunctionDisplay.FactorsToVectorString(ContinousTransferFunction.nomFactors);
            }

            private set
            {

            }
        }

        /// <summary>
        /// Metoda zwracajaca lańcuch znaków reprezentujacy człon opóźnienia transportowego transmitancji ciągłej
        /// </summary>
        public String ContinousTimeDelayString
        {
            get
            {
                return TransferFunctionDisplay.ConvertContinuesTimeDelayToString(ContinousTimeDelay);
            }

            private set
            {

            }
        }

        /// <summary>
        /// Metoda zwracajaca lańcuch znaków reprezentujacy wczłon opóźnienia transportowego transmitancji dyskretnej
        /// </summary>
        public String DiscreteTimeDelayString
        {
            get
            {
                return TransferFunctionDisplay.ConvertDiscreteTimeDelayToString(DiscreteTimeDelay);
            }

            private set
            {

            }
        }

        /// <summary>
        /// Zdarzenie zglaszane podczas gdy transmitancja dyskretna ulega zmianie
        /// </summary>
        public event DiscreteTransferFunctionChanged discreteTransferFunctionChanged;

        /// <summary>
        /// Zdarzenie zglaszane podczas gdy transmitancja dyskretna ulega zmianie
        /// </summary>
        public event ContinousTransferFunctionChanged continousTransferFunctionChanged;

        /// <summary>
        /// Obiekt sluzacy do przeprowadzania obliczen na transmitancjach
        /// </summary>
        private TransferFunctionCalculator transferFunctionCalculator;

        /// <summary>
        /// Obiekt umozliwiajacy analize czestotliwosciowa ukladu
        /// </summary>
        private FrequencyAnalizer frequencyAnalizer;

        /// <summary>
        /// Wlasciwosc okreslajaca czy układ po zamknieciu w petli sprzezenia zwrotnego bedzie stabilny
        /// </summary>
        public bool StableAfterClosedLoop
        {
            get
            {
                return IsStableAfterClose();
            }

            private set
            {

            }
        }

        /// <summary>
        /// Metoda okreslajaca czy uklad po zamknieciu w petli sprzezenia zwrotnego bedzie stabilny
        /// </summary>
        /// <returns>
        /// Czy uklad po zamknieciu w petli sprzeznia zwrotnego bedzie stabilny
        /// </returns>
        private bool IsStableAfterClose()
        {
            //Jezeli nie ma zapasu stabilnosci - uklad bedzie stabilny
            if (GainMargin == null)
            {
                return true;
            }

            //Jezeli zapas stabilnosci jest wiekszy od zera - uklad bedzie stabilny
            if(GainMargin.Value > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Kolekcja zapasow modulu ukladu - ujemnych punktow przeciecia z osia Rez 
        /// </summary>
        public Margin[] GainMargins
        {
            get
            {
                return frequencyAnalizer.GainMargins;
            }
            private set
            {

            }
        }

        /// <summary>
        /// Kolekcja zapasow fazy ukladu - w ktorych modul ma wartosc 1 
        /// </summary>
        public Margin[] PhaseMargins
        {
            get
            {
                return frequencyAnalizer.PhaseMargins;
            }

            private set
            {

            }
        }

        /// <summary>
        /// Zapas modulu ukladu - najmniejszy zapas modulu z ich kolekcji
        /// </summary>
        public Margin GainMargin
        {
            get
            {
                return frequencyAnalizer.GainMargin;
            }

            private set
            {

            }
        }

        /// <summary>
        /// Zapas fazy układu - punkt zapasy fazy o najmniejszej wartosci bezwzglednej
        /// </summary>
        public Margin PhaseMargin
        {
            get
            {
                return frequencyAnalizer.PhaseMargin;
            }

            private set
            {

            }
        }

        /// <summary>
        /// Punkt charakterystyki czestotliwosciwej dla nieskoczonej czestotliwosci
        /// </summary>
        public FrequencyPoint PointForOmegaInf
        {
            get
            {
                return frequencyAnalizer.PointForOmegaInf;
            }
            private set
            {

            }
        }

        /// <summary>
        /// Punkt charakterystyki czestotliwosciwej dla zerowej czestotliwosci
        /// </summary>
        public FrequencyPoint PointForOmegaZero
        {
            get
            {
                return frequencyAnalizer.PointForOmegaZero;
            }
            private set
            {

            }
        }

        private DiscreteTransferFunction discreteTransferFunction;
        /// <summary>
        /// Wlasciwosc reprezentujaca dyskretna transmitancja ukladu
        /// </summary>
        private DiscreteTransferFunction DiscreteTransferFunction
        {
            get
            {
                return discreteTransferFunction;
            }

            set
            {
                //Kazda zmiana dyskretnej wlasciwosci pociaga za soba koniecznosc zmiany transmitancji ciaglej
                discreteTransferFunction = value;
                RefreshContinousTransferFunction();

                if (discreteTransferFunctionChanged != null)
                {
                    discreteTransferFunctionChanged(this, new DiscreteTransferFunctionChangedEventArg(DiscreteNumerator, DiscreteDenumerator, DiscreteTimeDelay));
                }
            }
        }

        private ContinousTransferFunction continousTransferFunction;
        /// <summary>
        /// Wlasciwosc reprezentujaca transmitancje ciagla
        /// </summary>
        private ContinousTransferFunction ContinousTransferFunction
        {
            get
            {
                return continousTransferFunction;
            }
            set
            {
                //Kazda zmiana transmitancji ciaglej pociaga za soba koniecznosc odswiezenia transmitnacji dyskretnej oraz analizatora czestotliwosci
                continousTransferFunction = value;
                RefreshDiscreteTransferFunction();
                RefreshFrequencyAnalizer();

                if (continousTransferFunctionChanged != null)
                {
                    continousTransferFunctionChanged(this, new ContinousTransferFunctionChangedEventArg(ContinousNumerator, ContinousDenumerator, ContinousTimeDelay));
                }
            }
        }

        /// <summary>
        /// Czas opoznienia w postaci ciągłej
        /// </summary>
        public Double ContinousTimeDelay
        {
            get
            {
                return ContinousTransferFunction.timeDelay;
            }

            private set
            {
            }
        }

        /// <summary>
        /// Dyskretny czas opóżnienia
        /// </summary>
        public Double DiscreteTimeDelay
        {
            get
            {
                return DiscreteTransferFunction.timeDelay;
            }

            private set
            {

            }
        }
        
        /// <summary>
        /// Metoda odswiezajaca transmitancje dyskretna
        /// </summary>
        private void RefreshDiscreteTransferFunction()
        {
            //przelicza za pomoca metody goodwina transmitancje ciagla na dyskretna
            discreteTransferFunction = transferFunctionCalculator.ConvertToDiscreteTransferFunction(continousTransferFunction, SimulationSampleTime);

            //Kazda zmiana transmintacja ponosi za soba koniecznosc zgloszenia zdarzenia informujacego o jej zmianie
            if(discreteTransferFunctionChanged != null)
            {
                discreteTransferFunctionChanged(this, new DiscreteTransferFunctionChangedEventArg(DiscreteNumerator, DiscreteDenumerator, DiscreteTimeDelay));
            }
        }

        /// <summary>
        /// Metoda odswiezajaca transmitancje ciagla
        /// </summary>
        private void RefreshContinousTransferFunction()
        {
            //Przelicza za pomoca metody goodwina transmitancje dyskretna na ciagla
            continousTransferFunction = transferFunctionCalculator.ConvertToContinuesTransferFunction(discreteTransferFunction);

            //Odswiezenie analizatora czestotliwosci - konieczne poniewaz przy zmianie transmitancji ciaglej nalezy go na nowo zainicjalizowac
            RefreshFrequencyAnalizer();

            //Kazda zmiana transmintacja ponosi za soba koniecznosc zgloszenia zdarzenia informujacego o jej zmianie
            if (continousTransferFunctionChanged != null)
            {
                continousTransferFunctionChanged(this, new ContinousTransferFunctionChangedEventArg(ContinousNumerator, ContinousDenumerator, ContinousTimeDelay));
            }
        }

        /// <summary>
        /// Metoda odswiezajaca analizator czestotliwosci
        /// </summary>
        private void RefreshFrequencyAnalizer()
        {
            //Kazde jego odswiezenie wymaga stworzenia go ponownie
            frequencyAnalizer = new FrequencyAnalizer(continousTransferFunction);
        }

        private Double simulationSampleTime;
        /// <summary>
        /// Czas probkowania symulacji
        /// </summary>
        public Double SimulationSampleTime
        {
            get
            {
                return simulationSampleTime;
            }

            set
            {
                simulationSampleTime = value;

                //Kazda zmiana czasu probkowania wymaga na nowo przeliczenia transmitancji dyskretnej
                RefreshDiscreteTransferFunction();

            }
        }

        /// <summary>
        /// Metoda pozwalajaca na zmiane wspolczynnikow transmitancji poprzez okreslenie wspolczynnikow transmitancji dyskretnej
        /// </summary>
        /// <param name="discreteNominator">
        /// Wspolczynniki licznika transmitancji dyskretnej
        /// </param>
        /// <param name="discreteDenominator">
        /// Wspolczynniki mianownika transmitancji dyskretnej
        /// </param>
        /// <param name="timeDelay">
        /// Opoznienie transportowe
        /// </param>
        /// <param name="simulationSampleTime">
        /// Czas probkowania
        /// </param>
        public void EditDiscrete(Double[] discreteNominator, Double[] discreteDenominator, int timeDelay, Double simulationSampleTime)
        {
            DiscreteTransferFunction = new DiscreteTransferFunction(discreteNominator, discreteDenominator, timeDelay, simulationSampleTime);
        }

        /// <summary>
        /// Metoda pozwalajaca na zmiane wspolczynnikow transmitancji poprzez okreslenie wspolczynnikow transmitancji ciaglej
        /// </summary>
        /// <param name="continousNominator">        /// Wspolczynniki licznika transmitancji ciaglej
        /// </param>
        /// <param name="continousDenominator">
        /// Wspolczynniki mianownika transmitancji ciaglej
        /// </param>
        /// <param name="timeDelay">
        /// Opoznienie transportowe
        /// </param>
        public void EditContinous(Double[] continousNominator, Double[] continousDenominator, Double timeDelay)
        {
            ContinousTransferFunction = new ContinousTransferFunction(continousNominator, continousDenominator, timeDelay);
        }

        /// <summary>
        /// Klasa reprezentujaca uklad dynamiczny
        /// </summary>
        /// <param name="discreteTransferFunction">
        /// Transmitancja dyskretna ukladu
        /// </param>
        internal DynamicSystem(DiscreteTransferFunction discreteTransferFunction )
        {
            //Inicjalizacja obiektu przeliczajacego transmitancje - jest to konieczne aby zostal on stworzony w pierwszej kolejnosci - sluzy do stworzenia innego typu transmitancji
            this.transferFunctionCalculator = new TransferFunctionCalculator();

            this.DiscreteTransferFunction = discreteTransferFunction;

            this.simulationSampleTime = discreteTransferFunction.sampleTime;
          
        }

        /// <summary>
        /// Klasa reprezentujaca uklad dynamiczny
        /// </summary>
        /// <param name="continousTransferFunction">
        /// Transmitancja ciagla ukladu
        /// </param>
        /// <param name="simulationSampleTime">
        /// Czas probkowania symulacji
        /// </param>
        internal DynamicSystem(ContinousTransferFunction continousTransferFunction, Double simulationSampleTime)
        {
            //Inicjalizacja obiektu przeliczajacego transmitancje - jest to konieczne aby zostal on stworzony w pierwszej kolejnosci - sluzy do stworzenia innego typu transmitancji
            this.transferFunctionCalculator = new TransferFunctionCalculator();
            //Trzeba najpierw inicjalizowac czasu symulacji, aby samoczynnie wykonala sie konwersja transmitancji ciagłej na dyskretna, ktora korzysta z tego pola
            this.simulationSampleTime = simulationSampleTime;
            this.ContinousTransferFunction = continousTransferFunction;
        }

        /// <summary>
        /// Metoda wytworcza tworzaca obiekt ukladu dynamicznego na podstawie parametrow transmitancji ciaglej
        /// </summary>
        /// <param name="nominator">
        /// Licznik transmitancji ciaglej
        /// </param>
        /// <param name="denominator">
        /// Mianownik transmitnacji ciaglej
        /// </param>
        /// <param name="timeDelay">
        /// Czas opoznienia
        /// </param>
        /// <param name="sampleTime">
        /// Czas probkowania
        /// </param>
        /// <returns>
        /// Uklad dynamiczny o podanej transmitancji dyskretnej
        /// </returns>
        public static DynamicSystem FromDiscreteTransferFuntion(Double[] nominator, Double[] denominator, Int32 timeDelay, Double sampleTime)
        {
            return new DynamicSystem(new DiscreteTransferFunction(nominator, denominator, timeDelay, sampleTime));
        }

        /// <summary>
        /// Metoda wytworcza tworzaca obiekt ukladu dynamicznego na podstawie parametrow transmitancji dyskretnej
        /// </summary>
        /// <param name="nominator">
        /// Licznik transmitancji ciaglej
        /// </param>
        /// <param name="denominator">
        /// Mianownik transmitnacji ciaglej
        /// </param>
        /// <param name="timeDelay">
        /// Dyskretny czas opoznienia
        /// </param>
        /// <param name="simulationSampleTime">
        /// Czas probkowania symulacji ukladu
        /// </param>
        /// <returns>
        /// Uklad dynamiczny o podanej transmitancji ciaglej
        /// </returns>
        public static DynamicSystem FromContinousTransferFuntion(Double[] nominator, Double[] denominator, Double timeDelay, Double simulationSampleTime)
        {
            return new DynamicSystem(new ContinousTransferFunction(nominator, denominator, timeDelay), simulationSampleTime);
        }

        /// <summary>
        /// Metoda tworzaca uklad dynamiczny na podstawie podanych lancuchow znakow okreslajacych parametry jej transmitancji dyskretnej
        /// </summary>
        /// <param name="nominatorString">
        /// Lanuch znakow parametrow licznika
        /// [ a0 a1 a2 .. an-1 an ]
        /// </param>
        /// <param name="denominatorString">
        /// Lanuch znakow parametrow mianownika
        /// [ b0 b1 b2 .. bm-1 bm ]
        /// </param>
        /// <param name="timeDelayString">
        /// Opoznienie transportowe
        /// </param>
        /// <param name="sampleTimeString">
        /// Czas probkowania
        /// </param>
        /// <returns>
        /// Stworzony uklad dynamiczny
        /// </returns>
        public static DynamicSystem FromDiscreteFactorsString(String nominatorString, String denominatorString, String timeDelayString, string sampleTimeString)
        {
            //Pobranie wspolczynnikow na podstawie podanych lancuchow znakow
            Double[] nominator = TransferFunctionDisplay.FactorsFromString(nominatorString);
            Double[] denominator = TransferFunctionDisplay.FactorsFromString(denominatorString);

            Int32 timeDelay = Convert.ToInt32(Convert.ToDouble(timeDelayString.Replace('.',',')));
            Double sampleTime = Convert.ToDouble(sampleTimeString.Replace('.', ','));
            
            //Stworzenie i zwrocenie transmitancji
            return FromDiscreteTransferFuntion(nominator, denominator, timeDelay, sampleTime);
        }

        /// <summary>
        /// Metoda tworzaca uklad dynamiczny na podstawie podanych lancuchow znakow okreslajacych parametry jej transmitancji ciaglej
        /// </summary>
        /// <param name="nominatorString">
        /// Lanuch znakow parametrow licznika
        /// [ a0 a1 a2 .. an-1 an ]
        /// </param>
        /// <param name="denominatorString">
        /// Lanuch znakow parametrow mianownika
        /// [ b0 b1 b2 .. bm-1 bm ]
        /// </param>
        /// <param name="timeDelayString">
        /// Opoznienie transportowe
        /// </param>
        /// <param name="sampleTimeString">
        /// Czas probkowania
        /// </param>
        /// <returns>
        /// Stworzony uklad dynamiczny
        /// </returns>
        public static DynamicSystem FromContinousFactorsString(String nominatorString, String denominatorString, String timeDelayString, string sampleTimeString)
        {
            //Pobranie wspolczynnikow na podstawie podanych lancuchow znakow
            Double[] nominator = TransferFunctionDisplay.FactorsFromString(nominatorString);
            Double[] denominator = TransferFunctionDisplay.FactorsFromString(denominatorString);

            Double timeDelay = Convert.ToDouble(timeDelayString.Replace('.', ','));
            Double sampleTime = Convert.ToDouble(sampleTimeString.Replace('.', ','));

            //Stworzenie i zwrocenie transmitancji
            return FromContinousTransferFuntion(nominator, denominator, timeDelay, sampleTime);
        }

        /// <summary>
        /// Metoda tworzaca uklad dynamiczny na podstawie obiektu jezyka XML
        /// </summary>
        /// <param name="documentXML">
        /// dokument XML
        /// </param>
        /// <returns>
        /// Stworzony uklad dynamiczny
        /// </returns>
        public static DynamicSystem FromXML(XDocument documentXML)
        {
            return DynamicSystemXMLSerializer.DynamicSystemFromXML(documentXML);
        }

        /// <summary>
        /// Metoda symulajaca zachowanie ukladu dynamicznego w petli zamknietej z podanym regulatorem przy wystapieniu zaklocenia
        /// </summary>
        /// <param name="controller">
        /// Obiekt regulatora
        /// </param>
        /// <param name="disturbance">
        /// Tablica wartosci zaklocen
        /// </param>
        /// <returns>
        /// Tablica wartosci zmiennych mierzonych
        /// </returns>
        public Double[] SimulateCloseLoopWithControllerDisturbance(DynamicSystem controller,Double[] disturbance)
        {
            return DiscreteTransferFunctionSimulator.SimulateCloseLoopDisturbance(disturbance, controller.discreteTransferFunction, discreteTransferFunction);
        }

        /// <summary>
        /// Metoda symulajaca zachowanie ukladu dynamicznego w petli zamknietej z podanym regulatorem przy wystapieniu zaklocenia
        /// </summary>
        /// <param name="observableCollection">
        /// Kolekcja wartosci odpowiedzi ukladu dedykowana do wspolpracy z mechanizmem wiazania danych WPF
        /// </param>
        /// <param name="controller">
        /// Obiekt regulatora
        /// </param>
        /// <param name="disturbance">
        /// Tablica wartosci zaklocen
        /// </param>
        public void SimulateCloseLoopWithControllerDisturbanceObservable(ChartPointObservableCollection observableCollection, DynamicSystem controller, Double[] disturbance)
        {
            DiscreteTransferFunctionSimulator.SimulateCloseLoopDisturbanceObservable(observableCollection, disturbance, controller.discreteTransferFunction, discreteTransferFunction);
        }

        /// <summary>
        /// Metoda symulajaca zachowanie ukladu dynamicznego w petli zamknietej z podanym regulatorem przy wystapieniu zmiany sygnalu zadanego
        /// </summary>
        /// <param name="controller">
        /// Obiekt regulatora
        /// </param>
        /// <param name="setpoints">
        /// Tablica wartosci sygnalu zadanego
        /// </param>
        /// <returns>
        /// Tablica wartosci zmiennych mierzonych
        /// </returns>
        public Double[] SimulateCloseLoopWithControllerSetpoint( DynamicSystem controller,Double[] setpoints)
        {
            return DiscreteTransferFunctionSimulator.SimulateCloseLoopWithController(setpoints, controller.discreteTransferFunction, discreteTransferFunction);
        }

        /// <summary>
        /// Metoda symulajaca zachowanie ukladu dynamicznego w petli zamknietej z podanym regulatorem przy wystapieniu zmiany sygnalu zadanego
        /// </summary>
        /// <param name="observableCollection">
        /// Kolekcja wartosci odpowiedzi ukladu dedykowana do wspolpracy z mechanizmem wiazania danych WPF
        /// </param>
        /// <param name="controller">
        /// Obiekt regulatora
        /// </param>
        /// <param name="setpoints">
        /// Tablica wartosci sygnalu zadanego
        /// </param>
        public void SimulateCloseLoopWithControllerSetpoint(ChartPointObservableCollection observableCollection,DynamicSystem controller, Double[] setpoints)
        {
            DiscreteTransferFunctionSimulator.SimulateCloseLoopWithControllerObservable(observableCollection,setpoints, controller.discreteTransferFunction, discreteTransferFunction);
        }

        /// <summary>
        /// Metoda wyznaczajaca odpowiedz ukladu na tablice wymuszen podanej jako argument
        /// </summary>
        /// <param name="inputs">
        /// Tablica wymuszen ukladu
        /// </param>
        /// <returns>
        /// Tablica odpowiedzi ukladu
        /// </returns>
        public Double[] Simulate( Double[] inputs)
        {
            return DiscreteTransferFunction.Simulate(inputs);
        }

        /// <summary>
        /// Metoda wyznaczajaca odpowiedz ukladu na tablice wymuszen podanej jako argument
        /// </summary>
        /// <param name="inputs">
        /// Tablica wymuszen ukladu
        /// </param>
        /// <returns>
        /// Tablica odpowiedzi ukladu
        /// </returns>
        public Double[] Simulate(Double[] inputs, Double uInitialValue, Double yInitialValue)
        {
            return DiscreteTransferFunction.Simulate(inputs,uInitialValue,yInitialValue);
        }

       /// <summary>
        /// Metoda wyznaczajaca odpowiedz ukladu na tablice wymuszen podanej jako argument
       /// </summary>
       /// <param name="observableCollection">
        /// Kolekcja wartosci odpowiedzi ukladu dedykowana do wspolpracy z mechanizmem wiazania danych WPF
       /// </param>
       /// <param name="inputs">
        /// Tablica wymuszen ukladu
       /// </param>
        public void SimulateObservable(ChartPointObservableCollection observableCollection, Double[] inputs)
        {
            DiscreteTransferFunction.SimulateObservable(observableCollection,inputs);
        }
        
        /// <summary>
        /// Kopia licznika transmitancji ciaglej
        /// </summary>
        public Double[] ContinousNumerator
        {
            get
            {
                //Stworzenie i zwrocenie kopii licznika transmitancji ciaglej - w celu zapobieganiu konfiguracji transmitancji przez uzytkownika
                Double[] continousNumerator = new Double[ContinousTransferFunction.nomFactors.Length];
                for (int i = 0; i < continousNumerator.Length; i++)
                {
                    continousNumerator[i] = ContinousTransferFunction.nomFactors[i];
                }

                return continousNumerator;
            }

            private set
            {

            }

        }

        /// <summary>
        /// Kopia mianownika transmitancji ciaglej
        /// </summary>
        public Double[] ContinousDenumerator
        {
            get
            {
                //Stworzenie i zwrocenie kopii mianownika transmitancji ciaglej - w celu zapobieganiu konfiguracji transmitancji przez uzytkownika
                Double[] continousDenumerator = new Double[ContinousTransferFunction.denFactors.Length];
                for (int i = 0; i < continousDenumerator.Length; i++)
                {
                    continousDenumerator[i] = ContinousTransferFunction.denFactors[i];
                }

                return continousDenumerator;
            }

            private set
            {

            }
        }

        /// <summary>
        /// Kopia licznika transmitancji dyskretnej
        /// </summary>
        public Double[] DiscreteNumerator
        {
            get
            {
                //Stworzenie i zwrocenie kopii licznika transmitancji dyskretnej - w celu zapobieganiu konfiguracji transmitancji przez uzytkownika
                Double[] discreteNumerator = new Double[DiscreteTransferFunction.nomFactors.Length];
                for (int i = 0; i < discreteNumerator.Length; i++)
                {
                    discreteNumerator[i] = DiscreteTransferFunction.nomFactors[i];
                }

                return discreteNumerator;
            }
            
            private set
            {

            }
        }

        /// <summary>
        /// Kopia mianownika transmitancji dyskretnej
        /// </summary>
        public Double[] DiscreteDenumerator
        {

            get
            {
                //Stworzenie i zwrocenie kopii mianownika transmitancji dyskretnej - w celu zapobieganiu konfiguracji transmitancji przez uzytkownika
                Double[] discreteDenumerator = new Double[DiscreteTransferFunction.denFactors.Length];
                for (int i = 0; i < discreteDenumerator.Length; i++)
                {
                    discreteDenumerator[i] = DiscreteTransferFunction.denFactors[i];
                }

                return discreteDenumerator;
            }

            private set
            {

            }
        }

        /// <summary>
        /// Operator szeregowego łączenia dwóch układów dynamicznych
        /// Nie mozna laczyc ze soba transmitancji o roznych czasach probkowania
        /// </summary>
        /// <param name="ds1">
        /// Pierwszy uklad dynamiczny
        /// </param>
        /// <param name="ds2">
        /// Drugi układ dynamiczny
        /// </param>
        /// <returns>
        /// Wynikowy układ dynamiczny
        /// </returns>
        public static DynamicSystem operator *(DynamicSystem ds1, DynamicSystem ds2)
        {
            //Nie mozna laczyc ze soba transmitancji o roznych czasach probkowania
            if(ds1.SimulationSampleTime != ds2.SimulationSampleTime)
            {
                throw new InvalidOperationException("Sample time in both dynamic systems must be the same");
            }

            return new DynamicSystem(TransferFunctionCalculator.ConnectTransferFunctions(ds1.continousTransferFunction, ds2.continousTransferFunction), ds1.simulationSampleTime);
        }

        /// <summary>
        /// Metoda zwracajaca obiekt dokumentu XML reprezentujacy uklad dynamiczny
        /// </summary>
        /// <param name="type">
        /// Typ zapisu - transmitancja ciagla/dyskretna
        /// </param>
        /// <returns>
        /// Dokument XML
        /// </returns>
        public XDocument ToXML(SystemType type)
        {
            return DynamicSystemXMLSerializer.DynamicSystemToXML(this, type);
        }

        /// <summary>
        /// Metoda zwracajaca punkty reprezentujace charakterystyke czestotliwosciowa Nyquista
        /// </summary>
        /// <param name="frequencyStart">
        /// Czestotliwosc poczatkowa
        /// </param>
        /// <param name="frequencyStop">
        /// Czestotliwosc koncowa
        /// </param>
        /// <param name="numberOfPoints">
        /// Liczba punktow charakterystyki
        /// </param>
        /// <returns>
        /// Kolekcja punktow charakterystyki Nyquista
        /// </returns>
        public FrequencyPoint[] NyquistPlot(double frequencyStart, double frequencyStop, long numberOfPoints = 500)
        {
            return frequencyAnalizer.GetNyquistPlot(frequencyStart, frequencyStop, numberOfPoints);
        }

        /// <summary>
        /// Metoda zwracajaca punkty reprezentujace charakterystyke czestotliwosciowa Nyquista
        /// </summary>
        /// <returns>
        /// Kolekcja punktow charakterystyki Nyquista
        /// </returns>
        public FrequencyPoint[] NyquistPlot()
        {
            return frequencyAnalizer.GetNyquistPlot();
        }

        /// <summary>
        /// Metoda zwracajaca punkty reprezentujace charakterystyke czestotliwosciowa Nyquista
        /// </summary>
        /// <param name="observableCollection">Kolekcja punktow charakterystyki Nyquista w postaci kompatybilnej z mechanizmem wiazania danych WPF</param>
        /// <param name="omegaStart">Czestotliwosc poczatkowa</param>
        /// <param name="omegaStop">Czestotliwosc koncowa</param>
        /// <param name="numberOfPoints">Liczba punktow charakterystyki</param>
        public void NyquistPlotObservable(FrequencyPointsObservableCollection observableCollection,Double omegaStart, Double omegaStop, Int64 numberOfPoints = 1000)
        {
            frequencyAnalizer.GetNyquistPlotObservable(observableCollection,omegaStart, omegaStop, numberOfPoints);
        }

        /// <summary>
        /// Metoda zwracajaca punkty reprezentujace charakterystyke czestotliwosciowa Nyquista
        /// </summary>
        /// <param name="observableCollection">
        /// Kolekcja punktow charakterystyki Nyquista w postaci kompatybilnej z mechanizmem wiazania danych WPF
        /// </param>
        public void NyquistPlotObservable(FrequencyPointsObservableCollection observableCollection)
        {
            frequencyAnalizer.GetNyquistPlotObservable(observableCollection);
        }

        /// <summary>
        /// Metoda zwracajaca punkty reprezentujace charakterystyke czestotliwosciowa Bodego
        /// </summary>
        /// <returns>
        /// Kolekcja punktow charakterystyki Bodego
        /// </returns>
        public BodePoint[] BodePlot()
        {
            return frequencyAnalizer.GetBodePlot();
        }

        /// <summary>
        /// Metoda zwracajaca punkty reprezentujace charakterystyke czestotliwosciowa Bodego
        /// </summary>
        /// <param name="frequencyStart">
        /// Czestotliwosc poczatkowa
        /// </param>
        /// <param name="frequencyStop">
        /// Czestotliwosc końcowa
        /// </param>
        /// <param name="numberOfPoints">
        /// Liczba punktow charakterystyki
        /// </param>
        /// <returns>
        /// Kolekcja punktow charakterystyki Bodego
        /// </returns>
        public BodePoint[] BodePlot(double frequencyStart, double frequencyStop, long numberOfPoints = 500)
        {
            return frequencyAnalizer.GetBodePlot(frequencyStart, frequencyStop, numberOfPoints);
        }

        /// <summary>
        /// Metoda zwracajaca punkty reprezentujace charakterystyke czestotliwosciowa Bodego
        /// </summary>
        /// <param name="observableCollection">
        /// Kolekcja punktow charakterystyki Bodego w postaci kompatybilnej z mechanizmem wiazania danych WPF
        /// </param>
        public void BodePlotObservable(BodePointsDataPoints observableCollection)
        {
            frequencyAnalizer.GetBodePlotObservable(observableCollection);
        }

        /// <summary>
        /// Metoda zwracajaca punkty reprezentujace charakterystyke czestotliwosciowa Bodego
        /// </summary>
        /// <param name="observableCollection">
        /// Kolekcja punktow charakterystyki Bodego w postaci kompatybilnej z mechanizmem wiazania danych WPF
        /// </param>
        /// <param name="frequencyStart">
        /// Czestotliwosc poczatkowa
        /// </param>
        /// <param name="frequencyStop">
        /// Czestotliwosc końcowa
        /// </param>
        /// <param name="numberOfPoints">
        /// Liczba punktow charakterystyki
        /// </param>
        public void BodePlotObservable(BodePointsDataPoints observableCollection, double frequencyStart, double frequencyStop, long numberOfPoints = 500)
        {
            frequencyAnalizer.GetBodePlotObservable(observableCollection, frequencyStart, frequencyStop, numberOfPoints);
        }

        public void NormalizeContinous()
        {
            Double[] nominator = ContinousNumerator;
            Double[] denominator = ContinousDenumerator;

            int nominatorLength = FactorsLength(nominator);

            int denominatorLength = FactorsLength(denominator);

            Double[] normalizedDenominator = new Double[denominatorLength];
            Double[] normalizedNominator = new Double[nominatorLength];

            Double firstDenNonZeroFactorIndex = 0;

            for (int i = 0; i < denominatorLength; i++ )
            {
                if(denominator[i] != 0)
                {
                    firstDenNonZeroFactorIndex = denominator[i];
                    break;
                }
            }

            if(firstDenNonZeroFactorIndex == 0)
            {
                return;
            }

            for (int i = 0; i < nominatorLength; i++)
            {
                normalizedNominator[i] = nominator[i]/firstDenNonZeroFactorIndex;
            }

            for (int i = 0; i < denominatorLength; i++)
            {
                normalizedDenominator[i] = denominator[i] / firstDenNonZeroFactorIndex;
            }

            EditContinous(normalizedNominator, normalizedDenominator, ContinousTimeDelay);
        }

        private Int32 FactorsLength(Double[] factors)
        {
            for(int i = factors.Length - 1; i > 0; i--)
            {
                if(factors[i]!=0)
                {
                    return i+1;
                }

            }

            return 1;
        }

        public void NormalizeDiscrete()
        {
            Double[] nominator = DiscreteNumerator;
            Double[] denominator = DiscreteDenumerator;

            int nominatorLength = FactorsLength(nominator);

            int denominatorLength = FactorsLength(denominator);

            Double[] normalizedDenominator = new Double[denominatorLength];
            Double[] normalizedNominator = new Double[nominatorLength];

            Double firstDenNonZeroFactorIndex = 0;

            for (int i = 0; i < denominatorLength; i++)
            {
                if (denominator[i] != 0)
                {
                    firstDenNonZeroFactorIndex = denominator[i];
                    break;
                }
            }

            if (firstDenNonZeroFactorIndex == 0)
            {
                return;
            }

            for (int i = 0; i < nominatorLength; i++)
            {
                normalizedNominator[i] = nominator[i] / firstDenNonZeroFactorIndex;
            }

            for (int i = 0; i < denominatorLength; i++)
            {
                normalizedDenominator[i] = denominator[i] / firstDenNonZeroFactorIndex;
            }

            EditDiscrete(normalizedNominator, normalizedDenominator, Convert.ToInt32(DiscreteTimeDelay),SimulationSampleTime);
        }

    }
}
