using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAEPerformanceIndex
{
    [Method("ITAE", "")]
    public class CustomPerformanceIndex : PerfomanceIndexMethod
    {
        public override double GetValue(double[] pv, double[] sp, int sampleTime)
        {
            Double ITAE = 0;

            Double realSampleTime = Convert.ToDouble(sampleTime) / 1000;

            for (int i = 0; i < pv.Length; i++)
            {
                ITAE += Math.Abs(pv[i] - sp[i]) *i*realSampleTime * realSampleTime;
            }

            return ITAE;
        }
    }
}
