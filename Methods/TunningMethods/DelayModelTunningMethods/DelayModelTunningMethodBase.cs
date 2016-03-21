using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicMethodsLibrary;

namespace DelayModelTunningMethods
{
    public enum TunningType
    {
        ProcessReaction, RegulatorTuning, ServoTuning, OtherTuning
    }

    public abstract class DelayModelTunningMethodBase
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

        public DelayModelTunningMethodBase(TunningType typeOfTuning, PIDModeType typeOfAlgorithm, String name )
        {
            this.TypeOfTuning = typeOfTuning;
            this.TypeOfAglorithm = typeOfAlgorithm;
            this.Name = name;
        }
    }
}
