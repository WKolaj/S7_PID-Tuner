using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TransferFunctionLib
{
    /// <summary>
    /// Klasa reprezentujaca obiekt serializujacy uklad dynamiczny
    /// </summary>
    internal class DynamicSystemXMLSerializer
    {
        /// <summary>
        /// Metoda konwertujaca uklad dynamiczny na reprezentujacy go obiekt jezyka XML
        /// </summary>
        /// <param name="ds">
        /// Uklad dynamiczny
        /// </param>
        /// <param name="type">
        /// Typ zapisu - transmitancja dyskretna/ciagla
        /// </param>
        /// <returns>
        /// obiekt jezyka XML
        /// </returns>
        public static XDocument DynamicSystemToXML(DynamicSystem ds, SystemType type)
        {
            XDocument doc = new XDocument();

            if (type == SystemType.Continues)
            {
                
                var nominator = new XElement("Nominator", Factor.FromDoubleFactors(ds.ContinousNumerator).Select(x => new XElement("Factor", x.Value.ToString("R"), new XAttribute("rank", x.Rank))));

                var denominator = new XElement("Denominator", Factor.FromDoubleFactors(ds.ContinousDenumerator).Select(x => new XElement("Factor", x.Value.ToString("R"), new XAttribute("rank", x.Rank))));

                var timeDelay = new XElement("TimeDelay", ds.ContinousTimeDelay.ToString("R"));

                var sampleTime = new XElement("SampleTime", ds.SimulationSampleTime.ToString("R"));

                doc.Add(new XElement("ContinousDynamicSystem", nominator, denominator, timeDelay, sampleTime));

            }
            else if (type == SystemType.Discrete)
            {

                var nominator = new XElement("Nominator", Factor.FromDoubleFactors(ds.DiscreteNumerator).Select(x => new XElement("Factor", x.Value.ToString("R"), new XAttribute("rank", x.Rank))));

                var denominator = new XElement("Denominator", Factor.FromDoubleFactors(ds.DiscreteDenumerator).Select(x => new XElement("Factor", x.Value.ToString("R"), new XAttribute("rank", x.Rank))));

                var timeDelay = new XElement("TimeDelay", ds.DiscreteTimeDelay.ToString("R"));

                var sampleTime = new XElement("SampleTime", ds.SimulationSampleTime.ToString("R"));

                doc.Add(new XElement("DiscreteDynamicSystem", nominator, denominator, timeDelay, sampleTime));

            }

            return doc;
        }

        /// <summary>
        /// Metoda tworzaca uklad dynamiczny na podstawie obiektu jezyka XML
        /// </summary>
        /// <param name="documentXML">
        /// Obiekt jezyka XML
        /// </param>
        /// <returns>
        /// Uklad dynamiczny
        /// </returns>
        public static DynamicSystem DynamicSystemFromXML(XDocument documentXML)
        {
            //Pobranie pierwszego elementu glownego
            XElement dynamicSystem = documentXML.Root;
            
            //Nie moze byc on rowny null
            if(dynamicSystem == null)
            {
                throw new InvalidOperationException("Wrong file ");
            }

            //Jezeli reprezentuje on zapisana transmitancje ciagla
            if(dynamicSystem.Name == "ContinousDynamicSystem" )
            {
                Double[] nominator = Factor.FromFactors((from factor in dynamicSystem.Element("Nominator").Elements("Factor")
                                                        select new Factor(Convert.ToDouble(factor.Value),Convert.ToInt32(factor.Attribute("rank").Value))).ToList());

                Double[] denominator = Factor.FromFactors((from factor in dynamicSystem.Element("Denominator").Elements("Factor")
                                                            select new Factor(Convert.ToDouble(factor.Value),Convert.ToInt32(factor.Attribute("rank").Value))).ToList());

                Double timeDelay = Convert.ToDouble(dynamicSystem.Element("TimeDelay").Value);

                Double sampleTime = Convert.ToDouble(dynamicSystem.Element("SampleTime").Value);
                
                return DynamicSystem.FromContinousTransferFuntion(nominator, denominator, timeDelay, sampleTime);

            }
            //Jezeli reprezentuje on zapisana transmitancje dyskretna
            else if(dynamicSystem.Name == "DiscreteDynamicSystem")
            {
                Double[] nominator = Factor.FromFactors((from factor in dynamicSystem.Element("Nominator").Elements("Factor")
                                                         select new Factor(Convert.ToDouble(factor.Value), Convert.ToInt32(factor.Attribute("rank").Value))).ToList());

                Double[] denominator = Factor.FromFactors((from factor in dynamicSystem.Element("Denominator").Elements("Factor")
                                                           select new Factor(Convert.ToDouble(factor.Value), Convert.ToInt32(factor.Attribute("rank").Value))).ToList());

                Int32 timeDelay = Convert.ToInt32(dynamicSystem.Element("TimeDelay").Value);

                Double sampleTime = Convert.ToDouble(dynamicSystem.Element("SampleTime").Value);

                return DynamicSystem.FromDiscreteTransferFuntion(nominator, denominator, timeDelay, sampleTime);
            }
            throw new InvalidOperationException("Wrong file - root name is "+dynamicSystem.Name);
        }
    }

    /// <summary>
    /// Klasa reprezentujaca wspolczynnik transmitancji
    /// </summary>
    internal class Factor
    {
        /// <summary>
        /// Wartosc wspolczynnika
        /// </summary>
        public Double Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Rzad wspolczynnika
        /// </summary>
        public Int32 Rank
        {
            get;
            private set;
        }

        /// <summary>
        /// Konstruktor klasy wspolczynnika transmitancji
        /// </summary>
        /// <param name="value">
        /// wartosc
        /// </param>
        /// <param name="rank">
        /// rzad
        /// </param>
        public Factor(Double value, Int32 rank)
        {
            this.Value = value;
            this.Rank = rank;
        }

        /// <summary>
        /// Metoda konwertujaca tablice wspolczynnikow typu double na liste wspolczynnikow Factors
        /// </summary>
        /// <param name="factors">
        /// Tablice wspolczynnikow
        /// </param>
        /// <returns>
        /// Lista obiektow typu Factors
        /// </returns>
        public static List<Factor> FromDoubleFactors(Double[] factors)
        {
            List<Factor> factorsList = new List<Factor>();

            for (int i = 0; i < factors.Length; i++)
            {
                factorsList.Add(new Factor(factors[i], i));
            }

            return factorsList;
        }

        /// <summary>
        /// Metoda zwracajaca tablice typu Double na podstawie listy wspolczynnikow typu Factor
        /// </summary>
        /// <param name="factors">
        /// lista wspolczynnikow typu factor
        /// </param>
        /// <returns>
        /// Kolekcja obiektow typu double
        /// </returns>
        public static Double[] FromFactors(List<Factor> factors)
        {
            int rank = (from factor in factors
                        select factor.Rank).Max();

            Double[] doubleFactors = new Double[rank + 1];

            for (int i = 0; i < factors.Count; i++)
            {
                doubleFactors[factors[i].Rank] = factors[i].Value;
            }

            return doubleFactors;
        }

    }
}