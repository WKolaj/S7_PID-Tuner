using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace S7_PID_Tuner
{
    /// <summary>
    /// Interaction logic for RealTimePlot.xaml
    /// </summary>
    public partial class ProcessControlPlot : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Obiekt probkujacy odswiezanie interfejsu
        /// </summary>
        private System.Timers.Timer refreshPlotTimer = new System.Timers.Timer();

        /// <summary>
        /// Mechanizm wiazania danych WPF
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Mechanizm wiazania danych WPF
        /// </summary>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Wlasciwosc determinujaca czy wykres jest odswiezany
        /// </summary>
        public bool IsRunning
        {
            get
            {
                //Zwraca poprostu stan timera
                return refreshPlotTimer.Enabled;
            }

        }

        /// <summary>
        /// Dlugosc wykresu liczona w punktach
        /// </summary>
        public Int32 Length
        {
            get
            {
                return PVSeries.MaxLenght;
            }

            set
            {
                //Kazda zmiana dlugosci nosi za soba koniecznosc zmiany dlugosci buforow serii na wykresie
                PVSeries.MaxLenght = value;
                SPSeries.MaxLenght = value;
                CVSeries.MaxLenght = value;
                NotifyPropertyChanged("Length");
            }

        }

        /// <summary>
        /// Czas probkowania odswiezania iterfejsu
        /// </summary>
        public Int32 RefreshSampleTime
        {
            get
            {
                return Convert.ToInt32(refreshPlotTimer.Interval);
            }

            set
            {
                refreshPlotTimer.Interval = Convert.ToDouble(value);
                NotifyPropertyChanged("RefreshSampleTime");
            }
        }

        /// <summary>
        /// Konstruktor klasy wykresu czasu rzeczywistego
        /// </summary>
        public ProcessControlPlot()
        {
            InitializeComponent();

            //Inicjalizacja serii i timera odswiezajacego wykres
            InitRealTimeSeries();
            InitTimer();
        }

        /// <summary>
        /// Zatrzymanie odswiezania wykresu i pobierania probek
        /// </summary>
        public void Stop()
        {
            refreshPlotTimer.Stop();
        }

        /// <summary>
        /// Wznownienie odswiezania wykresu i pobierania probek
        /// </summary>
        public void Start()
        {
            refreshPlotTimer.Start();
        }

        /// <summary>
        /// Inicjalizacja timera odswiezajacego
        /// </summary>
        public void InitTimer()
        {
            RefreshSampleTime = 100;
            refreshPlotTimer.Elapsed += OnTimerTick;
        }

        /// <summary>
        /// Metoda inicjalizujaca serie wykresu
        /// </summary>
        public void InitRealTimeSeries()
        {
            PVSeries.InitRealTimeLineSeries(1000);
            SPSeries.InitRealTimeLineSeries(1000);
            CVSeries.InitRealTimeLineSeries(1000);
        }

        /// <summary>
        /// Metoda obslugi zdarzenia timera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnTimerTick(Object sender, System.Timers.ElapsedEventArgs args)
        {
            //Odswiezenie wykresu
            RefreshPlot();
        }

        /// <summary>
        /// Metoda odswiezajaca wykres
        /// </summary>
        public void RefreshPlot()
        {
            Dispatcher.BeginInvoke(new Action(() =>
                        {
                            realTimeChart.InvalidatePlot(true);
                        }));
        }

        /// <summary>
        /// Metoda dodajaca nowy punkt do serii PV
        /// </summary>
        /// <param name="value">
        /// Nowa wartosc mierzona
        /// </param>
        public void AddNewPVValue(Double value)
        {
            if(IsRunning)
            {
                PVSeries.AddValue(value);
            }
        }

        /// <summary>
        /// Metoda dodajaca nowy punkt do serii CV
        /// </summary>
        /// <param name="value">
        /// Nowy sygnal sterujacy
        /// </param>
        public void AddNewCVValue(Double value)
        {
            if (IsRunning)
            {
                CVSeries.AddValue(value);
            }
        }

        /// <summary>
        /// Metoda dodajaca nowy punkt do serii SP
        /// </summary>
        /// <param name="value">
        /// Nowa wartosc zadana
        /// </param>
        public void AddNewSPValue(Double value)
        {
            if (IsRunning)
            {
                SPSeries.AddValue(value);
            }
        }

        /// <summary>
        /// Metoda czyszczaca zawartosc wykresu
        /// </summary>
        public void Clear()
        {
            PVSeries.Clear();
            SPSeries.Clear();
            CVSeries.Clear();
            RefreshPlot();
        }
        
        /// <summary>
        /// Metoda zwracajaca obiekt string przechowujacy zawartosc wykresu do pliku do zapisu 
        /// </summary>
        /// <returns>
        /// Zawartosc pliku do zapisu w formacie CSV
        /// </returns>
        public StringBuilder ToTrendString()
        {
            StringBuilder plotString = new StringBuilder();

            int length = PVSeries.DataPoints.Count() > SPSeries.DataPoints.Count() ? PVSeries.DataPoints.Count() : SPSeries.DataPoints.Count();

            length = length > CVSeries.DataPoints.Count() ? CVSeries.DataPoints.Count() : length;

            for(int i=0; i< length; i++)
            {

                //Nalezy sprawdzic czy przypadkiem jedna z serii nie jest dluzsza/krotsza od drugiej

                if(i < PVSeries.DataPoints.Count())
                {
                    if( i < SPSeries.DataPoints.Count())
                    {
                        if (i < CVSeries.DataPoints.Count())
                        {
                            plotString.AppendLine((PVSeries.DataPoints[i].X).ToString("R") + ";" + PVSeries.DataPoints[i].Y.ToString("G6") + ";" + (SPSeries.DataPoints[i].X).ToString("R") + ";" + SPSeries.DataPoints[i].Y.ToString("G6") + ";" + (CVSeries.DataPoints[i].X).ToString("R") + ";" + CVSeries.DataPoints[i].Y.ToString("G6"));
                        }
                        else
                        {
                            plotString.AppendLine((PVSeries.DataPoints[i].X).ToString("R") + ";" + PVSeries.DataPoints[i].Y.ToString("G6") + ";" + (SPSeries.DataPoints[i].X).ToString("R") + ";" + SPSeries.DataPoints[i].Y.ToString("G6") + ";" +  ";" );
                        }
                    }
                    else
                    {
                        if (i < CVSeries.DataPoints.Count())
                        {
                            plotString.AppendLine((PVSeries.DataPoints[i].X).ToString("R") + ";" + PVSeries.DataPoints[i].Y.ToString("G6") + ";" + ";" + ";" + (CVSeries.DataPoints[i].X).ToString("R") + ";" + CVSeries.DataPoints[i].Y.ToString("G6"));
                        }
                        else
                        {
                            plotString.AppendLine((PVSeries.DataPoints[i].X).ToString("R") + ";" + PVSeries.DataPoints[i].Y.ToString("G6") + ";" + ";" +  ";" + ";");
                        }

                    }
                }
                else
                {
                    if( i < SPSeries.DataPoints.Count())
                    {
                        if (i < CVSeries.DataPoints.Count())
                        {
                            plotString.AppendLine(( ";" +  ";" + (SPSeries.DataPoints[i].X).ToString("R") + ";" + SPSeries.DataPoints[i].Y.ToString("G6") + ";" + (CVSeries.DataPoints[i].X).ToString("R") + ";" + CVSeries.DataPoints[i].Y.ToString("G6")));
                        }
                        else
                        {
                            plotString.AppendLine(( ";" +  ";" + (SPSeries.DataPoints[i].X).ToString("R") + ";" + SPSeries.DataPoints[i].Y.ToString("G6") + ";" + ";" ));
                        }
                    }
                    else
                    {
                         if (i < CVSeries.DataPoints.Count())
                        {
                            plotString.AppendLine(( ";" +  ";" +  ";"  + ";" + (CVSeries.DataPoints[i].X).ToString("R") + ";" + CVSeries.DataPoints[i].Y.ToString("G6")));
                        }
                        else
                        {
                            plotString.AppendLine(( ";" +  ";" +  ";"  + ";" +  ";" ));
                        }
                    }
                }
            }

            return plotString;
        }

    }

    /// <summary>
    /// Klasa reprezentujaca serie danych wykresu
    /// </summary>
    public partial class RealTimeLineSeries : OxyPlot.Wpf.LineSeries
    {
        private RealTimePointsCollection points;
        /// <summary>
        /// Punkty do wyswietlania
        /// </summary>
        public RealTimePointsCollection DataPoints
        {
            get { return points; }
            private set { points = value; }
        }

        /// <summary>
        /// Maksymalna dlugosc bufora w ktorym znajduja sie punkty
        /// </summary>
        public Int32 MaxLenght
        {
            get
            {
                return DataPoints.MaxLength;
            }

            set
            {
                DataPoints.MaxLength = value;
            }
        }

        /// <summary>
        /// Konstruktor klasy serii rzeczywistej
        /// </summary>
        public RealTimeLineSeries() : base ()
        {

        }

        /// <summary>
        /// Metoda inicjalizujaca serie czasu rzeczywsitego
        /// </summary>
        /// <param name="maxLength"></param>
        public void InitRealTimeLineSeries(Int32 maxLength)
        {
            //Stworzenie kolekcji punktow
            DataPoints = new RealTimePointsCollection(maxLength);

            //Przypisanie kontekstu do wyswietlanych punktow - mechanizm wiazania danych WPF
            DataContext = this;
        }

        /// <summary>
        /// Metoda dodajaca probke do serii wykresu
        /// </summary>
        /// <param name="value">
        /// Wartosc probki
        /// </param>
        public void AddValue(Double value)
        {
            DataPoints.Add(value);
        }

        /// <summary>
        /// Metoda czyszczaca serie wykresu
        /// </summary>
        public void Clear()
        {
            DataPoints.points.Clear();
        }

    }
}
