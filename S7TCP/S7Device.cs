using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace S7TCP
{

    /// <summary>
    /// Klasa sterownika S7
    /// </summary>
        public class S7Device : ObjectWithStateBase
        {

            /// <summary>
            /// Obiekt polaczenia sterownika
            /// </summary>
            protected internal Connection connection;

            /// <summary>
            /// Obiekt polaczenia sterownika
            /// </summary>
            public Connection ConnectionObject
            {
                get
                {
                    return connection;
                }

                protected set
                {
                    connection = value;
                }
            }
            
            /// <summary>
            /// Obiekt polaczenia ze sterownikiem
            /// </summary>
            protected Object lockingObject = new Object();

            /// <summary>
            /// Nazwa sterownika - musi byc unikalna
            /// </summary>
            /// Nazwa ta jest w rzeczywsitoscia nazwa polaczenia
            public String DeviceName
            {
                get
                {
                    return connection.connectionName;
                }
                set
                {
                    lock (lockingObject)
                    {
                        //Kazda zmiana nazwy wymaga rowniez zmiany nazwy polaczenia a co za tym idzie jego reinicjalizacji
                        if (connection.State == StatusType.Disconnected)
                        {
                            connection.connectionName = value;
                        }
                        else
                        {
                            StopConnection();
                            connection.connectionName = value;
                            StartConnection();
                        }
                    }
                }
            }

            /// <summary>
            /// Numer szyny sterownika
            /// </summary>
            public int Rack
            {
                get
                {
                    return connection.rack;
                }

                set
                {
                    lock (lockingObject)
                    {
                        //Kazda zmiana szyny sterownika wymaga rowniez zmiany szyny w polaczenia a co za tym idzie jego reinicjalizacji
                        if (connection.State == StatusType.Disconnected)
                        {
                            connection.rack = value;
                        }
                        else
                        {
                            StopConnection();
                            connection.rack = value;
                            StartConnection();
                        }
                    }
                }
            }

            /// <summary>
            /// Numer slotu sterownika
            /// </summary>
            public int Slot
            {
                get
                {
                    return connection.slot;
                }
                
                set
                {
                    lock (lockingObject)
                    {
                        //Kazda zmiana slotu sterownika wymaga rowniez zmiany slotu w polaczenia a co za tym idzie jego reinicjalizacji
                        if (connection.State == StatusType.Disconnected)
                        {
                            connection.slot = value;
                        }
                        else
                        {
                            StopConnection();
                            connection.slot = value;
                            StartConnection();
                        }
                    }
                }
            }

            /// <summary>
            /// Adres Ip Sterownika
            /// </summary>
            public string IpAdress
            {
                get
                {
                    return connection.ipAdress;
                }

                set
                {
                    lock (lockingObject)
                    {
                        //Sprawdzenie czy ciag znakow jest w ogole adresem IP
                        if (Connection.CheckIPAdress(value))
                        {
                            //Kazda zmiana adresu sterownika wymaga rowniez zmiany adresu w polaczenia a co za tym idzie jego reinicjalizacji
                            if (connection.State == StatusType.Disconnected)
                            {
                                connection.ipAdress = value;
                            }
                            else
                            {
                                StopConnection();
                                connection.ipAdress = value;
                                StartConnection();
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Czas wykorzystywany przez biblioteke libnodave do zakonczenia proby nieudanego polaczenia
            /// </summary>
            public int Timeout
            {
                get 
                {
                    return connection.timeout;
                }

                set
                {
                    lock (lockingObject)
                    {
                        //Kazda zmiana czasu timeout sterownika wymaga rowniez zmiany czasu timeout w polaczenia a co za tym idzie jego reinicjalizacji
                        if (connection.State == StatusType.Connected)
                        {
                            connection.timeout = value;
                        }
                        else
                        {
                            StopConnection();
                            connection.timeout = value;
                            StartConnection();
                        }
                    }
                }
            }

            /// <summary>
            /// Metoda rozpoczynajaca polaczenie sterownika
            /// </summary>
            public void StartConnection()
            {
                //Ustawienie nowego status sterownika
                SetNewStatus(StatusType.Connected);

                //Rozpoczecie polaczenia
                connection.StartConnection();

                //Rozpoczecie probkowania wszystkich obiektow
                foreach(SamplerBase sampler in Samplers)
                {
                    sampler.Start();
                }

            }

            /// <summary>
            /// Metoda zatrzymujaca polaczenie
            /// </summary>
            public void StopConnection()
            {
                //Snychronizacja dostepu do polaczenia
                lock(lockingObject)
                {
                    //Zatrzymanie wszystkich obiektow probkujacych
                    foreach (SamplerBase sampler in Samplers)
                    {
                        sampler.Stop();
                    }

                    //Zatrzymanie polaczenia
                    connection.StopConnection();

                    //Ustawienie stany polaczenia na rozlaczony
                    SetNewStatus(StatusType.Disconnected);
                }
            }

            /// <summary>
            ///Kolekcja obiektow probkujacych zmienne
            /// </summary>
            public SamplersCollection Samplers
            {
                get;
                private set;
            }

            /// <summary>
            /// Konstruktor klasy sterownika
            /// </summary>
            /// <param name="name">
            /// Unikalna nazwa sterownika
            /// </param>
            /// <param name="ipAdress">
            /// Adres IP sterownika
            /// </param>
            /// <param name="rack">
            /// Numer szyny sterownika
            /// </param>
            /// <param name="slot">
            /// Numer slotu sterownika
            /// </param>
            /// <param name="timeout">
            /// Czas timeout sterownika, wykorzystywany przez biblioteke libnodave do zakonczenia proby nieudanego polaczenia
            /// </param>
            public S7Device(string name,string ipAdress,int rack, int slot, int timeout = 1000 )
            {
                //Inicjalizacja zmiennych
                connection = new Connection(name);

                Samplers = new SamplersCollection();

                if (Connection.CheckIPAdress(ipAdress))
                {
                    connection.ipAdress = ipAdress;
                }
                else
                {
                    connection.ipAdress = "000.000.000.000";
                }

                connection.rack = rack;
                connection.slot = slot;
                connection.timeout = timeout;

            }

            /// <summary>
            /// Kolekcja zmiennych sterownika
            /// </summary>
            protected List<VariableBase> variables = new List<VariableBase>();

            /// <summary>
            /// Kolekcja zmiennych sterownika
            /// </summary>
            public List<VariableBase> Variables
            {
                get
                {
                    return variables;
                }
                protected set
                {
                    variables = value;
                }
            }

            /// <summary>
            /// Metoda usuwajaca zmienna z kolekcji sterownika
            /// </summary>
            /// <param name="variable">
            /// Zmienna usuwana
            /// </param>
            public void RemoveVariable(VariableBase variable)
            {
                lock(lockingObject)
                {
                    //Usuniecie zmiennych ze sterownika wymaga rowniez jej usuniecia z obiektu probkujacego
                    Variables.Remove(variable);
                    variable.RemoveSampler();
                }
            }

            /// <summary>
            /// Metoda dodajaca nowa zmienna do sterownika
            /// </summary>
            /// <param name="Name">
            /// Unikalna nazwa zmiennej
            /// </param>
            /// <param name="type">
            /// Typ zmiennej
            /// </param>
            /// <param name="memoryType">
            /// Typ pamieci w ktorej przechowywana jest zmienna
            /// </param>
            /// <param name="adress">
            /// Adres zmiennej
            /// </param>
            /// <param name="dataBlockNumber">
            /// Numer databloku zmiennej (istotne tylko dla zmiennych przechowywanych w DB)
            /// </param>
            /// <param name="bitNumber">
            /// Numer bitu (istotne tylko dla zmiennych typu Bool)
            /// </param>
            public void AddNewVariable(String Name, VariableType type, MemoryType memoryType, int adress, int dataBlockNumber = 0, int bitNumber = 0)
            {
                //Sprawdzenie czy zmienna o tej samej nazwie jest juz przypisana do sterownika - jezeli tak zmiennej takiej nie nalezy dodac
                bool nameExists = Variables.Exists((variable) => variable.Name == Name);
                if(nameExists)
                {
                    return;
                }
                
                //W zaleznosci od typu zmiennej nalezy dodac do sterownika zmienna roznego typu
                switch(type)
                {
                    case VariableType.Bool:
                        {
                            Variables.Add(new BooleanVariable(Name, connection, memoryType, adress, bitNumber, lockingObject, dataBlockNumber));
                            break;
                        }

                    case VariableType.Byte:
                        {
                            Variables.Add(new ByteVariable(Name, connection, memoryType, adress, lockingObject, dataBlockNumber));
                            break;
                        }

                    case VariableType.DateTime:
                        {
                            Variables.Add(new DateTimeVariable(Name, connection, memoryType, adress, lockingObject, dataBlockNumber));
                            break;
                        }

                    case VariableType.Float:
                        {
                            Variables.Add(new FloatVariable(Name, connection, memoryType, adress, lockingObject, dataBlockNumber));
                            break;
                        }

                    case VariableType.Int16:
                        {
                            Variables.Add(new Int16Variable(Name, connection, memoryType, adress, lockingObject, dataBlockNumber));
                            break;
                        }

                    case VariableType.Int32:
                        {
                            Variables.Add(new Int32Variable(Name, connection, memoryType, adress, lockingObject, dataBlockNumber));
                            break;
                        }
                    case VariableType.UInt16:
                        {
                            Variables.Add(new UInt16Variable(Name, connection, memoryType, adress, lockingObject, dataBlockNumber));
                            break;
                        }
                    case VariableType.UInt32:
                        {
                            Variables.Add(new UInt32Variable(Name, connection, memoryType, adress, lockingObject, dataBlockNumber));
                            break;
                        }
                }
            }

            /// <summary>
            /// Metoda dodajaca obiekt probkujacy do sterownika
            /// </summary>
            /// <param name="name">
            /// Unikalna nazwa obiektu probkujacego
            /// </param>
            /// <param name="samplerType">
            /// Typ dodawanego obiektu probkujacego
            /// </param>
            /// <param name="sampleTime">
            /// Czas probkowania [ms]
            /// </param>
            public void AddNewSampler(string name,SamplerType samplerType,int sampleTime)
            {
                //Sprawdzenie czy istnieje juz obiekt probkujacy o takiej nazwie
                bool nameExists = ((from sampler in Samplers
                                      where sampler.Name == name
                                      select sampler).Count()) != 0;
                if (nameExists)
                {
                    return;
                }

                //W zaleznosci od podanego typu obiektu probkujacego jest on tworzony i dodawany do sterownika
                switch(samplerType)
                {
                    case SamplerType.Reader:
                        {
                            Samplers.Add(new SamplerReader(name, connection, sampleTime, lockingObject));
                            break;
                        }
                    case SamplerType.Writer:
                        {
                            Samplers.Add(new SamplerWriter(name, connection, sampleTime, lockingObject));
                            break;
                        }
                }
            }

            /// <summary>
            /// Metoda usuwajaca obiekt probkujacy ze sterownika
            /// </summary>
            /// <param name="sampler">
            /// Obiekt probkujacy
            /// </param>
            public void RemoveSampler(SamplerBase sampler)
            {
                sampler.Stop();
                lock(lockingObject)
                {
                    Samplers.Remove(sampler);
                }
            }

            /// <summary>
            /// Metoda zwracajaca obiekt probkujacy sterownika
            /// </summary>
            /// <param name="name">
            /// Nazwa obiektu probkujacego
            /// </param>
            /// <returns>
            /// Obiekt probkujacy
            /// </returns>
            public SamplerBase GetSampler(string name)
            {
                return (from sampler in Samplers
                        where sampler.Name == name
                        select sampler).SingleOrDefault();
            }

            /// <summary>
            /// Metoda zwracjaca zmienna sterownika
            /// </summary>
            /// <param name="name">
            /// Nazwa zmiennej
            /// </param>
            /// <returns>
            /// Zmienna sterownika
            /// </returns>
            public VariableBase GetVariable(string name)
            {
                return (from variable in variables
                        where variable.Name == name
                        select variable).SingleOrDefault();
            }

            /// <summary>
            /// Metoda przypisujaca obiekt probkujacy danej zmiennej
            /// </summary>
            /// <param name="variableName">
            /// Nazwa zmiennej
            /// </param>
            /// <param name="samplerName">
            /// Nazwa obiektu probkujacego
            /// </param>
            public void AssignSamplerToVariable(string variableName, string samplerName)
            {
                VariableBase variable = GetVariable(variableName);
                if(variable == null)
                {
                    throw new InvalidOperationException(String.Format("There is no variable named {0} in current S7Device",variableName));
                }

                SamplerBase sampler = GetSampler(samplerName);
                if(sampler == null)
                {
                    throw new InvalidOperationException(String.Format("There is no sampler named {0} in current S7Device",samplerName));
                }

                AssignSamplerToVariable(variable,sampler);
            }

            /// <summary>
            /// Metoda przypisujaca obiekt probkujacy danej zmiennej
            /// </summary>
            /// <param name="variableIndex">
            /// Indeks zmiennej
            /// </param>
            /// <param name="samplerIndex">
            /// Indeks obiektu probkujacego
            /// </param>
            public void AssignSamplerToVariable(int variableIndex, int samplerIndex)
            {
                if(variableIndex >= variables.Count || variableIndex < 0)
                {
                    throw new InvalidOperationException(String.Format("Invalid variable index - There is only {0} variables in device",variables.Count));
                }

                if (samplerIndex >= Samplers.Count() || samplerIndex < 0)
                {
                    throw new InvalidOperationException(String.Format("Invalid sampler index - There is only {0} samplers in device", Samplers.Count()));
                }

                AssignSamplerToVariable(variables[variableIndex], Samplers[samplerIndex]);
            }

            /// <summary>
            /// Metoda przypisujaca obiekt probkujacy danej zmiennej
            /// </summary>
            /// <param name="variable">
            /// Obiekt zmiennej
            /// </param>
            /// <param name="sampler">
            /// Obiekt probkujący
            /// </param>
            public void AssignSamplerToVariable(VariableBase variable, SamplerBase sampler)
            {
                if (!variables.Contains(variable))
                {
                    throw new InvalidOperationException("There is no such variable in variables collection");
                }

                if (!Samplers.Contains(sampler))
                {
                    throw new InvalidOperationException("There is no such sampler in samplers collection");
                }

                lock(lockingObject)
                {
                    variable.SetSampler(sampler);
                }
            }

            /// <summary>
            /// Metoda ustawiajaca wartosc zmiennej w sterowniku
            /// </summary>
            /// <param name="variableName">
            /// Nazwa zmiennej
            /// </param>
            /// <param name="newValue">
            /// Nowa wartosc zmiennej
            /// </param>
            public void SetVariableValueInPLC(string variableName, object newValue)
            {
                VariableBase variable = GetVariable(variableName);
                if (variable == null)
                {
                    throw new InvalidOperationException(String.Format("There is no variable named {0} in current S7Device", variableName));
                }

                SetVariableValueInPLC(variable, newValue);
            }

            /// <summary>
            /// Metoda ustawiajaca wartosc zmiennej w sterowniku
            /// </summary>
            /// <param name="variableIndex">
            /// Indeks zmiennej
            /// </param>
            /// <param name="newValue">
            /// Nowa wartosc zmiennej
            /// </param>
            public void SetVariableValueInPLC(int variableIndex, object newValue)
            {
                if (variableIndex >= variables.Count || variableIndex < 0)
                {
                    throw new InvalidOperationException(String.Format("Invalid variable index - There is only {0} variables in device", variables.Count));
                }

                SetVariableValueInPLC(variables[variableIndex], newValue);
            }

            /// <summary>
            /// Metoda ustawiajaca wartosc zmiennej w sterowniku
            /// </summary>
            /// <param name="variable">
            /// Obiekt zmiennej
            /// </param>
            /// <param name="newValue">
            /// Nowa wartosc zmiennej
            /// </param>
            public void SetVariableValueInPLC(VariableBase variable, object newValue)
            {
                if (!variables.Contains(variable))
                {
                    throw new InvalidOperationException("There is no such variable in variables collection");
                }

                variable.SetValueInPLC(newValue);
            }

            /// <summary>
            /// Metoda odswiezajaca wartosc zmiennej
            /// </summary>
            /// <param name="variableName">
            /// Nazwa zmiennej
            /// </param>
            /// <returns>
            /// Wartosc zmiennej
            /// </returns>
            public object RefreshValueInVariable(string variableName)
            {
                VariableBase variable = GetVariable(variableName);
                if (variable == null)
                {
                    throw new InvalidOperationException(String.Format("There is no variable named {0} in current S7Device", variableName));
                }

                RefreshValueInVariable(variable);
                return variable.Value;
            }

            /// <summary>
            /// Metoda odswiezajaca wartosc zmiennej
            /// </summary>
            /// <param name="variableIndex">
            /// Indeks zmiennej
            /// </param>
            /// <returns>
            /// Wartosc zmiennej
            /// </returns>
            public object RefreshValueInVariable(int variableIndex)
            {
                if (variableIndex >= variables.Count || variableIndex < 0)
                {
                    throw new InvalidOperationException(String.Format("Invalid variable index - There is only {0} variables in device", variables.Count));
                }

                RefreshValueInVariable(variables[variableIndex]);
                
                return variables[variableIndex].Value;
            }

            /// <summary>
            /// Metoda odswiezajaca wartosc zmiennej
            /// </summary>
            /// <param name="variable">
            /// Obiekt zmiennej
            /// </param>
            /// <returns>
            /// Wartosc zmiennej
            /// </returns>
            public object RefreshValueInVariable(VariableBase variable)
            {
                if (!variables.Contains(variable))
                {
                    throw new InvalidOperationException("There is no such variable in variables collection");
                }

                variable.RefreshValue();
                return variable.Value;
            }

        }
}
