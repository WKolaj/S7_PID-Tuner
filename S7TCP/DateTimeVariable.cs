using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7TCP
{
    /// <summary>
    /// Klasa zmiennej typu DateTime
    /// </summary>
    public class DateTimeVariable: VariableBase
    {
        //Tymczasowe zmienne przechowywane w celu odtworzenia zmiennej typu DateTIme do odczytu
        protected int yearRead;
        protected int monthRead;
        protected int dayRead;
        protected int weekDayRead;
        protected int hourRead;
        protected int minuteRead;
        protected int secondRead;
        protected int nanosecondRead;

        //Tymczasowe zmienne przechowywane w celu odtworzenia zmiennej typu DateTIme do odczytu cyklicznego
        protected int yearCyclicRead;
        protected int monthCyclicRead;
        protected int dayCyclicRead;
        protected int weekDayCyclicRead;
        protected int hourCyclicRead;
        protected int minuteCyclicRead;
        protected int secondCyclicRead;
        protected int nanosecondCyclicRead;

        /// <summary>
        /// Bufor w ktorym zapisywane są bajty które mają być przesyłane sterownika
        /// </summary>
        byte[] bufferToWrite = new byte[14];

        /// <summary>
        /// Bufor w ktorym zapisywane są bajty które mają być przesyłane sterownika podczas zapisu cyklicznego
        /// </summary>
        byte[] bufferToCyclicWrite = new byte[14];

        /// <summary>
        /// Konstruktor zmiennej typu DateTime
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
        public DateTimeVariable(string Name, Connection connectionObject, MemoryType memoryType, int areaBegining, Object commonLockingObject, int dataBlockNumber = 0) :
            base(Name, connectionObject, memoryType, dataBlockNumber, areaBegining, 12, commonLockingObject)
        {
            //Inicjalizacja zmiennych
            internalValue = new DateTime();

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
            yearRead = libnodave.getU16from(buffer, 0);
            monthRead = (int)buffer[2];
            dayRead = (int)buffer[3];
            weekDayRead = (int)buffer[4];
            hourRead = (int)buffer[5];
            minuteRead = (int)buffer[6];
            secondRead = (int)buffer[7];
            nanosecondRead = (int)libnodave.getU32from(buffer, 8);

            return new DateTime(yearRead, monthRead, dayRead, hourRead, minuteRead, secondRead, nanosecondRead / 1000000);
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
        override public bool SetValueInPLC(Object value)
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

                DateTime convertedValue = (DateTime)value;
                
                //Przekopowianie zawartosci przygotowanych do wsylania do PLC danych do bufora do zapisu
                Array.Copy(BitConverter.GetBytes(libnodave.daveSwapIed_16(convertedValue.Year)), 0, bufferToWrite, 0, 2);
                Array.Copy(BitConverter.GetBytes((convertedValue.Month)), 0, bufferToWrite, 2, 1);
                Array.Copy(BitConverter.GetBytes((convertedValue.Day)), 0, bufferToWrite, 3, 1);
                Array.Copy(BitConverter.GetBytes(((int)convertedValue.DayOfWeek + 1)), 0, bufferToWrite, 4, 1);
                Array.Copy(BitConverter.GetBytes((convertedValue.Hour)), 0, bufferToWrite, 5, 1);
                Array.Copy(BitConverter.GetBytes((convertedValue.Minute)), 0, bufferToWrite, 6, 1);
                Array.Copy(BitConverter.GetBytes((convertedValue.Second)), 0, bufferToWrite, 7, 1);
                Array.Copy(BitConverter.GetBytes(libnodave.daveSwapIed_32(convertedValue.Millisecond * 1000000)), 0, bufferToWrite, 8, 4);

                //Pobranie rezultatu przechowujacego kod bledu jezeli cos poszlo nie tak
                int result = connection.libnodaveConnection.writeBytes((int)TypeOfMemory,dataBlockNumber , AreaBegining, DataLength, bufferToWrite);

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
        /// Metoda dodajaca zakres bajtow zmiennej do zadania zapisu obiektu probkujacego
        /// </summary>
        /// <param name="writeRequestPDU">
        /// obiekt jednostki danych protokołu 
        /// </param>
        protected internal override void AddSamplerWriteRequest(libnodave.PDU writeRequestPDU)
        {
            DateTime convertedValue = (DateTime)Value;

            //Przygotowanie tablicy bajtow ktora ma byc wyslana
            Array.Copy(BitConverter.GetBytes(libnodave.daveSwapIed_16(convertedValue.Year)), 0, bufferToCyclicWrite, 0, 2);
            Array.Copy(BitConverter.GetBytes((convertedValue.Month)), 0, bufferToCyclicWrite, 2, 1);
            Array.Copy(BitConverter.GetBytes((convertedValue.Day)), 0, bufferToCyclicWrite, 3, 1);
            Array.Copy(BitConverter.GetBytes(((int)convertedValue.DayOfWeek + 1)), 0, bufferToCyclicWrite, 4, 1);
            Array.Copy(BitConverter.GetBytes((convertedValue.Hour)), 0, bufferToCyclicWrite, 5, 1);
            Array.Copy(BitConverter.GetBytes((convertedValue.Minute)), 0, bufferToCyclicWrite, 6, 1);
            Array.Copy(BitConverter.GetBytes((convertedValue.Second)), 0, bufferToCyclicWrite, 7, 1);
            Array.Copy(BitConverter.GetBytes(libnodave.daveSwapIed_32(convertedValue.Millisecond * 1000000)), 0, bufferToCyclicWrite, 8, 4);

            //Dodanie do zadania tablic bajtow ktora ma byc wyslana
            writeRequestPDU.addVarToWriteRequest((int)TypeOfMemory, dataBlockNumber, AreaBegining, DataLength, bufferToCyclicWrite);
        }

        /// <summary>
        /// Metoda wykonywana za kazdym razem gdy obiekt probkujacy pobierze dane odpowiedzi sterownika i chce aby dana zmienna je poprawnie odczytała
        /// </summary>
        protected internal override void OnSamplerReadRequest()
        {
            //Przygotowanie wartosci ktore zostaly odczytane
            yearCyclicRead = connection.libnodaveConnection.getU16At(0);
            monthCyclicRead = connection.libnodaveConnection.getS8At(2);
            dayCyclicRead = connection.libnodaveConnection.getS8At(3);
            weekDayCyclicRead = connection.libnodaveConnection.getS8At(4);
            hourCyclicRead = connection.libnodaveConnection.getS8At(5);
            minuteCyclicRead = connection.libnodaveConnection.getS8At(6);
            secondCyclicRead = connection.libnodaveConnection.getS8At(7);
            nanosecondCyclicRead = connection.libnodaveConnection.getU32At(8);

            //Stworzenie nowego obiektu DateTime i przypisanie go wartosci Value
            Value = new DateTime(yearCyclicRead, monthCyclicRead, dayCyclicRead, hourCyclicRead, minuteCyclicRead, secondCyclicRead, nanosecondCyclicRead / 1000000);
            
        }
    }
}
