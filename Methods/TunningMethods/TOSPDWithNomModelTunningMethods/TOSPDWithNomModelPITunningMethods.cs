using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOSPDWithNomModelTunningMethods
{
    public class Vrancic1996MethodPI : TOSPDWithNomModelPITunningMethod
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);

            Double a3 = plantObject.Denominator[3] / plantObject.Denominator[0];
            Double a2 = plantObject.Denominator[2] / plantObject.Denominator[0];
            Double a1 = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double b3 = plantObject.Nominator[3] / plantObject.Nominator[0];
            Double b2 = plantObject.Nominator[2] / plantObject.Nominator[0];
            Double b1 = plantObject.Nominator[1] / plantObject.Nominator[0];

            Double T0 = plantObject.TimeDelay;

            Double A1 = Kob * (a1 - b1 + T0);
            Double A2 = Kob * (b2 - a2 + A1 * a1 - b1 * T0 + 0.5 * T0 * T0);
            Double A3 = Kob * (a3 - b3 + A2 * a1 - A1 * a2 + b2 * T0 - 0.5 * b1 * T0 * T0 + 0.167 * T0 * T0 * T0);

            Double Kp = 0.5 * A3 / (A1 * A2 - Kob * A3);
            Double Ti = A3 / A2;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public Vrancic1996MethodPI()
            : base(TunningType.FrequencyDomain, PIDModeType.PI, "Vrančić 1996")
        {
        }
    }

    public class Vrancic2004aMethodPI : TOSPDWithNomModelPITunningMethod
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);

            Double a3 = plantObject.Denominator[3] / plantObject.Denominator[0];
            Double a2 = plantObject.Denominator[2] / plantObject.Denominator[0];
            Double a1 = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double b3 = plantObject.Nominator[3] / plantObject.Nominator[0];
            Double b2 = plantObject.Nominator[2] / plantObject.Nominator[0];
            Double b1 = plantObject.Nominator[1] / plantObject.Nominator[0];

            Double T0 = plantObject.TimeDelay;

            Double A1 = Kob * (a1 - b1 + T0);
            Double A2 = Kob * (b2 - a2 + A1 * a1 - b1 * T0 + 0.5 * T0 * T0);
            Double A3 = Kob * (a3 - b3 + A2 * a1 - A1 * a2 + b2 * T0 - 0.5 * b1 * T0 * T0 + 0.167 * T0 * T0 * T0);

            Double Kp = 0.5 * A3 / (A1 * A2 - Kob * A3);
            Double Ti = A1 * A3 * (A1 * A2 - Kob * A3) / (Math.Pow(A1 * A2 - Kob * A3, 2) + Kob * A3 * (A1 * A2 - Kob * A3) + 0.25 * Kob * Kob * A3 * A3);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public Vrancic2004aMethodPI()
            : base(TunningType.FrequencyDomain, PIDModeType.PI, "Vrančić 2004a")
        {
        }
    }

    public class Vrancic2004bMethodPI : TOSPDWithNomModelPITunningMethod
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);

            Double a3 = plantObject.Denominator[3] / plantObject.Denominator[0];
            Double a2 = plantObject.Denominator[2] / plantObject.Denominator[0];
            Double a1 = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double b3 = plantObject.Nominator[3] / plantObject.Nominator[0];
            Double b2 = plantObject.Nominator[2] / plantObject.Nominator[0];
            Double b1 = plantObject.Nominator[1] / plantObject.Nominator[0];

            Double T0 = plantObject.TimeDelay;

            Double A1 = Kob * (a1 - b1 + T0);
            Double A2 = Kob * (b2 - a2 + A1 * a1 - b1 * T0 + 0.5 * T0 * T0);
            Double A3 = Kob * (a3 - b3 + A2 * a1 - A1 * a2 + b2 * T0 - 0.5 * b1 * T0 * T0 + 0.167 * T0 * T0 * T0);

            Double Kp = (A1 * A2 - A3 * Kob - (Sign(A1 * A2 - A3 * Kob)) * A1 * Math.Sqrt(A2 * A2 - A1 * A3)) / (A3 * Kob * Kob - 2 * A1 * A2 * Kob + A1 * A1 * A1);
            Double Ti = 2 * A1 * (A1 * A2 - A3 * Kob - (Sign(A1 * A2 - A3 * Kob)) * A1 * Math.Sqrt(A2 * A2 - A1 * A3)) / ((A3 * Kob * Kob - 2 * A1 * A2 * Kob + A1 * A1 * A1) * (1 + Kp * Kob) * (1 + Kp * Kob));

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public Vrancic2004bMethodPI()
            : base(TunningType.FrequencyDomain, PIDModeType.PI, "Vrančić 2004a")
        {
        }


        public Double Sign(Double value)
        {
            if (value > 0)
            {
                return 1;
            }

            if (value < 0)
            {
                return -1;
            }

            return 0;
        }

    }
}