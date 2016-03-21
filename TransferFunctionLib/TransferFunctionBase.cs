using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferFunctionLib
{
    /// <summary>
    /// Klasa abstrakcyjna reprezentujaca transmitancje
    /// </summary>
    internal abstract class TransferFunctionBase
    {
        /// <summary>
        /// Wspolczynniki mianownika transmitancji
        /// </summary>
        public Double[] denFactors;

        /// <summary>
        /// Wspolczynniki licznika transmitancji
        /// </summary>
        public Double[] nomFactors;
    }
}
