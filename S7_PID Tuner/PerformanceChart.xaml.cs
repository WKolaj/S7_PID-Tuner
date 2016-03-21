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
using OxyPlot;
using TransferFunctionLib;

namespace S7_PID_Tuner
{
    /// <summary>
    /// Klasa reprezentujaca kolekcje punktow odpowiedzi ukladu kompatybilna z mechanizmem wiazania danych WPF
    /// </summary>
    public class PerformancePoints
    {
        /// <summary>
        /// Kolekcja sygnalow wejsciowych
        /// </summary>
        public List<DataPoint> U
        {
            get;
            set;
        }

        /// <summary>
        /// Kolekcja sygnalow wyjsciowych
        /// </summary>
        public List<DataPoint> Y
        {
            get;
            set;
        }
        
        /// <summary>
        /// Konstruktor klasy reprezentujacej kolekcje punktow odpowiedzi ukladu kompatybilna z mechanizmem wiazania danych WPF
        /// </summary>
        public PerformancePoints()
        {
            U = new List<DataPoint>();
            Y = new List<DataPoint>();
        }

    }

    /// <summary>
    /// Interaction logic for PerformanceChart.xaml
    /// </summary>
    public partial class PerformanceChart : UserControl
    {
        /// <summary>
        /// Aktualny projekt
        /// </summary>
        private Project currentProject;

        /// <summary>
        /// Kolekcja punktow odpowiedzi ukladu na zaklocenie
        /// </summary>
        private PerformancePoints observableDisturbancePointsCollection = new PerformancePoints();

        /// <summary>
        /// Kolekcja punktow odpowiedzi ukladu na zmiane wartosci zadanej
        /// </summary>
        private PerformancePoints observableSetpointPointsCollection = new PerformancePoints();

        /// <summary>
        /// Konstruktor klasy kontrolki wskaznikow jakosci ukladu regulacji
        /// </summary>
        public PerformanceChart()
        {
            //Inicjalizacja UI
            InitializeComponent();

            //Inicjalizacja mechanizmu kontrolki
            Initialize();
        }

        /// <summary>
        /// Inicjalizacja kontrolki
        /// </summary>
        private void Initialize()
        {
            //Zaznaczenie automatycznego wyznaczania zakresu wykresu
            autoRangeCheckbox.IsChecked = true;

            //Przypisanie kolekcji kompatybilnych z wiazaniem danych WPF do kontenera wykresu
            disturbanceChart.DataContext = observableDisturbancePointsCollection;
            setpointChart.DataContext = observableSetpointPointsCollection;
        }

        private Int32 timeLength;
        /// <summary>
        /// Dlugosc badanej odpowiedzi
        /// </summary>
        public Int32 TimeLength
        {
            get
            {
                return timeLength;
            }
            private set
            {
                timeLength = value;
            }
        }

        private Int32 ratio;
        /// <summary>
        /// Stosunek czasu probkowania do czasu symulacji - nie wszystkie punkty sa wyswietlane
        /// </summary>
        public Int32 Ratio
        {
            get
            {
                return ratio;
            }
            private set
            {
                ratio = value;
            }

        }

        private Double chartSampleTime;
        /// <summary>
        /// Czas probkowania na wykresie
        /// </summary>
        public Double ChartSampleTime
        {
            get
            {
                return chartSampleTime;
            }
            private set
            {
                chartSampleTime = value;
            }

        }

        private Int32 pointsInChart;
        /// <summary>
        /// Liczba punktow na wykresie 
        /// </summary>
        public Int32 PointsInChart
        {
            get
            {
                return pointsInChart;
            }
            private set
            {
                //Liczba punktow powinna byc dostosowana do pozostalych parametrow - dlatego nalezy ja przeliczyc zanim zostanie ona przypisana 
                //Jej przeliczenie wplywa rowniez na parametry Ratio i ChartSampleTime i PointsOfSimulation !
                pointsInChart = CalculatePointsInChart(value);
            }

        }

        /// <summary>
        /// Ilosc punktow symulacji
        /// </summary>
        private Int32 PointsOfSimulation
        {
            get;
            set;
        }

