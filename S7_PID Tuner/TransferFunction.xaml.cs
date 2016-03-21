using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TransferFunctionLib;

namespace S7_PID_Tuner
{
    

    /// <summary>
    /// Kontrolka ukladu dynamicznego
    /// </summary>
    public partial class TransferFunction : UserControl
    {
        /// <summary>
        /// Obiekt akutalnego projektu
        /// </summary>
        private Project currentProject;

        /// <summary>
        /// Konstruktor kontrolki transmitancji
        /// </summary>
        public TransferFunction()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Metoda przypisujaca kontrolce aktualnie uzywany projekt
        /// </summary>
        /// <param name="currentProject">
        /// Aktualny projekt
        /// </param>
        public void AssignNewProject(Project currentProject)
        {
            //przypisanie aktualnego projektu
            this.currentProject = currentProject;

            //Jezeli aktualny obiekt regulacji jest rozny od null - nastepuje przypisanie obiektu regulacji projektu do 
            if(currentProject.PlantObject!=null)
            {
                //Jezeli obiekt nie jest zero - nalezy przypisac jego zdarzenia zmiany transmitancji do zdarzen odswiezania wyswietlanej transmitancji
                AssignProjectDynamicSystem();
            }
            else
            {
                //Jezeli obiekt regulacji jest pusty nalezy wyczyscic jego wyswietlanie
                ClearTransferFunction();
            }
        }

        /// <summary>
        /// Metoda wyczyszczajaca wyswietlanie transmitancji
        /// </summary>
        void ClearTransferFunction()
        {
            nominatorFactorsLabel.Content = "Nom(s)";
            denominatorFactorsLabel.Content = "Den(s)";
            timeDelayLabel.Content = "e⁻ᵀˢ";
        }

        /// <summary>
        /// Metoda przypisujaca uklad dynamiczny do wyswietlania
        /// </summary>
        /// <param name="ds">
        /// Wyswietlany uklad dynamiczny
        /// </param>
        /// <param name="type">
        /// Typ wyswietlania
        /// </param>
        public void AssignDynamicSystem(DynamicSystem ds, SystemType type)
        {
            //W przypadku gdy byla juz wczesniej wyswietlana jakas transmitancja, trzeba odlaczyc zdarzenia synchronizujace z nia kontrolke
            if(this.currentProject.PlantObject!=null)
            {
                DisconnectDS();
            }

            //Przypisanie nowego uklady dynamicznego
            this.currentProject.PlantObject = ds;
            this.currentProject.Type = type;

            //Odswiezenie wyswietlanych wspolczynnikow
            RefreshTransferFunctionDisplaying();

            //Podlaczenie nowej transmitancji
            ConnectDS();
        }

        /// <summary>
        /// Metoda przypisujaca transmitancje projektu do wyswietlania
        /// </summary>
        public void AssignProjectDynamicSystem()
        {
            //W przypadku gdy byla juz wczesniej wyswietlana jakas transmitancja, trzeba odlaczyc zdarzenia synchronizujace z nia kontrolke
            if (this.currentProject.PlantObject != null)
            {
                DisconnectDS();
            }

            //Odswiezenie wyswietlanych wspolczynnikow
            RefreshTransferFunctionDisplaying();

            //Podlaczenie nowej transmitancji
            ConnectDS();
        }

        /// <summary>
        /// Metoda odlaczajaca zdarzenia synchronizujace kontrolke z podlaczona do niej transmitancja
        /// </summary>
        public void DisconnectDS()
        {
            if(currentProject == null)
            {
                return;
            }

            if (currentProject.Type == SystemType.Continues)
            {
                currentProject.PlantObject.continousTransferFunctionChanged -= ContinouesTFChanged;
            }
            else if (currentProject.Type == SystemType.Discrete)
            {
                currentProject.PlantObject.discreteTransferFunctionChanged -= DiscreteTFChanged;
            }
        }

        /// <summary>
        /// Podlaczenie zdarzeń synchronizujacych kontrolke z podlaczona do niej transmitancja
        /// </summary>
        public void ConnectDS()
        {
            if(currentProject == null)
            {
                return;
            }

            if (currentProject.Type == SystemType.Continues)
            {
                currentProject.PlantObject.continousTransferFunctionChanged += ContinouesTFChanged;
            }
            else if (currentProject.Type == SystemType.Discrete)
            {
                currentProject.PlantObject.discreteTransferFunctionChanged += DiscreteTFChanged;
            }
        }

