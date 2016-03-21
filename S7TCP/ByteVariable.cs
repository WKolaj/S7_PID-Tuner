using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7TCP
{
    public class ByteVariable : VariableBase
    {
        /// <summary>
        /// Kontruktor zmiennej typu bajt
        /// </summary>
        /// <param name="Name">
        /// Nazwa zmiennej
        /// </param>
        /// <param name="connectionObject">
        /// Obiekt polaczenia
        /// </param>
        /// <param name="memoryType">
        /// Typ wykorzystywanej w sterowniku pamieci
        /// </param>
        /// <param name="areaBegining">
        /// Adres poczatkowy zmiennej
        /// </param>
        /// <param name="commonLockingObject">
        /// Obiekt synchornizujacy dostep do polaczenia
        /// </param>
        /// <param name="dataBlockNumber">
        /// Numer databloku
        /// </param>
        public ByteVariable(string Name, Connection connectionObject, MemoryType memoryType, int areaBegining, Object commonLockingObject, int dataBlockNumber = 0) :
            base(Name, connectionObject, memoryType, dataBlockNumber, areaBegining, 1, commonLockingObject)
        {
            //Inicjalizacja zmiennych
            internalValue = 0;

        }

        /// <summary>
        /// Metod dekodująca - zamieniajaca tablice bajtow odpowiedzi sterownika na konkretna wartosc zmiennej
        /// </summary>
        /// <param name="buffer">
        /// tablica bajtów do zdekodowania - buffor pobrany ze sterownika
        /// </param>
        /// <param name="position">
        /// Pozycja od ktorej rozpoczyna sie dekodowanie
        /// </param>
        /// <returns>
        /// Zdekodowana wartość zmiennej
        /// </returns>
        override protected Object DecodingMethod(byte[] buffer, int position)
        {
            return buffer[0];
        }

        /// <summary>
        /// Metoda zamieniajaca zmienna na jej odpowiednik w postaci liczby int stanowiacą sekwencje bajtow przesylanych do sterownika
        /// </summary>
        /// <param name="objectToCode">
        /// Przekształcana zmienna
        /// </param>
        /// <returns>
        /// Zmienna po przekształceniu
        /// </returns>
        override protected int CodingMethod(Object objectToCode)
        {
            return (int)((byte)objectToCode);
        }

        /// <summary>
        /// Metoda wykonywana za kazdym razem gdy obiekt probkujacy pobierze dane odpowiedzi sterownika i chce aby dana zmienna je poprawnie odczytała
        /// </summary>
        protected internal override void OnSamplerReadRequest()
        {
            //Odczyt wartosci za pomoca metody z biblioteki libnodave pobierajaca wartosc zmiennej liczbowej ze znakiem kodowanej na 8 bitach
            Value = (byte)connection.libnodaveConnection.getS8();
        }
    }
}


