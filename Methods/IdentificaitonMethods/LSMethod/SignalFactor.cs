using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSMethod
{
    public enum SignalType
    {
        Manual,Step, Pulse, DoublePulse, Ramp, RampPulse
    }

    public class SignalFactor : INotifyPropertyChanged
    {
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


        private SignalType signalType = SignalType.Manual;
        public SignalType SignalType
        {
            get
            {
                return signalType;
            }

            set
            {
                signalType = value;

            }
        }

        private StepSignal stepSignal = new StepSignal();
        public StepSignal StepSignal
        {
            get
            {
                return stepSignal;
            }

            set
            {
                stepSignal = value;
            }
        }

        private PulseSignal pulseSignal = new PulseSignal();
        public PulseSignal PulseSignal
        {
            get
            {
                return pulseSignal;
            }

            set
            {
                pulseSignal = value;
            }
        }

        private DoublePulseSignal doublePulseSignal = new DoublePulseSignal();
        public DoublePulseSignal DoublePulseSignal
        {
            get
            {
                return doublePulseSignal;
            }

            set
            {
                doublePulseSignal = value;
            }
        }

        private RampSignal rampSignal = new RampSignal();
        public RampSignal RampSignal
        {
            get
            {
                return rampSignal;
            }

            set
            {
                rampSignal = value;
            }
        }

        private RampPulseSignal rampPulseSignal = new RampPulseSignal();
        public RampPulseSignal RampPulseSignal
        {
            get
            {
                return rampPulseSignal;
            }

            set
            {
                rampPulseSignal = value;
            }
        }

        public InputSignal BuildSignal()
        {
            switch (this.SignalType)
            {
                case SignalType.Manual:
                    {

                        return null;
                    }

                case SignalType.Step:
                    {

                        return StepSignal;
                    }

                case SignalType.Pulse:
                    {

                        return PulseSignal;
                    }

                case SignalType.DoublePulse:
                    {

                        return DoublePulseSignal;
                    }

                case SignalType.Ramp:
                    {

                        return RampSignal;
                    }

                case SignalType.RampPulse:
                    {

                        return RampPulseSignal;
                    }
            }

            return null;
        }

        public event Action SomePropertyChanged;

        public void OnPropertyOfSomeSignalChanged(object sender, EventArgs arguments)
        {
            if(SomePropertyChanged!=null)
            {
                SomePropertyChanged();
            }
        }

        public SignalFactor()
        {
            ConnectEvents();
        }

        public void ConnectEvents()
        {
            StepSignal.SomePropertyChanged+= OnPropertyOfSomeSignalChanged;
            PulseSignal.SomePropertyChanged += OnPropertyOfSomeSignalChanged;
            DoublePulseSignal.SomePropertyChanged += OnPropertyOfSomeSignalChanged;
            RampSignal.SomePropertyChanged += OnPropertyOfSomeSignalChanged;
            RampPulseSignal.SomePropertyChanged += OnPropertyOfSomeSignalChanged;
        }
    }
}
