using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSMethod
{
    public class StepSignal : InputSignal
    {

        protected override List<DataPoint> GetDataPoints()
        {
            return new List<DataPoint>()
            {
                new DataPoint(TimeSpanAxis.ToDouble(TimeSpan.FromMilliseconds(0.0)),0.0),
                new DataPoint(TimeSpanAxis.ToDouble(StepTime),0.0),
                new DataPoint(TimeSpanAxis.ToDouble(StepTime),StepValue),
                new DataPoint(TimeSpanAxis.ToDouble(Length),StepValue)
            };
        }

        private TimeSpan stepTime;
        public TimeSpan StepTime
        {
            get
            {
                return stepTime;
            }

            set
            {
                stepTime = value;
                NotifyPropertyChanged("StepTime");
            }
        }

        private Double stepValue;
        public Double StepValue
        {
            get
            {
                return stepValue;
            }

            set
            {
                stepValue = value;
                NotifyPropertyChanged("StepValue");
            }
        }


        public StepSignal(TimeSpan sampleTime, TimeSpan length, TimeSpan stepTime, Double StepValue)
            : base(sampleTime, length)
        {
            this.stepTime = stepTime;
            this.stepValue = StepValue;
        }

        public StepSignal()
        {
            this.stepTime = TimeSpan.FromSeconds(10);
            this.stepValue = 10;
        }

        protected override void OnSampleTick(TimeSpan tickTime)
        {
            if (tickTime < Length)
            {
                if (tickTime < stepTime)
                {
                    if (OnSampleTimeValueUpdate != null && partOfPlot != 1)
                    {
                        partOfPlot = 1;

                        OnSampleTimeValueUpdate(InitialValue);
                    }
                }
                else
                {
                    if (OnSampleTimeValueUpdate != null && partOfPlot != 2)
                    {
                        partOfPlot = 2;

                        OnSampleTimeValueUpdate(InitialValue + stepValue);
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
