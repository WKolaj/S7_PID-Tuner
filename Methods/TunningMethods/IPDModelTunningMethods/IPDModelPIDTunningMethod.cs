using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPDModelTunningMethods
{

    public class ArbogastMethodPID : IPDModelTunningMethodBase
    {

        public override DynamicMethodsLibrary.PIDControllerClass TuningMethod(DynamicMethodsLibrary.TransferFunctionClass plantObject)
        {
            Double Kob = plantObject.Nominator[0] / plantObject.Denominator[1];
            Double T0 = plantObject.TimeDelay;

            Double Kp = 0;
            Double Ti = 0;
            Double Td = 0;
            Double N = 0;

            Kp = 0.447 / (T0 * Kob);
            Ti =7.528 * T0;
            Td = 0.189 * T0;
            N = 1 / 0.636;

            return new PIDControllerClass(Kp, Ti, Td, N, TypeOfAglorithm, plantObject.SampleTime / 1000);

        }

        public ArbogastMethodPID()
            : base(TunningType.RegulatorTuning, PIDModeType.PID, "Arbogast 2007")
        {

        }
    }


}
