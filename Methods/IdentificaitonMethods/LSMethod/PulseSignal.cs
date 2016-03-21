using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSMethod
{
    public class PulseSignal : InputSignal
    {
        protected override List<DataPoint> GetDataPoints()
        {
            return new List<DataPoint>()
            {
                new DataPoint(TimeSpanAxis.ToDouble(TimeSpan.FromMilliseconds(0.0)),0.0),
                new DataPoint(TimeSpanAxis.ToDouble(BeginPulseTime),0.0),
                new DataPoint(TimeSpanAxis.ToDouble(BeginPulseTime),PulseValue),
                new DataPoint(TimeSpanAxis.ToDouble(EndPulseTime),PulseValue),
                new DataPoint(TimeSpanAxis.ToDouble(EndPulseTime),0.0),
                new DataPoint(TimeSpanAxis.ToDouble(Length),0.0)
            };
        }

        private TimeSpan beginPulseTime;
        public TimeSpan BeginPulseTime
        {
            get
            {
                return beginPulseTime;
            }

            set
            {
                beginPulseTime = value;
                NotifyPropertyChanged("BeginPulseTime");
            }
        }

        private TimeSpan endPulseTime;
        public TimeSpan EndPulseTime
        {
            get
            {
                return endPulseTime;
            }

            set
            {
                endPulseTime = value;
                NotifyPropertyChanged("EndPulseTime");
            }
        }

        private Double pulseValue;
        public Double PulseValue
        {
            get
            {
                return pulseValue;
            }

            set
            {
                pulseValue = value;
                NotifyPropertyChanged("PulseValue");
            }
        }


        public PulseSignal()
        {
            this.beginPulseTime = TimeSpan.FromSeconds(10);
            this.endPulseTime = TimeSpan.FromSeconds(30);
            this.pulseValue = 10;
        }

        public PulseSignal(TimeSpan sampleTime, TimeSpan length, TimeSpan pulseStartTime, TimeSpan pulseEndTime, Double pulseValue)
            : base(sampleTime, length)
        {
            this.beginPulseTime = pulseStartTime;
            this.endPulseTime = pulseEndTime;
            this.pulseValue = pulseValue;
        }

        protected override void OnSampleTick(TimeSpan tickTime)
        {
            if (tickTime < Length)
            {
                if (tickTime < beginPulseTime)
                {
                    if (OnSampleTimeValueUpdate != null && partOfPlot != 1)
                    {
                        partOfPlot = 1;

                        OnSampleTimeValueUpdate(InitialValue);
                    }
                }
                else if (tickTime < endPulseTime)
                {
                    if (OnSampleTimeValueUpdate != null && partOfPlot != 2)
                    {
                        partOfPlot = 2;

                        OnSampleTimeValueUpdate(InitialValue + pulseValue);
                    }
                }
                else
                {
                    if (OnSampleTimeValueUpdate != null && partOfPlot != 3)
                    {
                        partOfPlot = 3;

                        OnSampleTimeValueUpdate(InitialValue);
                    }
                }
            }
            else
            {
                EndInputSource();
            }

        }

    }
}
