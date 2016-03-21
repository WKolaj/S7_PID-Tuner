using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferFunctionLib
{
    /// <summary>
    /// Klasa dyskretnej transmitancji
    /// </summary>
    internal class DiscreteTransferFunction : TransferFunctionBase
    {
        /// <summary>
        /// Obiekt symulatora, pozwalajacy wyznaczac odpowiedz na zadane wymuszenia
        /// </summary>
        private DiscreteTransferFunctionSimulator simulator;

        /// <summary>
        /// Dyskretny czas opoznienia
        /// </summary>
        public int timeDelay;

        /// <summary>
        /// Czas próbkowania
        /// </summary>
        public readonly Double sampleTime;

        /// <summary>
        /// Klasa dyskretnej transmitancji
        /// </summary>
        /// <param name="nomFactors">
        /// Wspolczynniki licznika (od zerowego)
        /// </param>
        /// <param name="denFactors">
        /// Wspolczynniki mianownika (od zerowego)
        /// </param>
        /// <param name="timeDelay">
        /// Dyskretne opóźnienie transportowe
        /// </param>
        /// <param name="sampleTime">
        /// Czas probkowania
        /// </param>
        public DiscreteTransferFunction(Double[] nomFactors, Double[] denFactors, int timeDelay, Double sampleTime)
        {
            //Jezeli podano puste tablice - nalezy zwrocic blad
            if(nomFactors.Length == 0)
            {
                throw new InvalidOperationException("Nominators factors cannot be empty");
            }

            if (denFactors.Length == 0)
            {
                throw new InvalidOperationException("Denominators factors cannot be empty");
            }

            if (sampleTime <= 0)
            {
                throw new InvalidOperationException("Invalid sample time value");
            }

            if (timeDelay < 0)
            {
                throw new InvalidOperationException("Invalid time delay value");
            }

            //Jezeli mianownik ma mniejszy rzad niz licznik - blad
            if(nomFactors.Length > denFactors.Length)
            {
                throw new InvalidOperationException("Denominators degree must be greater than nominators");
            }

            this.sampleTime = sampleTime;
            this.nomFactors = nomFactors;
            this.denFactors = denFactors;
            this.timeDelay = timeDelay;

            simulator = new DiscreteTransferFunctionSimulator(this);
        }

        /// <summary>
        /// Metoda wzynaczajaca odpowiedz ukladu na wymuszenie 
        /// </summary>
        /// <param name="inputVector">
        /// Wektor sygnalu wejsciowego
        /// </param>
        /// <returns>
        /// Wektor odpowiedzi ukladu
        /// </returns>
        public Double[] Simulate(Double[] inputVector)
        {
            return simulator.Simulate(inputVector);
        }

         /// <summary>
        /// Metoda wzynaczajaca odpowiedz ukladu na wymuszenie 
        /// </summary>
        /// <param name="inputVector">
        /// Wektor sygnalu wejsciowego
        /// </param>
        /// <returns>
        /// Wektor odpowiedzi ukladu
        /// </returns>
        public Double[] Simulate(Double[] inputVector, Double uInitialValue, Double yInitialValue)
        {
            return simulator.Simulate(inputVector, uInitialValue, yInitialValue); 
        }

        /// <summary>
        /// Metoda wzynaczajaca odpowiedz ukladu na wymuszenie 
        /// </summary>
        /// <param name="observableCollection">Kolekcja obiektu typu przeznaczonego do wspolpracy z mechanizmem wiazania danych WPF</param>
        /// <param name="inputVector">Wektor sygnalu wejsciowego</param>
        public void SimulateObservable(ChartPointObservableCollection observableCollection, Double[] inputVector)
        {
            simulator.SimulateObservable(observableCollection,inputVector);
        }

    }
}
