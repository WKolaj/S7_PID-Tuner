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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace S7_PID_Tuner
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        PIDControllerDevice controllerDevice;

        public ConnectionWindow()
        {
            InitializeComponent();
        }

        public void InitDevice(PIDControllerDevice currentDevice)
        {
            this.controllerDevice = currentDevice;

            Loaded += delegate
            {
                Storyboard blinkAnimation = (Storyboard)Resources["blinkAnimation"];
                blinkAnimation.Begin();

                TryToConnect();
            };

        }

        private bool loaded = false;

        public void TryToConnect()
        {
            Task.Factory.StartNew(new Action(() =>
                {
                    if (!loaded)
                    {
                        loaded = true;

                        Boolean isOk = false;

                        if (!controllerDevice.Connected)
                        {
                            controllerDevice.Connect();
                            controllerDevice.WaitUntilConnectionStateChanged();

                            if (controllerDevice.Connected)
                            {
                                try
                                {
                                    controllerDevice.TurnOnTuningMode();
                                }
                                catch(Exception exception)
                                {
                                    Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        System.Windows.MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        DialogResult = false;
                                    }));

                                    return;
                                }

                                System.Threading.Thread.Sleep(100);

                                controllerDevice.Mode = 4;

                                System.Threading.Thread.Sleep(100);

                                if(controllerDevice.TuningMode && controllerDevice.Mode == 4)
                                {
                                    isOk = true;
                                }

                            }

                        }
                        else
                        {
                            controllerDevice.TurnOnTuningMode();
                            isOk = true;
                        }

                        if (isOk)
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    DialogResult = true;
                                }));
                        }
                        else
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                                   {
                                       System.Windows.MessageBox.Show("Unable to connect to device", "Cannot connect to device", MessageBoxButton.OK, MessageBoxImage.Error);

                                       DialogResult = false;
                                   }));
                        }
                    }
                }));
            
        }

    }
}
