using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOSPDModelTunningMethods
{
    public class SchaedelMethodPID : TOSPDModelTunningMethodBase
    {

        private String[] typeOfProcess = new String[]
        {
            "N = 0.01",
            "N = 0.02",
            "N = 0.03",
            "N = 0.04",
            "N = 0.05",
            "N = 0.06",
            "N = 0.07",
            "N = 0.08",
            "N = 0.09",
            "N = 0.1",
            "N = 0.2",
            "N = 0.3",
            "N = 0.4",
            "N = 0.5",
            "N = 0.6",
            "N = 0.7",
            "N = 0.8",
            "N = 0.9",
            "N = 1.0"
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

            Double a1 = plantObject.Denominator[1] / plantObject.Denominator[0];
            Double a2 = plantObject.Denominator[2] / plantObject.Denominator[0];
            Double a3 = plantObject.Denominator[3] / plantObject.Denominator[0];

            Double N = 0;

            if (SelectedIndex == 0)
            {
                N = 0.01;
            }
            else if (SelectedIndex == 1)
            {
                N = 0.02;
            }
            else if (SelectedIndex == 2)
            {
                N = 0.03;
            }
            else if (SelectedIndex == 3)
            {
                N = 0.04;
            }
            else if (SelectedIndex == 4)
            {
                N = 0.05;
            }
            else if (SelectedIndex == 5)
            {
                N = 0.06;
            }
            else if (SelectedIndex == 6)
            {
                N = 0.07;
            }
            else if (SelectedIndex == 7)
            {
                N = 0.08;
            }
            else if (SelectedIndex == 8)
            {
                N = 0.09;
            }
            else if (SelectedIndex == 9)
            {
                N = 0.1;
            }
            else if (SelectedIndex == 10)
            {
                N = 0.2;
            }
            else if (SelectedIndex == 11)
            {
                N = 0.3;
            }
            else if (SelectedIndex == 12)
            {
                N = 0.4;
            }
            else if (SelectedIndex == 13)
            {
                N = 0.5;
            }
            else if (SelectedIndex == 14)
            {
                N = 0.6;
            }
            else if (SelectedIndex == 15)
            {
                N = 0.7;
            }
            else if (SelectedIndex == 16)
            {
                N = 0.8;
            }
            else if (SelectedIndex == 17)
            {
                N = 0.9;
            }
            else if (SelectedIndex == 18)
            {
                N = 1.0;
            }

            Double Td = (a2 + a1 * T0 + 0.5 * T0 * T0) / (a1 + T0) - (a3 + a2 * T0 + 0.5 * a1 * T0 * T0 + 0.167 * T0 * T0 * T0) / (a2 + a1 * T0 + 0.5 * T0 * T0);
            Double Ti = (a1 * a1 - a2 + a1 * T0 + 0.5 * T0 * T0) / (a1 + T0 - Td);
            Double Kp = 0.375 * Ti / (Kob * (a1 + T0 + Td * N - Ti));


            return new PIDControllerClass(Kp, Ti, Td, N, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public SchaedelMethodPID()
            : base(TunningType.FrequencyDomain, PIDModeType.PID, "Schaedel 1997", false)
        {
        }
    }

    public class MarchettiScaliPID : TOSPDModelTunningMethodBase
    {

        private String[] typeOfProcess = new String[]
        {
            "Robustness = 0.1",
            "Robustness = 0.2",
            "Robustness = 0.3",
            "Robustness = 0.4",
            "Robustness = 0.5",
            "Robustness = 0.6",
            "Robustness = 0.7",
            "Robustness = 0.8",
            "Robustness = 0.9",
            "Robustness = 1.0",
            "Robustness = 2.0",
            "Robustness = 3.0",
            "Robustness = 4.0",
            "Robustness = 5.0",
            "Robustness = 6.0",
            "Robustness = 7.0",
            "Robustness = 8.0",
            "Robustness = 9.0",
            "Robustness = 10.0"
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

            Double a1 = plantObject.Denominator[1] / plantObject.Denominator[0];
            Double a2 = plantObject.Denominator[2] / plantObject.Denominator[0];
            Double a3 = plantObject.Denominator[3] / plantObject.Denominator[0];

            Double lambda = 0;

            if (SelectedIndex == 0)
            {
                lambda = 0.1;
            }
            else if (SelectedIndex == 2)
            {
                lambda = 0.2;
            }
            else if (SelectedIndex == 3)
            {
                lambda = 0.3;
            }
            else if (SelectedIndex == 4)
            {
                lambda = 0.4;
            }
            else if (SelectedIndex == 5)
            {
                lambda = 0.5;
            }
            else if (SelectedIndex == 6)
            {
                lambda = 0.6;
            }
            else if (SelectedIndex == 7)
            {
                lambda = 0.7;
            }
            else if (SelectedIndex == 8)
            {
                lambda = 0.8;
            }
            else if (SelectedIndex == 9)
            {
                lambda = 0.9;
            }
            else if (SelectedIndex == 10)
            {
                lambda = 1.0;
            }
            else if (SelectedIndex == 11)
            {
                lambda = 2.0;
            }
            else if (SelectedIndex == 12)
            {
                lambda = 3.0;
            }
            else if (SelectedIndex == 13)
            {
                lambda = 4.0;
            }
            else if (SelectedIndex == 14)
            {
                lambda = 5.0;
            }
            else if (SelectedIndex == 15)
            {
                lambda = 6.0;
            }
            else if (SelectedIndex == 16)
            {
                lambda = 7.0;
            }
            else if (SelectedIndex == 17)
            {
                lambda = 8.0;
            }
            else if (SelectedIndex == 18)
            {
                lambda = 9.0;
            }
            else if (SelectedIndex == 19)
            {
                lambda = 10.0;
            }

            Double Td = (a2+0.5*a1*T0)/(a1+0.5*T0);
            Double Ti = a1 + 0.5 * T0;
            Double Kp = (a1 + 0.5 * T0) / ((3 * lambda + T0) * Kob);

            
            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public MarchettiScaliPID()
            : base(TunningType.Robust, PIDModeType.PID, "Marchetti and Scali 1997", false)
        {
        }
    }

    public class JonesThamPID : TOSPDModelTunningMethodBase
    {

        private String[] typeOfProcess = new String[]
        {
            "Robustness = 0.1",
            "Robustness = 0.2",
            "Robustness = 0.3",
            "Robustness = 0.4",
            "Robustness = 0.5",
            "Robustness = 0.6",
            "Robustness = 0.7",
            "Robustness = 0.8",
            "Robustness = 0.9",
            "Robustness = 1.0",
            "Robustness = 2.0",
            "Robustness = 3.0",
            "Robustness = 4.0",
            "Robustness = 5.0",
            "Robustness = 6.0",
            "Robustness = 7.0",
            "Robustness = 8.0",
            "Robustness = 9.0",
            "Robustness = 10.0"
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

            Double[] roots = (from root in CalculateDenominatorConstant(plantObject)
                              orderby root descending
                              select root).ToArray();

            Double T1 = roots[0];
            Double T2 = roots[1];
            Double T3 = roots[2];

            Double lambda = 0;

            if (SelectedIndex == 0)
            {
                lambda = 0.1;
            }
            else if (SelectedIndex == 2)
            {
                lambda = 0.2;
            }
            else if (SelectedIndex == 3)
            {
                lambda = 0.3;
            }
            else if (SelectedIndex == 4)
            {
                lambda = 0.4;
            }
            else if (SelectedIndex == 5)
            {
                lambda = 0.5;
            }
            else if (SelectedIndex == 6)
            {
                lambda = 0.6;
            }
            else if (SelectedIndex == 7)
            {
                lambda = 0.7;
            }
            else if (SelectedIndex == 8)
            {
                lambda = 0.8;
            }
            else if (SelectedIndex == 9)
            {
                lambda = 0.9;
            }
            else if (SelectedIndex == 10)
            {
                lambda = 1.0;
            }
            else if (SelectedIndex == 11)
            {
                lambda = 2.0;
            }
            else if (SelectedIndex == 12)
            {
                lambda = 3.0;
            }
            else if (SelectedIndex == 13)
            {
                lambda = 4.0;
            }
            else if (SelectedIndex == 14)
            {
                lambda = 5.0;
            }
            else if (SelectedIndex == 15)
            {
                lambda = 6.0;
            }
            else if (SelectedIndex == 16)
            {
                lambda = 7.0;
            }
            else if (SelectedIndex == 17)
            {
                lambda = 8.0;
            }
            else if (SelectedIndex == 18)
            {
                lambda = 9.0;
            }
            else if (SelectedIndex == 19)
            {
                lambda = 10.0;
            }

            Double Td = (T1*T2+T2*T3+T1*T3) / (T1 + T2 +T3);
            Double Ti = T1 + T2 + T3;
            Double Kp = (T1+T2+T3)/(Kob*(lambda + T0));


            return new PIDControllerClass(Kp, Ti, Td, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public JonesThamPID()
            : base(TunningType.Robust, PIDModeType.PID, "Jones and Tham 2006", true)
        {
        }
    }
}
