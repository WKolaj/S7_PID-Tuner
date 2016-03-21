using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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

namespace LSMethod
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SignalBuilderWindow : Window, INotifyPropertyChanged
    {
        public void AssignSignalFactor(SignalFactor signalFactor)
        {
            this.signalFactor = signalFactor;
            SignalFactor.SomePropertyChanged += RefreshOnParameterChange;
            InitSignalPoints();
            RefreshSignalPoints();
        }

        /// <summary>
        /// Mechanizm wiazania danych WPF
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Mechanizm wiazania danych WPF
        /// </summary>
        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private List<DataPoint> signalPoints = new List<DataPoint>();
        public List<DataPoint> SignalPoints
        {
            get
            {
                return signalPoints;
            }

            set
            {
                signalPoints = value;
                NotifyPropertyChanged("SignalPoints");
            }
        }

        public SignalType SignalType
        {
            get
            {
                return SignalFactor.SignalType;
            }

            set
            {
                SignalFactor.SignalType = value;
            }
        }

        private SignalFactor signalFactor = new SignalFactor();
        public SignalFactor SignalFactor
        {
            get
            {
                return signalFactor;
            }

            private set
            {
                signalFactor = value;

                NotifyPropertyChanged("SignalFactor");
            }
        }


        public void InitSignalPoints()
        {
            switch (this.SignalType)
            {
                case SignalType.Manual:
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                            {
                                manualRampRadioButton.IsChecked = true;
                            }));
                        break;
                    }

                case SignalType.Step:
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {

                            stepRadioButton.IsChecked = true;
                        }));
                        break;
                    }

                case SignalType.Pulse:
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            pulseRadioButton.IsChecked = true;
                        }));
                        break;
                    }

                case SignalType.DoublePulse:
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            doublePulseRadioButton.IsChecked = true;
                        }));
                        break;
                    }

                case SignalType.Ramp:
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            rampRadioButton.IsChecked = true;
                        }));
                        break;
                    }

                case SignalType.RampPulse:
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            pulseRampRadioButton.IsChecked = true;
                        }));
                        break;
                    }

            }
        }

        public void RefreshSignalPoints()
        {
            switch (this.SignalType)
            {
                case SignalType.Manual:
                    {
                        SignalPoints = new List<DataPoint>();
                        break;
                    }

                case SignalType.Step:
                    {
                        SignalPoints = SignalFactor.StepSignal.SignalPlot;
                        break;
                    }

                case SignalType.Pulse:
                    {
                        SignalPoints = SignalFactor.PulseSignal.SignalPlot;
                        break;
                    }

                case SignalType.DoublePulse:
                    {
                        SignalPoints = SignalFactor.DoublePulseSignal.SignalPlot;
                        break;
                    }

                case SignalType.Ramp:
                    {
                        SignalPoints = SignalFactor.RampSignal.SignalPlot;
                        break;
                    }

                case SignalType.RampPulse:
                    {
                        SignalPoints = SignalFactor.RampPulseSignal.SignalPlot;
                        break;
                    }

            }

            RefreshPlot();
        }

        public void RefreshPlot()
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                signalPlot.InvalidatePlot(true);
            }
                ));

        }

        public SignalBuilderWindow()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            DataContext = this;
            
        }

        private void stepRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                stepPanel.Visibility = System.Windows.Visibility.Visible;
            }));

            this.SignalType = LSMethod.SignalType.Step;

            RefreshSignalPoints();
        }

        private void stepRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                stepPanel.Visibility = System.Windows.Visibility.Collapsed;
            }));

        }

        private void pulseRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                pulsePanel.Visibility = System.Windows.Visibility.Visible;
            }));

            this.SignalType = LSMethod.SignalType.Pulse;

            RefreshSignalPoints();
        }

        private void pulseRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                pulsePanel.Visibility = System.Windows.Visibility.Collapsed;
            }));
        }

        private void doublePulseRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                doublePulsePanel.Visibility = System.Windows.Visibility.Visible;
            }));

            this.SignalType = LSMethod.SignalType.DoublePulse;

            RefreshSignalPoints();
        }

        private void doublePulseRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                doublePulsePanel.Visibility = System.Windows.Visibility.Collapsed;
            }));
        }

        private void rampRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                rampPanel.Visibility = System.Windows.Visibility.Visible;
            }));

            this.SignalType = LSMethod.SignalType.Ramp;

            RefreshSignalPoints();
        }

        private void rampRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                rampPanel.Visibility = System.Windows.Visibility.Collapsed;
            }));
        }

        private void pulseRampRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                rampPulsePanel.Visibility = System.Windows.Visibility.Visible;
            }));

            this.SignalType = LSMethod.SignalType.RampPulse;

            RefreshSignalPoints();
        }

        private void pulseRampRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                rampPulsePanel.Visibility = System.Windows.Visibility.Collapsed;
            }));
        }

        private void manualRampRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.SignalType = LSMethod.SignalType.Manual;

            RefreshSignalPoints();
        }

        private void manualRampRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void RefreshOnParameterChange()
        {
            RefreshSignalPoints();
        }

        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }


    }

    [ValueConversion(typeof(TimeSpan), typeof(String))]
    public class MinutesSecondsMiliTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            // TODO something like:
            return ((TimeSpan)value).ToString("mm\\:ss\\.fff");
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            // TODO something like:
            return TimeSpan.ParseExact(value.ToString(), "mm\\:ss\\.fff", CultureInfo.CurrentCulture);
        }
    }

    public class MinutesSecondsMiliTimeSpan : ValidationRule
    {
        // Implementing the abstract method in the Validation Rule class
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            TimeSpan time;

            if (TimeSpan.TryParseExact(value.ToString(), "mm\\:ss\\.fff", CultureInfo.CurrentCulture, out time))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, null);

            }
        }

    }
}
