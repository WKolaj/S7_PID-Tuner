using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicMethodsLibrary
{
    public enum TransferFunctionType
    {
        Continous,Discrete
    }

    public class TransferFunctionClass
    {
        private Double[] nominator;
        public Double[] Nominator
        {
            get
            {
                return nominator;
            }

            private set
            {
                nominator = value;
            }
        }

        private Double[] denominator;
        public Double[] Denominator
        {
            get
            {
                return denominator;
            }

            private set
            {
                denominator = value;
            }
        }

        private Double timeDelay;
        public Double TimeDelay
        {
            get
            {
                return timeDelay;
            }

            private set
            {
                timeDelay = value;
            }
        }

        private Double sampleTime;
        public Double SampleTime
        {
            get
            {
                return sampleTime;
            }

            private set
            {
                sampleTime = value;
            }

        }

        private TransferFunctionType transferFunctionType;
        public TransferFunctionType TransferFunctionType
        {
            get
            {
                return transferFunctionType;
            }

            private set
            {
                transferFunctionType = value;
            }
        }

        public TransferFunctionClass(Double[] nominator, Double[] denominator, Double timeDelay, Int32 simulationSampleTime, TransferFunctionType type )
        {
            this.nominator = nominator;
            this.denominator = denominator;
            this.timeDelay = timeDelay;
            this.sampleTime = Convert.ToDouble(simulationSampleTime)/1000;
            this.transferFunctionType = type;
        }

        public TransferFunctionLib.DynamicSystem ToDynamicSystem()
        {
            if(transferFunctionType == TransferFunctionType.Continous )
            {
                return TransferFunctionLib.DynamicSystem.FromContinousTransferFuntion(nominator, denominator, timeDelay, sampleTime);
            }
            else if (transferFunctionType == TransferFunctionType.Discrete)
            {
                return TransferFunctionLib.DynamicSystem.FromDiscreteTransferFuntion(nominator, denominator, Convert.ToInt32(timeDelay), sampleTime);
            }

            return null;
        }
    }
}
