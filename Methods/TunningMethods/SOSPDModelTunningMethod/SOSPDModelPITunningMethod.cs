using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SOSPDModelTunningMethod
{
    public class IAELopezMethodPI : SOSPDTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double x1 = 0;
            Double x2 = 0;

            Double factor = T0/Tob;

            if(factor <= 0.1)
            {
                if(ksi >= 4.0)
                {
                    x1 = 35;
                    x2 = 1.4;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 21;
                    x2 = 1.4;
                }

                else if (ksi >= 1.5)
                {
                    x1 = 15;
                    x2 = 1.7;
                }

                else if (ksi >= 1.0)
                {
                    x1 = 9.7;
                    x2 = 2.3;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 7.8;
                    x2 = 2.8;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 5.7;
                    x2 = 3.4;
                }
                else
                {
                    x1 = 4.8;
                    x2 = 4.2;
                }
                
                
            }
            else if(factor <= 0.2)
            {
                if (ksi >= 4.0)
                {
                    x1 = 27;
                    x2 = 1.2;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 11.5;
                    x2 = 1.7;
                }

                else if (ksi >= 1.5)
                {
                    x1 = 8.3;
                    x2 = 1.9;
                }

                else if (ksi >= 1.0)
                {
                    x1 = 5.1;
                    x2 = 2.4;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 3.9;
                    x2 = 2.7;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 2.7;
                    x2 = 3.0;
                }
                else
                {
                    x1 = 2.2;
                    x2 = 3.3;
                }

            }
            else if (factor <= 0.5)
            {
                if (ksi >= 4.0)
                {
                    x1 = 12.5;
                    x2 = 2.4;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 5.4;
                    x2 = 2.4;
                }

                else if (ksi >= 1.5)
                {
                    x1 = 3.7;
                    x2 = 2.4;
                }

                else if (ksi >= 1.0)
                {
                    x1 = 2.1;
                    x2 = 2.4;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 1.6;
                    x2 = 2.4;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 1.0;
                    x2 = 2.3;
                }
                else
                {
                    x1 = 0.76;
                    x2 = 2.1;
                }

            }
            else if (factor <= 1.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 6.6;
                    x2 = 3.7;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 3.0;
                    x2 = 3.3;
                }

                else if (ksi >= 1.5)
                {
                    x1 = 2.1;
                    x2 = 2.8;
                }

                else if (ksi >= 1.0)
                {
                    x1 = 1.2;
                    x2 = 2.5;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.82;
                    x2 = 2.2;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.50;
                    x2 = 1.60;
                }
                else
                {
                    x1 = 0.33;
                    x2 = 1.2;
                }

            }
            else if (factor <= 2.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 3.40;
                    x2 = 5.90;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 1.65;
                    x2 = 4.40;
                }

                else if (ksi >= 1.5)
                {
                    x1 = 1.15;
                    x2 = 3.80;
                }

                else if (ksi >= 1.0)
                {
                    x1 = 0.70;
                    x2 = 2.80;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.52;
                    x2 = 2.2;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.34;
                    x2 = 1.60;
                }
                else
                {
                    x1 = 0.23;
                    x2 = 1.2;
                }

            }
            else if (factor <= 5.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 1.50;
                    x2 = 9.6;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 0.80;
                    x2 = 6.3;
                }

                else if (ksi >= 1.5)
                {
                    x1 = 0.62;
                    x2 = 5.0;
                }

                else if (ksi >= 1.0)
                {
                    x1 = 0.46;
                    x2 = 3.80;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.38;
                    x2 = 3.3;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.34;
                    x2 = 2.9;
                }
                else
                {
                    x1 = 0.32;
                    x2 = 2.6;
                }

            }
            else
            {
                if (ksi >= 4.0)
                {
                    x1 = 0.90;
                    x2 = 8.3;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 0.52;
                    x2 = 8.3;
                }

                else if (ksi >= 1.5)
                {
                    x1 = 0.46;
                    x2 = 7.1;
                }

                else if (ksi >= 1.0)
                {
                    x1 = 0.39;
                    x2 = 5.9;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.38;
                    x2 = 5.6;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.35;
                    x2 = 5.3;
                }
                else
                {
                    x1 = 0.34;
                    x2 = 5.0;
                }

            }

            Double Kp = x1 / Kob;
            Double Ti = x2 * Tob;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public IAELopezMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum IAE - Lopez 1968",false)
        {
        }
    }

    public class IAEShinskeyMethodPI : SOSPDTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;
            
            Double[] timeConstants = CalculateDenominatorConstants(plantObject);

            Double T1 = timeConstants.Max();
            Double T2 = timeConstants.Min();

            Double factor = T2/T1;

            Double Kp = 0;
            Double Ti = 0;

            if(factor <= 0.1)
            {
                Kp = 0.77 * T1 / (Kob * T0);
                Ti = 2.83 * (T0 + T2);
            }   
            else if(factor <= 0.2)
            {
                Kp = 0.70 * T1 / (Kob * T0);
                Ti = 2.65 * (T0 + T2);
            }
            else if (factor <= 0.5)
            {
                Kp = 0.80 * T1 / (Kob * T0);
                Ti = 2.29 * (T0 + T2);
            }
            else
            {
                Kp = 0.80 * T1 / (Kob * T0);
                Ti = 1.67 * (T0 + T2);
            }

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public IAEShinskeyMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum IAE - Shinskey 1996", true)
        {
        }
    }

    public class ISELopezMethodPI : SOSPDTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double x1 = 0;
            Double x2 = 0;

            Double factor = T0 / Tob;

            if (factor <= 0.1)
            {
                if(ksi >= 4.0)
                {
                    x1 = 28;
                    x2 = 1.6;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 28;
                    x2 = 1.6;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 20.5;
                    x2 = 2.1;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 13;
                    x2 = 2.9;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 10.5;
                    x2 = 3.4;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 7.7;
                    x2 = 4.2;
                }
                else
                {
                    x1 = 6.5;
                    x2 = 4.8;
                }
            }
            else if (factor <= 0.2)
            {
                if (ksi >= 4.0)
                {
                    x1 = 33;
                    x2 = 1.4;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 15;
                    x2 = 2.0;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 11;
                    x2 = 2.4;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 7;
                    x2 = 2.9;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 5.4;
                    x2 = 3.3;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 3.9;
                    x2 = 3.8;
                }
                else
                {
                    x1 = 3.1;
                    x2 = 4.2;
                }

            }
            else if (factor <= 0.5)
            {
                if (ksi >= 4.0)
                {
                    x1 = 14.5;
                    x2 = 2.5;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 6.7;
                    x2 = 2.7;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 4.8;
                    x2 = 2.9;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 2.9;
                    x2 = 2.9;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 2.2;
                    x2 = 2.9;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 1.55;
                    x2 = 2.9;
                }
                else
                {
                    x1 = 1.25;
                    x2 = 2.9;
                }
                
            }
            else if (factor <= 1.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 8;
                    x2 = 4.2;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 3.8;
                    x2 = 3.8;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 2.7;
                    x2 = 3.7;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 1.65;
                    x2 = 3.1;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 1.2;
                    x2 = 2.9;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.83;
                    x2 = 2.5;
                }
                else
                {
                    x1 = 0.63;
                    x2 = 2.2;
                }

            }
            else if (factor <= 2.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 4.5;
                    x2 = 6.9;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 2.3;
                    x2 = 5.9;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 1.6;
                    x2 = 4.5;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 1.0;
                    x2 = 3.6;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.75;
                    x2 = 3.0;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.54;
                    x2 = 2.40;
                }
                else
                {
                    x1 = 0.42;
                    x2 = 1.9;

                }

            }
            else if (factor <= 5.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 2.0;
                    x2 = 11.6;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 1.05;
                    x2 = 7.7;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 0.85;
                    x2 = 6.7;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 0.65;
                    x2 = 5.3;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.56;
                    x2 = 4.4;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.48;
                    x2 = 3.8;
                }
                else
                {
                    x1 = 0.45;
                    x2 = 3.5;
                }

            }
            else
            {
                if (ksi >= 4.0)
                {
                    x1 = 1.15;
                    x2 = 16.1;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 0.75;
                    x2 = 10.6;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 0.63;
                    x2 = 9.1;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 0.56;
                    x2 = 8.0;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.53;
                    x2 = 7.4;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.48;
                    x2 = 6.7;
                }
                else
                {
                    x1 = 0.46;
                    x2 = 6.3;
                }

            }

            Double Kp = x1 / Kob;
            Double Ti = x2 * Tob;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ISELopezMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ISE - Lopez 1968", false)
        {
        }
    }

    public class ITAELopezMethodPI : SOSPDTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double x1 = 0;
            Double x2 = 0;

            Double factor = T0 / Tob;

            if (factor <= 0.1)
            {
                if (ksi >= 4.0)
                {
                    x1 = 16.5;
                    x2 = 1.2;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 16.5;
                    x2 = 1.2;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 11.5;
                    x2 = 1.5;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 7.4;
                    x2 = 2.0;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 5.5;
                    x2 = 2.3;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 3.8;
                    x2 = 2.7;
                }
                else
                {
                    x1 = 2.9;
                    x2 = 3.0;
                }
            }
            else if (factor <= 0.2)
            {
                if (ksi >= 4.0)
                {
                    x1 = 23;
                    x2 = 1.1;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 9.5;
                    x2 = 1.5;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 6.5;
                    x2 = 1.7;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 3.8;
                    x2 = 2.0;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 2.8;
                    x2 = 2.2;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 1.9;
                    x2 = 2.4;
                }
                else
                {
                    x1 = 1.35;
                    x2 = 2.4;
                }

            }
            else if (factor <= 0.5)
            {
                if (ksi >= 4.0)
                {
                    x1 = 10.5;
                    x2 = 2.0;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 4.5;
                    x2 = 2.1;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 3.0;
                    x2 = 2.1;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 1.7;
                    x2 = 2.0;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 1.15;
                    x2 = 1.9;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.65;
                    x2 = 1.6;
                }
                else
                {
                    x1 = 0.42;
                    x2 = 1.3;
                }

            }
            else if (factor <= 1.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 5.6;
                    x2 = 3.4;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 2.5;
                    x2 = 2.9;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 1.7;
                    x2 = 2.7;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 0.93;
                    x2 = 2.1;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.64;
                    x2 = 1.8;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.34;
                    x2 = 1.2;
                }
                else
                {
                    x1 = 0.21;
                    x2 = 0.8;
                }

            }
            else if (factor <= 2.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 3.2;
                    x2 = 5.4;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 1.4;
                    x2 = 4.0;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 0.98;
                    x2 = 3.3;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 0.58;
                    x2 = 2.4;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.42;
                    x2 = 1.9;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.26;
                    x2 = 1.3;
                }
                else
                {
                    x1 = 0.18;
                    x2 = 0.9;
                }

            }
            else if (factor <= 5.0)
            {
                if (ksi >= 4.0)
                {
                    x1 = 1.4;
                    x2 = 8.7;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 0.66;
                    x2 = 5.6;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 0.52;
                    x2 = 4.1;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 0.38;
                    x2 = 3.4;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.34;
                    x2 = 2.9;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.29;
                    x2 = 2.5;
                }
                else
                {
                    x1 = 0.25;
                    x2 = 2.3;
                }

            }
            else if (factor <= 10.0)
            {

                if (ksi >= 4.0)
                {
                    x1 = 0.46;
                    x2 = 7.4;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 0.46;
                    x2 = 7.4;
                }
                else if (ksi >= 1.5)
                {
                    x1 = 0.38;
                    x2 = 6.3;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 0.33;
                    x2 = 5.6;
                }
                else if (ksi >= 0.8)
                {
                    x1 = 0.32;
                    x2 = 4.9;
                }
                else if (ksi >= 0.6)
                {
                    x1 = 0.30;
                    x2 = 4.5;
                }
                else
                {
                    x1 = 0.28;
                    x2 = 4.3;
                }

            }

            Double Kp = x1 / Kob;
            Double Ti = x2 * Tob;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ITAELopezMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ITAE - Lopez 1968", false)
        {

        }
    }

    public class ITSEChaoMethodPI1 : SOSPDTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double factor = T0 / Tob;

            Double Kp = ((0.3366 + 0.6355 * ksi - 0.1066 * ksi * ksi) / Kob) * Math.Pow(T0 / (2 * ksi * Tob), (-1.0167 + 0.1743 * ksi - 0.03946 * ksi * ksi));
            Double Ti = 2 * ksi * Tob * (1.9255 - 0.3607 * ksi + 0.1299 * ksi * ksi) * Math.Pow(T0 / (2 * ksi * Tob), (-0.5848 + 0.7294 * ksi - 0.1136 * ksi * ksi));

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ITSEChaoMethodPI1()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ITSE - Chao 1989 1", false)
        {

        }
    }

    public class ISEKeviczkyMethodPI : SOSPDTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double x1 = 0;
            Double x2 = 0;

            Double factor = T0 / Tob;

            if (factor <= 0.1)
            {
                if(ksi>=10)
                {
                    x1 = 14;
                    x2 = 9;
                }
                else if (ksi >= 5.0)
                {
                    x1 = 14;
                    x2 = 9;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 14;
                    x2 = 9;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 6;
                    x2 = 6;
                }
                else if (ksi >= 0.5)
                {
                    x1 = 2.2;
                    x2 = 3.5;
                }
                else if (ksi >= 0.2)
                {
                    x1 = 0.55;
                    x2 = 2;
                }
                else
                {
                    x1 = 0.26;
                    x2 = 1.5;
                }

            }
            else if (factor <= 0.2)
            {
                if (ksi >= 10)
                {
                    x1 = 30;
                    x2 = 15;
                }
                else if (ksi >= 5.0)
                {
                    x1 = 30;
                    x2 = 15;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 9;
                    x2 = 8;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 3;
                    x2 = 4.5;
                }
                else if (ksi >= 0.5)
                {
                    x1 = 1.5;
                    x2 = 2.8;
                }
                else if (ksi >= 0.2)
                {
                    x1 = 0.4;
                    x2 = 1.5;
                }
                else
                {
                    x1 = 0.21;
                    x2 = 1.3;
                }

            }
            else if (factor <= 0.5)
            {
                if (ksi >= 10)
                {
                    x1 = 30;
                    x2 = 30;
                }
                else if (ksi >= 5.0)
                {
                    x1 = 13;
                    x2 = 14;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 5;
                    x2 = 7;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 2;
                    x2 = 3.5;
                }
                else if (ksi >= 0.5)
                {
                    x1 = 0.8;
                    x2 = 2;
                }
                else if (ksi >= 0.2)
                {
                    x1 = 0.25;
                    x2 = 1.3;
                }
                else
                {
                    x1 = 0.15;
                    x2 = 1.2;
                }


            }
            else if (factor <= 1.0)
            {

                if (ksi >= 10)
                {
                    x1 = 14;
                    x2 = 30;
                }
                else if (ksi >= 5.0)
                {
                    x1 = 7;
                    x2 = 14;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 3;
                    x2 = 6.5;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 1.2;
                    x2 = 3.3;
                }
                else if (ksi >= 0.5)
                {
                    x1 = 0.65;
                    x2 = 1.8;
                }
                else if (ksi >= 0.2)
                {
                    x1 = 0.2;
                    x2 = 1.2;
                }
                else
                {
                    x1 = 0.12;
                    x2 = 1.1;
                }


            }
            else if (factor <= 2.0)
            {
                if (ksi >= 10)
                {
                    x1 = 8;
                    x2 = 30;
                }
                else if (ksi >= 5.0)
                {
                    x1 = 4;
                    x2 = 15;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 1.8;
                    x2 = 7;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 0.9;
                    x2 = 3.5;
                }
                else if (ksi >= 0.5)
                {
                    x1 = 0.4;
                    x2 = 1.9;
                }
                else if (ksi >= 0.2)
                {
                    x1 = 0.16;
                    x2 = 1.2;
                }
                else
                {
                    x1 = 0.1;
                    x2 = 1.0;
                }

            }
            else if (factor <= 5.0)
            {
                if (ksi >= 10)
                {
                    x1 = 3.4;
                    x2 = 32;
                }
                else if (ksi >= 5.0)
                {
                    x1 = 1.9;
                    x2 = 17;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 1.0;
                    x2 = 8;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 0.65;
                    x2 = 5.0;
                }
                else if (ksi >= 0.5)
                {
                    x1 = 0.4;
                    x2 = 3.0;
                }
                else if (ksi >= 0.2)
                {
                    x1 = 0.16;
                    x2 = 1.4;
                }
                else
                {
                    x1 = 0.1;
                    x2 = 1.2;
                }


            }
            else if (factor <= 10.0)
            {
                if (ksi >= 10)
                {
                    x1 = 2.0;
                    x2 = 34;
                }
                else if (ksi >= 5.0)
                {
                    x1 = 1.1;
                    x2 = 19;
                }
                else if (ksi >= 2.0)
                {
                    x1 = 0.7;
                    x2 = 10;
                }
                else if (ksi >= 1.0)
                {
                    x1 = 0.6;
                    x2 = 7.5;
                }
                else if (ksi >= 0.5)
                {
                    x1 = 0.45;
                    x2 = 5.5;
                }
                else if (ksi >= 0.2)
                {
                    x1 = 0.2;
                    x2 = 2.0;
                }
                else
                {
                    x1 = 0.15;
                    x2 = 1.6;
                }

            }

            Double Kp = x1;
            Double Ti = x2 * T0;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ISEKeviczkyMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ISE - Keviczky 1973", false)
        {

        }
    }

    public class ITAEChaoMethodPI : SOSPDTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double x1 = 0;
            Double x2 = 0;

            Double factor = T0 / Tob;

            Double Kp = ((0.2763 + 0.3532 * ksi - 0.06955 * ksi * ksi) / Kob) * Math.Pow(T0 / (2 * ksi * Tob), (-0.3772 - 0.2414 * ksi + 0.03342 * ksi * ksi));
            Double Ti = 2 * ksi * Tob * (0.9953 + 0.07857 * ksi + 0.005317 * ksi * ksi) * Math.Pow(T0 / (2 * ksi * Tob), (-0.3276 + 0.3226 * ksi -0.05929 * ksi * ksi));

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ITAEChaoMethodPI()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ITAE - Chao 1989", false)
        {

        }
    }

    public class ITSEChaoMethodPI2 : SOSPDTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double x1 = 0;
            Double x2 = 0;

            Double factor = T0 / Tob;

            Double Kp = ((0.3970 + 0.4376 * ksi - 0.08168 * ksi * ksi) / Kob) * Math.Pow(T0 / (2 * ksi * Tob), (-0.5629 - 0.1187 * ksi + 0.01328 * ksi * ksi));
            Double Ti = 2 * ksi * Tob * (1.3414 - 0.1487 * ksi + 0.07685 * ksi * ksi) * Math.Pow(T0 / (2 * ksi * Tob), (-0.4287 + 0.1981 * ksi - 0.01978 * ksi * ksi));

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ITSEChaoMethodPI2()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Minimum ITSE - Chao 1989 2", false)
        {

        }
    }

    public class SomaniMethodPI : SOSPDTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Gain margin = 1.5",
            "Gain margin = 2.0",
            "Gain margin = 3.0",
            "Gain margin = 4.0",
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
            Double T0 = plantObject.TimeDelay;

            Double[] timeConstants = CalculateDenominatorConstants(plantObject);

            Double T1 = timeConstants.Max();
            Double T2 = timeConstants.Min();

            Double x1 = 0;
            Double x2 = 0;

            Double factor1 = T0 / T2;
            Double factor2 = T0 / T1;

            if (factor1 <= 0.1)
            {
                if (factor2 <= 0.147)
                {
                    x1 = 11.707;
                    x2 = 12.5;
                }
                else if (factor2 <= 0.440)
                {
                    x1 = 10.091;
                    x2 = 8.33;
                }
                else if (factor2 <= 0.954)
                {
                    x1 = 10.315;
                    x2 = 6.25;
                }
                else if (factor2 <= 1.953)
                {
                    x1 = 11.070;
                    x2 = 5.00;
                }
                else if (factor2 <= 4.587)
                {
                    x1 = 12.207;
                    x2 = 4.17;
                }
                else
                {
                    x1 = 13.337;
                    x2 = 3.70;
                }
            }
            else if (factor1 <= 0.125)
            {
                if (factor2 <= 0.121)
                {
                    x1 = 11.318;
                    x2 = 12.5;
                }
                else if (factor2 <= 0.404)
                {
                    x1 = 8.613;
                    x2 = 8.33;
                }
                else if (factor2 <= 0.897)
                {
                    x1 = 8.510;
                    x2 = 6.25;
                }
                else if (factor2 <= 1.842)
                {
                    x1 = 8.997;
                    x2 = 5.00;
                }
                else if (factor2 <= 4.219)
                {
                    x1 = 9.840;
                    x2 = 4.17;
                }
                else
                {
                    x1 = 10.707;
                    x2 = 3.70;
                }
            }
            else if (factor1 <= 0.25)
            {
                if (factor2 <= 0.064)
                {
                    x1 = 14.494;
                    x2 = 11.11;
                }
                else if (factor2 <= 0.256)
                {
                    x1 = 6.504;
                    x2 = 8.33;
                }
                else if (factor2 <= 0.667)
                {
                    x1 = 5.136;
                    x2 = 6.25;
                }
                else if (factor2 <= 1.406)
                {
                    x1 = 4.961;
                    x2 = 5.00;
                }
                else if (factor2 <= 3.021)
                {
                    x1 = 5.174;
                    x2 = 4.17;
                }
                else
                {
                    x1 = 5.502;
                    x2 = 3.70;
                }
            }
            else if (factor1 <= 0.5)
            {
                if (factor2 <= 0.0642)
                {
                    x1 = 14.918;
                    x2 = 8.33;
                }
                else if (factor2 <= 0.376)
                {
                    x1 = 4.353;
                    x2 = 6.25;
                }
                else if (factor2 <= 0.909)
                {
                    x1 = 3.274;
                    x2 = 5.00;
                }
                else if (factor2 <= 1.880)
                {
                    x1 = 3.025;
                    x2 = 4.17;
                }
                else
                {
                    x1 = 3.038;
                    x2 = 3.70;
                }
            }
            else if (factor1 <= 1.00)
            {
                if (factor2 <= 0.082)
                {
                    x1 = 12.389;
                    x2 = 6.25;
                }
                else if (factor2 <= 0.437)
                {
                    x1 = 3.462;
                    x2 = 5.00;
                }
                else if (factor2 <= 1.016)
                {
                    x1 = 2.370;
                    x2 = 4.17;
                }
                else
                {
                    x1 = 2.087;
                    x2 = 3.57;
                }
            }
            else if (factor1 <= 1.67)
            {
                if (factor2 <= 0.169)
                {
                    x1 = 6.871;
                    x2 = 5.00;
                }
                else if (factor2 <= 0.581)
                {
                    x1 = 2.773;
                    x2 = 4.17;
                }
                else if (factor2 <= 1.242)
                {
                    x1 = 1.930;
                    x2 = 3.57;
                }
                else if (factor2 <= 2.445)
                {
                    x1 = 1.624;
                    x2 = 3.13;
                }
                else
                {
                    x1 = 1.957;
                    x2 = 3.13;
                }
            }
            else if (factor1 <= 2.5)
            {
                if (factor2 <= 0.156)
                {
                    x1 = 0.156;
                    x2 = 7.607;
                }
                else if (factor2 <= 0.338)
                {
                    x1 = 0.338;
                    x2 = 4.015;
                }
                else if (factor2 <= 0.559)
                {
                    x1 = 0.559;
                    x2 = 2.797;
                }
                else
                {
                    x1 = 1.182;
                    x2 = 1.848;
                }
            }
            else if (factor1 <= 5.0)
            {
                if (factor2 <= 0.075)
                {
                    x1 = 16.187;
                    x2 = 4.17;
                }
                else if (factor2 <= 0.238)
                {
                    x1 = 5.623;
                    x2 = 3.85;
                }
                else if (factor2 <= 0.433)
                {
                    x1 = 3.448;
                    x2 = 3.57;
                }
                else if (factor2 <= 0.667)
                {
                    x1 = 2.52;
                    x2 = 3.33;
                }
                else
                {
                    x1 = 2.014;
                    x2 = 3.13;
                }
            }
            else
            {
                if (factor2 <= 0.073)
                {
                    x1 = 17.650;
                    x2 = 3.85;
                }
                else if (factor2 <= 0.235)
                {
                    x1 = 5.996;
                    x2 = 3.57;
                }
                else if (factor2 <= 0.424)
                {
                    x1 = 4.524;
                    x2 = 3.33;
                }
                else if (factor2 <= 0.649)
                {
                    x1 = 2.641;
                    x2 = 3.13;
                }
                else
                {
                    x1 = 2.093;
                    x2 = 2.94;
                }
            }

            Double Am = 0;

            if(SelectedIndex == 0)
            {
                Am = 1.5;
            }
            else if(SelectedIndex == 1)
            {
                Am = 2.0;
            }
            else if (SelectedIndex == 2)
            {
                Am = 3.0;
            }
            else if (SelectedIndex == 3)
            {
                Am = 4.0;
            }

            Double Tob = CalculateTob(plantObject);
            Double Ksi = CalculateKsi(plantObject);

            Double Kp = x1/(Kob*Am);
            Double Ti = x2 * T0;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public SomaniMethodPI()
            : base(TunningType.FrequencyDomain, PIDModeType.PI, "Somani 1991", true)
        {

        }
    }

    public class SchaedelMethodPI : SOSPDTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Normal design",
            "Sharp design"
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

            Double Ti = 0;
            Double Kp = 0;

            if (SelectedIndex == 0)
            {
                Ti = Math.Sqrt(Math.Pow((2 * ksi * Tob + T0), 2) - 2 * (Tob * Tob + 2 * ksi * Tob * T0 + 0.5 * T0 * T0));
                Kp = 0.5 * Ti / (Kob * (2 * Tob * ksi + T0 - Ti));
            }
            else if (SelectedIndex == 1)
            {
                Kp = (0.375 / Kob) * (4 * ksi * ksi * Tob * Tob + 2 * ksi * Tob * T0 + 0.5 * T0 * T0 - Tob * Tob) / (Tob * Tob + 2 * ksi * Tob * T0 + 0.5 * T0 * T0);
                Ti = (4 * ksi * ksi * Tob * Tob + 2 * ksi * T0 * Tob + 0.5 * T0 * T0 - Tob * Tob) / (2 * ksi * Tob + T0);
            }


            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public SchaedelMethodPI()
            : base(TunningType.FrequencyDomain, PIDModeType.PI, "Schaedel 1997", false)
        {

        }
    }

}
