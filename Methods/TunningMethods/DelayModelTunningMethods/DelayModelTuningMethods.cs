using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicMethodsLibrary;

namespace DelayModelTunningMethods
{
    public class CallenderMethod : DelayModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Decay ratio = 0.015",
            "Decay ratio = 0.100",
            "Decay ratio = 0.120",
            "Decay ratio = 0.330"
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

        public CallenderMethod() : base (TunningType.ProcessReaction,PIDModeType.PI,"Callender 1935/6")
        {

        }

        public override PIDControllerClass TuningMethod(TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0]/plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;

            switch(SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass(0.568 / (Kob * T0), 3.64 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime/1000);
                    }
                case 1:
                    {
                        return new PIDControllerClass(0.65 / (Kob * T0), 2.6 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 2:
                    {
                        return new PIDControllerClass(0.79 / (Kob * T0), 3.95 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 3:
                    {
                        return new PIDControllerClass(0.95 / (Kob * T0), 3.3 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }
    }

    public class WolfeMethod : DelayModelTunningMethodBase
    {

        public WolfeMethod()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "Wolfe 1951")
        {

        }

        public override PIDControllerClass TuningMethod(TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;

            return new PIDControllerClass(0.2 / (Kob * T0), 0.3 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }
    }

    public class MinimumError : DelayModelTunningMethodBase
    {

        public MinimumError()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum error: step load change")
        {

        }

        public override PIDControllerClass TuningMethod(TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;

            return new PIDControllerClass(0.3 / (Kob), 0.42 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }
    }

    public class MinimumIAERegulatorTuning : DelayModelTunningMethodBase
    {

        public MinimumIAERegulatorTuning()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum IAE")
        {

        }

        public override PIDControllerClass TuningMethod(TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;

            return new PIDControllerClass(0.4 / (Kob), 0.5 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }
    }

    public class MinimumIAEServoTuning : DelayModelTunningMethodBase
    {

        public MinimumIAEServoTuning()
            : base(TunningType.ServoTuning, PIDModeType.PI, "Minimum IAE")
        {

        }

        public override PIDControllerClass TuningMethod(TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;

            return new PIDControllerClass(0.36 / (Kob), 0.47 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }
    }

    public class FetrikMethod : DelayModelTunningMethodBase
    {

        public FetrikMethod()
            : base(TunningType.OtherTuning, PIDModeType.PI, "Fetrik 1975")
        {

        }

        public override PIDControllerClass TuningMethod(TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;

            return new PIDControllerClass(0.35 / (Kob), 0.4 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }
    }

    public class AHMethod : DelayModelTunningMethodBase
    {

        public AHMethod()
            : base(TunningType.OtherTuning, PIDModeType.PI, "Åström and Hägglund 2000")
        {

        }

        public override PIDControllerClass TuningMethod(TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;

            return new PIDControllerClass(0.25 / (Kob), 0.35 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }
    }

    public class SkogestadMethod : DelayModelTunningMethodBase
    {

        public SkogestadMethod()
            : base(TunningType.OtherTuning, PIDModeType.PI, "Skogestad 2003")
        {

        }

        public override PIDControllerClass TuningMethod(TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;

            return new PIDControllerClass(0.2 / (Kob), 0.318 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }
    }

    public class VanDerGrintenMethod : DelayModelTunningMethodBase
    {

        public VanDerGrintenMethod()
            : base(TunningType.OtherTuning, PIDModeType.PI, "Van der Grinten 1963 - Step disturbance")
        {

        }

        public override PIDControllerClass TuningMethod(TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;

            return new PIDControllerClass(0.5 / (Kob), 0.5 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }
    }

    public class HansenMethod : DelayModelTunningMethodBase
    {

        public HansenMethod()
            : base(TunningType.OtherTuning, PIDModeType.PI, "Hansen 2000")
        {

        }

        public override PIDControllerClass TuningMethod(TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;

            return new PIDControllerClass(0.2 / (Kob), 0.3 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }
    }

    public class McMillanMethod : DelayModelTunningMethodBase
    {

        public McMillanMethod()
            : base(TunningType.OtherTuning, PIDModeType.PI, "McMillan 2005")
        {

        }

        public override PIDControllerClass TuningMethod(TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;

            return new PIDControllerClass(0.25 / (Kob), 0.25 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }
    }

    public class AHDampingMethod : DelayModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Damping factor = 0.1",
            "Damping factor = 0.2",
            "Damping factor = 0.3",
            "Damping factor = 0.4",
            "Damping factor = 0.5",
            "Damping factor = 0.6",
            "Damping factor = 0.7",
            "Damping factor = 0.8",
            "Damping factor = 0.9",
            "Damping factor = 1.0"
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

        public AHDampingMethod()
            : base(TunningType.OtherTuning, PIDModeType.PI, "Åström and Hägglund - Damping factor")
        {

        }

        public override PIDControllerClass TuningMethod(TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;

            switch (SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass(0.388 / (Kob), 0.258 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 1:
                    {
                        return new PIDControllerClass(0.343 / (Kob ), 0.270 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 2:
                    {
                        return new PIDControllerClass(0.305 / (Kob ), 0.279 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 3:
                    {
                        return new PIDControllerClass(0.273 / (Kob ), 0.285 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 4:
                    {
                        return new PIDControllerClass(0.244 / (Kob), 0.288 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 5:
                    {
                        return new PIDControllerClass(0.218 / (Kob ), 0.288 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 6:
                    {
                        return new PIDControllerClass(0.195 / (Kob ), 0.284 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 7:
                    {
                        return new PIDControllerClass(0.174 / (Kob ), 0.276 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 8:
                    {
                        return new PIDControllerClass(0.154 / (Kob ), 0.265 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 9:
                    {
                        return new PIDControllerClass(0.135 / (Kob ), 0.250 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }
    }

    public class AHClosedLoopSensitivityMethod : DelayModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Closed loop sensitivity = 1.1",
            "Closed loop sensitivity = 1.2",
            "Closed loop sensitivity = 1.3",
            "Closed loop sensitivity = 1.4",
            "Closed loop sensitivity = 1.5",
            "Closed loop sensitivity = 1.6",
            "Closed loop sensitivity = 1.7",
            "Closed loop sensitivity = 1.8",
            "Closed loop sensitivity = 1.9",
            "Closed loop sensitivity = 2.0",
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

        public AHClosedLoopSensitivityMethod()
            : base(TunningType.OtherTuning, PIDModeType.PI, "Åström and Hägglund - Closed Loop Sensitivity")
        {

        }

        public override PIDControllerClass TuningMethod(TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;

            switch (SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass(0.057 / (Kob), 0.400 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 1:
                    {
                        return new PIDControllerClass(0.103/ (Kob), 0.389 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 2:
                    {
                        return new PIDControllerClass(0.139 / (Kob), 0.376 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 3:
                    {
                        return new PIDControllerClass(0.168 / (Kob), 0.363 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 4:
                    {
                        return new PIDControllerClass(0.191 / (Kob), 0.352 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 5:
                    {
                        return new PIDControllerClass(0.211 / (Kob), 0.342 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 6:
                    {
                        return new PIDControllerClass(0.227 / (Kob), 0.334 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 7:
                    {
                        return new PIDControllerClass(0.241 / (Kob), 0.326 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 8:
                    {
                        return new PIDControllerClass(0.254 / (Kob), 0.320 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 9:
                    {
                        return new PIDControllerClass(0.254 / (Kob), 0.314 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }
    }
}
