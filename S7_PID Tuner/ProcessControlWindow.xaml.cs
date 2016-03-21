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
using System.Windows.Shapes;
using OxyPlot;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using OxyPlot.Axes;

namespace S7_PID_Tuner
{
    /// <summary>
    /// Interaction logic for ControlProcessWindow.xaml
    /// </summary>
    public partial class ProcessControlWindow : Window
    {
        private PerformanceIndexCollection performanceIndexes;
        public PerformanceIndexCollection PerformanceIndexes
        {
            get
            {
                return performanceIndexes;
            }

            set
            {
                this.performanceIndexes = value;
            }
        }

        private List<PerfomanceIndexControl> performanceIndexControls = new List<PerfomanceIndexControl>();
        public List<PerfomanceIndexControl> PerfomanceIndexControls
        {
            get
            {
                return performanceIndexControls;
            }

            set
            {
                performanceIndexControls = value;
            }
        }

        private void InitPerformanceIndexesControls()
        {
            foreach(var index in performanceIndexes.PerformanceIndexList)
            {
                PerfomanceIndexControl indexControl = new PerfomanceIndexControl();
                indexControl.Init(index);
                PerfomanceIndexControls.Add(indexControl);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    indexes.Children.Add(indexControl);
                }));
            }

            
        }

        public void RefreshPerformanceIndexesControls()
        {
            DynamicMethodsLibrary.PointApproximator aproximator = new DynamicMethodsLibrary.PointApproximator(realTimePlot.PVSeries.DataPoints.ToList(), realTimePlot.SPSeries.DataPoints.ToList(), controllerDevice.IdentificationSampleTime);
            DateTime startTime = new DateTime();
            Double[][] collection = aproximator.GetNormalizedCollections(out startTime);
            foreach(var indexControl in PerfomanceIndexControls)
            {
                indexControl.AssignPVAndSP(collection[0], collection[1], controllerDevice.IdentificationSampleTime);

                if(indexControl.IsChecked)
                {
                    indexControl.Refresh();
                }
            }
        }

        /// <summary>
        /// Obiekt sterownika poprzez ktory nastepuje polaczenie aplikacji C# ze sterownikiem S7
        /// </summary>
        private PIDControllerDevice controllerDevice;

        private bool online;
        /// <summary>
        /// Wlasciwosc determinujaca czy zostal wlaczony tryb online
        /// </summary>
        private bool Online
        {
            get
            {
                return online;
            }

            set
            {
                online = value;
            }
        }

        /// <summary>
        /// Wlasciwosc determinujaca czy nawiazano polaczenie ze sterownikiem
        /// </summary>
        private bool Connected
        {
            get
            {
                return controllerDevice.Connected;
            }
        }

        /// <summary>
        /// Tryb pracy obiektu sterownika - Tuning/Normal
        /// </summary>
        private bool TuningMode
        {
            get
            {
                return controllerDevice.TuningMode;
            }
        }

        /// <summary>
        /// Konstruktor klasy okna ukladu regulacji
        /// </summary>
        public ProcessControlWindow()
        {
            InitializeComponent();
            DataContext = realTimePlot;
        }

        /// <summary>
        /// Metoda przypisujaca obiekt sterownika do okna
        /// </summary>
        /// <param name="controllerDevice">
        /// Obiekt sterownika
        /// </param>
        public void AssignControllerToDevice(PIDControllerDevice controllerDevice)
        {
            this.controllerDevice = controllerDevice;

            //Polaczenie zdarzen sterownika z kontrolka
            ConnectDeviceEvents();

            //Przypisanie kolekcji do mechanizmu wiazania danych
            onlineStatusLabel.DataContext = controllerDevice;
            simulationSampleTimeTextBox.DataContext = controllerDevice;
        }

        /// <summary>
        /// Metoda przypisujaca obiekt sterownika do okna
        /// </summary>
        /// <param name="controllerDevice">
        /// Obiekt sterownika
        /// </param>
        public void AssignIndexesToDevice(PerformanceIndexCollection performanceIndexes)
        {
            this.PerformanceIndexes = performanceIndexes;

            InitPerformanceIndexesControls();
        }

        /// <summary>
        /// Metoda inicjalizujaca tryb Online
        /// </summary>
        public void InitOnlineMode()
        {
            //Jezeli sterownik jest polaczony i jest w trybie online
            if (Connected && Online)
            {
                try
                {
                    //Wlaczenie trybu Tunning
                    controllerDevice.TurnOnTuningMode();
                }
                catch (Exception exception)
                {
                    //Jezeli nie udalo sie wlaczyc trybu tuning - przejscie offline
                    GoOffline();
                    System.Windows.MessageBox.Show(exception.Message, "Unable to turn on tuning mode", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        /// <summary>
        /// Metoda laczaca zdarzenia z metodami ich obslugi
        /// </summary>
        public void ConnectDeviceEvents()
        {
            controllerDevice.ConnectionStateChanged += OnDeviceConnectionChanged;
            controllerDevice.TuningModeStateChanged += OnTunningModeChanged;

            controllerDevice.SetpointUpdated += OnSetpointUpdated;
            controllerDevice.ProcessValueUpdated += OnProcessValueUpdated;
            controllerDevice.ManualOutputUpdated += OnManualValueUpdated;
            controllerDevice.ControllerOutputUpdated += OnControllerOutputUpdated;
            controllerDevice.ControllerModeUpdated += OnModeVariableUpdated;
        }

        /// <summary>
        /// Metoda rozlaczajaca zdarzenia z metodami ich obslugi
        /// </summary>
        public void DisconnectDeviceEvents()
        {
            controllerDevice.ConnectionStateChanged -= OnDeviceConnectionChanged;
            controllerDevice.TuningModeStateChanged -= OnTunningModeChanged;

            controllerDevice.SetpointUpdated -= OnSetpointUpdated;
            controllerDevice.ProcessValueUpdated -= OnProcessValueUpdated;
            controllerDevice.ManualOutputUpdated -= OnManualValueUpdated;
            controllerDevice.ControllerOutputUpdated -= OnControllerOutputUpdated;
            controllerDevice.ControllerModeUpdated -= OnModeVariableUpdated;
        }

        /// <summary>
        /// Metoda wywolana w przypadku zgloszenia zdarzenia zmiany stanu polaczenia
        /// </summary>
        public void OnDeviceConnectionChanged()
        {
            //Jezeli jest polaczenie i jest w trybie online - inicjalizacja trybu Online
            if(Connected && Online)
            {
                InitOnlineMode();
            }
            else
            {
                //Jezeli nie - nalezy wyswietlic animacje ktora wygasi obwodu przyciskow manual/auto
                PlayDifferentModeAnimation();
            }

            //Odswiezenie kolorow
            RefreshOnlineColor();

        }

        /// <summary>
        /// Metoda odswiezajaca kolory okna w zaleznosci od trybu w jakim sie znajduje
        /// </summary>
        public void RefreshOnlineColor()
        {
            //Jezeli sterownik jest polaczony i w trybi Tuning oraz jest wlaczony tryb online okna - rozne animacje 
            if (Connected && Online && TuningMode)
            {
                PlayOnlineModeOnAnimation();
            }
            else
            {
                PlayOnlineModeOffAnimation();
            }
        }

        /// <summary>
        /// Metoda uruchamiajaca animacje trybu Online
        /// </summary>
        public void PlayOnlineModeOnAnimation()
        {
            Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Storyboard processPlantMouseOver = (Storyboard)Resources["onlineModeOn"];
                            processPlantMouseOver.Begin();
                        }));
        }
        
        /// <summary>
        /// Metoda uruchamiajaca animacje trybu offline
        /// </summary>
        public void PlayOnlineModeOffAnimation()
        {
            Dispatcher.BeginInvoke(new Action(() =>
                       {
                           Storyboard processPlantMouseOver = (Storyboard)Resources["onlineModeOff"];
                           processPlantMouseOver.Begin();
                       }));
        }

        /// <summary>
        /// Metoda obslugi zdarzenia wcisniecia przycisku GoOnline
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOnline_Click(object sender, RoutedEventArgs e)
        {
            GoOnline();
        }

        /// <summary>
        /// Metoda przejscia w tryb obline
        /// </summary>
        public void GoOnline()
        {
            //Zmiana trybu okna
            Online = true;

            if(!Connected)
            {
                //Jezeli nie jest ustanowione polaczenie - nalezy je nawiazac - inicjalizacja trybu w metodzie oblsugi zdarzenia ConnectionStateChanged
                controllerDevice.StatusMessage = "Trying to connect..";
                
                controllerDevice.Connect();
            }
            else
            {   //Jezeli jest juz ustanowione polaczenie - inicjalizujemy tryb online
                InitOnlineMode();
            }
        }

        /// <summary>
        /// Metoda przejscia w tryb offline
        /// </summary>
        public void GoOffline()
        {
            //Jezeli jest aktywne polaczenie - nalezy wyjsc z trybu Tuning i rozlaczyc aplikacje
            if (Connected)
            {
                Online = false;
                controllerDevice.TurnOffTuningMode();
                controllerDevice.Disconnect();
            }

            //Odswizenie koloru 
            PlayDifferentModeAnimation();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia wcisniecia przycisku Offline
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOffline_Click(object sender, RoutedEventArgs e)
        {
            GoOffline();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia odswiezenia trybu regulatora Auto/Manual
        /// </summary>
        public void OnModeVariableUpdated()
        {
            if(Online && Connected)
            {
                if (controllerDevice.Mode == 3)
                {
                    PlayAutoModeAnimation();
                }
                else if (controllerDevice.Mode == 4)
                {
                    PlayManualModeAnimation();
                }
                else
                {
                    PlayDifferentModeAnimation();
                }
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zmiany trybu Tunning
        /// </summary>
        public void OnTunningModeChanged()
        {
            //Jezeli tryb nie jest Tuning - nalezy przejsc w tryb offline
            if (!TuningMode && Online)
            {
                GoOffline();
            }

            //Odswiezenie kolorow
            RefreshOnlineColor();
        }

        /// <summary>
        /// Metoda uruchamiajaca animacje trybu recznego
        /// </summary>
        public void PlayManualModeAnimation()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Storyboard animation = (Storyboard)Resources["onManualMode"];
                animation.Begin();
            }));
        }

        /// <summary>
        /// Metoda uruchamiajaca animacje trybu automatycznego
        /// </summary>
        public void PlayAutoModeAnimation()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Storyboard animation = (Storyboard)Resources["onAutoMode"];
                animation.Begin();
            }));
        }

        /// <summary>
        /// Metoda uruchamiajaca animacje trybu innego niz auto/reka - wygaszajaca podswietlenie przyciskow
        /// </summary>
        public void PlayDifferentModeAnimation()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Storyboard animation = (Storyboard)Resources["onDifferentMode"];
                animation.Begin();
            }));
        }

        /// <summary>
        /// Metoda obslugi zdarzenia odswiezenia wartosci zadanej
        /// </summary>
        public void OnSetpointUpdated()
        {
            //Odswiezenie teksboxu
            Dispatcher.BeginInvoke(new Action(() =>
                        {
                            //Jezeli jest aktywne pisanie - nie nalezy zmieniac zawartosci
                            if (!setpointTextBox.IsFocused)
                            {
                                setpointTextBox.Text = controllerDevice.Setpoint.ToString("G4");
                            }
                        }));

            //Dodanie wartosci do wykresu
            realTimePlot.AddNewSPValue(controllerDevice.Setpoint);
        }

        /// <summary>
        /// Metoda obslugi zdarzenia odswiezenia wartosci mierzonej
        /// </summary>
        public void OnProcessValueUpdated()
        {
            //Odswiezenie teksboxu
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!processValueTextBox.IsFocused)
                {
                    processValueTextBox.Text = controllerDevice.ProcessValue.ToString("G4");
                }
            }));

            //Dodanie wartosci do wykresu
            realTimePlot.AddNewPVValue(controllerDevice.ProcessValue);
        }

        /// <summary>
        /// Metoda obslugi zdarzenia odswiezenia wartosci wyjscia regulatora 
        /// </summary>
        public void OnControllerOutputUpdated()
        {
            //Odswiezenie teksboxu
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!controllerOutputTextBox.IsFocused)
                {
                    controllerOutputTextBox.Text = controllerDevice.ControllerOutput.ToString("G4");
                }
            }));

            realTimePlot.AddNewCVValue(controllerDevice.ControllerOutput);
        }

        /// <summary>
        /// Metoda obslugi zdarzenia odswiezenia wartosci wyjscia regulatora w trybie recznym
        /// </summary>
        public void OnManualValueUpdated()
        {
            //Odswiezenie teksboxu
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //Jezeli jest aktywne pisanie - nie nalezy zmieniac zawartosci
                if (!manualValueTextBox.IsFocused)
                {
                    manualValueTextBox.Text = controllerDevice.ManualValue.ToString("G4");
                }
            }));
        }

        /// <summary>
        /// Metoda obslugi zdarzenia wcisniecia przycisku Play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            realTimePlot.Start();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia wcisniecia przycisku Stop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            realTimePlot.Stop();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia wcisniecia przycisku Clear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            realTimePlot.Clear();
        }

        /// <summary>
        /// Zdarzenia zakonczenia recznej edycji texboxu SP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setpointTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Connected && Online)
            {
                Double value;
                if (Double.TryParse(setpointTextBox.Text.Replace(".", ","), out value))
                {
                    controllerDevice.Setpoint = value;
                }
            }
        }

        /// <summary>
        /// Zdarzenia zakonczenia recznej edycji texboxu wyjscia sterownika w trybie recznym
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void manualValueTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

            if (Connected && Online)
            {
                Double value;
                if (Double.TryParse(manualValueTextBox.Text.Replace(".", ","), out value))
                {
                    controllerDevice.ManualValue = value;
                }
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia wcisniecia trybu Auto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoButton_Click(object sender, RoutedEventArgs e)
        {
            if (Connected && Online)
            {
                controllerDevice.Mode = 3;
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia wcisniecia trybu Manual
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualButton_Click(object sender, RoutedEventArgs e)
        {
            if(Connected && Online)
            {
                controllerDevice.Mode = 4;
            }
        }

        /// <summary>
        ///  Metoda obslugi zdarzenia wcisniecia przycisku Save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SavePlot();
        }

        /// <summary>
        /// Metoda zapisujaca wykres
        /// </summary>
        private void SavePlot()
        {
            //Otwarcie okna do zapisu pliku 
            System.Windows.Forms.SaveFileDialog saveFileDialogWindow = new System.Windows.Forms.SaveFileDialog();
            Stream writeStream;
            //Ustawienie filtra wyswietlanych w tym oknie wartosci
            saveFileDialogWindow.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialogWindow.FilterIndex = 0;
            //Ustawienie domyslnej sciezki tego okno jako projekt\Plots
            saveFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plots");

            if (saveFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    writeStream = saveFileDialogWindow.OpenFile();
                    StreamWriter writer = new StreamWriter(writeStream);
                    writer.Write(realTimePlot.ToTrendString());

                    writer.Flush();
                    writer.Close();
                }
                catch (Exception exception)
                {
                    System.Windows.MessageBox.Show(exception.Message, "Unable to turn on tuning mode", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Metoda pozwalajaca na zakonczenie edycji pola po wcisniecu przycisku Enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LostFocusOnEnter(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)   
            {
                var scope = FocusManager.GetFocusScope(((TextBox)sender)); // elem is the UIElement to unfocus
                FocusManager.SetFocusedElement(scope, null); // remove logical focus
                Keyboard.ClearFocus(); 
            }
        }

        private void RefreshIndexexButton_Click(object sender, RoutedEventArgs e)
        {
            if (realTimePlot.PVSeries.DataPoints.Count() != 0 && realTimePlot.SPSeries.DataPoints.Count() != 0)
            {
                RefreshPerformanceIndexesControls();
            }
        }

        public void ReadFromFile(Stream file)
        {
            realTimePlot.Clear();


            try
            {
                StreamReader reader = new StreamReader(file);


                List<string> allLines = new List<string>();
            

                while (!reader.EndOfStream)
                {
                    allLines.Add(reader.ReadLine());
                }

                file.Close();
                reader.Close();

                realTimePlot.Length = allLines.Count;

                foreach(var line in allLines)
                {
                    String[] values = line.Split(';');

                    if (!String.IsNullOrEmpty(values[0]) && !String.IsNullOrEmpty(values[1]))
                    {
                        realTimePlot.PVSeries.DataPoints.Add(DateTimeAxis.CreateDataPoint(DateTimeAxis.ToDateTime(Convert.ToDouble(values[0])), Convert.ToDouble(values[1])));
                    }

                    if (!String.IsNullOrEmpty(values[2]) && !String.IsNullOrEmpty(values[3]))
                    {
                        realTimePlot.SPSeries.DataPoints.Add(DateTimeAxis.CreateDataPoint(DateTimeAxis.ToDateTime(Convert.ToDouble(values[2])), Convert.ToDouble(values[3])));
                    }

                    if (!String.IsNullOrEmpty(values[4]) && !String.IsNullOrEmpty(values[5]))
                    {
                        realTimePlot.CVSeries.DataPoints.Add(DateTimeAxis.CreateDataPoint(DateTimeAxis.ToDateTime(Convert.ToDouble(values[4])), Convert.ToDouble(values[5])));
                    }
                }

                realTimePlot.RefreshPlot();

            }
            catch (Exception exception)
            {
                realTimePlot.Clear();
                System.Windows.MessageBox.Show(exception.Message, "Cannot read from file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            //Otwarcie okna do wskazania pliku
            System.Windows.Forms.OpenFileDialog readFileDialogWindow = new System.Windows.Forms.OpenFileDialog();

            readFileDialogWindow.Filter = "CSV Files (*.CSV)|*.CSV";
            readFileDialogWindow.FilterIndex = 0;
            readFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plots");

            //Jezeli okno to zostalo zaakceptowane - nalezy wczytac zawartosc pliku
            if (readFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ReadFromFile(readFileDialogWindow.OpenFile());
            }

        }

        private string PreparePerformanceFileString()
        {
            StringBuilder builder = new StringBuilder();

            foreach(var index in performanceIndexControls)
            {
                if(index.IsChecked)
                {
                    builder.AppendLine(String.Format("{0};{1}", index.PerformanceIndex.Name, index.PerformanceIndex.Value));
                }
            }


            return builder.ToString();
        }

        private void SaveIndexes()
        {
            //Otwarcie okna do zapisu pliku 
            System.Windows.Forms.SaveFileDialog saveFileDialogWindow = new System.Windows.Forms.SaveFileDialog();
            Stream writeStream;
            //Ustawienie filtra wyswietlanych w tym oknie wartosci
            saveFileDialogWindow.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialogWindow.FilterIndex = 0;
            //Ustawienie domyslnej sciezki tego okno jako projekt\Plots
            saveFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Performance index calculations");

            if (saveFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    writeStream = saveFileDialogWindow.OpenFile();
                    StreamWriter writer = new StreamWriter(writeStream);
                    writer.Write(PreparePerformanceFileString());

                    writer.Flush();
                    writer.Close();
                }
                catch (Exception exception)
                {
                    System.Windows.MessageBox.Show(exception.Message, "Unable to save indexes", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void saveTrendButton_Click(object sender, RoutedEventArgs e)
        {
            SaveIndexes();
        }

        private void saveAllButton_Click(object sender, RoutedEventArgs e)
        {
            CalculateSomeIndexes();
        }

        private void CalculateSomeIndexes()
        {
            String[] files = OpenFolderBrowser();

            if(files == null)
            {
                return;
            }

            StringBuilder newFile = new StringBuilder();

            newFile.Append("File name;");

            foreach (var index in performanceIndexControls)
            {
                if (index.IsChecked)
                {
                    newFile.Append(index.PerformanceIndex.Name + ";");
                }
            }

            newFile.Append("\n");

            foreach(var file in files)
            {
                ReadFromFile(File.OpenRead(file));

                newFile.Append(System.IO.Path.GetFileNameWithoutExtension(file) +";");

                if (realTimePlot.PVSeries.DataPoints.Count() != 0 && realTimePlot.SPSeries.DataPoints.Count() != 0)
                {
                    RefreshPerformanceIndexesControls();

                    foreach (var index in performanceIndexControls)
                    {
                        if (index.IsChecked)
                        {
                            newFile.Append(index.PerformanceIndex.Value.ToString() + ";");
                        }
                    }
                }

                newFile.Append("\n");

            }

            //Otwarcie okna do zapisu pliku 
            System.Windows.Forms.SaveFileDialog saveFileDialogWindow = new System.Windows.Forms.SaveFileDialog();
            Stream writeStream;
            //Ustawienie filtra wyswietlanych w tym oknie wartosci
            saveFileDialogWindow.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialogWindow.FilterIndex = 0;
            //Ustawienie domyslnej sciezki tego okno jako projekt\Plots
            saveFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Performance index calculations");

            if (saveFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    writeStream = saveFileDialogWindow.OpenFile();
                    StreamWriter writer = new StreamWriter(writeStream);
                    writer.Write(newFile.ToString());

                    writer.Flush();
                    writer.Close();
                }
                catch (Exception exception)
                {
                    System.Windows.MessageBox.Show(exception.Message, "Unable to save indexes", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private string[] OpenFolderBrowser()
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fbd.ShowDialog();

            if(result == System.Windows.Forms.DialogResult.Cancel)
            {
                return null;
            }
            return Directory.GetFiles(fbd.SelectedPath);
        }
    }
}
