using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TOSPDModelTunningMethods
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Mechanizm wiazania danych WPF
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Mechanizm wiazania danych WPF
        /// </summary>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private TransferFunctionClass plantObject;

        private Boolean hasRoots = true;
        protected Boolean HasRoots
        {
            get
            {
                return hasRoots;
            }

            set
            {
                hasRoots = value;
                RefreshChoosenTuningMethods();
            }
        }

        private PIDControllerClass pidController = null;
        public PIDControllerClass PIDController
        {
            get
            {
                return pidController;
            }

            private set
            {
                pidController = value;
            }
        }


        private MethodsCollectionView tuningMethodsToDisplay = new MethodsCollectionView();
        public MethodsCollectionView TuningMethodToDisplay
        {
            get
            {
                return tuningMethodsToDisplay;
            }

            set
            {
                tuningMethodsToDisplay = value;
            }
        }

        private TOSPDModelTunningMethodBase[] tuningMethods;

        private KuwatarMethodPI kuwatarMethodPI = new KuwatarMethodPI();
        public KuwatarMethodPI KuwatarMethodPI
        {
            get
            {
                return kuwatarMethodPI;
            }

            set
            {
                kuwatarMethodPI = value;
                NotifyPropertyChanged("KuwatarMethodPI");
            }

        }

        private HougenMethodPI hougenMethodPI = new HougenMethodPI();
        public HougenMethodPI HougenMethodPI
        {
            get
            {
                return hougenMethodPI;
            }

            set
            {
                hougenMethodPI = value;
                NotifyPropertyChanged("HougenMethodPI");
            }

        }

        private MarchettiScaliMethodPI marchettiScaliMethodPI = new MarchettiScaliMethodPI();
        public MarchettiScaliMethodPI MarchettiScaliMethodPI
        {
            get
            {
                return marchettiScaliMethodPI;
            }

            set
            {
                marchettiScaliMethodPI = value;
                NotifyPropertyChanged("MarchettiScaliMethodPI");
            }

        }

        private SchaedelMethodPID schaedelMethodPI = new SchaedelMethodPID();
        public SchaedelMethodPID SchaedelMethodPI
        {
            get
            {
                return schaedelMethodPI;
            }

            set
            {
                schaedelMethodPI = value;
                NotifyPropertyChanged("SchaedelMethodPI");
            }

        }

        private MarchettiScaliPID marchettiScaliPID = new MarchettiScaliPID();
        public MarchettiScaliPID MarchettiScaliPID
        {
            get
            {
                return marchettiScaliPID;
            }

            set
            {
                marchettiScaliPID = value;
                NotifyPropertyChanged("MarchettiScaliPID");
            }

        }

        private JonesThamPID jonesThamPID = new JonesThamPID();
        public JonesThamPID JonesThamPID
        {
            get
            {
                return jonesThamPID;
            }

            set
            {
                jonesThamPID = value;
                NotifyPropertyChanged("JonesThamPID");
            }

        }

        private PIDAlgoritm[] possibleAlgoritms = new PIDAlgoritm[]
        {
            new PIDAlgoritm(PIDModeType.PI),
            new PIDAlgoritm(PIDModeType.PID)
        };

        public PIDAlgoritm[] PossibleAlgoritms
        {
            get
            {
                return possibleAlgoritms;
            }

            set
            {
                possibleAlgoritms = value;
                NotifyPropertyChanged("PossibleAlgoritms");
            }
        }

        private TunningMethodType[] possibleTuningTypes = new TunningMethodType[]
        {
            new TunningMethodType(TunningType.ProcessReaction),
            new TunningMethodType(TunningType.RegulatorTuning),
            new TunningMethodType(TunningType.ServoTuning),
            new TunningMethodType(TunningType.OtherTuning)
        };

        public TunningMethodType[] PossibleTuningTypes
        {
            get
            {
                return possibleTuningTypes;
            }

            set
            {
                possibleTuningTypes = value;
                NotifyPropertyChanged("PossibleTuningTypes");
            }
        }

        public SettingsWindow()
        {
            InitializeComponent();
            Init();
        }


        public void Init()
        {
            InitTuningMethods();
            InitDataContext();
        }

        public void InitDataContext()
        {
            MethodsComboBox.DataContext = TuningMethodToDisplay;
            AlgorithmTypeComboBox.DataContext = this;
            TuningMethodTypeComboBox.DataContext = this;

            MarchettiScaliMethodStackPanel.DataContext = MarchettiScaliMethodPI;
            SchaedelMethodStackPanel.DataContext = SchaedelMethodPI;
            MarchettiScaliPIDMethodStackPanel.DataContext = MarchettiScaliPID;
            JonesThamPIDMethodStackPanel.DataContext = JonesThamPID;

        }

        public void InitTuningMethods()
        {
            tuningMethods = new TOSPDModelTunningMethodBase[]
            { 
        
                KuwatarMethodPI,
                HougenMethodPI,
                MarchettiScaliMethodPI,
                SchaedelMethodPI,
                MarchettiScaliPID,
                JonesThamPID

            };
        }

        public void AssignPlantObject(TransferFunctionClass plantObject)
        {
            this.plantObject = plantObject;
            this.HasRoots = HasPlantObjectRoots(plantObject);
        }

        public void RefreshChoosenMethods()
        {
            if (AlgorithmTypeComboBox.SelectedItem != null && TuningMethodTypeComboBox.SelectedItem != null)
            {
                TuningMethodToDisplay.Methods = new ObservableCollection<TOSPDModelTunningMethodBase>((from method in tuningMethods
                                                                                                  where method.TypeOfAglorithm == ((PIDAlgoritm)AlgorithmTypeComboBox.SelectedItem).Mode && method.TypeOfTuning == ((TunningMethodType)TuningMethodTypeComboBox.SelectedItem).Type && (method.RootsRequired == HasRoots || !method.RootsRequired)
                                                                                                  select method));
                MethodsComboBox.SelectedIndex = 0;
            }
        }

        public void RefreshChoosenTuningMethods()
        {
            if (AlgorithmTypeComboBox.SelectedItem != null)
            {
                PossibleTuningTypes = (from type in
                                           ((from method in tuningMethods
                                             where method.TypeOfAglorithm == ((PIDAlgoritm)AlgorithmTypeComboBox.SelectedItem).Mode && (method.RootsRequired == HasRoots || !method.RootsRequired)
                                             select method.TypeOfTuning).Distinct())
                                       select new TunningMethodType(type)).ToArray();

                TuningMethodTypeComboBox.SelectedIndex = 0;

            }
        }

        private void AlgorithmTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshChoosenTuningMethods();
        }

        private void TuningMethodTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshChoosenMethods();
        }

        private void MethodsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MethodsComboBox.SelectedItem == MarchettiScaliMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MarchettiScaliMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    SchaedelMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    MarchettiScaliPIDMethodStackPanel.Visibility = Visibility.Collapsed;
                    JonesThamPIDMethodStackPanel.Visibility = Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == SchaedelMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MarchettiScaliMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SchaedelMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    MarchettiScaliPIDMethodStackPanel.Visibility = Visibility.Collapsed;
                    JonesThamPIDMethodStackPanel.Visibility = Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == MarchettiScaliPID)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MarchettiScaliMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SchaedelMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    MarchettiScaliPIDMethodStackPanel.Visibility = Visibility.Visible;
                    JonesThamPIDMethodStackPanel.Visibility = Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == JonesThamPID)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MarchettiScaliMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SchaedelMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    MarchettiScaliPIDMethodStackPanel.Visibility = Visibility.Collapsed;
                    JonesThamPIDMethodStackPanel.Visibility = Visibility.Visible;

                }));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MarchettiScaliMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SchaedelMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    MarchettiScaliPIDMethodStackPanel.Visibility = Visibility.Collapsed;
                    JonesThamPIDMethodStackPanel.Visibility = Visibility.Collapsed;

                }));
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PIDController = ((TOSPDModelTunningMethodBase)MethodsComboBox.SelectedItem).TuningMethod(plantObject);
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.Message, "Error during calculation", MessageBoxButton.OK, MessageBoxImage.Error);
                PIDController = null;
                return;
            }

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            PIDController = null;
            DialogResult = false;
        }


        private Boolean HasPlantObjectRoots(TransferFunctionClass plantObject)
        {
            RootFinder rt = new RootFinder(plantObject.Denominator);

            if(rt.GetRealRoots().Length != plantObject.Denominator.Length - 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class PIDAlgoritm
    {
        public PIDModeType Mode
        {
            get;
            private set;
        }

        public String ModeName
        {
            get;
            private set;
        }

        public PIDAlgoritm(PIDModeType mode)
        {
            this.Mode = mode;

            switch (mode)
            {
                case PIDModeType.P:
                    {
                        ModeName = "P";
                        break;
                    }
                case PIDModeType.PD:
                    {
                        ModeName = "PD";
                        break;
                    }
                case PIDModeType.PI:
                    {
                        ModeName = "PI";
                        break;
                    }
                case PIDModeType.PID:
                    {
                        ModeName = "PID";
                        break;
                    }
            }
        }

    }


    public class TunningMethodType
    {
        public TunningType Type
        {
            get;
            private set;
        }

        public String TypeName
        {
            get;
            private set;
        }

        public TunningMethodType(TunningType mode)
        {
            this.Type = mode;

            switch (mode)
            {
                case TunningType.OtherTuning:
                    {
                        TypeName = "Others";
                        break;
                    }
                case TunningType.ProcessReaction:
                    {
                        TypeName = "Process reaction modeling";
                        break;
                    }
                case TunningType.RegulatorTuning:
                    {
                        TypeName = "Regulator tuning";
                        break;
                    }
                case TunningType.ServoTuning:
                    {
                        TypeName = "Servo tuning";
                        break;
                    }
                case TunningType.TimeDomain:
                    {
                        TypeName = "Time domain criteria";
                        break;
                    }
                case TunningType.FrequencyDomain:
                    {
                        TypeName = "Frequency domain criteria";
                        break;
                    } 
                case TunningType.Robust:
                    {
                        TypeName = "Robustness";
                        break;
                    }
            }
        }

    }

    public class MethodsCollectionView : INotifyPropertyChanged
    {
        /// <summary>
        /// Mechanizm wiazania danych WPF
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Mechanizm wiazania danych WPF
        /// </summary>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        private ObservableCollection<TOSPDModelTunningMethodBase> methods = new ObservableCollection<TOSPDModelTunningMethodBase>();
        public ObservableCollection<TOSPDModelTunningMethodBase> Methods
        {
            get
            {
                return methods;

            }

            set
            {
                methods = value;
                NotifyPropertyChanged("Methods");
            }
        }
    }
}
