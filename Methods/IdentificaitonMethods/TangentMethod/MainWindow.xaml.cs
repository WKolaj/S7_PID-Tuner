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
using TransferFunctionLib;
using DynamicMethodsLibrary;

namespace Tangent_method
{
    [Method("Tangent Method", "Styczna.png")]
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IdentificationWindow
    {
        public Double StepSize
        {
            get
            {
                return identificationPulseFactory.StepValue;
            }
            set
            {
                identificationPulseFactory.StepValue = value;
                NotifyPropertyChanged("StepSize");
            }
        }

        private Int32 numberOfAproximationPoints = 5;
        public Int32 NumberOfAproximationPoints
        {
            get
            {
                return numberOfAproximationPoints;
            }
            set
            {
                numberOfAproximationPoints = value;
                NotifyPropertyChanged("NumberOfAproximationPoints");
            }
        }

        private IdentificationPulseFactory identificationPulseFactory;

        public MainWindow()
        {
            InitializeComponent();

            identificationControl.Initialize(this);

            InitializePulseFactory();

            identificationControl.OnSettingStarted = OnSettingsClicked;
            identificationControl.OnIdentificationModeStart = OnIdentificationProcessStarted;
            SampleTimeTick += OnSampleTimeTick;
            identificationControl.OnModeChanged += OnModeChange;
            identificationControl.OnIdentificationStart += OnIdentificationCalculation;

        }

        public void OnSettingsClicked()
        {
            ShowSettingsWindow();
        }

        public void OnIdentificationProcessStarted()
        {
            identificationPulseFactory.Start(PV, CV);
            StartSampling();
        }

        public void ShowSettingsWindow()
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.InitializeWindow(this);

            settingsWindow.ShowDialog();
        }

        public TransferFunctionClass OnIdentificationCalculation(Double[] CVPoints, Double[] PVPoints, Double SampleTime)
        {
            FirstRankAproximator aproximator = new FirstRankAproximator(CVPoints, PVPoints, NumberOfAproximationPoints, SampleTime/1000);

            return aproximator.GetTransferFunction();
        }

        public void OnModeChange(IdentificationMode newMode)
        {
            if (newMode != IdentificationMode.Identification)
            {
                StopSampling();
                identificationPulseFactory.Stop();
            }
        }

        public void OnSampleTimeTick(DateTime dateTime)
        {
            identificationPulseFactory.OnTimerTick(dateTime, PV);
        }

        public void InitializePulseFactory()
        {
            identificationPulseFactory = new IdentificationPulseFactory(this, 10.0);
            identificationPulseFactory.IdentificationProcessStopped += OnIdentificationStopped;
        }

        public void OnIdentificationStopped()
        {
            if(identificationControl.Mode == IdentificationMode.Identification)
            {
                StopSampling();
                identificationControl.EndIdentificationProcess(true);
                identificationControl.Mode = IdentificationMode.Stop;

            }
        }
    }
}
