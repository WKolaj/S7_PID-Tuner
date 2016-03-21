using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TransferFunctionLib;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Globalization;

namespace S7_PID_Tuner
{

    /// <summary>
    /// Interaction logic for NyquistPlotChart.xaml
    /// </summary>
    public partial class NyquistPlotChart : UserControl
    {
        /// <summary>
        /// Akutalny projekt
        /// </summary>
        Project currentProject;

        /// <summary>
        /// Kolekcja punktow nyquista, kompatybilna z mechanizmem wiazania danych WPF z wykresami
        /// </summary>
        FrequencyPointsObservableCollection observableCollectionOfFrequencyPoints = new FrequencyPointsObservableCollection();

        /// <summary>
        /// Konstruktor kontrolki wykresu Nyquista
        /// </summary>
        public NyquistPlotChart()
        {
            //Inicjalizacja UI kontrolki
            InitializeComponent();

            //Inicjalizacja mechanizmu kontrolki
            InitializeControl();
        }

        /// <summary>
        /// Inicjalizacja mechanizmu kontrolki
        /// </summary>
        public void InitializeControl()
        {
            //Przypisanie kolekcji punktow do kontenera danych zwiazanych z wykresem
            nyquistPlotChart.DataContext = observableCollectionOfFrequencyPoints;

            //Zaznaczenie pola automatycznego wyboru zakresu
            autoRangeCheckbox.IsChecked = true;
        }

