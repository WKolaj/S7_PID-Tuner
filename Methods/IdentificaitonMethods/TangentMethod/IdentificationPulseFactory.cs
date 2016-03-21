using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicMethodsLibrary;

namespace Tangent_method
{
    public class IdentificationPulseFactory
    {
        private IdentificationWindow window;

        public event Action IdentificationProcessStopped;

        private Int32 numberOfSamplesBeforePulse = 0;
        public Int32 NumberOfSamplesBeforePulse
        {
            get
            {
                return numberOfSamplesBeforePulse;
            }

            private set
            {
                numberOfSamplesBeforePulse = value;
            }

        }

        private Int32 numberOfSamplesAfterPulse = 0;
        public Int32 NumberOfSamplesAfterPulse
        {
            get
            {
                return numberOfSamplesAfterPulse;
            }

            private set
            {
                numberOfSamplesAfterPulse = value;
            }

        }

        private Boolean inProgress = false;
        public Boolean InProgress
        {
            get
            {
                return inProgress;
            }

            private set
            {
                inProgress = value;
            }
        }

        private Boolean afterPVGreaterThanStep = false;
        public Boolean AfterPVGreaterThanStep
        {
            get
            {
                return afterPVGreaterThanStep;
            }

            private set
            {
                afterPVGreaterThanStep = value;
            }
        }

        private Boolean afterPulse = false;
        public Boolean AfterPulse
        {
            get
            {
                return afterPulse;
            }

            private set
            {
                afterPulse = value;
            }
        }


        private Double stepValue = 10;
        public Double StepValue
        {
            get
            {
                return stepValue;
            }

            set
            {
                stepValue = value;
            }
        }

        private Double pvZero;
        public Double PVZero
        {
            get
            {
                return pvZero;
            }

            private set
            {
                pvZero = value;
            }
        }

        private Double cvZero;
        public Double CVZero
        {
            get
            {
                return cvZero;
            }

            private set
            {
                cvZero = value;
            }
        }

        private Int32 maxNumberOfSamplesBeforePulse = 5;
        private Int32 maxNumberOfSamplesAfterPulse = 5;

        public void OnTimerTick(DateTime time, Double pvValue)
        {
            if (InProgress)
            {
                if (!AfterPulse)
                {
                    if (NumberOfSamplesBeforePulse >= maxNumberOfSamplesBeforePulse)
                    {
                        window.CV = CVZero + StepValue;
                        AfterPulse = true;
                    }
                    else
                    {
                        NumberOfSamplesBeforePulse++;
                    }
                }
                else if (!AfterPVGreaterThanStep)
                {
                    if (Math.Abs(pvValue - PVZero) >= Math.Abs(StepValue))
                    {
                        AfterPVGreaterThanStep = true;
                    }
                }
                else
                {
                    if (NumberOfSamplesAfterPulse >= maxNumberOfSamplesAfterPulse)
                    {
                        window.CV = CVZero;
                        Stop();
                    }
                    else
                    {
                        NumberOfSamplesAfterPulse++;
                    }
                }
            }
        }

        public void Stop()
        {
            if(InProgress)
            {
                Reset();

                if (IdentificationProcessStopped != null)
                {
                    IdentificationProcessStopped();
                }
            }
        }

        public void Start(Double pv0, Double cv0)
        {
            Reset();

            PVZero = pv0;
            CVZero = cv0;

            InProgress = true;
        }

        public void Reset()
        {
            InProgress = false;
            AfterPulse = false;
            NumberOfSamplesAfterPulse = 0;
            NumberOfSamplesBeforePulse = 0;
            AfterPVGreaterThanStep = false;
        }

        public IdentificationPulseFactory(IdentificationWindow window, Double step)
        {
            this.window = window;
            this.StepValue = step;
            Reset();
        }
    }
}
