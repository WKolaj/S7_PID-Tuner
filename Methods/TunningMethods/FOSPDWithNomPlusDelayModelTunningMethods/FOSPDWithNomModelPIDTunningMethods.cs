using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FOSPDWithNomPlusDelayModelTunningMethods
{
    public class Vrancic1999MethodPID : FOSPDWithNomModelPITunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);

            Double a5 = plantObject.Denominator[5] / plantObject.Denominator[0];
            Double a4 = plantObject.Denominator[4] / plantObject.Denominator[0];
            Double a3 = plantObject.Denominator[3] / plantObject.Denominator[0];
            Double a2 = plantObject.Denominator[2] / plantObject.Denominator[0];
            Double a1 = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double b5 = plantObject.Nominator[5] / plantObject.Nominator[0];
            Double b4 = plantObject.Nominator[4] / plantObject.Nominator[0];
            Double b3 = plantObject.Nominator[3] / plantObject.Nominator[0];
            Double b2 = plantObject.Nominator[2] / plantObject.Nominator[0];
            Double b1 = plantObject.Nominator[1] / plantObject.Nominator[0];

            Double T0 = plantObject.TimeDelay;

            Double x1 = 
                360*(a1*a1*a1*a1*b2 - a1*a1*a1*(a2*b1+a3+b1*b2+b3) +a1*a1*(a2*a2+a2*(b1*b1 - 2*b2) + 3*a3*b1 + 2*a4 +b1*b3 + b2*b2-b4) -a1*(a2*(2*a3-3*b3) + a3*(2*b1*b1 - b2) +3*a4*b1 + a5 - b1*b4 + 2*b2*b3 - b5 ) -a2*b1*b3+a3*a3 + a3*(b1*b2 - 2*b3) + a4*b1*b1 + a5*b1-b1*b5+b3*b3)
                -360*T0*(a1*a1*a1*a1*b1 - a1*a1*a1*(a2+b1*b1 + 2*b2) + 3*a1*a1*(a3+b1*b2) + a1*(3*a2*b2 - 3*a3*b1 -3*a4 - b1*b3 - 2*b2*b2 +2*b4 ) -a2*(b1*b2+b3) +a3*(b1*b1 - b2) + 2*a4*b1 +a5 - b1*b4 +2*b2*b3 -b5) 
                +180*T0*T0*(a1*a1*a1*a1 - 4*a1*a1*a1*b1 +3*a1*a1*(b1*b1+b2) +a1*(3*a2*b1 - 3*a3-5*b1*b2 +b3) -a2*(b1*b1+2*b2) +a3*b1 +2*a4 +b1*b3 +2*(b2*b2 -b4)) 
                +60*T0*T0*T0*(4*a1*a1*a1 - 9*a1*a1*b1 - a1*(3*a2-5*b1*b1 - 4*b2) +4*a2*b1 - a3 -5*b1*b2 +b3) 
                +15*T0*T0*T0*T0 *(9*a1*a1 - 14*a1*b1 - 4*a2 + 5*b1*b1 + 4*b2) 
                -42*T0*T0*T0*T0*T0 * (b1-a1) 
                +7*T0*T0*T0*T0*T0*T0 ;
            
            Double x2 = a1*a1*a1*a1*b3 - a1*a1*a1*(a2*b2+a1*b3+a4 +b1*b3)
                +a1*a1*(a2*a2*b1 + a2*(2*a3+b1*b2 - 3*b3) + a3*(b1*b1 + b2) + a4*b1 +a5 + b2*b3 - b5)
                -a1*(a2*a2*a2 + a2*a2*(b1*b1 - 2*b2) + a2*(2*a3*b1 - 2*b1*b3 + b2*b2 - b4)
                +2*a3*a3+a3*(2*b1*b2 - 3*b3) + a4*(b1*b1 + b2)+a5*b1 -b1*b5 + b3*b3)
                +a2*a2*a3 + a2*(a3*(b1*b1-2*b2) -a5-b1*b4+b5) +a3*a3*b1
                +a3*(a4-b1*b3+b2*b2-b4) + a4*(b1*b2-b3) +a5*b2 - b2*b5 + b3*b4;

            Double x3 = a1*a1*a1*a1*b2 - a1*a1*a1*(a2*b1 + a3 + b1*b2 + b3)
                +a1*a1*(a2*a2 + a2*(b1*b1 - 2*b2) + 3*a3*b1 +2*a4 +b1*b3 +b2*b2 - b4)
                -a1*(a2*(2*a3-3*b3) +a3*(2*b1*b1-b2) +3*a4*b1 + a5 -b1*b4 +2*b2*b3 - b5)
                -a2*b1*b3 +a3*a3+a3*(b1*b2 -2*b3) +a4*b1*b1+a5*b1 -b1*b5+b3*b3;

            Double x4 = a1*a1*a1*a1*b1 - a1*a1*a1 *(a2 +b1*b1 +2*b2)
                +3*a1*a1*(a3+b1*b2) + a1*(3*a2*b2 -3*a3*b1 -3*a4 -b1*b3 -2*b2*b2 +2*b4)
                -a2*(b1*b2+b3) +a3*(b1*b1-b2) +2*a4*b1 + a5 -b1*b4 +2*b2*b3 - b5;

            Double x5 = a1*a1*a1*a1 -4*a1*a1*a1*b1 +3*a1*a1*(b1*b1+b2) + a1*(3*a2*b1-3*a3-5*b1*b2+b3)
                -a2*(b1*b1 +2*b2) +a3*b1 +2*a4 +b1*b3 +2*(b2*b2-b4);

            Double x6 = 4*a1*a1*a1 -9*a1*a1*b1 -a1*(3*a2-5*b1*b1 -4*b2) +4*a2*b1 -a3 -5*b1*b2 + b3;

            Double x7 = 9*a1*a1 -14*a1*b1 -4*a2 +5*b1*b1 +4*b2;

            Double Td = -(360*x2 - T0*360*x3 +180*T0*T0*x4 -60*T0*T0*T0*x5 -15*T0*T0*T0*T0*x6 -3*T0*T0*T0*T0*T0*x7 +7*T0*T0*T0*T0*T0*T0*(b1-a1) - T0*T0*T0*T0*T0*T0*T0)/x1;

            Double y1 = T0*(a1*a1 - a1*b1 -a2+b2) +0.5*(a1-b1)*T0*T0 +0.167*T0*T0*T0;
            Double y2 = (a1-b1)*(a1-b1)*T0 + (a1-b1)*T0*T0 + 0.333*T0*T0*T0 -(a1-b1+T0)*(a1-b1+T0)*Td;

            Double Kp = (a1*a1*a1 -a1*a1*b1 +a1*b2 -2*a1*a2 +a2*b1 +a3 -b3 +y1)/(2*Kob*(-a1*a1*b1+a1*a2+a1*b1*b1-a3-b1*b2+b3+y2));

            Double Ti = (a1*a1*a1 -a1*a1*b1 +a1*b2 -2*a1*a2 +a2*b1 +a3 -b3 +y1)/(a1*a1-a1*b1-a2+b2+(a1-b1)*T0 +0.5*T0*T0 - (a1-b1+T0)*Td);

            Double N = 0.1;

            return new PIDControllerClass(Kp, Ti, Td, N, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public Vrancic1999MethodPID()
            : base(TunningType.FrequencyDomain, PIDModeType.PID, "Vrančić 1999")
        {
        }
    }

    public class Vrancic1996MethodPID : FOSPDWithNomModelPITunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
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

            Double a5 = plantObject.Denominator[5] / plantObject.Denominator[0];
            Double a4 = plantObject.Denominator[4] / plantObject.Denominator[0];
            Double a3 = plantObject.Denominator[3] / plantObject.Denominator[0];
            Double a2 = plantObject.Denominator[2] / plantObject.Denominator[0];
            Double a1 = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double b5 = plantObject.Nominator[5] / plantObject.Nominator[0];
            Double b4 = plantObject.Nominator[4] / plantObject.Nominator[0];
            Double b3 = plantObject.Nominator[3] / plantObject.Nominator[0];
            Double b2 = plantObject.Nominator[2] / plantObject.Nominator[0];
            Double b1 = plantObject.Nominator[1] / plantObject.Nominator[0];

            Double T0 = plantObject.TimeDelay;

            Double A1 = Kob*(a1 - b1 + T0);
            Double A2 = Kob*(b2 - a2 + A1 * a1 - b1 * T0 + 0.5 * T0 * T0);
            Double A3 = Kob*(a3-b3+A2*a1 -A1*a2 +b2*T0 -0.5*b1*T0*T0 +0.167*T0*T0*T0);
            Double A4 = Kob*(b4 - a4 + A3*a1 - A2*a2 +A1*a3 -b3*T0+0.5*b2*T0*T0 + 0.167*b1*T0*T0*T0 +0.042*T0*T0*T0*T0);
            Double A5 = Kob*(a5-b5+A4*a1-A3*a2+A2*a3 -A1*a4+b4*T0-0.5*b3*T0*T0) + Kob*(0.167*b2*T0*T0*T0 -0.042*b1*T0*T0*T0*T0+0.008*T0*T0*T0*T0*T0);
            Double N = Convert.ToDouble(SelectedIndex + 1)/10;

            Double Td = (-(A3*A3-A5*A1)+Math.Sqrt((A3*A3-A5*A1)*(A3*A3-A5*A1) - (4*N)*(A3*A2 -A5)*(A5*A2 - A4*A3)))/((2*N)*(A3*A2-A5));

            Double Ti = A3/(A2-Td*A1-(Td*Td*N));

            Double Kp = Ti/(2*(A1-Ti));
            return new PIDControllerClass(Kp, Ti, Td, N, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public Vrancic1996MethodPID()
            : base(TunningType.FrequencyDomain, PIDModeType.PID, "Vrančić 1996")
        {
        }
    }
}