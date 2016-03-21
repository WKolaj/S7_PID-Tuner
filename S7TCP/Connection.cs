using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7TCP
{

    /// <summary>
    /// Klasa obiektu polaczenia ze sterownikiem
    /// </summary>
    public class Connection:ObjectWithStateBase
    {
        /// <summary>
        /// Obiekt blokujący przed kilkukrotna proba uzyskania polaczenia na raz
        /// </summary>
        protected bool startedTryingToConnect = false;

        /// <summary>
        /// Obiekt blokujacy dostep do zmiany parametrow polaczenia podczas jego konfiguracji
        /// </summary>
        Object connectionLockingObject = new Object();

        /// <summary>
        /// Obiekt polaczenia biblioteki Libnodave
        /// </summary>
        protected internal libnodave.daveConnection libnodaveConnection;

        /// <summary>
        /// Obiekt interfejsu biblioteki libnodave
        /// </summary>
        protected internal libnodave.daveInterface libnodaveInterface;

        /// <summary>
        /// Obiekt glowny biblioteki libnodave
        /// </summary>
        protected internal libnodave.daveOSserialType libnodaveObject;

        /// <summary>
        /// Numer szyny urzadzenia
        /// </summary>
        protected internal int rack;

        /// <summary>
        /// Numer slotu urzadzenia
        /// </summary>
        protected internal int slot;

        /// <summary>
        /// Adres ip urzadzenia
        /// </summary>
        protected internal string ipAdress;

        /// <summary>
        /// Czas po ktorym biblioteki libnodave zamyka polaczenie jezeli nie udaje sie jej go nawiazac
        /// </summary>
        protected internal int timeout;

        /// <summary>
        /// Unikalna nazwa polaczenia
        /// </summary>
        protected internal string connectionName;

        /// <summary>
        /// Timer próbkujący metode odnawiajaca polaczenie w przypadku jego zerwania
        /// </summary>
        protected System.Timers.Timer connector ;

        /// <summary>
        /// Metoda inicjalizujaca timer probkujacy polaczenie
        /// </summary>
        protected void InitializeConnector()
        {
            connector = new System.Timers.Timer();
            //Czas odnawiania polaczenia jest ustawiony "na sztywno" jako 1000
            connector.Interval = 1000;
            connector.AutoReset = true;
            connector.Enabled = true;
            connector.Stop();
            connector.Elapsed += OnConnectorRequest;
        }

        /// <summary>
        /// Metoda wznawiajaca probkowanie polaczenia
        /// </summary>
        protected void StartConnector()
        {
                connector.Start();
        }

        /// <summary>
        /// Metoda zatrzymujaca probkowanie polaczenia
        /// </summary>
        protected void StopConnector()
        {
                connector.Stop();
        }

        /// <summary>
        /// Zdarzenie zglaszane, co kazdy cykl odnawiania polaczenia
        /// </summary>
        /// <param name="sender">
        /// Obiekt zglaszajacy zdarzenie
        /// </param>
        /// 
        /// <param name="argument">
        /// Argument zdarzenia
        /// </param>
        /// 
        protected void OnConnectorRequest(Object sender, EventArgs argument)
        {
            //Metoda ta jest wykonywana co kazdy cykl proby odnowienie polaczenia 
            //Jezeli nadal istnieje proba polaczenia ze sterownikiem nie rozpoczynamy nastepnej
            if(startedTryingToConnect)
            {
                return;
            }

            //Proba uzyskania polaczenia - wymaga synchronizacji
            lock(connectionLockingObject)
            {
                startedTryingToConnect = true;

                //Jezeli stan polaczenia jest Ok nalezy wylaczyc probkowanie odnawiania polaczenia - jest juz ono nie potrzebne
                if (State == StatusType.Connected)
                {
                    StopConnector();

                    startedTryingToConnect = false;

                    return;
                }

                //Reinicjalizacja połączenia
                ReinitializeConnection();

                //Jezeli udalo sie polaczyc nalezy zakonczyc probkowanie odnawiania polaczenia - jest juz ono nie potrzebne
                if(TryToConnect())
                {
                    StopConnector();
                }

                startedTryingToConnect = false;
            }
        }

       /// <summary>
       /// Zwolenienie zasobow polaczenia
       /// </summary>
        protected void ReleaseResources()
        {
            //Mozliwe jest zwolnienie zasobow pod warunkiem ze zostaly one wczesniej stworzone
            if (libnodaveConnection != null)
            {
                //Zwolnenie zasobow polaczenia i interfejsu polaczenia
                libnodaveConnection.freePointer();
                libnodaveInterface.freePointer();
            }
        }

        /// <summary>
        /// Metoda kończąca połączenie ze sterownikiem
        /// </summary>
        protected void DisconnectConnection()
        {
            if (libnodaveConnection != null)
            {
                //Rozlaczenie polaczenia
                libnodaveConnection.disconnectPLC();
                libnodaveInterface.disconnectAdapter();

                //Zamkniecie gniazda polaczenia
                int result = libnodave.closeSocket(libnodaveObject.rfd);
            }
        }


        /// <summary>
        /// Destruktor klasy connection - nalezy zwolnic zasoby obiektu tej klasy
        /// </summary>
        ~Connection()
        {
            DisconnectConnection();
            ReleaseResources();
        }
        
        
        /// <summary>
        /// Metoda reinicjalizujaca zasoby polaczenia
        /// </summary>
        protected void ReinitializeConnection()
        {
            //Rozlaczenie starego polaczenia
            DisconnectConnection();

            //Zwolenienie wczesniejszych zasobow polaczenia
            ReleaseResources();

            //Stworzenie i inicjalizacja nowych obiektow polaczenia libnodave
            InitNewConnectionObjects();

        }

        /// <summary>
        /// Stworzenie i inicjalizacja nowych obiektow polaczenia
        /// </summary>
        protected void InitNewConnectionObjects()
        {
            //Stworzenie nowych zasobow polaczenia, zgodnie z dokumentacja biblioteki libnodave
            libnodaveObject = new libnodave.daveOSserialType();
            libnodaveObject.rfd = libnodave.openSocket(102, ipAdress);
            libnodaveObject.wfd = libnodaveObject.rfd;
            libnodaveInterface = new libnodave.daveInterface(libnodaveObject, connectionName, 0, libnodave.daveProtoISOTCP, libnodave.daveSpeed500k);
            libnodaveInterface.setTimeout(timeout);
            libnodaveConnection = new libnodave.daveConnection(libnodaveInterface, 0, rack, slot);
        }

        /// <summary>
        /// Metoda proby uzyskania polaczenia
        /// </summary>
        /// <returns>
        /// Wartosc determinujaca, czy udalo sie dokonac polaczenia ze sterownikiem
        /// </returns>
        protected bool TryToConnect()
        {
            //Proba uzyskania polaczenia
            int result = libnodaveConnection.connectPLC();
            //Jezeli zwrocony zostal kod bledow połączenie należy ustawić w stan Faulted sygnalizujacy ze nie udalo sie polaczyc
            //Oraz uniemozliwajacy innym mechanizmom dostep do tego polaczenia
            if(result != 0)
            {
                //Console.WriteLine("Libnodave error: " + libnodave.daveStrerror(result));
                SetNewStatus(StatusType.Fault,"Cannot connect to device");
                
                return false;
            }
            //Ustawienie nowego stanu, w przypadku gdy proces polaczenia przebiegl poprawnie
            SetNewStatus(StatusType.Connected);
            return true;
        }

        /// <summary>
        /// Metoda wznawiajaca polaczenie
        /// </summary>
        public void StartConnection()
        {
            //Wznowienie polaczenia ogranicza sie tylko do wznowienia probkowania metoda probujaca je wznowic
            StartConnector();
        }

        /// <summary>
        /// Metoda ustawiajaca stan polaczenia na Fault
        /// </summary>
        /// <param name="errorDetails">
        /// Komentarz wystepujacego bledu
        /// </param>
        public void SetStatusToFalut(string errorDetails)
        {
            //Ustawienie stanu na Fault wymaga wczesniejszej synchronizacji - aby zapobiec sytuacji w ktorej kilka zmiennych probuje ja na raz ustawic na Fault
            lock(connectionLockingObject)
            {
                SetNewStatus(StatusType.Fault, errorDetails);

                StartConnector();
            }
        }

        /// <summary>
        /// Metoda zatrzymujaca polaczenie
        /// </summary>
        public void StopConnection()
        {
            //Nalezy upewnic sie czy inne zasoby nie korzystaja z tego polaczenia
            lock(connectionLockingObject)
            {
                //Jezeli jest wlaczony connector nalezy go wylaczyc
                StopConnector();

                //Jezeli stan polaczenia juz jest rozlaczony nie trzeba nic robic
                if (State == StatusType.Disconnected)
                {
                    return;
                }

                //Jezeli stan polaczenia jest bledny wystarczy jedynie zmienic jego stan na rozlaczony
                if (State == StatusType.Fault)
                {
                    State = StatusType.Disconnected;
                    return;
                }

                //Zatrzymanie polaczenia
                libnodaveConnection.disconnectPLC();

                //Ustawienie jego statusu jako rozlaczony
                SetNewStatus(StatusType.Disconnected);

                //Zwolenieni wszystkich innych zasobow zwiazanych z polaczeniem odbedzie sie podczas jego nastepnej proby polaczenia
            }
        }

        /// <summary>
        /// Konstruktor klasy polaczenia
        /// </summary>
        /// <param name="name">
        /// Unikalna nazwa polaczenia
        /// </param>
        public Connection(string name)
        {
            //Inicjalizacja nazwy i obiektu probkujacego polaczenie
            this.connectionName = name;
            InitializeConnector();
        }

        /// <summary>
        /// Metoda sprawdzajaca adres IP
        /// </summary>
        /// <param name="ipText">
        /// Adres IP
        /// </param>
        /// <returns>
        /// Wartosc determinujaca czy adres ip zostal wpisany poprawnie
        /// </returns>
        public static bool CheckIPAdress(string ipText)
        {
            string[] ipOctets = ipText.Split('.');

            if (ipOctets.Length != 4)
            {
                return false;
            }

            int tempValue;

            foreach (var octet in ipOctets)
            {
                if (!int.TryParse(octet, out tempValue))
                {
                    return false;
                }

                if (tempValue < 0 || tempValue > 255)
                {
                    return false;
                }
            }

            return true;

        }
    }


}