        /// <summary>
        /// Odswiezenie wsywietlanych wspolczynnikow
        /// </summary>
        public void RefreshTransferFunctionDisplaying()
        {
            //Jezeli projekt lub transmitancja jest pusta nalezy wyczyscic jej wyswietlanie
            if(currentProject == null)
            {
                ClearTransferFunction();
                return;
            }

            if(currentProject.PlantObject == null)
            {
                ClearTransferFunction();
                return;
            }

            //Nastepnie nalezy sprawdzic, czy aktualny obiekt jest ciagly czy dyskretny i przypisac wektory transmitancji jego licznika i mianownika do wyswietlania
            if (currentProject.Type == SystemType.Continues)
            {
                nominatorFactorsLabel.Content = currentProject.PlantObject.ContinousNominatorString;
                denominatorFactorsLabel.Content = currentProject.PlantObject.ContinousDenominatorString;
                timeDelayLabel.Content = currentProject.PlantObject.ContinousTimeDelayString;
            }
            else if (currentProject.Type == SystemType.Discrete)
            {
                nominatorFactorsLabel.Content = currentProject.PlantObject.DiscreteNominatorString;
                denominatorFactorsLabel.Content = currentProject.PlantObject.DiscreteDenominatorString;
                timeDelayLabel.Content = currentProject.PlantObject.DiscreteTimeDelayString;
            }
        }

        /// <summary>
        /// Zdarzenie wywolane gdy wspolczynniki transmitancji dyskretnej ulegna zmianie
        /// </summary>
        /// <param name="sender">
        /// Obiekt wysylajacy zdarzenie
        /// </param>
        /// <param name="argument">
        /// Argument zdarzenia
        /// </param>
        void DiscreteTFChanged(object sender, DiscreteTransferFunctionChangedEventArg argument)
        {
            RefreshTransferFunctionDisplaying();
        }

        /// <summary>
        /// Zdarzenie wywolane gdy wspolczynniki transmitancji ciagłej ulegna zmianie
        /// </summary>
        /// <param name="sender">
        /// Obiekt wysylajacy zdarzenie
        /// </param>
        /// <param name="argument">
        /// Argument zdarzenia
        /// </param>
        void ContinouesTFChanged(object sender, ContinousTransferFunctionChangedEventArg argument)
        {
            RefreshTransferFunctionDisplaying();
        }

        /// <summary>
        /// Metoda wywolywana gdy wcisniety jest blok transmitancji - zostaje wyswietlone okno jej modyfikacji
        /// </summary>
        /// <param name="sender">
        /// Obiekt zglaszajacy zdarzenie
        /// </param>
        /// <param name="e">
        /// Argument zdarzenia
        /// </param>
        public void OnLoadClicked(object sender, MouseButtonEventArgs e)
        {
            OpenEditWinow();
        }

        /// <summary>
        /// Metoda otwierajaca okno edycji obiektu regulacji
        /// </summary>
        public void OpenEditWinow()
        {
            //Stworzenie nowego okna edycji ukladu dynamicznego
            EditPlantWindow window = new EditPlantWindow(currentProject.PlantObject, currentProject.Type);

            //Jezeli w oknie kliknieto ok - zostalo zaakceptowane - nalezy podpiac nowo stworzona w nim transmitancje do wyswietlania
            bool? result = window.ShowDialog();

            if (result.HasValue)
            {
                if ((bool)result)
                {
                    //Podpiecie nowej transmitancji jest konieczne tylko w przypadku gdy zmienona została transmitancja wewnatrz okna jej edycji
                    if (window.Changed)
                    {
                        AssignDynamicSystem(window.ModyfiedDynamicSystem, window.Type);
                    }
                }
            }
        }

        /// <summary>
        /// Metoda zmieniajaca kolor tła transmitancji - sluzace do zaznaczania
        /// </summary>
        /// <param name="colorCode">
        /// lancuch znakow reprezentujacy wartosci koloru w kodzie szesnastkowym
        /// </param>
        public void ChangeColor(String colorCode)
        {
            MainBorder.Background = (Brush)(new BrushConverter()).ConvertFrom(colorCode);
        }

    }
}
