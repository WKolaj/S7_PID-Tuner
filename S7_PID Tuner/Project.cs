using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TransferFunctionLib;

namespace S7_PID_Tuner
{ 
    /// <summary>
    /// Zdarzenie zgłaszane gdy obiekt regulacji ulega zmianie
    /// </summary>
    /// <param name="sender">
    /// Obiekt którego transmitancja dyskretna ulega zmianie
    /// </param>
    /// <param name="eventArgument">
    /// Argument metody obslugi zdarzenia
    /// </param>
    public delegate void PlantChanged(object sender, EventArgs eventArgument);

    /// <summary>
    /// Zdarzenie zgłaszane gdy regulator ulega zmianie
    /// </summary>
    /// <param name="sender">
    /// Obiekt którego transmitancja dyskretna ulega zmianie
    /// </param>
    /// <param name="eventArgument">
    /// Argument metody obslugi zdarzenia
    /// </param>
    public delegate void ControllerChanged(object sender, EventArgs eventArgument);

    /// <summary>
    /// Klasa reprezentujaca projekt aplikacji
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Zdarzenie zglaszane jezeli obiekt regulacji uległ zmianie
        /// </summary>
        public event PlantChanged PlantChangedEvent;

        /// <summary>
        /// Zdarzenie zglaszane jezeli regulator uległ zmianie
        /// </summary>
        public event ControllerChanged ControllerChangedEvent;

