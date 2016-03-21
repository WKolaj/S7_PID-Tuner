using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SOSPDZeroModelTunningMethod
{
    public class ChienMethodPI : SOSPDZeroTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);

            Double[] constants = (from constant in CalculateDenominatorConstants(plantObject)
                                 select Math.Abs(constant)).ToArray();

            Double T1 = constants.Max();
            Double T2 = constants.Min();
            Double T3 = Math.Abs(CalculateT3(plantObject));
            Double T0 = plantObject.TimeDelay;

            Double x1 = 0.707 * T2 + 0.5 * Math.Sqrt(4*(T3*T0+T2*T3+T2*T0)+2*T2*T2);
            Double Kp = T1/(Kob*(1.414*x1 + T3+T0));
            Double Ti = T1;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ChienMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Chien 2003",true)
        {
        }
    }

    public class PomerleauMethodPI : SOSPDZeroTunningMethodBase
    {
        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = CalculateKob(plantObject);
            Double Tob = CalculateTob(plantObject);
            Double ksi = CalculateKsi(plantObject);

            Double[] constants = (from constant in CalculateDenominatorConstants(plantObject)
                                 select Math.Abs(constant)).ToArray();

            Double T1 = constants.Max();
            Double T2 = constants.Min();
            Double T3 = Math.Abs(CalculateT3(plantObject));
            Double T0 = plantObject.TimeDelay;

            Double Kp = (1 / Kob) * (T1) / (T1 + T3 + T0);
            Double Ti = 1.5*T1;

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public PomerleauMethodPI()
            : base(TunningType.TimeDomain, PIDModeType.PI, "Pomerleau-Poulin 2004", true)
        {
        }
    }

    public class MarchettiMethodPI : SOSPDZeroTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Robustness = 1",
            "Robustness = 3",
            "Robustness = 5",
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

            Double lambda = 0;

            if(SelectedIndex==0)
            {
                lambda = 1;
            }
            else if( SelectedIndex == 1)
            {
                lambda = 3;
            }
            else if (SelectedIndex == 2)
            {
                lambda = 5;
            }
            else if (SelectedIndex == 3)
            {
                lambda = 10;
            }

            if(T3 >= 0)
            {
                Kp = (2 * ksi * Tob + 0.5 * T0) / (Kob * (2*lambda + T0));
                Ti = 2 * ksi * Tob + 0.5 * T0;
            }
            else
            {
                Kp = (2 * ksi * Tob + 0.5 * T0) / (Kob * (2 * lambda + T0 - 2 * T3));
                Ti = 2 * ksi * Tob + 0.5 * T0;
            }

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public MarchettiMethodPI()
            : base(TunningType.Robust, PIDModeType.PI, "Marchetti-Scali 2000", false)
        {
        }
    }

}
