using OxyPlot;
using System;
using System.Collections.Generic;
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

namespace S7_PID_Tuner
{
    /// <summary>
    /// Interaction logic for PerfomanceIndexControl.xaml
    /// </summary>
    public partial class PerfomanceIndexControl : UserControl,INotifyPropertyChanged
    {

        private Double[] PV = null;
        private Double[] SP = null;
        private Int32 SampleTime;

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


        private PerformanceIndex perfomanceIndex;
        public PerformanceIndex PerformanceIndex
        {
            get
            {
                return perfomanceIndex;
            }

            private set
            {
                perfomanceIndex = value;
            }
        }

        private Boolean isChecked = true;
        public Boolean IsChecked
        {
            get
            {
                return isChecked;
            }

            set
            {
                Refresh();
                isChecked = value;
                NotifyPropertyChanged("IsChecked");
                RefreshValueVisibility();
            }
        }

        public void RefreshValueVisibility()
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (IsChecked)
                    {
                        valueTextBox.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        valueTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    }

                }));
           
        }

        public PerfomanceIndexControl()
        {
            InitializeComponent();
        }

        public void Init(PerformanceIndex perfomanceIndex)
        {
            this.perfomanceIndex = perfomanceIndex;
            indexCheckBox.DataContext = this;
            valueTextBox.DataContext = this.perfomanceIndex;


            Dispatcher.BeginInvoke(new Action(() =>
                {
                    indexCheckBox.Content = this.perfomanceIndex.Name;
                }));
        }

        public void Refresh()
        {
            if(perfomanceIndex!=null && PV!=null && SP!=null)
            {
                perfomanceIndex.RefreshValue(PV,SP,SampleTime);
            }
        }

        public void AssignPVAndSP(Double[] PV, Double[] SP, Int32 SampleTime)
        {
            this.PV = PV;
            this.SP = SP;
            this.SampleTime = SampleTime;
        }
    }
}
