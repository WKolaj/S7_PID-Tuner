using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace TransferFunctionLib
{
    /// <summary>
    /// Klasa elementu wyznaczajcaego miejsca zerowe wielomianu
    /// </summary>
    internal class RootFinder
    {
        /// <summary>
        /// Maksymalna liczba iteracji Laguerre
        /// </summary>
        private readonly long maxIteration;

        /// <summary>
        /// Dokładność wyszukiwania pierwiastków
        /// </summary>
        public Double Accuracy
        {
            get;
            private set;
        }

        /// <summary>
        /// Domyslny punkt startowy metody Laguerre
        /// </summary>
        private Complex startPoint;

        /// <summary>
        /// Wspolczynniki wielomianu
        /// </summary>
        private Complex[] polynomialFactors;

        /// <summary>
        /// Obiekt wyznaczajacy miejsca zerowe wielomianu
        /// </summary>
        /// <param name="polynomialFactors">
        /// Wspolczynniki wielomianu
        /// </param>
        /// <param name="accuracy">
        /// Dokladność wyznaczania
        /// </param>
        /// <param name="maxNumberOfIteration">
        /// Maksymalna liczba iteracji metody Laguerre
        /// </param>
        public RootFinder(Complex[] polynomialFactors, Double accuracy = 1E-6,long maxNumberOfIteration = 1000, Double startPoint = 0.0)
        {
            this.polynomialFactors = NormalizeFactors(polynomialFactors);
            this.maxIteration = maxNumberOfIteration;
            this.Accuracy = accuracy;
            this.startPoint = new Complex(startPoint, 0);
        }

        /// <summary>
        /// Obiekt wyznaczajacy miejsca zerowe wielomianu
        /// </summary>
        /// <param name="polynomialFactors">
        /// Wspolczynniki wielomianu
        /// </param>
        public RootFinder(Double[] polynomialFactors, Double accuracy = 1E-6, long maxNumberOfIteration = 1000, Double startPoint = 0.0)
        {
            this.polynomialFactors = NormalizeFactors(polynomialFactors);
            this.maxIteration = maxNumberOfIteration;
            this.Accuracy = accuracy;
            this.startPoint = new Complex(startPoint, 0);

        }

        /// <summary>
        /// Metoda normalizująca wspolczynniki wielomianu - usuwajaca zerowe wspolczynniki z najwyzszych poteg
        /// </summary>
        /// <param name="factors">
        /// Wspolczynniki wielomianu
        /// </param>
        /// <returns>
        /// Znormalizowane wspolczynniki wielomianu
        /// </returns>
        private Complex[] NormalizeFactors(Double[] factors)
        {
            //Sprawdzenie czy podany wektor jest pusty
            if (factors.Length == 0)
            {
                throw new InvalidOperationException("factors cannot be empty!");
            }

            //Sprawdzenie czy wszystkie elementy są puste
            int zeroFactorsNumber = (from factor in factors
                               where factor == 0
                               select factor).Count();

            if (zeroFactorsNumber == factors.Length)
            {
                throw new InvalidOperationException("factors cannot have all zero elements");
            }

            //Wyszukanie liczby elementow zerowych w najwyzszych potegach
            int lastEmptyFactors = 0;

            for (int i = factors.Length - 1; factors[i] == 0; i--)
            {
                lastEmptyFactors++;
            }

            //Znalezione elementy zerowe nalezy pominac przy przepisywaniu
            Complex[] normalizedFactors = new Complex[factors.Length - lastEmptyFactors];

            for (int i = 0; i < normalizedFactors.Length; i++)
            {
                normalizedFactors[i] = new Complex(factors[i],0);
            }

            return normalizedFactors;
        }

        /// <summary>
        /// Metoda normalizująca wspolczynniki wielomianu - usuwajaca zerowe wspolczynniki z najwyzszych poteg
        /// </summary>
        /// <param name="factors">
        /// Wspolczynniki wielomianu
        /// </param>
        /// <returns>
        /// Znormalizowane wspolczynniki wielomianu
        /// </returns>
        private Complex[] NormalizeFactors(Complex[] factors)
        {
            //Sprawdzenie czy podany wektor jest pusty
            if (factors.Length == 0)
            {
                throw new InvalidOperationException("factors cannot be empty!");
            }

            //Sprawdzenie czy wszystkie elementy są puste
            int zeroFactorsNumber = (from factor in factors
                                     where factor == Complex.Zero
                                     select factor).Count();

            if (zeroFactorsNumber == factors.Length)
            {
                throw new InvalidOperationException("factors cannot have all zero elements");
            }

            //Wyszukanie liczby elementow zerowych w najwyzszych potegach
            int lastEmptyFactors = 0;

            for (int i = factors.Length - 1; factors[i] == Complex.Zero; i--)
            {
                lastEmptyFactors++;
            }

            //Znalezione elementy zerowe nalezy pominac przy przepisywaniu
            Complex[] normalizedFactors = new Complex[factors.Length - lastEmptyFactors];

            for (int i = 0; i < normalizedFactors.Length; i++)
            {
                normalizedFactors[i] = factors[i];
            }

            return normalizedFactors;
        }
        
        /// <summary>
        /// Metoda wyliczajaca wartosc pochodnej wielomianu w danym punkcie
        /// </summary>
        /// <param name="x">
        /// Punkt w ktorym ma zostac wyliczona wartosc pochodnej
        /// </param>
        /// <param name="factors">
        /// Wspolczynniki wielomianu
        /// </param>
        /// <returns>
        /// Wartosc pochodnej w punkcie
        /// </returns>
        private Complex CalculateFirstDerivativeInX(Complex x, Complex[] factors)
        {
            Complex value = Complex.Zero;

            for (int i = factors.Length - 1; i > 0; i--)
            {
                value = value * x + i * factors[i];
            }

            return value;
        }

        /// <summary>
        /// Metoda wyliczajaca wartosc drugiej pochodnej wielomianu w danym punkcie
        /// </summary>
        /// <param name="x">
        /// Punkt w ktorym ma zostac wyliczona wartosc drugiej pochodnej
        /// </param>
        /// <param name="factors">
        /// Wspolczynniki wielomianu
        /// </param>
        /// <returns>
        /// Wartosc drugiej pochodnej w punkcie
        /// </returns>
        private Complex CalculateSecondDerivativeInX(Complex x, Complex[] factors)
        {
            Complex value = Complex.Zero;

            for (int i = factors.Length - 1; i > 1; i--)
            {
                value = value * x + i * (i - 1) * factors[i];
            }

            return value;
        }

        /// <summary>
        /// Metoda Laguerre obliczająca pojedyncze miejsca zerowe wielomianu
        /// </summary>
        /// <param name="startPoint">
        /// Punkt poczatkowy wyszukiwania
        /// </param>
        /// <param name="accuracy">
        /// 
        /// </param>
        /// <returns></returns>
        private Complex LaguerreMethod(Complex[] factors, Complex startPoint)
        {
            //Inicjalizacja miejsca zerowego
            Complex zero = startPoint;

            //Inicjalizacja rzedu wielomianu
            Complex n = new Complex(factors.Length - 1, 0);

            //Jezeli punktem poszukiwan jest 0 i jest ono rowniez miejscem zerowym wielomianu ( ostatni wspolczynnik = 0) mozna juz zwrocic 0 jako miejsce zerowe
            //Jest to nawet konieczne, poniewaz metoda Lagrange dla zerowego miejsca zerowego moze nie moc byc wykonana
            if (factors[0] == Complex.Zero && startPoint == 0)
            {
                return Complex.Zero;
            }

            //Sprawdzenie czy wartość startowa nie zeruje wielomianu
            Complex polyValue = TransferFunctionCalculator.Horner(zero,factors);

            if (Complex.Abs(polyValue) == 0)
            {
                return zero;
            }

            //Wykonanie pierwszego cyklu metody Laguerre
            Complex nominator = polyValue*n;

            Complex firstDer = CalculateFirstDerivativeInX(zero,factors);
            Complex secondDer = CalculateSecondDerivativeInX(zero, factors);

            Complex denominatorSqrt = Complex.Sqrt((n - 1) * ((n - 1) * (firstDer * firstDer) - nominator * secondDer));

            Complex denominatorMinus = firstDer - denominatorSqrt;
            Complex denominatorPlus = firstDer + denominatorSqrt;

            Complex denominator = (Complex.Abs(denominatorMinus) > Complex.Abs(denominatorPlus)) ? denominatorMinus : denominatorPlus;

            Complex newZero = zero - nominator / denominator;

            //Inicjalizacja zmiennej liczacej liczbe wykonanej operacji
            long numberOfIteration = 0;

            //Wykonywanie cyklu Laguerre do poki wyznaczona wartosc nie bedzie mniejsza od dokladnosci
            while ( Complex.Abs(newZero - zero) > Accuracy)
            {
                //Jezeli cykl wykonal sie juz 1000 razy - nalezy na nowo obrac punkt startowy, poniewaz mogl sie zawiesic
                if (numberOfIteration > maxIteration)
                {
                    Random rnd = new Random();
                    int real = rnd.Next(-1000, 1000);
                    int img = rnd.Next(-1000, 1000);
                    zero = new Complex(real, img);
                    numberOfIteration = 0;
                }
                else
                {
                    zero = newZero;
                }

                //Pobranie wartosci prawdopodonego zera
                polyValue = TransferFunctionCalculator.Horner(zero, factors);
                
                //Sprawdzenie wartosci prawdopobnego zera
                if (Complex.Abs(polyValue) < Accuracy)
                {
                    return zero;
                }

                nominator = n * polyValue;
                firstDer = CalculateFirstDerivativeInX(zero, factors);
                secondDer = CalculateSecondDerivativeInX(zero, factors);


                //Jezeli poszukiwanym miejscem jest 0 i wyraz wolny wielomianu jest rowny zero - zero dzieli wielomian
                if (factors[0] == Complex.Zero && zero == 0)
                {
                    return Complex.Zero;
                }

                //Cykl Laguerre
                denominatorSqrt = Complex.Sqrt((n - 1) * ((n - 1) * (firstDer * firstDer) - nominator * secondDer));
                denominatorMinus = firstDer - denominatorSqrt;
                denominatorPlus = firstDer + denominatorSqrt;

                denominator = (Complex.Abs(denominatorMinus) > Complex.Abs(denominatorPlus)) ? denominatorMinus : denominatorPlus;
                
                newZero = zero - nominator / denominator;

                //Inkrementacja cyklu
                numberOfIteration++;
            }

            return newZero;
        }

        /// <summary>
        /// Metoda pobierajaca zespolone miejsca zerowe wielomianu
        /// </summary>
        /// <returns>
        /// Tablica zespolonych miejsc zerowych
        /// </returns>
        public Complex[] GetRoots()
        {
            //Pobranie stopnia wielomianu 
            Int32 n = polynomialFactors.Length - 1;
            if (n <= 0)
            {
                return new Complex[0];
            }

            //Tyle powinno byc znalezionych pierwiastkow
            Complex[] roots = new Complex[n];

            Complex[] dividedPol = polynomialFactors;

            roots[0] = LaguerreMethod(polynomialFactors, startPoint);

            for (int i = 1; i < polynomialFactors.Length - 1; i++)
            {
                dividedPol = DividePolByRoot(dividedPol, roots[i - 1]);
                roots[i] = LaguerreMethod(dividedPol, startPoint);
                roots[i] = LaguerreMethod(polynomialFactors, roots[i]);
            }

            return roots;
        }

        /// <summary>
        /// Metoda pobierajaca rzeczywiste miejsca zerowe wielomianu
        /// </summary>
        /// <returns>
        /// Tablica rzeczywistych miejsc zerowych
        /// </returns>
        public Double[] GetRealRoots()
        {
            //Pobranie zespolonych miejsc zerowych
            Complex[] complexRoots = GetRoots();

            //Wybor tylko rzeczywistych miejsc zerowych
            return (from root in complexRoots
                   where Math.Abs(root.Imaginary) < Accuracy
                   select root.Real).ToArray();
        }

        /// <summary>
        /// Metoda dokonujaca deflacji wielomianu, dzielac go przez znalezione miejsce zerowe
        /// </summary>
        /// <param name="factors">
        /// Wspolczynniki wielomianu
        /// </param>
        /// <param name="root">
        /// pierwiastek
        /// </param>
        /// <returns>
        /// Wspolczynniki wielomianu po deflacji
        /// </returns>
        private static Complex[] DividePolByRoot(Complex[] factors, Complex root)
        {
            //Tablica nowych wspolczynnikow
            Complex[] newFactors = new Complex[factors.Length - 1];

            //Przepisanie pierwszego wspolczynnika
            newFactors[factors.Length - 2] = factors[factors.Length - 1];

            //Obliczenie reszty
            for (int i = factors.Length - 3; i >= 0; i--)
            {
                newFactors[i] = factors[i + 1] + newFactors[i + 1] * root;
            }

            return newFactors;

        }

        
    }
}
