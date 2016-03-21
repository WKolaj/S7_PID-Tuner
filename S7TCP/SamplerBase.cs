using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace S7TCP
{
    /// <summary>
    /// Klasa bazowa obiektu probkujacego zmienne
    /// </summary>
    public abstract class SamplerBase: ObjectWithStateBase, IEnumerable<VariableBase>
    {
        /// <summary>
        /// Obiekt jednostki polaczenia wykorzystywana do tworzenia zadan dla uzyskania do 20 zmiennych
        /// </summary>
        protected libnodave.PDU pduObject;

        /// <summary>
        /// Obiekt resultatu dzieki ktorym mozliwy jest dostep do bajtow otrzymanych od sterownika
        /// </summary>
        protected libnodave.resultSet resultObject = new libnodave.resultSet();

        /// <summary>
        /// Czas probkowania [ms]
        /// </summary>
        protected int sampleTime;

        /// <summary>
        /// Czas probkowania [ms]
        /// </summary>
        public int SampleTime
        {
            get
            {
                return sampleTime;
            }
            set
            {
                //Kazda zmiana czasu probkowania wymaga Restartu i zmiany interwału timera probkujacego
                lock(lockingObject)
                {
                    timer.Stop();
                    timer.Period = value;
                    timer.Start();
                    sampleTime = value;
                }
            }
        }

        /// <summary>
        /// Zmienna pozwalajaca na blokade przed nakladajacymi sie zadaniami obslugi zdarzenia probkowania timera
        /// </summary>
        public bool InProgress
        {
            get;
            protected set;
        }

        /// <summary>
        /// Obiekt blokujacy przed jednoczesnym dostepem do polaczenia kilku obiektow
        /// </summary>
        protected object lockingObject;

        /// <summary>
        /// Obiekt polaczenia
        /// </summary>
        protected Connection connection;

        /// <summary>
        /// Probkujacy z bardzo duza dokladnoscia timer
        /// </summary>
        protected Timer timer = new Timer();

        /// <summary>
        /// Nazwa obiektu probkujacego
        /// </summary>
        protected string name;
        public string Name
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
        /// Kolekcja zmiennych ktore nalezy probkowac
        /// </summary>
        protected List<VariableBase> variables = new List<VariableBase>();

        /// <summary>
        /// Kolekcja zmiennych ktore nalezy probkowac
        /// </summary>
        protected internal List<VariableBase> Variables
        {
            get
            {
                return variables;
            }

            set
            {
                variables = value;
            }

        }

        /// <summary>
        /// Meotoda wywolywana co kazdy takt timera taktujacego
        /// </summary>
        private void TimerTick(object sender,EventArgs e)
        {
            
            //Metoda 
            if (connection.State == StatusType.Connected)
            {
                if (Variables.Count == 0)
                {
                    return;
                }

                onTimerTick();
            }
        }

        /// <summary>
        /// Metoda wywolywana co kazdy takt timera taktujacego - w tej metodzie nalezy zawiera sie cala obsluga timera
        /// </summary>
        protected virtual void onTimerTick()
        {

        }

        /// <summary>
        /// Metoda rozpoczynajaca probkowanie
        /// </summary>
        public void Start()
        {
            timer.Start();
            SetNewStatus(StatusType.Connected);
        }

        /// <summary>
        /// Metoda zatrzymujaca probkowanie
        /// </summary>
        public void Stop()
        {
            lock(lockingObject)
            {
                timer.Stop();
                SetNewStatus(StatusType.Disconnected);

                //Status wszystkich zmiennych rowniez zmienia sie na Disconnected
                foreach(var variable in variables)
                {
                    variable.SetNewStatus(StatusType.Disconnected);
                }
            }
        }

        /// <summary>
        /// Konstruktor obiektu probkujacego
        /// </summary>
        /// <param name="name">
        /// Nazwa obiektu probkujacego
        /// </param>
        /// <param name="connectionObject">
        /// Nazwa obiektu polaczenia
        /// </param>
        /// <param name="sampleTime">
        /// Czas probkowania [ms]
        /// </param>
        /// <param name="lockingObject">
        /// Obiekt synchronizujacy dostep do polaczenia
        /// </param>
        public SamplerBase(string name, Connection connectionObject, int sampleTime, Object lockingObject):
            base()
        {
            //Sprawdzam podane argumenty
            if (lockingObject == null) throw new NullReferenceException("lockingObject cannot be set to null");
            if (connectionObject == null) throw new NullReferenceException("connectionObject cannot be set to null");

            //Inicjalizuje pola
            this.lockingObject = lockingObject;
            this.connection = connectionObject;

            this.name = name;
            timer.Period = sampleTime;
            this.sampleTime = sampleTime;

            //Inicjalizuje timer
            InitTimer();

        }

        /// <summary>
        /// Metoda inicjalizujaca timer probkujacy
        /// </summary>
        private void InitTimer()
        {
            //Ustawiam najwieksza z mozliwych dokladnosci
            timer.Resolution = 1;

            //Dodaje metode obslugi timera
            timer.Tick += TimerTick;
        }

        /// <summary>
        /// Metoda dodajaca zmienna do probkowanej kolekcji
        /// </summary>
        /// <param name="variable">
        /// Nazwa zmiennej
        /// </param>
        protected internal void Add(VariableBase variable)
        {
            variable.SetSampler(this);
        }

        /// <summary>
        /// Metoda usuwajaca zmienna z probkowanej kolekcji
        /// </summary>
        /// <param name="variable"></param>
        protected internal void Remove(VariableBase variable)
        {
            //Usuwanie wymaga synchronizacji - aby nie usunac zmiennej ktora wlasnie jest probkowana
            lock(lockingObject)
            {
                variable.RemoveSampler();
            }
        }

        /// Metoda zwracajaca Enumerator kolekcji zmiennych obiektu probkujacego
        /// </summary>
        /// <returns>
        /// Enumerator kolekcji zmiennych obiektu probkujacego
        /// </returns>
        public IEnumerator<VariableBase> GetEnumerator()
        {
            return new VariableBaseEnumerator(Variables);
        }

        /// Metoda zwracajaca Enumerator kolekcji zmiennych obiektu probkujacego
        /// </summary>
        /// <returns>
        /// Enumerator kolekcji zmiennych obiektu probkujacego
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new VariableBaseEnumerator(Variables);
        }

        /// <summary>
        /// Iterator obiektu probkujacego
        /// </summary>
        /// <param name="index">
        /// Index zmiennej
        /// </param>
        /// <returns>
        /// Zmienna o indeksie index
        /// </returns>
        public VariableBase this[int index]
        {
            get
            {
                return Variables[index];
            }
            protected internal set
            {
                Variables[index] = value;
            }
        } 
    }

    /// <summary>
    /// Klasa enumeratora zmiennej
    /// </summary>
    public class VariableBaseEnumerator : IEnumerator<VariableBase>
    {
        protected int currentIndex = -1;

        protected List<VariableBase> variables;

        public VariableBaseEnumerator(List<VariableBase> variablesList)
        {
            this.variables = variablesList;
        }

        public VariableBase Current
        {
            get
            {
                if(this.currentIndex == -1)
                {
                    throw new InvalidOperationException("Use MoveNext before calling Current");
                }

                return variables.ElementAt(currentIndex);
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

                return variables.ElementAt(currentIndex);
            }
        }

        public bool MoveNext()
        {
            currentIndex++;

            if(currentIndex >= variables.Count)
            {
                currentIndex = variables.Count - 1;
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
