using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOLPDModelTunningMethods
{
    public class VariableSPMethodP : FOLPFModelTunningMethodBase
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
            Double Kob = plantObject.Nominator[0]/plantObject.Denominator[0];
            Double T0 = plantObject.TimeDelay;
            Double Tob = plantObject.Denominator[1]/plantObject.Denominator[0];

            switch(SelectedIndex)
            {
                case 0:
                    {
                        return new PIDControllerClass((0.3 * Tob) / (Kob * T0), 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((0.7 * Tob) / (Kob * T0), 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }

        public VariableSPMethodP() : base (TunningType.TimeDomain,PIDModeType.P,"Step set point")
        {

        }
    }

    public class VariableDisturbanceMethodP : FOLPFModelTunningMethodBase
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
                        return new PIDControllerClass((0.3 * Tob) / (Kob * T0), 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }

                case 1:
                    {
                        return new PIDControllerClass((0.7 * Tob) / (Kob * T0), 9999, 0, 0.1, TypeOfAglorithm, plantObject.SampleTime / 1000);
                    }
            }

            return null;
        }

        public VariableDisturbanceMethodP()
            : base(TunningType.TimeDomain, PIDModeType.P, "Step disturbance")
        {

        }
    }
}
