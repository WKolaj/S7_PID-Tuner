using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StrejcModelTunningMethod
{
    public class DavydovPIDTunningMethod : StrejcModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateDenominatorConstant(plantObject);
            Double T0 = plantObject.TimeDelay;
            Double n = CalculateRank(plantObject);

            Double factor1 = 0;

            for(int i=1; i<=n;i++)
            {
                factor1 += 1 / (Math.Pow(n - 1, n - i) * Factorial(i - 1));
            }

            factor1 *= Factorial(Convert.ToInt32(n - 1));

            factor1 += (n-1) - Factorial(Convert.ToInt32(n-1))/(Math.Pow(n-1,n-1)*Math.Exp(-(n-1)));

            Double timeConst = T0 + Tob * factor1;

            Double factor = timeConst / (n * Tob + T0);

            Double Kp = 0;
            Double Ti = 0;
            Double Td  = 0;
            Double N  = 0;

            if(factor > 0.5)
            {
                Kp = 1 / (Kob * ((3.540 * (timeConst) / (n * Tob + T0)) - 0.718));
                Ti = (n * Tob + T0) * (0.911 - 0.716 * ((timeConst) / (n * Tob + T0)));
                Td = 0.25 * Ti;
                N = 1 / Kp;
            }
            else
            {
                Kp = 1 / (Kob * ((2.766 * (timeConst) / (n * Tob + T0)) - 0.521));
                Ti = (n * Tob + T0) * (0.559 - 0.716 * ((1.50*timeConst) / (n * Tob + T0)));
                Td = 0.25 * Ti;
                N = 1 / Kp;
            }


            return new PIDControllerClass(Kp, Ti, Td, N, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public DavydovPIDTunningMethod()
            : base(TunningType.TimeDomain, PIDModeType.PID, "Davydov 1995")
        {
        }


        /// <summary>
        /// Silnia
        /// </summary>
        /// <param name="n">
        /// Argument silni
        /// </param>
        /// <returns>
        /// Wartosc silni
        /// </returns>
        private static Double Factorial(int n)
        {
            int result = 1;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        
    }

    public class LarionescuPIDTunningMethod : StrejcModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Robustness = 1",
            "Robustness = 2",
            "Robustness = 3",
            "Robustness = 4",
            "Robustness = 5"
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
            Double Tob = CalculateDenominatorConstant(plantObject);
            Double T0 = plantObject.TimeDelay;
            Double n = CalculateRank(plantObject);

           
            Double Kp = 0;
            Double Ti = 0;
            Double Td = 0;
            Double N = 5;

            Double lambda = 0;

            lambda = n*Tob*(SelectedIndex+1);

            Kp = n * Tob / (Kob * (lambda + T0));
            Ti = n * Tob;
            Td = 0.5*(n-1)*Tob;

            return new PIDControllerClass(Kp, Ti, Td, N, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public LarionescuPIDTunningMethod()
            : base(TunningType.Robust, PIDModeType.PID, "Larionescu 2002")
        {
        }


        /// <summary>
        /// Silnia
        /// </summary>
        /// <param name="n">
        /// Argument silni
        /// </param>
        /// <returns>
        /// Wartosc silni
        /// </returns>
        private static Double Factorial(int n)
        {
            int result = 1;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }


    }

    
}
