using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAEPerformanceIndex
{
    [Method("IAE", "")]
    public class CustomPerformanceIndex : PerfomanceIndexMethod
    {
        public override double GetValue(double[] pv, double[] sp, int sampleTime)
        {
            Double IAE = 0;

            Double realSampleTime = Convert.ToDouble(sampleTime) / 1000;

            for (int i = 0; i < pv.Length; i++)
            {
                IAE += Math.Abs(pv[i] - sp[i])  * realSampleTime;
            }

            return IAE;
        }
    }
}
