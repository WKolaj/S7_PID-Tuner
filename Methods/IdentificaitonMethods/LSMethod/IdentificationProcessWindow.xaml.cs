using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using DynamicMethodsLibrary;

namespace LSMethod
{
    /// <summary>
    /// Interaction logic for IdentificationProcessWindow.xaml
    /// </summary>
    public partial class IdentificationProcessWindow : Window
    {
        private TransferFunctionClass transferFunction;
        public TransferFunctionClass TransferFunction
        {
            get
            {
                return transferFunction;
            }

            protected set
            {
                transferFunction = value;
            }
        }

        public void IdentificationDone()
        {
              Dispatcher.BeginInvoke(new Action(() =>
                {
                    DialogResult = true;
                }));
        }

        LSFactory factory;

        public IdentificationProcessWindow()
        {
            InitializeComponent();
        }

        public void InitializeProcessWindow(LSFactory factory)
        {
            this.factory = factory;

            ConnectWindow();

            Task.Factory.StartNew(new Action(() =>
                {
                    TransferFunction = factory.GetTransferFunctiomFactors();
                    IdentificationDone();
                }));
        }

        public void ConnectWindow()
        {
            factory.AutoDelayCalculationPercentageChanged += OnAutoDelayProgressChanged;
            factory.CalculationAutoDelayStarted += OnAutoDelayCalculationStarted;
            factory.CalculationAutoDelayStopped += OnAutoDelayCalculationStopped;

            factory.IdentificationProgressChanged += OnLSMethodProgressChanged;
            factory.IdentificationStarted += OnLSMethodStarted;
            factory.IdentificationStopped += OnLSMethodStopped;
        }

        public void OnLSMethodProgressChanged(Double progress)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    progressBar.Value = progress;
                }));
        }

        public void OnAutoDelayProgressChanged(Double progress)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                progressBar.Value = progress;
            }));
        }

        public void OnAutoDelayCalculationStarted()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                calculatingLabel.Content = "Calculating time delay...";
            }));
        }

        public void OnAutoDelayCalculationStopped()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                calculatingLabel.Content = "Calculating model...";
            }));
        }

        public void OnLSMethodStarted()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                calculatingLabel.Content = "Calculating model...";
            }));
        }

        public void OnLSMethodStopped()
        {
        }
    }
}
