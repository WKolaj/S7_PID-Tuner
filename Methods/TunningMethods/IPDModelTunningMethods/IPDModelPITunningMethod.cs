using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPDModelTunningMethods
{
    public class ZiglerNicholsMethodPI: IPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[1];
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0.9 / (T0 * Kob);
            Double Ti = 3.33 * T0;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ZiglerNicholsMethodPI()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "Ziegler-Nichols")
        {

        }
    }

    public class WolfeMethodPI : IPDModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Decay ratio = 0.4",
            "Decay ratio = Small"
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
            Double Ti = 0;

            switch (SelectedIndex)
            {
                case 0:
                    {
                        Kp = 0.6 / (T0 * Kob);
                        Ti = 2.78 * T0;
                        break;
                    }

                case 1:
                    {
                        Kp = 0.87 / (T0 * Kob);
                        Ti = 4.35 * T0;
                        break;
                    }
            }

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public WolfeMethodPI()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "Wolfe - 1951")
        {

        }
    }

    public class AH1MethodPI : IPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[1];
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0.63 / (T0 * Kob);
            Double Ti = 3.2 * T0;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public AH1MethodPI()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "Åström-Hägglund 1995")
        {

        }
    }

    public class LabviewMethodPI : IPDModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Quarter decay ratio",
            "Some overshoot",
            "No overshoot"
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
            Double Ti = 0;

            switch (SelectedIndex)
            {
                case 0:
                    {
                        Kp = 0.9 / (T0 * Kob);
                        Ti = 3.33 * T0;
                        break;
                    }

                case 1:
                    {
                        Kp = 0.4 / (T0 * Kob);
                        Ti = 5.33 * T0;
                        break;
                    }
                case 2:
                    {
                        Kp = 0.24 / (T0 * Kob);
                        Ti = 5.33 * T0;
                        break;
                    }
            }

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public LabviewMethodPI()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "NI Labview")
        {

        }
    }

    public class IAEShinskeyMethodPI : IPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[1];
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0.9524 / (T0 * Kob);
            Double Ti = 4 * T0;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public IAEShinskeyMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum IAE - Shinskey 1994")
        {

        }
    }

    public class ISEHazebroekMethodPI : IPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[1];
            Double T0 = plantObject.TimeDelay;

            Double Kp = 1.5 / (T0 * Kob);
            Double Ti = 5.56 * T0;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ISEHazebroekMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ISE - Hazebroek 1950")
        {

        }
    }

    public class ITAEPoulinMethodPI : IPDModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Process output step load disturbance",
            "Process input step load disturbance",
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
            Double Ti = 0;

            switch (SelectedIndex)
            {
                case 0:
                    {
                        Kp = 0.5264 / (T0 * Kob);
                        Ti = 4.5804 * T0;
                        break;
                    }

                case 1:
                    {
                        Kp = 0.5327 / (T0 * Kob);
                        Ti = 3.8853 * T0;
                        break;
                    }
            }

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public ITAEPoulinMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ITAE - Poulin 1996")
        {

        }
    }

    public class SkogestadMethodPI : IPDModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Closed loop sensitivity = 1.4",
            "Closed loop sensitivity = 1.7",
            "Closed loop sensitivity = 2.0"
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
            Double Ti = 0;

            switch (SelectedIndex)
            {
                case 0:
                    {
                        Kp = 0.28 / (T0 * Kob);
                        Ti = 7 * T0;
                        break;
                    }

                case 1:
                    {
                        Kp = 0.404 / (T0 * Kob);
                        Ti = 7 * T0;
                        break;
                    }
                case 2:
                    {
                        Kp = 0.49 / (T0 * Kob);
                        Ti = 3.77 * T0;
                        break;
                    }
            }

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public SkogestadMethodPI()
            : base(TunningType.OtherTuning, PIDModeType.PI, "Skogestad 2001")
        {

        }
    }

    public class FruehaufMethodPI : IPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[1];
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0.5 / (T0 * Kob);
            Double Ti = 5 * T0;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public FruehaufMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Fruehauf 1993")
        {

        }
    }

    public class CluettWangfMethodPI : IPDModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Closed loop system time constant = T0",
            "Closed loop system time constant = 2T0",
            "Closed loop system time constant = 3T0",
            "Closed loop system time constant = 4T0",
            "Closed loop system time constant = 5T0",
            "Closed loop system time constant = 6T0"
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
            Double Ti = 0;

            switch (SelectedIndex)
            {
                case 0:
                    {
                        Kp = 0.9588/ (T0 * Kob);
                        Ti = 3.0425 * T0;
                        break;
                    }

                case 1:
                    {
                        Kp = 0.6232 / (T0 * Kob);
                        Ti = 5.2586 * T0;
                        break;
                    }
                case 2:
                    {
                        Kp = 0.4668 / (T0 * Kob);
                        Ti = 7.2291 * T0;
                        break;
                    }

                case 3:
                    {
                        Kp = 0.3752 / (T0 * Kob);
                        Ti = 9.1925 * T0;
                        break;
                    }
                case 4:
                    {
                        Kp = 0.3114 / (T0 * Kob);
                        Ti = 11.1637 * T0;
                        break;
                    }

                case 5:
                    {
                        Kp = 0.2709 / (T0 * Kob);
                        Ti = 13.1416 * T0;
                        break;
                    }
            }

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public CluettWangfMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Cluett-Wang 1997")
        {

        }
    }

    public class KooksMethodPI : IPDModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Gain margin = 1.5 Phase margin = 22.5",
            "Gain margin = 2.0 Phase margin = 30.0",
            "Gain margin = 3.0 Phase margin = 45.0",
            "Gain margin = 4.0 Phase margin = 60.0",
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
            Double Ti = 0;

            switch (SelectedIndex)
            {
                case 0:
                    {
                        Kp = 0.942 / (T0 * Kob);
                        Ti = 4.510 * T0;
                        break;
                    }

                case 1:
                    {
                        Kp = 0.698 / (T0 * Kob);
                        Ti = 4.098 * T0;
                        break;
                    }
                case 2:
                    {
                        Kp = 0.491 / (T0 * Kob);
                        Ti = 6.942 * T0;
                        break;
                    }

                case 3:
                    {
                        Kp = 0.384 / (T0 * Kob);
                        Ti = 18.710 * T0;
                        break;
                    }
            }

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }


        public KooksMethodPI()
            : base(TunningType.FrequencyDomain, PIDModeType.PI, "Kooks 1999")
        {

        }
    }

    public class ODwayerMethodPI : IPDModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Gain margin = 1.5 Phase margin = 46.2",
            "Gain margin = 2.0 Phase margin = 45.5",
            "Gain margin = 3.0 Phase margin = 59.9",
            "Gain margin = 4.0 Phase margin = 60.0",
            "Gain margin = 5.0 Phase margin = 75.0"
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
            Double Ti = 0;

            switch (SelectedIndex)
            {
                case 0:
                    {
                        Kp = 0.558 / (T0 * Kob);
                        Ti = 1.4 * T0;
                        break;
                    }

                case 1:
                    {
                        Kp = 0.484 / (T0 * Kob);
                        Ti = 1.55 * T0;
                        break;
                    }
                case 2:
                    {
                        Kp = 0.458 / (T0 * Kob);
                        Ti = 3.35 * T0;
                        break;
                    }
                case 3:
                    {
                        Kp = 0.357 / (T0 * Kob);
                        Ti = 4.3 * T0;
                        break;
                    }
                case 4:
                    {
                        Kp = 0.305 / (T0 * Kob);
                        Ti = 12.15 * T0;
                        break;
                    }
            }

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }


        public ODwayerMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "O’Dwyer 2001")
        {

        }
    }

    public class AH2MethodPI : IPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[1];
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0.35 / (T0 * Kob);
            Double Ti = 7 * T0;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public AH2MethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Åström-Hägglund 2004")
        {

        }
    }

    public class AH3MethodPI : IPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[1];
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0.35 / (T0 * Kob);
            Double Ti = 13.4 * T0;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public AH3MethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Åström-Hägglund 2006")
        {

        }
    }

    public class OgawaMethodPI : IPDModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "20% uncertainty in process parameters",
            "30% uncertainty in process parameters",
            "40% uncertainty in process parameters",
            "50% uncertainty in process parameters",
            "60% uncertainty in process parameters"
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
            Double Ti = 0;

            switch (SelectedIndex)
            {
                case 0:
                    {
                        Kp = 0.45 / (T0 * Kob);
                        Ti = 11 * T0;
                        break;
                    }

                case 1:
                    {
                        Kp = 0.39 / (T0 * Kob);
                        Ti = 12 * T0;
                        break;
                    }
                case 2:
                    {
                        Kp = 0.34 / (T0 * Kob);
                        Ti = 13 * T0;
                        break;
                    }
                case 3:
                    {
                        Kp = 0.30 / (T0 * Kob);
                        Ti = 14 * T0;
                        break;
                    }
                case 4:
                    {
                        Kp = 0.27 / (T0 * Kob);
                        Ti = 15 * T0;
                        break;
                    }
            }

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }


        public OgawaMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Ogawa 1995")
        {

        }
    }

}