        /// <summary>
        /// Metoda wyznaczajaca najbardziej przyblizona do podanej liczby punktow na wykresie wartosc, ktora pozwoli na calkowita roznice miedzy czasami probkowania wykresu i symulacji
        /// </summary>
        /// <param name="newPointsInChartValue">
        /// Liczba punktow na wykresie
        /// </param>
        /// <returns>
        /// Mozliwa liczba punktow na wykresie
        /// </returns>
        public Int32 CalculatePointsInChart(Double newPointsInChartValue)
        {
            if(currentProject == null )
            {
                throw new InvalidOperationException("Project is null");
            }

            if(TimeLength <= 0)
            {
                throw new InvalidOperationException("Length must be greater then 0");
            }

            if (currentProject.PlantObject == null )
            {
                throw new InvalidOperationException("Plant object is null");
            }

            if(currentProject.PlantObject.SimulationSampleTime == 0)
            {
                throw new InvalidOperationException("Sample time is 0");
            }

            //Mechanizm wyznaczania najbardziej przyblizonej do podanej liczby punktow na wykresie, ktora pozwoli na calkowita roznice miedzy czasami probkowania wykresu i symulacji
            PointsOfSimulation = Convert.ToInt32(Math.Floor(Convert.ToDouble(TimeLength)/currentProject.PlantObject.SimulationSampleTime));

            Ratio = Convert.ToInt32(Math.Floor(Convert.ToDouble(PointsOfSimulation) / Convert.ToDouble(newPointsInChartValue)));
            
            if(Ratio == 0)
            {
                Ratio = 1;
            }

            ChartSampleTime = Ratio * currentProject.PlantObject.SimulationSampleTime;

            return Convert.ToInt32(Math.Floor(Convert.ToDouble(PointsOfSimulation)/Convert.ToDouble(Ratio)));
        }

        /// <summary>
        /// Metoda przypisujaca projekt do kontrolki
        /// </summary>
        /// <param name="currentProject">
        /// Nowy projekt
        /// </param>
        public void AssignNewProject(Project currentProject)
        {
            this.currentProject = currentProject;
        }

        /// <summary>
        /// Metoda wyznaczajaca automatyczny zakres czasu odpowiedzi ukladu regulacji
        /// </summary>
        /// <returns>
        /// Automatycznie wyznaczony zakres odpowiedzi ukladu regulacji
        /// </returns>
        public Int32 CalculateAutoTimeLength()
        {
            //Zakres wyznaczony jako 10 razy wiekszy niz suma wspolczynnikow mianownika transmitancji ciaglej obiektu regulacji
            if(currentProject.Controller!=null && currentProject.PlantObject!=null)
            {
                Double sum = 0;
                foreach(var factor in currentProject.PlantObject.ContinousDenumerator)
                {
                    sum += Math.Abs(factor);
                }

                sum /= currentProject.PlantObject.ContinousDenumerator.Length;

                return Convert.ToInt32(10*sum);
            }

            return -1;
        }

        /// <summary>
        /// Metoda obslugi klikniecia przycisku Refresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshClicked(object sender, RoutedEventArgs e)
        {
            //Odswiezenie wykresu
            RefreshPlotValues();
        }

