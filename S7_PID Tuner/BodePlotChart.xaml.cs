using OxyPlot;
using OxyPlot.Series;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TransferFunctionLib;

namespace S7_PID_Tuner
{
    /// <summary>
    /// Interaction logic for BodePlotChart.xaml
    /// </summary>
    public partial class BodePlotChart : UserControl
    {
        /// <summary>
        /// Aktualnie wybrany projekt
        /// </summary>
        Project currentProject;

        /// <summary>
        /// Kolekcja punktow bodego, kompatybilna z mechanizmem wiazania danych WPF z wykresami
        /// </summary>
        BodePointsDataPoints observableCollectionOfBodePoints = new BodePointsDataPoints();

        /// <summary>
        /// Konstruktor klasy kontrolki wykresu Bodego 
        /// </summary>
        public BodePlotChart()
        {
            //Inicjalizacja UI
            InitializeComponent();

            //Inicjalizacja mechanizmu kontrolki
            InitializeControl();
        }

        /// <summary>
        /// Metoda inicjalizujaca mechanizm kontrolki
        /// </summary>
        public void InitializeControl()
        {
            //Przypisanie kolekcji punktow do kontenera danych zwiazanych z wykresami moduli i fazy
            gainChart.DataContext = observableCollectionOfBodePoints;
            phaseChart.DataContext = observableCollectionOfBodePoints;

            //Zaznaczenie pola automatycznego wyboru zakresu
            autoRangeCheckbox.IsChecked = true;
        }

        /// <summary>
        /// Odswiezenie wartosci wykresu
        /// </summary>
        public void RefreshPlotValues()
        {
            //Wartosci wykresu Bodego moga byc wyznaczone jedynie jesli projekt, regulator i obiekt nie sa puste
            if (currentProject != null)
            {
                if (currentProject.Controller != null)
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

                        //Pobranie punktow charakterystyki Bodego
                        commonSystem.BodePlotObservable(observableCollectionOfBodePoints,omegaFrom, omegaTo, numberOfPts);

                        //Wyczyszczenie kolekcji zapasow fazy i modolu
                        observableCollectionOfBodePoints.GainMargins.Clear();
                        observableCollectionOfBodePoints.PhaseMargins.Clear();

                        //Jezeli sa zapasy modulu - nalezy jest wyswietlic na wykresie oraz wyswietlic wartosc zapasu modulu 
                        if(commonSystem.GainMargins.Count() > 0)
                        {
                            gainMarginLabel.Content = commonSystem.GainMargin.Value.ToString("G4");
                            DrawMarginGains(omegaFrom,omegaTo,commonSystem);
                            
                        }
                        else
                        {
                            gainMarginLabel.Content = "-";
                        }

                        //Jezeli sa zapasy fazy - nalezy jest wyswietlic na wykresie oraz wyswietlic wartosc zapasu fazy 
                        if (commonSystem.PhaseMargins.Count() > 0)
                        {
                            phaseMarginLabel.Content = commonSystem.PhaseMargin.Value.ToString("G4");
                            DrawMarginPhase(omegaFrom, omegaTo, commonSystem);
                        }
                        else
                        {
                            phaseMarginLabel.Content = "-";
                        }

                        //Odswiezenie wyswietlania wykresu
                        RefreshPlotsDisplay();
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
        /// Metoda wyswietlajaca zapasy modulu na wykresie
        /// </summary>
        /// <param name="omegaFrom">
        /// Czestotliwosc poczatkowa
        /// </param>
        /// <param name="omegaTo">
        /// Czestotliwosc koncowa
        /// </param>
        /// <param name="commonSystem">
        /// Transmitancja ukladu otwartego
        /// </param>
        private void DrawMarginGains(Double omegaFrom, Double omegaTo, DynamicSystem commonSystem)
        {
            //Wyczysczenie kolekcji zapasow modulu
            observableCollectionOfBodePoints.GainMargins.Clear();
            
            //Pobranie wszysktich zapasow modulu transmitancji, ktore leza miedzy podanym zakresem czestotliwosci
            var margins = from margin in commonSystem.GainMargins
                        where (margin.Frequency >= omegaFrom && margin.Frequency <= omegaTo)
                        select new ScatterPoint(margin.Frequency, margin.BodeValue);

            //Jezeli nie ma takich - nalezy zakonczyc rysowanie
            if(margins.Count() <= 0)
            {
                return;
            }

            //Jezeli są takie zapasy - nalezy je dodac do wyswietlanej kolekcji zapasow modulu
            foreach(var margin in margins)
            {
                observableCollectionOfBodePoints.GainMargins.Add(margin);
            }
        }

        /// <summary>
        /// Metoda wyswietlajaca zapasy fazy na wykresie
        /// </summary>
        /// <param name="omegaFrom">
        /// Czestotliwosc poczatkowa
        /// </param>
        /// <param name="omegaTo">
        /// Czestotliwosc koncowa
        /// </param>
        /// <param name="commonSystem">
        /// Transmitancja ukladu otwartego
        /// </param>
        private void DrawMarginPhase(Double omegaFrom, Double omegaTo, DynamicSystem commonSystem)
        {
            //Wyczysczenie kolekcji zapasow fazy
            observableCollectionOfBodePoints.PhaseMargins.Clear();

            //Pobranie wszysktich zapasow fazy transmitancji, ktore leza miedzy podanym zakresem czestotliwosci
            var margins = from margin in commonSystem.PhaseMargins
                          where (margin.Frequency >= omegaFrom && margin.Frequency <= omegaTo)
                          select new ScatterPoint(margin.Frequency, margin.BodeValue);

            //Jezeli nie ma takich - nalezy zakonczyc rysowanie
            if (margins.Count() <= 0)
            {
                return;
            }

            //Jezeli są takie zapasy - nalezy je dodac do wyswietlanej kolekcji zapasow fazy
            foreach (var margin in margins)
            {
                observableCollectionOfBodePoints.PhaseMargins.Add(margin);
            }
        }

        /// <summary>
        /// Odswiezenie wyswietlania wykresu
        /// </summary>
        private void RefreshPlotsDisplay()
        {
            phaseChart.InvalidatePlot(true);
            gainChart.InvalidatePlot(true);
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
            if (autoRangeCheckbox.IsChecked == true)
            {
                return 0;
            }

            Double value;

            if (!Double.TryParse(box.Text, out value))
            {
                return -1;
            }

            if (value <= 0)
            {
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// Metoda przypisujaca nowy projekt do kontrolki
        /// </summary>
        /// <param name="currentProject">
        /// Nowy projekt
        /// </param>
        public void AssignNewProject(Project currentProject)
        {
            this.currentProject = currentProject;
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia przycisku Refresh
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
            catch (Exception exception)
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
