using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace TransferFunctionLib
{
    /// <summary>
    /// Klasa symulatora transmitancji dyskretnej
    /// </summary>
    internal class DiscreteTransferFunctionSimulator
    {
        /// <summary>
        /// Referencja do dyskretnej transmitancji symulowanej
        /// </summary>
        private DiscreteTransferFunction discreteTransferFunction;

        /// <summary>
        /// Konstruktor klasy symulatora
        /// </summary>
        /// <param name="discreteTransferFunction">
        /// Symulowana transmitancja dyskretna
        /// </param>
        public DiscreteTransferFunctionSimulator(DiscreteTransferFunction discreteTransferFunction)
        {
            this.discreteTransferFunction = discreteTransferFunction;
        }

        /// <summary>
        /// Metoda symulujaca uklad o podanej transmitancji dyskretnej
        /// </summary>
        /// <param name="inputVector">
        /// Wektor sygnalu wejsciowego
        /// </param>
        /// <returns>
        /// Wektor sygnalu wyjsciowego
        /// </returns>
        public Double[] Simulate(Double[] inputVector)
        {
            //Stworzenie buforow dla wartosci wejsciowych i wyjsciowych - musza posiadac odpowiednie dlugosci uwzgledniajaca rzad i opoznienie obiektu
            ValueBuffor inputBuffor = new ValueBuffor(discreteTransferFunction.nomFactors.Length + discreteTransferFunction.timeDelay,0);
            ValueBuffor outputBuffor = new ValueBuffor(discreteTransferFunction.denFactors.Length - 1, 0);

            //Wektor sygnalow wyjsciowych
            Double[] outputVector = new Double[inputVector.Length];

            //Normalizacja wspolycznnikow licznika i mianownika - tak aby wspolczynnik a0 mial wartosc 1
            Double[] normalizedNomFactors = NormalizedNomFactors();
            Double[] normalizedDenFactors = NormalizedDenFactors();

            //Wyznaczenie kolejnych wartosci sygnalow wyjsciowych
            for(int i=0; i<outputVector.Length; i++)
            {
                //Inicjalizacja wartosci nowo wyliczonego wyjscia
                Double newOutputValue = 0;

                //Dodanie nowej wartosci wejsciowej do bufora ich przejmujacego
                inputBuffor.AddNewValue(inputVector[i]);

                //Wyzaczenie wartosci od wspolczynnikow licznika ( sygnalu U )
                for(int j=0; j < discreteTransferFunction.nomFactors.Length; j++)
                {
                    newOutputValue += normalizedNomFactors[j] * inputBuffor[j + discreteTransferFunction.timeDelay];
                }

                //Wyznaczenie wartosci od wspolczynnikow mianownika ( sygnal Y )
                for (int j = 1; j < discreteTransferFunction.denFactors.Length; j++)
                {
                    newOutputValue -= normalizedDenFactors[j] * outputBuffor[j-1];
                }

                //Przypisanie nowej wartosci do ich kolekcji
                outputVector[i] = newOutputValue;

                //Dodanie nowej wartosci sygnalu wyjsciowego do bufora
                outputBuffor.AddNewValue(newOutputValue);
            }

            return outputVector;
        }

        public Double[] Simulate(Double[] inputVector, Double uInitialValue, Double yInitialValue)
        {
            //Stworzenie buforow dla wartosci wejsciowych i wyjsciowych - musza posiadac odpowiednie dlugosci uwzgledniajaca rzad i opoznienie obiektu
            ValueBuffor inputBuffor = new ValueBuffor(discreteTransferFunction.nomFactors.Length + discreteTransferFunction.timeDelay, uInitialValue);
            ValueBuffor outputBuffor = new ValueBuffor(discreteTransferFunction.denFactors.Length - 1, yInitialValue);

            //Wektor sygnalow wyjsciowych
            Double[] outputVector = new Double[inputVector.Length];

            //Normalizacja wspolycznnikow licznika i mianownika - tak aby wspolczynnik a0 mial wartosc 1
            Double[] normalizedNomFactors = NormalizedNomFactors();
            Double[] normalizedDenFactors = NormalizedDenFactors();

            //Wyznaczenie kolejnych wartosci sygnalow wyjsciowych
            for (int i = 0; i < outputVector.Length; i++)
            {
                //Inicjalizacja wartosci nowo wyliczonego wyjscia
                Double newOutputValue = 0;

                //Dodanie nowej wartosci wejsciowej do bufora ich przejmujacego
                inputBuffor.AddNewValue(inputVector[i]);

                //Wyzaczenie wartosci od wspolczynnikow licznika ( sygnalu U )
                for (int j = 0; j < discreteTransferFunction.nomFactors.Length; j++)
                {
                    newOutputValue += normalizedNomFactors[j] * inputBuffor[j + discreteTransferFunction.timeDelay];
                }

                //Wyznaczenie wartosci od wspolczynnikow mianownika ( sygnal Y )
                for (int j = 1; j < discreteTransferFunction.denFactors.Length; j++)
                {
                    newOutputValue -= normalizedDenFactors[j] * outputBuffor[j - 1];
                }

                //Przypisanie nowej wartosci do ich kolekcji
                outputVector[i] = newOutputValue;

                if (discreteTransferFunction.denFactors.Length > 1)
                {
                    //Dodanie nowej wartosci sygnalu wyjsciowego do bufora
                    outputBuffor.AddNewValue(newOutputValue);
                }
            }

            return outputVector;
        }

        /// <summary>
        /// Metoda symulujaca uklad o podanej transmitancji dyskretnej
        /// </summary>
        /// <param name="observableCollection">
        /// Kolekcja przeznaczona do wspolpracy z mechanizmem wiazania danych WPF
        /// </param>
        /// <param name="inputVector">
        /// Wektor sygnalow wymuszenie
        /// </param>
        public void SimulateObservable(ChartPointObservableCollection observableCollection, Double[] inputVector)
        {
            //Wyczyszczenie wartosci odpowiedzi ukladu
            observableCollection.y.Clear();

            //Stworzenie buforow dla wartosci wejsciowych i wyjsciowych - musza posiadac odpowiednie dlugosci uwzgledniajaca rzad i opoznienie obiektu
            ValueBuffor inputBuffor = new ValueBuffor(discreteTransferFunction.nomFactors.Length + discreteTransferFunction.timeDelay, 0);
            ValueBuffor outputBuffor = new ValueBuffor(discreteTransferFunction.denFactors.Length - 1, 0);

            //Normalizacja wspolycznnikow licznika i mianownika - tak aby wspolczynnik a0 mial wartosc 1
            Double[] normalizedNomFactors = NormalizedNomFactors();
            Double[] normalizedDenFactors = NormalizedDenFactors();

            //Wyznaczenie kolejnych wartosci sygnalow wyjsciowych
            for (int i = 0; i < inputVector.Length; i++)
            {
                //Inicjalizacja wartosci nowo wyliczonego wyjscia
                Double newOutputValue = 0;

                //Dodanie nowej wartosci wejsciowej do bufora ich przejmujacego
                inputBuffor.AddNewValue(inputVector[i]);

                //Wyzaczenie wartosci od wspolczynnikow licznika ( sygnalu U )
                for (int j = 0; j < discreteTransferFunction.nomFactors.Length; j++)
                {
                    newOutputValue += normalizedNomFactors[j] * inputBuffor[j + discreteTransferFunction.timeDelay];
                }

                //Wyznaczenie wartosci od wspolczynnikow mianownika ( sygnal Y )
                for (int j = 1; j < discreteTransferFunction.denFactors.Length; j++)
                {
                    newOutputValue -= normalizedDenFactors[j] * outputBuffor[j - 1];
                }

                //Przypisanie nowej wartosci do ich kolekcji
                observableCollection.y.Add(new DataPoint(i * discreteTransferFunction.sampleTime, newOutputValue));

                //Dodanie nowej wartosci sygnalu wyjsciowego do bufora
                outputBuffor.AddNewValue(newOutputValue);
            }

        }

        /// <summary>
        /// Metoda normalizujaca wspolczynniki mianownika - przeksztalcajac transmitancje tak aby wspolczynnik mianownika rzedu 0 byl rowny 1
        /// </summary>
        /// <returns>
        /// Kolekcja znormalizowanych wspolczynnikow mianownika
        /// </returns>
        private Double[] NormalizedDenFactors()
        {
            //Stworzenie nowej tablicy znormalizowanych wspolczynnikow
            Double[] normalizedDenFactors = new Double[discreteTransferFunction.denFactors.Length];

            //Dzielimi kazdy ze wspolczynnikow przez wspolczynnik zerowy
            for(int i=0; i<normalizedDenFactors.Length; i++)
            {
                normalizedDenFactors[i] = discreteTransferFunction.denFactors[i]/discreteTransferFunction.denFactors[0];
            }

            return normalizedDenFactors;
        }

        /// <summary>
        /// Metoda normalizujaca wspolczynniki mianownika - przeksztalcajac transmitancje tak aby wspolczynnik mianownika rzedu 0 byl rowny 1
        /// </summary>
        /// <returns>
        /// Kolekcja znormalizowanych wspolczynnikow mianownika
        /// </returns>
        private static Double[] NormalizedDenFactors(DiscreteTransferFunction discreteTransferFunction)
        {
            //Stworzenie nowej tablicy znormalizowanych wspolczynnikow
            Double[] normalizedDenFactors = new Double[discreteTransferFunction.denFactors.Length];

            //Dzielimi kazdy ze wspolczynnikow przez wspolczynnik zerowy
            for (int i = 0; i < normalizedDenFactors.Length; i++)
            {
                normalizedDenFactors[i] = discreteTransferFunction.denFactors[i] / discreteTransferFunction.denFactors[0];
            }

            return normalizedDenFactors;
        }

        /// <summary>
        /// Metoda normalizujaca wspolczynniki licznika - przeksztalcajac transmitancje tak aby wspolczynnik mianownika rzedu 0 byl rowny 1
        /// </summary>
        /// <returns>
        /// Kolekcja znormalizowanych wspolczynnikow licznika
        /// </returns>
        private Double[] NormalizedNomFactors()
        {
            //Stworzenie nowej tablicy znormalizowanych wspolczynnikow
            Double[] normalizedNomFactors = new Double[discreteTransferFunction.nomFactors.Length];

            //Dzielimi kazdy ze wspolczynnikow przez wspolczynnik zerowy mianownika
            for (int i = 0; i < normalizedNomFactors.Length; i++)
            {
                normalizedNomFactors[i] = discreteTransferFunction.nomFactors[i] / discreteTransferFunction.denFactors[0];
            }

            return normalizedNomFactors;
        }

        /// <summary>
        /// Metoda normalizujaca wspolczynniki licznika - przeksztalcajac transmitancje tak aby wspolczynnik mianownika rzedu 0 byl rowny 1
        /// </summary>
        /// <returns>
        /// Kolekcja znormalizowanych wspolczynnikow licznika
        /// </returns>
        private static Double[] NormalizedNomFactors(DiscreteTransferFunction discreteTransferFunction)
        {
            //Stworzenie nowej tablicy znormalizowanych wspolczynnikow
            Double[] normalizedNomFactors = new Double[discreteTransferFunction.nomFactors.Length];

            //Dzielimi kazdy ze wspolczynnikow przez wspolczynnik zerowy mianownika
            for (int i = 0; i < normalizedNomFactors.Length; i++)
            {
                normalizedNomFactors[i] = discreteTransferFunction.nomFactors[i] / discreteTransferFunction.denFactors[0];
            }

            return normalizedNomFactors;
        }

        /// <summary>
        /// Metoda symulujaca wystąpienie zakłócenia w układzie zamkniętym z podanym regulatorem
        /// </summary>
        /// <param name="disturbance">
        /// Tablica wartosci zaklocen
        /// </param>
        /// <param name="controller">
        /// Transmitancja dyskretna regulatora
        /// </param>
        /// <param name="dynamicSystem">
        /// Transmitancja dyskretna obiektu regulacji
        /// </param>
        /// <returns>
        /// Tablica wartosci mierzonej układu
        /// </returns>
        public static Double[] SimulateCloseLoopDisturbance(Double[] disturbance,DiscreteTransferFunction controller, DiscreteTransferFunction dynamicSystem)
        {
            //Obie transmitancje powinny miec takie same czasy probkowania
            if(controller.sampleTime != dynamicSystem.sampleTime)
            {
                throw new InvalidOperationException("Transfer functions have different sample times");
            }

            //Stworzenie tablicy sygnalów wyjściowych
            Double[] y = new Double[disturbance.Length];

            //
            // y(z)            NomObject x DenController x z ^ (-dObject)                                                   nom
            //------ =  -------------------------------------------------------------------------------------- =    --------------------- z^(-dObject)
            // z(z)     DenObject x DenController - NomObject x (-NomController) x z ^ (-dObject-dController)             den2 - den1

            //Przemnozenie przez -1 wspolczynnikow licznika transmitancji regulatora - ujemne sprzezenie zwrotne
            Double[] controllerNom = new Double[controller.nomFactors.Length];

            for (int i = 0; i < controllerNom.Length; i++)
            {
                controllerNom[i] = -controller.nomFactors[i];
            }

            //Wyznaczenie wyzej wymienonych wspolczynnikow transmitancji
            Double[] nom = TransferFunctionCalculator.CrossFactors(dynamicSystem.nomFactors,controller.denFactors);
            Double[] den1 = TransferFunctionCalculator.MoveFactors(TransferFunctionCalculator.CrossFactors(dynamicSystem.nomFactors, controllerNom), dynamicSystem.timeDelay + controller.timeDelay);
            Double[] den2 = TransferFunctionCalculator.CrossFactors(dynamicSystem.denFactors, controller.denFactors);

            //Dodanie wspolczynnikow mianownika
            int length = den1.Length > den2.Length ? den1.Length : den2.Length;
            
            Double[] den = new Double[length];

            for(int i=0; i<den.Length; i++)
            {
                if(i<den1.Length)
                {
                    den[i] -= den1[i];
                }

                if (i < den2.Length)
                {
                    den[i] += den2[i];
                }
            }

            //Symulacja stworzonej transmitancji
            DiscreteTransferFunctionSimulator simulator = new DiscreteTransferFunctionSimulator(new DiscreteTransferFunction(nom, den, dynamicSystem.timeDelay, dynamicSystem.sampleTime));

            return simulator.Simulate(disturbance);
        }

        /// <summary>
        /// Metoda symulujaca wystąpienie zakłócenia w układzie zamkniętym z podanym regulatorem
        /// </summary>
        /// <param name="observableCollection">
        /// Kolekcja wartosci odpowiedzi przyznaczona do wspolpracy z mechanizmem wiazania danych WPF
        /// </param>
        /// <param name="disturbance">
        /// Tablica wartosci zaklocen
        /// </param>
        /// <param name="controller">
        /// Transmitancja dyskretna regulatora
        /// </param>
        /// <param name="dynamicSystem">
        /// Transmitancja dyskretna obiektu regulacji
        /// </param>
        public static void SimulateCloseLoopDisturbanceObservable(ChartPointObservableCollection observableCollection, Double[] disturbance, DiscreteTransferFunction controller, DiscreteTransferFunction dynamicSystem)                                                                           
        {
            //Obie transmitancje powinny miec takie same czasy probkowania
            if (controller.sampleTime != dynamicSystem.sampleTime)
            {
                throw new InvalidOperationException("Transfer functions have different sample times");
            }

            //
            // y(z)            NomObject x DenController x z ^ (-dObject)                                                   nom
            //------ =  -------------------------------------------------------------------------------------- =    --------------------- z^(-dObject)
            // z(z)     DenObject x DenController - NomObject x (-NomController) x z ^ (-dObject-dController)             den2 - den1

            //Przemnozenie przez -1 wspolczynnikow licznika transmitancji regulatora - ujemne sprzezenie zwrotne
            Double[] controllerNom = new Double[controller.nomFactors.Length];

            for (int i = 0; i < controllerNom.Length; i++)
            {
                controllerNom[i] = -controller.nomFactors[i];
            }

            //Wyznaczenie wyzej wymienonych wspolczynnikow transmitancji
            Double[] nom = TransferFunctionCalculator.CrossFactors(dynamicSystem.nomFactors, controller.denFactors);
            Double[] den1 = TransferFunctionCalculator.MoveFactors(TransferFunctionCalculator.CrossFactors(dynamicSystem.nomFactors, controllerNom), dynamicSystem.timeDelay + controller.timeDelay);
            Double[] den2 = TransferFunctionCalculator.CrossFactors(dynamicSystem.denFactors, controller.denFactors);

            //Dodanie wspolczynnikow mianownika
            int length = den1.Length > den2.Length ? den1.Length : den2.Length;

            Double[] den = new Double[length];

            for (int i = 0; i < den.Length; i++)
            {
                if (i < den1.Length)
                {
                    den[i] -= den1[i];
                }

                if (i < den2.Length)
                {
                    den[i] += den2[i];
                }
            }

            //Symulacja stworzonej transmitancji
            DiscreteTransferFunctionSimulator simulator = new DiscreteTransferFunctionSimulator(new DiscreteTransferFunction(nom, den, dynamicSystem.timeDelay, dynamicSystem.sampleTime));

            //Symulacja stworzonego obiektu 
            simulator.SimulateObservable(observableCollection, disturbance);
        }

        /// <summary>
        /// Metoda symulujaca wystąpienie zmiany sygnalu zadanego w układzie zamkniętym z podanym regulatorem
        /// </summary>
        /// <param name="setpoints">
        /// Tablica wartosci sygnalu zadanego
        /// </param>
        /// <param name="controller">
        /// Transmitancja dyskretna regulatora
        /// </param>
        /// <param name="dynamicSystem">
        /// Transmitancja dyskretna obiektu regulacji
        /// </param>
        /// <returns>
        /// Tablica wartosci mierzonej
        /// </returns>
        public static Double[] SimulateCloseLoopWithController(Double[] setpoints, DiscreteTransferFunction controller, DiscreteTransferFunction dynamicSystem)
        {
            //Obie transmitancje powinny miec takie same czasy probkowania
            if (controller.sampleTime != dynamicSystem.sampleTime)
            {
                throw new InvalidOperationException("Transfer functions have different sample times");
            }

            //
            // y(z)            NomObject x NomController x z ^ (-dObject-dController)                                       nom x z ^ (-dObject-dController) 
            //------ =  -------------------------------------------------------------------------------------- =    --------------------- z^(-dObject)
            // ys(z)     DenObject x DenController + NomObject x NomController x z ^ (-dObject-dController)             den2 + den1

            //Wyznaczenie wyzej wymienonych wspolczynnikow transmitancji
            Double[] nom = TransferFunctionCalculator.CrossFactors(dynamicSystem.nomFactors, controller.nomFactors);
            Double[] den1 = TransferFunctionCalculator.MoveFactors(TransferFunctionCalculator.CrossFactors(dynamicSystem.nomFactors, controller.nomFactors), dynamicSystem.timeDelay + controller.timeDelay);
            Double[] den2 = TransferFunctionCalculator.CrossFactors(dynamicSystem.denFactors, controller.denFactors);

            //Dodanie wspolczynnikow mianownika
            int length = den1.Length > den2.Length ? den1.Length : den2.Length;

            Double[] den = new Double[length];

            for (int i = 0; i < den.Length; i++)
            {
                if (i < den1.Length)
                {
                    den[i] += den1[i];
                }

                if (i < den2.Length)
                {
                    den[i] += den2[i];
                }
            }

            //Symulacja stworzonej transmitancji
            DiscreteTransferFunctionSimulator simulator = new DiscreteTransferFunctionSimulator(new DiscreteTransferFunction(nom, den, dynamicSystem.timeDelay+controller.timeDelay, dynamicSystem.sampleTime));

            return simulator.Simulate(setpoints);
        }

        /// <summary>
        /// Metoda symulujaca wystąpienie zmiany sygnalu zadanego w układzie zamkniętym z podanym regulatorem
        /// </summary>
        /// <param name="observableCollection">Kolekcja wartosci odpowiedzi przyznaczona do wspolpracy z mechanizmem wiazania danych WPF</param>
        /// <param name="setpoints">Tablica wartosci sygnalu zadanego</param>
        /// <param name="controller">Transmitancja dyskretna regulatora</param>
        /// <param name="dynamicSystem">Tablica wartosci mierzonej</param>
        public static void SimulateCloseLoopWithControllerObservable(ChartPointObservableCollection observableCollection, Double[] setpoints, DiscreteTransferFunction controller, DiscreteTransferFunction dynamicSystem)
        {
            //Obie transmitancje powinny miec takie same czasy probkowania
            if (controller.sampleTime != dynamicSystem.sampleTime)
            {
                throw new InvalidOperationException("Transfer functions have different sample times");
            }

            //
            // y(z)            NomObject x NomController x z ^ (-dObject-dController)                                       nom x z ^ (-dObject-dController) 
            //------ =  -------------------------------------------------------------------------------------- =    --------------------- z^(-dObject)
            // ys(z)     DenObject x DenController + NomObject x NomController x z ^ (-dObject-dController)             den2 + den1

            //Wyznaczenie wyzej wymienonych wspolczynnikow transmitancji
            Double[] nom = TransferFunctionCalculator.CrossFactors(dynamicSystem.nomFactors, controller.nomFactors);
            Double[] den1 = TransferFunctionCalculator.MoveFactors(TransferFunctionCalculator.CrossFactors(dynamicSystem.nomFactors, controller.nomFactors), dynamicSystem.timeDelay + controller.timeDelay);
            Double[] den2 = TransferFunctionCalculator.CrossFactors(dynamicSystem.denFactors, controller.denFactors);

            //Dodanie wspolczynnikow mianownika
            int length = den1.Length > den2.Length ? den1.Length : den2.Length;

            Double[] den = new Double[length];

            for (int i = 0; i < den.Length; i++)
            {
                if (i < den1.Length)
                {
                    den[i] += den1[i];
                }

                if (i < den2.Length)
                {
                    den[i] += den2[i];
                }
            }

            //Symulacja stworzonej transmitancji
            DiscreteTransferFunctionSimulator simulator = new DiscreteTransferFunctionSimulator(new DiscreteTransferFunction(nom, den, dynamicSystem.timeDelay + controller.timeDelay, dynamicSystem.sampleTime));

            //Symulacja stworzonego obiektu
            simulator.SimulateObservable(observableCollection, setpoints);
        }
    }

    /// <summary>
    /// Klasa bufora przechowujacego wartosci
    /// </summary>
    class ValueBuffor
    {
        /// <summary>
        /// Tablica w ktorej przechowywane sa wartosci
        /// </summary>
        private Double[] values;

        /// <summary>
        /// Konstruktor klasy bufora
        /// </summary>
        /// <param name="length">
        /// Długość bufora
        /// </param>
        /// <param name="initialValue">
        /// Wartość początkowa
        /// </param>
        public ValueBuffor(int length, Double initialValue)
        {
            //Tworzenie tablicy
            values = new Double[length];

            //Inicjalizuje wartosci poczatkowe
            Reset(initialValue);
        }

        /// <summary>
        /// Metoda resetujaca bufor wartosciami poczatkujacymi
        /// </summary>
        /// <param name="initialValue">
        /// Wartosc ktora jest inicjalizowana bufor
        /// </param>
        public void Reset(Double initialValue)
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = initialValue;
            }
        }

        /// <summary>
        /// Metoda dodajaca wartosc do bufora - reszta jest przewijana
        /// </summary>
        /// <param name="Value">
        /// Wartosc dodawana do bufora
        /// </param>
        public void AddNewValue(Double Value)
        {
            //Przewiniecie bufora
            for (int i = values.Length-1; i > 0; i--)
            {
                values[i] = values[i-1];
            }

            //Dodanie nowej wartosci
            values[0]=Value;
        }

        /// <summary>
        /// Indeksator
        /// </summary>
        /// <param name="index">
        /// Index
        /// </param>
        /// <returns>
        /// Wartosc
        /// </returns>
        public Double this[int index]
        {
            get
            {
                return this.values[index];
            }

            private set
            {
                this.values[index] = value;
            }

        }

    }


}