        /// <summary>
        /// Metoda odswiezajaca wartosci punktow wykresu
        /// </summary>
        private void RefreshPlotValues()
        {
            try
            {
                //Nalezy najpierw odswiezyc wartosc dlugosci odpowiedzi - jest to konieczne poniewaz mechanizm odswiezania ilosci punktow wykresu korzysta z dlugosci odpowiedzi
                RefreshLengthValue();
                //Odswiezenie wartosci liczby punktow
                RefreshPointsInChartValue();

                //Przygotowanie wymuszenia - w pierwszym punkcie na wykresie zmienia sie z 0 na 1 i pozostaje 1 do konca
                Double[] u = new Double[PointsOfSimulation];

                //Wyczyszczenie kolekcji wiazanych
                observableDisturbancePointsCollection.U.Clear();
                observableSetpointPointsCollection.U.Clear();
                
                //Wyznaczenie wartosci wymuszenia
                int temp = -1;

                for(int i = 0,j=-1; i<u.Length; i++)
                {
                    j = ConvertSimulationToChart(i);

                    if(j > 0)
                    {
                        u[i] = 1;
                    }
                    else
                    {
                        u[i] = 0;
                    }

                    //Jezeli nowe j jest rozne od poprzedniego - nowy punkt wykresu
                    if(temp!=j)
                    {
                        temp = j;

                        //Dla tego punktu tworze nowy punkt na wykresach
                        observableDisturbancePointsCollection.U.Add(new DataPoint(j*ChartSampleTime,u[i]));
                        observableSetpointPointsCollection.U.Add(new DataPoint(j * ChartSampleTime, u[i]));
                    }
                }

                //Pobieram odpowiedzi ukladow na zaklocenie i zmiane wartosci zadanej
                Double[] yDisturbance = currentProject.SimulateCloseLoopDisturbance(u);
                Double[] ySetpoint = currentProject.SimulateCloseLoopSetPoint(u);

                //Czyszcze listy wyswietlajace wartosci odpowiedzi 
                observableDisturbancePointsCollection.Y.Clear();
                observableSetpointPointsCollection.Y.Clear();

                //Przechodze przez każdy punkt symulacji i co Ratio tworze nowy punkt na wykresie - dodaje go do wyzej wyzerowanych kolekcji
                temp = -1;

                for (int i = 0, j = -1; i < yDisturbance.Length; i++)
                {
                    j = ConvertSimulationToChart(i);

                    if (temp != j)
                    {
                        temp = j;
                        observableDisturbancePointsCollection.Y.Add(new DataPoint(j * ChartSampleTime, yDisturbance[i]));
                        observableSetpointPointsCollection.Y.Add(new DataPoint(j * ChartSampleTime, ySetpoint[i]));
                    }
                }

                //Licze wskazniki jakosci regulaji - Suma kwadratow bledu
                Double ISEDisturbance = CalculateIntegrealErrorSquare(yDisturbance);
                Double ISESetpoint = CalculateIntegrealErrorSquare(u, ySetpoint);

                //Przypisuje je wartosciom Label
                iseSPLabel.Content = ISESetpoint.ToString("G6");
                iseDSLabel.Content = ISEDisturbance.ToString("G6");

                //Odswiezam wyswietlanie wykresu
                RefreshPlotDisplay();

            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Metoda odswiezajaca wyswietlanie wykresow
        /// </summary>
        private void RefreshPlotDisplay()
        {
            disturbanceChart.InvalidatePlot(true);
            setpointChart.InvalidatePlot(true);
        }

        /// <summary>
        /// Metoda konwertujaca numer punktu symulacji na numer punktu na wykresie
        /// </summary>
        /// <param name="simulationTime">
        /// Numer punktu symulacji
        /// </param>
        /// <returns>
        /// Odpowiadajacy my numer punktu wykresu
        /// </returns>
        private int ConvertSimulationToChart(int simulationTime)
        {
            return Convert.ToInt32(Math.Floor(Convert.ToDouble(simulationTime) / Convert.ToDouble(Ratio)));
        }

        /// <summary>
        /// Metoda odswiezajaca wartosc dlugosci odpowiedzi - na podstawie wypelnionego TextBox'a
        /// </summary>
        private void RefreshLengthValue()
        {
            if (autoRangeCheckbox.IsChecked == false)
            {
                Int32 newLength;
                if (Int32.TryParse(timeBox.Text, out newLength))
                {
                    TimeLength = newLength;
                }
                else
                {
                    throw new InvalidOperationException("Invalid Length format - must an integer");
                }
            }
            else
            {
                TimeLength = CalculateAutoTimeLength();
            }

            //Oraz na podstawie wyznaczonej wartosci przypisuje ja na nowo do Textboxu - konieczne jezezli jest zaznaczone autoRange
            timeBox.Text = TimeLength.ToString();
        }
        
        /// <summary>
        /// Metoda odswiezajaca liczbe punktow na wykresie - na podstawie wypelnionego TextBox'a
        /// </summary>
        private void RefreshPointsInChartValue()
        {
            Int32 newNumberOfPoints;
            if (Int32.TryParse(numberOfPoints.Text, out newNumberOfPoints))
            {
                if(newNumberOfPoints > 0)
                {
                    PointsInChart = newNumberOfPoints;
                }
                else
                {
                    throw new InvalidOperationException("Invalid Number of points format - must an positive integer");
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid Number of points format - must an positive integer");
            }

            //Oraz na podstawie wyznaczonej wartosci przypisuje ja na nowo do Textboxu - konieczne jezezli jest zaznaczone autoRange oraz gdy podana liczba punktow jest niemolziwa do zrealizowania
            numberOfPoints.Text = PointsInChart.ToString();
        }

        /// <summary>
        /// Metoda wyznaczajaca wartosc ISE dla skokowej zmiany wartosci zadanej
        /// </summary>
        /// <param name="u">
        /// Wartosc zadana
        /// </param>
        /// <param name="y">
        /// Wielkosc mierzona
        /// </param>
        /// <returns>
        /// Wartosc ISE
        /// </returns>
        private Double CalculateIntegrealErrorSquare(Double[] u, Double[] y)
        {
            Double ISE = 0;

            for(int i = 0; i<u.Length; i++)
            {
                ISE += (u[i] - y[i]) * (u[i] - y[i])*currentProject.PlantObject.SimulationSampleTime;
            }

            return ISE;
        }

        /// <summary>
        /// Metoda wyznaczajaca wartosc ISE dla skokowej zmiany zaklocenia ( przyjmuje wartosc SP = 0 )
        /// </summary>
        /// <param name="y">
        /// Wartosc zadana
        /// </param>
        /// <returns>
        /// Wartosc ISE
        /// </returns>
        private Double CalculateIntegrealErrorSquare(Double[] y)
        {
            Double ISE = 0;

            for (int i = 0; i < y.Length; i++)
            {
                ISE += (y[i]) * (y[i]) * currentProject.PlantObject.SimulationSampleTime;
            }

            return ISE;
        }

        /// <summary>
        /// Metoda obslugi znaczenia zaznaczenia funkcji automatycznego wyzanczania zakresu wykresu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoRangeCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            //Zablokowanie textboxu od zakresu czasu
            timeBox.IsEnabled = false;
        }

        /// <summary>
        /// Metoda obslugi odznaczenia zaznaczenia funkcji automatycznego wyzanczania zakresu wykresu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoRangeCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            //Odblokowanie textboxu od zakresu czasu
            timeBox.IsEnabled = true;
        }

    }
}
