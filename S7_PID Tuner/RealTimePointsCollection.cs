using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using System.Diagnostics;

namespace S7_PID_Tuner
{
    /// <summary>
    /// Klasa kolekcji punktow czasu rzeczywistego
    /// </summary>
    public class RealTimePointsCollection:IEnumerable<DataPoint>
    { 
        private Int32 maxLength;
        /// <summary>
        /// Maksymalna dlugosc kolekcji
        /// </summary>
        public Int32 MaxLength
        {
            get
            {
                return maxLength;
            }
            set
            {
                //kazde ustawienie nowej wartosci dlugosci wymaga usuniecia starej zawartosci
                points.Clear();
                maxLength = value;
            }
        }

        /// <summary>
        /// Lista w ktorej przechowywane sa punkty kolekcji
        /// </summary>
        public List<DataPoint> points = new List<DataPoint>();

        public IEnumerator<DataPoint> GetEnumerator()
        {
            return new RealTimePointsEnumerator(points);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new RealTimePointsEnumerator(points);
        }

        /// <summary>
        /// Metoda dodajaca element do kolekcji - elementowi zostanie przypisany dokladny czas w ktorym zostal on dodany
        /// </summary>
        /// <param name="point">
        /// Nowa wartosc
        /// </param>
        public void Add(DataPoint point)
        {
            //Jezeli jest zbyt duzo elementow - nalezy usunac pierwszy
            if(points.Count >= MaxLength && MaxLength > 0)
            {
                points.RemoveAt(0);
            }

            points.Add(point);
        }

        /// <summary>
        /// Metoda dodajaca element do kolekcji - elementowi zostanie przypisany dokladny czas w ktorym zostal on dodany
        /// </summary>
        /// <param name="yValue">
        /// Nowa wartosc
        /// </param>
        public void Add(Double yValue)
        {
            Add(DateTimeAxis.CreateDataPoint(DateTime.Now,yValue));
        }

        /// <summary>
        /// Metoda usuwajaca element
        /// </summary>
        /// <param name="point">
        /// Usuwany element
        /// </param>
        public void Remove(DataPoint point)
        {
            points.Remove(point);
        }

        public DataPoint this[int index]
        {
            get
            {
                return this.points[index];
            }

            set
            {
                this.points[index] = value;
            }
        }

        /// <summary>
        /// Konstruktor klasy kolekcji czasu rzeczywistego
        /// </summary>
        /// <param name="maxLength">
        /// Maksymalna dlugosc bufora
        /// </param>
        public RealTimePointsCollection(Int32 maxLength)
        {
            this.MaxLength = maxLength;
        }

    }

    /// <summary>
    /// Enumerator kolekcji czasu rzeczywistego
    /// </summary>
    public class RealTimePointsEnumerator : IEnumerator<DataPoint>
    {
        protected List<DataPoint> points;

        protected int currentIndex = -1;

        public RealTimePointsEnumerator(List<DataPoint> points)
        {
            this.points = points;
        }

        public DataPoint Current
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
