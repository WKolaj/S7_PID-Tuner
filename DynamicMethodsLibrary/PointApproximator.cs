using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicMethodsLibrary
{
    internal class PointApproximator
    {
        private List<DataPoint> firstDataPointCollection;
        private List<DataPoint> secondDataPointCollection;

        private List<DataPoint> firstOriginalCollection;
        private List<DataPoint> secondOriginalCollection;

        private DateTime zeroPoint;

        private DateTime startDateTime;
        private Double sampleTime;
        private Double startTime;
        private Double stopTime;

        private Int32 firstCollectionOffset;
        private Int32 secondCollectionOffset;

        public PointApproximator(List<DataPoint> firstDataPointCollection, List<DataPoint> secondDataPointCollection, Double SampleTime)
        {
            this.sampleTime = SampleTime;

            firstOriginalCollection = firstDataPointCollection;
            secondOriginalCollection = secondDataPointCollection;

            CalculateZeroPoint(firstDataPointCollection, secondDataPointCollection);
            CalculateCollectionsPoint(firstDataPointCollection, secondDataPointCollection);
            CalculateStartTime();
            CalculateStopTime();
            CalculateOffsets();
        }

        private void CalculateCollectionsPoint(List<DataPoint> firstDataPointCollection, List<DataPoint> secondDataPointCollection)
        {
            this.firstDataPointCollection = (from point in firstDataPointCollection
                                             select new DataPoint((DateTimeAxis.ToDateTime(point.X) - zeroPoint).TotalMilliseconds, point.Y)).ToList();

            this.secondDataPointCollection = (from point in secondDataPointCollection
                                              select new DataPoint((DateTimeAxis.ToDateTime(point.X) - zeroPoint).TotalMilliseconds, point.Y)).ToList();
        }

        private void CalculateZeroPoint(List<DataPoint> firstDataPointCollection, List<DataPoint> secondDataPointCollection)
        {

            zeroPoint = firstDataPointCollection.First().X > secondDataPointCollection.First().X ? DateTimeAxis.ToDateTime(secondDataPointCollection.First().X) : DateTimeAxis.ToDateTime(firstDataPointCollection.First().X);
        }

        private void CalculateStartTime()
        {
            if (firstDataPointCollection.First().X < secondDataPointCollection.First().X)
            {
                Int32 j = 0;

                while (firstDataPointCollection[j].X < secondDataPointCollection.First().X)
                {
                    j++;
                }

                startDateTime = DateTimeAxis.ToDateTime(firstOriginalCollection[j].X);
                startTime = firstDataPointCollection[j].X;
            }
            else
            {
                Int32 j = 0;

                while (secondDataPointCollection[j].X < firstDataPointCollection.First().X)
                {
                    j++;
                }

                startDateTime = DateTimeAxis.ToDateTime(secondOriginalCollection[j].X);
                startTime = secondDataPointCollection[j].X;
            }
        }

        private void CalculateStopTime()
        {
            if (firstDataPointCollection[firstDataPointCollection.Count - 2].X < secondDataPointCollection[secondDataPointCollection.Count - 2].X)
            {
                stopTime = firstDataPointCollection[firstDataPointCollection.Count - 2].X;
            }
            else
            {
                stopTime = secondDataPointCollection[secondDataPointCollection.Count - 2].X;
            }
        }

        private void CalculateOffsets()
        {
            firstCollectionOffset = 0;

            Int32 i = 0;

            while (firstDataPointCollection[i].X < startTime)
            {
                i++;
            }

            firstCollectionOffset = i;

            Int32 j = 0;

            while (secondDataPointCollection[j].X < startTime)
            {
                j++;
            }

            secondCollectionOffset = j;
        }

        private Double AproxValue(List<DataPoint> collection, Double xValue)
        {
            int i = collection.Count - 2;

            while (collection[i].X > xValue)
            {
                i--;
            }

            if (collection[i].X == xValue)
            {
                return collection[i].Y;
            }
            else
            {
                return ((xValue - collection[i].X) * (collection[i + 1].Y - collection[i].Y) / (collection[i + 1].X - collection[i].X)) + collection[i].Y;
            }
        }

        public Double[][] GetNormalizedCollections(out DateTime startDateTime)
        {
            List<Double> firstCollectionPoints = new List<Double>();
            List<Double> secondCollectionPoints = new List<Double>();

            Double time = startTime;
            startDateTime = this.startDateTime;

            while (time <= stopTime)
            {
                firstCollectionPoints.Add(AproxValue(firstDataPointCollection, time));
                secondCollectionPoints.Add(AproxValue(secondDataPointCollection, time));

                time += sampleTime;
            }

            return new Double[2][] { firstCollectionPoints.ToArray(), secondCollectionPoints.ToArray() };
        }

    }
}
