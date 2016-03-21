using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOSIPDModelTunningMethod
{
    public enum TunningType
    {
        ProcessReaction, RegulatorTuning, ServoTuning, TimeDomain, OtherTuning, FrequencyDomain,Robust
    }

    public abstract class SOSIPDTunningMethodBase
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

        public SOSIPDTunningMethodBase(TunningType typeOfTuning, PIDModeType typeOfAlgorithm, String name)
        {
            this.TypeOfTuning = typeOfTuning;
            this.TypeOfAglorithm = typeOfAlgorithm;
            this.Name = name;
        }

        protected Double CalculateDenominatorConstant(TransferFunctionClass plantObject)
        {
            Double a = plantObject.Denominator[3];
            Double b = plantObject.Denominator[2];
            Double c = plantObject.Denominator[1];

            Double delta = b * b - 4 * a * c;

            Double T1 = (-2 * a) / (-b - Math.Sqrt(delta) );
            Double T2 = (-2 * a) / (-b + Math.Sqrt(delta) );

            return (T1+T2)/2;
        }

        protected Double CalculateKob(TransferFunctionClass plantObject)
        {
            return plantObject.Nominator[0] / plantObject.Denominator[1];
        }

    }
}
