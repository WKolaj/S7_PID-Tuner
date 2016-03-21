using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOLPDModelTunningMethods
{
    public class VariableSPMethodPID : FOLPFModelTunningMethodBase
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
                        return new PIDControllerClass((0.6 * Tob) / (Kob * T0), Tob, 0.5 * T0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((0.95 * Tob) / (Kob * T0), 1.36 * Tob, 0.64 * T0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }

        public VariableSPMethodPID()
            : base(TunningType.TimeDomain, PIDModeType.PID, "Step set point")
        {

        }
    }

    public class VariableDisturbanceMethodPID : FOLPFModelTunningMethodBase
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
                        return new PIDControllerClass((0.95 * Tob) / (Kob * T0), 2.4*T0, 0.4 * T0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((0.95 * Tob) / (Kob * T0), 2.0 * Tob, 0.4 * T0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }

        public VariableDisturbanceMethodPID()
            : base(TunningType.TimeDomain, PIDModeType.PID, "Step disturbance")
        {

        }
    }

    public class IAEAlfaRuizMethodPIDRegulator : FOLPFModelTunningMethodBase
    {
        
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];
            
            Double Kp = (0.3295 + 0.7182 * Math.Pow(( Tob / T0 ),0.9971))/Kob;
            Double Ti = Tob*(0.9781 + 0.3723 * Math.Pow(( T0 / Tob ),0.8456));
            Double Td = 0.3416 * Tob * Math.Pow(( T0 / Tob ),0.9414);

            return new PIDControllerClass(Kp, Ti, Td, 0.1,TypeOfAglorithm, plantObject.SampleTime / 1000);
              
        }

        public IAEAlfaRuizMethodPIDRegulator()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Minimum IAE - Alfa Ruiz")
        {

        }
    }

    public class IAEAlfaRuizMethodPIDServo : FOLPFModelTunningMethodBase
    {

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.2068 + 1.1597 * Math.Pow((Tob / T0), 1.0158)) / Kob;
            Double Ti = Tob * (-0.2228 + 1.3009 * Math.Pow((T0 / Tob), 0.5022));
            Double Td = 0.3953 * Tob * Math.Pow((T0 / Tob), 0.8469);

            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public IAEAlfaRuizMethodPIDServo()
            : base(TunningType.ServoTuning, PIDModeType.PID, "Minimum IAE - Alfa Ruiz")
        {

        }
    }

    public class IAEAOMethodPID : FOLPFModelTunningMethodBase
    {

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.1050 + 1.2432 * Math.Pow((Tob / T0), 0.9946)) / Kob;
            Double Ti = Tob*(-0.2512 + 1.3581 * Math.Pow((T0 / Tob), 0.4796));
            Double Td = Tob *(-0.0003 + 0.3838 * Math.Pow((T0 / Tob), 0.9479));

            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public IAEAOMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Minimum IAE - Arrieta Orozco")
        {

        }
    }

    public class ITAEAlfaRuizMethodPID : FOLPFModelTunningMethodBase
    {

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.1230+ 1.1891 * Math.Pow((Tob / T0), 1.0191)) / Kob;
            Double Ti = Tob*(-0.3173 + 1.4489 * Math.Pow((T0 / Tob), 0.4440));
            Double Td = Tob * (0.0053 + 0.3695 * Math.Pow((T0 / Tob), 0.9286));

            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ITAEAlfaRuizMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Minimum ITAE - Alfa Ruiz")
        {

        }
    }

    public class ITAEAOMethodPID : FOLPFModelTunningMethodBase
    {

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (0.1230 + 1.1891 * Math.Pow((Tob / T0), 1.0191)) / Kob;
            Double Ti = Tob*(-0.3173 + 1.4489 * Math.Pow((T0 / Tob), 0.4440));
            Double Td = Tob * (-0.0053 + 0.3695 * Math.Pow((T0 / Tob), 0.9286));

            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ITAEAOMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Minimum ITAE - Arrieta Orozco")
        {

        }
    }

    public class ISEArrietaVilanovaMethodPID : FOLPFModelTunningMethodBase
    {

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double factor = T0/Tob;

            Double x1;
            Double Kp;
            Double Ti;
            Double Td;

            if(factor <= 1.0)
            {
                x1 = 0.5778 - 0.5753 * factor + 0.5528 * factor * factor;

                Kp = (x1*1.473/Kob)*Math.Pow(Tob/T0,0.970) + ((1-x1)*1.048/Kob)*Math.Pow(Tob/T0,0.897);
                Ti = (x1 * Tob / 1.115) * Math.Pow(T0 / Tob, 0.753) + ((1 - x1) * Tob / (1.195 - 0.368 * (T0 / Tob)));
                Td = (x1 * 0.550 *Tob) * Math.Pow(T0 / Tob, 0.948) + (0.489*(1 - x1) * Tob) * Math.Pow(T0 / Tob, 0.888);
            }
            else
            {
                x1 = 0.2382+0.2313*Math.Pow(Tob/T0,7.1208);

                Kp = (x1 * 1.524 / Kob) * Math.Pow(Tob / T0, 0.735) + ((1 - x1) * 1.154 / Kob) * Math.Pow(Tob / T0, 0.567);
                Ti = (x1 * Tob / 1.130) * Math.Pow(T0 / Tob, 0.641) + ((1 - x1) * Tob / (1.047 - 0.220 * (T0 / Tob)));
                Td = (x1 * 0.552 * Tob) * Math.Pow(T0 / Tob, 0.851) + (0.490 * (1 - x1) * Tob) * Math.Pow(T0 / Tob, 0.708);
            }
            

            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ISEArrietaVilanovaMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Minimum ISE - Arrieta Vilanova")
        {

        }
    }

    public class ISTSEArrietaVilanovaMethodPID : FOLPFModelTunningMethodBase
    {

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double factor = T0 / Tob;

            Double x1;
            Double Kp;
            Double Ti;
            Double Td;

            if (factor <= 1.0)
            {
                x1 = 0.5778 - 0.5753 * factor + 0.5528 * factor * factor;

                Kp = (x1 * 1.468 / Kob) * Math.Pow(Tob / T0, 0.970) + ((1 - x1) * 1.042 / Kob) * Math.Pow(Tob / T0, 0.897);
                Ti = (x1 * Tob / 0.942) * Math.Pow(T0 / Tob, 0.725) + ((1 - x1) * Tob / (0.987 - 0.238 * (T0 / Tob)));
                Td = (x1 * 0.443 * Tob) * Math.Pow(T0 / Tob, 0.939) + (0.385 * (1 - x1) * Tob) * Math.Pow(T0 / Tob, 0.906);
            }
            else
            {
                x1 = 0.2382 + 0.2313 * Math.Pow(Tob / T0, 7.1208);

                Kp = (x1 * 1.515 / Kob) * Math.Pow(Tob / T0, 0.730) + ((1 - x1) * 1.142 / Kob) * Math.Pow(Tob / T0, 0.579);
                Ti = (x1 * Tob / 0.957) * Math.Pow(T0 / Tob, 0.598) + ((1 - x1) * Tob / (0.919 - 0.172 * (T0 / Tob)));
                Td = (x1 * 0.444 * Tob) * Math.Pow(T0 / Tob, 0.847) + (0.384 * (1 - x1) * Tob) * Math.Pow(T0 / Tob, 0.839);
            }


            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ISTSEArrietaVilanovaMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Minimum ISTSE - Arrieta Vilanova")
        {

        }
    }

    public class ISTESArrietaVilanovaMethodPID : FOLPFModelTunningMethodBase
    {

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double factor = T0 / Tob;

            Double x1;
            Double Kp;
            Double Ti;
            Double Td;

            if (factor <= 1.0)
            {
                x1 = 0.5778 - 0.5753 * factor + 0.5528 * factor * factor;

                Kp = (x1 * 1.531 / Kob) * Math.Pow(Tob / T0, 0.960) + ((1 - x1) * 0.968 / Kob) * Math.Pow(Tob / T0, 0.904);
                Ti = (x1 * Tob / 0.971) * Math.Pow(T0 / Tob, 0.746) + ((1 - x1) * Tob / (0.977 - 0.253 * (T0 / Tob)));
                Td = (x1 * 0.413 * Tob) * Math.Pow(T0 / Tob, 0.933) + (0.316 * (1 - x1) * Tob) * Math.Pow(T0 / Tob, 0.892);
            }
            else
            {
                x1 = 0.2382 + 0.2313 * Math.Pow(Tob / T0, 7.1208);

                Kp = (x1 * 1.592 / Kob) * Math.Pow(Tob / T0, 0.705) + ((1 - x1) * 1.061 / Kob) * Math.Pow(Tob / T0, 0.583);
                Ti = (x1 * Tob / 0.957) * Math.Pow(T0 / Tob, 0.597) + ((1 - x1) * Tob / (0.892 - 0.165 * (T0 / Tob)));
                Td = (x1 * 0.414 * Tob) * Math.Pow(T0 / Tob, 0.850) + (0.315 * (1 - x1) * Tob) * Math.Pow(T0 / Tob, 0.832);
            }

            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ISTESArrietaVilanovaMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Minimum ISTES - Arrieta Vilanova")
        {

        }
    }

    public class KuhnMethodPID : FOLPFModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Normal tunning",
            "Rapid tunning"
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
                        return new PIDControllerClass((1) / (Kob), 0.66 * (Tob + T0), 0.167 * (Tob + T0), 0.5, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((2) / (Kob), 0.8 * (Tob + T0), 0.194 * (Tob + T0), 0.5, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }

        public KuhnMethodPID()
            : base(TunningType.TimeDomain, PIDModeType.PID, "Kuhn 1995")
        {

        }
    }
    
}
