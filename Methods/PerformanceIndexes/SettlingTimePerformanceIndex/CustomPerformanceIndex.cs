using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettlingTimePerformanceIndex
{
    [Method("Settling time [s]", "")]
    public class CustomPerformanceIndex : PerfomanceIndexMethod
    {
        public override double GetValue(double[] pv, double[] sp, int sampleTime)
        {
            Double realSampleTime = Convert.ToDouble(sampleTime) / 1000;

            Double[] error = new Double[pv.Length];

            for(int i=0; i<error.Length; i++)
            {
                error[i] = pv[i] - sp[i];
            }

            Double maxError = Math.Abs(error.Min()) > Math.Abs(error.Max()) ? Math.Abs(error.Min()) : Math.Abs(error.Max());
            Double SteadyStateDelta = 0.05 * maxError;

            Double MinimSteadyStateValue = error.Last() - SteadyStateDelta;
            Double MaxSteadyStateValue = error.Last() + SteadyStateDelta;

            Boolean startOfSteadyStateFound = false;
            Int32 startOfSteadyState = 0;
            
            Int32 cVChangeTime = 0;

            for(int i = error.Length -1; i>0; i--)
            {
                if (!startOfSteadyStateFound && (error[i] >= MaxSteadyStateValue || error[i] <= MinimSteadyStateValue))
                {
                    startOfSteadyState = i;
                    startOfSteadyStateFound = true;
                }

                if(sp[i] != sp[i-1])
                {
                    cVChangeTime = i - 1;
                }
            }

            return Convert.ToDouble(startOfSteadyState - cVChangeTime) * realSampleTime;

        }
    }
}
