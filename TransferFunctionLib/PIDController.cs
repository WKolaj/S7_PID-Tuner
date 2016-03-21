using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TransferFunctionLib
{
    /// <summary>
    /// Typ algorytmu regulatora PID
    /// </summary>
    public enum PIDModeType
    {
        P=0,PD=1,PI=2,PID=3
    }

    public class PIDController:DynamicSystem
    {
        private PIDModeType mode;
        /// <summary>
        /// Tryb algorytmu regulatora PID - od niego zalezy tworzona transmitancja
        /// Kazda jego zmiana powoduje odswiezenie jego transmitancji
        /// </summary>
        public PIDModeType Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;

                //Odswiezenie transmitancji regulatora po ustawieniu nowego trybu
                RefreshTransferFunctions();
            }
        }

        private Double kp;
        /// <summary>
        /// Wzmocnienie regulatora
        /// </summary>
        public Double Kp
        {
            get
            {
                return kp;
            }
            set
            {
                kp = value;

                //Odswiezenie transmitancji regulatora po zmianie wartosci wzmocnienia
                RefreshTransferFunctions();
            }
        }

        private Double ti;
        /// <summary>
        /// Stala calkowania regulatora
        /// </summary>
        public Double Ti
        {
            get
            {
                return ti;
            }
            set
            {
                ti = value;
                //Odswiezenie transmitancji regulatora po zmianie stalej calkowania regulatora
                RefreshTransferFunctions();
            }
        }

        private Double td;
        /// <summary>
        /// Stala rozniczkowania regulatora
        /// </summary>
        public Double Td
        {
            get
            {
                return td;
            }
            set
            {
                td = value;
                //Odswiezenie transmitancji regulatora po zmianie stalej rozniczkowania regulatora
                RefreshTransferFunctions();
            }
        }

        private Double n;
        /// <summary>
        /// Wspolczynnik czlonu inercyjnego (N*Td) czesci rozniczkujacej
        /// </summary>
        public Double N
        {
            get
            {
                return n;
            }
            set
            {
                n = value;
                //Odswiezenie transmitancji regulatora po zmianie wartosci czlonu inercyjnego (N*Td) czesci rozniczkujacej
                RefreshTransferFunctions();
            }
        }

        private bool inverted;
        /// <summary>
        /// Tryb regulatora:
        /// True - Reverse -> e = PV - SP
        /// False - Normal -> e = SP - PV
        /// </summary>
        public bool Inverted
        {
            get
            {
                return inverted;
            }
            set
            {
                inverted = value;
                Debug.WriteLine("Odswiezam Tf");
                //Odswiezenie transmitancji regulatora po zmianie jego trybu
                RefreshTransferFunctions();
            }
        }

        /// <summary>
        /// Wyliczenie wspolczynnikow licznika tranmistancji ciaglej regulatora PID
        /// </summary>
        /// <param name="kp">
        /// Wzmocnienie regulatora
        /// </param>
        /// <param name="ti">
        /// Czas calkowania regulatora
        /// </param>
        /// <param name="td">
        /// Czas rozniczkowania regulatora
        /// </param>
        /// <param name="n">
        /// Stala czesci inercyjnej czlonu rozniczkujacego
        /// </param>
        /// <param name="mode">
        /// Tryb algorytmu regulatora
        /// </param>
        /// <param name="inverted">
        /// Tryb regulatora
        /// </param>
        /// <returns>
        /// Tablica wspolczynnikow licznika transmitancji ciaglej regulatora PID
        /// </returns>
        private static Double[] CalculateNominator(Double kp, Double ti, Double td, Double n, PIDModeType mode, bool inverted)
        {
            Double[] nominator;

            //Wspolczynniki sa rozne w zaleznosci od trybu regulatora
            switch (mode)
            {
                case PIDModeType.P:
                    {
                        nominator = new Double[1];
                        nominator[0] = kp;

                        break;
                    }
                case PIDModeType.PD:
                    {
                        nominator = new Double[2];
                        nominator[0] = kp;
                        nominator[1] = kp * td * (1 + (n));


                        break;
                    }
                case PIDModeType.PI:
                    {
                        nominator = new Double[2];
                        nominator[0] = kp;
                        nominator[1] = kp * ti;


                        break;
                    }
                case PIDModeType.PID:
                    {
                        nominator = new Double[3];
                        nominator[0] = kp;
                        nominator[1] = kp * (ti + (td * n));
                        nominator[2] = ti * td * kp * ((n) + 1);


                        break;
                    }
                default:
                    {
                        nominator = new Double[0];
                        break;
                    }
            }

            if(inverted)
            {
                for(int i=0; i<nominator.Length; i++)
                {
                    nominator[i] *= -1;
                }
            }

            return nominator;
        }

        /// <summary>
        /// Wyznaczenie wspolczynnikow transmitancji regulatora
        /// </summary>
        /// <returns>
        /// Wspolczynniki licznika transmitancji ciaglej regulatora
        /// </returns>
        private Double[] CalculateNominator()
        {
            return PIDController.CalculateNominator(Kp, Ti, Td, N, Mode, Inverted);
        }

        /// <summary>
        /// Wyliczenie wspolczynnikow mianownika tranmistancji ciaglej regulatora PID
        /// </summary>
        /// <param name="kp">
        /// Wzmocnienie regulatora
        /// </param>
        /// <param name="ti">
        /// Czas calkowania regulatora
        /// </param>
        /// <param name="td">
        /// Czas rozniczkowania regulatora
        /// </param>
        /// <param name="n">
        /// Stala czesci inercyjnej czlonu rozniczkujacego
        /// </param>
        /// <param name="mode">
        /// Tryb algorytmu regulatora
        /// </param>
        /// <returns>
        /// Tablica wspolczynnikow mianownika transmitancji ciaglej regulatora PID
        /// </returns>
        private static Double[] CalculateDenominator(Double kp, Double ti, Double td, Double n, PIDModeType mode)
        {
            Double[] denominator;

            //Transmitancja regualtora zalezy od jego trybu algorytmu
            switch (mode)
            {
                case PIDModeType.P:
                    {
                        denominator = new Double[1];
                        denominator[0] = 1;

                        break;
                    }
                case PIDModeType.PD:
                    {
                        denominator = new Double[2];
                        denominator[0] = 1;
                        denominator[1] = td * n;

                        break;
                    }
                case PIDModeType.PI:
                    {
                        denominator = new Double[2];
                        denominator[0] = 0;
                        denominator[1] = ti;

                        break;
                    }
                case PIDModeType.PID:
                    {
                        denominator = new Double[3];
                        denominator[0] = 0;
                        denominator[1] = ti;
                        denominator[2] = ti * td * n;

                        break;
                    }
                default:
                    {
                        denominator = new Double[0];
                        break;
                    }
            }
            return denominator;
        }

        /// <summary>
        /// Wyliczenie wspolczynnikow mianownika tranmistancji ciaglej regulatora PID
        /// </summary>
        /// <returns>
        /// Tablica wspolczynnikow mianownika transmitancji ciaglej regulatora PID
        /// </returns>
        private Double[] CalculateDenominator()
        {
            return PIDController.CalculateDenominator(Kp, Ti, Td, N, Mode);
        }

        /// <summary>
        /// Odswiezenie wspolczynnikow transmitancji regulatora - przy kazdej zmianie parametrow
        /// </summary>
        private void RefreshTransferFunctions()
        {
            EditContinous(CalculateNominator(), CalculateDenominator(),ContinousTimeDelay);
        }

        /// <summary>
        /// Konstruktor klasy regulatora PID
        /// </summary>
        /// <param name="kp">
        /// Wzmocnienie
        /// </param>
        /// <param name="ti">
        /// Stala calkowania
        /// </param>
        /// <param name="td">
        /// Stala rozniczkowania
        /// </param>
        /// <param name="n">
        /// Stala czesci inercyjnej czlonu rozniczkujacego
        /// </param>
        /// <param name="sampleTime">
        /// Czas probkowania symulacji regulatora
        /// </param>
        /// <param name="mode">
        /// Tryb algorytmu regulatora
        /// </param>
        /// <param name="inverted">
        /// Tryb regulatora
        /// </param>
        public PIDController(Double kp, Double ti, Double td, Double n, Double sampleTime, PIDModeType mode, bool inverted):
            base(new ContinousTransferFunction(CalculateNominator(kp,ti,td,n,mode,inverted),CalculateDenominator(kp,ti,td,n,mode),0),sampleTime)
        {
            //Konieczna inicjalizacja pol a nie wlasciwosci! - Aby nie powodowac wywolywania RefreshTransferFunction
            this.kp = kp;
            this.td = td;
            this.ti = ti;
            this.n = n;
            this.mode = mode;
            this.inverted = inverted;
        }

        /// <summary>
        /// Konwersja regulatora PID do dokumentu XML
        /// </summary>
        /// <returns>
        /// Dokument XML reprezentujacy regulator
        /// </returns>
        public XDocument PIDToXML()
        {
            XDocument doc = new XDocument();
                
            var kp = new XElement("Kp", Kp.ToString("R"));

            var ti = new XElement("Ti", Ti.ToString("R"));

            var td = new XElement("Td", Td.ToString("R"));

            var n = new XElement("N", N.ToString("R"));

            var sampleTime = new XElement("SampleTime", SimulationSampleTime.ToString("R"));

            var mode = new XElement("Mode", Mode);

            var inverted = new XElement("Inverted", Inverted);

            doc.Add(new XElement("PIDController", kp, ti, td, n,sampleTime, mode, inverted));

            return doc;
        }

        /// <summary>
        /// Metoda tworzace regulator PID na podstawie pliku XML
        /// </summary>
        /// <param name="xmlDocument">
        /// Dokument XML reprezentujacy regulator PID
        /// </param>
        /// <returns>
        /// Obiekt regulatora PID
        /// </returns>
        public static PIDController PIDFromXML( XDocument xmlDocument)
        {
            //Pobranie pierwszego elementu glownego
            XElement PIDControllerElement= xmlDocument.Root;
            
            //Nie moze byc on rowny null
            if(PIDControllerElement == null)
            {
                throw new InvalidOperationException("Wrong file ");
            }

            Double Kp = Convert.ToDouble(PIDControllerElement.Element("Kp").Value);
            Double Ti = Convert.ToDouble(PIDControllerElement.Element("Ti").Value);
            Double Td = Convert.ToDouble(PIDControllerElement.Element("Td").Value);
            Double N = Convert.ToDouble(PIDControllerElement.Element("N").Value);
            Double SampleTime = Convert.ToDouble(PIDControllerElement.Element("SampleTime").Value);
            PIDModeType Type = PIDTypeFromString(PIDControllerElement.Element("Mode").Value.ToString());
            Boolean Inverted = Convert.ToBoolean(PIDControllerElement.Element("Inverted").Value);
            return new PIDController(Kp, Ti, Td, N, SampleTime,Type,Inverted);
        }

        /// <summary>
        /// Metoda tworzace regulator PID na podstawie pliku XML i przypisujaca mu podany czas probkownia symulacji
        /// </summary>
        /// <param name="xmlDocument">
        /// Dokument XML reprezentujacy regulator PID
        /// </param>
        /// <param name="SampleTime">
        /// Czas probkowania symulacji
        /// </param>
        /// <returns>
        /// Obiekt regulatora PID
        /// </returns>
        public static PIDController PIDFromXML(XDocument xmlDocument, Double SampleTime)
        {
            //Pobranie pierwszego elementu glownego
            XElement PIDControllerElement = xmlDocument.Root;

            //Nie moze byc on rowny null
            if (PIDControllerElement == null)
            {
                throw new InvalidOperationException("Wrong file ");
            }

            Double Kp = Convert.ToDouble(PIDControllerElement.Element("Kp").Value);
            Double Ti = Convert.ToDouble(PIDControllerElement.Element("Ti").Value);
            Double Td = Convert.ToDouble(PIDControllerElement.Element("Td").Value);
            Double N = Convert.ToDouble(PIDControllerElement.Element("N").Value);

            PIDModeType Type = PIDTypeFromString(PIDControllerElement.Element("Mode").Value.ToString());
            Boolean Inverted = Convert.ToBoolean(PIDControllerElement.Element("Inverted").Value);

            return new PIDController(Kp, Ti, Td, N, SampleTime, Type,Inverted);
        }

        /// <summary>
        /// Metoda konwerujaca tryb algorytmu regulatora PID do reprezentujacej go nazwy - jest ona zapisywana i odczytywana przez mechanizm konwersji do XML
        /// </summary>
        /// <param name="name">
        /// Nazwa trybu regulatora
        /// </param>
        /// <returns>
        /// Tryb regulatora
        /// </returns>
        static PIDModeType PIDTypeFromString(string name)
        {
            switch(name)
            {
                case "P":
                    {
                        return PIDModeType.P;
                    }
                case "PD":
                    {
                        return PIDModeType.PD;
                    }
                case "PI":
                    {
                        return PIDModeType.PI;
                    }
                case "PID":
                    {
                        return PIDModeType.PID;
                    }
            }

            throw new InvalidCastException("Wrong type of PID");
        }
    }
}
