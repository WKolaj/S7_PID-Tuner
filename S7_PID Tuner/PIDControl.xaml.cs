using S7TCP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
    /// Interaction logic for PIDControl.xaml
    /// </summary>
    public partial class PIDControl : UserControl
    {
        /// <summary>
        /// Obiekt sterownika pozwalajacy sie polaczyc z urzadzeniem
        /// </summary>
        private PIDControllerDevice controllerDevice;

        /// <summary>
        /// Metoda przypisujaca obiekt sterownika do kontrolki
        /// </summary>
        /// <param name="controllerDevice">
        /// Przypisywany obiekt sterownika
        /// </param>
        public void AssignControllerToDevice(PIDControllerDevice controllerDevice)
        {
            this.controllerDevice = controllerDevice;
            ConnectToDeviceEvent();
            OnlineStatusLabel2.DataContext = controllerDevice;
        }

        private PerformanceIndexCollection performanceIndexes;
        public void AssingPerformanceIndexes(PerformanceIndexCollection performanceIndexes)
        {
            this.performanceIndexes = performanceIndexes;
        }

        private bool online;
        /// <summary>
        /// Wlasciwosc okresjalaca czy sterownika jest w trybie online
        /// </summary>
        private bool Online
        {
            get
            {
                return online;
            }

            set
            {
                online = value;
            }
        }

        /// <summary>
        /// Wlasciwosc okreslajaca czy sterownik jest polaczony z aplikacja
        /// </summary>
        private bool Connected
        {
            get
            {
                if(controllerDevice != null)
                {
                    return controllerDevice.Connected;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia odswiezania wartosc wzmocnienia - wystarczy odswiezenie jednej wartosci zeby inicjowac odswiezenie wszystkich TextBoxów
        /// </summary>
        private void OnOneOfParametersUpdated()
        {
            //Jezeli jest wlaczony tryb Online - nalezy stworzyc regulator z pobranych wartosci, wszystkie texboxy odswieza sie wtedy same bo zostanie zgloszone zdarzenie ControllerChanged Projektu
            if(Online)
            {
                CreateControllerFromOnlineParameters();
            }
        }

        /// <summary>
        /// Metoda tworzaca regulator na podstawie parametrow pobranych Online
        /// </summary>
        private void CreateControllerFromOnlineParameters()
        {
            //W zaleznosci od tego czy istnieje obiekt regulacji - rozne czasy probkowania symulacji
            if (currentProject.PlantObject != null)
            {
                currentProject.Controller = new PIDController(controllerDevice.Kp, controllerDevice.Ti, controllerDevice.Td, controllerDevice.N, currentProject.PlantObject.SimulationSampleTime, RefreshOnlineMode(), controllerDevice.Inverted);
            }
            else
            {
                currentProject.Controller = new PIDController(controllerDevice.Kp, controllerDevice.Ti, controllerDevice.Td, controllerDevice.N, 0.1, RefreshOnlineMode(), controllerDevice.Inverted);
            }
        }

        /// <summary>
        /// Aktualnie wybrany projekt
        /// </summary>
        private Project currentProject;

        /// <summary>
        /// Metoda obslugi zmiany transmitancji regulatora 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgument"></param>
        private void OnControllerChanged(object sender, EventArgs eventArgument)
        {
            //Odswiezenie wyswietlania regulatora
            RefreshControllerDisplaying();
        }

        /// <summary>
        /// Metoda laczaca zdarzenie zmiany transmitancji regulatora z metoda jego obslugi - odswiezajacej wyswietlanie
        /// </summary>
        private void ConnectToControllerEvent()
        {
            if(currentProject!=null)
            {
                currentProject.ControllerChangedEvent += OnControllerChanged;
            }
        }

        /// <summary>
        /// Metoda podlaczajaca zdarzenia urzadzenia do metod ich obslugi
        /// </summary>
        private void ConnectToDeviceEvent()
        {
            if (controllerDevice != null)
            {
                controllerDevice.ConnectionStateChanged += OnDeviceConnectionChanged;
                controllerDevice.TuningModeStateChanged += OnTuningModeChanged;

                //UI wystarczy odswiezac tylk jednym zdarzeniem - i tak stempluje co okrelsony czas
                controllerDevice.KpUpdated += OnOneOfParametersUpdated;
                
            }
        }

        /// <summary>
        /// Metoda rozlaczajaca zdarzenia obiektu sterownika od metod ich obslugi
        /// </summary>
        private void DisconnectToDeviceEvent()
        {
            if (controllerDevice != null)
            {
                controllerDevice.ConnectionStateChanged -= OnDeviceConnectionChanged;
                controllerDevice.TuningModeStateChanged -= OnTuningModeChanged;

                //UI wystarczy odswiezac tylk jednym zdarzeniem - i tak stempluje co okrelsony czas
                controllerDevice.KpUpdated -= OnOneOfParametersUpdated;

            }
        }

        /// <summary>
        /// Metoda wywolywana przy zdarzeniu zmiany stanu polaczenia - zeby odswiezyc UI
        /// </summary>
        private void OnDeviceConnectionChanged()
        {
            RefreshLabelColor();
            RefreshOnlineTextBoxesEnable();
            RefreshTextBoxEnable();
        }

        /// <summary>
        /// Metoda wywolywana podczas zmiany trybu Tuning/Normal obiektu sterownika
        /// </summary>
        private void OnTuningModeChanged()
        {

        }

        /// <summary>
        /// Metoda odswiezajaca kolor glownej etykiety w zaleznosci od trybu pracy
        /// </summary>
        private void RefreshLabelColor()
        {
            var bc = new BrushConverter();

            if(Connected && Online)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    LabelName.Background = (Brush)bc.ConvertFrom("#FFFF9E00");
                })); 

            }
            else
            {

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    LabelName.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCACCD9"));
                })); 
            }
        }

        /// <summary>
        /// Metoda filtrujaca przyciski Online, Offline i Download w zaleznosci od trybu pracy
        /// </summary>
        private void FilterOnlineButtons()
        {
            if(Online)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    buttonOnline.IsEnabled = false;
                    buttonOffline.IsEnabled = true;
                    buttonDownload.IsEnabled = false;
                })); 
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    buttonOnline.IsEnabled = true;
                    buttonOffline.IsEnabled = false;
                    buttonDownload.IsEnabled = true;
                })); 
            }
        }

        /// <summary>
        /// Metoda odswiezajaca textboxy podczas trybu online - okresla czy maja one byc zablokowane czy udostepnione
        /// </summary>
        private void RefreshOnlineTextBoxesEnable()
        {
            if(controllerDevice != null)
            {
                if (Connected)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        invertedCheckBox.IsEnabled = false;
                        PIDBox.IsEnabled = false;
                        KpTextBox.IsEnabled = false;
                        TiTextBox.IsEnabled = false;
                        TdTextBox.IsEnabled = false;
                        NTextBox.IsEnabled = false;

                    })); 
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        PIDBox.IsEnabled = true;
                        KpTextBox.IsEnabled = true;
                        TiTextBox.IsEnabled = true;
                        TdTextBox.IsEnabled = true;
                        NTextBox.IsEnabled = true;
                        invertedCheckBox.IsEnabled = true;
                    })); 
                }
            }
        }

        /// <summary>
        /// Metoda odswiezajaca tryb algorytmu regulatora podczas trybu Online
        /// Sprawdza czy wartosci w texboxach nie przekracza wartosci granicznej czlonu calkujacego - wtedy nie jest to juz regulator np. PI a P
        /// oraz czy Td nie jest rowne 0
        /// </summary>
        /// <returns>
        /// Odpowiadajacy typ regulatora
        /// </returns>
        private PIDModeType RefreshOnlineMode()
        {
            if (controllerDevice.Ti >= controllerDevice.TiForNonI)
            {
                if (controllerDevice.Td == 0)
                {
                    return PIDModeType.P;
                }
                else
                {
                    return PIDModeType.PD;
                }
            }
            else
            {
                if (controllerDevice.Td == 0)
                {
                    return PIDModeType.PI;
                }
                else
                {
                    return PIDModeType.PID;
                }
            }
        }

        /// <summary>
        /// Metoda rozlaczajaca zdarzenie zmiany transmitancji regulatora z metoda jego obslugi - odswiezajacej wyswietlanie
        /// </summary>
        public void DisconnectToControllerEvent()
        {
            if (currentProject != null)
            {
                currentProject.ControllerChangedEvent -= OnControllerChanged;
            }
        }

        /// <summary>
        /// Metoda przypisujaca kontrolce nowy projekt
        /// </summary>
        /// <param name="currentProject">
        /// Obiekt projektu
        /// </param>
        public void AssignProjectToControl(Project currentProject)
        {
            //Rozlaczenie zdarzen poprzedniego projektu
            DisconnectToControllerEvent();
            this.currentProject = currentProject;

            //Przypisanie zdarzen nowego projektu
            ConnectToControllerEvent();

            //Odswiezenie wyswietlania nastaw regulatora
            RefreshControllerDisplaying();

            FilterOnlineButtons();
        }

        /// <summary>
        /// Kontruktor klasy kontrolki regulatora PID
        /// </summary>
        public PIDControl()
        {
            //Inicjalizacja kompomenentow UI
            InitializeComponent();

            //Odswiezenie mechanizmu blokady niepotrzebnych pol tekstowych - w zaleznosci od typu algorytmu regulatora
            RefreshTextBoxEnable();

        }

        /// <summary>
        /// Metoda przechodzaca w tryb offline sterownika
        /// </summary>
        public void GoOffline()
        {
            //Odswiezenie wyswietlania etykiety
            RefreshLabelColor();

            //Wylaczenie trybu offline i odswiezenie wyswietlania etykiet oraz rozlaczenie polaczenia
            if (controllerDevice != null)
            {
                Online = false;

                OnlineStatusLabel1.Visibility = System.Windows.Visibility.Hidden;
                OnlineStatusLabel2.Visibility = System.Windows.Visibility.Hidden;
                controllerDevice.Disconnect();
            }
            
            //Odswiezenie pozostalych elementow UI
            FilterOnlineButtons();
            RefreshTextBoxEnable();
            RefreshOnlineTextBoxesEnable();
        }

        /// <summary>
        /// Metoda przechodzaca w tryb online sterownika
        /// </summary>
        public void GoOnline()
        {
            //Wylaczenie trybu offline i odswiezenie wyswietlania etykiet oraz rozlaczenie polaczenia
            if (controllerDevice != null)
            {
                Online = true;

                OnlineStatusLabel1.Visibility = System.Windows.Visibility.Visible;
                OnlineStatusLabel2.Visibility = System.Windows.Visibility.Visible;
                
                if(!Connected)
                {
                    controllerDevice.StatusMessage = "Trying to connect...";
                    controllerDevice.Connect();
                }
                
            }
            //Odswiezenie wyswietlania etykiety
            FilterOnlineButtons();
            RefreshLabelColor();
            RefreshOnlineTextBoxesEnable();
        }

        /// <summary>
        /// Metoda ograniczajaca mozliwosc wpisywania jedynie liczb w pola tekstowego regulatora
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllowOnlyNumber(object sender, TextCompositionEventArgs e)
        {
            Double value;

            //Dokonywana jest proba konwersji wpisanego tekstu powiekszonego o 0 - w celu umozliwienia wpisania przecinka
            if (!Double.TryParse(e.Text + "0",out value))
            {
                e.Handled = true;

            }
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
                OnMouseDoubleClicked(sender, e);
            }
        }

        /// <summary>
        /// Metod obslugi zdarzenia podwojnego kliknicia myszy - nalezy wyswietlic okno sterowania ukladem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            //Wyswietlenie okna ukladu regulacji
            ShowProcessControlWindow();
        }

        /// <summary>
        /// Metoda wyswietlajaca okno ukladu regulacji
        /// </summary>
        private void ShowProcessControlWindow()
        {
            //Rozlaczenie zdarzen - zeby nie zmienialy UI tego okna poki nie zostanie zamkniete nowe
            DisconnectToDeviceEvent();

            //Stworzenie nowego okna i jego inicjalizacja
            ProcessControlWindow processControlWindow = new ProcessControlWindow();
            processControlWindow.AssignControllerToDevice(controllerDevice);
            processControlWindow.AssignIndexesToDevice(performanceIndexes);
            processControlWindow.ShowDialog();

            //Rozlaczenie okna poprzedniego i jego zdarzen
            processControlWindow.GoOffline();
            processControlWindow.DisconnectDeviceEvents();

            //Ponowne polaczenie zdarzen
            ConnectToDeviceEvent();

            //Jezeli byl wczesniej tryb online - nalezy na nowo polaczyc 
            if (Online)
            {
                GoOnline();
            }
            else
            {
                GoOffline();
                RefreshLabelColor();
            }
        }

        /// <summary>
        /// Metoda odswiezajaca wyswietlanie regulatora 
        /// </summary>
        public void RefreshControllerDisplaying()
        {
                //Wyswietlenie regulatora - tylko w przypadku gdy regulator i projekt nie sa puste
                //Jezeli regulator lub projekt sa puste - nalezy wyczyscic pola jego nastaw
                if (currentProject != null)
                {
                    if (currentProject.Controller != null)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                         //Metoda przypisujaca polom tekstowym wartosci nastaw obiektu regulatora
                         KpTextBox.Text = currentProject.Controller.Kp.ToString("G4");
                         TiTextBox.Text = currentProject.Controller.Ti.ToString("G4");
                         TdTextBox.Text = currentProject.Controller.Td.ToString("G4");
                         NTextBox.Text = currentProject.Controller.N.ToString("G4");
                         PIDBox.SelectedIndex = (int)currentProject.Controller.Mode;
                         invertedCheckBox.IsChecked = currentProject.Controller.Inverted;
                        }));
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {

                        KpTextBox.Clear();
                        TiTextBox.Clear();
                        TdTextBox.Clear();
                        NTextBox.Clear();
                        PIDBox.SelectedIndex = 0;
                        invertedCheckBox.IsChecked = false;

                        }));
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                        {

                    KpTextBox.Clear();
                    TiTextBox.Clear();
                    TdTextBox.Clear();
                    NTextBox.Clear();
                    PIDBox.SelectedIndex = 0;
                    invertedCheckBox.IsChecked = false;

                        }));
                }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia pojawienia sie myszy nad kontrolka
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseOverMainGrid(object sender, MouseEventArgs e)
        {
            if (Connected && Online)
            {
                //Wyswietlenie odpowiedniej animacji
                PlayMouseOverMainGridAnimationDuringOnline();
            }
            else
            {
                //Wyswietlenie odpowiedniej animacji
                PlayMouseOverMainGridAnimation();
            }
        }

        /// <summary>
        /// Uruchomienie odpowiedniej animacji odpowiadajcaej zdarzeniu pojawienia sie kursora nad kontrolka - zmiana tla kontrolki
        /// </summary>
        private void PlayMouseOverMainGridAnimation()
        {
            Storyboard processPlantMouseOver = (Storyboard)Resources["processPlantMouseOver"];
            processPlantMouseOver.Begin();
        }

        /// <summary>
        /// Uruchomienie odpowiedniej animacji odpowiadajcaej zdarzeniu pojawienia sie kursora nad kontrolka - zmiana tla kontrolki podczas trybu online
        /// </summary>
        private void PlayMouseOverMainGridAnimationDuringOnline()
        {
            Storyboard processPlantMouseOverDuringOnline = (Storyboard)Resources["processPlantMouseOverDuringOnline"];
            processPlantMouseOverDuringOnline.Begin();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia opuszczenia kursora z pola kontrolki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeftMainGrid(object sender, MouseEventArgs e)
        {
            PlayMouseLeftMainGridAnimation();
        }

        /// <summary>
        /// Uruchomienie odpowiedniej animacji odpowiadajcaej zdarzeniu opuszczenia kursora a pola kontrolki - zmiana tla kontrolki
        /// </summary>
        private void PlayMouseLeftMainGridAnimation()
        {
            Storyboard processPlantMouseLeft = (Storyboard)Resources["processPlantMouseLeft"];
            processPlantMouseLeft.Begin();
        }

        /// <summary>
        /// Metoda obslugi zmiany obramownia kontrolki - wykorzystywana gdy kursor najedzie na kontrolke
        /// </summary>
        /// <param name="colorCode"></param>
        private void ChangeBroderColor(String colorCode)
        {
            MainBorder.BorderBrush = (Brush)(new BrushConverter()).ConvertFrom(colorCode);
        }

        /// <summary>
        /// Metoda zaznaczajaca kontrolke regulatora
        /// </summary>
        public void Select()
        {
            ChangeBroderColor("#3366D3");
        }

        /// <summary>
        /// Metoda odznaczajaca kontrolke regulatora
        /// </summary>
        public void Unselect()
        {
            ChangeBroderColor("#FFFFFF");
        }

        /// <summary>
        /// Metoda sprawdzajaca czy pola tekstowe sa wypelnione poprawnie
        /// </summary>
        /// <returns>
        /// Czy pola tekstowe sa wypelnione poprawnie
        /// </returns>
        private bool AreTextboxesFilledCorectly()
        {
            //Pobranie typu algorytmu regulatora
            PIDModeType Mode = (PIDModeType)PIDBox.SelectedIndex;

            //W zaleznosci od typu sprawdzane sa tylko odpowiednie pola
            switch(Mode)
            {
                case PIDModeType.P:
                    {
                        Double Kp;
                        if (!Double.TryParse(KpTextBox.Text, out Kp))
                        {
                            return false;
                        }

                        break;
                    }
                case PIDModeType.PD:
                    {
                        Double Kp;
                        if (!Double.TryParse(KpTextBox.Text, out Kp))
                        {
                            return false;
                        }

                        Double Td;
                        if (!Double.TryParse(TdTextBox.Text, out Td))
                        {
                            return false;
                        }

                        Double N;
                        if (!Double.TryParse(NTextBox.Text, out N))
                        {
                            return false;
                        }

                        break;
                    }
                case PIDModeType.PI:
                    {
                        Double Kp;
                        if (!Double.TryParse(KpTextBox.Text, out Kp))
                        {
                            return false;
                        }

                        Double Ti;
                        if (!Double.TryParse(TiTextBox.Text, out Ti))
                        {
                            return false;
                        }

                        break;
                    }
                case PIDModeType.PID:
                    {
                        Double Kp;
                        if (!Double.TryParse(KpTextBox.Text, out Kp))
                        {
                            return false;
                        }

                        Double Ti;
                        if (!Double.TryParse(TiTextBox.Text, out Ti))
                        {
                            return false;
                        }

                        Double Td;
                        if (!Double.TryParse(TdTextBox.Text, out Td))
                        {
                            return false;
                        }

                        Double N;
                        if (!Double.TryParse(NTextBox.Text, out N))
                        {
                            return false;
                        }

                        break;
                    }
            }
           
            return true;
        }

        /// <summary>
        /// Metoda tworzaca nowy regulator na podstawie wypelnionych pol
        /// </summary>
        /// <returns>
        /// Czy udalo sie utworzyc regulator
        /// </returns>
        private bool CreateNewController()
        {
            //Sprawdzenie czy pola sa wypelnione poprawnie
            if (AreTextboxesFilledCorectly())
            {
                bool Inverted = (bool)invertedCheckBox.IsChecked;
                PIDModeType Mode = (PIDModeType)PIDBox.SelectedIndex;
                Double Kp = 0;
                Double Ti = 0;
                Double Td = 0;
                Double N = 0;

                if (Double.TryParse(KpTextBox.Text, out Kp))
                {

                }
                else
                {
                    Kp = 0;
                }

                if (Double.TryParse(TiTextBox.Text, out Ti))
                {

                }
                else
                {
                    Ti = 0;
                }

                if (Double.TryParse(TdTextBox.Text, out Td))
                {

                }
                else
                {
                    Td = 0;
                }

                if (Double.TryParse(NTextBox.Text, out N))
                {

                }
                else
                {
                    N = 0;
                }

                if (currentProject.PlantObject != null)
                {
                    currentProject.Controller = new PIDController(Kp, Ti, Td, N, currentProject.PlantObject.SimulationSampleTime, Mode, Inverted);
                }
                else
                {
                    currentProject.Controller = new PIDController(Kp, Ti, Td, N, 0.1, Mode, Inverted);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zmiany algorytmu regulatora PID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PIDBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.currentProject == null)
            {
                return;
            }

            if (currentProject.Controller == null)
            {
                //Jezeli nie ma jeszcze regulatora - nalezy sprobowac go stworzyc
                CreateNewController();
            }
            else
            {
                currentProject.Controller.Mode = (PIDModeType) PIDBox.SelectedIndex;
            }

            //Po wyborze nowego trybu algorytmu regulatora nalezy odswiezyc pola ktore maja byc blokowane a ktore nie
            RefreshTextBoxEnable();
        }

        /// <summary>
        /// Metoda odswiezajaca ktore pola maja byc umozliwione do edycji a ktore nie - w zaleznosci od typu algorytmu regulatora
        /// </summary>
        public void RefreshTextBoxEnable()
        {
            if (Online)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                        {
                            KpTextBox.IsEnabled = false;
                            TiTextBox.IsEnabled = false;
                            TdTextBox.IsEnabled = false;
                            NTextBox.IsEnabled = false;
                        }));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                            {
                                switch ((PIDModeType)PIDBox.SelectedIndex)
                                {
                                    case PIDModeType.P:
                                        {

                                            KpTextBox.IsEnabled = true;
                                            TiTextBox.IsEnabled = false;
                                            TdTextBox.IsEnabled = false;
                                            NTextBox.IsEnabled = false;

                                            break;
                                        }

                                    case PIDModeType.PD:
                                        {
                                            KpTextBox.IsEnabled = true;
                                            TiTextBox.IsEnabled = false;
                                            TdTextBox.IsEnabled = true;
                                            NTextBox.IsEnabled = true;

                                            break;
                                        }

                                    case PIDModeType.PI:
                                        {
                                            KpTextBox.IsEnabled = true;
                                            TiTextBox.IsEnabled = true;
                                            TdTextBox.IsEnabled = false;
                                            NTextBox.IsEnabled = false;
                                            break;
                                        }

                                    case PIDModeType.PID:
                                        {
                                            KpTextBox.IsEnabled = true;
                                            TiTextBox.IsEnabled = true;
                                            TdTextBox.IsEnabled = true;
                                            NTextBox.IsEnabled = true;
                                            break;
                                        }
                                }
                            }));
                
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zmiany pola Inverted regulatora na true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void invertedCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Checked");
            if (this.currentProject == null)
            {
                return;
            }

            //Jezeli nie ma jeszcze regulatora - nalezy sprobowac go stworzyc
            CreateNewController();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zmiany pola Inverted regulatora na false
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void invertedCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.currentProject == null)
            {
                return;
            }

                CreateNewController();
        }

        /// <summary>
        /// Metoda oblugi zdarzenia zakonczenia edycji pola tekstowego Kp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KpTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Double value;

            //Jezeli pole nie zostalo poprawnie wypelnione - nalezy je wyczyscic
            if (!Double.TryParse(KpTextBox.Text, out value))
            {
                KpTextBox.Clear();
                return;
            }

            if (!CheckKp(value))
            {
                KpTextBox.Clear();
                return;
            }

            if (this.currentProject == null)
            {
                return;
            }

            if (currentProject.Controller == null)
            {
                //Jezeli nie ma jeszcze regulatora - nalezy sprobowac go stworzyc
                CreateNewController();
            }
            else
            {
                currentProject.Controller.Kp = Convert.ToDouble(KpTextBox.Text);
            }
        }

        /// <summary>
        /// Metoda oblugi zdarzenia zakonczenia edycji pola tekstowego Ti
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TiTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Double value;

            //Jezeli pole nie zostalo poprawnie wypelnione - nalezy je wyczyscic
            if (!Double.TryParse(TiTextBox.Text, out value))
            {
                TiTextBox.Clear();
                return;
            }

            if (!CheckTi(value))
            {
                TiTextBox.Clear();
                return;
            }

            if (this.currentProject == null)
            {
                return;
            }

            if (currentProject.Controller == null)
            {
                //Jezeli nie ma jeszcze regulatora - nalezy sprobowac go stworzyc
                CreateNewController();
            }
            else
            {
                currentProject.Controller.Ti = Convert.ToDouble(TiTextBox.Text);
            }
        }

        /// <summary>
        /// Metoda oblugi zdarzenia zakonczenia edycji pola tekstowego Td
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TdTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Double value;

            //Jezeli pole nie zostalo poprawnie wypelnione - nalezy je wyczyscic
            if (!Double.TryParse(TdTextBox.Text,out value))
            {
                TdTextBox.Clear();
                return;
            }

            if (!CheckTd(value))
            {
                TdTextBox.Clear();
                return;
            }

            if (this.currentProject == null)
            {
                return;

            }

            if (currentProject.Controller == null)
            {
                //Jezeli nie ma jeszcze regulatora - nalezy sprobowac go stworzyc
                CreateNewController();
            }
            else
            {
                currentProject.Controller.Td = Convert.ToDouble(TdTextBox.Text);
            }
        }

        /// <summary>
        /// Metoda oblugi zdarzenia zakonczenia edycji pola tekstowego N
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Double value;

            //Jezeli pole nie zostalo poprawnie wypelnione - nalezy je wyczyscic
            if (!Double.TryParse(NTextBox.Text, out value))
            {
                NTextBox.Clear();
                return;
            }

            if (!CheckN(value))
            {
                NTextBox.Clear();
                return;
            }

            if (this.currentProject == null)
            {
                return;
            }

            if (currentProject.Controller == null)
            {
                //Jezeli nie ma jeszcze regulatora - nalezy sprobowac go stworzyc
                CreateNewController();
            }
            else
            {
                currentProject.Controller.N = Convert.ToDouble(NTextBox.Text);
            }
        }

        /// <summary>
        /// Metoda sprawdzajaca czy wartosc Kp jest odpowiednia - wartosc ta musi byc wieksza od 0
        /// </summary>
        /// <param name="value">
        /// Wartosc Kp
        /// </param>
        /// <returns>
        /// Czy wartosc Kp jest odpowiednia
        /// </returns>
        private bool CheckKp(Double value)
        {
            if(value <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Metoda sprawdzajaca czy wartosc Td jest odpowiednia - wartosc ta musi byc wieksza od 0
        /// </summary>
        /// <param name="value">
        /// Wartosc Td
        /// </param>
        /// <returns>
        /// Czy wartosc Td jest odpowiednia
        /// </returns>
        private bool CheckTd(Double value)
        {
            if (value <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Metoda sprawdzajaca czy wartosc Ti jest odpowiednia - wartosc ta musi byc wieksza od 0
        /// </summary>
        /// <param name="value">
        /// Wartosc Ti
        /// </param>
        /// <returns>
        /// Czy wartosc Ti jest odpowiednia
        /// </returns>
        private bool CheckTi(Double value)
        {
            if (value <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Metoda sprawdzajaca czy wartosc N jest odpowiednia - wartosc ta musi byc wieksza od 0
        /// </summary>
        /// <param name="value">
        /// Wartosc N
        /// </param>
        /// <returns>
        /// Czy wartosc N jest odpowiednia
        /// </returns>
        private bool CheckN(Double value)
        {
            if (value <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia Save z pola tekstowego
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            //Zapisanie nowego regulatora
            SaveNewControllerObject();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia Load z pola tekstoweg
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadClicked(object sender, RoutedEventArgs e)
        {
            //Wczytanie nowego regulatora
            LoadNewControllerObject();
        }

        /// <summary>
        /// Metoda zapisujaca obiekt regulatora
        /// </summary>
        public void SaveNewControllerObject()
        {
            SaveWindowShow();

        }

        /// <summary>
        /// Metoda wczytania nowego obiektu regulatora
        /// </summary>
        public void LoadNewControllerObject()
        {
            //Wyswietlenie okna wczytania
            LoadWindowShow();
        }

        /// <summary>
        /// Metoda wyswietlajaca okno do zapisu obiektu regulatora
        /// </summary>
        private void SaveWindowShow()
        {
            if (this.currentProject == null)
            {
                return;
            }

            if (currentProject.Controller == null)
            {
                return;
            }

            //Otwarcie okna do zapisu pliku 
            System.Windows.Forms.SaveFileDialog saveFileDialogWindow = new System.Windows.Forms.SaveFileDialog();
            XDocument XMLFile;
            Stream writeStream;
            //Ustawienie filtra wyswietlanych w tym oknie wartosci
            saveFileDialogWindow.Filter = "Controller files (*.pid)|*.pid";
            saveFileDialogWindow.FilterIndex = 0;
            //Ustawienie domyslnej sciezki tego okno jako projekt\Projects
            saveFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Projects");

            if (saveFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                writeStream = saveFileDialogWindow.OpenFile();
                XMLFile = currentProject.Controller.PIDToXML();
                XMLFile.Save(writeStream);

                //Zakonczenie pracy ze strumieniem
                writeStream.Flush();
                writeStream.Close();
            }
        }

        /// <summary>
        /// Metoda wyswietlajaca okno do zapisu obiektu regulatora
        /// </summary>
        private void LoadWindowShow()
        {
            //Otwarcie okna do wskazania pliku
            System.Windows.Forms.OpenFileDialog readFileDialogWindow = new System.Windows.Forms.OpenFileDialog();

            readFileDialogWindow.Filter = "Dynamic system files (*.pid)|*.pid";
            readFileDialogWindow.FilterIndex = 0;
            readFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Projects");

            //Jezeli okno to zostalo zaakceptowane - nalezy wczytac zawartosc pliku
            if (readFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Stream readStream = readFileDialogWindow.OpenFile();
                XDocument xmlDocument = XDocument.Load(readStream);

                PIDController newPID;

                if (currentProject.PlantObject != null)
                {
                    newPID = PIDController.PIDFromXML(xmlDocument, currentProject.PlantObject.SimulationSampleTime);
                }
                else
                {
                    newPID = PIDController.PIDFromXML(xmlDocument);
                }

                currentProject.Controller = newPID;
                RefreshControllerDisplaying();
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia wyswietlenia menu kontekstowego za pomoca prawego przycisku myszy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            //Filtracja kontekstowego menu
            FilterContextMenu();
        }

        /// <summary>
        /// Metoda filtrujaca kontekstowe menu - zapewniajaca ze nie mozna zapisac pustego regulatora
        /// </summary>
        private void FilterContextMenu()
        {
            if (this.currentProject == null)
            {
                SaveMenuItem.IsEnabled = false;
                DownloadMenuItem.IsEnabled = false;
            }
            else if (currentProject.Controller == null)
            {
                SaveMenuItem.IsEnabled = false;
                DownloadMenuItem.IsEnabled = false;
            }
            else
            {
                SaveMenuItem.IsEnabled = true;
                DownloadMenuItem.IsEnabled = true;
            }

            if(Connected)
            {
                OnlineMenuItem.IsEnabled = false;
                OfflineMenuItem.IsEnabled = true;
                DownloadMenuItem.IsEnabled = false;
            }
            else
            {
                OnlineMenuItem.IsEnabled = true;
                OfflineMenuItem.IsEnabled = false;
                DownloadMenuItem.IsEnabled = true;
            }

        }

        /// <summary>
        /// Metoda obslugi zdarzenia wcisniecia przycisku GoOnline z kontekstowego menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            GoOnline();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia wcisniecia przycisku GoOffline z kontekstowego menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OfflineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            GoOffline();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia wcisniecia przycisku GoOnline
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOnline_Click(object sender, RoutedEventArgs e)
        {
            GoOnline();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia wcisniecia przycisku GoOffline
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOffline_Click(object sender, RoutedEventArgs e)
        {
            GoOffline();
        }

        /// <summary>
        /// Metoda obslugi przycisku Download to device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDownload_Click(object sender, RoutedEventArgs e)
        {
            DownloadParametersToDevice();
        }

        /// <summary>
        /// Metoda zapisujaca parametry do sterownika
        /// </summary>
        private void DownloadParametersToDevice()
        {
            if (currentProject != null)
            {
                if (currentProject.Controller != null)
                {
                    try
                    {
                        //Stworzenie nowego sterownika na podstawie stanu blokow tekstowych
                        CreateNewController();

                        //Zapisanie odpowiednich wartosci w zaleznosci od trybu algorytmu
                        switch (currentProject.Controller.Mode)
                        {
                            case PIDModeType.P:
                                {
                                    controllerDevice.SavePIDParameters(currentProject.Controller.Kp, controllerDevice.TiForNonI, 0, currentProject.Controller.N, controllerDevice.PIDSampleTime, currentProject.Controller.Inverted);

                                    break;
                                }
                            case PIDModeType.PD:
                                {
                                    controllerDevice.SavePIDParameters(currentProject.Controller.Kp, controllerDevice.TiForNonI, currentProject.Controller.Td, currentProject.Controller.N, controllerDevice.PIDSampleTime, currentProject.Controller.Inverted);

                                    break;
                                }
                            case PIDModeType.PI:
                                {
                                    controllerDevice.SavePIDParameters(currentProject.Controller.Kp, currentProject.Controller.Ti, 0, currentProject.Controller.N, controllerDevice.PIDSampleTime, currentProject.Controller.Inverted);

                                    break;
                                }
                            case PIDModeType.PID:
                                {
                                    controllerDevice.SavePIDParameters(currentProject.Controller.Kp, currentProject.Controller.Ti, currentProject.Controller.Td, currentProject.Controller.N, controllerDevice.PIDSampleTime, currentProject.Controller.Inverted);

                                    break;
                                }
                        }

                        System.Windows.MessageBox.Show("Settings has been sucesfully downloaded to device", "Downloading done", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception exception)
                    {
                        System.Windows.MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia wcisniecia download z menu kontextowego
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DownloadParametersToDevice();
        }



    }


}
