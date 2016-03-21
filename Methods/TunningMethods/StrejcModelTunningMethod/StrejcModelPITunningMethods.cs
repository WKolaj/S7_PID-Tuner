using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrejcModelTunningMethod
{
    public class IAEMurataSagaraRegulatorMethodPI : StrejcModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateDenominatorConstant(plantObject);
            Double T0 = plantObject.TimeDelay;
            Double n = CalculateRank(plantObject);
            
            Double x1 = 0;
            Double x2 = 0;

            Double factor = T0 / Tob;

            if(n<5)
            {
                if(factor<= 0.2)
                {
                    x1 = 0.9;
                    x2 = 4.0;
                }
                else if (factor <= 0.4)
                {
                    x1 = 0.7;
                    x2 = 4.0;
                }
                else if (factor <= 0.6)
                {
                    x1 = 0.6;
                    x2 = 4.2;
                }
                else if (factor <= 0.8)
                {
                    x1 = 0.5;
                    x2 = 4.4;
                }
                else
                {
                    x1 = 0.5;
                    x2 = 4.7;
                }
            }
            else
            {
                if (factor <= 0.2)
                {
                    x1 = 0.7;
                    x2 = 4.5;
                }
                else if (factor <= 0.4)
                {
                    x1 = 0.6;
                    x2 = 4.7;
                }
                else if (factor <= 0.6)
                {
                    x1 = 0.5;
                    x2 = 4.9;
                }
                else if (factor <= 0.8)
                {
                    x1 = 0.5;
                    x2 = 5.2;
                }
                else
                {
                    x1 = 0.4;
                    x2 = 5.5;

                }
            }

            Double Kp = x1 / Kob;
            Double Ti = x2 * Tob;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public IAEMurataSagaraRegulatorMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Murata-Sagara 1997")
        {
        }
    }

    public class IAEMurataSagaraServoMethodPI : StrejcModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateDenominatorConstant(plantObject);
            Double T0 = plantObject.TimeDelay;
            Double n = CalculateRank(plantObject);

            Double x1 = 0;
            Double x2 = 0;

            Double factor = T0 / Tob;

            if (n < 5)
            {
                if (factor <= 0.2)
                {
                    x1 = 0.7;
                    x2 = 4.5;
                }
                else if (factor <= 0.4)
                {
                    x1 = 0.6;
                    x2 = 4.7;
                }
                else if (factor <= 0.6)
                {
                    x1 = 0.5;
                    x2 = 4.9;
                }
                else if (factor <= 0.8)
                {
                    x1 = 0.5;
                    x2 = 5.2;
                }
                else
                {
                    x1 = 0.4;
                    x2 = 5.5;
                }
            }
            else
            {
                if (factor <= 0.2)
                {
                    x1 = 0.6;
                    x2 = 4.1;
                }
                else if (factor <= 0.4)
                {
                    x1 = 0.5;
                    x2 = 4.3;
                }
                else if (factor <= 0.6)
                {
                    x1 = 0.5;
                    x2 = 4.6;
                }
                else if (factor <= 0.8)
                {
                    x1 = 0.5;
                    x2 = 5.0;
                }
                else
                {
                    x1 = 0.4;
                    x2 = 5.3;

                }
            }

            Double Kp = x1 / Kob;
            Double Ti = x2 * Tob;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public IAEMurataSagaraServoMethodPI()
            : base(TunningType.ServoTuning, PIDModeType.PI, "Murata-Sagara 1997")
        {
        }
    }
}
