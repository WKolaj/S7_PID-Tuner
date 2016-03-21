using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7TCP
{
    /// <summary>
    /// Klasa zmiennej typu UInt32
    /// </summary>
    public class UInt32Variable : VariableBase
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
        public UInt32Variable(string Name, Connection connectionObject, MemoryType memoryType, int areaBegining, Object commonLockingObject, int dataBlockNumber = 0) :
            base(Name, connectionObject, memoryType, dataBlockNumber, areaBegining, 4, commonLockingObject)
        {
            internalValue = 0;

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
        override protected int CodingMethod(object objectToCode)
        {
            return libnodave.daveSwapIed_32((int)objectToCode);
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
        override protected object DecodingMethod(byte[] buffer, int position)
        {
            return libnodave.getS32from(buffer, position);
        }

        /// <summary>
        /// Metoda wykonywana za kazdym razem gdy obiekt probkujacy pobierze dane odpowiedzi sterownika i chce aby dana zmienna je poprawnie odczytała
        /// </summary>
        protected internal override void OnSamplerReadRequest()
        {
            Value = connection.libnodaveConnection.getU32();
        }
    }
}
