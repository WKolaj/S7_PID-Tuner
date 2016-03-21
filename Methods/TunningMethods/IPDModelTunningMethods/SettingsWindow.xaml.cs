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

namespace IPDModelTunningMethods
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

        private IPDModelTunningMethodBase[] tuningMethods;

        private ZiglerNicholsMethodP ziglerNicholsMethodP = new ZiglerNicholsMethodP();
        public ZiglerNicholsMethodP ZiglerNicholsMethodP
        {
            get
            {
                return ziglerNicholsMethodP;
            }

            set
            {
                ziglerNicholsMethodP = value;
                NotifyPropertyChanged("ZiglerNicholsMethodP");
            }
        }

        private LabviewMethodP labviewMethodP = new LabviewMethodP();
        public LabviewMethodP LabviewMethodP
        {
            get
            {
                return labviewMethodP;
            }

            set
            {
                labviewMethodP = value;
                NotifyPropertyChanged("LabviewMethodP");
            }
        }

        private ISEHaalmanMethodP iSEHaalmanMethodP = new ISEHaalmanMethodP();
        public ISEHaalmanMethodP ISEHaalmanMethodP
        {
            get
            {
                return iSEHaalmanMethodP;
            }

            set
            {
                iSEHaalmanMethodP = value;
                NotifyPropertyChanged("ISEHaalmanMethodP");
            }
        }

        private ViteckovaMethodP viteckovaMethodP = new ViteckovaMethodP();
        public ViteckovaMethodP ViteckovaMethodP
        {
            get
            {
                return viteckovaMethodP;
            }

            set
            {
                viteckovaMethodP = value;
                NotifyPropertyChanged("ViteckovaMethodP");
            }
        }

        private ZiglerNicholsMethodPI ziglerNicholsMethodPI = new ZiglerNicholsMethodPI();
        public ZiglerNicholsMethodPI ZiglerNicholsMethodPI
        {
            get
            {
                return ziglerNicholsMethodPI;
            }

            set
            {
                ziglerNicholsMethodPI = value;
                NotifyPropertyChanged("ZiglerNicholsMethodPI");
            }
        }

        private WolfeMethodPI wolfeMethodPI = new WolfeMethodPI();
        public WolfeMethodPI WolfeMethodPI
        {
            get
            {
                return wolfeMethodPI;
            }

            set
            {
                wolfeMethodPI = value;
                NotifyPropertyChanged("WolfeMethodPI");
            }
        }

        private AH1MethodPI aH1MethodPI = new AH1MethodPI();
        public AH1MethodPI AH1MethodPI
        {
            get
            {
                return aH1MethodPI;
            }

            set
            {
                aH1MethodPI = value;
                NotifyPropertyChanged("AH1MethodPI");
            }
        }

        private LabviewMethodPI labviewMethodPI = new LabviewMethodPI();
        public LabviewMethodPI LabviewMethodPI
        {
            get
            {
                return labviewMethodPI;
            }

            set
            {
                labviewMethodPI = value;
                NotifyPropertyChanged("LabviewMethodPI");
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

        private ISEHazebroekMethodPI iSEHazebroekMethodPI = new ISEHazebroekMethodPI();
        public ISEHazebroekMethodPI ISEHazebroekMethodPI
        {
            get
            {
                return iSEHazebroekMethodPI;
            }

            set
            {
                iSEHazebroekMethodPI = value;
                NotifyPropertyChanged("ISEHazebroekMethodPI");
            }
        }

        private ITAEPoulinMethodPI iTAEPoulinMethodPI = new ITAEPoulinMethodPI();
        public ITAEPoulinMethodPI ITAEPoulinMethodPI
        {
            get
            {
                return iTAEPoulinMethodPI;
            }

            set
            {
                iTAEPoulinMethodPI = value;
                NotifyPropertyChanged("ITAEPoulinMethodPI");
            }
        }

        private SkogestadMethodPI skogestadMethodPI = new SkogestadMethodPI();
        public SkogestadMethodPI SkogestadMethodPI
        {
            get
            {
                return skogestadMethodPI;
            }

            set
            {
                skogestadMethodPI = value;
                NotifyPropertyChanged("SkogestadMethodPI");
            }
        }

        private FruehaufMethodPI fruehaufMethodPI = new FruehaufMethodPI();
        public FruehaufMethodPI FruehaufMethodPI
        {
            get
            {
                return fruehaufMethodPI;
            }

            set
            {
                fruehaufMethodPI = value;
                NotifyPropertyChanged("FruehaufMethodPI");
            }
        }

        private CluettWangfMethodPI cluettWangfMethodPI = new CluettWangfMethodPI();
        public CluettWangfMethodPI CluettWangfMethodPI
        {
            get
            {
                return cluettWangfMethodPI;
            }

            set
            {
                cluettWangfMethodPI = value;
                NotifyPropertyChanged("CluettWangfMethodPI");
            }
        }

        private KooksMethodPI kooksMethodPI = new KooksMethodPI();
        public KooksMethodPI KooksMethodPI
        {
            get
            {
                return kooksMethodPI;
            }

            set
            {
                kooksMethodPI = value;
                NotifyPropertyChanged("KooksMethodPI");
            }
        }

        private ODwayerMethodPI oDwayerMethodPI = new ODwayerMethodPI();
        public ODwayerMethodPI ODwayerMethodPI
        {
            get
            {
                return oDwayerMethodPI;
            }

            set
            {
                oDwayerMethodPI = value;
                NotifyPropertyChanged("ODwayerMethodPI");
            }
        }

        private AH2MethodPI aH2MethodPI = new AH2MethodPI();
        public AH2MethodPI AH2MethodPI
        {
            get
            {
                return aH2MethodPI;
            }

            set
            {
                aH2MethodPI = value;
                NotifyPropertyChanged("AH2MethodPI");
            }
        }

        private AH3MethodPI aH3MethodPI = new AH3MethodPI();
        public AH3MethodPI AH3MethodPI
        {
            get
            {
                return aH3MethodPI;
            }

            set
            {
                aH3MethodPI = value;
                NotifyPropertyChanged("AH3MethodPI");
            }
        }

        private OgawaMethodPI ogawaMethodPI = new OgawaMethodPI();
        public OgawaMethodPI OgawaMethodPI
        {
            get
            {
                return ogawaMethodPI;
            }

            set
            {
                ogawaMethodPI = value;
                NotifyPropertyChanged("OgawaMethodPI");
            }
        }

        private ArbogastMethodPID arbogastMethodPID = new ArbogastMethodPID();
        public ArbogastMethodPID ArbogastMethodPID
        {
            get
            {
                return arbogastMethodPID;
            }

            set
            {
                arbogastMethodPID = value;
                NotifyPropertyChanged("ArbogastMethodPID");
            }
        }

        private PIDAlgoritm[] possibleAlgoritms = new PIDAlgoritm[]
        {
            new PIDAlgoritm(PIDModeType.P),
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

        public void AssingPlantObject(TransferFunctionClass plantObject)
        {
            this.plantObject = plantObject;
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
            
            WolfePIMethodStackPanel.DataContext = WolfeMethodPI;
            LabviewPMethodStackPanel.DataContext = LabviewMethodP;
            LabviewPIMethodStackPanel.DataContext = LabviewMethodPI;
            SkogestadPIMethodStackPanel.DataContext = SkogestadMethodPI;
            CluettWangPIMethodStackPanel.DataContext = CluettWangfMethodPI;
            ViteckovaPMethodStackPanel.DataContext = ViteckovaMethodP;
            KooksPIMethodStackPanel.DataContext = KooksMethodPI;
            ODwyerPIMethodStackPanel.DataContext = ODwayerMethodPI;
            OgawaPIMethodStackPanel.DataContext = OgawaMethodPI;

        }

        public void InitTuningMethods()
        {
            tuningMethods = new IPDModelTunningMethodBase[]
            {
                ZiglerNicholsMethodP,
                LabviewMethodP,
                ISEHaalmanMethodP,
                ViteckovaMethodP,
                ZiglerNicholsMethodPI,
                WolfeMethodPI,
                AH1MethodPI,
                LabviewMethodPI,
                IAEShinskeyMethodPI,
                ISEHazebroekMethodPI,
                ITAEPoulinMethodPI,
                SkogestadMethodPI,
                FruehaufMethodPI,
                CluettWangfMethodPI,
                KooksMethodPI,
                ODwayerMethodPI,
                AH2MethodPI,
                AH3MethodPI,
                OgawaMethodPI,
                ArbogastMethodPID
            };
        }

        public void RefreshChoosenMethods()
        {
            if (AlgorithmTypeComboBox.SelectedItem != null && TuningMethodTypeComboBox.SelectedItem != null)
            {
                TuningMethodToDisplay.Methods = new ObservableCollection<IPDModelTunningMethodBase>((from method in tuningMethods
                                                                                                       where method.TypeOfAglorithm == ((PIDAlgoritm)AlgorithmTypeComboBox.SelectedItem).Mode && method.TypeOfTuning == ((TunningMethodType)TuningMethodTypeComboBox.SelectedItem).Type
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
                                             where method.TypeOfAglorithm == ((PIDAlgoritm)AlgorithmTypeComboBox.SelectedItem).Mode
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
            if (MethodsComboBox.SelectedItem == WolfeMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WolfePIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    LabviewPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SkogestadPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    CluettWangPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KooksPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ODwyerPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    OgawaPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == LabviewMethodP)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WolfePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    LabviewPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SkogestadPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    CluettWangPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KooksPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ODwyerPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    OgawaPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == LabviewMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WolfePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    SkogestadPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    CluettWangPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KooksPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ODwyerPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    OgawaPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == SkogestadMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WolfePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SkogestadPIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    CluettWangPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KooksPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ODwyerPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    OgawaPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == CluettWangfMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WolfePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SkogestadPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    CluettWangPIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    ViteckovaPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KooksPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ODwyerPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    OgawaPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == ViteckovaMethodP)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WolfePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SkogestadPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    CluettWangPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    KooksPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ODwyerPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    OgawaPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == KooksMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WolfePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SkogestadPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    CluettWangPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KooksPIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    ODwyerPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    OgawaPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == ODwayerMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WolfePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SkogestadPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    CluettWangPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KooksPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ODwyerPIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    OgawaPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == OgawaMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WolfePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SkogestadPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    CluettWangPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KooksPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ODwyerPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    OgawaPIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;

                }));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WolfePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    LabviewPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SkogestadPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    CluettWangPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ViteckovaPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KooksPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ODwyerPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    OgawaPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PIDController = ((IPDModelTunningMethodBase)MethodsComboBox.SelectedItem).TuningMethod(plantObject);
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
        private ObservableCollection<IPDModelTunningMethodBase> methods = new ObservableCollection<IPDModelTunningMethodBase>();
        public ObservableCollection<IPDModelTunningMethodBase> Methods
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
