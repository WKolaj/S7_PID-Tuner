using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOLIPDModelTunningMethods
{
    public class CoonMethodP : FOLIPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double factor = T0 / Tob;

            Double Kp = 0;
            Double x1 = 0;

            if(factor <= 0.020)
            {
                x1 = 5.0;
            }
            else if (factor <= 0.053)
            {
                x1 = 4.0;
            }
            else if (factor <= 0.11)
            {
                x1 = 3.0;
            }
            else if (factor <= 0.25)
            {
                x1 = 2.2;
            }
            else if (factor <= 0.43)
            {
                x1 = 1.7;
            }
            else if (factor <= 1.0)
            {
                x1 = 1.3;
            }
            else             
            {
                x1 = 1.1;
            }

            Kp = (x1) / (Kob * (T0 + Tob));

            return new PIDControllerClass(Kp, 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public CoonMethodP()
            : base(TunningType.ProcessReaction, PIDModeType.P, "Coon 1964")
        {

        }
    }

    public class ShinskeyMethodP : FOLIPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0.556/(Kob * (T0 + Tob));
            Double Ti = 3.7 * (T0 + Tob);

            Double factor = T0 / Tob;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public ShinskeyMethodP()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Shinskey 1994")
        {

        }
    }

    public class VelaquezFigueroaRegulatorMethodP : FOLIPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double Kp = (0.029/Kob)*Math.Pow(Tob/T0,0.157);
            
            return new PIDControllerClass(Kp, 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public VelaquezFigueroaRegulatorMethodP()
            : base(TunningType.RegulatorTuning, PIDModeType.P, "Velázquez-Figueroa 1997")
        {

        }
    }

    public class VelaquezFigueroaServoMethodP : FOLIPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double Kp = (0.031 / Kob) * Math.Pow(Tob / T0, 0.528);

            return new PIDControllerClass(Kp, 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public VelaquezFigueroaServoMethodP()
            : base(TunningType.ServoTuning, PIDModeType.P, "Velázquez-Figueroa 1997")
        {

        }
    }

    public class HubaZakovaMethodP : FOLIPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double Kp = (-2 * Tob + Math.Sqrt(T0 * T0 + 4 * Tob * Tob)) / (Kob * T0 * T0 * Math.Exp((2*Tob + T0 - Math.Sqrt(T0*T0+4*Tob*Tob))/(2*Tob)));

            return new PIDControllerClass(Kp, 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public HubaZakovaMethodP()
            : base(TunningType.TimeDomain, PIDModeType.P, "Huba-Žáková 2003")
        {

        }
    }

}
