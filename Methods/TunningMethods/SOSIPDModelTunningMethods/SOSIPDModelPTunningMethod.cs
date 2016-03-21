using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SOSIPDModelTunningMethod
{
    public class KuwataMethodP : SOSIPDTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double T1 = CalculateDenominatorConstant(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0.5 / (Kob * (2 * T1 + T0));

            return new PIDControllerClass(Kp, 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public KuwataMethodP()
            : base(TunningType.TimeDomain, PIDModeType.P, "Kuwata - 1987")
        {
        }
    }

}