        private DynamicSystem plantObject;
        /// <summary>
        /// Obiekt regulacji
        /// </summary>
        public DynamicSystem PlantObject
        {
            get
            {
                return plantObject;
            }
            set
            {
                //Kazda zmiana obiektu regulacji wymaga odlaczenia zdarzen zmian jego transmitancji od zdarzenia zmiany obiektu regulacji PlantChangedEvent
                DisconnectPlantChangedEvent();

                this.plantObject = value;

                //Nastepnie po przypisaniu nowego obiektu regulacji nalezy podlaczyc zdarzenia jego transmitancji do zdarzenia PlantChangedEvent
                ConnectPlantChangedEvent();

                //Nastepnie nalezy zglosic zdarzenie zmiany obiektu regulacji
                if(PlantChangedEvent!=null)
                {
                    PlantChangedEvent(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Metoda podlaczajaca zdarzenia zmiany tranmistancji obiektu regulacji do metod ich obslugi zglaszajacych zdarzenie PlantChanged
        /// </summary>
        private void ConnectPlantChangedEvent()
        {
            if(plantObject!=null)
            {
                plantObject.discreteTransferFunctionChanged += PlantDiscreteTransferEventToDynamicSystemEvent;
                plantObject.continousTransferFunctionChanged += PlantContinousTransferEventToDynamicSystemEvent;
            }
        }

        /// <summary>
        /// Metoda odlaczajaca zdarzenia zmiany tranmistancji obiektu regulacji do metod ich obslugi zglaszajacych zdarzenie PlantChanged
        /// </summary>
        private void DisconnectPlantChangedEvent()
        {
            if (plantObject != null)
            {
                plantObject.discreteTransferFunctionChanged -= PlantDiscreteTransferEventToDynamicSystemEvent;
                plantObject.continousTransferFunctionChanged -= PlantContinousTransferEventToDynamicSystemEvent;
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zmiany transmitancji dyskretnej obiektu regulacji - zglasza zdarzenie zmiany obiektu regulacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="argument"></param>
        private void PlantDiscreteTransferEventToDynamicSystemEvent(object sender, DiscreteTransferFunctionChangedEventArg argument)
        {
            if (PlantChangedEvent != null)
            {
                PlantChangedEvent(this, new EventArgs());
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zmiany transmitancji ciaglej obiektu regulacji - zglasza zdarzenie zmiany obiektu regulacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="argument"></param>
        private void PlantContinousTransferEventToDynamicSystemEvent(object sender, ContinousTransferFunctionChangedEventArg argument)
        {
            if (PlantChangedEvent != null)
            {
                PlantChangedEvent(this, new EventArgs());
            }
        }

        private PIDController controller;
        /// <summary>
        /// Regulator ukladu regulacji
        /// </summary>
        public PIDController Controller
        {
            get
            {
                return controller;
            }
            set
            {
                //Kazda zmiana regulatora wymaga odlaczenia zdarzen zmian jego transmitancji od zdarzenia zmiany obiektu regulacji ControllerChangedEvent
                DisconnectControllerChangedEvent();
                this.controller = value;

                //Nastepnie po przypisaniu nowego obiektu regulatora nalezy podlaczyc zdarzenia jego transmitancji do zdarzenia ControllerChangedEvent
                ConnectControllerChangedEvent();

                //Nastepnie nalezy zglosic zdarzenie zmiany obiektu regulatora
                if(ControllerChangedEvent!=null)
                {
                    ControllerChangedEvent(this,new EventArgs());
                }
            }
        }

        /// <summary>
        /// Metoda podlaczajaca zdarzenia zmiany tranmistancji obiektu regulatra do metod ich obslugi zglaszajacych zdarzenie ControllerChanged
        /// </summary>
        private void ConnectControllerChangedEvent()
        {
            if (controller != null)
            {
                controller.discreteTransferFunctionChanged += ControllerDiscreteTransferEventToDynamicSystemEvent;
                controller.continousTransferFunctionChanged += ControllerContinousTransferEventToDynamicSystemEvent;
            }
        }

        /// <summary>
        /// Metoda odlaczajaca zdarzenia zmiany tranmistancji obiektu regulatora do metod ich obslugi zglaszajacych zdarzenie ControllerChanged
        /// </summary>
        private void DisconnectControllerChangedEvent()
        {
            if (controller != null)
            {
                controller.discreteTransferFunctionChanged -= ControllerDiscreteTransferEventToDynamicSystemEvent;
                controller.continousTransferFunctionChanged -= ControllerContinousTransferEventToDynamicSystemEvent;
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zmiany transmitancji dyskretnej obiektu regulatora - zglasza zdarzenie zmiany obiektu regulatora
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="argument"></param>
        private void ControllerDiscreteTransferEventToDynamicSystemEvent(object sender, DiscreteTransferFunctionChangedEventArg argument)
        {
            if (ControllerChangedEvent != null)
            {
                ControllerChangedEvent(this, new EventArgs());
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zmiany transmitancji ciaglej obiektu regulatora - zglasza zdarzenie zmiany obiektu regulatora
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="argument"></param>
        private void ControllerContinousTransferEventToDynamicSystemEvent(object sender, ContinousTransferFunctionChangedEventArg argument)
        {
            if (ControllerChangedEvent != null)
            {
                ControllerChangedEvent(this, new EventArgs());
            }
        }

        /// <summary>
        /// Metoda synchronizujaca czas probkowania regulatora i obiektu regulacji - przypisuje czas probkowania obiektu regulacji do regulatora
        /// Jest ona zglaszana za kazdym razem gdy 
        /// </summary>
        private void SimulationSampleTimeSynchronization()
        {
            if(Controller!=null && PlantObject!=null)
            {
                Controller.SimulationSampleTime = PlantObject.SimulationSampleTime;
            }
        }

        /// <summary>
        /// Metoda zglaszana za kazdym razem gdy zmieniony zostaje obiekt regulacjiW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnPlantObjectChanged(object sender, EventArgs eventArgs)
        {
            //Synchornizowany jest wtedy czas probkowania regulatora
            SimulationSampleTimeSynchronization();
        }

        /// <summary>
        /// Metoda podlaczajaca metode obslugi zdarzenia PlantChangedEvent
        /// </summary>
        private void ConnectPlantObjectEventToOnPlantObjectChanged()
        {
            PlantChangedEvent += OnPlantObjectChanged;
        }

        public SystemType type;
        /// <summary>
        /// Typ obiektu regulacji - dyskretny, ciagly
        /// </summary>
        public SystemType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;

                if (PlantChangedEvent != null)
                {
                    PlantChangedEvent(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Konstruktor obiektu klasy reprezentujacej projekt aplikacji
        /// </summary>
        /// <param name="plantObject">
        /// Obiekt regulacji
        /// </param>
        /// <param name="controller">
        /// Obiekt regulatora
        /// </param>
        /// <param name="type">
        /// Typ obiektu regulacji - dyskretny/ciagly
        /// </param>
        public Project(DynamicSystem plantObject, PIDController controller, SystemType type)
        {
            this.PlantObject = plantObject;
            this.Controller = controller;
            this.Type = type;

            //Podlaczenie metody obslugi zdarzenia zmiany obiektu regulacji - synchornizujacej czas jego probkowania z czasem probkownia obiektu regulacji
            ConnectPlantObjectEventToOnPlantObjectChanged();
        }

        /// <summary>
        /// Metoda konwerujaca projekt aplikacji do dokumentu XML
        /// </summary>
        /// <returns></returns>
        public XDocument ToXML()
        {
            XDocument mainXML;
            XDocument plantXML, controllerXML;

            if (PlantObject != null)
            {
                plantXML = PlantObject.ToXML(Type);

                if (Controller != null)
                {
                    controllerXML = Controller.PIDToXML();
                    mainXML = new XDocument(new XElement("Project", plantXML.Root, controllerXML.Root));
                }
                else
                {
                    mainXML = new XDocument(new XElement("Project", plantXML.Root));
                }

            }
            else 
            {
                if (Controller != null)
                {
                    controllerXML = Controller.PIDToXML();
                    mainXML = new XDocument(new XElement("Project", controllerXML.Root));
                }
                else
                {
                    mainXML = new XDocument(new XElement("Project"));
                }
            }

            return mainXML;
        }

        /// <summary>
        /// Metoda tworzaca projekt na podstawie dokumentu XML
        /// </summary>
        /// <param name="document">
        /// Dokument XML reprezentujacy projekt
        /// </param>
        /// <returns>
        /// Obiekt projektu
        /// </returns>
        public static Project FromXML(XDocument document)
        {
            //Pobranie elementu glownego - jezeli nie jest on elementem projektu "Project" nalezy zglosic wyjatek
            var rootElement = document.Root;

            if(rootElement.Name != "Project")
            {
                throw new InvalidOperationException("Wrong file format");
            }

            //Pobranie elementu regulatora
            var controller = from xmlElement in rootElement.Elements()
                             where xmlElement.Name == "PIDController"
                             select xmlElement;

            //Pobranie elementu ciaglego obiektu regulacji
            var continousDynamicSystem = from xmlElement in rootElement.Elements()
                                            where xmlElement.Name == "ContinousDynamicSystem"
                                            select xmlElement;

            //Pobranie elementu dyskretnego elementu regulacji
            var discreteDynamicSystem = from xmlElement in rootElement.Elements()
                                            where xmlElement.Name == "DiscreteDynamicSystem"
                                            select xmlElement;

            PIDController controllerObject = null;
            DynamicSystem plant = null;
            SystemType type = SystemType.Continues;

            //Sprawdzenie czy udalo sie pobrac ciagly czy dyskretny obiekt regulacji
            if(continousDynamicSystem.Count() != 0)
            {
                plant = DynamicSystem.FromXML(new XDocument(continousDynamicSystem));
                type = SystemType.Continues;
            } 
            else if (discreteDynamicSystem.Count() != 0)
            {
                plant = DynamicSystem.FromXML(new XDocument(discreteDynamicSystem));
                type = SystemType.Discrete;
            }

            //Sprawdzenie czy udalo sie pobrac regulator
            if(controller.Count() != 0)
            {
                controllerObject = PIDController.PIDFromXML(new XDocument(controller));
            }

            //Stworzenie nowego projektu na podstawie pobranych danych
            return new Project(plant, controllerObject, type);
        }

        /// <summary>
        /// Metoda symulujaca zachowanie ukladu dla zmiany sygnalu zdanego
        /// </summary>
        /// <param name="setpoints">
        /// Sygnal zdany
        /// </param>
        /// <returns>
        /// Odpowiedzi ukladu na podany sygnal zdany
        /// </returns>
        public Double[] SimulateCloseLoopSetPoint(Double[] setpoints)
        {
            return plantObject.SimulateCloseLoopWithControllerSetpoint(controller, setpoints);
        }

        /// <summary>
        /// Metoda symulujaca zachowanie ukladu dla wystepujacego zaklocenia
        /// </summary>
        /// <param name="disturbance">
        /// Zaklocenie
        /// </param>
        /// <returns>
        /// Odpowiedz ukladu na zadane zaklocenie
        /// </returns>
        public Double[] SimulateCloseLoopDisturbance(Double[] disturbance)
        {
            return plantObject.SimulateCloseLoopWithControllerDisturbance(controller, disturbance);
        }
    }
}
