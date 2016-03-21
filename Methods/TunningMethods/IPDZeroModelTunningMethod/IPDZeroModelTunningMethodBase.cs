using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPDZeroModelTunningMethod
{
    public enum TunningType
    {
        ProcessReaction, RegulatorTuning, ServoTuning, TimeDomain, OtherTuning, FrequencyDomain
    }

    public abstract class IPDZeroModelTunningMethodBase
    {
        public TunningType TypeOfTuning
        {
            get;
            private set;
        }

        public PIDModeType TypeOfAglorithm
        {
            get;
            private set;
        }

        public String Name
        {
            get;
            private set;
        }

        public abstract PIDControllerClass TuningMethod(TransferFunctionClass plantObject);

        public IPDZeroModelTunningMethodBase(TunningType typeOfTuning, PIDModeType typeOfAlgorithm, String name)
        {
            this.TypeOfTuning = typeOfTuning;
            this.TypeOfAglorithm = typeOfAlgorithm;
            this.Name = name;
        }

        protected Double CalculateKob(TransferFunctionClass plantObject)
        {
            return (plantObject.Nominator[0] / plantObject.Denominator[1]);
        }

        protected Double CalculateTob(TransferFunctionClass plantObject)
        {
            return plantObject.Nominator[1] / plantObject.Nominator[0];
        }
    }
}
