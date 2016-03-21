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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using TransferFunctionLib;

namespace S7_PID_Tuner
{
    /// <summary>
    /// Interaction logic for Process.xaml
    /// </summary>
    public partial class PlantControl : UserControl
    {
        /// <summary>
        /// Aktualnie uzywany projekt
        /// </summary>
        private Project currentProject;

        /// <summary>
        /// Konstruktor kontrolki obiektu regulacji
        /// </summary>
        public PlantControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zmiany obiektu regulacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnPlantChanged(object sender, EventArgs eventArgument)
        {
            //W przypadku gdy transmitancja obiektu regulacji ulegla zmianie - nalezy odswiezyc jej wyswietlanie
            RefreshPlantDisplaing();
        }

        /// <summary>
        /// Metoda odswiezajaca wsyweitlana transmitancje
        /// </summary>
        public void RefreshPlantDisplaing()
        {
            //Odswiezenie pola transmitancji
            transferFunctionControl.RefreshTransferFunctionDisplaying();
        }

        /// <summary>
        ///Metoda podlaczajaca zdarzenie zmiany obiektu regulacji do metody jego obslugi odswiezajacej wyswietlanie transmitancji
        /// </summary>
        public void ConnectToPlantEvent()
        {
            if(currentProject!=null)
            {
                currentProject.PlantChangedEvent += OnPlantChanged;
            }
        }

        /// <summary>
        /// Metoda odlaczajaca zdarzenie zmiany obiektu regulacji od metody jego obslugi odswiezajacej wyswietlanie transmitancji
        /// </summary>
        public void DisconnectToPlantEvent()
        {
            if (currentProject != null)
            {
                currentProject.PlantChangedEvent -= OnPlantChanged;
            }
        }

        /// <summary>
        /// Metoda przypisujaca projekt do kontrolki
        /// </summary>
        /// <param name="currentProject">
        /// Aktualny projekt
        /// </param>
        public void AssignProjectToControl(Project currentProject)
        {
            //Rozlaczenie zdarzen aktualnego projektu - a raczej jego obiektu regulacji od metod obslugi
            DisconnectToPlantEvent();
            this.currentProject=currentProject;

            //Polaczenie zdarzen aktuanlego projektu - a raczej jego obiektu regulacji do metod obslugi
            ConnectToPlantEvent();

            //Przypisanie tego projektu rowniez do kontrolki transmitancji
            transferFunctionControl.AssignNewProject(this.currentProject);
        }

        /// <summary>
        /// Metoda obslugi zdarzenia pojawienia sie kursora myszy nad kontrolka obiektu regulacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseOverMainGrid(object sender, MouseEventArgs e)
        {
            //Uruchomienie animacji - zmiany koloru tła kontrolki
            Storyboard processPlantMouseOver = (Storyboard)Resources["processPlantMouseOver"];

            transferFunctionControl.ChangeColor("#F7FBFD");

            processPlantMouseOver.Begin();
        }

        /// <summary>
        /// Metoda obslugi opuczczenia przez kursor myszy kontrolki obiektu regulacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeftMainGrid(object sender, MouseEventArgs e)
        {
            //Wyswietlenie animacji odpowiedajacej temu zdarzeniu - powrot koloru tla do poprzedniego
            Storyboard processPlantMouseLeft = (Storyboard)Resources["processPlantMouseLeft"];

            processPlantMouseLeft.Begin();

            transferFunctionControl.ChangeColor("#FFFFFF");
        }

