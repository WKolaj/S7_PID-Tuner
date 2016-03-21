using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSMethod
{
    public class RampSignal : InputSignal
    {
        protected override List<DataPoint> GetDataPoints()
        {
            return new List<DataPoint>()
            {
                new DataPoint(TimeSpanAxis.ToDouble(TimeSpan.FromMilliseconds(0.0)),0.0),
                new DataPoint(TimeSpanAxis.ToDouble(BeginRampTime),0.0),
                new DataPoint(TimeSpanAxis.ToDouble(EndRampTime),RampValue),
                new DataPoint(TimeSpanAxis.ToDouble(Length),RampValue)
            };
        }


        private TimeSpan beginRampTime;
        public TimeSpan BeginRampTime
        {
            get
            {
                return beginRampTime;
            }

            set
            {
                beginRampTime = value;
                NotifyPropertyChanged("BeginRampTime");
            }
        }

        private TimeSpan endRampTime;
        public TimeSpan EndRampTime
        {
            get
            {
                return endRampTime;
            }

            set
            {
                endRampTime = value;
                NotifyPropertyChanged("EndRampTime");
            }
        }

        private Double rampValue;
        public Double RampValue
        {
            get
            {
                return rampValue;
            }

            set
            {
                rampValue = value;
                NotifyPropertyChanged("RampValue");
            }
        }


        public RampSignal(TimeSpan sampleTime, TimeSpan length, TimeSpan rampStartTime, TimeSpan rampEndTime, Double rampValue)
            : base(sampleTime, length)
        {
            this.beginRampTime = rampStartTime;
            this.endRampTime = rampEndTime;
            this.rampValue = rampValue;
        }

        public RampSignal()
        {
            this.beginRampTime = TimeSpan.FromSeconds(10);
            this.endRampTime = TimeSpan.FromSeconds(20);
            this.rampValue = 10;
        }

        protected override void OnSampleTick(TimeSpan tickTime)
        {
            if (tickTime < Length)
            {
                if (tickTime < beginRampTime)
                {
                    if (OnSampleTimeValueUpdate != null && partOfPlot != 1)
                    {
                        partOfPlot = 1;
                        OnSampleTimeValueUpdate(InitialValue);
                    }
                }
                else if (tickTime < endRampTime)
                {
                    if (OnSampleTimeValueUpdate != null )
                    {
                        partOfPlot = 2;
                        OnSampleTimeValueUpdate(InitialValue + rampValue * Convert.ToDouble((tickTime - beginRampTime).TotalMilliseconds) / (Convert.ToDouble((endRampTime - beginRampTime).TotalMilliseconds)));
                    }
                }
                else
                {
                    if (OnSampleTimeValueUpdate != null && partOfPlot != 3 && tickTime >= endRampTime)
                    {
                        partOfPlot = 3;
                        OnSampleTimeValueUpdate(InitialValue + rampValue);
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
