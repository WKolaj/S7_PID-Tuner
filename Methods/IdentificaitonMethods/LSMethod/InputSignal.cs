using OxyPlot;
using S7TCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSMethod
{
    public abstract class InputSignal : INotifyPropertyChanged
    {
        public List<DataPoint> SignalPlot
        {
            get
            {
                return GetDataPoints();
            }
        }

        protected abstract List<DataPoint> GetDataPoints();

        /// <summary>
        /// Mechanizm wiazania danych WPF
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Mechanizm wiazania danych WPF
        /// </summary>
        protected void NotifyPropertyChanged(String propertyName)
        {
            if(SomePropertyChanged != null)
            {
                SomePropertyChanged(this,EventArgs.Empty);
            }

            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event Action<object, EventArgs> SomePropertyChanged;

        private Double initialValue;
        protected Double InitialValue
        {
            get
            {
                return initialValue;
            }

            private set
            {
                this.initialValue = value;
            }
        }

        protected DateTime startTime;
        protected DateTime stopTime;

        private TimeSpan length;
        public TimeSpan Length
        {
            get
            {
                return length;
            }

            set
            {
                length = value;

                NotifyPropertyChanged("Length");
            }
        }

        public TimeSpan SampleTime
        {
            get
            {
                return TimeSpan.FromMilliseconds(Convert.ToDouble(timer.Period));
            }
            set
            {
                timer.Period = Convert.ToInt32(value.TotalMilliseconds);
                NotifyPropertyChanged("SampleTime");
            }
        }

        private Timer timer = new Timer();

        protected abstract void OnSampleTick(TimeSpan tickTime);

        public Action<Double> OnSampleTimeValueUpdate;

        private void OnTimerTick(object sender, EventArgs eventArgument)
        {
            OnSampleTick(DateTime.Now - startTime);

            if(OnPercentageChange != null)
            {
                OnPercentageChange(100 * (DateTime.Now - startTime).TotalMilliseconds / (stopTime - startTime).TotalMilliseconds);
            }
        }

        protected Int32 partOfPlot = 0;

        private void ConnectEvents()
        {
            timer.Tick += OnTimerTick;
        }

        private void DisconnectEvents()
        {
            timer.Tick -= OnTimerTick;
        }

        public void SetTimes(DateTime startTime)
        {
            this.startTime = startTime;
            this.stopTime = startTime + length;
        }

        public void Start(Double initialValue)
        {
            this.initialValue = initialValue;

            SetTimes(DateTime.Now);

            partOfPlot = 0;

            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        protected void EndInputSource()
        {
            timer.Stop();

            if (InputSourceEnded != null)
            {
                InputSourceEnded();
            }
        }

        public event Action InputSourceEnded;


        public InputSignal(TimeSpan sampleTime, TimeSpan length)
        {
            this.length = length;
            this.SampleTime = sampleTime;

            ConnectEvents();
        }

        public InputSignal()
        {
            this.length = TimeSpan.FromSeconds(60.0);
            this.SampleTime = TimeSpan.FromMilliseconds(100);

            ConnectEvents();
        }

        public event Action<Double> OnPercentageChange;
    }
}
