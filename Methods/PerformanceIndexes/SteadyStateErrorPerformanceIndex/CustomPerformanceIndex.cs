using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteadyStateErrorPerformanceIndex
{

    [Method("Steady state error", "")]
    public class CustomPerformanceIndex : PerfomanceIndexMethod
    {
        public override double GetValue(double[] pv, double[] sp, int sampleTime)
        {
            

            return pv.Last()-sp.Last();
        }
    }
}
