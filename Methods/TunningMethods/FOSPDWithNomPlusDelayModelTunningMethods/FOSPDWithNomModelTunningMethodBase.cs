using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOSPDWithNomPlusDelayModelTunningMethods
{
    public enum TunningType
    {
        ProcessReaction, RegulatorTuning, ServoTuning, TimeDomain, OtherTuning, FrequencyDomain,Robust
    }

    public abstract class FOSPDWithNomModelPITunningMethodBase
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

        public FOSPDWithNomModelPITunningMethodBase(TunningType typeOfTuning, PIDModeType typeOfAlgorithm, String name)
        {
            this.TypeOfTuning = typeOfTuning;
            this.TypeOfAglorithm = typeOfAlgorithm;
            this.Name = name;
        }

        protected Double CalculateKob(TransferFunctionClass plantObject)
        {
            return plantObject.Nominator[0] / plantObject.Denominator[0];
        }

    }
}
