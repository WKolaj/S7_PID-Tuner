using DynamicMethodsLibrary;
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

namespace StrejcMethod
{
    [Method("Strejc Method", "Strejc.png")]
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IdentificationWindow
    {
        Double CVOperationPoint
        {
            get;
            set;
        }

        Double CVAfterStep
        {
            get;
            set;
        }


        private Double identificationLength = 100;
        public Double IdentificationLength
        {
            get
            {
                return identificationLength;
            }
            set
            {
                identificationLength = value;
                NotifyPropertyChanged("IdentificationLength");

            }
        }

        private Int32 sensitivity = 5;
        public Int32 Sensitivity
        {
            get
            {
                return sensitivity;
            }
            set
            {
                if(value >= 1 )
                {
                    sensitivity = value;
                }

                NotifyPropertyChanged("Sensitivity");
               
            }
        }

        private Int32 numberOfAproxxPoints = 10;
        public Int32 NumberOfAproxxPoints
        {
            get
            {
                return numberOfAproxxPoints;
            }
            set
            {
                if (value >= 1)
                {
                    numberOfAproxxPoints = value;
                }

                NotifyPropertyChanged("NumberOfAproxxPoints");

            }
        }

        public void ConnectEvents()
        {
            identificationControl.OnSettingStarted = OnSettingsClicked;
            identificationControl.OnIdentificationModeStart = OnIdentificationProcessStarted;
            SampleTimeTick += OnSampleTimeTick;
            identificationControl.OnModeChanged += OnModeChange;
            identificationControl.OnIdentificationStart += OnIdentificationCalculation;
        }

        private Double stepSize = 10;
        public Double StepSize
        {
            get
            {
                return stepSize;
            }
            set
            {
                stepSize = value;
                NotifyPropertyChanged("StepSize");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            InitMethod();
        }

        public void InitMethod()
        {
            identificationControl.Initialize(this);
            ConnectEvents();
        }

        public void ShowSettingsWindow()
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.InitializeWindow(this);

            settingsWindow.ShowDialog();
        }

        public void OnSettingsClicked()
        {
            ShowSettingsWindow();
        }


        public void OnIdentificationProcessStarted()
        {
            CVOperationPoint = CV;
            CVAfterStep = CV + StepSize;
            CalculateTime();
            StartSampling();
        }

        public void OnModeChange(IdentificationMode newMode)
        {
            if (newMode != IdentificationMode.Identification)
            {
                StopSampling();
            }
        }



        DateTime TimeOnIdentificationStarted;
        DateTime TimeOnStep;
        DateTime TimeOnStop;

        public void CalculateTime()
        {
            TimeOnIdentificationStarted = DateTime.Now;
            TimeOnStep = DateTime.Now.AddSeconds(5);
            TimeOnStop = DateTime.Now.AddSeconds(identificationLength);
        }

        public void OnSampleTimeTick(DateTime dateTime)
        {
            identificationControl.SetProgress(CalculatePercentage(dateTime));

            if (dateTime >= TimeOnStep)
            {
                if (CV != CVAfterStep)
                {
                    CV = CVAfterStep;
                }
            }

            if (dateTime >= TimeOnStop)
            {
                StopSampling();
                identificationControl.EndIdentificationProcess(true);
                identificationControl.Mode = IdentificationMode.Stop;
            }
        }

        public TransferFunctionClass OnIdentificationCalculation(Double[] CVPoints, Double[] PVPoints, Double SampleTime)
        {
            StrejcMethod strejcMethod = new StrejcMethod(CVPoints, PVPoints, SampleTime/1000, Sensitivity,NumberOfAproxxPoints);

            return strejcMethod.GetTransferFunction();
        }

        protected Double CalculatePercentage(DateTime time)
        {
            return 100.0 * (time - TimeOnIdentificationStarted).TotalMilliseconds / (TimeOnStop - TimeOnIdentificationStarted).TotalMilliseconds;
        }

    }
}
