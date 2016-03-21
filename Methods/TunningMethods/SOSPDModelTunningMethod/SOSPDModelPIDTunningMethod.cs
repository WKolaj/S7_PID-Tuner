using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SOSPDModelTunningMethod
{
    public class AflaroRuizMethodPID : SOSPDTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "x1 = 1.0",
            "x1 = 1.1",
            "x1 = 1.2",
            "x1 = 1.3",
            "x1 = 1.4",
            "x1 = 1.5",
            "x1 = 1.6",
            "x1 = 1.7"
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
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double x1 = 1.0 + Convert.ToDouble(SelectedIndex)/10;

            Double[] timeConstants = CalculateDenominatorConstants(plantObject);

            Double T1 = timeConstants.Max();
            Double T2 = timeConstants.Min();
            
            Double T = (T1+T2)/2;

            Double Kp = (x1 / Kob)*(0.70+((2.0*T)/(T0)));
            Double Ti = ((16.57+(20.83*(T0)/(T)))/(1+(11.36*(T0)/(T))))* T0;
            Double Td = ((2.65 + (3.33 * (T0) / (T))) / (1 + (11.36 * (T0) / (T)))) * T0;

            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public AflaroRuizMethodPID()
            : base(TunningType.ProcessReaction, PIDModeType.PID, "Aflaro - Ruiz 2005",true)
        {
        }
    }

    public class IAELopezMethodPID : SOSPDTunningMethodBase
    {
        
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double x1 = 0;
            Double x2 = 0;
            Double x3 = 0;

            Double factor = T0/Tob;

            if(factor <= 0.1)
            {
                if(ksi >= 4.0)
                {
                    x1 = 68;
                    x2 = 0.53;
                    x3 = 0.29;
                }
                else if(ksi >=2.0)
                {
                    x1 = 40;
                    x2 = 0.56;
                    x3 = 0.25;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 35;
                    x2 = 0.56;
                    x3 = 0.22;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 31;
                    x2 = 0.59;
                    x3 = 0.24;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 30;
                    x2 = 0.57;
                    x3 = 0.26;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 28;
                    x2 = 0.59;
                    x3 = 0.26;
                }
                else
                {
                    x1 = 27;
                    x2 = 0.61;
                    x3 = 0.28;
                }
            }
            else if (factor <= 0.2)
            {
                if (ksi >= 4.0)
                {
                    x1 = 47;
                    x2 = 0.53;
                    x3 = 0.29;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 19.5;
                    x2 = 0.56;
                    x3 = 0.25;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 16.0;
                    x2 = 0.56;
                    x3 = 0.30;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 13.0;
                    x2 = 0.59;
                    x3 = 0.43;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 11.5;
                    x2 = 0.57;
                    x3 = 0.41;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 10.5;
                    x2 = 0.59;
                    x3 = 0.45;
                }
                else
                {
                    x1 = 9.5;
                    x2 = 0.61;
                    x3 = 0.48;
                }
            }
            else if (factor <= 0.5)
            {
                if (ksi >= 4.0)
                {
                    x1 = 16.5;
                    x2 = 1.14;
                    x3 = 0.29;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 7.8;
                    x2 = 1.14;
                    x3 = 0.39;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 6.0;
                    x2 = 1.11;
                    x3 = 0.46;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 4.0;
                    x2 = 1.06;
                    x3 = 0.59;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 3.5;
                    x2 = 1.05;
                    x3 = 0.69;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 2.6;
                    x2 = 1.00;
                    x3 = 0.82;
                }
                else
                {
                    x1 = 2.25;
                    x2 = 0.97;
                    x3 = 0.92;
                }
            }
            else if (factor <= 1.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 9.0;
                    x2 = 1.92;
                    x3 = 0.52;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 4.3;
                    x2 = 1.85;
                    x3 = 0.60;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 3.0;
                    x2 = 1.75;
                    x3 = 0.68;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 1.85;
                    x2 = 1.56;
                    x3 = 0.82;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 1.4;
                    x2 = 1.47;
                    x3 = 0.94;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 1.0;
                    x2 = 1.25;
                    x3 = 1.20;
                }
                else
                {
                    x1 = 0.78;
                    x2 = 1.11;
                    x3 = 1.40;
                }
            }
            else if (factor <= 2.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 4.8;
                    x2 = 3.3;
                    x3 = 0.98;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 2.3;
                    x2 = 2.9;
                    x3 = 1.00;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 1.65;
                    x2 = 2.6;
                    x3 = 1.05;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 1.05;
                    x2 = 2.3;
                    x3 = 1.10;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.77;
                    x2 = 1.92;
                    x3 = 1.15;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.52;
                    x2 = 1.56;
                    x3 = 1.30;
                }
                else
                {
                    x1 = 0.39;
                    x2 = 1.27;
                    x3 = 1.40;
                }
            }
            else if (factor <= 5.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 2.15;
                    x2 = 6.5;
                    x3 = 2.3;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 1.1;
                    x2 = 4.8;
                    x3 = 2.15;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 0.85;
                    x2 = 4.3;
                    x3 = 2.10;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 0.62;
                    x2 = 3.6;
                    x3 = 1.90;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.53;
                    x2 = 3.0;
                    x3 = 1.70;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.45;
                    x2 = 2.9;
                    x3 = 1.50;
                }
                else
                {
                    x1 = 0.42;
                    x2 = 2.7;
                    x3 = 1.40;
                }
            }
            else
            {
                if (ksi >= 4.0)
                {
                    x1 = 1.2;
                    x2 = 10.0;
                    x3 = 4.0;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 0.70;
                    x2 = 7.4;
                    x3 = 3.45;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 0.60;
                    x2 = 6.9;
                    x3 = 2.9;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 0.52;
                    x2 = 6.1;
                    x3 = 2.35;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.47;
                    x2 = 5.7;
                    x3 = 1.90;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.41;
                    x2 = 5.3;
                    x3 = 1.40;
                }
                else
                {
                    x1 = 0.38;
                    x2 = 4.9;
                    x3 = 1.15;
                }
            }

            Double Kp = Kob / x1;
            Double Ti = x2 * Tob;
            Double Td = x3 * Tob;

            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public IAELopezMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Minimum IAE - Lopez 1968", false)
        {
        }
    }

    public class ISELopezMethodPID : SOSPDTunningMethodBase
    {

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double x1 = 0;
            Double x2 = 0;
            Double x3 = 0;

            Double factor = T0 / Tob;

            if (factor <= 0.1)
            {
                if (ksi >= 4.0)
                {
                    x1 = 70;
                    x2 = 0.21;
                    x3 = 0.20;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 42;
                    x2 = 0.21;
                    x3 = 0.23;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 38;
                    x2 = 0.21;
                    x3 = 0.25;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 32;
                    x2 = 0.21;
                    x3 = 0.30;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 31;
                    x2 = 0.21;
                    x3 = 0.31;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 30;
                    x2 = 0.21;
                    x3 = 0.33;
                }
                else
                {
                    x1 = 29;
                    x2 = 0.21;
                    x3 = 0.34;
                }
            }
            else if (factor <= 0.2)
            {
                if (ksi >= 4.0)
                {
                    x1 = 39;
                    x2 = 0.38;
                    x3 = 0.20;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 21;
                    x2 = 0.38;
                    x3 = 0.31;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 17;
                    x2 = 0.38;
                    x3 = 0.36;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 13;
                    x2 = 0.38;
                    x3 = 0.44;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 12.0;
                    x2 = 0.38;
                    x3 = 0.48;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 10.5;
                    x2 = 0.38;
                    x3 = 0.54;
                }
                else
                {
                    x1 = 10.0;
                    x2 = 0.38;
                    x3 = 0.56;
                }
            }
            else if (factor <= 0.5)
            {
                if (ksi >= 4.0)
                {
                    x1 = 17.5;
                    x2 = 0.83;
                    x3 = 0.37;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 8.5;
                    x2 = 0.82;
                    x3 = 0.50;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 6.2;
                    x2 = 0.80;
                    x3 = 0.57;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 4.2;
                    x2 = 0.77;
                    x3 = 0.74;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 3.2;
                    x2 = 0.74;
                    x3 = 0.83;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 2.8;
                    x2 = 0.69;
                    x3 = 1.10;
                }
                else
                {
                    x1 = 2.4;
                    x2 = 0.67;
                    x3 = 1.10;
                }
            }
            else if (factor <= 1.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 9.5;
                    x2 = 1.49;
                    x3 = 0.66;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 4.4;
                    x2 = 1.39;
                    x3 = 0.77;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 3.2;
                    x2 = 1.33;
                    x3 = 0.85;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 2.0;
                    x2 = 1.18;
                    x3 = 1.05;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 1.55;
                    x2 = 1.11;
                    x3 = 1.20;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 1.1;
                    x2 = 0.95;
                    x3 = 1.45;
                }
                else
                {
                    x1 = 0.86;
                    x2 = 0.87;
                    x3 = 1.70;
                }
            }
            else if (factor <= 2.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 5.2;
                    x2 = 2.6;
                    x3 = 1.2;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 2.9;
                    x2 = 2.3;
                    x3 = 1.3;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 1.8;
                    x2 = 2.1;
                    x3 = 1.35;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 1.1;
                    x2 = 1.8;
                    x3 = 1.5;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.85;
                    x2 = 1.6;
                    x3 = 1.6;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.57;
                    x2 = 1.3;
                    x3 = 1.80;
                }
                else
                {
                    x1 = 0.45;
                    x2 = 1.1;
                    x3 = 2.00;
                } 
            }
            else if (factor <= 5.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 2.25;
                    x2 = 5.3;
                    x3 = 3.0;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 1.2;
                    x2 = 4.2;
                    x3 = 2.85;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 0.9;
                    x2 = 3.7;
                    x3 = 2.8;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 0.66;
                    x2 = 3.1;
                    x3 = 2.6;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.57;
                    x2 = 2.9;
                    x3 = 2.40;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.49;
                    x2 = 2.6;
                    x3 = 2.20;
                }
                else
                {
                    x1 = 0.44;
                    x2 = 2.40;
                    x3 = 2.00;
                } 
            }
            else
            {
                if (ksi >= 4.0)
                {
                    x1 = 1.25;
                    x2 = 8.3;
                    x3 = 3.0;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 0.80;
                    x2 = 6.7;
                    x3 = 2.85;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 0.67;
                    x2 = 6.3;
                    x3 = 4.0;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 0.58;
                    x2 = 5.9;
                    x3 = 3.0;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.53;
                    x2 = 5.9;
                    x3 = 2.50;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.48;
                    x2 = 5.6;
                    x3 = 1.85;
                }
                else
                {
                    x1 = 0.45;
                    x2 = 5.3;
                    x3 = 1.50;
                } 
            }

            Double Kp = Kob / x1;
            Double Ti = x2 * Tob;
            Double Td = x3 * Tob;

            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ISELopezMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Minimum ISE - Lopez 1968", false)
        {
        }
    }

    public class ITAESungMethodPID : SOSPDTunningMethodBase
    {

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

           
            Double factor = T0 / Tob;

            Double Kp = 0;

            if(factor < 0.9)
            {
                Kp = (-0.67 + 0.297 * Math.Pow(Tob / T0, 2.001) + 2.189 * Math.Pow(Tob / T0, 0.766)*ksi) / Kob;
            }
            else
            {
                Kp = (-0.365 + 0.260 * Math.Pow((T0/Tob)-1.4, 2) + 2.189 * Math.Pow(Tob / T0, 0.766) * ksi) / Kob;
            }

            Double Ti = 0;

            if (factor < 0.4)
            {
                Ti = Tob * (2.212 * Math.Pow(T0 / Tob, 0.520) - 0.3);
            }
            else
            {
                Double x1 = (1 - Math.Exp(ksi / (0.15 + 0.33 * (T0 / Tob)))) * (5.25 - 0.88 * (T0 / Tob - 2.8) * (T0 / Tob - 2.8));

                Ti = Tob * (-0.975 + 0.910 * (T0 / Tob - 1.845) * (T0 / Tob - 1.845) + x1);
            }

            Double Td = 0;

            Td = Tob / ((1 - Math.Exp(ksi / (-0.15 + 0.939 * Math.Pow(Tob / T0, 1.121)))) * (1.45 + 0.969 * Math.Pow(Tob / T0, 1.171)) - 1.9 + 1.576 * Math.Pow(Tob / T0, 0.530));


            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ITAESungMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Minimum ITAE - Sung 1996", false)
        {
        }
    }

    public class ChienMethodPID : SOSPDTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Robustness = 0.2",
            "Robustness = 0.4",
            "Robustness = 0.6",
            "Robustness = 1.0"
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
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double[] timeConstants = CalculateDenominatorConstants(plantObject);

            Double T1 = timeConstants.Max();
            Double T2 = timeConstants.Min();
            
            Double lambda = 0;

            if(SelectedIndex == 0)
            {
                lambda = 0.2;
            }
            else if (SelectedIndex == 1)
            {
                lambda = 0.4;
            }
            else if (SelectedIndex == 2)
            {
                lambda = 0.6;
            }
            else if (SelectedIndex == 3)
            {
                lambda = 1.0;
            }

            Double Kp = (lambda * T1 + T2) / (Kob * (1 + lambda * T0));
            Double Ti = T1+T2;
            Double Td = T1 * T2 / (T1 + T2);

            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ChienMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Chien - 1973", true)
        {
        }
    }

    public class ViteckovaMetodPID : SOSPDTunningMethodBase
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
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double x1 = 0;

            if(SelectedIndex == 0)
            {
                x1 = 0.736;
            }
            else if (SelectedIndex == 1)
            {
                x1 = 1.028;
            }
            else if (SelectedIndex == 2)
            {
                 x1 = 1.162;
            }
            else if (SelectedIndex == 3)
            {
                 x1 = 1.282;
            }
            else if (SelectedIndex == 4)
            {
                 x1 = 1.392;
            }
            else if (SelectedIndex == 5)
            {
                 x1 = 1.496;
            }
            else if (SelectedIndex == 6)
            {
                 x1 = 1.602;
            }
            else if (SelectedIndex == 7)
            {
                 x1 = 1.706;
            }
            else if (SelectedIndex == 8)
            {
                 x1 = 1.812;
            }
            else if (SelectedIndex == 9)
            {
                 x1 = 1.914;
            }
            else if (SelectedIndex == 10)
            {
                 x1 = 2.016;
            }


            Double Kp = x1*ksi*Tob / (Kob*T0);

            Double Ti = 2*ksi*Tob;


            Double Td = Tob/(2*ksi);

            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ViteckovaMetodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Vítečková 2000", false)
        {

        }
    }

    public class SkogestadMethodPID : SOSPDTunningMethodBase
    {

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double TCL = T0;

            Double Kp = 2*ksi*Tob/(Kob*(TCL+T0));

            Double Ti = 2*ksi*Tob;

            Double Td = Tob/(2*ksi);

            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public SkogestadMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Skogestad 2004", false)
        {
        }
    }

    public class BiMethodPID : SOSPDTunningMethodBase
    {

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double TCL = T0;

            Double Kp = 1.0128 * ksi * Tob / (Kob * T0);

            Double Ti = 1.9747 * Kob * T0;

            Double Td = (0.5064*Tob*Tob) / (Kob * T0);

            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public BiMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Bi 2000", false)
        {
        }
    }

    public class LavanyaMethodPID : SOSPDTunningMethodBase
    {

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double factor1 = 0.25 * T0;
            Double factor2 = 0.2 * Tob;
            Double lambda = factor1 > factor2 ? factor1 : factor2;

            Double factor = T0 / Tob;

            Double Kp = 0;

            Double Ti = 0;

            Double Td = 0;

            Double N = 0.1;

            if(factor < 1)
            {
                Kp = 2 * ksi * Tob / (Kob * (lambda + T0));
                Ti = 2 * ksi * Tob;
                Td = 0.5 * ksi / Tob;
            }
            else
            {
               Ti = 2*ksi*Tob - (2*lambda*lambda - T0*T0)/(2*(2*lambda + T0));
               Td = Ti - 2*ksi*Tob + (Tob*Tob - (T0*T0*T0)/(6*(2*lambda + T0)))/Ti;
               Kp = Ti / (Kob * (2 * lambda + T0));
            }

            return new PIDControllerClass(Kp, Ti, Td, N, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public LavanyaMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Lavanya 2006", false)
        {
        }
    }

}
