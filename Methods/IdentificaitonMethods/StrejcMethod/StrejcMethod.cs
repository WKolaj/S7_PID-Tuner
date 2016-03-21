using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StrejcMethod
{
    public class StrejcMethod
    {
        private Double[] pvPoints = null;
        public Double[] PVPoints
        {
            get
            {
                return pvPoints;
            }

            set
            {
                pvPoints = value;
            }
        }

        private Double[] cvPoints = null;
        public Double[] CVPoints
        {
            get
            {
                return cvPoints;
            }

            set
            {
                cvPoints = value;
            }
        }

        private Double[] firstDerivative = null;
        public Double[] FirstDerivative
        {
            get
            {
                if (firstDerivative == null)
                {
                    firstDerivative = CalculateFirstDerivative();
                }

                return firstDerivative;
            }

            set
            {
                firstDerivative = value;
            }
        }

        private Double[] secondDerivative = null;
        public Double[] SecondDerivative
        {
            get
            {
                if (secondDerivative == null)
                {
                    secondDerivative = CalculateSecondDerivative();
                }

                return secondDerivative;
            }

            set
            {
                secondDerivative = value;
            }
        }

        private Double sampleTime;
        public Double SampleTime
        {
            get
            {
                return sampleTime;
            }

            set
            {
                sampleTime = value;
            }
        }

        private Int32 sensitivity = 1;
        public Int32 Sensitivity
        {
            get
            {
                return sensitivity;
            }

            set
            {
                sensitivity = value;
            }
        }

        private Int32 aprroximationPointsNumber = 5;
        private Int32 ApproximationPointsNumber
        {
            get
            {
                return aprroximationPointsNumber;
            }

            set
            {
                aprroximationPointsNumber = value;
            }
        }

        private Double cv0;
        private Double pv0;

        public Int32 GetRank(Double T1, Double T2)
        {
            Double factor = T1 / T2;
            Int32 rank = 0;

            for (int i = 0; i < StrejcTable.Length; i++)
            {
                if (StrejcTable[i][3] <= factor)
                {
                    rank = Convert.ToInt32(StrejcTable[i][0]);
                }
            }

            return rank;
        }

        public Double GetTimeDelay(Double T1, Double T2, Int32 rank)
        {
            Double alpha = StrejcTable[rank - 1][3];

            return (T1 - (T2 * alpha));
        }

        public Double GetT(Double ti, Int32 rank, Double timeDelay)
        {
            Double beta = StrejcTable[rank - 1][4];
            return (ti - timeDelay) / beta;
        }

        public TransferFunctionClass GetTransferFunction()
        {
            try
            {
                Double ti = Convert.ToDouble(InflectionPoint) * SampleTime;
                Double T1 = ti - (PVPoints[InflectionPoint] - pv0) / FirstDerivative[InflectionPoint];
                Double T2 = ti + (PVPoints[InflectionPoint] - pv0) / FirstDerivative[InflectionPoint];
                Int32 rank = GetRank(T1, T2);
                Double timeDelay = GetTimeDelay(T1, T2, rank);
                Double Tob = GetT(ti, rank, timeDelay);
                Double[] denominator = GetDenominator(rank, Tob);
                Double[] nominator = new Double[] { (pvPoints.Last() - pv0) / (cvPoints.Last() - cv0) };
                return new TransferFunctionClass(nominator, denominator, timeDelay, Convert.ToInt32(SampleTime * 1000), TransferFunctionType.Continous);
            }
            catch
            {
                if(InflectionPoint == 0)
                {
                    throw new Exception("Second derivative has been found too early - decrese sentitivity of algoritm or sample time");
                }
                else
                {
                    throw new Exception("Second derivative has not been found - increse sentitivity of algoritm");
                }
            }

        }

        public Double[] GetDenominator(Int32 rank, Double Tob)
        {
            Double[] array = GetNewtonTable(rank);

            for (int i = 0; i < array.Length; i++)
            {
                array[i] *= Math.Pow(Tob, i);
            }

            return array;
        }

        public Double[] GetNewtonTable(int n)
        {
            Double[] newtonTable = new Double[n + 1];

            newtonTable[0] = 1;

            for (int i = 1; i < n + 1; i++)
            {
                newtonTable[i] = newtonTable[i - 1] * Convert.ToDouble(n - i + 1) / Convert.ToDouble(i);
            }

            return newtonTable;
        }

        public StrejcMethod(Double[] cvPoints, Double[] pvPoints, Double sampleTime, Int32 sensitivity, Int32 aproximatingPointsNumber)
        {

            Int32 StepK = GetStepK(cvPoints);
            this.CVPoints = NormalizePointArray(cvPoints, StepK);
            this.PVPoints = NormalizePointArray(pvPoints, StepK);
            this.SampleTime = sampleTime;
            this.cv0 = cvPoints.First();
            this.pv0 = pvPoints.First();
            this.Sensitivity = sensitivity;
            this.aprroximationPointsNumber = aproximatingPointsNumber;
        }

        public Int32 GetStepK(Double[] cvPoints)
        {
            for (int i = 1; i < cvPoints.Length; i++)
            {
                if (cvPoints[i] - cvPoints[i - 1] != 0)
                {
                    return i;
                }
            }

            return cvPoints.Length - 1;
        }

        private Double[] NormalizePointArray(Double[] array, Int32 stepK)
        {
            List<Double> newPoints = new List<double>();

            for (int i = stepK; i < array.Length; i++)
            {
                newPoints.Add(array[i]);
            }

            return newPoints.ToArray();
        }

        private Double[][] StrejcTable = new Double[][]
        {
            new Double[] {  1,   1.000,   0.000,  0.000,  0,  0.000 },
            new Double[] {  2,   2.718,   0.282,  0.104,  1,  0.264 },
            new Double[] {  3,   3.695,   0.805,  0.218,  2,  0.323 },
            new Double[] {  4,   4.463,   1.425,  0.319,  3,  0.353 },
            new Double[] {  5,   5.119,   2.100,  0.410,  4,  0.371 },
            new Double[] {  6,   5.699,   2.811,  0.493,  5,  0.384 },
            new Double[] {  7,   6.226,   3.549,  0.570,  6,  0.394 },
            new Double[] {  8,   6.711,   4.307,  0.642,  7,  0.401 },
            new Double[] {  9,   7.164,   5.081,  0.709,  8,  0.407 },
            new Double[] { 10,   7.590,   5.869,  0.773,  9,  0.413 }
        };

        private Int32 inflectionPoint = -1;
        private Int32 InflectionPoint
        {
            get
            {
                if (inflectionPoint < 0)
                {
                    inflectionPoint = GetInflectionPoint();
                }

                return inflectionPoint;
            }

            set
            {
                inflectionPoint = value;
            }
        }

        private Int32 GetInflectionPoint()
        {
            for (int i = 0; i < SecondDerivative.Length - 1; i++)
            {
                if (SecondDerivative[i] * SecondDerivative[i + 1] < 0 && Math.Round(FirstDerivative[i], Sensitivity) != 0)
                {
                    return i;
                }
            }

            return SecondDerivative.Length - 1;
        }

        Double[] CalculateFirstDerivative()
        {
            Double[] derivative = new Double[PVPoints.Count()];

            for (int i = 0; i < derivative.Length; i++)
            {
                derivative[i] = AlfaOfLinearFunction(i, aprroximationPointsNumber, PVPoints);
            }

            return derivative;
        }

        Double[] CalculateSecondDerivative()
        {
            Double[] secondDerivative = new Double[CVPoints.Count()];

            for (int i = 0; i < secondDerivative.Length; i++)
            {
                secondDerivative[i] = AlfaOfLinearFunction(i, aprroximationPointsNumber, FirstDerivative);
            }

            return secondDerivative;
        }

        private Double AlfaOfLinearFunction(int k, int numberOfPoints, Double[] functionPoints)
        {
            Int32 numberOfPointsBefore = numberOfPoints / 2;
            Int32 numberOfPointsAfter = numberOfPoints - numberOfPointsBefore;

            Double X = 0;
            Double Y = 0;
            Double sumX2 = 0;
            Double sumXY = 0;
            Double sumX = 0;
            Double sumY = 0;

            int actualNumber;

            Double numberOfSample = 0.0;

            for (int i = k - numberOfPointsBefore; i < k + numberOfPointsAfter; i++)
            {
                if (i < 0)
                {
                    actualNumber = 0;
                }
                else if (i > functionPoints.Length - 1)
                {
                    actualNumber = functionPoints.Length - 1;
                }
                else
                {
                    actualNumber = i;
                }

                X = SampleTime * numberOfSample;
                Y = functionPoints[actualNumber];

                sumX2 += X * X;
                sumX += X;
                sumXY += X * Y;
                sumY += Y;

                numberOfSample++;

            }

            Double alfa = (Convert.ToDouble(numberOfPoints) * sumXY - sumX * sumY) / (Convert.ToDouble(numberOfPoints) * sumX2 - sumX * sumX);

            return alfa;
        }

    }
}