        /// <summary>
        /// Metoda obslugi klikniecia na kontrolke przyciskiem myszy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Zgloszenie zdarzenia klikniecia podwojnego jezeli kliknieto dwa razy lewy przycisk myszy
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                OnMouseDoubleClicked(sender,e);
            }
        }

        /// <summary>
        /// Metod obslugi zdarzenia podwojnego kliknicia myszy - nalezy wyswietlic okno edycji transmitancji obiektu regulacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            //Jezeli kliknieto dwa razy lewy przycisk myszy - 
            transferFunctionControl.OpenEditWinow();
        }

        /// <summary>
        /// Metoda zmieniajaca kolor obramowania - swykorzystywana podczas najechania myszka na kontrolke
        /// </summary>
        /// <param name="colorCode"></param>
        private void ChangeBroderColor(String colorCode)
        {
            MainBorder.BorderBrush = (Brush)(new BrushConverter()).ConvertFrom(colorCode);
        }

        /// <summary>
        /// Metoda obslugi edycji z pola kontekstowego wyswietlanego po prawym przycisnieciu myszy na kontrolke
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditClicked(object sender, RoutedEventArgs e)
        {
            EditPlantObject();
        }

        /// <summary>
        /// Metoda obslugi wczytania nowego obiektu regulacji z pola kontekstowego wyswietlanego po prawym przycisnieciu myszy na kontrolke
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadClicked(object sender, RoutedEventArgs e)
        {
            LoadNewPlantObject();
        }

        /// <summary>
        /// Metoda obslugi zapisu nowego obiektu regulacji z pola kontekstowego wyswietlanego po prawym przycisnieciu myszy na kontrolke
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            SaveNewPlantObject();
        }

        /// <summary>
        /// Metoda edytujaca transmitancje obiektu regulacji
        /// </summary>
        public void EditPlantObject()
        {
            EditWindowShow();
        }

        /// <summary>
        /// Metoda wczytania nowego obiektu regulacji
        /// </summary>
        public void LoadNewPlantObject()
        {
            //Wyswietlenie okna wczytania
            LoadWindowShow();
        }

        /// <summary>
        /// Metoda zapisujaca obiekt regulacji
        /// </summary>
        public void SaveNewPlantObject()
        {
            SaveWindowShow();

        }

        /// <summary>
        /// Metoda wyswietlajca okno edycji obiektu regulacji
        /// </summary>
        private void EditWindowShow()
        {
            //Wyswietlenie okna edycji transmitancji
            transferFunctionControl.OpenEditWinow();
        }

        /// <summary>
        /// Metoda wyswietlajca okno wyboru nowego obiektu regulacji
        /// </summary>
        private void LoadWindowShow()
        {
            //Otwarcie okna do wskazania pliku
            System.Windows.Forms.OpenFileDialog readFileDialogWindow = new System.Windows.Forms.OpenFileDialog();

            readFileDialogWindow.Filter = "Dynamic system files (*.ds)|*.ds";
            readFileDialogWindow.FilterIndex = 0;
            readFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Projects");

            //Jezeli okno to zostalo zaakceptowane - nalezy wczytac zawartosc pliku
            if (readFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Stream readStream = readFileDialogWindow.OpenFile();
                XDocument xmlDocument = XDocument.Load(readStream);
                SystemType type;
                DynamicSystem newDs;

                try
                {
                    type = EditPlantWindow.FileType(xmlDocument);

                    //Stworzyc nowa transmitancje
                    newDs = DynamicSystem.FromXML(xmlDocument);
                }
                catch (Exception exception)
                {
                    System.Windows.MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                transferFunctionControl.AssignDynamicSystem(newDs, type);

                readStream.Close();
            }
        }

        /// <summary>
        /// Metoda wyswietlajaca okno wyboru sciezki zapisu nowego projektu
        /// </summary>
        private void SaveWindowShow()
        {
            if (currentProject.PlantObject == null)
            {
                return;
            }

            //Otwarcie okna do zapisu pliku 
            System.Windows.Forms.SaveFileDialog saveFileDialogWindow = new System.Windows.Forms.SaveFileDialog();
            XDocument XMLFile;
            Stream writeStream;
            //Ustawienie filtra wyswietlanych w tym oknie wartosci
            saveFileDialogWindow.Filter = "Dynamic system files (*.ds)|*.ds";
            saveFileDialogWindow.FilterIndex = 0;
            //Ustawienie domyslnej sciezki tego okno jako projekt\Projects
            saveFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Projects");

            if (saveFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                writeStream = saveFileDialogWindow.OpenFile();
                XMLFile = currentProject.PlantObject.ToXML(currentProject.Type);
                XMLFile.Save(writeStream);

                //Zakonczenie pracy ze strumieniem
                writeStream.Flush();
                writeStream.Close();
            }

        }

        /// <summary>
        /// Metoda obslugi zdarzenia wsywietlania kontekstowego menu podczas klikniecia prawym przyciskiem myszy na kontrolke
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            //Filtracja menu kontekstowego
            FilterContextMenu();  
        }

        /// <summary>
        /// Metoda filtrujaca menu kontekstowe - uniemozliwajaca zapisanie pustego obiektu regulacji
        /// </summary>
        private void FilterContextMenu()
        {
            if (this.currentProject == null)
            {
                SaveMenuItem.IsEnabled = false;
            }
            else if (currentProject.PlantObject == null)
            {
                SaveMenuItem.IsEnabled = false;
            }
            else
            {
                SaveMenuItem.IsEnabled = true;
            }
        }

        /// <summary>
        /// Metoda zaznaczajaca kontrolke obiektu regulacji
        /// </summary>
        public void Select()
        {
            ChangeBroderColor("#3366D3");
        }

        /// <summary>
        /// Metoda odznaczajaca kontrolke obiektu regulacji
        /// </summary>
        public void Unselect()
        {
            ChangeBroderColor("#FFFFFF");
        }
    }
}
