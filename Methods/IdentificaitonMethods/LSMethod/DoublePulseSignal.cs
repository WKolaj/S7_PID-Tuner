using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSMethod
{
    public class DoublePulseSignal : InputSignal
    {
        protected override List<DataPoint> GetDataPoints()
        {
            return new List<DataPoint>()
            {
                new DataPoint(TimeSpanAxis.ToDouble(TimeSpan.FromMilliseconds(0.0)),0.0),
                new DataPoint(TimeSpanAxis.ToDouble(BeginPulseTime),0.0),
                new DataPoint(TimeSpanAxis.ToDouble(BeginPulseTime),FirstPulseValue),
                new DataPoint(TimeSpanAxis.ToDouble(EndFirstPulseTime),FirstPulseValue),
                new DataPoint(TimeSpanAxis.ToDouble(EndFirstPulseTime),-SecondPulseValue),
                new DataPoint(TimeSpanAxis.ToDouble(EndSecondPulseTime),-SecondPulseValue),
                new DataPoint(TimeSpanAxis.ToDouble(EndSecondPulseTime),0.0),
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

        private TimeSpan endFirstPulseTime;
        public TimeSpan EndFirstPulseTime
        {
            get
            {
                return endFirstPulseTime;
            }

            set
            {
                endFirstPulseTime = value;
                NotifyPropertyChanged("EndFirstPulseTime");
            }
        }


        private TimeSpan endSecondPulseTime;
        public TimeSpan EndSecondPulseTime
        {
            get
            {
                return endSecondPulseTime;
            }

            set
            {
                endSecondPulseTime = value;
                NotifyPropertyChanged("EndSecondPulseTime");
            }
        }

        private Double firstPulseValue;
        public Double FirstPulseValue
        {
            get
            {
                return firstPulseValue;
            }

            set
            {
                firstPulseValue = value;
                NotifyPropertyChanged("FirstPulseValue");
            }
        }

        private Double secondPulseValue;
        public Double SecondPulseValue
        {
            get
            {
                return secondPulseValue;
            }

            set
            {
                secondPulseValue = value;
                NotifyPropertyChanged("SecondPulseValue");
            }
        }

        

        public DoublePulseSignal(TimeSpan sampleTime, TimeSpan length, TimeSpan pulseStartTime, TimeSpan firstPulseEndTime, TimeSpan secondPulseEndTime, Double firstPulseValue, Double secondPulseValue)
            : base(sampleTime, length)
        {
            this.beginPulseTime = pulseStartTime;
            this.endFirstPulseTime = firstPulseEndTime;
            this.endSecondPulseTime = secondPulseEndTime;
            this.firstPulseValue = firstPulseValue;
            this.secondPulseValue = secondPulseValue;
        }

        public DoublePulseSignal()
        {
            this.beginPulseTime = TimeSpan.FromSeconds(10);
            this.endFirstPulseTime = TimeSpan.FromSeconds(25);
            this.endSecondPulseTime = TimeSpan.FromSeconds(40);
            this.firstPulseValue = 10;
            this.secondPulseValue = 10;
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
                else if (tickTime < endFirstPulseTime)
                {
                    if (OnSampleTimeValueUpdate != null && partOfPlot != 2)
                    {
                        partOfPlot = 2;

                        OnSampleTimeValueUpdate(InitialValue + firstPulseValue);
                    }
                }

                else if (tickTime < endSecondPulseTime)
                {
                    if (OnSampleTimeValueUpdate != null && partOfPlot != 3)
                    {
                        partOfPlot = 3;

                        OnSampleTimeValueUpdate(InitialValue - secondPulseValue);
                    }
                }
                else
                {
                    if (OnSampleTimeValueUpdate != null && partOfPlot != 4)
                    {
                        partOfPlot = 4;

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
