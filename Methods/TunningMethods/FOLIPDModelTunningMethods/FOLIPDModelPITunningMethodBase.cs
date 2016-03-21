using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOLIPDModelTunningMethods
{

    public class ITAEPoulinMethodPI : FOLIPDModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Process input disturbance",
            "Process output disturbance"
        };

        public String[] TypeOfProcess
        {
            get
            {
                return typeOfProcess;
            }

            set
            {
                typeOfProcess = value;
            }
        }

        public Int32 SelectedIndex
        {
            get;
            set;
        }


        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0;
            Double Ti = 0;
            Double x1 = 0;
            Double x2 = 0;

            Double factor = T0 / Tob;

            if (SelectedIndex == 0)
            {
                if (factor <= 0.2)
                {
                    x1 = 3.9465;
                    x2 = 0.5320;
                }
                else if (factor <= 0.4)
                {
                    x1 = 3.9981;
                    x2 = 0.5315;
                }
                else if (factor <= 0.6)
                {
                    x1 = 4.0397;
                    x2 = 0.5311;
                }
                else if (factor <= 0.8)
                {
                    x1 = 4.0397;
                    x2 = 0.5311;
                }
                else if (factor <= 1.0)
                {
                    x1 = 4.0397;
                    x2 = 0.5311;
                }
                else if (factor <= 1.2)
                {
                    x1 = 4.0337;
                    x2 = 0.5312;
                }
                else if (factor <= 1.4)
                {
                    x1 = 4.0278;
                    x2 = 0.5312;
                }
                else if (factor <= 1.6)
                {
                    x1 = 4.0278;
                    x2 = 0.5312;
                }
                else if (factor <= 1.8)
                {
                    x1 = 4.0218;
                    x2 = 0.5313;
                }
                else if (factor <= 2.0)
                {
                    x1 = 4.0099;
                    x2 = 0.5314;
                }
            }
            else if (SelectedIndex == 1)
            {
                if (factor <= 0.2)
                {
                    x1 = 5.0728;
                    x2 = 0.5231;
                }
                else if (factor <= 0.4)
                {
                    x1 = 4.9688;
                    x2 = 0.5237;
                }
                else if (factor <= 0.6)
                {
                    x1 = 4.8983;
                    x2 = 0.5241;
                }
                else if (factor <= 0.8)
                {
                    x1 = 4.8218;
                    x2 = 0.5245;
                }
                else if (factor <= 1.0)
                {
                    x1 = 4.7839;
                    x2 = 0.5249;
                }
                else if (factor <= 1.2)
                {
                    x1 = 4.7565;
                    x2 = 0.5250;
                }
                else if (factor <= 1.4)
                {
                    x1 = 4.7293;
                    x2 = 0.5252;
                }
                else if (factor <= 1.6)
                {
                    x1 = 4.7107;
                    x2 = 0.5254;
                }
                else if (factor <= 1.8)
                {
                    x1 = 4.7837;
                    x2 = 0.5256;
                }
                else if (factor <= 2.0)
                {
                    x1 = 4.6837;
                    x2 = 0.5257;
                }
            }

            Kp = (x2 / (Kob * (T0 + Tob))) * Math.Sqrt(((Tob * Tob) / (x1 * (T0 + Tob) * (T0 + Tob))) + 1);
            Ti = x1 * (T0 + Tob);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public ITAEPoulinMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ITAE - Poulin 1996")
        {

        }
    }

    public class VelaquezFigueroaRegulatorMethodPI : FOLIPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double Kp = (0.044 / Kob) * Math.Pow(T0 / Tob, 0.122);
            Double Ti = 6.824 * Tob * Math.Pow(T0 / Tob, 0.195);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public VelaquezFigueroaRegulatorMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Velázquez-Figueroa 1997")
        {

        }
    }

    public class VelaquezFigueroaServoMethodPI : FOLIPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double Kp = (0.042 / Kob) * Math.Pow(T0 / Tob, 0.406);
            Double Ti = 8.807 * Tob * Math.Pow(T0 / Tob, 0.119);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public VelaquezFigueroaServoMethodPI()
            : base(TunningType.ServoTuning, PIDModeType.PI, "Velázquez-Figueroa 1997")
        {

        }
    }

    public class HubaZakovaMethodPI : FOLIPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double factor = T0 / Tob;

            Double Kp = 0;
            Double Ti = 0;

            if (factor <= 0.5)
            {
                Kp = 0.193 / (Kob * T0);
                Ti = 3.717 * T0;
            }
            else if (factor <= 1)
            {
                Kp = 0.207 / (Kob * T0);
                Ti = 3.386 * T0;
            }
            else
            {
                Kp = 0.219 / (Kob * T0);
                Ti = 3.127 * T0;
            }

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public HubaZakovaMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Huba-Žáková 2003")
        {

        }
    }


}
