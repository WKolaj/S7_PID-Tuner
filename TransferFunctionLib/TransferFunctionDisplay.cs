using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TransferFunctionLib
{
    /// <summary>
    /// Typ wyliczeniowy okreslajacy rodzaj wykorzystywanej transmitancji
    /// </summary>
    public enum SystemType
    {
        Continues,Discrete
    }

    /// <summary>
    /// Klasa reprezentujaca obiekty umozliwiajace wyswietlanie parametrow ukladu dynamicznego
    /// </summary>
    internal class TransferFunctionDisplay
    {
        /// <summary>
        /// Metoda konwertujaca czlon opozniajacy na czlon do wyswietlania
        /// </summary>
        /// <param name="TimeDelay">
        /// Czas opznienia
        /// </param>
        /// <returns>
        /// Czlon postaci eˢ
        /// </returns>
        public static String ConvertContinuesTimeDelayToString(Double TimeDelay)
        {
            TimeDelay *= -1;
            StringBuilder factorsStringBuilder = new StringBuilder();

            factorsStringBuilder.Append("e" + SubscriptNumber(TimeDelay) + "ˢ");

            return factorsStringBuilder.ToString();
        }

        /// <summary>
        /// Metoda konwertujaca czlon opozniajacy na czlon do wyswietlania 
        /// </summary>
        /// <param name="TimeDelay">
        /// Czas opoznienia
        /// </param>
        /// <returns>
        /// Czlon postaci z⁻
        /// </returns>
        public static String ConvertDiscreteTimeDelayToString(Double TimeDelay)
        {
            TimeDelay *= -1;
            StringBuilder factorsStringBuilder = new StringBuilder();

            factorsStringBuilder.Append("z" + SubscriptNumber(TimeDelay));

            return factorsStringBuilder.ToString();
        }

        /// <summary>
        /// Metoda konwertujaca wspolczynniki transmitancji ciaglej do wyswietlania 
        /// </summary>
        /// <param name="factors">
        /// Wspolczynniki transmitancji ciaglej
        /// </param>
        /// <returns>
        /// ciag znakow typu s⁵ + s⁶ +..
        /// </returns>
        public static String ConvertContinuesFactorsToString(Double[] factors)
        {
            StringBuilder factorsStringBuilder = new StringBuilder();

            //Dodawanie do tworzonego lacucha wszystkich znakow z odpowiadajacymi im superskryptami w odwrotnej kolejnosci
            for (int i = factors.Length - 1; i > 0; i--)
            {
                factorsStringBuilder.Append(factors[i].ToString("G4") + "s" + SubscriptNumber(i) + " + ");
            }

            //Dodanie ostatniego elementu - ostatnie nie posiada s bo potega jest zero
            factorsStringBuilder.Append(factors[0].ToString("G4"));

            return factorsStringBuilder.ToString();
        }

        /// <summary>
        /// Metoda konwertujaca wspolczynniki transmitancji dyskretnej do wyswietlania 
        /// </summary>
        /// <param name="factors">
        /// Wspolczynniki transmitancji dyskretnej
        /// </param>
        /// <returns>
        /// ciag znakow typu z⁵ + z⁶ +..
        /// </returns>
        public static String ConvertDiscreteFactorsToString(Double[] factors)
        {
            StringBuilder factorsStringBuilder = new StringBuilder();

            //Dodawanie do tworzonego lacucha wszystkich znakow z odpowiadajacymi im superskryptami w odwrotnej kolejnosci
            for (int i = factors.Length - 1; i > 0; i--)
            {
                factorsStringBuilder.Append(factors[i].ToString("G4") + "z⁻" + SubscriptNumber(i) + " + ");
            }

            //Dodanie ostatniego elementu - ostatnie nie posiada s bo potega jest zero
            factorsStringBuilder.Append(factors[0].ToString("G4"));

            return factorsStringBuilder.ToString();
        }

        /// <summary>
        /// Metoda konwerujaca numer na superskrypt
        /// </summary>
        /// <param name="number">
        /// Numer
        /// </param>
        /// <returns>
        /// Superskrypt
        /// </returns>
        private static string SubscriptNumber(int number)
        {
            var res = new StringBuilder();
            string normal = number.ToString();
            //Konwersja kazdej z cyfr - zgodnie z kodem Unicode
            foreach (var c in normal)
            {
                char c1 = c;

                if (c == '0' || c >= '4' && c <= '9')
                {
                    c1 = (char)(c - '0' + 0x2070);
                }
                else if (c >= '2' && c <= '3')
                {
                    c1 = (char)(c - '0' + 0x00B0);
                }
                else
                {
                    c1 = (char)(0x00B9);
                }
                res.Append(c1);
            }

            return res.ToString();
        }

        /// <summary>
        /// Metoda konwerujaca znak na superskrypt
        /// </summary>
        /// <param name="numberChar">
        /// Konwertowany znak
        /// </param>
        /// <returns>
        /// Superskrypt
        /// </returns>
        private static string SubscriptNumber(char numberChar)
        {
                char c = numberChar;
                char c1 = numberChar;
                //Konwersja znaku - zgodnie z kodem Unicode
                if (c == '0' || c >= '4' && c <= '9')
                {
                    c1 = (char)(c - '0' + 0x2070);
                }
                else if (c >= '2' && c <= '3')
                {
                    c1 = (char)(c - '0' + 0x00B0);
                }
                else
                {
                    c1 = (char)(0x00B9);
                }

          
            return c1.ToString();
        }

        /// <summary>
        /// Konwersja liczby zmiennoprzecinkowej na superskrypt
        /// </summary>
        /// <param name="number">
        /// Liczba zmiennoprzecinkowa
        /// </param>
        /// <returns>
        /// Skrypt
        /// </returns>
        private static string SubscriptNumber(Double number)
        {
            var res = new StringBuilder();
            string normal = number.ToString("G4");
            foreach (var c in normal)
            {
                //Konwersja cyfry
                if(c >= '0' && c <= '9')
                {
                    res.Append(SubscriptNumber(c));
                }
                //Konwersja minusa
                else if(c == '-')
                {
                    res.Append('⁻');
                }
                //Konwersja formatu wykladniczego
                else if(c=='E')
                {
                    res.Append('ᴱ');
                }
                else if(c=='e')
                {
                    res.Append('ᵉ');
                }
                //Konwersja przecinku lub kropki
                else if(c==',')
                {
                    res.Append('·');
                }
                else if (c == '.')
                {
                    res.Append('·');
                }
            }
     
            return res.ToString();
        }

        /// <summary>
        /// Metoda przeksztalcajaca kolekcje wspolczynnikow na reprezentujacy je lancuch znakow
        /// </summary>
        /// <param name="factors">
        /// Wspolczynniki
        /// </param>
        /// <returns>
        /// Lancuch znakow postaci
        /// [ a0 a1 .. an ]
        /// </returns>
        public static string FactorsToVectorString(Double[] factors)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[ ");
            foreach (var factor in factors)
            {
                builder.Append(factor + " ");
            }

            builder.Append("]");

            return builder.ToString();
        }

        /// <summary>
        /// Metoda wyznaczajaca kolekcje wspolczynnikow na podstawie reprezentujacego jej lancucha znakow
        /// </summary>
        /// <param name="factorString">
        /// lancuch znakow wspolczynnikow postaci
        /// [ a0 a1 .. an ]
        /// </param>
        /// <returns>
        /// Kolekcja wspolczynnikow
        /// </returns>
        public static Double[] FactorsFromString(String factorString)
        {
            List<Double> factorList = new List<Double>();

            //Sprawdzenie czy lanuch znakow nie jest pusty
            if(String.IsNullOrEmpty(factorString))
            {
                throw new InvalidCastException("Factors string cannot be empty");
            }

            //Sprawdzenie czy lancuch znakow ma skarjnie nawiasy kwadratowe
            if(factorString.First() != '[' || factorString.Last() != ']' )
            {
                throw new InvalidCastException("Invalid string format");
            }

            //Pozbycie sie nawiasow kwadratowych i spacji skrajnych
            string trimedString = (factorString.Trim('[', ']')).Trim(' ');

            if (trimedString.Length == 0)
            {
                throw new InvalidCastException("Invalid string format");
            }

            //Podzielenie wspolczynnikow wzgledem spacji
            String[] Values = trimedString.Split(' ');

            //Konwersja kazdego ze wspolczynnikow
            foreach(var value in Values)
            {
                Double temp;

                try
                {
                    temp = Convert.ToDouble(value.Replace('.',','));
                }
                catch(Exception e)
                {
                    throw new InvalidCastException("Invalid string format", e);
                }

                factorList.Add(temp);
            }

            return factorList.ToArray();

        }

    }

}
