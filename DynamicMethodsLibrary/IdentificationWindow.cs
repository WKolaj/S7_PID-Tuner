using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using S7TCP;
using System.Drawing;

[assembly: InternalsVisibleTo("S7_PID_Tuner")]

namespace DynamicMethodsLibrary
{
    public abstract partial class IdentificationWindow : Window, INotifyPropertyChanged
    {
        private Timer timer = new Timer();

        public event Action<DateTime> SampleTimeTick;
        
        private void OnTimerTick(object sender,EventArgs argument)
        {
            if(SampleTimeTick != null)
            {
                SampleTimeTick(DateTime.Now);
            }
        }

        /// <summary>
        /// Mechanizm wiazania danych WPF
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Mechanizm wiazania danych WPF
        /// </summary>
        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool IdentificationSucessfully
        {
            get;
            protected set;
        }

        Object sampleTimeLockingObject = new Object();

        internal Action<Double> SetCVOutside;

        internal Action<Int32> SetSampleTimeOutside;

        internal Double[] nominator;

        internal Double[] denominator;

        internal Double timeDelay;

        internal Boolean isDiscrete;

        internal Int32 simulationSampleTime;

        public void EndIdentification(Double[] nominator, Double[] denominator, Double timeDelay, Boolean isDiscrete, Int32 sampleTime)
        {
            timer.Stop();

            this.nominator = nominator;
            this.denominator = denominator;
            this.timeDelay = timeDelay;
            this.isDiscrete = isDiscrete;
            this.simulationSampleTime = sampleTime;

            IdentificationSucessfully = true;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
                {
                    DialogResult = true;
                }
                else
                {
                    Close();
                }
            }));
        }

        public void StopIdentification(string cause)
        {
            timer.Stop();

            System.Windows.MessageBox.Show(cause, "Identification stopped", MessageBoxButton.OK, MessageBoxImage.Error);
            IdentificationSucessfully = false;

            Dispatcher.BeginInvoke(new Action (()=>
                {
                    if( System.Windows.Interop.ComponentDispatcher.IsThreadModal )
                    {
                        DialogResult = false;
                    }
                    else
                    {
                        Close();
                    }
                }));
        }

        public event Action<Double> PVUpdated;

        public event Action<Double> CVUpdated;

        public event Action<Int32> SampleTimeUpdated;

        internal void SetCVFromOutside(Double value)
        {
                cV = value;

                if (CVUpdated != null)
                {
                    CVUpdated(value);
                }

                NotifyPropertyChanged("CV");
        }

        internal void SetPVFromOutside(Double value)
        {
            pV = value;

            if (PVUpdated != null)
            {
                PVUpdated(value);
            }

            NotifyPropertyChanged("PV");
        }

        internal void SetSampleTimeFromOutside(Int32 value)
        {
                sampleTime = value;
                timer.Period = value;

                if (SampleTimeUpdated != null)
                {
                    SampleTimeUpdated(value);
                }

                NotifyPropertyChanged("SampleTime");
        }

        private double pV;
        public Double PV
        {
            get
            {
                return pV;
            }

            set
            {
                throw new Exception("An attempt ot modify Read-Only property"); 
            }

        }

        private double cV;
        public Double CV
        {
            get
            {
                return cV;
            }

            set
            {
                    if (SetCVOutside != null)
                    {
                        SetCVOutside(value);
                    }

                    cV = value;

                    if (CVUpdated != null)
                    {
                        CVUpdated(value);
                    }

                NotifyPropertyChanged("CV");
            }
        }

        private Int32 sampleTime;
        public Int32 SampleTime
        {
            get
            {
                return sampleTime;
            }

            set
            {
                lock (sampleTimeLockingObject)
                {
                    if (SetSampleTimeOutside != null)
                    {
                        SetSampleTimeOutside(value);
                    }

                    sampleTime = value;
                    timer.Period = value;

                    if (SampleTimeUpdated != null)
                    {
                        SampleTimeUpdated(value);
                    }
                }

                NotifyPropertyChanged("SampleTime");
            }
        }

        public IdentificationWindow()
        {
            InitTimer();
        }

        private void InitTimer()
        {
            timer.Tick += OnTimerTick;
        }

        public void StartSampling()
        {
            timer.Start();
        }

        public void StopSampling()
        {
            timer.Stop();
        }

    }
}
