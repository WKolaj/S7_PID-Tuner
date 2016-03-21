using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7TCP
{
    /// <summary>
    /// Klasa zmiennej typu UInt16
    /// </summary>
    public class UInt16Variable : VariableBase
    {
        /// <summary>
        /// Konstruktor klasy zmiennej typu UInt16
        /// </summary>
        /// <param name="Name">
        /// Nazwa zmiennej
        /// </param>
        /// <param name="connectionObject">
        /// Obiekt polaczenia ze sterownikiem
        /// </param>
        /// <param name="memoryType">
        /// Typ pamieci w ktorej przechowywana jest zmienna w sterowniku
        /// </param>
        /// <param name="areaBegining">
        /// Poczatek adresu zmiennej
        /// </param>
        /// <param name="commonLockingObject">
        /// Obiekt synchronizujacy  
        /// </param>
        /// <param name="dataBlockNumber">
        /// Numer wykorzystywanego databloku
        /// </param>
        public UInt16Variable(string Name, Connection connectionObject, MemoryType memoryType, int areaBegining, Object commonLockingObject, int dataBlockNumber = 0) :
            base(Name, connectionObject, memoryType, dataBlockNumber, areaBegining, 2, commonLockingObject)
        {
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
            return libnodave.getS16from(buffer, position);
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
            return libnodave.daveSwapIed_16((int)objectToCode);
        }

        /// <summary>
        /// Metoda wykonywana za kazdym razem gdy obiekt probkujacy pobierze dane odpowiedzi sterownika i chce aby dana zmienna je poprawnie odczytała
        /// </summary>
        protected internal override void OnSamplerReadRequest()
        {
            Value = connection.libnodaveConnection.getU16();
        }
    }
}
