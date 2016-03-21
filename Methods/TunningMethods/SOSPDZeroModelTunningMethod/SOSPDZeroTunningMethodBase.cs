using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOSPDZeroModelTunningMethod
{
    public enum TunningType
    {
        ProcessReaction, RegulatorTuning, ServoTuning, TimeDomain, OtherTuning, FrequencyDomain,Robust
    }

    public abstract class SOSPDZeroTunningMethodBase
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

        public SOSPDZeroTunningMethodBase(TunningType typeOfTuning, PIDModeType typeOfAlgorithm, String name, Boolean rootsRequired)
        {
            this.TypeOfTuning = typeOfTuning;
            this.TypeOfAglorithm = typeOfAlgorithm;
            this.Name = name;
            this.RootsRequired = rootsRequired;
        }

        protected Double[] CalculateDenominatorConstants(TransferFunctionClass plantObject)
        {
            Double a = plantObject.Denominator[2];
            Double b = plantObject.Denominator[1];
            Double c = plantObject.Denominator[0];

            Double delta = b * b - 4 * a * c;

            Double T1 = (-2 * a) / (-b - Math.Sqrt(delta) );
            Double T2 = (-2 * a) / (-b + Math.Sqrt(delta) );

            return new Double[] { T1, T2 };
        }

        protected Double CalculateKsi(TransferFunctionClass plantObject)
        {
            Double T1 = Math.Sqrt(plantObject.Denominator[2] / plantObject.Denominator[0]);

            return (plantObject.Denominator[1] / plantObject.Denominator[0]) / (2 * CalculateTob(plantObject));
        }

        protected Double CalculateTob(TransferFunctionClass plantObject)
        {
            return  Math.Sqrt(plantObject.Denominator[2] / plantObject.Denominator[0]);
        }

        protected Double CalculateKob(TransferFunctionClass plantObject)
        {
            return plantObject.Nominator[0] / plantObject.Denominator[0];
        }

        protected Double CalculateT3(TransferFunctionClass plantObject)
        {
            return plantObject.Nominator[1] / plantObject.Nominator[0];
        }
    }
}
