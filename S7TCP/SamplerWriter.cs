using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7TCP
{
    /// <summary>
    /// Klasa obiektu probkujacego zapis zmiennych 
    /// </summary>
    public class SamplerWriter:SamplerBase
    {
        /// <summary>
        /// Konstruktor klasy obiektu probkujacego zapisujacego zmienne
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
        public SamplerWriter(string name, Connection connectionObject, int sampleTime, Object lockingObject) :
            base(name, connectionObject, sampleTime, lockingObject)
        {

        }

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
                pduObject = connection.libnodaveConnection.prepareWriteRequest();

                for (int i = 0; i < Variables.Count; i++)
                {
                    //Dodawania kolejnych żądań od wszystkich zmiennych obiektu probkujacego
                    Variables[i].AddSamplerWriteRequest(pduObject);
                }

                //Pobranie resultatu (kodu bledu) proby wykonania stworzonego wyzej zadania
                int result = connection.libnodaveConnection.execWriteRequest(pduObject, resultObject);

                //Jezeli wystapil blad - polaczenie nie dziala poprawnie
                if (result != 0)
                {
                    SetNewStatus(StatusType.Fault, libnodave.daveStrerror(result));
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

                //Zwolenienie zasobow 
                resultObject.freeResult();
                pduObject.freePointer();

                InProgress = false;

                }
        }
    }
}
