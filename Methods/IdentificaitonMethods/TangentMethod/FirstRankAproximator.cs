using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tangent_method
{
    public class FirstRankAproximator
    {
        Double[] CVPoints;
        Double[] PVPoints;

        Double firstCV;

        private Int32 lastPointOfIdentification;

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


        private Int32 aproximatingPointsNumber = 5;
        public Int32 AproximatingPointsNumber
        {
            get
            {
                return aproximatingPointsNumber;
            }

            private set
            {
                aproximatingPointsNumber = value;
            }
        }


        public FirstRankAproximator(Double[] CVPoints, Double[] PVPoints, Int32 aproximatingPointsNumber, Double sampleTime)
        {
            Int32 firstPoint;
            Int32 lastPoint;

            this.firstCV = CVPoints.First();

            this.SampleTime = sampleTime;

            this.CVPoints = NormalizeCVArray(CVPoints, out firstPoint, out lastPoint);
            this.PVPoints = NormalizePVArray(PVPoints, firstPoint, lastPoint);

            this.lastPointOfIdentification = GetLastPointOfIdentification(this.CVPoints, this.PVPoints);
            this.AproximatingPointsNumber = aproximatingPointsNumber;
        }

        private Double[] NormalizeCVArray(Double[] CVPoints, out Int32 firstPoint, out Int32 lastPoint)
        {
            Double fristCV = CVPoints.First();
            firstPoint = 0;
            lastPoint = CVPoints.Length - 1;

            for (int i = 0; i < CVPoints.Length; i++)
            {
                if (CVPoints[i] != fristCV && CVPoints[i] == CVPoints[i+1])
                {
                    firstPoint = i;
                    break;
                }

            }

            for (int i = firstPoint; i < CVPoints.Length; i++)
            {
                if (CVPoints[i] != CVPoints[firstPoint])
                {
                    lastPoint = i;
                    break;
                }
            }

            List<Double> newPoints = new List<double>();

            for (int i = firstPoint; i < lastPoint; i++)
            {
                newPoints.Add(CVPoints[i]);
            }

            return newPoints.ToArray();
        }

        private Int32 GetLastPointOfIdentification(Double[] CVPoints, Double[] PVPoints)
        {
            Double delta = CVPoints.Last() - firstCV;

            Double finalPV = PVPoints.First() + delta;

            for (int i = 0; i < PVPoints.Length; i++)
            {
                if (delta > 0)
                {
                    if (PVPoints[i] >= finalPV)
                    {
                        return i;
                    }
                }
                else
                {
                    if (PVPoints[i] <= finalPV)
                    {
                        return i;
                    }
                }
            }

            return PVPoints.Length - 1;
        }

        private Double[] NormalizePVArray(Double[] PVPoints, Int32 firstPoint, Int32 lastPoint)
        {
            List<Double> newPoints = new List<double>();

            for (int i = firstPoint; i < lastPoint; i++)
            {
                newPoints.Add(PVPoints[i]);
            }

            return newPoints.ToArray();
        }

        private Double AlfaOfLinearFunction(int k, int numberOfPoints, Double[] functionPoints)
        {
            Int32 numberOfPointsBefore = numberOfPoints;
            Int32 numberOfPointsAfter = 0;

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

        public TransferFunctionClass GetTransferFunction()
        {
            Double dervitavie = AlfaOfLinearFunction(PVPoints.Length - 1, AproximatingPointsNumber, PVPoints);
            Double T = (PVPoints[lastPointOfIdentification] - PVPoints.First()) / dervitavie;
            Double length = lastPointOfIdentification * SampleTime;
            Double timeDelay = length - T;
            Double[] nominator = new Double[] { 1 };
            Double[] denominator = new Double[] { 0,T };

            return new TransferFunctionClass(nominator, denominator, timeDelay, Convert.ToInt32(SampleTime * 1000), TransferFunctionType.Continous);
        }
    }
}
