using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("TestConsoleApp")]

namespace TransferFunctionLib
{
    /// <summary>
    /// Klasa reprezentujaca transmitancje ciagłą
    /// </summary>
    internal class ContinousTransferFunction : TransferFunctionBase
    {
        /// <summary>
        /// Opóźnienie transportowe
        /// </summary>
        public Double timeDelay;

        /// <summary>
        /// Klasa transmitancji ciągłej
        /// </summary>
        /// <param name="nomFactors">
        /// Wspolczynniki licznika (od zerowego)
        /// </param>
        /// <param name="denFactors">
        /// Wspolczynniki mianownika (od zerowego)
        /// </param>
        /// <param name="timeDelay">
        /// Opóźnienie transportowe
        /// </param>
        public ContinousTransferFunction(Double[] nomFactors, Double[] denFactors, Double timeDelay)
        {
            //Jezeli podano puste tablice - nalezy zwrocic blad
            if (nomFactors.Length == 0)
            {
                throw new InvalidOperationException("Nominators factors cannot be empty");
            }

            if (denFactors.Length == 0)
            {
                throw new InvalidOperationException("Denominators factors cannot be empty");
            }

            //Bledem jest rowniez podanie transmitancji o liczniku wiekszym niz mianownik
            if(nomFactors.Length > denFactors.Length)
            {
                throw new InvalidOperationException("Denominators degree must be greater than nominators");
            }


            this.timeDelay = timeDelay;
            this.nomFactors = nomFactors;
            this.denFactors = denFactors;

        }

    }
}
