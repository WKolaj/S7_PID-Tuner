using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOLPDModelTunningMethods
{
    //--------------------------PROCESS REACTION--------------------

    public class CallenderMethodPI : FOLPFModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Decay ratio = 0.015",
            "Decay ratio = 0.043"
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
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            switch (SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass((0.568) / (Kob * T0), 3.64 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((0.690) / (Kob * T0), 2.45 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }

        public CallenderMethodPI()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "Callender 1935/6")
        {

        }

    }

    public class ZNMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];
            
            return new PIDControllerClass((0.9*Tob) / (Kob * T0), 3.3 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public ZNMethodPI()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "Ziegler-Nichols")
        {

        }
    }

    public class CohenAndCoonMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.9 * (Tob / T0) + 0.083) / Kob;
            Double Ti = Tob * ((3.33 * (T0 / Tob) + 0.31 * (T0 / Tob) * (T0 / Tob)) / (1 + 2.22 * (T0 / Tob)));

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public CohenAndCoonMethodPI()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "Cohen and Coon")
        {

        }
    }

    public class MurrillMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.928/Kob)*Math.Pow((Tob/T0),0.946);
            Double Ti = (Tob/ 1.078) * Math.Pow((T0 / Tob), 0.583);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public MurrillMethodPI()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "Murrill 1967")
        {

        }
    }

    public class FertikSharpeMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.56 / Kob);
            Double Ti = (0.65 * Tob);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public FertikSharpeMethodPI()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "Fertik-Sharpe 1979")
        {

        }
    }

    public class BorresenAndGrindalMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (Tob / (Kob * T0));
            Double Ti = (3 * T0);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public BorresenAndGrindalMethodPI()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "Borresen and Grindal 1990")
        {

        }
    }

    public class McMillanMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (Kob/3);
            Double Ti = (T0);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public McMillanMethodPI()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "McMillan 1994")
        {

        }
    }

    public class StClairMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.333* Tob / (Kob * T0));
            Double Ti = (Tob);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public StClairMethodPI()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "St. Clair 1997")
        {

        }
    }

    public class FaanesAndSkogestadMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.71* Tob / (Kob * T0));
            Double Ti = (3.3*T0);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public FaanesAndSkogestadMethodPI()
            : base(TunningType.ProcessReaction, PIDModeType.PI, "Faanes-Skogestad 2004")
        {

        }
    }



    //---------------------------REGULATOR TUNNING------------------------

    public class ChienRegulatorMethodPI : FOLPFModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Overshoot = 0%",
            "Overshoot = 20%"
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
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            switch (SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass((0.6 * Tob) / (Kob * T0), 4 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((0.7 * Tob) / (Kob * T0), 2.33 * T0, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }

        public ChienRegulatorMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Chien 1952")
        {

        }
    }

    public class IAEMurrilMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.984 / Kob)*Math.Pow((Tob/T0),0.986);
            Double Ti = (Tob / 0.608) * Math.Pow((T0 / Tob ), 0.707);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public IAEMurrilMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum IAE - Murril 1967")
        {

        }
    }

    public class IAEPembertonMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (Tob / (Kob*T0));
            Double Ti = (Tob);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public IAEPembertonMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum IAE - Pemberton 1972")
        {

        }
    }

    public class IAEMarlinMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp;
            Double Ti;

            Double x = T0/Tob;

            if(x <=0.11)
            {
                Kp = 1.4/Kob;
                Ti = 0.24*T0;
            }
            else if(x<=0.25)
            {
                Kp = 1.8/Kob;
                Ti = 0.52*T0;
            }
            else if(x<=0.43)
            {
                Kp = 1.4/Kob;
                Ti = 0.75*T0;
            }
            else if(x<=0.67)
            {
                Kp = 1.0/Kob;
                Ti = 0.68*T0;
            }
            else if(x<=1.0)
            {
                Kp = 0.8/Kob;
                Ti = 0.71*T0;
            }
            else if(x<=1.5)
            {
                Kp = 0.55/Kob;
                Ti = 0.60*T0;
            }
            else if(x<=2.33)
            {
                Kp = 0.45/Kob;
                Ti = 0.54*T0;
            }
            else 
            {
                Kp = 0.35/Kob;
                Ti = 0.49*T0;
            }

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public IAEMarlinMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum IAE - Marlin 1995")
        {

        }
    }

    public class IAEAOMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (1/Kob)*(0.4485 + 0.6494*Math.Pow(Tob/T0,1.1251));
            Double Ti = (Tob) * (-0.2551 + 1.8205 * Math.Pow((T0 / Tob), 0.4749));

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public IAEAOMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum IAE - Arrieta Orozco 2003")
        {

        }
    }

    public class IAEShinskeyMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = 0.74 * Tob / (Kob * T0);
            Double Ti = 4.06 * T0;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public IAEShinskeyMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum IAE - Shinskey 2003")
        {

        }
    }

    public class IAETFMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (1 / Kob) * (0.4849 * (Tob / T0) + 0.3047);
            Double Ti = (Tob) * (0.4262 * (T0 / Tob) + 0.9581);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public IAETFMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum IAE - Tavakoli 2003")
        {

        }
    }

    public class ISEMurrillMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (1.305 / Kob ) * Math.Pow(Tob/T0,0.959);
            Double Ti = (Tob / 0.492) * Math.Pow(T0 / Tob, 0.739);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public ISEMurrillMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ISE - Murrill 1967")
        {

        }
    }

    public class ISEZhuangMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double x = T0 / Tob;

            Double Kp;
            Double Ti;

            if (x <= 1.0)
            {
                Kp = (0.980 / Kob) * Math.Pow(Tob / T0, 0.892);
                Ti = (Tob / ((0.690 - (0.155 * T0 / Tob))));
            }
            else
            {
                Kp = (1.072 / Kob) * Math.Pow(Tob / T0, 0.560);
                Ti = (Tob / ((0.648 - (0.114 * T0 / Tob))));
            }


            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public ISEZhuangMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ISE - Zhuang 1992")
        {

        }
    }

    public class ITAEMurrillMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.859 / Kob) * Math.Pow(Tob / T0, 0.977);
            Double Ti = (Tob / 0.674) * Math.Pow(T0 / Tob, 0.680);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public ITAEMurrillMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ITAE - Murrill 1967")
        {

        }
    }

    public class ITAEAOMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (1 / Kob) * (0.2607 + 0.6470 * Math.Pow(Tob / T0, 1.1055));
            Double Ti = (Tob) * (-1.5926 + 2.9191 * Math.Pow((T0 / Tob), 0.1789));

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public ITAEAOMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ITAE - Arrieta Orozco 2003")
        {

        }
    }

    public class ITAEBarberaMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double x = T0 / Tob;

            Double Kp = (1 / (Kob * (1.5853 * Math.Pow(T0 / Tob, 2) + 2.0214 * (T0 / Tob) + 0.00911)));
            Double Ti = 0.9894 * Math.Exp(-0.9707 * (T0 / Tob));

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public ITAEBarberaMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ITAE - Barbera 2006")
        {

        }
    }

    public class ITAEABBMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double x = T0 / Tob;

            Double Kp = (0.8591 / Kob) * Math.Pow(Tob / T0, 0.977);
            Double Ti = 1.4837 * Tob * Math.Pow(T0 / Tob, 0.68);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public ITAEABBMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ITAE - ABB 2001")
        {

        }
    }

    public class ISTSEZhuangMethodPI : FOLPFModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double x = T0/Tob;

            Double Kp;
            Double Ti;

            if(x <= 1.0)
            {
                Kp = (1.015 / Kob) * Math.Pow(Tob / T0, 0.957);
                Ti = (Tob / 0.667) * Math.Pow(T0 / Tob, 0.552);
            }
            else
            {
                Kp = (1.065 / Kob) * Math.Pow(Tob / T0, 0.673);
                Ti = (Tob / 0.687) * Math.Pow(T0 / Tob, 0.427);
            }
            

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
        }

        public ISTSEZhuangMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ISTSE - Zhuang 1992")
        {

        }
    }

    
    //--------------------------SERVO TUNING--------------------

    public class ChienServoMethodPI : FOLPFModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Overshoot = 0%",
            "Overshoot = 20%"
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
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            switch (SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass((0.35 * Tob) / (Kob * T0), 1.17 * Tob, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((0.6 * Tob) / (Kob * T0), Tob, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }

        public ChienServoMethodPI()
            : base(TunningType.ServoTuning, PIDModeType.PI, "Chien 1952")
        {

        }
    }

    //--------------------------TIME DOMAIN--------------------

    public class VariableSPMethodPI : FOLPFModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Overshoot = 0%",
            "Overshoot = 20%"
        };

        public Int32 SelectedIndex
        {
            get;
            set;
        }

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

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            switch (SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass((0.35 * Tob) / (Kob * T0), 1.17 * Tob, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((0.6 * Tob) / (Kob * T0), Tob, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }

        public VariableSPMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Step set point")
        {

        }
    }

    public class VariableDistrubanceMethodPI : FOLPFModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Overshoot = 0%",
            "Overshoot = 20%"
        };

        public Int32 SelectedIndex
        {
            get;
            set;
        }

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

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            switch (SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass((0.6 * Tob) / (Kob * T0), 0.8 * T0 + 0.5 * Tob, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((0.7 * Tob) / (Kob * T0), T0 + 0.3 * Tob, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }

        public VariableDistrubanceMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Step disturbance")
        {

        }
    }

    public class SmithMethodPI : FOLPFModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Overshoot = 1%",
            "Overshoot = 5%"
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
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            switch (SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass((0.44 * Tob) / (Kob * T0), Tob, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((0.52 * Tob) / (Kob * T0), Tob, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }

        public SmithMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Smith 1975")
        {

        }
    }

    public class BekkerMethodPI : FOLPFModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Damping factor = 0.0",
            "Damping factor = 0.6",
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

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            switch (SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass((1.571 * Tob) / (Kob * T0), Tob, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((0.403 * Tob) / (Kob * T0), Tob, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 2:
                    {
                        return new PIDControllerClass((0.368 * Tob) / (Kob * T0), Tob, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }

        public BekkerMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Bekker 1991")
        {

        }
    }

    public class KuhnMethodPI : FOLPFModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Normal tuning",
            "Rapid tuning",
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
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            switch (SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass((0.5) / (Kob), 0.5 * (Tob + T0), 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((1.0) / (Kob), 0.7 * (Tob + T0), 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }

        public KuhnMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Kuhn 1995")
        {

        }
    }

    public class TrybusMethodPI : FOLPFModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Overshoot = 0%",
            "Overshoot = 5%",
            "Overshoot = 10%",
            "Overshoot = 15%",
            "Overshoot = 20%",
            "Overshoot = 25%",
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
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double factor = T0 / Tob;

            if(factor <=2)
            {
                return MethodForFactorLessThen2(Kob, Tob, T0, plantObject);
            }
            else
            {
                return MethodForFactorGreaterThen2(Kob, Tob, T0, plantObject);
            }

        }

        private PIDControllerClass MethodForFactorLessThen2(Double Kob, Double Tob, Double T0, TransferFunctionClass plantObject)
        {
            switch (SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass((0.34 * Tob) / (Kob * T0), Tob, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((0.54) / (Kob), 0.7 * (Tob + T0), 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 2:
                    {
                        return new PIDControllerClass((0.64) / (Kob), 0.5 * (Tob + T0), 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 3:
                    {
                        return new PIDControllerClass((0.74) / (Kob), 0.7 * (Tob + T0), 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 4:
                    {
                        return new PIDControllerClass((0.82) / (Kob), 0.5 * (Tob + T0), 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 5:
                    {
                        return new PIDControllerClass((0.90) / (Kob), 0.7 * (Tob + T0), 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;

        }

        private PIDControllerClass MethodForFactorGreaterThen2(Double Kob, Double Tob, Double T0, TransferFunctionClass plantObject)
        {
            switch (SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass((0.17 * Tob) / (Kob * T0), Tob, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((0.27) / (Kob), 0.7 * (Tob + T0), 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 2:
                    {
                        return new PIDControllerClass((0.32) / (Kob), 0.5 * (Tob + T0), 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 3:
                    {
                        return new PIDControllerClass((0.37) / (Kob), 0.7 * (Tob + T0), 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
                case 4:
                    {
                        return new PIDControllerClass((0.41) / (Kob), 0.5 * (Tob + T0), 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 5:
                    {
                        return new PIDControllerClass((0.45) / (Kob), 0.7 * (Tob + T0), 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;

        }
        public TrybusMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Trybus 2005")
        {

        }
    }

    public class AH1MethodPI : FOLPFModelTunningMethodBase
    {

        public Int32 SelectedIndex
        {
            get;
            set;
        }

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (1 / Kob) * (0.14 + 0.28 * (Tob / T0));
            Double Ti = 0.33*T0 + (6.8*T0*Tob)/(10*T0 + Tob); 

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                
        }

        public AH1MethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Åström-Hägglund 2002")
        {

        }
    }

    public class AH2MethodPI : FOLPFModelTunningMethodBase
    {

        public Int32 SelectedIndex
        {
            get;
            set;
        }

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.15 / Kob) + ((0.35 - ((T0 * Tob) / ((Tob + T0) * (Tob + T0)))) * (Tob) / (Kob * T0));
            Double Ti = 0.35 * T0 + (6.7 * T0 * Tob * Tob) / (Tob * Tob + 2 * (Tob * T0) + 10 * T0 * T0);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public AH2MethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Åström-Hägglund 2004")
        {

        }
    }

    public class AH3MethodPI : FOLPFModelTunningMethodBase
    {

        public Int32 SelectedIndex
        {
            get;
            set;
        }

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.15 / Kob) + ((0.35 - ((T0 * Tob) / ((Tob + T0) * (Tob + T0)))) * (Tob) / (Kob * T0));
            Double Ti = 0.35 * T0 + (13 * T0 * Tob * Tob) / (Tob * Tob + 12 * (Tob * T0) + 7 * T0 * T0);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public AH3MethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Åström-Hägglund 2006")
        {

        }
    }

    public class ClarkeMethodPI : FOLPFModelTunningMethodBase
    {

        public Int32 SelectedIndex
        {
            get;
            set;
        }

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.5 / Kob) * Math.Sqrt(1+(Tob)/(T0));
            Double Ti = Math.Sqrt(Tob*T0);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ClarkeMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Clarke 2006")
        {

        }
    }

    public class WangMethodPI : FOLPFModelTunningMethodBase
    {

        public Int32 SelectedIndex
        {
            get;
            set;
        }

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double x1, x2, factor = T0 / Tob;

            if(factor <= 0.05)
            {
                x1 = 5.1;
                x2 = 0.23;
            }
            else if(factor <= 0.11)
            {
                x1 = 4.5;
                x2 = 0.34;
            }
            else if (factor <= 0.18)
            {
                x1 = 3.6;
                x2 = 0.44;
            }
            else if (factor <= 0.25)
            {
                x1 = 2.9;
                x2 = 0.5;
            }
            else if (factor <= 0.33)
            {
                x1 = 2.3;
                x2 = 0.57;
            }
            else if (factor <= 0.43)
            {
                x1 = 1.8;
                x2 = 0.6;
            }
            else if (factor <= 0.54)
            {
                x1 = 1.4;
                x2 = 0.63;
            }
            else if (factor <= 0.67)
            {
                x1 = 1.1;
                x2 = 0.65;
            }
            else if (factor <= 0.82)
            {
                x1 = 0.9;
                x2 = 0.64;
            }
            else if (factor <= 1.0)
            {
                x1 = 0.8;
                x2 = 0.63;
            }
            else if (factor <= 1.22)
            {
                x1 = 0.8;
                x2 = 0.62;
            }
            else if (factor <= 1.5)
            {
                x1 = 0.7;
                x2 = 0.61;
            }
            else if (factor <= 1.9)
            {
                x1 = 0.6;
                x2 = 0.58;
            }
            else if (factor <= 2.3)
            {
                x1 = 0.5;
                x2 = 0.56;
            }
            else if (factor <= 3.0)
            {
                x1 = 0.4;
                x2 = 0.5;
            }
            else if (factor <= 4.0)
            {
                x1 = 0.4;
                x2 = 0.5;
            }
            else if (factor <= 5.7)
            {
                x1 = 0.4;
                x2 = 0.46;
            }
            else if (factor <= 9.0)
            {
                x1 = 0.4;
                x2 = 0.44;
            }
            else
            {
                x1 = 0.3;
                x2 = 0.42;
            }

            Double Kp = ( x1/ Kob);
            Double Ti = x2*(Tob + T0);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public WangMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Wang 2003")
        {

        }
    }

}
