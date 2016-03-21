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

namespace SOSPDModelTunningMethod
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

        private SOSPDTunningMethodBase[] tuningMethods;

        private IAELopezMethodPI iAELopezMethodPI = new IAELopezMethodPI();
        public IAELopezMethodPI IAELopezMethodPI
        {
            get
            {
                return iAELopezMethodPI;
            }

            set
            {
                iAELopezMethodPI = value;
                NotifyPropertyChanged("IAELopezMethodPI");
            }
        }

        private IAEShinskeyMethodPI iAEShinskeyMethodPI = new IAEShinskeyMethodPI();
        public IAEShinskeyMethodPI IAEShinskeyMethodPI
        {
            get
            {
                return iAEShinskeyMethodPI;
            }

            set
            {
                iAEShinskeyMethodPI = value;
                NotifyPropertyChanged("IAEShinskeyMethodPI");
            }

        }

        private ISELopezMethodPI iSELopezMethodPI = new ISELopezMethodPI();
        public ISELopezMethodPI ISELopezMethodPI
        {
            get
            {
                return iSELopezMethodPI;
            }

            set
            {
                iSELopezMethodPI = value;
                NotifyPropertyChanged("ISELopezMethodPI");
            }

        }

        private ITAELopezMethodPI iTAELopezMethodPI = new ITAELopezMethodPI();
        public ITAELopezMethodPI ITAELopezMethodPI
        {
            get
            {
                return iTAELopezMethodPI;
            }

            set
            {
                iTAELopezMethodPI = value;
                NotifyPropertyChanged("ITAELopezMethodPI");
            }

        }


        private ITSEChaoMethodPI1 iTSEChaoMethodPI1 = new ITSEChaoMethodPI1();
        public ITSEChaoMethodPI1 ITSEChaoMethodPI1
        {
            get
            {
                return iTSEChaoMethodPI1;
            }

            set
            {
                iTSEChaoMethodPI1 = value;
                NotifyPropertyChanged("ITSEChaoMethodPI1");
            }

        }

        private ISEKeviczkyMethodPI iSEKeviczkyMethodPI = new ISEKeviczkyMethodPI();
        public ISEKeviczkyMethodPI ISEKeviczkyMethodPI
        {
            get
            {
                return iSEKeviczkyMethodPI;
            }

            set
            {
                iSEKeviczkyMethodPI = value;
                NotifyPropertyChanged("ISEKeviczkyMethodPI");
            }

        }


        private ITAEChaoMethodPI iTAEChaoMethodPI = new ITAEChaoMethodPI();
        public ITAEChaoMethodPI ITAEChaoMethodPI
        {
            get
            {
                return iTAEChaoMethodPI;
            }

            set
            {
                iTAEChaoMethodPI = value;
                NotifyPropertyChanged("ITAEChaoMethodPI");
            }

        }

        private ITSEChaoMethodPI2 iTSEChaoMethodPI2 = new ITSEChaoMethodPI2();
        public ITSEChaoMethodPI2 ITSEChaoMethodPI2
        {
            get
            {
                return iTSEChaoMethodPI2;
            }

            set
            {
                iTSEChaoMethodPI2 = value;
                NotifyPropertyChanged("ITSEChaoMethodPI2");
            }

        }

        private SomaniMethodPI somaniMethodPI = new SomaniMethodPI();
        public SomaniMethodPI SomaniMethodPI
        {
            get
            {
                return somaniMethodPI;
            }

            set
            {
                somaniMethodPI = value;
                NotifyPropertyChanged("SomaniMethodPI");
            }

        }

        private SchaedelMethodPI schaedelMethodPI = new SchaedelMethodPI();
        public SchaedelMethodPI SchaedelMethodPI
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

        //sdasdasd

        private AflaroRuizMethodPID aflaroRuizMethodPID = new AflaroRuizMethodPID();
        public AflaroRuizMethodPID AflaroRuizMethodPID
        {
            get
            {
                return aflaroRuizMethodPID;
            }

            set
            {
                aflaroRuizMethodPID = value;
                NotifyPropertyChanged("AflaroRuizMethodPID");
            }

        }

        private IAELopezMethodPID iAELopezMethodPID = new IAELopezMethodPID();
        public IAELopezMethodPID IAELopezMethodPID
        {
            get
            {
                return iAELopezMethodPID;
            }

            set
            {
                iAELopezMethodPID = value;
                NotifyPropertyChanged("IAELopezMethodPID");
            }

        }

        private ISELopezMethodPID iSELopezMethodPID = new ISELopezMethodPID();
        public ISELopezMethodPID ISELopezMethodPID
        {
            get
            {
                return iSELopezMethodPID;
            }

            set
            {
                iSELopezMethodPID = value;
                NotifyPropertyChanged("ISELopezMethodPID");
            }

        }

        private ITAESungMethodPID iTAESungMethodPID = new ITAESungMethodPID();
        public ITAESungMethodPID ITAESungMethodPID
        {
            get
            {
                return iTAESungMethodPID;
            }

            set
            {
                iTAESungMethodPID = value;
                NotifyPropertyChanged("ITAESungMethodPID");
            }

        }

        private ChienMethodPID chienMethodPID = new ChienMethodPID();
        public ChienMethodPID ChienMethodPID
        {
            get
            {
                return chienMethodPID;
            }

            set
            {
                chienMethodPID = value;
                NotifyPropertyChanged("ChienMethodPID");
            }

        }

        private ViteckovaMetodPID viteckovaMetodPID = new ViteckovaMetodPID();
        public ViteckovaMetodPID ViteckovaMetodPID
        {
            get
            {
                return viteckovaMetodPID;
            }

            set
            {
                viteckovaMetodPID = value;
                NotifyPropertyChanged("ViteckovaMetodPID");
            }

        }

        private SkogestadMethodPID skogestadMethodPID = new SkogestadMethodPID();
        public SkogestadMethodPID SkogestadMethodPID
        {
            get
            {
                return skogestadMethodPID;
            }

            set
            {
                skogestadMethodPID = value;
                NotifyPropertyChanged("SkogestadMethodPID");
            }

        }

        private BiMethodPID biMethodPID = new BiMethodPID();
        public BiMethodPID BiMethodPID
        {
            get
            {
                return biMethodPID;
            }

            set
            {
                biMethodPID = value;
                NotifyPropertyChanged("BiMethodPID");
            }

        }

        private LavanyaMethodPID lavanyaMethodPID = new LavanyaMethodPID();
        public LavanyaMethodPID LavanyaMethodPID
        {
            get
            {
                return lavanyaMethodPID;
            }

            set
            {
                lavanyaMethodPID = value;
                NotifyPropertyChanged("LavanyaMethodPID");
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

            SchaedelPIMethodStackPanel.DataContext = SchaedelMethodPI;
            SomaniPIMethodStackPanel.DataContext = SomaniMethodPI;
            ARPIDMethodStackPanel.DataContext = AflaroRuizMethodPID;
            ChienPIDMethodStackPanel.DataContext = ChienMethodPID;
            ViteckovaPIDMethodStackPanel.DataContext = ViteckovaMetodPID;

        }

        public void InitTuningMethods()
        {
            tuningMethods = new SOSPDTunningMethodBase[]
            {
                IAELopezMethodPI,
                IAEShinskeyMethodPI,
                ISELopezMethodPI,
                ISEKeviczkyMethodPI,
                ITAELopezMethodPI,
                ITAEChaoMethodPI,
                ITSEChaoMethodPI1,
                ITSEChaoMethodPI2,
                SomaniMethodPI,
                SchaedelMethodPI,
                AflaroRuizMethodPID ,
                IAELopezMethodPID ,
                ISELopezMethodPID,
                ITAESungMethodPID ,
                ChienMethodPID,
                ViteckovaMetodPID ,
                SkogestadMethodPID,
                BiMethodPID ,
                LavanyaMethodPID 
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
                TuningMethodToDisplay.Methods = new ObservableCollection<SOSPDTunningMethodBase>((from method in tuningMethods
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
            if (MethodsComboBox.SelectedItem == SomaniMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SomaniPIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    SchaedelPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ARPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    
                }));
            }
            else if (MethodsComboBox.SelectedItem == SchaedelMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SomaniPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SchaedelPIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    ARPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == AflaroRuizMethodPID)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SomaniPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SomaniPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ARPIDMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    ChienPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == ChienMethodPID)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SomaniPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SomaniPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ARPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienPIDMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    ViteckovaPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == ViteckovaMetodPID)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SomaniPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SomaniPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ARPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPIDMethodStackPanel.Visibility = System.Windows.Visibility.Visible;

                }));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SomaniPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SchaedelPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ARPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PIDController = ((SOSPDTunningMethodBase)MethodsComboBox.SelectedItem).TuningMethod(plantObject);
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
        private ObservableCollection<SOSPDTunningMethodBase> methods = new ObservableCollection<SOSPDTunningMethodBase>();
        public ObservableCollection<SOSPDTunningMethodBase> Methods
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
