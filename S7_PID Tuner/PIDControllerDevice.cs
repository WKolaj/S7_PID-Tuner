using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S7TCP;
using TransferFunctionLib;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows.Data;
using System.Globalization;

namespace S7_PID_Tuner
{
    /// <summary>
    /// Klasa obiektu sterownika - do polaczenia sie z fizycznym urzadzeniem
    /// </summary>
    public class PIDControllerDevice : INotifyPropertyChanged
    {
        /// <summary>
        /// Zmienna okreslajaca czy obiekt zostal juz zinicjalizowany
        /// </summary>
        private bool initiallized = false;

        /// <summary>
        /// Konstruktor klasy obiektu sterownika do laczenia sie z fizycznym urzadzeniem
        /// </summary>
        /// <param name="ipAdress">
        /// Adres ip sterownika
        /// </param>
        /// <param name="rack">
        /// Kaseta sterownika
        /// </param>
        /// <param name="slot">
        /// Gniazdo sterownika
        /// </param>
        /// <param name="sampleTime">
        /// Czas probkowania sluzacy do odswiezania wartosci sterownika w aplikacji
        /// </param>
        /// <param name="dbNumber">
        /// Numer Data Bloku
        /// </param>
        /// <param name="tiForNonI">
        /// Wartosc zastepcza stalej calkowania dla algorytmow bez czlonu calkujacego
        /// </param>
        /// <param name="pidSampleTime">
        /// Czas probkowania algorytmu PID sterownika
        /// </param>
        public PIDControllerDevice(string ipAdress, int rack, int slot, int sampleTime, int dbNumber, Double tiForNonI, Double pidSampleTime)
        {
            tcpDevice = new S7Device("PIDController", ipAdress, rack, slot);
            DBNumber = dbNumber;
            TiForNonI = tiForNonI;
            PIDSampleTime = pidSampleTime;

            //Inicjalizacka sterownika
            InitDevice(sampleTime);
        }

        /// <summary>
        /// Metoda inicjalizujaca sterownik - a raczej obiekt klasy S7TCP
        /// </summary>
        /// <param name="sampleTime"></param>
        public void InitDevice(int sampleTime)
        {
            //Stworzenie dwoch obiektow probkujacych - kazdy dla odpowiedniego trybu
            tcpDevice.AddNewSampler("TuningModeSampler", SamplerType.Reader, sampleTime);
            tcpDevice.AddNewSampler("NormalModeSampler", SamplerType.Reader, 500);

            //Zmienne odswiezane w trybie normalnym
            tcpDevice.AddNewVariable("KpTunning", VariableType.Float, MemoryType.DB, 92, dbNumber);
            tcpDevice.AddNewVariable("TiTunning", VariableType.Float, MemoryType.DB, 96, dbNumber);
            tcpDevice.AddNewVariable("TdTunning", VariableType.Float, MemoryType.DB, 100, dbNumber);
            tcpDevice.AddNewVariable("NTunning", VariableType.Float, MemoryType.DB, 104, dbNumber);
            tcpDevice.AddNewVariable("TpTunning", VariableType.Float, MemoryType.DB, 108, dbNumber);
            tcpDevice.AddNewVariable("InvertedTuning", VariableType.Bool, MemoryType.DB, 114, dbNumber, 1);
            tcpDevice.AddNewVariable("DerivativeActionWeightTuning", VariableType.Float, MemoryType.DB, 116, dbNumber);
            tcpDevice.AddNewVariable("ProportionalActionWeightTuning", VariableType.Float, MemoryType.DB, 120, dbNumber);
            
            //Zmienne odswiezane w trybie tuning
            tcpDevice.AddNewVariable("Tuning", VariableType.Bool, MemoryType.DB, 124, dbNumber, 0);
            tcpDevice.AddNewVariable("Setpoint", VariableType.Float, MemoryType.DB, 74, dbNumber);
            tcpDevice.AddNewVariable("Input", VariableType.Float, MemoryType.DB, 78, dbNumber);
            tcpDevice.AddNewVariable("Output", VariableType.Float, MemoryType.DB, 82, dbNumber);
            tcpDevice.AddNewVariable("ModeTunning", VariableType.Int16, MemoryType.DB, 112, dbNumber);
            tcpDevice.AddNewVariable("ManualValueTunning", VariableType.Float, MemoryType.DB, 88, dbNumber);

            //Pozostale zmienne
            tcpDevice.AddNewVariable("SetTuning", VariableType.Bool, MemoryType.DB, 72, dbNumber, 0);
            tcpDevice.AddNewVariable("ResetTuning", VariableType.Bool, MemoryType.DB, 72, dbNumber, 1);
            tcpDevice.AddNewVariable("ManualEnableTunning", VariableType.Bool, MemoryType.DB, 86, dbNumber, 0);
            tcpDevice.AddNewVariable("BlockConstant", VariableType.Int16, MemoryType.DB, 70, dbNumber);
            tcpDevice.AddNewVariable("ResetTunning", VariableType.Bool, MemoryType.DB, 114, dbNumber, 0);
            tcpDevice.AddNewVariable("EnableTuning", VariableType.Bool, MemoryType.DB, 0, dbNumber, 0);

            //Przypisanie zmiennych do obiektu probkujacego
            tcpDevice.AssignSamplerToVariable("ModeTunning", "TuningModeSampler");
            tcpDevice.AssignSamplerToVariable("Tuning", "TuningModeSampler");
            tcpDevice.AssignSamplerToVariable("Setpoint", "TuningModeSampler");
            tcpDevice.AssignSamplerToVariable("Input", "TuningModeSampler");
            tcpDevice.AssignSamplerToVariable("Output", "TuningModeSampler");
            tcpDevice.AssignSamplerToVariable("ManualValueTunning", "TuningModeSampler");

            tcpDevice.AssignSamplerToVariable("KpTunning", "NormalModeSampler");
            tcpDevice.AssignSamplerToVariable("TiTunning", "NormalModeSampler");
            tcpDevice.AssignSamplerToVariable("TdTunning", "NormalModeSampler");
            tcpDevice.AssignSamplerToVariable("NTunning", "NormalModeSampler");
            tcpDevice.AssignSamplerToVariable("TpTunning", "NormalModeSampler");
            tcpDevice.AssignSamplerToVariable("InvertedTuning", "NormalModeSampler");
            tcpDevice.AssignSamplerToVariable("DerivativeActionWeightTuning", "NormalModeSampler");
            tcpDevice.AssignSamplerToVariable("ProportionalActionWeightTuning", "NormalModeSampler");

            //Polaczenie zdarzen
            ConnectEvents();

            //Zainicjalizowano zmienne
            initiallized = true;
        }

