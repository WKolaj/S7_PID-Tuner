using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferFunctionLib;


namespace DynamicMethodsLibrary
{
    public abstract class TuningMethod
    {
        private TransferFunctionClass transferFunction;
        public TransferFunctionClass TransferFunction
        {
            get
            {
                return transferFunction;
            }

            protected set
            {
                transferFunction = value;
            }
        }

        public abstract PIDControllerClass Tuning();

        internal PIDController StartTuning(TransferFunctionClass transferFunction)
        {
            TransferFunction = transferFunction;

            PIDControllerClass controller = Tuning();

            if (controller != null)
            {
                return controller.ToDSPIDController();
            }
            else
            {
                return null;
            }
        }

        public static Int32 LengthOf(Double[] vector)
        {
            for(int i=vector.Length - 1; i>=0; i--)
            {
                if(vector[i]!=0)
                {
                    return i + 1;
                }
            }

            return 0;
        }
    }
}
