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

namespace SOSPDZeroModelTunningMethod
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

        private SOSPDZeroTunningMethodBase[] tuningMethods;

        private ChienMethodPI chienMethodPI = new ChienMethodPI();
        public ChienMethodPI ChienMethodPI
        {
            get
            {
                return chienMethodPI;
            }

            set
            {
                chienMethodPI = value;
                NotifyPropertyChanged("ChienMethodPI");
            }

        }

        private PomerleauMethodPI pomerleauMethodPI = new PomerleauMethodPI();
        public PomerleauMethodPI PomerleauMethodPI
        {
            get
            {
                return pomerleauMethodPI;
            }

            set
            {
                pomerleauMethodPI = value;
                NotifyPropertyChanged("PomerleauMethodPI");
            }

        }

        private MarchettiMethodPI marchettiMethodPI = new MarchettiMethodPI();
        public MarchettiMethodPI MarchettiMethodPI
        {
            get
            {
                return marchettiMethodPI;
            }

            set
            {
                marchettiMethodPI = value;
                NotifyPropertyChanged("MarchettiMethodPI");
            }

        }

        private Chien2003MethodPID chien2003MethodPID = new Chien2003MethodPID();
        public Chien2003MethodPID Chien2003MethodPID
        {
            get
            {
                return chien2003MethodPID;
            }

            set
            {
                chien2003MethodPID = value;
                NotifyPropertyChanged("Chien2003MethodPID");
            }

        }


        private Chien1988MethodPID chien1988MethodPID = new Chien1988MethodPID();
        public Chien1988MethodPID Chien1988MethodPID
        {
            get
            {
                return chien1988MethodPID;
            }

            set
            {
                chien1988MethodPID = value;
                NotifyPropertyChanged("Chien1988MethodPID");
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

            Chien1988PIMethodStackPanel.DataContext = Chien1988MethodPID;
            MarchettiPIMethodStackPanel.DataContext = MarchettiMethodPI;
        }

        public void InitTuningMethods()
        {
            tuningMethods = new SOSPDZeroTunningMethodBase[]
            {
                ChienMethodPI,
                PomerleauMethodPI,
                MarchettiMethodPI,
                Chien2003MethodPID,
                Chien1988MethodPID
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
                TuningMethodToDisplay.Methods = new ObservableCollection<SOSPDZeroTunningMethodBase>((from method in tuningMethods
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
            if (MethodsComboBox.SelectedItem == MarchettiMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Chien1988PIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    MarchettiPIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                }));
            }
            else if (MethodsComboBox.SelectedItem == Chien1988MethodPID)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Chien1988PIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    MarchettiPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                }));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Chien1988PIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    MarchettiPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PIDController = ((SOSPDZeroTunningMethodBase)MethodsComboBox.SelectedItem).TuningMethod(plantObject);
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
            Double a = plantObject.Denominator[2];
            Double b = plantObject.Denominator[1];
            Double c = plantObject.Denominator[0];

            Double delta = b * b - 4 * a * c;

            if (delta < 0)
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
                        TypeName = "Robust criteria";
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
        private ObservableCollection<SOSPDZeroTunningMethodBase> methods = new ObservableCollection<SOSPDZeroTunningMethodBase>();
        public ObservableCollection<SOSPDZeroTunningMethodBase> Methods
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
