using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TOSPDModelTunningMethods
{
    public class KuwatarMethodPI : TOSPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);

            Double[] roots = (from root in CalculateDenominatorConstant(plantObject)
                             orderby root descending
                             select root).ToArray();

            Double Tob = roots.Average();

            Double T0 = plantObject.TimeDelay;

            Double x1 = Math.Sqrt(Math.Pow(3*Tob+T0,2)-0.267*(6*Math.Pow(Tob,3)+18*Tob*Tob*T0+9*Tob*T0*T0+Math.Pow(T0,3)));

            Double Kp = ((0.4 / Kob) * (3 * Tob + T0) / (3 * Tob + T0 - x1)) - 0.5/Kob;
            Double Ti = 1.25 * x1 - 0.25 * (3*Tob + T0);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public KuwatarMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Kuwata 1987",true)
        {
        }
    }

    public class HougenMethodPI : TOSPDModelTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);

            Double[] roots = (from root in CalculateDenominatorConstant(plantObject)
                              orderby root descending
                              select root).ToArray();

            Double T1 = roots[0];
            Double T2 = roots[1];
            Double T3 = roots[2];

            Double T0 = plantObject.TimeDelay;

            Double factor = T0 / T1;

            Double Kp = 0;
            Double Ti = 0;

            if (factor > 0.04)
            {
                Kp = (0.7 / Kob) * Math.Pow(T1 / T0, 0.333);
                Ti = 1.5 * Math.Pow(T0, 0.08) * Math.Sqrt(T1 * (T2 + T3));
            }
            else
            {
                Kp = (1 / (2 * Kob)) * (0.7 * Math.Pow(T1 / T0, 0.333) + 0.8 * (T1 + T2 + T3) / Math.Pow(T1 * T2 * T3, 0.333));
                Ti = 1.5 * Math.Pow(T0, 0.08) * Math.Sqrt(T1 * (T2 + T3));
            }

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public HougenMethodPI()
            : base(TunningType.FrequencyDomain, PIDModeType.PI, "Hougen 1979", true)
        {
        }
    }

    public class MarchettiScaliMethodPI : TOSPDModelTunningMethodBase
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
            "Robustness = 10"
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


            Double lambda = SelectedIndex + 1;

            Double a1 = plantObject.Denominator[1] / plantObject.Denominator[0];

            Double Kp = (a1 + 0.5 * T0) / ((3 * lambda + T0) * Kob);
            Double Ti = a1 + 0.5*T0;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public MarchettiScaliMethodPI()
            : base(TunningType.Robust, PIDModeType.PI, "Marchetti-Scali 2000", false)
        {
        }
    }

  

}
