
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
using DynamicMethodsLibrary;
using System.Diagnostics;
using TransferFunctionLib;

namespace LSMethod
{
    [Method("LS Method", "LSMethod.png")]
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IdentificationWindow
    {


        private LSFactory lsFactor;
        public LSFactory LSFactory
        {
            get
            {
                return lsFactor;
            }

            protected set
            {
                lsFactor = value;
            }
        }

        private SignalFactor signalFactor = new SignalFactor();

        private InputSignal inputSignal;

        public MainWindow()
        {
            InitializeComponent();
            identificationControl.Initialize(this);
            ConnectSignals();
            DataContext = this;
            InitializeLSFactor();
        }

        public void InitializeLSFactor()
        {
            this.LSFactory = new LSFactory(1,1,new Double[0],new Double[0],0,Convert.ToDouble(SampleTime)/1000);

            this.LSFactory.IdentificationProgressChanged += OnLSMethodProgressChanged;
            this.LSFactory.IdentificationStarted +=OnLSMethodStarted;
            this.LSFactory.IdentificationStopped += OnLSMethodStopped;

            this.LSFactory.AutoDelayCalculationPercentageChanged += OnAutoDelayProgressChanged;
            this.LSFactory.CalculationAutoDelayStarted += OnAutoDelayCalculationStarted;
            this.LSFactory.CalculationAutoDelayStopped +=OnAutoDelayCalculationStopped;

            this.yRankTextBox.DataContext = this.lsFactor;
            this.uRankTextBox.DataContext = this.lsFactor;
            this.timeDelayTextBox.DataContext = this.lsFactor;
            this.startTimeTextBox.DataContext = this.lsFactor;
            this.endTimeTextBox.DataContext = this.lsFactor;

            identificationControl.EnableCVBoxInIdentification = true;

        }

        private void BuildSignal()
        {
            SignalBuilderWindow signalBuilder = new SignalBuilderWindow();

            signalBuilder.AssignSignalFactor(signalFactor);

            bool? result = signalBuilder.ShowDialog();

            if ((bool)result)
            {
                inputSignal = signalFactor.BuildSignal();
                ConnectSignalEvents();
            }
        }


        public void ConnectSignals()
        {
            identificationControl.OnIdentificationModeStart = OnIdentificationProcessStarted;
            identificationControl.OnModeChanged += OnModeChange;
            identificationControl.OnIdentificationStart = OnIdentificationStarted;
            identificationControl.OnSettingStarted = BuildSignal;
            
        }

        public void OnModeChange(IdentificationMode newMode)
        {
            if(newMode == IdentificationMode.Stop)
            {
                if(inputSignal != null)
                {
                    inputSignal.Stop();
                }

            }
            else if(newMode == IdentificationMode.Identification)
            {

                
            }
            else if(newMode == IdentificationMode.Normal)
            {

            }

        }

        public void RefreshButtonEnable(IdentificationMode newMode)
        {
            if (newMode == IdentificationMode.Stop)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    identificationControl.settingsButton.IsEnabled = true;

                }));
            }
            else if (newMode == IdentificationMode.Identification)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    identificationControl.settingsButton.IsEnabled = false;

                }));
            }
            else if (newMode == IdentificationMode.Normal)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    identificationControl.settingsButton.IsEnabled = false;

                }));
            }
        }

        public void OnPercentageChange(Double value)
        {
            identificationControl.SetProgress(value);
            
        }

        public void ConnectSignalEvents()
        {
            if(inputSignal != null)
            {
                inputSignal.OnSampleTimeValueUpdate = OnSampleTimeValueUpdated;
                inputSignal.InputSourceEnded += OnIdentificationProcessEnd;
                inputSignal.OnPercentageChange += OnPercentageChange;

            }
        }

        public void OnLSMethodProgressChanged(Double progress)
        {

        }

        public void OnAutoDelayProgressChanged(Double progress)
        {

        }

        public void OnAutoDelayCalculationStarted()
        {

        }

        public void OnAutoDelayCalculationStopped()
        {
        }

        public void OnLSMethodStarted()
        {
        }

        public void OnLSMethodStopped()
        {

        }

        public TransferFunctionClass OnIdentificationStarted(Double[] CVPoints, Double[] PVPoints, Double SampleTime)
        {
            this.LSFactory.AssignPoints(CVPoints, PVPoints);
            this.LSFactory.SampleTime = SampleTime;

            IdentificationProcessWindow identificationWindow = new IdentificationProcessWindow();
            identificationWindow.Owner = this;
            identificationWindow.InitializeProcessWindow(LSFactory);

            identificationWindow.ShowDialog();

            return identificationWindow.TransferFunction;
        }

        public void OnIdentificationProcessStarted()
        {
            if (signalFactor.SignalType == SignalType.Manual)
            {
                identificationControl.EnableCVBoxInIdentification = true;
            }
            else
            {
                identificationControl.EnableCVBoxInIdentification = false;
            }

            if(inputSignal!= null)
            {
                

                inputSignal.Start(CV);

            }
        }

        public void OnSampleTimeValueUpdated(Double value)
        {
            CV = value;
        }

        public void OnIdentificationProcessEnd()
        {
            identificationControl.EndIdentificationProcess(true);
            identificationControl.Mode = IdentificationMode.Stop;
        }

        private void timeDelayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.lsFactor.AutoTimeDelay = true;
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    timeDelayTextBox.IsEnabled = false;
                    AutoTimeDelayStackPanel.Visibility = System.Windows.Visibility.Visible;
                }));
        }


        private void timeDelayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.lsFactor.AutoTimeDelay = false;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                timeDelayTextBox.IsEnabled = true;
                AutoTimeDelayStackPanel.Visibility = System.Windows.Visibility.Hidden;
            }));
        }

        private void modelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(modelComboBox.SelectedIndex == 0)
            {
                if(lsFactor!= null)
                {
                    lsFactor.TypeOfMethod = LSMethodType.Normal;
                }
            }
            else if (modelComboBox.SelectedIndex == 1)
            {
                if (lsFactor != null)
                {
                    lsFactor.TypeOfMethod = LSMethodType.Extended;
                }
            }
        }

    }
}
