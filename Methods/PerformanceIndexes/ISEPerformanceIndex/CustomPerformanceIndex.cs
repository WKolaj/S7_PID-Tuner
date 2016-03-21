using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISEPerformanceIndex
{
    [Method("ISE", "")]
    public class CustomPerformanceIndex : PerfomanceIndexMethod
    {
        public override double GetValue(double[] pv, double[] sp, int sampleTime)
        {
            Double ISE = 0;

            Double realSampleTime = Convert.ToDouble(sampleTime)/1000;

            for(int i=0; i<pv.Length; i++)
            {
                ISE += (pv[i] - sp[i]) * (pv[i] - sp[i]) * realSampleTime;
            }

            return ISE;
        }
    }
}
