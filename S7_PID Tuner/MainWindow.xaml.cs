using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using TransferFunctionLib;

namespace S7_PID_Tuner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Wersja programu
        /// </summary>
        public static string version = "v1.0";

        /// <summary>
        /// Obiekt sterownika - do polaczenia z fizycznym urzadzeniem S7
        /// </summary>
        PIDControllerDevice controllerDevice;

        /// <summary>
        /// Aktualnie wybrany projekt
        /// </summary>
        Project currentProject;

        /// <summary>
        /// Konstruktor okna glownego aplikacji
        /// </summary>
        public MainWindow()
        {
            //Inicjalizacja UI
            InitializeComponent();

            //Inicjalizacja mechanizmu aplikacji
            InitWindow();

            InitController();

            InitMethods();
        }

        /// <summary>
        /// Inicjalizacja mechanizmu okna glownego aplikacji
        /// </summary>
        public void InitWindow()
        {
            //Wyswietlenie wszystkich Toolbarow
            projectToolbarMenuItem.IsChecked = true;
            plantToolbarMenuItem.IsChecked = true;
            controllerToolbarMenuItem.IsChecked = true;

            //Stworzenie nowego, pustego projektu
            CreateNewEmptyProject();

        }

        /// <summary>
        /// Metoda inicjalizujaca obiekt sterownika
        /// </summary>
        public void InitController()
        {
            try
            {
                //Otwarcie pliku ustwien glownych i pobranie z niego ustawien
                Stream readStream = File.OpenRead("Controller.dat");
                XDocument xmlDocument = XDocument.Load(readStream);

                //Na tej podstawie dokonywane jest tworzenie urzadzenia sterownika
                controllerDevice = PIDControllerDevice.FromXML(xmlDocument);

                readStream.Close();
            }
            catch
            {
                //Jezeli nie udalo sie zapisac ustawien - tworzony jest domyslny obiekt sterownika
                controllerDevice = new PIDControllerDevice("000.000.000.000", 0, 1, 100, 1, 9999,0.1);
            }

            //Przypisanie do okna procesu obiektu sterownika
            controlProcessControl.AssignControllerToDevice(controllerDevice);
        }

        public void InitMethods()
        {
            InitIdentificationMethods();
            InitTuningMethods();
            InitPerformanceIndexes();
        }

        public void InitIdentificationMethods()
        {
            ReadIdentificationMethodsFromXMLFile();
            RefreshIdentificationMethodDisplaying();
        }

        public void InitTuningMethods()
        {
            ReadTuningMethodsFromXMLFile();
            RefreshTuningMethodDisplaying();
        }

        public void InitPerformanceIndexes()
        {
            ReadPerformanceIndexesFromXMLFile();
            controlProcessControl.pidControllerControl.AssingPerformanceIndexes(PerformanceIndexes);
        }

        public void RefreshIdentificationMethodDisplaying()
        {
            for (int i = 0; i < identificationMethodsMenuItem.Items.Count - 2; i++ )
            {
                identificationMethodsMenuItem.Items.RemoveAt(0);
            }

            for (int i = 5; i < plantToolbar.Items.Count; i++)
            {
                plantToolbar.Items[i] = null;

            }

            foreach (IdentificationMethod method in IdentificationMethods.MethodList)
            {
                System.Windows.Controls.MenuItem newMenuItem = new System.Windows.Controls.MenuItem();
                newMenuItem.Header = method.Name;
                newMenuItem.Icon = new Image { Source = method.Icon, Width = 18, Height = 18 };
                
                newMenuItem.Click += (s,e) => 
                {
                    SystemType type;
                    DynamicSystem ds = method.Start(controllerDevice, out type);

                    if(ds != null)
                    {
                        currentProject.PlantObject = ds;
                        currentProject.Type = type;
                       
                    }
                };

                identificationMethodsMenuItem.Items.Insert(0, newMenuItem);

                System.Windows.Controls.Button newButton = new System.Windows.Controls.Button { ToolTip=method.Name };
                newButton.Content = new Image { Source = method.Icon, Width = 24, Height = 24 };
                
                newButton.Click += (s, e) =>
                {
                    SystemType type;
                    DynamicSystem ds = method.Start(controllerDevice, out type);

                    if(ds != null)
                    {
                        currentProject.PlantObject = ds;
                        currentProject.Type = type;
                    }
                };

                plantToolbar.Items.Add(newButton);
            }
        }

        public void RefreshTuningMethodDisplaying()
        {
            for (int i = 0; i < TuningMethodsCollection.Items.Count - 2; i++)
            {
                TuningMethodsCollection.Items.RemoveAt(0);
            }

            for (int i = 5; i < controllerToolbar.Items.Count; i++)
            {
                controllerToolbar.Items[i] = null;
            }

            foreach (TuningMethod method in TuningMethods.MethodList)
            {
                System.Windows.Controls.MenuItem newMenuItem = new System.Windows.Controls.MenuItem();
                newMenuItem.Header = method.Name;
                newMenuItem.Icon = new Image { Source = method.Icon, Width = 18, Height = 18 };

                newMenuItem.Click += (s, e) =>
                {
                    PIDController controller = method.Start();
                    
                    if(controller != null)
                    {
                        if (currentProject.PlantObject != null)
                        {
                            controller.SimulationSampleTime = currentProject.PlantObject.SimulationSampleTime;
                        }

                        currentProject.Controller = controller;
                    }
                    
                };

                TuningMethodsCollection.Items.Insert(0, newMenuItem);

                System.Windows.Controls.Button newButton = new System.Windows.Controls.Button { ToolTip = method.Name };
                newButton.Content = new Image { Source = method.Icon, Width = 24, Height = 24 };

                newButton.Click += (s, e) =>
                {
                    PIDController controller = method.Start();

                    if (controller != null)
                    {
                        if (currentProject.PlantObject != null)
                        {
                            controller.SimulationSampleTime = currentProject.PlantObject.SimulationSampleTime;
                        }

                        currentProject.Controller = controller;
                    }
                };

                controllerToolbar.Items.Add(newButton);
            }
        }



        public void ReadIdentificationMethodsFromXMLFile()
        {
            if (File.Exists("IdentificationMethods.dat"))
            {
                try
                {
                    Stream streamRead = File.OpenRead("IdentificationMethods.dat");
                    XDocument xmlDocument = XDocument.Load(streamRead);
                    IdentificationMethods = new IdentificationMethodCollection();
                    IdentificationMethods.FromXML(xmlDocument);
                    streamRead.Close();
                }
                catch
                {
                    IdentificationMethods = new IdentificationMethodCollection();
                }
            }
            else
            {
                IdentificationMethods = new IdentificationMethodCollection();
            }

            WriteIdentificationMethodsToXMLFile();
            
        }

        public void ReadTuningMethodsFromXMLFile()
        {
            if (File.Exists("TuningMethods.dat"))
            {
                try
                {
                    Stream streamRead = File.OpenRead("TuningMethods.dat");
                    XDocument xmlDocument = XDocument.Load(streamRead);
                    TuningMethods = new TuningMethodCollection();
                    TuningMethods.FromXML(xmlDocument);
                    TuningMethods.AssignProject(currentProject);
                    streamRead.Close();
                }
                catch
                {
                    TuningMethods = new TuningMethodCollection();
                    TuningMethods.AssignProject(currentProject);
                }
            }
            else
            {
                TuningMethods = new TuningMethodCollection();
                TuningMethods.AssignProject(currentProject);
            }

            WriteTuningMethodsToXMLFile();

        }

        public void ReadPerformanceIndexesFromXMLFile()
        {
            if (File.Exists("PerformanceIndexes.dat"))
            {
                try
                {
                    Stream streamRead = File.OpenRead("PerformanceIndexes.dat");
                    XDocument xmlDocument = XDocument.Load(streamRead);
                    PerformanceIndexes = new PerformanceIndexCollection();
                    PerformanceIndexes.FromXML(xmlDocument);
                    streamRead.Close();
                }
                catch
                {
                    PerformanceIndexes = new PerformanceIndexCollection();
                }
            }
            else
            {
                PerformanceIndexes = new PerformanceIndexCollection();
            }

            WritePerformanceIndexesToXMLFile();

        }

        public void WriteIdentificationMethodsToXMLFile()
        {
            Stream writeStream = File.Open("IdentificationMethods.dat", FileMode.Create);

            XDocument xmlDocument = IdentificationMethods.ToXML();
            
            xmlDocument.Save(writeStream);

            writeStream.Flush();
            writeStream.Close();
        }

        public void WriteTuningMethodsToXMLFile()
        {
            Stream writeStream = File.Open("TuningMethods.dat", FileMode.Create);

            XDocument xmlDocument = TuningMethods.ToXML();

            xmlDocument.Save(writeStream);

            writeStream.Flush();
            writeStream.Close();
        }

        public void WritePerformanceIndexesToXMLFile()
        {
            Stream writeStream = File.Open("PerformanceIndexes.dat", FileMode.Create);

            XDocument xmlDocument = PerformanceIndexes.ToXML();

            xmlDocument.Save(writeStream);

            writeStream.Flush();
            writeStream.Close();
        }

        public IdentificationMethodCollection IdentificationMethods
        {
            get;
            set;
        }

        public TuningMethodCollection TuningMethods
        {
            get;
            set;
        }

        public PerformanceIndexCollection PerformanceIndexes
        {
            get;
            set;
        }

        /// <summary>
        /// Metoda zapisujaca ustawienia glowne
        /// </summary>
        public void SaveController()
        {
            try
            {
                //Otwrcie pliku ustawien glownych
                Stream writeStream = File.Open("Controller.dat",FileMode.Create);
                XDocument xmlDocument = controllerDevice.ToXML();

                //Zapisanie ustawien do pliku
                xmlDocument.Save(writeStream);

                //Zamkniecie strumieni
                writeStream.Flush();
                writeStream.Close();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Metoda przypisujaca projekt do aplikacji
        /// </summary>
        /// <param name="project">
        /// Obiekt projektu
        /// </param>
        public void AssignNewProject(Project project)
        {
            //Przypisanie projektu do okna
            this.currentProject = project;

            //Przypisanie projektu do kontrolki ukladu regulacji 
            controlProcessControl.AssignProjectToControl(currentProject);
            
            //Przypisanie projektu do kontrolki wykresow
            chartPanel.AssignNewProject(currentProject);

            if(TuningMethods != null)
            {
                TuningMethods.AssignProject(project);
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia Save Project As w pasku menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveProjectMenuItem_Clicked(object sender, RoutedEventArgs e)
        {
            //Zapisanie projektu
            SaveProject();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia Load Project w pasku menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadProjectMenuItem_Clicked(object sender, RoutedEventArgs e)
        {
            //Wczytanie projektu
            LoadProject();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia New Project w pasku menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewProjectMenuItem_Clicked(object sender, RoutedEventArgs e)
        {
            //Stworzenie nowego pustego projektu
            CreateNewEmptyProject();
        }
        
        /// <summary>
        /// Metoda zapisujaca aktualny projekt
        /// </summary>
        public void SaveProject()
        {
            //Wyswietlenie okna zapisu nowego projektu
            ShowSaveProjectWindow();
        }

        /// <summary>
        /// Metoda wczytujaca nowy projekt
        /// </summary>
        public void LoadProject()
        {
            //Wyswietlenie okna wczytania projektu
            ShowLoadProjectWindow();
        }

        /// <summary>
        /// Metoda tworzaca nowy pusty projekt
        /// </summary>
        public void CreateNewEmptyProject()
        {
            //Stworzenie nowego pustego projektu
            currentProject = new Project(null, null, SystemType.Continues);

            //Przypisanie go do aplikacji
            AssignNewProject(currentProject);
        }

        /// <summary>
        /// Metoda wyswietlajaca okno zapisu projektu
        /// </summary>
        private void ShowSaveProjectWindow()
        {
            if (this.currentProject == null)
            {
                return;
            }

            //Otwarcie okna do zapisu pliku 
            System.Windows.Forms.SaveFileDialog saveFileDialogWindow = new System.Windows.Forms.SaveFileDialog();
            XDocument XMLFile;
            Stream writeStream;
            //Ustawienie filtra wyswietlanych w tym oknie wartosci
            saveFileDialogWindow.Filter = "Poject files (*.prj)|*.prj";
            saveFileDialogWindow.FilterIndex = 0;
            //Ustawienie domyslnej sciezki tego okno jako projekt\Projects
            saveFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Projects");

            if (saveFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                writeStream = saveFileDialogWindow.OpenFile();
                XMLFile = currentProject.ToXML();
                XMLFile.Save(writeStream);

                //Zakonczenie pracy ze strumieniem
                writeStream.Flush();
                writeStream.Close();
            }
        }

        /// <summary>
        /// Metoda wyswietlajaca okno wczytania nowego projektu
        /// </summary>
        private void ShowLoadProjectWindow()
        {
            //Otwarcie okna do wskazania pliku
            System.Windows.Forms.OpenFileDialog readFileDialogWindow = new System.Windows.Forms.OpenFileDialog();

            readFileDialogWindow.Filter = "Poject files (*.prj)|*.prj";
            readFileDialogWindow.FilterIndex = 0;
            readFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Projects");

            //Jezeli okno to zostalo zaakceptowane - nalezy wczytac zawartosc pliku
            if (readFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Stream readStream = readFileDialogWindow.OpenFile();
                XDocument xmlDocument = XDocument.Load(readStream);
                Project newProject = null;
                try
                {
                    newProject = Project.FromXML(xmlDocument);
                }
                catch (Exception exception)
                {
                    System.Windows.MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AssignNewProject(newProject);
            }
        }

        /// <summary>
        /// Metoda wywolana podczas wyswietlania kontekstowego menu paska - > Project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            //Filtracja menu projektu
            FilterProjectContextMenu();
        }

        /// <summary>
        /// Metoda filtrujaca menu projektu paska
        /// </summary>
        private void FilterProjectContextMenu()
        {
            //Zabezpieczenie przez proba zapisu pustego projektu
            if (currentProject.PlantObject == null && currentProject.Controller == null)
            {
                SaveProjectMenuItem.IsEnabled = false;
            }
            else
            {
                SaveProjectMenuItem.IsEnabled = true;
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia LoadPlant w pasku menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadPlantMenuItem_Click(object sender, RoutedEventArgs e)
        {
            controlProcessControl.LoadProcessControl();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia SavePlant w pasku menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePlantMenuItem_Click(object sender, RoutedEventArgs e)
        {
            controlProcessControl.SaveProcessControl();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia New Plant w pasku menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewEmptyPlantMenuItem_Click(object sender, RoutedEventArgs e)
        {
            currentProject.PlantObject = null;
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zaznaczenia pola menu kontekstowego paska gornego - projectToobarMenuItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectToolbarMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            projectToolbar.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Metoda obslugi zdarzenia odznaczenia pola menu kontekstowego paska gornego - projectToobarMenuItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectToolbarMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            projectToolbar.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Metoda wywolana podczas wyswietlania kontekstowego menu paska - > Plant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlantMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            if (currentProject.PlantObject == null)
            {
                SavePlantMenuItem.IsEnabled = false;
            }
            else
            {
                SavePlantMenuItem.IsEnabled = true;
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia Load Controller w pasku menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadPIDMenuItem_Click(object sender, RoutedEventArgs e)
        {
            controlProcessControl.LoadPIDController();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia Save Controller w pasku menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePidSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            controlProcessControl.SavePIDController();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia New Controller w pasku menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewEmptyPIDController(object sender, RoutedEventArgs e)
        {
            currentProject.Controller = null;
        }

        /// <summary>
        /// Metoda wywolana podczas wyswietlania kontekstowego menu paska - > Controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PIDControllerMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            if (currentProject.Controller == null)
            {
                SavePidSettingsMenuItem.IsEnabled = false;
            }
            else
            {
                SavePidSettingsMenuItem.IsEnabled = true;
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia About w pasku menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("S7 PID Tuner : " + version + "\nAuthor: Witold Kolaj ", "About...", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia przycisku NewProject w pasku zadan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newProjectToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            CreateNewEmptyProject();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia przycisku Load Project w pasku zadan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadProjectToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLoadProjectWindow();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia przycisku Save Project w pasku zadan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveProjectToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentProject == null)
            {
                return;
            }

            if(this.currentProject.Controller == null && this.currentProject.PlantObject == null)
            {
                return;
            }

            ShowSaveProjectWindow();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia przycisku New Plant w pasku zadan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newPlantToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            if(currentProject != null)
            {
                currentProject.PlantObject = null;
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia przycisku Load Plant w pasku zadan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadPlantToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            if(currentProject !=null)
            {
                controlProcessControl.LoadProcessControl();
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia przycisku Save Plant w pasku zadan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savePlantToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentProject != null)
            {
                if(currentProject.PlantObject != null)
                {
                    controlProcessControl.SaveProcessControl();
                }
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia przycisku Add Identification Method w pasku zadan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addPlantMethodToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewIdentificationMethods();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zaznaczenia pola menu kontekstowego paska gornego - plantToobarMenuItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void plantToolbarMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            plantToolbar.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Metoda obslugi zdarzenia odznaczenia pola menu kontekstowego paska gornego - plantToobarMenuItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void plantToolbarMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            plantToolbar.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia przycisku New Controller Toolbar w pasku zadan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newControllerToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentProject != null)
            {
                currentProject.Controller = null;
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia przycisku Load Controller w pasku zadan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadControllerToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentProject != null)
            {
                controlProcessControl.LoadPIDController();
            }
        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia przycisku Save Controller w pasku zadan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveControllerToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentProject != null)
            {
                if (currentProject.Controller != null)
                {
                    controlProcessControl.SavePIDController();
                }
            }

        }

        /// <summary>
        /// Metoda obslugi zdarzenia klikniecia przycisku New Tuning Method w pasku zadan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addControllerMethodToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewTuningMethods();
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zaznaczenia pola menu kontekstowego paska gornego - controllerToobarMenuItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void controllerToolbarMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            controllerToolbar.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Metoda obslugi zdarzenia zaznaczenia pola menu kontekstowego paska gornego - controllerToobarMenuItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void controllerToolbarMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            controllerToolbar.Visibility = System.Windows.Visibility.Hidden;

        }

        /// <summary>
        /// Obsluga zdarzenia wcisniecia przycisku CTRL+S
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                if (this.currentProject == null)
                {
                    return;
                }

                if (this.currentProject.Controller == null && this.currentProject.PlantObject == null)
                {
                    return;
                }

                //Jezeli projekt nie jest pusty - nastepuje jego zapis
                SaveProject();
            }
        }

        /// <summary>
        /// Metoda uruchamiana po przycisnieciu przycisku ustawien menu glownego
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Uruchomienie i inicjalizacja okna ustawien
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.AssignControllerToDevice(controllerDevice);

            //Jezeli okno zostalo zaakceptowane nalezy zapisac nowe ustawienia sterownika
            bool? result = settingsWindow.ShowDialog();

            if (result.HasValue)
            {
                if ((bool)result)
                {
                    SaveController();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addNewMethodButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewIdentificationMethods();
        }

        public void LoadNewIdentificationMethods()
        {
            //Otwarcie okna do wskazania pliku
            OpenFileDialog readFileDialogWindow = new OpenFileDialog();

            readFileDialogWindow.Filter = "Dynamic library files (*.dll)|*.dll";
            readFileDialogWindow.FilterIndex = 0;
            readFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Projects");

            //Jezeli okno to zostalo zaakceptowane - nalezy wczytac zawartosc pliku
            if (readFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!IdentificationMethod.CheckAssemlby(readFileDialogWindow.FileName))
                {
                    System.Windows.MessageBox.Show("Invalid identification method library", "Cannot load method", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string copyPath = System.IO.Path.Combine("IdentificationMethods", System.IO.Path.GetFileName(readFileDialogWindow.FileName));

                bool createFile = false;

                if(File.Exists(Method.GetFullPath(copyPath)))
                {
                    if (!IsFileLocked(Method.GetFullPath(copyPath)))
                    {
                        MessageBoxResult result = System.Windows.MessageBox.Show("Identification method file with this name already exists!\nDo you want to overwrite it", "File already exists", MessageBoxButton.YesNo, MessageBoxImage.Information);

                        if (result == MessageBoxResult.Yes)
                        {
                            IdentificationMethods.MethodList.RemoveAll((method1) => 
                                {
                                    return method1.AssemblyPath.Equals(copyPath);
                                });

                            createFile = true;
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Identification method file with this name already exists and is being used by this application!! ", "File already exists", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
                else
                {
                    createFile = true;
                }

                if(createFile)
                {
                    try
                    {
                        File.Copy(readFileDialogWindow.FileName, Method.GetFullPath(copyPath), true);
                    }
                    catch (Exception exception)
                    {
                        System.Windows.MessageBox.Show(exception.Message, "Cannot copy method file", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    IdentificationMethod newMethod;

                    try
                    {
                        newMethod = new IdentificationMethod(copyPath);
                    }
                    catch (Exception exception)
                    {
                        System.Windows.MessageBox.Show(exception.Message, "Cannot create new method", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    try
                    {
                        IdentificationMethods.MethodList.Add(newMethod);
                        System.Windows.MessageBox.Show(String.Format("Method {0} was added sucesfully", newMethod.Name), "Cannot add new method", MessageBoxButton.OK, MessageBoxImage.Information);
                        WriteIdentificationMethodsToXMLFile();
                    }
                    catch (Exception exception)
                    {
                        System.Windows.MessageBox.Show(exception.Message, "Cannot add new method", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                RefreshIdentificationMethodDisplaying();
            }
        }

        protected virtual bool IsFileLocked(String path)
        {
            FileStream stream = null;

            try
            {
                stream = File.OpenWrite(path);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

        public void LoadNewTuningMethods()
        {
            //Otwarcie okna do wskazania pliku
            OpenFileDialog readFileDialogWindow = new OpenFileDialog();

            readFileDialogWindow.Filter = "Dynamic library files (*.dll)|*.dll";
            readFileDialogWindow.FilterIndex = 0;
            readFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Projects");

            //Jezeli okno to zostalo zaakceptowane - nalezy wczytac zawartosc pliku
            if (readFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!TuningMethod.CheckAssemlby(readFileDialogWindow.FileName))
                {
                    System.Windows.MessageBox.Show("Invalid tuning method library", "Cannot load method", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string copyPath = System.IO.Path.Combine("TuningMethods", System.IO.Path.GetFileName(readFileDialogWindow.FileName));

                bool createFile = false;

                if (File.Exists(Method.GetFullPath(copyPath)))
                {
                    if (!IsFileLocked(Method.GetFullPath(copyPath)))
                    {
                        MessageBoxResult result = System.Windows.MessageBox.Show("Tuning method file with this name already exists!\nDo you want to overwrite it", "File already exists", MessageBoxButton.YesNo, MessageBoxImage.Information);

                        if (result == MessageBoxResult.Yes)
                        {
                            TuningMethods.MethodList.RemoveAll((method1) =>
                            {
                                return method1.AssemblyPath.Equals(copyPath);
                            });

                            createFile = true;
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Identification method file with this name already exists and is being used by this application!! ", "File already exists", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
                else
                {
                    createFile = true;
                }

                if (createFile)
                {
                    try
                    {
                        File.Copy(readFileDialogWindow.FileName, Method.GetFullPath(copyPath), true);
                    }
                    catch (Exception exception)
                    {
                        System.Windows.MessageBox.Show(exception.Message, "Cannot copy method file", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    TuningMethod newMethod;

                    try
                    {
                        newMethod = new TuningMethod(copyPath);
                        newMethod.AssignProject(currentProject);
                    }
                    catch (Exception exception)
                    {
                        System.Windows.MessageBox.Show(exception.Message, "Cannot create new method", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    try
                    {
                        TuningMethods.MethodList.Add(newMethod);
                        System.Windows.MessageBox.Show(String.Format("Method {0} was added sucesfully", newMethod.Name), "Cannot add new method", MessageBoxButton.OK, MessageBoxImage.Information);
                        WriteTuningMethodsToXMLFile();
                    }
                    catch (Exception exception)
                    {
                        System.Windows.MessageBox.Show(exception.Message, "Cannot add new method", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                RefreshTuningMethodDisplaying();
            }
        }

        public void LoadNewPerformanceIndex()
        {
            //Otwarcie okna do wskazania pliku
            OpenFileDialog readFileDialogWindow = new OpenFileDialog();

            readFileDialogWindow.Filter = "Dynamic library files (*.dll)|*.dll";
            readFileDialogWindow.FilterIndex = 0;
            readFileDialogWindow.InitialDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Projects");

            //Jezeli okno to zostalo zaakceptowane - nalezy wczytac zawartosc pliku
            if (readFileDialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!PerformanceIndex.CheckAssemlby(readFileDialogWindow.FileName))
                {
                    System.Windows.MessageBox.Show("Invalid performance index library", "Cannot load index", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string copyPath = System.IO.Path.Combine("PerformanceIndexes", System.IO.Path.GetFileName(readFileDialogWindow.FileName));

                bool createFile = false;

                if (File.Exists(Method.GetFullPath(copyPath)))
                {
                    if (!IsFileLocked(Method.GetFullPath(copyPath)))
                    {
                        MessageBoxResult result = System.Windows.MessageBox.Show("Perfomance index file with this name already exists!\nDo you want to overwrite it", "File already exists", MessageBoxButton.YesNo, MessageBoxImage.Information);

                        if (result == MessageBoxResult.Yes)
                        {

                            PerformanceIndexes.PerformanceIndexList.RemoveAll((method1) =>
                            {
                                return method1.AssemblyPath.Equals(copyPath);
                            });

                            createFile = true;
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Perfomance index file with this name already exists and is being used by this application!! ", "File already exists", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
                else
                {
                    createFile = true;
                }

                if (createFile)
                {
                    try
                    {
                        File.Copy(readFileDialogWindow.FileName, Method.GetFullPath(copyPath), true);
                    }
                    catch (Exception exception)
                    {
                        System.Windows.MessageBox.Show(exception.Message, "Cannot copy index file", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    PerformanceIndex newMethod;

                    try
                    {
                        newMethod = new PerformanceIndex(copyPath);
                    }
                    catch (Exception exception)
                    {
                        System.Windows.MessageBox.Show(exception.Message, "Cannot index new method", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    try
                    {
                        PerformanceIndexes.PerformanceIndexList.Add(newMethod);
                        System.Windows.MessageBox.Show(String.Format("Index {0} was added sucesfully", newMethod.Name), "Cannot add new index", MessageBoxButton.OK, MessageBoxImage.Information);
                        WritePerformanceIndexesToXMLFile();
                    }
                    catch (Exception exception)
                    {
                        System.Windows.MessageBox.Show(exception.Message, "Cannot add new index", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
        }

        private void addNewTuningMethod_Click(object sender, RoutedEventArgs e)
        {
            LoadNewTuningMethods();
        }


        private void addNewIndexButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewPerformanceIndex();

        }
    }
}
