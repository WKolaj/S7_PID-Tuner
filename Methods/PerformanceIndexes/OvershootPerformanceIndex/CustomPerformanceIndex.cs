using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvershootPerformanceIndex
{
    [Method("Overshoot [%]", "")]
    public class CustomPerformanceIndex : PerfomanceIndexMethod
    {
        public override double GetValue(double[] pv, double[] sp, int sampleTime)
        {
            Double[] ABSError = new Double[pv.Length];

            Double steadyStateError = pv.Last() - sp.Last();

            for (int i = 0; i < ABSError.Length; i++)
            {
                ABSError[i] = Math.Abs((pv[i] - sp[i]) - steadyStateError);
            }

            Double e1 = ABSError.Max();
            Int32 indexE1 = Array.IndexOf(ABSError, e1);

            Double[] errorsAfterMax = new Double[ABSError.Length - indexE1];

            for (int i = 0; i < errorsAfterMax.Length; i++)
            {
                errorsAfterMax[i] = ((pv[i + indexE1] - sp[i + indexE1]) - steadyStateError);
            }


            Double e2 = 0;

            if(errorsAfterMax[0] >= 0)
            {
                e2 = errorsAfterMax.Min();
            }
            else
            {
                e2 = errorsAfterMax.Max();
            }



            if (errorsAfterMax[0] * e2 < 0)
            {

                    return Math.Abs(e2 / e1) * 100;
               
            }

            return 0;

        }
    }
}
