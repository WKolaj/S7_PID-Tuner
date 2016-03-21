using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPDModelTunningMethods
{
    public class ZiglerNicholsMethodP: IPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[1];
            Double T0 = plantObject.TimeDelay;

            Double Kp = 1.0/(T0*Kob);

            return new PIDControllerClass(Kp, 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                 
        }

        public ZiglerNicholsMethodP()
            : base(TunningType.ProcessReaction, PIDModeType.P, "Ziegler-Nichols")
        {

        }
    }

    public class LabviewMethodP : IPDModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Overshoot = 0%",
            "Some oversoot"
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
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[1];
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0;

            switch (SelectedIndex)
            {
                case 0:
                    {
                        Kp = 0.26/(T0*Kob);
                        break;
                    }

                case 1:
                    {
                        Kp = 0.44/(T0*Kob);
                        break;
                    }
            }

            return new PIDControllerClass(Kp, 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public LabviewMethodP()
            : base(TunningType.ProcessReaction, PIDModeType.P, "NI Labview")
        {

        }
    }

    public class ISEHaalmanMethodP : IPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[1];
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0.6667 / (T0 * Kob);

            return new PIDControllerClass(Kp, 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ISEHaalmanMethodP()
            : base(TunningType.RegulatorTuning, PIDModeType.P, "Minimum ISE - Haalman")
        {

        }
    }

    public class ViteckovaMethodP : IPDModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Overshoot = 0%",
            "Overshoot = 5%",
            "Overshoot = 10%",
            "Overshoot = 15%",
            "Overshoot = 20%",
            "Overshoot = 25%",
            "Overshoot = 30%",
            "Overshoot = 35%",
            "Overshoot = 40%",
            "Overshoot = 45%",
            "Overshoot = 50%"
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
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[1];
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0;

            switch (SelectedIndex)
            {
                case 0:
                    {
                        Kp = 0.368 / (T0 * Kob);
                        break;
                    }

                case 1:
                    {
                        Kp = 0.514 / (T0 * Kob);
                        break;
                    }
                case 2:
                    {
                        Kp = 0.581 / (T0 * Kob);
                        break;
                    }

                case 3:
                    {
                        Kp = 0.641 / (T0 * Kob);
                        break;
                    }
                case 4:
                    {
                        Kp = 0.696 / (T0 * Kob);
                        break;
                    }

                case 5:
                    {
                        Kp = 0.748 / (T0 * Kob);
                        break;
                    }
                case 6:
                    {
                        Kp = 0.801 / (T0 * Kob);
                        break;
                    }

                case 7:
                    {
                        Kp = 0.853 / (T0 * Kob);
                        break;
                    }
                case 8:
                    {
                        Kp = 0.906 / (T0 * Kob);
                        break;
                    }
                case 9:
                    {
                        Kp = 0.957 / (T0 * Kob);
                        break;
                    }
                case 10:
                    {
                        Kp = 1.008 / (T0 * Kob);
                        break;
                    }
            }

            return new PIDControllerClass(Kp, 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public ViteckovaMethodP()
            : base(TunningType.TimeDomain, PIDModeType.P, "Vítečková - 1999")
        {

        }
    }
}
