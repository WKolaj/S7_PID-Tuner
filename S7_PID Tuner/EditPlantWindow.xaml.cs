using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using TransferFunctionLib;

namespace S7_PID_Tuner
{
    /// <summary>
    /// Okno edycji transmitancji
    /// </summary>
    public partial class EditPlantWindow : Window
    {
        /// <summary>
        /// Wlasciwosc determinujaca czy od otwarcia okna parametry transmitancji zostaly zmienione
        /// </summary>
        public bool Changed
        {
            get;
            private set;
        }

        /// <summary>
        /// Typ wyswietlanej transmitancji - ciagla/dyskretna
        /// </summary>
        public SystemType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// Modyfikowany uklad dynamiczny
        /// </summary>
        public DynamicSystem ModyfiedDynamicSystem
        {
            get;
            private set;
        }

        /// <summary>
        /// Kontruktor okna edycji transmitancji
        /// </summary>
        /// <param name="ds">
        /// Modyfikowany uklad dynamiczny
        /// </param>
        /// <param name="type">
        /// Typ wyswietlania ukladu
        /// </param>
        public EditPlantWindow(DynamicSystem ds, SystemType type)
        {
            //Inicjalizacja formularza
            InitializeComponent();

            //Przypisanie kompomenentow
            this.ModyfiedDynamicSystem = ds;
            this.Type = type;

            //Odswiezenie wsywietlanych wspolczynnikow
            RefreshDS();

            //Inicjalizacja mechanizmu kontroli zmiany transmitancji
            InitWindowsMechanism();

            Changed = false;
        }

        /// <summary>
        /// Metoda inicjalizujaca mechanizm zmiany transmitancji - jezeli klikniety zostanie przycisk zmiany typu transmitancji badz jej parametry - nalezy przy kliknieciu OK
        /// zmienic ja w kontrolce ukladu dynamicznego
        /// </summary>
        private void InitWindowsMechanism()
        {
            NominatorTextBox.TextChanged += TextChanged;
            DenominatorTextBox.TextChanged += TextChanged;
            TimeDelayTextBox.TextChanged += TextChanged;
        }

        /// <summary>
        /// Metoda odswiezajaca wyswietlane wspolczynniki transmitancji
        /// </summary>
        private void RefreshDS()
        {
                if(Type == SystemType.Continues)
                {

                    if (ModyfiedDynamicSystem != null)
                    {
                        NominatorTextBox.Text = ModyfiedDynamicSystem.ContinousNominatorVectorString;
                        DenominatorTextBox.Text = ModyfiedDynamicSystem.ContinousDenominatorVectorString;
                        TimeDelayTextBox.Text = ModyfiedDynamicSystem.ContinousTimeDelay.ToString();
                        SampleTimeTextBox.Text = ModyfiedDynamicSystem.SimulationSampleTime.ToString();
                    }
                }
                else if (Type == SystemType.Discrete)
                {

                    if (ModyfiedDynamicSystem != null)
                    {
                        NominatorTextBox.Text = ModyfiedDynamicSystem.DiscreteNominatorVectorString;
                        DenominatorTextBox.Text = ModyfiedDynamicSystem.DiscreteDenominatorVectorString;
                        TimeDelayTextBox.Text = ModyfiedDynamicSystem.DiscreteTimeDelay.ToString();
                        SampleTimeTextBox.Text = ModyfiedDynamicSystem.SimulationSampleTime.ToString();
                    }
                }

            //Pod odswiezeniu wspolczynnikow nalezy rowniez odswiezyc przyciski typu transmitancji
            RefreshType();
        }

        /// <summary>
        /// Metoda odswiezajaca przyciski typu transmitancji - blokuje odpowiedni
        /// </summary>
        public void RefreshType()
        {
            if(Type == SystemType.Continues)
            {
                TransferFunctionName.Content = "Continous Transfer Function";
                ContinousButton.IsEnabled = false;
                ToDiscreteButton.IsEnabled = true;

            }
            else if(Type == SystemType.Discrete)
            {
                TransferFunctionName.Content = "Discrete Transfer Function";
                ContinousButton.IsEnabled = true;
                ToDiscreteButton.IsEnabled = false;

            }
        }

        /// <summary>
        /// Metoda wywolywana przy wcisnieciu przycisku ToDiscrete - przeksztalca typ wyswietlanej transmitancji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToDiscreteButtonClicked(object sender, RoutedEventArgs e)
        {
            //Kazde wcisniecie tego przycisku powoduje wylapanie wprowadzenia zmian
            Changed = true;
            Type = SystemType.Discrete;
            RefreshType();
        }

        /// <summary>
        /// Metoda wywolywana przy wcisnieciu przycisku ToContinous - przeksztalca typ wyswietlanej transmitancji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToContinousButtonClicked(object sender, RoutedEventArgs e)
        {
            //Kazde wcisniecie tego przycisku powoduje wylapanie wprowadzenia zmian
            Changed = true;
            Type = SystemType.Continues;
            RefreshType();
        }

        /// <summary>
        /// Metoda wywolywana gdy zawartosc ktoregos z textbox'ow ulega zmianie - zauwaza wprowadzanie zmian w transmitancji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            Changed = true;
        }

        /// <summary>
        /// Metoda wywolywana przy nacisnieciu przycisku Apply
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyButtonClicked(object sender, RoutedEventArgs e)
        {
            //Transmitancje nalezy tworzyc na nowo tylko w przypadku gdy zostaly w niej wprowadzone zmiany
            if(Changed)
            {
                //Proba stworzenia transmitanji - jezeli sie nie uda nalezy zakonczyc te metode
                if(!TryCreateDynamicSystem())
                {
                    return;
                }
            }

            //Zaakceptowanie okna
            DialogResult = true;
        }

