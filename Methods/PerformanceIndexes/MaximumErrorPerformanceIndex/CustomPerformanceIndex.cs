using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaximumErrorPerformanceIndex
{
    [Method("Maximum error ", "")]
    public class CustomPerformanceIndex : PerfomanceIndexMethod
    {
        public override double GetValue(double[] pv, double[] sp, int sampleTime)
        {
            Double[] Error = new Double[pv.Length];

            Double steadyStateError = pv.Last() - sp.Last();

            for (int i = 0; i < Error.Length; i++)
            {
                Error[i] = (pv[i] - sp[i]) - steadyStateError;
            }

            Double emax = Error.Max();
            Double emin = Error.Min();

            return Math.Abs(Error.Max()) > Math.Abs(Error.Min()) ? Error.Max() : Error.Min();

        }
    }
}
