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

namespace DelayModelTunningMethods
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

        private DelayModelTunningMethodBase[] tuningMethods;

        private CallenderMethod callenderMethod = new CallenderMethod();
        public CallenderMethod CallenderMethod
        {
            get
            {
                return callenderMethod;
            }

            set
            {
                callenderMethod = value;
                NotifyPropertyChanged("CallenderMethod");
            }
        }

        private WolfeMethod wolfeMethod = new WolfeMethod();
        public WolfeMethod WolfeMethod
        {
            get
            {
                return wolfeMethod;
            }

            set
            {
                wolfeMethod= value;
                NotifyPropertyChanged("WolfeMethod");
            }
        }

        private MinimumError minimumMethod = new MinimumError();
        public MinimumError MinimumMethod
        {
            get
            {
                return minimumMethod;
            }

            set
            {
                minimumMethod = value;
                NotifyPropertyChanged("MinimumMethod");
            }
        }

        private MinimumIAERegulatorTuning minimumIAERegulatorMethod = new MinimumIAERegulatorTuning();
        public MinimumIAERegulatorTuning MinimumIAERegulatorMethod
        {
            get
            {
                return minimumIAERegulatorMethod;
            }

            set
            {
                minimumIAERegulatorMethod = value;
                NotifyPropertyChanged("MinimumIAERegulatorMethod");
            }
        }

        private MinimumIAEServoTuning minimumIAEServoMethod = new MinimumIAEServoTuning();
        public MinimumIAEServoTuning MinimumIAEServoMethod
        {
            get
            {
                return minimumIAEServoMethod;
            }

            set
            {
                minimumIAEServoMethod = value;
                NotifyPropertyChanged("MinimumIAEServoMethod");
            }
        }

        private FetrikMethod fetrikMethod = new FetrikMethod();
        public FetrikMethod FetrikMethod
        {
            get
            {
                return fetrikMethod;
            }

            set
            {
                fetrikMethod = value;
                NotifyPropertyChanged("FetrikMethod");
            }
        }

        private AHMethod aHMethod = new AHMethod();
        public AHMethod AHMethod
        {
            get
            {
                return aHMethod;
            }

            set
            {
                aHMethod = value;
                NotifyPropertyChanged("AHMethod");
            }
        }

        private SkogestadMethod skogestadMethod = new SkogestadMethod();
        public SkogestadMethod SkogestadMethod
        {
            get
            {
                return skogestadMethod;
            }

            set
            {
                skogestadMethod = value;
                NotifyPropertyChanged("SkogestadMethod");
            }
        }

        private VanDerGrintenMethod vanDerGrintenMethod = new VanDerGrintenMethod();
        public VanDerGrintenMethod VanDerGrintenMethod
        {
            get
            {
                return vanDerGrintenMethod;
            }

            set
            {
                vanDerGrintenMethod = value;
                NotifyPropertyChanged("VanDerGrintenMethod");
            }
        }

        private HansenMethod hansenMethod = new HansenMethod();
        public HansenMethod HansenMethod
        {
            get
            {
                return hansenMethod;
            }

            set
            {
                hansenMethod = value;
                NotifyPropertyChanged("HansenMethod");
            }
        }

        private McMillanMethod mcMillanMethod = new McMillanMethod();
        public McMillanMethod McMillanMethod
        {
            get
            {
                return mcMillanMethod;
            }

            set
            {
                mcMillanMethod = value;
                NotifyPropertyChanged("McMillanMethod");
            }
        }

        private AHDampingMethod aHDampingMethod = new AHDampingMethod();
        public AHDampingMethod AHDampingMethod
        {
            get
            {
                return aHDampingMethod;
            }

            set
            {
                aHDampingMethod = value;
                NotifyPropertyChanged("AHDampingMethod");
            }
        }

        private AHClosedLoopSensitivityMethod aHClosedLoopSensitivityMethod = new AHClosedLoopSensitivityMethod();
        public AHClosedLoopSensitivityMethod AHClosedLoopSensitivityMethod
        {
            get
            {
                return aHClosedLoopSensitivityMethod;
            }

            set
            {
                aHClosedLoopSensitivityMethod = value;
                NotifyPropertyChanged("AHClosedLoopSensitivityMethod");
            }
        }

        private PIDAlgoritm[] possibleAlgoritms = new PIDAlgoritm[]
        {
            new PIDAlgoritm(PIDModeType.PI)
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
            CallenderMethodComboBox.DataContext = CallenderMethod;
            AHDampingMethodComboBox.DataContext = AHDampingMethod;
            AHCLSMethodComboBox.DataContext = AHClosedLoopSensitivityMethod;
        }

        public void InitTuningMethods()
        {
            tuningMethods = new DelayModelTunningMethodBase[]
            {
                CallenderMethod,
                WolfeMethod,
                MinimumMethod,
                MinimumIAERegulatorMethod,
                MinimumIAEServoMethod,
                FetrikMethod,
                AHMethod,
                SkogestadMethod,
                VanDerGrintenMethod,
                HansenMethod,
                McMillanMethod,
                AHDampingMethod,
                AHClosedLoopSensitivityMethod
            };
        }

        public void RefreshChoosenMethods()
        {
            if (AlgorithmTypeComboBox.SelectedItem != null && TuningMethodTypeComboBox.SelectedItem != null)
            {
                Object sel = AlgorithmTypeComboBox.SelectedItem;
                Debug.WriteLine(sel.GetType());
                TuningMethodToDisplay.Methods = new ObservableCollection<DelayModelTunningMethodBase>((from method in tuningMethods
                                                                                                       where method.TypeOfAglorithm == ((PIDAlgoritm)AlgorithmTypeComboBox.SelectedItem).Mode && method.TypeOfTuning == ((TunningMethodType)TuningMethodTypeComboBox.SelectedItem).Type
                                                                                                       select method));
                MethodsComboBox.SelectedIndex = 0;
            }
        }

        private void AlgorithmTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshChoosenMethods();
        }

        private void TuningMethodTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshChoosenMethods();
        }

        private void MethodsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(MethodsComboBox.SelectedItem == CallenderMethod)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                    {
                        AHDampingMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        AHCLSMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        callenderMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                            
                    }));
            }
            else if(MethodsComboBox.SelectedItem == AHClosedLoopSensitivityMethod)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    AHDampingMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    AHCLSMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    callenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else if(MethodsComboBox.SelectedItem == AHDampingMethod)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    AHDampingMethodStackPanel.Visibility = System.Windows.Visibility.Visible;
                    AHCLSMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    callenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
            else 
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    AHDampingMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    AHCLSMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    callenderMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                }));
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PIDController = ((DelayModelTunningMethodBase)MethodsComboBox.SelectedItem).TuningMethod(plantObject);
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
        private ObservableCollection<DelayModelTunningMethodBase> methods = new ObservableCollection<DelayModelTunningMethodBase>();
        public ObservableCollection<DelayModelTunningMethodBase> Methods
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
