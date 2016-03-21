using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace S7TCP
{
    /// <summary>
    /// Klasa obiektu probkujacego odczyt zmiennych
    /// </summary>
    public class SamplerReader:SamplerBase
    {
        
        /// <summary>
        /// Metoda wykonywana co każdy cykl probkowania, pod warunkiem ze stan obiektu jest Connected
        /// </summary>
        protected override void onTimerTick()
        {
            //Jezeli jeszcze nie zakonczono poprzedniego probkowania, nie ma sensu tworzenie kolejnego zadania
            if (InProgress)
            {
                return;
            }


            //Synchronizacja dostepu do polaczenia
            lock(lockingObject)
            {
                InProgress = true;

                //Jezeli polaczenie nie jest polaczone nie ma sensu kontynuacji operacji
                if (connection.State != StatusType.Connected)
                {
                    InProgress = false;
                    return;
                }

                //Stworzenie obiektu jednostki polaczenia do ktorej beda dodawane kolejne zadania
                pduObject = connection.libnodaveConnection.prepareReadRequest();


                for (int i = 0; i < Variables.Count; i++)
                {
                    //Dodawania kolejnych żądań od wszystkich zmiennych obiektu probkujacego
                    Variables[i].AddSamplerReadRequest(pduObject);
                }

                //Pobranie resultatu (kodu bledu) proby wykonania stworzonego wyzej zadania
                int result = connection.libnodaveConnection.execReadRequest(pduObject, resultObject);

                //Jezeli wystapil blad - polaczenie nie dziala poprawnie
                if (result != 0)
                {
                    Console.WriteLine("Error");
                    connection.SetStatusToFalut("libnodave error: " + libnodave.daveStrerror(result));
                    pduObject.freePointer();
                    InProgress = false;
                    return;
                }
                else
                {
                    //Jezeli wszystko dziala poprawnie - Stan obiektu probkujacego powinien byc polaczony
                    SetNewStatus(StatusType.Connected);
                }

                //Pobranie wszystkich rezultatow i przypisanie ich zmiennych ktore je dodalu do obiektu PDU

                for (int i = 0; i < Variables.Count; i++)
                {
                    result = connection.libnodaveConnection.useResult(resultObject, i);
                    //Jezeli nie udalo sie pobrac rezultatu - oznacza to ze jest blad ze zmienna
                    if (result != 0)
                    {
                        InProgress = false;
                        Variables[i].SetNewStatus(StatusType.Fault, "Libnodave error: " + libnodave.daveStrerror(result));
                        continue;
                    }
                    else
                    {
                        Variables[i].SetNewStatus(StatusType.Connected);
                    }

                    try
                    {
                        //Proba przypisania zmiennej wartosci
                        Variables[i].OnSamplerReadRequest();
                    }
                    catch(Exception ex)
                    {
                        Variables[i].SetNewStatus(StatusType.Fault, ex.Message);
                        continue;
                    }
                }

                //Zwolenienie zasobow 
                resultObject.freeResult();

                pduObject.freePointer();

                InProgress = false;
            }


        }

        /// <summary>
        /// Konstruktor klasy obiektu probkujacego odczytujacego zmienne ze sterownika
        /// </summary>
        /// 
        /// <param name="name">
        /// Nazwa polaczenia
        /// </param>
        /// 
        /// <param name="connectionObject">
        /// Obiekt polaczenia
        /// </param>
        /// 
        /// <param name="sampleTime">
        /// Czas probkowania [ms]
        /// </param>
        /// 
        /// <param name="lockingObject">
        /// Obiekt synchronizujacy dostep do polaczenia
        /// </param>
        public SamplerReader(string name, Connection connectionObject, int sampleTime, Object lockingObject)
            : base(name, connectionObject, sampleTime, lockingObject)
        {

        }

    }
}
