using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Series;

namespace TransferFunctionLib
{
    /// <summary>
    /// Klasa reprezentujaca punkt charakterystyki czestotliwosciowej
    /// </summary>
    public class FrequencyPoint
    {
        /// <summary>
        /// Punkt na charakterystyce czestotliwoscej
        /// </summary>
        public Complex Point;
        
        /// <summary>
        /// Czesc rzeczywista punktu
        /// </summary>
        public Double Real
        {
            get
            {
                return Point.Real;
            }

        }

        /// <summary>
        /// Czesc urojona punktu
        /// </summary>
        public Double Imaginary
        {
            get
            {
                return Point.Imaginary;
            }

        }

        /// <summary>
        /// Czestotliwosc dla wyznaczonego punktu
        /// </summary>
        public Double Frequency
        {
            get;
            private set;
        }

        /// <summary>
        /// Klasa reprezentujaca punkt charakterystyki czestotliwosciowej
        /// </summary>
        /// <param name="real">
        /// Czesc rzeczywista
        /// </param>
        /// <param name="img">
        /// Czesc urojona
        /// </param>
        /// <param name="frequency">
        /// Czestotliwosc
        /// </param>
        public FrequencyPoint(Double real, Double img, Double frequency)
        {
            Point = new Complex(real, img);
            this.Frequency = frequency;
        }

        /// <summary>
        /// Klasa reprezentujaca punkt charakterystyki czestotliwosciowej
        /// </summary>
        /// <param name="point">
        /// Punkt na charakterystyce
        /// </param>
        /// <param name="frequency">
        /// Czestotliwosc
        /// </param>
        public FrequencyPoint(Complex point, Double frequency)
        {
            this.Point = point;
            this.Frequency = frequency;
        }

        /// <summary>
        /// Konwertuje punkt na pare klucz-wartosc
        /// </summary>
        /// <returns>
        /// Para klucz wartosc:
        /// Klucz - czesc rzeczywista
        /// Wartosc - czesc urojona
        /// </returns>
        public KeyValuePair<Double,Double> ToKeyValuePair()
        {
            return new KeyValuePair<double, double>(Real, Imaginary);
        }
    }

    /// <summary>
    /// Kolekcja punktow charakterystyki czestotliwosciowej komaptybilna z mechanizmem laczenia danych WPF
    /// </summary>
    public class FrequencyPointsObservableCollection
    {
        /// <summary>
        /// Kolekcja punktow charakterystyki czestotlwiosciowej
        /// </summary>
        public List<DataPoint> FrequencyPoints { get; set; }

        /// <summary>
        /// Konstruktor klasy kolekcji punktow charakterystyki czestotliwosciowej komaptybilna z mechanizmem laczenia danych WPF
        /// </summary>
        public FrequencyPointsObservableCollection()
        {
            this.FrequencyPoints = new List<DataPoint>();
        }

    }

    /// <summary>
    /// Kolekcja punktow charakterystyki czestotliwosciowej komaptybilna z mechanizmem laczenia danych WPF
    /// </summary>
    public class BodePointsDataPoints
    {
        /// <summary>
        /// Kolekcja punktow charakterystkyki amplitudowej
        /// </summary>
        public List<DataPoint> MarginBodePoints { get; set; }

        /// <summary>
        /// Kolekcja punktow charakterystkyki fazowej
        /// </summary>
        public List<DataPoint> PhaseBodePoints { get; set; }

        /// <summary>
        /// Kolekcja zapasow modulu
        /// </summary>
        public List<ScatterPoint> GainMargins { get; set; }

        /// <summary>
        /// Kolekcja zapasow fazy
        /// </summary>
        public List<ScatterPoint> PhaseMargins { get; set; }

        /// <summary>
        /// Konstruktor klasy kolekcji punktow charakterystyki czestotliwosciowej komaptybilna z mechanizmem laczenia danych WPF
        /// </summary>
        public BodePointsDataPoints()
        {
            this.MarginBodePoints = new List<DataPoint>();
            this.PhaseBodePoints = new List<DataPoint>();

            this.GainMargins = new List<ScatterPoint>();
            this.PhaseMargins = new List<ScatterPoint>();
        }

    }

    /// <summary>
    /// Kolekcja punktow odpowiedzi ukladu regulacji kompatybilna z mechanizmem wiazania danych WPF
    /// </summary>
    public class ChartPointObservableCollection
    {
        /// <summary>
        /// Kolekcja punktow odpowiedzi
        /// </summary>
        public List<DataPoint> y
        {
            get;
            set;
        }

        /// <summary>
        /// Kolekcja punktow wymuszenia
        /// </summary>
        public List<DataPoint> u
        {
            get;
            set;
        }

        /// <summary>
        /// Konstruktor kolekcji punktow odpowiedzi ukladu regulacji kompatybilna z mechanizmem wiazania danych WPF
        /// </summary>
        public ChartPointObservableCollection()
        {
            y = new List<DataPoint>();
            u = new List<DataPoint>();
        }

    }

