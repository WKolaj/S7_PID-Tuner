using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace DynamicMethodsLibrary
{
    /// <summary>
    /// Interaction logic for IdentificationControl.xaml
    /// </summary>
    public partial class IdentificationControl : UserControl, INotifyPropertyChanged
    {
        public event Action OnButtonStartIdentificationClicked;
        public event Action OnButtonStopClicked;
        public event Action OnButtonPlayClicked;
        public event Action OnIdentifyClicked;
        public event Action<IdentificationMode> OnModeChanged;


        private Double iSE;
        public Double ISE
        {
            get
            {
                return iSE;
            }

            private set
            {
                iSE = value;

                NotifyPropertyChanged("ISE");
            }
        }

        private Boolean EnableIdentification
        {
            get;
            set;
        }

        private Boolean IsDiscrete
        {
            get;
            set;
        }

        private Double[] DiscretePVPoints;
        private Double[] DiscreteCVPoints;

        private SystemType choosenType;
        public SystemType ChoosenType
        {
            get
            {
                return choosenType;
            }

            private set
            {
                choosenType = value;

                if(value == SystemType.Continues)
                {
                    IsDiscrete = false;
                    continousTransferFunctionDisplay.ChangeColor("#CEE3F1");
                    continousBorder.BorderBrush = new SolidColorBrush(Colors.Black);
                    discreteTransferFunctionDisplay.ChangeColor("White");
                    discreteBorder.BorderBrush = new SolidColorBrush(Colors.White);
                }
                else if(value == SystemType.Discrete)
                {
                    IsDiscrete = true;
                    continousTransferFunctionDisplay.ChangeColor("White");
                    continousBorder.BorderBrush = new SolidColorBrush(Colors.White);
                    discreteTransferFunctionDisplay.ChangeColor("#CEE3F1");
                    discreteBorder.BorderBrush = new SolidColorBrush(Colors.Black);
                }
            }
        }


        public Action OnIdentificationModeStart;
        public Action OnSettingStarted;
        public Func<Double[], Double[], Double, TransferFunctionClass> OnIdentificationStart;

        public void EndIdentificationProcess(bool IdentificationOk)
        {
            Mode = IdentificationMode.Stop;
            EnableIdentification = IdentificationOk;
        }

        private DynamicSystem dynamicSystem;

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

        private IdentificationMode mode = IdentificationMode.Stop;
        public IdentificationMode Mode
        {
            get
            {
                return mode;
            }

            set
            {
                SetProgress(0);

                if(mode!=value && OnModeChanged != null)
                {
                    OnModeChanged(value);
                }

                mode = value;

                if (value == IdentificationMode.Identification)
                {
                    EnableIdentification = true;
                }
                else if (value == IdentificationMode.Normal)
                {
                    EnableIdentification = false;
                }


                NotifyPropertyChanged("Mode");

                RefreshMode();
                
            }

        }

        private void RefreshMode()
        {
            RefreshModeControlFilters();
            realTimePlot.RefreshMode();
        }

        private bool TryEnableModelResponseBoxEnable()
        {
            if(DiscretePVPoints!=null && DiscreteCVPoints!= null && dynamicSystem != null)
            {
                modelResponseCheckBox.IsEnabled = true;
                return true;
            }
            else
            {
                modelResponseCheckBox.IsEnabled = false;
                return false;
            }
        }

        public Boolean EnableCVBoxInIdentification
        {
            get;
            set;
        }

        private void RefreshModeControlFilters()
        {
            switch(Mode)
            {
                case IdentificationMode.Identification:
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                            {
                                NormalTrendButton.IsEnabled = true;
                                IdentificationButton.IsEnabled = false;
                                StopButton.IsEnabled = true;
                                LengthTextBox.IsEnabled = false;
                                StartIdentificationButton.IsEnabled = false;
                                modelResponseCheckBox.IsChecked = false;
                                modelResponseCheckBox.IsEnabled = false;
                                SampleTimeTextBox.IsEnabled = false;

                                if(!EnableCVBoxInIdentification)
                                {
                                    controllerOutputTextBox.IsEnabled = false;
                                }

                                saveAllButton.IsEnabled = false; 
                                loadButton.IsEnabled = false;
                            }));
                        break;
                    }

                case IdentificationMode.Normal:
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            NormalTrendButton.IsEnabled = false;
                            IdentificationButton.IsEnabled = true;
                            StopButton.IsEnabled = true;
                            LengthTextBox.IsEnabled = true;
                            StartIdentificationButton.IsEnabled = false;
                            modelResponseCheckBox.IsChecked = false;
                            modelResponseCheckBox.IsEnabled = false;
                            SampleTimeTextBox.IsEnabled = true;
                            controllerOutputTextBox.IsEnabled = true;
                            saveAllButton.IsEnabled = false;
                            loadButton.IsEnabled = false;
                        }));
                        break;
                    }
                case IdentificationMode.Stop:
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            NormalTrendButton.IsEnabled = true;
                            IdentificationButton.IsEnabled = true;
                            StopButton.IsEnabled = false;
                            LengthTextBox.IsEnabled = true;
                            
                            if(EnableIdentification)
                            {
                                StartIdentificationButton.IsEnabled = true;
                            }
                            else
                            {
                                StartIdentificationButton.IsEnabled = false;
                            }

                            TryEnableModelResponseBoxEnable();
                            SampleTimeTextBox.IsEnabled = true;
                            controllerOutputTextBox.IsEnabled = true;
                            saveAllButton.IsEnabled = true;
                            loadButton.IsEnabled = true;
                        }));
                        break;
                    }
            }
        }


        internal IdentificationWindow mainWindow;

        public IdentificationControl()
        {
            InitializeComponent();
        }

        public void Initialize(IdentificationWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            InitDataContext();

            InitializeRealTimeChart();

            RefreshModeControlFilters();

            InitTransferFunctionDisplaying();

            InitIdentificationMode();

            ConnectEvents();
        }

        private void InitDataContext()
        {
            DataContext = mainWindow;
            ISELabel1.DataContext = this;
            LengthTextBox.DataContext = realTimePlot;
        }

        private void InitIdentificationMode()
        {
            EnableIdentification = false;
            Mode = IdentificationMode.Stop;
            ChoosenType = SystemType.Continues;

        }

        private void InitTransferFunctionDisplaying()
        {
            continousTransferFunctionDisplay.Type = SystemType.Continues;
            discreteTransferFunctionDisplay.Type = SystemType.Discrete;
        }

        private void InitializeRealTimeChart()
        {
            realTimePlot.AssignControl(this);
        }

        private void SetCVTextBox(Double value)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (!controllerOutputTextBox.IsFocused)
                        {
                            controllerOutputTextBox.Text = value.ToString("G4");
                        }
                    }));
                
        }

        private void SetSampleTimeTextBox(Int32 value)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!SampleTimeTextBox.IsFocused)
                {
                    SampleTimeTextBox.Text = value.ToString();
                }
            }));
        }

        private void ConnectEvents()
        {
            mainWindow.CVUpdated += OnCVUpdated;
            mainWindow.SampleTimeUpdated += OnSampleTimeChanged;
            mainWindow.PVUpdated += OnPVUpdated;
        }

        private void OnCVUpdated(Double newCV)
        {
            SetCVTextBox(newCV);

            realTimePlot.AddRealTimeCVPoint(newCV);
        }

        private void OnPVUpdated(Double newPV)
        {
            try
            {
                realTimePlot.AddRealTimePVPoint(newPV);
            }
            catch(Exception exception)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {

                    MessageBox.Show(exception.Message);

                }));    
            }
        }

        public void OnSampleTimeChanged(Int32 Value)
        {
            SetSampleTimeTextBox(Value);
        }

        private void controllerOutputTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Double value = 0;

            if(Double.TryParse(controllerOutputTextBox.Text,out value))
            {
                mainWindow.CV = value;
            }
        }
        /// <summary>
        /// Metoda pozwalajaca na zakonczenie edycji pola po wcisniecu przycisku Enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LostFocusOnEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var scope = FocusManager.GetFocusScope(((TextBox)sender)); // elem is the UIElement to unfocus
                FocusManager.SetFocusedElement(scope, null); // remove logical focus
                Keyboard.ClearFocus();
            }
        }

        private void SampleTimeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Int32 value = 0;

            if (Int32.TryParse(SampleTimeTextBox.Text, out value))
            {
                mainWindow.SampleTime = value;
            }
        }


        private void NormalTrendButton_Click(object sender, RoutedEventArgs e)
        {
            Mode = IdentificationMode.Normal;

            if (OnButtonPlayClicked != null)
            {
                OnButtonPlayClicked();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Mode = IdentificationMode.Stop;

            if (OnButtonStopClicked != null)
            {
                OnButtonStopClicked();
            }
        }

        private void IdentificationButton_Click(object sender, RoutedEventArgs e)
        {
            EnableIdentification= false;

            Mode = IdentificationMode.Identification;

            if(OnIdentificationModeStart != null)
            {
                OnIdentificationModeStart();
            }

            if(OnButtonStartIdentificationClicked != null)
            {
                OnButtonStartIdentificationClicked();
            }
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            if(OnSettingStarted != null)
            {
                OnSettingStarted();
            }
        }

        public void RefreshDiscretePoints()
        {
            Double[][] points = realTimePlot.GetDiscretePoints();

            DiscreteCVPoints = points[0];
            DiscretePVPoints = points[1];

        }

        private void StartIdentificationButton_Click(object sender, RoutedEventArgs e)
        {
            if(OnIdentifyClicked != null)
            {
                OnIdentifyClicked();
            }

            if (OnIdentificationStart != null)
            {
                try
                {
                    RefreshDiscretePoints();

                    TransferFunctionClass tf = OnIdentificationStart(DiscreteCVPoints, DiscretePVPoints, mainWindow.SampleTime);

                    if (tf.TransferFunctionType == TransferFunctionType.Continous)
                    {
                        ChoosenType = SystemType.Continues;
                    }
                    else if (tf.TransferFunctionType == TransferFunctionType.Discrete)
                    {
                        ChoosenType = SystemType.Discrete;
                    }

                    dynamicSystem = tf.ToDynamicSystem();

                    RefreshDynamicSystemDisplaying();

                    TryEnableModelResponseBoxEnable();
                    modelResponseCheckBox.IsChecked = false;
                    modelResponseCheckBox.IsChecked = true;
                }
                catch(Exception exception)
                {
                    System.Windows.MessageBox.Show(exception.Message, "Error during identification", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshDynamicSystemDisplaying()
        {
            continousTransferFunctionDisplay.DynamicSystemObject = dynamicSystem;
            discreteTransferFunctionDisplay.DynamicSystemObject = dynamicSystem;
        }

        private void saveAllButton_Click(object sender, RoutedEventArgs e)
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
                    writer.Write(realTimePlot.PointsToString());

                    writer.Flush();
                    writer.Close();
                }
                catch (Exception exception)
                {
                    System.Windows.MessageBox.Show(exception.Message, "Error during saving", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExitWithoutSaving()
        {
            mainWindow.DialogResult = false;
        }

        private void ExitAndSave()
        {
            if(dynamicSystem != null)
            {

                if(IsDiscrete)
                {
                    mainWindow.EndIdentification(dynamicSystem.DiscreteNumerator, dynamicSystem.DiscreteDenumerator, dynamicSystem.DiscreteTimeDelay, true, Convert.ToInt32(dynamicSystem.SimulationSampleTime*1000));
                }
                else
                {
                    mainWindow.EndIdentification(dynamicSystem.ContinousNumerator, dynamicSystem.ContinousDenumerator, dynamicSystem.ContinousTimeDelay, false, Convert.ToInt32(dynamicSystem.SimulationSampleTime * 1000));
                }
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ExitAndSave();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ExitWithoutSaving();
        }

        private List<DataPoint> SimulateModel(Double[] CV, Double[] PV, Int32 sampleTime, DateTime startTime )
        {
            ISE = 0;

            Double[] points = dynamicSystem.Simulate(CV, CV[0], PV[0]);

            List<DataPoint> pointsToReturn = new List<DataPoint>();

            DateTime time = startTime;

            for(int i=0; i<points.Length; i++)
            {
                pointsToReturn.Add(DateTimeAxis.CreateDataPoint(time,points[i]));

                ISE += (points[i] - PV[i]) * (points[i] - PV[i]) * (Convert.ToDouble(sampleTime) / 1000) * (Convert.ToDouble(sampleTime) / 1000);

                time = time.AddMilliseconds(sampleTime);
            }

            return pointsToReturn;
        }

        private void continousTransferFunctionDisplay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChoosenType = SystemType.Continues;
        }

        private void discreteTransferFunctionDisplay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChoosenType = SystemType.Discrete;
        }

        private void modelResponseCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RefreshDiscretePoints();

            ISELabel2.Visibility = System.Windows.Visibility.Visible;
            ISELabel1.Visibility = System.Windows.Visibility.Visible;

            realTimePlot.ShowModelCollection(SimulateModel(DiscreteCVPoints, DiscretePVPoints, mainWindow.SampleTime, realTimePlot.StartModelDateTimePoint));
        }

        private void modelResponseCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ISELabel2.Visibility = System.Windows.Visibility.Hidden;
            ISELabel1.Visibility = System.Windows.Visibility.Hidden;

            realTimePlot.HideModelCollection();
        }

        private void loadAllButton_Click(object sender, RoutedEventArgs e)
        {
            //Otwarcie okna do wskazania pliku
            System.Windows.Forms.OpenFileDialog readFileDialogWindow = new System.Windows.Forms.OpenFileDialog();

            readFileDialogWindow.Filter = "CSV Files (*.CSV)|*.CSV";
            readFileDialogWindow.FilterIndex = 0;
            readFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plots");

            //Jezeli okno to zostalo zaakceptowane - nalezy wczytac zawartosc pliku
            if (readFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                realTimePlot.ReadFromFile(readFileDialogWindow.OpenFile());
            }

            modelResponseCheckBox.IsChecked = false;

            if (realTimePlot.PVPoints.Count > 0 && realTimePlot.CVPoints.Count > 0)
            {
                EnableIdentification = true;
                modelResponseCheckBox.IsChecked = false;
                RefreshModeControlFilters();
            }
        }

        public void SetProgress(Double Percent)
        {
            if(Percent > 100)
            {
                Percent = 100;
            }

            if(Percent < 0)
            {
                Percent = 0;
            }

            Dispatcher.BeginInvoke(new Action(() =>
                {
                    prograssBar.Value = Percent;
                }));
        }

        private void NormalizeDiscreteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            discreteTransferFunctionDisplay.Normalize();
            continousTransferFunctionDisplay.RefreshTransferFunctionDisplaying();
        }

        private void NormalizeContinousMenuItem_Click(object sender, RoutedEventArgs e)
        {
            continousTransferFunctionDisplay.Normalize();
            discreteTransferFunctionDisplay.RefreshTransferFunctionDisplaying();
        }

    }
}
