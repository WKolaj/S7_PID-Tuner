using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace S7_PID_Tuner
{
    /// <summary>
    /// Interaction logic for ControlProcessPanel.xaml
    /// </summary>
    public partial class ControlSystemControl : UserControl
    {
        /// <summary>
        /// Obiekt sterownika - do polaczenia sie z fizycznym urzadzeniem
        /// </summary>
        private PIDControllerDevice controllerDevice;

        /// <summary>
        /// Metoda przypisujaca obiekt sterownika do polaczenia z fizycznym urzadzeniem do okna
        /// </summary>
        /// <param name="controllerDevice"></param>
        public void AssignControllerToDevice(PIDControllerDevice controllerDevice)
        {
            this.controllerDevice = controllerDevice;

            //Przypisanie sterownika do kontrolki PidCOntrol
            pidControllerControl.AssignControllerToDevice(controllerDevice);
        }

        /// <summary>
        /// Aktualnie edytowany projekt
        /// </summary>
        private Project currentProject;

        /// <summary>
        /// Konstruktor klasy panelu 
        /// </summary>
        public ControlSystemControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Przypisanie projektu do kontrolki
        /// </summary>
        /// <param name="currentProject">
        /// Aktualny projekt
        /// </param>
        public void AssignProjectToControl(Project currentProject)
        {
            //Odlaczenie aktualnego projektu od zdarzen
            DisconnectFromPlantProjectEvent();
            this.currentProject = currentProject;
            //Polaczenie nowego projektu do metod obslugi zdarzen
            ConnectToPlantProjectEvent();

            //Przypisanie projektu do elementow wewnetrznych - kontrolki obiektu regulacji i regulatora
            processControl.AssignProjectToControl(currentProject);
            pidControllerControl.AssignProjectToControl(currentProject);
        }

        /// <summary>
        /// Metoda laczaca zdarzenie zmiany obiektu regulacji z metoda jego obslugi 
        /// </summary>
        private void ConnectToPlantProjectEvent()
        {
            if(currentProject!=null)
            {
                currentProject.PlantChangedEvent += OnPlantInProjectChanged;
            }
        }

        /// <summary>
        /// Metoda rozlaczajaca zdarzenie zmiany obiektu regulacji z metoda jego obslugi 
        /// </summary>
        private void DisconnectFromPlantProjectEvent()
        {
            if (currentProject != null)
            {
                currentProject.PlantChangedEvent -= OnPlantInProjectChanged;
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia przycisku wewnatrz kontrolki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseDownOverPanel(object sender, MouseButtonEventArgs e)
        {
            //Jezeli zostal klikniety obiekt regulacji - nalezy go zaznaczyc
            if(processControl.IsMouseOver)
            {
                processControl.Select();
            }
            else
            {
                processControl.Unselect();
            }

            //Jezeli zostal klikniety regulator nalezy jego zaznaczyc
            if (pidControllerControl.IsMouseOver)
            {
                pidControllerControl.Select();
            }
            else
            {
                pidControllerControl.Unselect();
            }
        }

        /// <summary>
        /// Metoda wczytajaca nowy obiekt regulacji
        /// </summary>
        public void LoadProcessControl()
        {
            processControl.LoadNewPlantObject();
        }

        /// <summary>
        /// Metoda zapisujaca obiekt regulacji
        /// </summary>
        public void SaveProcessControl()
        {
            processControl.SaveNewPlantObject();
        }

        /// <summary>
        /// Metoda wczytujaca nowy obiekt regulatora
        /// </summary>
        public void LoadPIDController()
        {
            pidControllerControl.LoadNewControllerObject();
        }

        /// <summary>
        /// Metoda zapisujaca obiekt regulatora
        /// </summary>
        public void SavePIDController()
        {
            pidControllerControl.SaveNewControllerObject();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zmiany obiektu regulacji - w celu wyswietlenia wartosci czasu probkowania
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnPlantInProjectChanged(object sender, EventArgs eventArgs)
        {
            if(currentProject!=null)
            {
                if(currentProject.PlantObject!=null)
                {
                    sampleTimeLabel.Content = currentProject.PlantObject.SimulationSampleTime;
                }
            }
        }

    }
}