        /// <summary>
        /// Odswiezenie wartosci wykresu
        /// </summary>
        public void RefreshPlotValues()
        {
            //Wartosci wykresu Nyquista moga byc wyznaczone jedynie jesli projekt, regulator i obiekt nie sa puste
            if(currentProject!=null)
            {
                if(currentProject.Controller!=null)
                {
                    if (currentProject.PlantObject != null)
                    {
                        //Stworzenie nowej transmitancji z polaczenia szeregowego regulatora i obiektu regulacji
                        DynamicSystem commonSystem = currentProject.Controller * currentProject.PlantObject;

                        //Pobranie i sprawdzenie poprawnosci wypelniania czestotliwosci poczatkowych i koncowych zakresu wykresu
                        Double omegaFrom;
                        Double omegaTo;

                        if (CheckOmegaBox(omegaFromBox) < 0)
                        {
                            throw new InvalidOperationException("Wrong omega from box string format");
                        }
                        else if (CheckOmegaBox(omegaFromBox) == 0)
                        {
                            omegaFrom = commonSystem.OmegaFrom;
                            omegaFromBox.Text = omegaFrom.ToString("G6");
                        }
                        else
                        {
                            omegaFrom = Convert.ToDouble(omegaFromBox.Text);
                        }

                        if (CheckOmegaBox(omegaToBox) < 0)
                        {
                            throw new InvalidOperationException("Wrong omega to box string format");
                        }
                        else if (CheckOmegaBox(omegaToBox) == 0)
                        {
                            omegaTo = commonSystem.OmegaTo;
                            omegaToBox.Text = omegaTo.ToString("G6");
                        }
                        else
                        {
                            omegaTo = Convert.ToDouble(omegaToBox.Text);
                        }

                        //Sprawdzenie czy czestotliwosc pocztkowa nie jest wieksza od czestotliwosci koncowej
                        if (omegaFrom >= omegaTo)
                        {
                            throw new InvalidOperationException("Omega from cannot be greater then omega to");
                        }

                        //Pobranie liczby punktow
                        Int32 numberOfPts;

                        if (!Int32.TryParse(numberOfPoints.Text, out numberOfPts))
                        {
                            throw new InvalidOperationException("number of points box string format is invalid");
                        }

                        if (numberOfPts <= 0)
                        {
                            throw new InvalidOperationException("number of points must be greater or equal to zero");
                        }

                        //Pobranie punktow charakterystyki Nyquista
                        commonSystem.NyquistPlotObservable(observableCollectionOfFrequencyPoints,omegaFrom, omegaTo, numberOfPts);

                        //Pobranie punktow dla czestotliwosci zerowej i nieskonczonej
                        omegaZeroLabel.Content = GetStringFromComplexPoint(commonSystem.PointForOmegaZero);
                        omegaInfLabel.Content = GetStringFromComplexPoint(commonSystem.PointForOmegaInf);

                        //Odswiezenie wyswietlenia wykresu
                        RefreshPlotDisplay();

                    }
                    else
                    {
                        throw new InvalidOperationException("Plant is not fill correctly");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Fill all textboxes of PID - In case of P, PD or PI Unnecessary values will not be taken into account !");
                }
            }
            else
            {
                throw new InvalidOperationException("Project is Empty !");
            }

        }

        /// <summary>
        /// Metoda odswiezajaca wyswietlany wykres
        /// </summary>
        public void RefreshPlotDisplay()
        {
            //Odswiezenie wykresu Nyquista
            nyquistPlotChart.InvalidatePlot(true);
        }

        /// <summary>
        /// Metoda konwertujaca punkt charakterystyki Nyquista na odpowiedni lanuch znakow - w celu wyswietlenia wartosci dla omega zero i omega inf
        /// </summary>
        /// <param name="point">
        /// Punkt charakterystyki nquista
        /// </param>
        /// <returns>
        /// Lanuch znakow reprezentujacy punkt charakterystyki Nyquista
        /// </returns>
        private string GetStringFromComplexPoint(FrequencyPoint point)
        {
            //Wyznaczenie wartosci dla niestandardowej czesci rzeczywistej
            Double real = point.Real;
            string realPart = real.ToString("G4");

            if(Double.IsNaN(real))
            {
                realPart = "NaN";
            }
            else if(Double.IsPositiveInfinity(real))
            {
                realPart = "∞";
            }
            else if (Double.IsNegativeInfinity(real))
            {
                realPart = "-∞";
            }


            //Wyznaczenie wartosci dla niestandardowej czesci urojonej
            Double imaginary = point.Imaginary;
            string imaginaryPart = (Math.Abs(imaginary)).ToString("G4");

            if (Double.IsNaN(imaginary))
            {
                imaginaryPart = "NaN";
            }
            else if (Double.IsPositiveInfinity(imaginary))
            {
                imaginaryPart = "∞";
            }
            else if (Double.IsNegativeInfinity(imaginary))
            {
                imaginaryPart = "∞";
            }
            
            //Obsluga ujemnych czesci urojonych
            if(imaginary < 0)
            {
                imaginaryPart = " - j" + imaginaryPart;
            }
            else
            {
                imaginaryPart = " + j" + imaginaryPart;
            }

            //Polaczenie czesci rzeczywistej i urojonej
            return realPart + imaginaryPart;
        }
        
        /// <summary>
        /// Sprawdzenie tekstboksu czestotliwosci
        /// </summary>
        /// <param name="box">
        /// TextBox sprawdzany
        /// </param>
        /// <returns>
        /// Czy wartosc jest poprawna?
        /// 0 - zakres automatyczny
        /// -1 - wartosci niepoprawna
        /// 1 - wartosc poprawna
        /// </returns>
        private int CheckOmegaBox(TextBox box)
        {
            //Jezeli wlaczony jest auto
            if(autoRangeCheckbox.IsChecked == true)
            {
                return 0;
            }

            Double value;

            if(!Double.TryParse(box.Text,out value))
            {
                return -1;
            }
            
            if(value <=0 )
            {
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// Przypisanie nowego projektu do kontrolki
        /// </summary>
        /// <param name="currentProject">
        /// Nowy projekt
        /// </param>
        public void AssignNewProject(Project currentProject)
        {
            this.currentProject = currentProject;
        }

        /// <summary>
        /// Metoda obslugi przycisku Refresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                //Proba odswiezenia wartosci wykresu
                RefreshPlotValues();
            }
            catch(Exception exception)
            {
                //Jezeli nie zakonczyla sie powodzeniem - wyswietlenie komunikatu z bledem
                System.Windows.MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zaznaczenia checkbox'u automatycznego zakresu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoRangeCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            omegaFromBox.IsEnabled = false;
            omegaToBox.IsEnabled = false;
        }

        /// <summary>
        /// Metoda obslugi zdarzenia odznaczenia checkbox'u automatycznego zakresu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoRangeCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            omegaFromBox.IsEnabled = true;
            omegaToBox.IsEnabled = true;
        }

    }

}
