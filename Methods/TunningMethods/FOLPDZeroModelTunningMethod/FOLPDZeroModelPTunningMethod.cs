using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOLPDZeroModelTunningMethod
{
    public class SlattekeMethodP : FOLPDZeroModelTunningMethodBase
    {
        private String[] typeOfProcess = new String[]
        {
            "Closed loop sensitivity = 1.1",
            "Closed loop sensitivity = 1.2",
            "Closed loop sensitivity = 1.3",
            "Closed loop sensitivity = 1.4"
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
            Double Tob2 = CalculateTob2(plantObject);
            Double Tob1 = CalculateTob1(plantObject);
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0;
            Double Ti = 0;
            Double x1 = 0;
            Double x2 = 0;
            Double x3 = 0;
            Double x4 = 0;
            Double x5 = 0;
            Double x6 = 0;

            if(SelectedIndex == 0)
            {
                x1 = 0.09;
                x2 = 4;
                x3 = 6;
                x4 = 1;
                x5 = 1;
                x6 = 21;

            }
            else if (SelectedIndex == 1)
            {
                x1 = 0.16;
                x2 = 5;
                x3 = 23;
                x4 = 4;
                x5 = 9;
                x6 = 105;
            }
            else if (SelectedIndex == 2)
            {
                x1 = 0.23;
                x2 = 1;
                x3 = 100;
                x4 = 17;
                x5 = 11;
                x6 = 94;
            }
            else if (SelectedIndex == 3)
            {
                x1 = 0.28;
                x2 = 1;
                x3 = 88;
                x4 = 15;
                x5 = 12;
                x6 = 85;
            }

            Kp = x1 * (Tob1 + 0.333 * T0) / (Kob * Tob2 * T0);
            Ti = x2 * T0 * (x3 * Tob1 + x4 * T0) / (x5 * Tob1 + x6 * T0);

            return new PIDControllerClass(Kp, Ti, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public SlattekeMethodP()
            : base(TunningType.RegulatorTuning, PIDModeType.PI, "Slätteke 2006")
        {

        }
    }
}
