using DynamicMethodsLibrary;
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

namespace StrejcMethod
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        IdentificationWindow window;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        public void InitializeWindow(IdentificationWindow window)
        {
            this.window = window;
            StepSizeBox.DataContext = window;
            StepTimeBox.DataContext = window;
            sensitivityBox.DataContext = window;
            aproximationPointsTextBox.DataContext = window;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
