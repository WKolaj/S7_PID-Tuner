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
using System.ComponentModel;

namespace SecantMethod
{
    [Method("Secant Method","sieczna.png")]
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
            if(newMode != IdentificationMode.Identification)
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

                if(dateTime >= TimeOnStep)
                {
                    if(CV != CVAfterStep)
                    {
                        CV = CVAfterStep;
                    }
                }
                
                if(dateTime >= TimeOnStop)
                {
                    StopSampling();
                    identificationControl.EndIdentificationProcess(true);
                    identificationControl.Mode = IdentificationMode.Stop;
                }
        }

        public TransferFunctionClass OnIdentificationCalculation( Double[] CVPoints, Double[] PVPoints, Double SampleTime)
        {
            Double deltaCV = CVPoints.Last() - CVPoints.First();
            Double deltaPV = PVPoints.Last() - PVPoints.First();

            Double kob = deltaPV / deltaCV;

            Double PV1 = 0.5 * deltaPV + PVPoints.First();
            Double PV2 = 0.632 * deltaPV + PVPoints.First();

            Double T1 = 0;
            Double T2 = 0;
            Double CVStartTime = 0;

            for(int i=0; i < PVPoints.Length; i++)
            {
                if(CVPoints[i] != CVPoints.First() && CVStartTime == 0)
                {
                    CVStartTime = Convert.ToDouble(i) * SampleTime / 1000;
                }

                if (deltaPV < 0)
                {
                    if (PVPoints[i] < PV1 && T1 == 0)
                    {
                        T1 = (Convert.ToDouble(i) * SampleTime / 1000) - CVStartTime;
                    }

                    if (PVPoints[i] < PV2 && T2 == 0)
                    {
                        T2 = (Convert.ToDouble(i) * SampleTime / 1000) - CVStartTime;
                    }
                }
                else
                {
                    if (PVPoints[i] > PV1 && T1 == 0)
                    {
                        T1 = (Convert.ToDouble(i) * SampleTime / 1000) - CVStartTime;
                    }

                    if (PVPoints[i] > PV2 && T2 == 0)
                    {
                        T2 = (Convert.ToDouble(i) * SampleTime / 1000) - CVStartTime;
                    }
                }
            }

            Double T0 = (T1 - T2 * Math.Log(2)) / (1 - Math.Log(2));
            Double Tz = T2 - T0;

            return new TransferFunctionClass(new Double[] { kob }, new Double[] { 1, Tz }, T0, Convert.ToInt32(SampleTime) , TransferFunctionType.Continous);
        }

        protected Double CalculatePercentage(DateTime time)
        {
            return 100.0 * (time - TimeOnIdentificationStarted).TotalMilliseconds /  (TimeOnStop- TimeOnIdentificationStarted).TotalMilliseconds;
        }

    }
}
