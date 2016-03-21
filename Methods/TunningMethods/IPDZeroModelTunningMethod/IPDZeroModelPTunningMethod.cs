using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPDZeroModelTunningMethod
{
    public class JyothiMethodP : IPDZeroModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double factor = Math.Abs(T0 / Tob);

            Double Kp = 0;

            if(Tob >= 0)
            {
                if(factor < 1)
                {
                    Kp = 1.57 / (Kob * T0*(Math.Sqrt(1 + 9.870 * (Tob / T0) * (Tob / T0))));
                }
                else
                {
                    Kp = 0.79 / (Kob * T0 * (Math.Sqrt(1 + 2.467 * (Tob / T0) * (Tob / T0))));
                }
            }
            else
            {
                if (factor < 1)
                {
                    Kp = 0.5/(Kob*(-Tob));
                }
                else
                {
                    Kp = 0.79 / (Kob * T0 * (Math.Sqrt(1 + 2.467 * (Tob / T0) * (Tob / T0))));
                }
            }


            return new PIDControllerClass(Kp, 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public JyothiMethodP()
            : base(TunningType.FrequencyDomain, PIDModeType.P, "Jyothi 2001")
        {

        }
    }
}
