using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSMethod
{
    class PointCollection : IEnumerable<Double>
    {
        List<Double> points = new List<double>();

        public IEnumerator<Double> GetEnumerator()
        {
            return new PointCollectionEnumerator(points);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new PointCollectionEnumerator(points);
        }

        public void Add(Double value)
        {
            points.Add(value);
        }

        public void Remove(Double point)
        {
            points.Remove(point);
        }

        public Double this[int index]
        {
            get
            {
                if (index < 0)
                {
                    return points.First();
                }

                if (index >= points.Count)
                {
                    return points.Last();
                }

                return points[index];
            }

            set
            {
                this.points[index] = value;
            }
        }

        public void AssignList(List<Double> points)
        {
            this.points = points;
        }

        public void AssignArray(Double[] points)
        {
            this.points = points.ToList();
        }
    }


    public class PointCollectionEnumerator : IEnumerator<Double>
    {
        protected List<Double> points;

        protected int currentIndex = -1;

        public PointCollectionEnumerator(List<Double> pointCollection)
        {
            points = pointCollection;

        }

        public Double Current
        {
            get
            {
                if (this.currentIndex == -1)
                {
                    throw new InvalidOperationException("Use MoveNext before calling Current");
                }


                return points.ElementAt(currentIndex);
            }
        }

        public void Dispose()
        {

        }

        Object System.Collections.IEnumerator.Current
        {
            get
            {
                if (this.currentIndex == -1)
                {
                    throw new InvalidOperationException("Use MoveNext before calling Current");
                }


                return points.ElementAt(currentIndex);
            }
        }

        public bool MoveNext()
        {
            currentIndex++;

            if (currentIndex >= points.Count)
            {
                currentIndex = points.Count - 1;
                return false;
            }

            return true;
        }

        public void Reset()
        {
            currentIndex = -1;
        }
    }


    
}