        /// <summary>
        /// Metoda laczaca metody obslugi do konkretnych zdarzen
        /// </summary>
        private void ConnectEvents()
        {
            tcpDevice.ConnectionObject.StatusChanged += OnDeviceConnectionStatusChanged;

            tcpDevice.GetVariable("KpTunning").ValueUpdated += OnKpValueUpdated;
            tcpDevice.GetVariable("TiTunning").ValueUpdated += OnTiValueUpdated;
            tcpDevice.GetVariable("TdTunning").ValueUpdated += OnTdValueUpdated;
            tcpDevice.GetVariable("NTunning").ValueUpdated += OnNValueUpdated;
            tcpDevice.GetVariable("TpTunning").ValueUpdated += OnPIDSampleTimeValueUpdated;
            tcpDevice.GetVariable("InvertedTuning").ValueUpdated += OnInvertedValueUpdated;

            tcpDevice.GetVariable("ModeTunning").ValueUpdated += OnControllerModeValueUpdated;
            tcpDevice.GetVariable("Tuning").ValueUpdated += OnTuningVariableValueUpdated;
            tcpDevice.GetVariable("Setpoint").ValueUpdated += OnSetpointValueUpdated;
            tcpDevice.GetVariable("Input").ValueUpdated += OnProcessValueValueUpdated;
            tcpDevice.GetVariable("Output").ValueUpdated += OnControllerOutputValueUpdated;
            tcpDevice.GetVariable("ManualValueTunning").ValueUpdated += OnManualOutputValueUpdated;

        }

