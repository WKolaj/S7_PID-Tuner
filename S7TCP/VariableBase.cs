using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7TCP
{
 
    /// <summary>
    /// Abstrakcyjna klasa bazowa zmiennej:
    /// Kazda zmienna moze posiadac status - dlatego dziedziczy po klasie bazowej ObjectWithStateBase
    /// </summary>
    public abstract class VariableBase:ObjectWithStateBase
    {
        /// <summary>
        /// Zdarzenie zglaszane za kazdym razem gdy zmiennej przypisywana jest wartosc
        /// Zdarzenie to jest zglaszane rowniez wtedy, gdy wartosc zmiennej nie ulega zmianie - przypisywana jest jej taka sama wartosc jaka zmienna miała poprzednio
        /// </summary>
        public event ValueUpdatedEventHandler ValueUpdated;

        /// <summary>
        /// Zdarzenie zglaszane za kazdym razem gdy wartosc zmiennej ulega zmianie
        /// </summary>
        public event ValueChangedEventHandler ValueChanged;

        /// <summary>
        /// Nazwa zmiennej
        /// W obrebie danego urządzenia nazwa musi być unikalna
        /// </summary>
        protected string name;

        /// <summary>
        /// Nazwa zmiennej
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                lock(lockingObject)
                {
                    name = value;
                }

            }
        }

        /// <summary>
        /// Obiekt próbkujący zmienną
        /// </summary>
        protected SamplerBase sampler;

        /// <summary>
        /// Metoda ustawiajaca nowy obiekt probkujacy dla danej zmiennej
        /// </summary>
        /// <param name="newSampler">
        /// Nowy obiekt probkujacy
        /// </param>
        protected internal void SetSampler(SamplerBase newSampler)
        {
            //Ograniczenie protokolu wymaga, aby obiekt probkujacy nie posiadał więcej, niż 20 zmiennych
            if (newSampler.Variables.Count == 20)
            {
                throw new InvalidOperationException("Maxmium number of variables, assingned to the sampler has been reached (20)");
            }

            //Jezeli zmienna posiada juz obiekt probkujacy nalezy go usunąć
            if(this.sampler != null)
            {
                //Jezeli obiekt probkujacy posiada juz w swojej kolekcji dana zmienna, nie powinno się jej dodawać kolejny raz
                if (newSampler.Variables.Contains(this))
                {
                    throw new InvalidOperationException("Sampler already has this variable");
                }

                //Usuniecie starego obiektu probkującego
                RemoveSampler();
            }

            //Ustawienie nowego obiektu probkujacego i dodanie zmiennej do jego kolekcji
            this.sampler = newSampler;

            this.sampler.Variables.Add(this);
        }

        /// <summary>
        /// Metoda usuwajaca obiekt probkujacy ze zmiennej
        /// </summary>
        public void RemoveSampler()
        {
            if(sampler!=null)
            {
                this.sampler.Variables.Remove(this);
                this.sampler = null;
            }
        }

        /// <summary>
        /// Zmienna zapobiegajaca przed nagromadzeniem sie wielu żądań odświeżenia wartości
        /// Jezeli zdarzenie odswiezania bedzie trwalo długo nie zostanie rozpoczete zdane nowe zadanie odswiezania zanim poprzednie się nie zakończy
        /// </summary>
        protected bool whileRefreshing;

        /// <summary>
        /// Zmienna zapobiegajaca przed nagromadzeniem sie wielu żądań przeslania wartości do sterownika
        /// Jezeli zdarzenie odswiezania bedzie trwalo długo nie zostanie rozpoczete zdane nowe zadanie przeslania wartości do sterownika zanim poprzednie się nie zakończy
        /// </summary>
        protected bool whileSetting;

        /// <summary>
        /// Obiekt blokujący dostep do zmiennej - wykorzystywany przez synchronizujacy monitor
        /// </summary>
        protected object lockingObject;

        /// <summary>
        /// Obiekt połączenia, przez które następuje połączenie ze zmienna w sterowniku
        /// </summary>
        protected Connection connection;

        /// <summary>
        /// Typ pamieci zmiennej w sterowniku
        /// </summary>
        protected MemoryType typeOfMemory;

        /// <summary>
        /// Typ pamieci zmiennej w sterowniku
        /// </summary>
        public MemoryType TypeOfMemory
        {
            get
            {
                return typeOfMemory;
            }
            set
            {
                //Ustawienie typu wymaga synchronizacji
                lock(lockingObject)
                {
                    typeOfMemory = value;
                }
            }
        }

        /// <summary>
        /// Numer databloku zmiennej w sterowniku
        /// </summary>
        protected int dataBlockNumber;

        /// <summary>
        /// Numer databloku zmiennej w sterowniku
        /// </summary>
        public int DataBlockNumber
        {
            get
            {
                return dataBlockNumber;
            }

            set
            {
                //Ustawienie databloku wymaga synchronizacji
                lock(lockingObject)
                {
                    //Jezeli typem wykorzystywanej w sterowniku pamieci nie jest Datablock numer databloku nie ma znaczenia i przymuje wartosc 0
                    if (TypeOfMemory == MemoryType.DB)
                    {
                        //Numer databloku nie moze byc mniejszy od 0
                        if(value <= 0)
                        {
                            dataBlockNumber = 1;
                        }
                        else
                        {
                            dataBlockNumber = value;
                        }
                    }
                    else
                    {
                        dataBlockNumber = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Obszar pamieci od ktorej rozpoczyna się zakres zmiennej
        /// </summary>
        protected int areaBegining;
        public int AreaBegining
        {
            get
            {
                return areaBegining;
            }
            set
            {
                //zmiana obszaru pamieci wymaga synchronizacji
                lock(lockingObject)
                {
                    areaBegining = value;
                }
            }
        }

        /// <summary>
        /// Bufor w ktorym przechowywane są poszczogolne bajty odpowiedzi sterownika
        /// </summary>
        /// Bajty te są następnie konwertowane na na odpowiednia wartosc zmiennej
        protected Byte[] buffer;
        
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
        protected virtual Object DecodingMethod(byte[] buffer, int position)
        {
            return null;
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
        protected virtual int CodingMethod(Object objectToCode)
        {
            return 0;
        }

        /// <summary>
        /// Dlugosc zmiennej w sterowniku wyrazona w bajtach
        /// </summary>
        protected int dataLength;

        /// <summary>
        /// Dlugosc zmiennej w sterowniku wyrazona w bajtach
        /// </summary>
        protected int DataLength
        {
            get
            {
                return dataLength;
            }
            private set
            {
                //Dlugosc taka nie moze byc ani mniejsza ani rowna 0
                if(value <= 0 )
                {
                    dataLength = 1;
                    buffer = new byte[1];
                }
                else
                {
                    dataLength = value;
                    buffer = new byte[value];
                }
            }
        }

        /// <summary>
        /// wartosc zmiennej
        /// </summary>
        protected object internalValue;

        /// <summary>
        /// Wartość zmiennej
        /// </summary>
        public object Value
        {
            get
            {
                return internalValue;
            }

            protected set
            {
                //Zmienna nie zostanie zmieniona jeżeli użytkownik bedzie jej przypisywal te samą wartość
                if(!internalValue.Equals(value))
                {
                    internalValue = value;

                    //Zgloszenie zdarzenia informujacego o zmianie zmiennej
                    if (ValueChanged != null)
                    {
                        ValueChanged(this, new ValueChangedEventArgument(internalValue));
                    }
                }

                //Zgloszenie zdarzenia o probie przypisania wartosci do zmiennej
                if(ValueUpdated!=null)
                {
                    ValueUpdated(this, new ValueUpdatedEventArgument(internalValue));
                }
            }
        }

        /// <summary>
        /// Konstruktor obiektu zmiennej
        /// </summary>
        /// <param name="name">
        /// Nazwa zmiennej
        /// </param>
        /// <param name="connectionObject">
        /// Obiekt polaczenia wykorzystywanego przez zmienna
        /// </param>
        /// <param name="memoryType">
        /// Typ pamięci sterownika w której przechowywana jest zmienna
        /// </param>
        /// <param name="dataBlockNumber">
        /// Numer databloku w którym przechowywana jest zmienna
        /// (Istotny tylko w przypadku, gdy zmienna wykorzystuje jako typ pamieci databloki)
        /// </param>
        /// <param name="areaBegining">
        /// Poczatek zakresu pamieci, od ktorej rozpoczyna sie zmienna
        /// </param>
        /// <param name="dataLength">
        /// Dlugosc zmiennej
        /// </param>
        /// <param name="commonLockingObject">
        /// Wspolny obiekt synchronizujacy dostep do polaczenia sterownika
        /// </param>
        protected VariableBase(string name, Connection connectionObject, MemoryType memoryType, int dataBlockNumber, int areaBegining, int dataLength, Object commonLockingObject):
            base()
        {
            //Sprawdzam argumenty podane do konstruktora
            if (commonLockingObject == null)
            {
                SetNewStatus(StatusType.Fault, "Locking object cannont be set as null");
                throw new ArgumentNullException("lockingObject", StateComment);
            }

            if (String.IsNullOrEmpty(name))
            {
                SetNewStatus(StatusType.Fault, "Name cannont be set as null or empty string");
                throw new ArgumentNullException("name", StateComment);
            }

            if (connectionObject == null)
            {
                SetNewStatus(StatusType.Fault, "Connection object cannot be set to null");
                throw new ArgumentNullException("ConnectionObject", StateComment);
            }

            //Inicjalizuje zmienne

            this.lockingObject = commonLockingObject;
            this.name = name;

            //Wazne aby typ byl przed numerem bloku - dopiero gdy zmienna wie, ze jest datablokiem moze przyjac odpowiedni numer db
            this.typeOfMemory = memoryType;
            this.dataBlockNumber = dataBlockNumber;
            this.areaBegining = areaBegining;

            this.connection = connectionObject;
            //Od razu dokonywana jest inicjalizacja bufora o dlugosci dataLength
            this.DataLength = dataLength;
            
        }

        /// <summary>
        /// Zsynchronizowana metoda pozwalajaca na zmiane wartosci zmiennej
        /// </summary>
        /// <param name="newValue">
        /// Nowa wartosc zmiennej
        /// </param>
        public void SetValue(object newValue)
        {
            //Nie mozna przypisać zmiennej innego typu, niż zmienna była inicjalizowana
            if(!Value.GetType().IsAssignableFrom(newValue.GetType()))
            {
                SetNewStatus(StatusType.Fault, String.Format("Value of type {0} cannot be set as a object of diffrent type {1}", Value.GetType(), newValue.GetType()));
                throw new InvalidCastException(StateComment);
            }

            //zsynchronizowane ustawienie nowej wartosci zmiennej
            lock (lockingObject)
            {
                Value = newValue;
            }

        }

        /// <summary>
        /// Metoda odswiezajaca wartosc zmiennej, pobierajac ja ze sterownika
        /// </summary>
        /// <returns>
        /// Wartosc zwrocona determinujaca czy udalo sie odswiezyc dana zmienna
        /// </returns>
        public virtual bool RefreshValue()
        {
            //Jezeli polaczenie nie jest polaczone nie mozna dokonac odswiezenia zmiennej
            if (connection.State != StatusType.Connected)
            {
                throw new InvalidOperationException(String.Format("Connection connection state is {0}",connection.State.ToString()));
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
                int result = connection.libnodaveConnection.readBytes((int)TypeOfMemory, DataBlockNumber, AreaBegining, DataLength, buffer);

                //Sprawdzenie czy zostal wyslany jakis kod bledu
                if(result != 0)
                {
                    //Jezeli tak to ustawiamy status zmiennej na błędny i zwracamy wyjątek
                    SetNewStatus(StatusType.Fault, "Libnodave error: " + libnodave.daveStrerror(result));
                    whileRefreshing = false;
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
        /// Metoda ustawiajaca wartosc zmiennej w sterowniki po czym odswiezajaca wartosc zmiennej
        /// </summary>
        /// <param name="value">
        /// Wartosc zmiennej ktora ma byc ustawiona
        /// </param>
        /// <returns>
        /// Wartosc determinujaca czy ustawienie wartosci zmiennej w sterowniku zostalo wykonane poprawnie
        /// </returns>
        public virtual bool SetValueInPLC(Object value)
        {
            //Jezeli polaczenie nie jest polaczone nie mozna dokonac ustawienia zmiennej w sterowniku
            if (connection.State != StatusType.Connected)
            {
                throw new InvalidOperationException(String.Format("PLC connection state is {0}", connection.State.ToString()));
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
                int result = connection.libnodaveConnection.writeBytes((int)TypeOfMemory, DataBlockNumber, AreaBegining, DataLength, BitConverter.GetBytes(CodingMethod(value)));

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
        /// Metoda dodajaca zakres bajtow zmiennej do zadania odczytu obiektu probkujacego
        /// </summary>
        /// <param name="readRequestPDU">
        /// obiekt jednostki danych protokołu 
        /// </param>
        protected internal virtual void AddSamplerReadRequest(libnodave.PDU readRequestPDU)
        {
            readRequestPDU.addVarToReadRequest((int)TypeOfMemory, dataBlockNumber, AreaBegining, DataLength);
        }

        /// <summary>
        ///  Metoda dodajaca zakres bajtow zmiennej do zadania zapisu obiektu probkujacego
        /// </summary>
        /// <param name="writeRequestPDU">
        /// obiekt jednostki danych protokołu 
        /// </param>
        protected internal virtual void AddSamplerWriteRequest(libnodave.PDU writeRequestPDU)
        {
            writeRequestPDU.addVarToWriteRequest((int)TypeOfMemory, dataBlockNumber, AreaBegining, DataLength, BitConverter.GetBytes(CodingMethod(Value)));
        }

        /// <summary>
        /// Metoda wykonywana za kazdym razem gdy obiekt probkujacy pobierze dane odpowiedzi sterownika i chce aby dana zmienna je poprawnie odczytała
        /// </summary>
        protected internal virtual void OnSamplerReadRequest()
        {

        }
        
    }
}
