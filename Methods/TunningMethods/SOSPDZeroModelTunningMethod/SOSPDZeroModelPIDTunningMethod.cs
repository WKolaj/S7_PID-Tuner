using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOSPDZeroModelTunningMethod
{
    public class Chien2003MethodPID : SOSPDZeroTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);

            Double T0 = plantObject.TimeDelay;
            Double T3 = CalculateT3(plantObject);
            Double Kp = 0;
            Double Ti = 0;
            Double Td = 0;
            double N = 0;

            if (T3 < 0)
            {
                Td = Tob * (10 * ksi - 3.02 * Math.Sqrt(11 * ksi * ksi - 1));
                Double x1 = 0.1 * Td + 0.5 * Math.Sqrt(-4 * T3 * T0 - 0.4 * Td * T3 + 0.4 * Td * T0 + 0.04 * Td * Td);
                Kp = Tob / (Kob * (2 * x1 - T3 +T0));
                Ti = 2 * ksi * Tob - 0.1 * Td;
                N = 0.1;
            }
            else
            {
                Kp = 0.829 * ksi * Tob / (Kob * T0);
                Ti = 2 * ksi * Tob - T3;
                Td = ((Tob * Tob) / (2 * ksi * Tob - T3)) -T3;
                N = 1/((Tob / (T3 * (2 * ksi * Tob - T3))) - 1);
            }
            

            return new PIDControllerClass(Kp, Ti, Td, N, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public Chien2003MethodPID()
            : base(TunningType.TimeDomain, PIDModeType.PID, "Chien 2003", false)
        {
        }
    }

    public class Chien1988MethodPID : SOSPDZeroTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Robustness = 1",
            "Robustness = 2",
            "Robustness = 3",
            "Robustness = 4",
            "Robustness = 5",
            "Robustness = 6",
            "Robustness = 7",
            "Robustness = 8",
            "Robustness = 9",
            "Robustness = 10",
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
            Double T3 = CalculateT3(plantObject);

            Double Kp = 0;
            Double Ti = 0;
            Double Td = 0;
            Double N = 0;

            Double lambda = 0;

            Double begin = Math.Min(Tob,T0);
            Double end = Math.Max(Tob,T0);

            if (SelectedIndex == 0)
            {
                lambda = begin + 1*((end-begin)/10);
            }
            else if (SelectedIndex == 1)
            {
                lambda = begin + 2*((end-begin)/10);
            }
            else if (SelectedIndex == 2)
            {
                lambda = begin + 3*((end-begin)/10);
            }
            else if (SelectedIndex == 3)
            {
                lambda = begin + 4*((end-begin)/10);
            }
            else if (SelectedIndex == 4)
            {
                lambda = begin + 5*((end-begin)/10);
            }
            else if (SelectedIndex == 5)
            {
                lambda = begin + 6*((end-begin)/10);
            }
            else if (SelectedIndex == 6)
            {
                lambda = begin + 7*((end-begin)/10);
            }
            else if (SelectedIndex == 7)
            {
                lambda = begin + 8*((end-begin)/10);
            }
            else if (SelectedIndex == 8)
            {
                lambda = begin + 9*((end-begin)/10);
            }
            else if (SelectedIndex == 9)
            {
                lambda = begin + 10*((end-begin)/10);
            }
            else if (SelectedIndex == 10)
            {
                lambda = begin + 1*((end-begin)/10);
            }

            if (T3 >= 0)
            {
                Kp = (2*ksi*Tob - T3)/(Kob*(lambda + T3));
                Ti = 2*ksi*Tob-T3;
                Td = (Tob*Tob-(2*ksi*Tob-T3)*T3)/(2*ksi*Tob - T3);
                N=0.1;
            }
            else
            {
                T3 = -T3;

                Kp = (2*ksi*Tob + (T3*T0)/(lambda + T3+T0))/(Kob*(lambda + T3 + T0));
                Ti = 2*ksi*Tob + ((T3*T0)/(lambda + T3+T0));
                Td = ((T3*T0)/(lambda + T3+T0)) + (Tob*Tob)/(2*ksi*Tob + ((T3*T0)/(lambda + T3+T0)));
                N=0.1;
            }

            return new PIDControllerClass(Kp, Ti, Td, N, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public Chien1988MethodPID()
            : base(TunningType.Robust, PIDModeType.PID, "Chien 1988", false)
        {
        }
    }

}