        /// <summary>
        /// Metoda czekajaca na zmiane status polaczenia - dziala tylko jezeli polaczenie nie jest aktywne
        /// </summary>
        public void WaitUntilConnectionStateChanged()
        {
            if (!Connected)
            {
                //Zresetiwanie flagi
                ConnectionStateFlag = false;

                //Oczekiwanie az flaga zostanie zmienona przez zdarzenie zmiany stanu polaczenia
                while (!ConnectionStateFlag)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Metoda zapisujaca do sterownika wprowadzone parametry
        /// </summary>
        /// <param name="kp">
        /// Wzmocnienie
        /// </param>
        /// <param name="ti">
        /// Stala calkowania
        /// </param>
        /// <param name="td">
        /// Stala rozniczkowania
        /// </param>
        /// <param name="n">
        /// Stala czlonu inercyjnego rozniczkowania
        /// </param>
        /// <param name="tp">
        /// Czas probkowania algorytmu regulatora
        /// </param>
        /// <param name="inverted">
        /// Tryb pracy Normal/Reverse
        /// </param>
        public void SavePIDParameters(Double kp, Double ti, Double td, Double n, Double tp, Boolean inverted)
        {
            if (Connected)
            {
                //Wlaczenie trybu zapisu zmiennych
                TurnOnTuningMode();

                //Odczekanie az tryb zostanie zmienony przez sterownik
                System.Threading.Thread.Sleep(100);

                //Jezeli tryb nie zostal zmienony - zglaszam wyjatek
                if (!TuningMode)
                {
                    TurnOffTuningMode();
                    throw new InvalidProgramException("Device has not turned into tuning mode");
                }

                //Ustawienie podanych parametrow w sterowniku
                tcpDevice.GetVariable("KpTunning").SetValueInPLC(Convert.ToSingle(kp));
                tcpDevice.GetVariable("TiTunning").SetValueInPLC(Convert.ToSingle(ti));
                tcpDevice.GetVariable("TdTunning").SetValueInPLC(Convert.ToSingle(td));
                tcpDevice.GetVariable("NTunning").SetValueInPLC(Convert.ToSingle(n));
                tcpDevice.GetVariable("TpTunning").SetValueInPLC(Convert.ToSingle(tp));
                tcpDevice.GetVariable("InvertedTuning").SetValueInPLC(inverted);
                tcpDevice.GetVariable("DerivativeActionWeightTuning").SetValueInPLC((Convert.ToSingle( 1.0 )));
                tcpDevice.GetVariable("ProportionalActionWeightTuning").SetValueInPLC((Convert.ToSingle( 1.0 )));

                //Odczekanie az sterownik ustawi parametry
                System.Threading.Thread.Sleep(100);

                //Wylaczenie trybu zapisu
                TurnOffTuningMode();
            }
            else
            {
                //Jezeli aplikacja nie jest polaczona ze sterownikiem - nalezy ja polaczyc
                Connect();

                //Odczekanie na zmiane stanu polaczenia
                WaitUntilConnectionStateChanged();

                //Jezeli nie udalo sie polaczyc - rozlaczamy i zwracamy wyjatek
                if (!Connected)
                {
                    Disconnect();
                    throw new InvalidProgramException("Cannot connect to device");
                }

                //Sprawdzenie czy mozna zapisywac parametry
                tcpDevice.GetVariable("EnableTuning").RefreshValue();

                if (!(bool)tcpDevice.GetVariable("EnableTuning").Value)
                {
                    Disconnect();
                    throw new InvalidProgramException("No permission to save parameters in device");
                }

                //Wlaczenie trybu zapisu zmiennych
                TurnOnTuningMode();

                //Odczekanie az tryb zostanie zmienony przez sterownik
                System.Threading.Thread.Sleep(100);

                //Jezeli tryb nie zostal zmienony - zglaszam wyjatek
                if (!TuningMode)
                {
                    TurnOffTuningMode();
                    Disconnect();
                    throw new InvalidProgramException("Device has not turned into tuning mode");
                }

                //Ustawienie podanych parametrow w sterowniku
                tcpDevice.GetVariable("KpTunning").SetValueInPLC(Convert.ToSingle(kp));
                tcpDevice.GetVariable("TiTunning").SetValueInPLC(Convert.ToSingle(ti));
                tcpDevice.GetVariable("TdTunning").SetValueInPLC(Convert.ToSingle(td));
                tcpDevice.GetVariable("NTunning").SetValueInPLC(Convert.ToSingle(n));
                tcpDevice.GetVariable("TpTunning").SetValueInPLC(Convert.ToSingle(tp));
                tcpDevice.GetVariable("InvertedTuning").SetValueInPLC(inverted);
                tcpDevice.GetVariable("DerivativeActionWeightTuning").SetValueInPLC((Convert.ToSingle(1.0)));
                tcpDevice.GetVariable("ProportionalActionWeightTuning").SetValueInPLC((Convert.ToSingle(1.0)));

                //Odczekanie az sterownik ustawi parametry
                System.Threading.Thread.Sleep(100);

                //Wylaczenie trybu zapisu
                TurnOffTuningMode();

                //Rozlaczenie
                Disconnect();
            }
        }

        private bool connected;
        /// <summary>
        /// Wlasciwosc okreslajaca czy obiekt jest polaczony ze sterownikiem
        /// </summary>
        public bool Connected
        {
            get
            {
                return connected;
            }

            private set
            {
                //Jezeli nowa wartosc rozni sie od poprzedniej nalezy ja zapisac i zglosic zdarzenie ConnectionStateChanged
                if (value != connected)
                {
                    connected = value;

                    if (ConnectionStateChanged != null)
                    {
                        ConnectionStateChanged();
                    }

                    //Nalezy rowniez odswiezyc obiektu probkujace - jezeli obiekt nie jest w trybie tuning uruchamiany jest inny obiekt probkujacy niz jezeli jest w trybie normalnym
                    RefreshSamplersMode();
                }

                //Odswiezenie mechanizmu wiazania danych
                NotifyPropertyChanged("Connected");
            }
        }

        /// <summary>
        /// Zdarzenia zmiany stanu polaczenia
        /// </summary>
        public event Action ConnectionStateChanged;

        /// <summary>
        /// Flaga pozwalajaca na okreslenie i zatrzymanie watku do momentu zmiany stanu polaczenia - kazda nowa zmiana stanu polaczenia ustawia flage na wartosc true
        /// </summary>
        public bool ConnectionStateFlag;

        /// <summary>
        /// Metoda laczaca ze sterownikiem
        /// </summary>
        public void Connect()
        {
            tcpDevice.ConnectionObject.StartConnection();
        }

        /// <summary>
        /// Metoda rozlaczajaca ze sterownikiem
        /// </summary>
        public void Disconnect()
        {
            tcpDevice.ConnectionObject.StopConnection();
        }

        private bool tuningMode;
        /// <summary>
        /// Wlasciwosc okreslajaca czy sterownik jest w trybie zmiany parametrow regulatora
        /// Tryb Tunning umozliwia zapis parametrow do bloku regulatora
        /// W trybie normalnym wartosci sa pobierane z bloku regulatora
        /// </summary>
        public bool TuningMode
        {
            get
            {
                return tuningMode;
            }

            private set
            {
                //Jezeli nowa wartosc rozni sie od poprzedniej nalezy ja zapisac i zglosic zdarzenie TuningModeStateChanged
                if (value != tuningMode)
                {
                    tuningMode = value;

                    if (TuningModeStateChanged != null)
                    {
                        TuningModeStateChanged();
                    }

                    //Nalezy rowniez odswiezyc obiektu probkujace - jezeli obiekt nie jest w trybie tuning uruchamiany jest inny obiekt probkujacy niz jezeli jest w trybie normalnym
                    RefreshSamplersMode();
                }

                //Odswiezenie mechanizmu wiazania danych
                NotifyPropertyChanged("TuningMode");
            }
        }

        /// <summary>
        /// Zdarzenia zmiany stanu trybu wspolpracy z parametrami
        /// </summary>
        public event Action TuningModeStateChanged;

        /// <summary>
        /// Metoda przestawiajaca sterownik w tryb Tuning
        /// </summary>
        public void TurnOnTuningMode()
        {
            //Przejscie moze odbywac sie tylko jezeli sterownik jest polaczony z aplikacja
            if (Connected)
            {
                //Zmiana trybu moze sie odbywac jedynie w przypadku gdy zezwolony jest w programie sterownika
                tcpDevice.GetVariable("EnableTuning").RefreshValue();

                if (!(bool)tcpDevice.GetVariable("EnableTuning").Value)
                {
                    throw new InvalidProgramException("No permission to save parameters in device");
                }

                //Ustawienie flag odpowiadajacym trybowi Tuning
                tcpDevice.SetVariableValueInPLC("ResetTuning", false);
                tcpDevice.SetVariableValueInPLC("SetTuning", true);

                //Poczekanie az sterownik przetworzy dane
                System.Threading.Thread.Sleep(100);

                //Odswiezenie zmiennej trybu Tuning
                tcpDevice.GetVariable("Tuning").RefreshValue();

            }
        }

        /// <summary>
        /// Metoda przestawiajaca sterownik w tryb Tuning
        /// </summary>
        public void TurnOffTuningMode()
        {
            //Przejscie moze odbywac sie tylko jezeli sterownik jest polaczony z aplikacja
            if (Connected)
            {
                //Ustawienie flag odpowiadajacym trybowi Tuning
                tcpDevice.SetVariableValueInPLC("ResetTuning", true);
                tcpDevice.SetVariableValueInPLC("SetTuning", false);

                //Poczekanie az sterownik przetworzy dane
                System.Threading.Thread.Sleep(100);

                //Odswiezenie zmiennej trybu Tuning
                tcpDevice.GetVariable("Tuning").RefreshValue();
            }
        }

        /// <summary>
        /// Metoda odswiezeajaca stan trybu Tuning/Normal
        /// </summary>
        private void RefreshTuningMode()
        {
            //Pobranie i sprawdzenie wartosci zmiennej Tunnig sterownika
            //Nie trzeba odswiezac - stworzone z mysla wywolywania w metodzie obslugi zdarzenia OnTuningVariableValueUpdated ktore jest wywolywane gdy ta wartosc zostanie odswiezona
            if (Convert.ToBoolean(tcpDevice.GetVariable("Tuning").Value))
            {
                TuningMode = true;
            }
            else
            {
                TuningMode = false;
            }
        }

        /// <summary>
        /// Metoda odswiezajaca obiekty probkujace w zaleznosi od trybu Tuning/Normal
        /// W trybie normalnym dziala - NormalModeSampler
        /// W trybie tuning dziala - TuningModeSample
        /// </summary>
        private void RefreshSamplersMode()
        {
            if (TuningMode)
            {
                StopNormalModeSampler();
                StartTuningModeSampler();
            }
            else
            {
                StartNormalModeSampler();
                StopTuningModeSampler();
            }
        }



        private string statusMessage;
        /// <summary>
        /// Wlasciwosc okreslajaca status polaczenia obiektu
        /// </summary>
        public String StatusMessage
        {
            get
            {
                return statusMessage;
            }

            set
            {
                statusMessage = value;
                NotifyPropertyChanged("StatusMessage");
            }
        }

        

        /// <summary>
        /// Obiekt biblioteki S7TCP do polaczenia ze sterownikiem
        /// </summary>
        private S7Device tcpDevice;

        /// <summary>
        /// Metoda wywolywana za kazdym razem gdy zmieni sie stan polaczenia obiektu biblioteki S7TCP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnDeviceConnectionStatusChanged(object sender, ObjectStatusChangedEventArgument eventArgument)
        {
            //Ustawienie flagi polaczenia na wartosc true
            ConnectionStateFlag = true;

            //Jezeli nowy status to Fault - nalezy ustawic nowa wartosc false wlasciowsci Connected i rozlaczyc polaczenie
            if (eventArgument.NewStatus == StatusType.Fault)
            {
                StatusMessage = eventArgument.ErrorMessage;
                Connected = false;
                Disconnect();
            }

            //Jeżeli nowy status to Connected - nalezy sprawdzicz czy odczytywany jest poprawny blok i nastepnie przypisac true wlasciwosci Connected
            if (eventArgument.NewStatus == StatusType.Connected)
            {
                try
                {
                    //Odswiezenie wartosci zmiennej charakteryzujacej blok - jej wartosc powinna wynosci 12345
                    tcpDevice.GetVariable("BlockConstant").RefreshValue();

                    if ((int)tcpDevice.GetVariable("BlockConstant").Value != 12345)
                    {
                        //Jezeli wartosc ta jest rozna nalezy zwrocic wyjatek
                        throw new InvalidProgramException("Wrong Data Block");
                    }

                }
                catch (InvalidProgramException)
                {
                    //Jezeli nie udalo sie polaczyc z danym blokiem - nalezy rozlaczyc polaczenie
                    Connected = false;
                    StatusMessage = "Invalid Data Block";
                    Disconnect();
                    return;
                }

                StatusMessage = "Connected";

                Connected = true;
            }

            //Jezeli nowy status to Disconnected - nalezy przypisac wlasciwosci connected wartosc false
            if (eventArgument.NewStatus == StatusType.Disconnected)
            {
                Connected = false;
                StatusMessage = "Disconnected";
            }

        }



        /// <summary>
        /// Metoda startujaca obiekt probkujacy trybu tuning
        /// </summary>
        private void StartTuningModeSampler()
        {
            tcpDevice.GetSampler("TuningModeSampler").Start();
        }

        /// <summary>
        /// Metoda zatrzymujaca obiekt probkujacy trybu tuning
        /// </summary>
        private void StopTuningModeSampler()
        {
            tcpDevice.GetSampler("TuningModeSampler").Stop();
        }

        /// <summary>
        /// Metoda startujaca obiekt probkujacy trybu normalnego
        /// </summary>
        private void StartNormalModeSampler()
        {
            tcpDevice.GetSampler("NormalModeSampler").Start();
        }

        /// <summary>
        /// Metoda startujaca obiekt probkujacy trybu normalnego
        /// </summary>
        private void StopNormalModeSampler()
        {
            tcpDevice.GetSampler("NormalModeSampler").Stop();
        }



        private Double kp;
        /// <summary>
        /// Wlasciwosc okreslajaca wzmocnienie bloku PID_Compact
        /// </summary>
        public Double Kp
        {
            get
            {
                //Jezeli obiekt jest polaczony - mozna pobrac wartosc kp z obiektu sterownika biblioteki S7TCP
                if (Connected)
                {
                    try
                    {
                        kp = Convert.ToDouble(tcpDevice.GetVariable("KpTunning").Value);
                        return kp;
                    }
                    catch
                    {
                        return kp;
                    }
                }
                else
                {
                    return kp;
                }
            }
        }

        /// <summary>
        /// Zdarzenie wywolywane za kazdym razem gdy wartosc wzmocnienia pobrana ze sterownika zostaje odswiezona
        /// </summary>
        public event Action KpUpdated;

        /// <summary>
        /// Metoda obslugi zdarzenia zglaszana gdy tylko wartosc wzmocnienia obiektu biblioteki S7TCP zostanie odswiezona
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnKpValueUpdated(object sender, ValueUpdatedEventArgument eventArgument)
        {
            //Zgloszenie zdarzenia odswiezeniea
            if (KpUpdated != null)
            {
                KpUpdated();
            }
        }




        private Double ti;
        /// <summary>
        /// Wlasciwosc okreslajaca czas calkowania bloku PID_Compact
        /// </summary>
        public Double Ti
        {
            get
            {
                if (Connected)
                {
                    try
                    {
                        ti = Convert.ToDouble(tcpDevice.GetVariable("TiTunning").Value);
                        return ti;
                    }
                    catch
                    {
                        return ti;
                    }
                }
                else
                {
                    return ti;
                }
            }
        }

        /// <summary>
        /// Zdarzenie wywolywane za kazdym razem gdy wartosc stalej calkowania pobrana ze sterownika zostaje odswiezona
        /// </summary>
        public event Action TiUpdated;

        /// <summary>
        /// Metoda obslugi zdarzenia zglaszana gdy tylko wartosc stalej calkowania obiektu biblioteki S7TCP zostanie odswiezona
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnTiValueUpdated(object sender, ValueUpdatedEventArgument eventArgument)
        {
            if (TiUpdated != null)
            {
                TiUpdated();
            }
        }




        private Double td;
        /// <summary>
        /// Wlasciwosc okreslajaca czas rozniczkowania bloku PID_Compact
        /// </summary>
        public Double Td
        {
            get
            {
                if (Connected)
                {
                    try
                    {
                        //Jezeli obiekt jest polaczony - mozna pobrac wartosc Td z obiektu sterownika biblioteki S7TCP
                        td = Convert.ToDouble(tcpDevice.GetVariable("TdTunning").Value);
                        return td;
                    }
                    catch
                    {
                        return td;
                    }
                }
                else
                {
                    return td;
                }
            }
        }

        /// <summary>
        /// Zdarzenie wywolywane za kazdym razem gdy wartosc stalej rozniczkowania pobrana ze sterownika zostaje odswiezona
        /// </summary>
        public event Action TdUpdated;

        /// <summary>
        /// Metoda obslugi zdarzenia zglaszana gdy tylko wartosc stalej rozniczkowania obiektu biblioteki S7TCP zostanie odswiezona
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnTdValueUpdated(object sender, ValueUpdatedEventArgument eventArgument)
        {
            if (TdUpdated != null)
            {
                TdUpdated();
            }
        }



        private Double n;
        /// <summary>
        /// Wlasciwosc okreslajaca stala inercji czlonu rozniczkujacego PID_Compact
        /// </summary>
        public Double N
        {
            get
            {
                if (Connected)
                {
                    try
                    {
                        //Jezeli obiekt jest polaczony - mozna pobrac wartosc N z obiektu sterownika biblioteki S7TCP
                        n = Convert.ToDouble(tcpDevice.GetVariable("NTunning").Value);
                        return n;
                    }
                    catch
                    {
                        return n;
                    }
                }
                else
                {
                    return n;
                }
            }
        }

        /// <summary>
        /// Zdarzenie wywolywane za kazdym razem gdy wartosc inercji czlonu rozniczkujacego pobrana ze sterownika zostaje odswiezona
        /// </summary>
        public event Action NUpdated;

        /// <summary>
        /// Metoda obslugi zdarzenia zglaszana gdy tylko wartosc inercji czlonu rozniczkujacego pobrana ze sterownika zostanie odswiezona
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnNValueUpdated(object sender, ValueUpdatedEventArgument eventArgument)
        {
            if (NUpdated != null)
            {
                NUpdated();
            }
        }



        private bool inverted;
        /// <summary>
        /// Wlasciwosc okreslajaca tryb pracy regulatora PID - normal/reverse
        /// </summary>
        public Boolean Inverted
        {
            get
            {
                if (Connected)
                {
                    try
                    {
                        //Jezeli obiekt jest polaczony - mozna pobrac wartosc okreslajaca tryb pracy regulatora z obiektu sterownika biblioteki S7TCP
                        inverted = Convert.ToBoolean(tcpDevice.GetVariable("InvertedTuning").Value);
                        return inverted;
                    }
                    catch
                    {
                        return inverted;
                    }
                }
                else
                {
                    return inverted;
                }
            }
        }

        /// <summary>
        /// Zdarzenie wywolywane za kazdym razem gdy tryb pracy Normal/Reverse pobrany ze sterownika zostaje odswiezony
        /// </summary>
        public event Action InvertedUpdated;

        /// <summary>
        /// Metoda obslugi zdarzenia zglaszana gdy tylko wartosc trybu pracy Normal/Reverse obiektu biblioteki S7TCP zostanie odswiezona
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnInvertedValueUpdated(object sender, ValueUpdatedEventArgument eventArgument)
        {
            if (InvertedUpdated != null)
            {
                InvertedUpdated();
            }
        }




        private Double pidSampleTime;
        /// <summary>
        /// Wlasciwosc okreslajaca wartosc czasu probkowania algorytmu regulatora
        /// </summary>
        public Double PIDSampleTime
        {
            get
            {
                if (Connected)
                {
                    try
                    {
                        pidSampleTime = Convert.ToDouble(tcpDevice.GetVariable("TpTunning").Value);
                    }
                    catch
                    {

                    }
                }

                return pidSampleTime;
            }

            set
            {
                pidSampleTime = value;
                NotifyPropertyChanged("PIDSampleTime");
            }
        }

        /// <summary>
        /// Zdarzenie wywolywane za kazdym razem gdy wartosc czasu probkowania pobrana ze sterownika zostaje odswiezona
        /// </summary>
        public event Action PIDSampleTimeUpdated;

        /// <summary>
        /// Metoda obslugi zdarzenia zglaszana gdy tylko wartoscczasu probkowania obiektu biblioteki S7TCP zostanie odswiezona
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnPIDSampleTimeValueUpdated(object sender, ValueUpdatedEventArgument eventArgument)
        {
            if (PIDSampleTimeUpdated != null)
            {
                PIDSampleTimeUpdated();
            }
        }



        /// <summary>
        /// Wlasciwosc okreslajaca wartosc sygnalu zadanego regulatora
        /// </summary>
        public Double Setpoint
        {
            get
            {
                return Convert.ToDouble(tcpDevice.GetVariable("Setpoint").Value);
            }

            set
            {
                try
                {
                    tcpDevice.GetVariable("Setpoint").SetValueInPLC(Convert.ToSingle(value));
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Zdarzenie wywolywane za kazdym razem gdy wartosc sygnalu zadnego pobrana ze sterownika zostaje odswiezona
        /// </summary>
        public event Action SetpointUpdated;

        /// <summary>
        /// Metoda obslugi zdarzenia zglaszana gdy tylko wartosc sygnalu zadanego obiektu biblioteki S7TCP zostanie odswiezona
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnSetpointValueUpdated(object sender, ValueUpdatedEventArgument eventArgument)
        {
            if (SetpointUpdated != null)
            {
                SetpointUpdated();
            }
        }




        /// <summary>
        /// Wlasciwosc okreslajaca wartosc wielkosci mierzonej
        /// </summary>
        public Double ProcessValue
        {
            get
            {
                return Convert.ToDouble(tcpDevice.GetVariable("Input").Value);
            }

            set
            {
                try
                {
                    tcpDevice.GetVariable("Input").SetValueInPLC(Convert.ToSingle(value));
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Zdarzenie wywolywane za kazdym razem gdy wartosc sygnalu mierzonego pobrana ze sterownika zostaje odswiezona
        /// </summary>
        public event Action ProcessValueUpdated;

        /// <summary>
        /// Metoda obslugi zdarzenia zglaszana gdy tylko wartosc sygnalu mierzonego obiektu biblioteki S7TCP zostanie odswiezona
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnProcessValueValueUpdated(object sender, ValueUpdatedEventArgument eventArgument)
        {
            if (ProcessValueUpdated != null)
            {
                ProcessValueUpdated();
            }
        }




        /// <summary>
        /// Wlasciwosc wyjsca regulatora
        /// </summary>
        public Double ControllerOutput
        {
            get
            {
                return Convert.ToDouble(tcpDevice.GetVariable("Output").Value);
            }

            set
            {
                try
                {
                    tcpDevice.GetVariable("Output").SetValueInPLC(Convert.ToSingle(value));
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Zdarzenie wywolywane za kazdym razem gdy wartosc wyjsca regulatora pobrana ze sterownika zostaje odswiezona
        /// </summary>
        public event Action ControllerOutputUpdated;

        /// <summary>
        /// Metoda obslugi zdarzenia zglaszana gdy tylko wartosc wyjsca regulatora obiektu biblioteki S7TCP zostanie odswiezona
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnControllerOutputValueUpdated(object sender, ValueUpdatedEventArgument eventArgument)
        {
            if (ControllerOutputUpdated != null)
            {
                ControllerOutputUpdated();
            }
        }




        /// <summary>
        /// Wlasciwosc okreslajaca wartosc wyjscia regulatora w trybie recznym
        /// </summary>
        public Double ManualValue
        {
            get
            {
                return Convert.ToDouble(tcpDevice.GetVariable("ManualValueTunning").Value);
            }

            set
            {
                try
                {
                    tcpDevice.GetVariable("ManualValueTunning").SetValueInPLC(Convert.ToSingle(value));
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Zdarzenie wywolywane za kazdym razem gdy wartosc wyjsca regulatora w trybie recznym pobrana ze sterownika zostaje odswiezona
        /// </summary>
        public event Action ManualOutputUpdated;

        /// <summary>
        /// Metoda obslugi zdarzenia zglaszana gdy tylko wartosc wyjscia regulatora w trybie recznym obiektu biblioteki S7TCP zostanie odswiezona
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnManualOutputValueUpdated(object sender, ValueUpdatedEventArgument eventArgument)
        {
            if (ManualOutputUpdated != null)
            {
                ManualOutputUpdated();
            }
        }




        /// <summary>
        /// Wlasciwosc okreslajaca tryb pracy regulatora PID - auto/reka
        /// </summary>
        public Int32 Mode
        {
            get
            {
                return Convert.ToInt32(tcpDevice.GetVariable("ModeTunning").Value);
            }

            set
            {
                try
                {
                    tcpDevice.GetVariable("ModeTunning").SetValueInPLC(value);
                }
                catch
                {

                }

            }
        }

        /// <summary>
        /// Zdarzenie wywolywane za kazdym razem gdy tryb pracy regulatora - auto/reka pobrany ze sterownika zostaje odswiezony
        /// </summary>
        public event Action ControllerModeUpdated;

        /// <summary>
        /// Metoda obslugi zdarzenia zglaszana gdy tylko wartosc trybu pracy Auto/Manual obiektu biblioteki S7TCP zostanie odswiezona
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnControllerModeValueUpdated(object sender, ValueUpdatedEventArgument eventArgument)
        {
            if (ControllerModeUpdated != null)
            {
                ControllerModeUpdated();
            }
        }



        /// <summary>
        /// Metoda obslugi zdarzenia zglaszana gdy tylko wartosc okreslajaca tryb bloku - Tryb tuning/normal zostanie odswiezona - wykorzystywane do mechanizmu zmiany trybu pracy Tuning/Normal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnTuningVariableValueUpdated(object sender, ValueUpdatedEventArgument eventArgument)
        {
            RefreshTuningMode();
        }



        /// <summary>
        /// Wlasciwosc okreslajaca adres ip sterownika
        /// </summary>
        public string IPAdress
        {
            get
            {
                return tcpDevice.IpAdress;
            }

            set
            {
                if (Connection.CheckIPAdress(value))
                {
                    tcpDevice.IpAdress = value;
                    NotifyPropertyChanged("IPAdress");
                }
                else
                {
                    NotifyPropertyChanged("IPAdress");
                }

            }
        }



        /// <summary>
        /// Wlasciwosc okreslajaca numer kasety sterownika
        /// </summary>
        public int Rack
        {
            get
            {
                return tcpDevice.Rack;
            }

            set
            {
                tcpDevice.Rack = value;
                NotifyPropertyChanged("Rack");
            }
        }



        /// <summary>
        /// Wlasciwosc okreslajaca numer gniazda kasety sterownika
        /// </summary>
        public int Slot
        {
            get
            {
                return tcpDevice.Slot;
            }

            set
            {
                tcpDevice.Slot = value;
                NotifyPropertyChanged("Slot");
            }
        }



        /// <summary>
        /// Wlasciwosc okreslajaca czas probkowania wykorzystywany do cyklicznego pobierania danych ze sterownika
        /// </summary>
        public Int32 IdentificationSampleTime
        {
            get
            {
                return tcpDevice.GetSampler("TuningModeSampler").SampleTime;
            }

            set
            {
                tcpDevice.GetSampler("TuningModeSampler").SampleTime = value;
                NotifyPropertyChanged("IdentificationSampleTime");
            }
        }



        private Double tiForNonI = 9999;
        /// <summary>
        /// Wlasciwosc okreslajaca wartosc stalej calkowania ktora bedzie przpisywana do regulatora jezeli wybrany zostanie algorytm bez czlonu calkujacego
        /// </summary>
        public Double TiForNonI
        {
            get
            {
                return tiForNonI;
            }

            set
            {
                tiForNonI = value;
                NotifyPropertyChanged("TiForNonI");
            }
        }



        private Int32 dbNumber = 1;
        /// <summary>
        /// Numer bloku danych 
        /// </summary>
        public Int32 DBNumber
        {
            get
            {
                return dbNumber;
            }

            set
            {
                dbNumber = value;
                SetNewDBNumber();
                NotifyPropertyChanged("DBNumber");
            }
        }

        /// <summary>
        /// Metoda ustawiajaca nowy numer bloku danych - synchronizuje wartosc DBNumber z wartosciami w zmiennych
        /// </summary>
        private void SetNewDBNumber()
        {
            if (initiallized)
            {
                tcpDevice.GetVariable("SetTuning").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("ResetTuning").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("ManualEnableTunning").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("ManualValueTunning").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("KpTunning").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("TiTunning").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("TdTunning").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("NTunning").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("TpTunning").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("ModeTunning").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("BlockConstant").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("ResetTunning").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("InvertedTuning").DataBlockNumber = dbNumber;

                //Cyklicznie odswiezane zmienne
                tcpDevice.GetVariable("EnableTuning").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("Tuning").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("Setpoint").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("Input").DataBlockNumber = dbNumber;
                tcpDevice.GetVariable("Output").DataBlockNumber = dbNumber;

            }
        }

        /// <summary>
        ///Mechanizm wiazania danych WPF
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///Mechanizm wiazania danych WPF
        /// </summary>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
       
        /// <summary>
        /// Metoda zapisujaca ustawienia regulatora do dokumentu XML
        /// </summary>
        /// <returns>
        /// Dokument XML z ustawieniami regulatora
        /// </returns>
        public XDocument ToXML()
        {
            XDocument doc = new XDocument();
            var ipAdressXML = new XElement("IPAdress", IPAdress);

            var rackXML = new XElement("Rack",Rack );

            var slotXML = new XElement("Slot",Slot);

            var sampleTimeXML = new XElement("IdentificationSampleTime", IdentificationSampleTime);

            var tiForNonIXML = new XElement("TiForNonI", TiForNonI );

            var dbNumberXML = new XElement("DBNumber", DBNumber);

            var pidSampleTimeXML = new XElement("PIDSampleTime", PIDSampleTime);

            doc.Add(new XElement("PIDControllerDevice", ipAdressXML, rackXML, slotXML, sampleTimeXML, dbNumberXML, tiForNonIXML, pidSampleTimeXML));

            return doc;
        }

        /// <summary>
        /// Metoda tworzaca obiekt sterownika na podstawie dokumentu XML przechowujacego jego ustawienia
        /// </summary>
        /// <param name="documentXML">
        /// Dokument XML z ustawieniami sterownika
        /// </param>
        /// <returns>
        /// Obiekt sterownika
        /// </returns>
        public static PIDControllerDevice FromXML(XDocument documentXML)
        {
            //Pobranie pierwszego elementu glownego
            XElement pidControllerXML = documentXML.Root;
            String ipAdressXML = pidControllerXML.Element("IPAdress").Value;
            Int32 rackXML = Convert.ToInt32(pidControllerXML.Element("Rack").Value);
            Int32 slotXML = Convert.ToInt32(pidControllerXML.Element("Slot").Value);
            Int32 sampleTimeXML = Convert.ToInt32(pidControllerXML.Element("IdentificationSampleTime").Value);
            Double tiForNonIXML = Convert.ToDouble(pidControllerXML.Element("TiForNonI").Value.ToString().Replace(".", ","));
            Int32 dbNumberXML = Convert.ToInt32(pidControllerXML.Element("DBNumber").Value);
            Double pidSampleTimeXML = Convert.ToDouble(pidControllerXML.Element("PIDSampleTime").Value.ToString().Replace(".", ","));
            return new PIDControllerDevice(ipAdressXML, rackXML, slotXML, sampleTimeXML, dbNumberXML, tiForNonIXML,pidSampleTimeXML);
        }

        
    }

}


