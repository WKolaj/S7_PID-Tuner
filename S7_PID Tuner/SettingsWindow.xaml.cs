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

namespace S7_PID_Tuner
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        /// <summary>
        /// Obiekt sterownika za pomoc ktorego aplikacja laczy sie z rzeczywsitym urzadzeniem
        /// </summary>
        private PIDControllerDevice controllerDevice;

        /// <summary>
        /// Konstruktor klasy okna ustawien
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Metoda przypisujaca konstruktor obiekt sterownika do kontrolki
        /// </summary>
        /// <param name="controllerDevice">
        /// Obiekt sterownika
        /// </param>
        public void AssignControllerToDevice(PIDControllerDevice controllerDevice)
        {
            this.controllerDevice = controllerDevice;
            DataContext = controllerDevice;

            //Jezeli jest tryb online - nalezy zablokowac textbox od czasu probkowania
            if(controllerDevice.Connected)
            {
                PIDsampleTimeTextBox.IsEnabled = false;
            }
     
        }

        /// <summary>
        /// Metoda obslugi zdarzenia wcisniecia Ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

    }
}