        /// <summary>
        /// Metoda anulujaca wprowadzanie modyfikacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButtonClicked(object sender, RoutedEventArgs e)
        {
            //Zakonczenie wykonywania okna
            DialogResult = false;
        }

        /// <summary>
        /// Metoda zapisujaca podana transmitancje do pliku - wywolywana podczas przyciskania przycisku Save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButtonClicked(object sender, RoutedEventArgs e)
        {
            //Jezeli Transmitancja nie zostala okreslona w konstruktorze oraz nie wprowadzono zmian - nalezy zglosic blad
            if (!Changed && ModyfiedDynamicSystem == null)
            {
                System.Windows.MessageBox.Show("No transfer function specified", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
                
            //Otwarcie okna do zapisu pliku 
            SaveFileDialog saveFileDialogWindow = new SaveFileDialog();
            XDocument XMLFile;
            Stream writeStream;
            //Ustawienie filtra wyswietlanych w tym oknie wartosci
            saveFileDialogWindow.Filter = "Dynamic system files (*.ds)|*.ds";
            saveFileDialogWindow.FilterIndex = 0;
            //Ustawienie domyslnej sciezki tego okno jako projekt\Projects
            saveFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Projects");

            //Jezeli transmitancja nie zostala zmodyfikowana - w celu zachowania wiekszej dokladnosci ( brak konwersji DOuble na string i odwrotnie)
            //Stworzyc plik XML na podstawie wspolczynnikow transmitancji , a nie pola tekstowego ze wspolczynnikami
            if (!Changed)
            {
                if (saveFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    writeStream = saveFileDialogWindow.OpenFile();
                    XMLFile = ModyfiedDynamicSystem.ToXML(Type);
                    XMLFile.Save(writeStream);

                    //Zakonczenie pracy ze strumieniem
                    writeStream.Flush();
                    writeStream.Close();
                }
            }
            else
            {
                //Jezeli zostaly wprowadzone zmiany nalezy sprobowac Stworzyc nowa transmitancje
                if(!TryCreateDynamicSystem())
                {
                    return;
                }

                //Jezeli okno zapisu zostalo zaakceptowane nalezy stworzyc reprezentujacy obiekt dynamiczny dokument XML i zapisac go w podanym przez uzytkownika sciezce
                if (saveFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    writeStream = saveFileDialogWindow.OpenFile();
                    XMLFile = ModyfiedDynamicSystem.ToXML(Type);
                    XMLFile.Save(writeStream);

                    //Zakonczenie pracy ze strumieniem
                    writeStream.Flush();
                    writeStream.Close();
                }
            }
        }
        
        /// <summary>
        /// Metoda probujaca Stworzyc na podstawie pol tekstowych nowa transmitancje - jezeli sie to nie uda wyswietla okno bledu i zwraca false
        /// </summary>
        /// <returns>
        /// Czy udalo sie stworzyc transmitancje
        /// </returns>
        private bool TryCreateDynamicSystem()
        {
            try
            {
                if (Type == SystemType.Continues)
                {
                    ModyfiedDynamicSystem = DynamicSystem.FromContinousFactorsString(NominatorTextBox.Text, DenominatorTextBox.Text, TimeDelayTextBox.Text, SampleTimeTextBox.Text);
                }
                else if (Type == SystemType.Discrete)
                {
                    ModyfiedDynamicSystem = DynamicSystem.FromDiscreteFactorsString(NominatorTextBox.Text, DenominatorTextBox.Text, TimeDelayTextBox.Text, SampleTimeTextBox.Text);
                }

            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Metoda wczytujaca transmitancje z pliku XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadButtonClicked(object sender, RoutedEventArgs e)
        {
            //Otwarcie okna do wskazania pliku
            OpenFileDialog readFileDialogWindow = new OpenFileDialog();

            readFileDialogWindow.Filter = "Dynamic system files (*.ds)|*.ds";
            readFileDialogWindow.FilterIndex = 0;
            readFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Projects");

            //Jezeli okno to zostalo zaakceptowane - nalezy wczytac zawartosc pliku
            if (readFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Stream readStream = readFileDialogWindow.OpenFile();
                XDocument xmlDocument = XDocument.Load(readStream);
                
                try
                {
                    //Sprawdzic typ ukladu w tym pliku - dyskretny/ciagly
                    Type = FileType(xmlDocument);
                }
                catch(Exception exception)
                {
                    System.Windows.MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //Stworzyc nowa transmitancje
                ModyfiedDynamicSystem = DynamicSystem.FromXML(xmlDocument);

                // i ja odswiezyc
                RefreshDS();

                //Zakonczenie pracy ze strumieniem
                readStream.Close();
                Changed = true;
            }
        }

        /// <summary>
        /// Metoda Zwracajaca typ ukladu dynamicznego na podstawie dokumentu XML podanego jako jej argument
        /// </summary>
        /// <param name="document">
        /// Obiekt XML reprezentujacy dany uklad dynamiczny
        /// </param>
        /// <returns>
        /// Typ ukladu dynamicznego
        /// </returns>
        public static SystemType FileType(XDocument document)
        {
            if(document == null)
            {
                throw new InvalidOperationException("File is null");
            }

            //Pobranie i sprawdzenie glownego obiektu tego dokumentu
            if(document.Root.Name == "DiscreteDynamicSystem" )
            {
                return SystemType.Discrete;
            }
            else if (document.Root.Name == "ContinousDynamicSystem")
            {
                return SystemType.Continues;
            }
            else
            {
                throw new InvalidOperationException(String.Format("Invalid file format: root value = {0}", document.Root.Name));
            }
        }

    }
}

