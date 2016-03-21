using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSMethod
{
    public class RampPulseSignal : InputSignal
    {
        protected override List<DataPoint> GetDataPoints()
        {
            return new List<DataPoint>()
            {
                new DataPoint(TimeSpanAxis.ToDouble(TimeSpan.FromMilliseconds(0.0)),0.0),
                new DataPoint(TimeSpanAxis.ToDouble(BeginFirstRampTime),0.0),
                new DataPoint(TimeSpanAxis.ToDouble(EndFirstRampTime),RampValue),
                new DataPoint(TimeSpanAxis.ToDouble(BeginSecondRampTime),RampValue),
                new DataPoint(TimeSpanAxis.ToDouble(EndSecondRampTime),0.0),
                new DataPoint(TimeSpanAxis.ToDouble(Length),0.0)
            };
        }


        private TimeSpan beginFirstRampTime;
        public TimeSpan BeginFirstRampTime
        {
            get
            {
                return beginFirstRampTime;
            }
            set
            {
                beginFirstRampTime = value;
                NotifyPropertyChanged("BeginFirstRampTime");
            }
        }

        private TimeSpan endFirstRampTime;
        public TimeSpan EndFirstRampTime
        {
            get
            {
                return endFirstRampTime;
            }

            set
            {
                endFirstRampTime = value;
                NotifyPropertyChanged("EndFirstRampTime");
            }
        }


        private TimeSpan beginSecondRampTime;
        public TimeSpan BeginSecondRampTime
        {
            get
            {
                return beginSecondRampTime;
            }

            set
            {
                beginSecondRampTime = value;
                NotifyPropertyChanged("BeginSecondRampTime");
            }
        }

        private TimeSpan endSecondRampTime;
        public TimeSpan EndSecondRampTime
        {
            get
            {
                return endSecondRampTime;
            }

            set
            {
                endSecondRampTime = value;
                NotifyPropertyChanged("EndSecondRampTime");
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


        public RampPulseSignal(TimeSpan sampleTime, TimeSpan length, TimeSpan beginFirstRampTime, TimeSpan endFirstRampTime, TimeSpan beginSecondRampTime, TimeSpan endSecondRampTime, Double rampValue)
            : base(sampleTime, length)
        {
            this.beginFirstRampTime = beginFirstRampTime;
            this.endFirstRampTime = endFirstRampTime;
            this.beginSecondRampTime = beginSecondRampTime;
            this.endSecondRampTime = endSecondRampTime;

            this.rampValue = rampValue;
        }

        public RampPulseSignal()
        {
            this.beginFirstRampTime = TimeSpan.FromSeconds(10);
            this.endFirstRampTime = TimeSpan.FromSeconds(20);
            this.beginSecondRampTime = TimeSpan.FromSeconds(40);
            this.endSecondRampTime = TimeSpan.FromSeconds(50);

            this.rampValue = 10;
        }

        protected override void OnSampleTick(TimeSpan tickTime)
        {
            if (tickTime < Length)
            {
                if (tickTime < beginFirstRampTime)
                {
                    if (OnSampleTimeValueUpdate != null && partOfPlot != 1)
                    {
                        partOfPlot = 1;
                        OnSampleTimeValueUpdate(InitialValue);
                    }
                }
                else if (tickTime < endFirstRampTime)
                {
                    if (OnSampleTimeValueUpdate != null )
                    {
                        partOfPlot = 2;
                        OnSampleTimeValueUpdate(InitialValue + rampValue * Convert.ToDouble((tickTime - beginFirstRampTime).TotalMilliseconds) / (Convert.ToDouble((endFirstRampTime - beginFirstRampTime).TotalMilliseconds)));
                    }
                }
                else if (tickTime < beginSecondRampTime && partOfPlot != 3)
                {
                    if (OnSampleTimeValueUpdate != null)
                    {
                        partOfPlot = 3;
                        OnSampleTimeValueUpdate(InitialValue + rampValue);
                    }
                }
                else if (tickTime < endSecondRampTime && tickTime >= beginSecondRampTime)
                {
                    if (OnSampleTimeValueUpdate != null)
                    {
                        partOfPlot = 4;
                        OnSampleTimeValueUpdate(InitialValue + rampValue - rampValue * Convert.ToDouble((tickTime - beginSecondRampTime).TotalMilliseconds) / (Convert.ToDouble((endSecondRampTime - beginSecondRampTime).TotalMilliseconds)));
                    }
                }
                else if (tickTime >= endSecondRampTime)
                {
                    if (OnSampleTimeValueUpdate != null && partOfPlot != 5)
                    {
                        partOfPlot = 5;
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
