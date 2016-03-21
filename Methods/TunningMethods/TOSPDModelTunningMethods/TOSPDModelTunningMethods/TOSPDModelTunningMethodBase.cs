using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOSPDModelTunningMethods
{
    public enum TunningType
    {
        ProcessReaction, RegulatorTuning, ServoTuning, TimeDomain, OtherTuning, FrequencyDomain,Robust
    }

    public abstract class TOSPDModelTunningMethodBase
    {
        public Boolean RootsRequired
        {
            get;
            set;
        }

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

        public TOSPDModelTunningMethodBase(TunningType typeOfTuning, PIDModeType typeOfAlgorithm, String name, Boolean rootsRequired)
        {
            this.TypeOfTuning = typeOfTuning;
            this.TypeOfAglorithm = typeOfAlgorithm;
            this.Name = name;
            this.RootsRequired = rootsRequired;
        }

        protected Double[] CalculateDenominatorConstant(TransferFunctionClass plantObject)
        {  
            RootFinder rootFinder = new RootFinder(plantObject.Denominator,0.001);
            Double[] roots = rootFinder.GetRealRoots();

            return (from root in roots
                       select -1/root).ToArray();
        }

        protected Double CalculateKob(TransferFunctionClass plantObject)
        {
            return plantObject.Nominator[0] / plantObject.Denominator[0];
        }

    }
}
