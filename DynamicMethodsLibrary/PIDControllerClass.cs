using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferFunctionLib;

namespace DynamicMethodsLibrary
{
    public enum PIDModeType
    {
        P,PI,PD,PID
    }

    public class PIDControllerClass
    {
        public PIDModeType ModeOfAlgorithm
        {
            get;
            private set;
        }

        public Double Kp
        {
            get;
            private set;
        }

        public Double Ti
        {
            get;
            private set;
        }

        public Double Td
        {
            get;
            private set;
        }

        public Double N
        {
            get;
            private set;
        }

        public Double SampleTime
        {
            get;
            private set;
        }

        public PIDControllerClass(Double kp, Double ti, Double td, Double n, PIDModeType mode, Double sampleTime )
        {
            Kp = kp;
            Ti = ti;
            Td = td;
            N = n;
            ModeOfAlgorithm = mode;
            SampleTime = sampleTime;
        }

        internal PIDController ToDSPIDController()
        {
            switch(ModeOfAlgorithm)
            {
                case PIDModeType.P:
                    {
                        return new PIDController(Kp, Ti, Td, N, SampleTime, TransferFunctionLib.PIDModeType.P, false);
                        break;
                    }
                case PIDModeType.PI:
                    {
                        return new PIDController(Kp, Ti, Td, N, SampleTime, TransferFunctionLib.PIDModeType.PI, false);
                        break;
                    }
                case PIDModeType.PD:
                    {
                        return new PIDController(Kp, Ti, Td, N, SampleTime, TransferFunctionLib.PIDModeType.PD, false);
                        break;
                    }
                case PIDModeType.PID:
                    {
                        return new PIDController(Kp, Ti, Td, N, SampleTime, TransferFunctionLib.PIDModeType.PID, false);
                        break;
                    }
            }

            return null; 
        }

    }
}
