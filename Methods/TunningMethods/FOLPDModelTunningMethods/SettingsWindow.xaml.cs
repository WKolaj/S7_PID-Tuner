using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

namespace FOLPDModelTunningMethods
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

        private FOLPFModelTunningMethodBase[] tuningMethods;

        private VariableSPMethodP variableSPMethodP = new VariableSPMethodP();
        public VariableSPMethodP VariableSPMethodP
        {
            get
            {
                return variableSPMethodP;
            }

            set
            {
                variableSPMethodP = value;
                NotifyPropertyChanged("VariableSPMethodP");
            }
        }

        private VariableDisturbanceMethodP variableDisturbanceMethodP = new VariableDisturbanceMethodP();
        public VariableDisturbanceMethodP VariableDisturbanceMethodP
        {
            get
            {
                return variableDisturbanceMethodP;
            }

            set
            {
                variableDisturbanceMethodP = value;
                NotifyPropertyChanged("VariableDisturbanceMethodP");
            }
        }

        private CallenderMethodPI callenderMethodPI = new CallenderMethodPI();
        public CallenderMethodPI CallenderMethodPI
        {
            get
            {
                return callenderMethodPI;
            }

            set
            {
                callenderMethodPI = value;
                NotifyPropertyChanged("CallenderMethodPI");
            }
        }

        private ZNMethodPI zNMethodPI = new ZNMethodPI();
        public ZNMethodPI ZNMethodPI
        {
            get
            {
                return zNMethodPI;
            }

            set
            {
                zNMethodPI = value;
                NotifyPropertyChanged("ZNMethodPI");
            }
        }

        private CohenAndCoonMethodPI cohenAndCoonMethodPI = new CohenAndCoonMethodPI();
        public CohenAndCoonMethodPI CohenAndCoonMethodPI
        {
            get
            {
                return cohenAndCoonMethodPI;
            }

            set
            {
                cohenAndCoonMethodPI = value;
                NotifyPropertyChanged("CohenAndCoonMethodPI");
            }
        }

        private FertikSharpeMethodPI fertikSharpeMethodPI = new FertikSharpeMethodPI();
        public FertikSharpeMethodPI FertikSharpeMethodPI
        {
            get
            {
                return fertikSharpeMethodPI;
            }

            set
            {
                fertikSharpeMethodPI = value;
                NotifyPropertyChanged("FertikSharpeMethodPI");
            }
        }

        private BorresenAndGrindalMethodPI borresenAndGrindalMethodPI = new BorresenAndGrindalMethodPI();
        public BorresenAndGrindalMethodPI BorresenAndGrindalMethodPI
        {
            get
            {
                return borresenAndGrindalMethodPI;
            }

            set
            {
                borresenAndGrindalMethodPI = value;
                NotifyPropertyChanged("BorresenAndGrindalMethodPI");
            }
        }

        private McMillanMethodPI mcMillanMethodPI = new McMillanMethodPI();
        public McMillanMethodPI McMillanMethodPI
        {
            get
            {
                return mcMillanMethodPI;
            }

            set
            {
                mcMillanMethodPI = value;
                NotifyPropertyChanged("McMillanMethodPI");
            }
        }

        private StClairMethodPI stClairMethodPI = new StClairMethodPI();
        public StClairMethodPI StClairMethodPI
        {
            get
            {
                return stClairMethodPI;
            }

            set
            {
                stClairMethodPI = value;
                NotifyPropertyChanged("StClairMethodPI");
            }
        }

        private FaanesAndSkogestadMethodPI faanesAndSkogestadMethodPI = new FaanesAndSkogestadMethodPI();
        public FaanesAndSkogestadMethodPI FaanesAndSkogestadMethodPI
        {
            get
            {
                return faanesAndSkogestadMethodPI;
            }

            set
            {
                faanesAndSkogestadMethodPI = value;
                NotifyPropertyChanged("FaanesAndSkogestadMethodPI");
            }
        }

        private ChienRegulatorMethodPI chienRegulatorMethodPI = new ChienRegulatorMethodPI();
        public ChienRegulatorMethodPI ChienRegulatorMethodPI
        {
            get
            {
                return chienRegulatorMethodPI;
            }

            set
            {
                chienRegulatorMethodPI = value;
                NotifyPropertyChanged("ChienRegulatorMethodPI");
            }
        }

        private IAEMurrilMethodPI iAEMurrilMethodPI = new IAEMurrilMethodPI();
        public IAEMurrilMethodPI IAEMurrilMethodPI
        {
            get
            {
                return iAEMurrilMethodPI;
            }

            set
            {
                iAEMurrilMethodPI = value;
                NotifyPropertyChanged("IAEMurrilMethodPI");
            }
        }

        private IAEPembertonMethodPI iAEPembertonMethodPI = new IAEPembertonMethodPI();
        public IAEPembertonMethodPI IAEPembertonMethodPI
        {
            get
            {
                return iAEPembertonMethodPI;
            }

            set
            {
                iAEPembertonMethodPI = value;
                NotifyPropertyChanged("IAEPembertonMethodPI");
            }
        }

        private IAEMarlinMethodPI iAEMarlinMethodPI = new IAEMarlinMethodPI();
        public IAEMarlinMethodPI IAEMarlinMethodPI
        {
            get
            {
                return iAEMarlinMethodPI;
            }

            set
            {
                iAEMarlinMethodPI = value;
                NotifyPropertyChanged("IAEMarlinMethodPI");
            }
        }

        private IAEAOMethodPI iAEAOMethodPI = new IAEAOMethodPI();
        public IAEAOMethodPI IAEAOMethodPI
        {
            get
            {
                return iAEAOMethodPI;
            }

            set
            {
                iAEAOMethodPI = value;
                NotifyPropertyChanged("IAEAOMethodPI");
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

        private IAETFMethodPI iAETFMethodPI = new IAETFMethodPI();
        public IAETFMethodPI IAETFMethodPI
        {
            get
            {
                return iAETFMethodPI;
            }

            set
            {
                iAETFMethodPI = value;
                NotifyPropertyChanged("IAETFMethodPI");
            }
        }

        private ISEZhuangMethodPI iSEZhuangMethodPI = new ISEZhuangMethodPI();
        public ISEZhuangMethodPI ISEZhuangMethodPI
        {
            get
            {
                return iSEZhuangMethodPI;
            }

            set
            {
                iSEZhuangMethodPI = value;
                NotifyPropertyChanged("ISEZhuangMethodPI");
            }
        }

        private ISEMurrillMethodPI iSEMurrillMethodPI = new ISEMurrillMethodPI();
        public ISEMurrillMethodPI ISEMurrillMethodPI
        {
            get
            {
                return iSEMurrillMethodPI;
            }

            set
            {
                iSEMurrillMethodPI = value;
                NotifyPropertyChanged("ISEMurrillMethodPI");
            }
        }

        private ITAEMurrillMethodPI iTAEMurrillMethodPI = new ITAEMurrillMethodPI();
        public ITAEMurrillMethodPI ITAEMurrillMethodPI
        {
            get
            {
                return iTAEMurrillMethodPI;
            }

            set
            {
                iTAEMurrillMethodPI = value;
                NotifyPropertyChanged("ITAEMurrillMethodPI");
            }
        }

        private ITAEAOMethodPI iTAEAOMethodPI = new ITAEAOMethodPI();
        public ITAEAOMethodPI ITAEAOMethodPI
        {
            get
            {
                return iTAEAOMethodPI;
            }

            set
            {
                iTAEAOMethodPI = value;
                NotifyPropertyChanged("ITAEAOMethodPI");
            }
        }

        private ITAEBarberaMethodPI iTAEBarberaMethodPI = new ITAEBarberaMethodPI();
        public ITAEBarberaMethodPI ITAEBarberaMethodPI
        {
            get
            {
                return iTAEBarberaMethodPI;
            }

            set
            {
                iTAEBarberaMethodPI = value;
                NotifyPropertyChanged("ITAEBarberaMethodPI");
            }
        }

        private ITAEABBMethodPI iTAEABBMethodPI = new ITAEABBMethodPI();
        public ITAEABBMethodPI ITAEABBMethodPI
        {
            get
            {
                return iTAEABBMethodPI;
            }

            set
            {
                iTAEABBMethodPI = value;
                NotifyPropertyChanged("ITAEABBMethodPI");
            }
        }

        private ISTSEZhuangMethodPI iSTSEZhuangMethodPI = new ISTSEZhuangMethodPI();
        public ISTSEZhuangMethodPI ISTSEZhuangMethodPI
        {
            get
            {
                return iSTSEZhuangMethodPI;
            }

            set
            {
                iSTSEZhuangMethodPI = value;
                NotifyPropertyChanged("ISTSEZhuangMethodPI");
            }
        }

        private ChienServoMethodPI chienServoMethodPI = new ChienServoMethodPI();
        public ChienServoMethodPI ChienServoMethodPI
        {
            get
            {
                return chienServoMethodPI;
            }

            set
            {
                chienServoMethodPI = value;
                NotifyPropertyChanged("ChienServoMethodPI");
            }
        }

        private VariableSPMethodPI variableSPMethodPI = new VariableSPMethodPI();
        public VariableSPMethodPI VariableSPMethodPI
        {
            get
            {
                return variableSPMethodPI;
            }

            set
            {
                variableSPMethodPI = value;
                NotifyPropertyChanged("VariableSPMethodPI");
            }
        }

        private VariableDistrubanceMethodPI variableDistrubanceMethodPI = new VariableDistrubanceMethodPI();
        public VariableDistrubanceMethodPI VariableDistrubanceMethodPI
        {
            get
            {
                return variableDistrubanceMethodPI;
            }

            set
            {
                variableDistrubanceMethodPI = value;
                NotifyPropertyChanged("VariableDistrubanceMethodPI");
            }
        }

        private SmithMethodPI smithMethodPI = new SmithMethodPI();
        public SmithMethodPI SmithMethodPI
        {
            get
            {
                return smithMethodPI;
            }

            set
            {
                smithMethodPI = value;
                NotifyPropertyChanged("SmithMethodPI");
            }
        }

        private BekkerMethodPI bekkerMethodPI = new BekkerMethodPI();
        public BekkerMethodPI BekkerMethodPI
        {
            get
            {
                return bekkerMethodPI;
            }

            set
            {
                bekkerMethodPI = value;
                NotifyPropertyChanged("BekkerMethodPI");
            }
        }

        private KuhnMethodPI kuhnMethodPI = new KuhnMethodPI();
        public KuhnMethodPI KuhnMethodPI
        {
            get
            {
                return kuhnMethodPI;
            }

            set
            {
                kuhnMethodPI = value;
                NotifyPropertyChanged("KuhnMethodPI");
            }
        }

        private TrybusMethodPI trybusMethodPI = new TrybusMethodPI();
        public TrybusMethodPI TrybusMethodPI
        {
            get
            {
                return trybusMethodPI;
            }

            set
            {
                trybusMethodPI = value;
                NotifyPropertyChanged("TrybusMethodPI");
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

        private ClarkeMethodPI clarkeMethodPI = new ClarkeMethodPI();
        public ClarkeMethodPI ClarkeMethodPI
        {
            get
            {
                return clarkeMethodPI;
            }

            set
            {
                clarkeMethodPI = value;
                NotifyPropertyChanged("ClarkeMethodPI");
            }
        }

        private WangMethodPI wangMethodPI = new WangMethodPI();
        public WangMethodPI WangMethodPI
        {
            get
            {
                return wangMethodPI;
            }

            set
            {
                wangMethodPI = value;
                NotifyPropertyChanged("WangMethodPI");
            }
        }

        private VariableSPMethodPID variableSPMethodPID = new VariableSPMethodPID();
        public VariableSPMethodPID VariableSPMethodPID
        {
            get
            {
                return variableSPMethodPID;
            }

            set
            {
                variableSPMethodPID = value;
                NotifyPropertyChanged("VariableSPMethodPID");
            }
        }

        private VariableDisturbanceMethodPID variableDisturbanceMethodPID = new VariableDisturbanceMethodPID();
        public VariableDisturbanceMethodPID VariableDisturbanceMethodPID
        {
            get
            {
                return variableDisturbanceMethodPID;
            }

            set
            {
                variableDisturbanceMethodPID = value;
                NotifyPropertyChanged("VariableDisturbanceMethodPID");
            }
        }

        private IAEAlfaRuizMethodPIDRegulator iAEAlfaRuizMethodPID = new IAEAlfaRuizMethodPIDRegulator();
        public IAEAlfaRuizMethodPIDRegulator IAEAlfaRuizMethodPID
        {
            get
            {
                return iAEAlfaRuizMethodPID;
            }

            set
            {
                iAEAlfaRuizMethodPID = value;
                NotifyPropertyChanged("IAEAlfaRuizMethodPID");
            }
        }

        private IAEAlfaRuizMethodPIDServo iAEAlfaRuizMethodPIDServo = new IAEAlfaRuizMethodPIDServo();
        public IAEAlfaRuizMethodPIDServo IAEAlfaRuizMethodPIDServo
        {
            get
            {
                return iAEAlfaRuizMethodPIDServo;
            }

            set
            {
                iAEAlfaRuizMethodPIDServo = value;
                NotifyPropertyChanged("IAEAlfaRuizMethodPIDServo");
            }
        }

        private IAEAOMethodPID iAEAOMethodPID = new IAEAOMethodPID();
        public IAEAOMethodPID IAEAOMethodPID
        {
            get
            {
                return iAEAOMethodPID;
            }

            set
            {
                iAEAOMethodPID = value;
                NotifyPropertyChanged("IAEAOMethodPID");
            }
        }

        private ITAEAlfaRuizMethodPID iTAEAlfaRuizMethodPID = new ITAEAlfaRuizMethodPID();
        public ITAEAlfaRuizMethodPID ITAEAlfaRuizMethodPID
        {
            get
            {
                return iTAEAlfaRuizMethodPID;
            }

            set
            {
                iTAEAlfaRuizMethodPID = value;
                NotifyPropertyChanged("ITAEAlfaRuizMethodPID");
            }
        }

        private ITAEAOMethodPID iTAEAOMethodPID = new ITAEAOMethodPID();
        public ITAEAOMethodPID ITAEAOMethodPID
        {
            get
            {
                return iTAEAOMethodPID;
            }

            set
            {
                iTAEAOMethodPID = value;
                NotifyPropertyChanged("ITAEAOMethodPID");
            }
        }

        private ISEArrietaVilanovaMethodPID iSEArrietaVilanovaMethodPID = new ISEArrietaVilanovaMethodPID();
        public ISEArrietaVilanovaMethodPID ISEArrietaVilanovaMethodPID
        {
            get
            {
                return iSEArrietaVilanovaMethodPID;
            }

            set
            {
                iSEArrietaVilanovaMethodPID = value;
                NotifyPropertyChanged("ISEArrietaVilanovaMethodPID");
            }

        }

        private ISTSEArrietaVilanovaMethodPID iSTSEArrietaVilanovaMethodPID = new ISTSEArrietaVilanovaMethodPID();
        public ISTSEArrietaVilanovaMethodPID ISTSEArrietaVilanovaMethodPID
        {
            get
            {
                return iSTSEArrietaVilanovaMethodPID;
            }

            set
            {
                iSTSEArrietaVilanovaMethodPID = value;
                NotifyPropertyChanged("ISTSEArrietaVilanovaMethodPID");
            }

        }

        private ISTESArrietaVilanovaMethodPID iSTESArrietaVilanovaMethodPID = new ISTESArrietaVilanovaMethodPID();
        public ISTESArrietaVilanovaMethodPID ISTESArrietaVilanovaMethodPID
        {
            get
            {
                return iSTESArrietaVilanovaMethodPID;
            }

            set
            {
                iSTESArrietaVilanovaMethodPID = value;
                NotifyPropertyChanged("ISTESArrietaVilanovaMethodPID");
            }

        }

        private KuhnMethodPID kuhnMethodPID = new KuhnMethodPID();
        public KuhnMethodPID KuhnMethodPID
        {
            get
            {
                return kuhnMethodPID;
            }

            set
            {
                kuhnMethodPID = value;
                NotifyPropertyChanged("KuhnMethodPID");
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
            }
        }

        private TunningMethodType[] possibleTuningTypes = new TunningMethodType[]
        {
            new TunningMethodType(TunningType.ProcessReaction),
            new TunningMethodType(TunningType.RegulatorTuning),
            new TunningMethodType(TunningType.ServoTuning),
            new TunningMethodType(TunningType.OtherTuning),
            new TunningMethodType(TunningType.TimeDomain)
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

            CallenderMethodStackPanel.DataContext = CallenderMethodPI;
            ChienRegulatorMethodStackPanel.DataContext = ChienRegulatorMethodPI;
            ChienServoMethodStackPanel.DataContext = ChienServoMethodPI;
            SmithMethodStackPanel.DataContext = SmithMethodPI;
            BekkerMethodStackPanel.DataContext = BekkerMethodPI;
            KuhnMethodStackPanel.DataContext = KuhnMethodPI;
            TrybusMethodStackPanel.DataContext = TrybusMethodPI;

            StepDisturbancePMethodStackPanel.DataContext = VariableDisturbanceMethodP;
            StepDisturbancePIMethodStackPanel.DataContext = VariableDistrubanceMethodPI;
            StepDisturbancePIDMethodStackPanel.DataContext = VariableDisturbanceMethodPID;
            StepSPPMethodStackPanel.DataContext = VariableSPMethodP;
            StepSPPIMethodStackPanel.DataContext = VariableSPMethodPI;
            StepSPPIDMethodStackPanel.DataContext = VariableSPMethodPID;
            KuhnMethodStackPanel.DataContext = KuhnMethodPID;
        }

        public void InitTuningMethods()
        {
            tuningMethods = new FOLPFModelTunningMethodBase[]
            {
                VariableSPMethodP,
                VariableDisturbanceMethodP,
                CallenderMethodPI,
                ZNMethodPI,
                CohenAndCoonMethodPI,
                FertikSharpeMethodPI,
                BorresenAndGrindalMethodPI,
                McMillanMethodPI,
                StClairMethodPI,
                FaanesAndSkogestadMethodPI,
                ChienRegulatorMethodPI,
                IAEMurrilMethodPI,
                IAEPembertonMethodPI,
                IAEMarlinMethodPI,
                IAEAOMethodPI,
                IAEShinskeyMethodPI,
                IAETFMethodPI,
                ISEZhuangMethodPI,
                ISEMurrillMethodPI,
                ITAEMurrillMethodPI,
                ITAEAOMethodPI,
                ITAEBarberaMethodPI,
                ITAEABBMethodPI,
                ISTSEZhuangMethodPI,
                ChienServoMethodPI,
                VariableSPMethodPI,
                VariableDistrubanceMethodPI,
                SmithMethodPI,
                BekkerMethodPI,
                KuhnMethodPI,
                TrybusMethodPI,
                AH1MethodPI,
                AH2MethodPI,
                AH3MethodPI,
                ClarkeMethodPI,
                WangMethodPI,
                VariableSPMethodPID,
                VariableDisturbanceMethodPID,
                IAEAlfaRuizMethodPID,
                IAEAlfaRuizMethodPIDServo,
                IAEAOMethodPID,
                ITAEAlfaRuizMethodPID,
                ITAEAOMethodPID,
                ISEArrietaVilanovaMethodPID,
                ISTSEArrietaVilanovaMethodPID,
                ISTESArrietaVilanovaMethodPID,
                KuhnMethodPID
            };
        }

        public void RefreshChoosenMethods()
        {
            if (AlgorithmTypeComboBox.SelectedItem != null && TuningMethodTypeComboBox.SelectedItem != null)
            {
                TuningMethodToDisplay.Methods = new ObservableCollection<FOLPFModelTunningMethodBase>((from method in tuningMethods
                                                                                                       where method.TypeOfAglorithm == ((PIDAlgoritm)AlgorithmTypeComboBox.SelectedItem).Mode && method.TypeOfTuning == ((TunningMethodType)TuningMethodTypeComboBox.SelectedItem).Type
                                                                                                       select method));
                MethodsComboBox.SelectedIndex = 0;
            }
        }

        public void RefreshChoosenTuningMethods()
        {
            if (AlgorithmTypeComboBox.SelectedItem != null)
            {
                PossibleTuningTypes = (from type in ((  from method in tuningMethods
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
            if (MethodsComboBox.SelectedItem == CallenderMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                    {
                        CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                        ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                    }));
            }
            else if (MethodsComboBox.SelectedItem == ChienRegulatorMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == ChienServoMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == SmithMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == BekkerMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == KuhnMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == TrybusMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == VariableSPMethodP)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == VariableSPMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == VariableSPMethodPID)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == VariableDisturbanceMethodP)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == VariableDistrubanceMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == VariableDisturbanceMethodPID)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if (MethodsComboBox.SelectedItem == KuhnMethodPID)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Visible;

                }));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CallenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienRegulatorMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    ChienServoMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SmithMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    BekkerMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    TrybusMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepDisturbancePIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StepSPPIDMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    KuhnMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PIDController = ((FOLPFModelTunningMethodBase)MethodsComboBox.SelectedItem).TuningMethod(plantObject);
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

            switch(mode)
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
            }
        }
    }


    public class MethodsCollectionView: INotifyPropertyChanged
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
        private ObservableCollection<FOLPFModelTunningMethodBase> methods = new ObservableCollection<FOLPFModelTunningMethodBase>();
        public ObservableCollection<FOLPFModelTunningMethodBase> Methods
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
