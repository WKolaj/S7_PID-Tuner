using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOLPDModelTunningMethods
{
    public enum TunningType
    {
        ProcessReaction, RegulatorTuning, ServoTuning, TimeDomain, OtherTuning
    }

    public abstract class FOLPFModelTunningMethodBase
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

        public FOLPFModelTunningMethodBase(TunningType typeOfTuning, PIDModeType typeOfAlgorithm, String name)
        {
            this.TypeOfTuning = typeOfTuning;
            this.TypeOfAglorithm = typeOfAlgorithm;
            this.Name = name;
        }

    }
}