    /// <summary>
    /// Klasa reprezentujaca punkt zerowy charakterystyki
    /// </summary>
    public class ZeroPoint : FrequencyPoint
    {
        /// <summary>
        /// Kierunek punktu zerowego:
        /// true - kierunek zmiany z Imz mniejszego od 0 do Imz wiekszego od 0
        /// false - kierunek zmiany z Imz wiekszego od 0 do Imz mniejszego od 0
        /// </summary>
        public bool Direction
        {
            get;
            private set;
        }

        /// <summary>
        /// Klasa reprezentujaca punkt zerowy charakterystyki
        /// </summary>
        /// <param name="real">
        /// Czesc rzeczywista
        /// </param>
        /// <param name="img">
        /// Czesc urojona
        /// </param>
        /// <param name="frequency">
        /// Czestotliwosc
        /// </param>
        /// <param name="direction">
        /// Kierunek zmian
        /// </param>
        public ZeroPoint(Double real, Double img, Double frequency, Boolean direction)
            : base(real, img, frequency)
        {
            this.Direction = direction;
        }

        /// <summary>
        /// Klasa reprezentujaca punkt zerowy charakterystyki
        /// </summary>
        /// <param name="point">
        /// Punkt zespolony charakterystyki
        /// </param>
        /// <param name="frequency">
        /// Czestotliwosc
        /// </param>
        /// <param name="direction">
        /// Kierunek
        /// </param>
        public ZeroPoint(Complex point, Double frequency, Boolean direction)
            : base(point, frequency)
        {
            this.Direction = direction;
        }
    }

    /// <summary>
    /// Klasa reprezentujaca punkt zapasu stabilnosci
    /// </summary>
    public class Margin:ZeroPoint
    {
        /// <summary>
        /// Wartosc zapasu stabilnosci
        /// </summary>
        public Double Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Wartosc funkcji bodego - zapasu modulu, lub zapas fazy, w zaleznosci czego ten zapas dotyczy
        /// </summary>
        public Double BodeValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Klasa reprezentujaca punkt zapasu stabilnosci
        /// </summary>
        /// <param name="real">
        /// Czesc rzeczywista
        /// </param>
        /// <param name="img">
        /// Czesc urojona
        /// </param>
        /// <param name="frequency">
        /// Czestotlwiosc
        /// </param>
        /// <param name="direction">
        /// Kierunek zmian
        /// </param>
        /// <param name="value">
        /// Wartosc
        /// </param>
        public Margin(Double real, Double img, Double frequency, bool direction, Double value, Double bodeValue):base(real,img,frequency,direction)
        {
            this.Value = value;
            this.BodeValue = bodeValue;
        }

        /// <summary>
        /// Klasa reprezentujaca punkt zapasu stabilnosci
        /// </summary>
        /// <param name="point">
        /// Liczba zespolona reprezentujaca punkt na charakterystyce
        /// </param>
        /// <param name="frequency">
        /// Czestotliwosc
        /// </param>
        /// <param name="direction">
        /// Kierunek
        /// </param>
        /// <param name="value">
        /// Wartosc zapasu stabilnosci
        /// </param>
        public Margin(Complex point, Double frequency, bool direction, Double value, Double bodeValue)
            : base(point, frequency, direction)
        {
            this.Value = value;
            this.BodeValue = bodeValue;
        }

    }

    /// <summary>
    /// Klasa reprezentujaca punkt charakterystki Bodego
    /// </summary>
    public class BodePoint:FrequencyPoint
    {
        /// <summary>
        /// Wartosc wzmocnienia
        /// </summary>
        public Double GainValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Wartosc fazy
        /// </summary>
        public Double PhaseValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Klasa reprezentujaca punkt charakterystki Bodego
        /// </summary>
        /// <param name="real">
        /// Czesc rzeczywista
        /// </param>
        /// <param name="img">
        /// Czesc urojona
        /// </param>
        /// <param name="frequency">
        /// Czestotliwosc
        /// </param>
        /// <param name="gainValue">
        /// Wartosc modułu
        /// </param>
        /// <param name="phaseValue">
        /// Wartosc fazy
        /// </param>
        public BodePoint(Double real, Double img, Double frequency,Double gainValue, Double phaseValue):
            base(real,img,frequency)
        {
            this.GainValue = gainValue;
            this.PhaseValue = phaseValue;
        }

        /// <summary>
        /// Klasa reprezentujaca punkt charakterystki Bodego
        /// </summary>
        /// <param name="point">
        /// Punkt na charakterystyce czestotliwosciowej
        /// </param>
        /// <param name="frequency">
        /// Czestotliwosc
        /// </param>
        /// <param name="gainValue">
        /// Wartosc modułu
        /// </param>
        /// <param name="phaseValue">
        /// Wartosc fazy
        /// </param>
        public BodePoint(Complex point, Double frequency, Double gainValue, Double phaseValue) :
            base(point, frequency)
        {
            this.GainValue = gainValue;
            this.PhaseValue = phaseValue;
        }

    }
}
