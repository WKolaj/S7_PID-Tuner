using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7TCP
{
    /// <summary>
    /// Klasa zmiennej typu bool
    /// </summary>
    public class BooleanVariable : VariableBase
    {
        /// <summary>
        /// Offset bitu w bajcie w adresie danej zmiennej
        /// </summary>
        protected int bitOffset;

        /// <summary>
        /// Offset bitu w bajcie w adresie danej zmiennej
        /// </summary>
        public int BitOffset
        {
            get
            {
                return bitOffset;
            }
            set
            {
                //zmiana offsetu wymaga synchronizacji
                lock(lockingObject)
                {
                    //Offset bitu nie moze byc mniejszy od zera ani wiekszy niz 7
                    if (value < 0)
                    {
                        bitOffset = 0;
                    }
                    else
                    {
                        if (value > 7)
                        {
                            bitOffset = 7;
                        }
                        else
                        {
                            bitOffset = value;
                        } 
                    }
                    
                }
            }
        }

        /// <summary>
        /// Konstruktor zmiennej typu Bool
        /// </summary>
        /// <param name="Name">
        /// Nazwa zmiennej
        /// </param>
        /// <param name="connectionObject">
        /// Obiekt polaczenia wykorzystywany przez zmienna
        /// </param>
        /// <param name="memoryType">
        /// Typ wykorzystywanej pamieci
        /// </param>
        /// <param name="byteNumber">
        /// Numer bajtu
        /// </param>
        /// <param name="bitNumber">
        /// Numer bitu
        /// </param>
        /// <param name="commonLockingObject">
        /// Obiekt synchronizujacy dostep do polaczenia
        /// </param>
        /// <param name="dataBlockNumber">
        /// Numer databloku
        /// </param>
        public BooleanVariable(string Name, Connection connectionObject, MemoryType memoryType, int byteNumber, int bitNumber, Object commonLockingObject, int dataBlockNumber = 0) :
            base(Name, connectionObject, memoryType, dataBlockNumber, byteNumber, 1, commonLockingObject)
        {
            //inicjalizacja zmiennych
            internalValue = false;

            if (bitNumber < 0) bitNumber = 0;
            if (bitNumber > 7) bitNumber = 7;

            bitOffset = bitNumber;

        }

        /// <summary>
        /// Metoda pozwalajaca na przeliczenie adresu w postaci bajtow i bitu na adres w postaci ilosci bitow
        /// </summary>
        /// <returns>
        /// Adres bitu w postaci ich ilosci od poczatkowego adresu 0.0
        /// </returns>
        protected int CalculateBitAdress()
        {
            return 8 * AreaBegining + BitOffset;
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
            return buffer[0] == 1;
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
            return ((bool)objectToCode) ? 1 : 0;
        }

        /// <summary>
        /// Metoda ustawiajaca wartosc zmiennej w sterowniku
        /// </summary>
        /// <param name="value">
        /// Nowa wartosc zmiennej
        /// </param>
        /// <returns>
        /// Zmienna determinujaca czy przebieg zapisywania zmiennej w sterowniku przebiegł poprawnie
        /// </returns>
        public override bool SetValueInPLC(Object value)
        {
            //Jezeli polaczenie nie jest polaczone nie mozna dokonac ustawienia zmiennej w sterowniku
            if (connection.State != StatusType.Connected)
            {
                throw new InvalidOperationException(String.Format("Connection state is {0}", connection.State.ToString()));
            }

            //Nie mozna przypisać zmiennej innego typu, niż zmienna była inicjalizowana
            if (!value.GetType().IsAssignableFrom(internalValue.GetType()))
            {
                throw new InvalidCastException(String.Format("Value of type {0} cannot be set as a object of diffrent type {1}", internalValue.GetType(), value.GetType()));
            }

            //Jezeli inne zadanie ustawiania wartosci jest w trakcie wykonywania nie mozna wykonac nowego zadania
            if (whileSetting)
            {
                return false;
            }

            //Synchronizacja dostepu do polaczenia
            lock (lockingObject)
            {
                whileSetting = true;

                //Pobranie rezultatu przechowujacego kod bledu jezeli cos poszlo nie tak
                int result = connection.libnodaveConnection.writeBits((int)TypeOfMemory, DataBlockNumber, CalculateBitAdress(), DataLength, BitConverter.GetBytes(CodingMethod(value)));

                //Sprawdzenie czy zostal wyslany jakis kod bledu
                if (result != 0)
                {
                    //Jezeli tak to ustawiamy status zmiennej na błędny i zwracamy wyjątek
                    whileSetting = false;
                    SetNewStatus(StatusType.Fault, "Libnodave error: " + libnodave.daveStrerror(result));
                    throw new InvalidProgramException(StateComment);
                }
                else
                {
                    //Jeżeli zmienna w sterowniku udalo sie odswiezyc oznacza to, ze polaczenie z nia jest poprawne
                    SetNewStatus(StatusType.Connected);
                }

                whileSetting = false;
            }

            //Zmienna w sterowniku zostala ustawiona - nalezy wiec teraz odswiezyc wartosc zmiennej na komputerze
            RefreshValue();

            return true;
        }

        /// <summary>
        /// Metoda odswiezajaca wartosc zmiennej, pobierajac ja ze sterownika
        /// </summary>
        /// <returns>
        /// Wartosc zwrocona determinujaca czy udalo sie odswiezyc dana zmienna
        /// </returns>
        public override bool RefreshValue()
        {
            //Jezeli polaczenie nie jest polaczone nie mozna dokonac odswiezenia zmiennej
            if (connection.State != StatusType.Connected)
            {
                throw new InvalidOperationException(String.Format("PLC connection state is {0}", connection.State.ToString()));
            }

            //Jezeli inne zadanie odswiezenia jest w trakcie wykonywania nie ma mozliwosci wykonania nowego
            if (whileRefreshing)
            {
                return false;
            }

            //Synchronizacja dostepu do polaczenia
            lock (lockingObject)
            {
                whileRefreshing = true;

                //Pobranie rezultatu przechowujacego kod bledu jezeli cos poszlo nie tak
                int result = connection.libnodaveConnection.readBits((int)TypeOfMemory, DataBlockNumber, CalculateBitAdress(), DataLength, buffer);

                //Sprawdzenie czy zostal wyslany jakis kod bledu
                if (result != 0)
                {
                    //Jezeli tak to ustawiamy status zmiennej na błędny i zwracamy wyjątek
                    whileRefreshing = false;
                    SetNewStatus(StatusType.Fault, "Libnodave error: " + libnodave.daveStrerror(result));
                    throw new InvalidProgramException(StateComment);
                }
                else
                {
                    //Jeżeli zmienna udalo sie odswiezyc oznacza to ze polaczenie jest poprawne
                    SetNewStatus(StatusType.Connected);
                }

                //Zdekodowanie pobranej tablicy bajtow
                Value = DecodingMethod(buffer, 0);

                whileRefreshing = false;
            }

            //Jezeli udala sie operacja zwracamy wartosc true
            return true;
        }
        /// <summary>
        /// Metoda dodajaca zakres bajtow zmiennej do zadania odczytu obiektu probkujacego
        /// </summary>
        /// <param name="readRequestPDU">
        /// obiekt jednostki danych protokołu 
        /// </param>
        protected internal override void AddSamplerReadRequest(libnodave.PDU readRequestPDU)
        {
            readRequestPDU.addBitVarToReadRequest((int)TypeOfMemory, DataBlockNumber, CalculateBitAdress(), DataLength);
        }

        /// <summary>
        /// Metoda dodajaca zakres bajtow zmiennej do zadania zapisu obiektu probkujacego
        /// </summary>
        /// <param name="writeRequestPDU">
        /// obiekt jednostki danych protokołu 
        /// </param>
        protected internal override void AddSamplerWriteRequest(libnodave.PDU writeRequestPDU)
        {
            writeRequestPDU.addBitVarToWriteRequest((int)TypeOfMemory, DataBlockNumber, CalculateBitAdress(), DataLength, BitConverter.GetBytes(CodingMethod(Value)));
        }

        /// <summary>
        /// Metoda wykonywana za kazdym razem gdy obiekt probkujacy pobierze dane odpowiedzi sterownika i chce aby dana zmienna je poprawnie odczytała
        /// </summary>
        protected internal override void OnSamplerReadRequest()
        {
            //Pobrana przez sampler wartosc nalezy przekonwertowac na byte i sprawdzic czy jego wartosc jest rowna 1
            Value = connection.libnodaveConnection.getU8() == 1;
        }
    }
}
