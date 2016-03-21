using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferFunctionLib;


namespace DynamicMethodsLibrary
{
    public abstract class PerfomanceIndexMethod
    {
        public abstract Double GetValue(Double[] pv, Double[] sp, Int32 sampleTime);
    }

}
