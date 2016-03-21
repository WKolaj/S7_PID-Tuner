using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7TCP
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa obiektu, ktory moze zmieniac swoj stan
    /// </summary>
    public abstract class ObjectWithStateBase
    {
        /// <summary>
        /// Zdarzenie zglaszane za kazdym razem, gdy status obiektu ulega zmianie
        /// </summary>
        public event ObjectStateChangedEventHandler StatusChanged;

        /// <summary>
        /// Status obiektu
        /// </summary>
        private StatusType state;

        /// <summary>
        /// Status obiektu
        /// </summary>
        public StatusType State
        {
            get
            {
                return state;
            }

            protected set
            {
                state = value;
            }

        }

        /// <summary>
        /// Komentarz do zmienionego statusu
        /// </summary>
        private string comment;

        /// <summary>
        /// Komentarz do zmienionego statusu
        /// </summary>
        public String StateComment
        {
            get
            {
                return comment;
            }

            protected set
            {
                comment = value;
            }
        }

        /// <summary>
        /// Metoda ustawiajaca nowy status
        /// </summary>
        /// <param name="newState">
        /// Obiekt nowego statusu
        /// </param>
        /// <param name="newStateMessage">
        /// Nowy komentarz statusu
        /// </param>
        protected internal void SetNewStatus(StatusType newState, String newStateMessage = "no errors")
        {
            //Jeżeli nowy status lub komentarz rozni sie od poprzedniego ustawiany jest ustawiane są jego nowe wartosc
            if (StateComment != newStateMessage || State != newState)
            {
                StateComment = newStateMessage;
                State = newState;

                //Oraz zgloszone zostaje zdarzenie zmiany statusu
                if (StatusChanged != null)
                {
                    StatusChanged(this, new ObjectStatusChangedEventArgument(newState, newStateMessage));
                }
            }
        }

        /// <summary>
        /// Konstruktor klasy bazowej obiektu posiadajacego status
        /// </summary>
        public ObjectWithStateBase()
        {
            //Domyslnie obiekt jest rozlaczony i nie posiada zadnych bledow
            state = StatusType.Disconnected;
            comment = "no error";
        }

    }

}
