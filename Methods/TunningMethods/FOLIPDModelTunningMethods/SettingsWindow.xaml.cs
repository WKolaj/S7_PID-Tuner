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

namespace FOLIPDModelTunningMethods
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

        private FOLIPDModelTunningMethodBase[] tuningMethods;

        private CoonMethodP coonMethodP = new CoonMethodP();
        public CoonMethodP CoonMethodP
        {
            get
            {
                return coonMethodP;
            }

            set
            {
                coonMethodP = value;
                NotifyPropertyChanged("CoonMethodP");
            }
        }

        private ShinskeyMethodP shinskeyMethodP = new ShinskeyMethodP();
        public ShinskeyMethodP ShinskeyMethodP
        {
            get
            {
                return shinskeyMethodP;
            }

            set
            {
                shinskeyMethodP = value;
                NotifyPropertyChanged("ShinskeyMethodP");
            }
        }

        private VelaquezFigueroaRegulatorMethodP velaquezFigueroaRegulatorMethodP = new VelaquezFigueroaRegulatorMethodP();
        public VelaquezFigueroaRegulatorMethodP VelaquezFigueroaRegulatorMethodP
        {
            get
            {
                return velaquezFigueroaRegulatorMethodP;
            }

            set
            {
                velaquezFigueroaRegulatorMethodP = value;
                NotifyPropertyChanged("VelaquezFigueroaRegulatorMethodP");
            }
        }

        private VelaquezFigueroaServoMethodP velaquezFigueroaServoMethodP = new VelaquezFigueroaServoMethodP();
        public VelaquezFigueroaServoMethodP VelaquezFigueroaServoMethodP
        {
            get
            {
                return velaquezFigueroaServoMethodP;
            }

            set
            {
                velaquezFigueroaServoMethodP = value;
                NotifyPropertyChanged("VelaquezFigueroaServoMethodP");
            }
        }

        private HubaZakovaMethodP hubaZakovaMethodP = new HubaZakovaMethodP();
        public HubaZakovaMethodP HubaZakovaMethodP
        {
            get
            {
                return hubaZakovaMethodP;
            }

            set
            {
                hubaZakovaMethodP = value;
                NotifyPropertyChanged("HubaZakovaMethodP");
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

        private VelaquezFigueroaRegulatorMethodPI velaquezFigueroaRegulatorMethodPI = new VelaquezFigueroaRegulatorMethodPI();
        public VelaquezFigueroaRegulatorMethodPI VelaquezFigueroaRegulatorMethodPI
        {
            get
            {
                return velaquezFigueroaRegulatorMethodPI;
            }

            set
            {
                velaquezFigueroaRegulatorMethodPI = value;
                NotifyPropertyChanged("VelaquezFigueroaRegulatorMethodPI");
            }
        }

        private VelaquezFigueroaServoMethodPI velaquezFigueroaServoMethodPI = new VelaquezFigueroaServoMethodPI();
        public VelaquezFigueroaServoMethodPI VelaquezFigueroaServoMethodPI
        {
            get
            {
                return velaquezFigueroaServoMethodPI;
            }

            set
            {
                velaquezFigueroaServoMethodPI = value;
                NotifyPropertyChanged("VelaquezFigueroaServoMethodPI");
            }
        }

        private HubaZakovaMethodPI hubaZakovaMethodPI = new HubaZakovaMethodPI();
        public HubaZakovaMethodPI HubaZakovaMethodPI
        {
            get
            {
                return hubaZakovaMethodPI;
            }

            set
            {
                hubaZakovaMethodPI = value;
                NotifyPropertyChanged("HubaZakovaMethodPI");
            }
        }

        private PIDAlgoritm[] possibleAlgoritms = new PIDAlgoritm[]
        {
            new PIDAlgoritm(PIDModeType.P),
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

            ITAEPoulinMethodPIMethodStackPanel.DataContext = ITAEPoulinMethodPI;


        }

        public void InitTuningMethods()
        {
            tuningMethods = new FOLIPDModelTunningMethodBase[]
            {
                CoonMethodP,
                ShinskeyMethodP,
                VelaquezFigueroaRegulatorMethodP,
                VelaquezFigueroaServoMethodP,
                HubaZakovaMethodP,
                ITAEPoulinMethodPI,
                VelaquezFigueroaRegulatorMethodPI,
                VelaquezFigueroaServoMethodPI,
                HubaZakovaMethodPI
            };
        }

        public void RefreshChoosenMethods()
        {
            if (AlgorithmTypeComboBox.SelectedItem != null && TuningMethodTypeComboBox.SelectedItem != null)
            {
                TuningMethodToDisplay.Methods = new ObservableCollection<FOLIPDModelTunningMethodBase>((from method in tuningMethods
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
            if (MethodsComboBox.SelectedItem == ITAEPoulinMethodPI)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    ITAEPoulinMethodPIMethodStackPanel.Visibility = System.Windows.Visibility.Visible;

                }));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    ITAEPoulinMethodPIMethodStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                }));
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PIDController = ((FOLIPDModelTunningMethodBase)MethodsComboBox.SelectedItem).TuningMethod(plantObject);
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
        private ObservableCollection<FOLIPDModelTunningMethodBase> methods = new ObservableCollection<FOLIPDModelTunningMethodBase>();
        public ObservableCollection<FOLIPDModelTunningMethodBase> Methods
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
