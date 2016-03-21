using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7TCP
{
    /// <summary>
    /// Typ używanej przez zmienną pamięci:
    /// M - Flagi
    /// Q - Wyjścia
    /// I - Wejścia
    /// DB - Databloki
    /// </summary>
    public enum MemoryType
    {
        M = 131, Q = 130, I = 129, DB = 132
    }

    /// <summary>
    /// Zdarzenie zgłaszane za każdym razem, gdy zmiennej przypisywana jest wartość
    /// </summary>
    /// <param name="sender">
    /// Obiekt zmiennej ktorej wartość ulega zmianie
    /// </param>
    /// <param name="eventArgument">
    /// Argument metody obslugi zdarzenia
    /// </param>
    public delegate void ValueUpdatedEventHandler(object sender, ValueUpdatedEventArgument eventArgument);

    /// <summary>
    /// Argument zdarzenia wywoływanego za każdym gdy zmiennej przypisywana jest wartosc
    /// </summary>
    public class ValueUpdatedEventArgument : EventArgs
    {
        /// <summary>
        /// Nowa wartosc zmiennej
        /// </summary>
        public object newValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Konstruktor argumentu zdarzenia
        /// </summary>
        /// <param name="newValue">
        /// Wartosc przypisywana zmiennej
        /// </param>
        public ValueUpdatedEventArgument(Object newValue)
        {
            this.newValue = newValue;
        }

    }

    /// <summary>
    /// Zdarzenie zgłaszane za każdym razem gdy wartosc zmiennej ulega zmianie
    /// </summary>
    /// <param name="sender">
    /// Obiekt zmiennej która zgłasza zdarzenie
    /// </param>
    /// <param name="eventArgument">
    /// Argument zdarzenia
    /// </param>
    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgument eventArgument);

    /// <summary>
    /// Argument zdarzenia zglaszanego za kazdym razem, gdy wartosc zmiennej ulega zmianie
    /// </summary>
    public class ValueChangedEventArgument : EventArgs
    {
        /// <summary>
        /// Nowa wartosc zmiennej
        /// </summary>
        public object newValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Kontruktor argumentu zdarzenia
        /// </summary>
        /// <param name="newValue">
        /// Nowa wartosc przyjmowana przez zmienna
        /// </param>
        public ValueChangedEventArgument(Object newValue)
        {
            this.newValue = newValue;
        }

    }

    /// <summary>
    /// Zdarzenie zglaszane za kazdym razem, gdy status danego obiektu ulega zmianie
    /// </summary>
    /// <param name="sender">
    /// Obiekt ktorego status ulega zmianie
    /// </param>
    /// <param name="eventArgument">
    /// Argument zdarzenia
    /// </param>
    public delegate void ObjectStateChangedEventHandler(object sender, ObjectStatusChangedEventArgument eventArgument);

    /// <summary>
    /// Argument zdarzenia wywolywanego za kazdym razem, gdy status zmiennej ulega zmianie
    /// </summary>
    public class ObjectStatusChangedEventArgument : EventArgs
    {
        /// <summary>
        /// Nowy status zmiennej
        /// </summary>
        public StatusType NewStatus
        {
            get;
            protected set;
        }

        /// <summary>
        /// Komentarz statusu
        /// </summary>
        public String ErrorMessage
        {
            get;
            protected set;
        }

        /// <summary>
        /// Konstruktor argumentu zdarzenia
        /// </summary>
        /// <param name="newStatus">
        /// Nowy status obiektu
        /// </param>
        /// <param name="errorMessage">
        /// Komentarz nowego statusu
        /// </param>
        public ObjectStatusChangedEventArgument(StatusType newStatus, String errorMessage)
        {
            this.NewStatus = newStatus;
            this.ErrorMessage = errorMessage;
        }
    }
  
    /// <summary>
    /// Typ statusu obiektu
    /// Connected - polaczony
    /// Disconnected - rozlaczony
    /// </summary>
    public enum StatusType
    {
        Connected, Disconnected, Fault
    }

    /// <summary>
    /// Typ zmiennej ktora ma byc dodana sterownika
    /// </summary>
    public enum VariableType
    {
        Byte, Int32, Int16, Float, Bool, DateTime, UInt16, UInt32

    }

    /// <summary>
    /// Typ obiektu probkujacego ktory ma byc dodany do sterownika
    /// </summary>
    public enum SamplerType
    {
        Reader, Writer
    }


}
