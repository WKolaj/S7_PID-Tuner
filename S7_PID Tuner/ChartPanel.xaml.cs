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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace S7_PID_Tuner
{
    /// <summary>
    /// Interaction logic for ChartPanel.xaml
    /// </summary>
    public partial class ChartPanel : UserControl
    {
        /// <summary>
        /// Aktualny projekt
        /// </summary>
        Project currentProject;

        /// <summary>
        /// Konstruktor kontrolki panelu wykresow
        /// </summary>
        public ChartPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Metoda przypisujaca kontrolce aktualny projekt
        /// </summary>
        /// <param name="project">
        /// Nowy projekt
        /// </param>
        public void AssignNewProject(Project project)
        {
            this.currentProject = project;

            //Przypisanie aktualnego projektu do pozostalych kontrolek wbudowanych
            nyquistPlotControl.AssignNewProject(currentProject);
            bodePlotControl.AssignNewProject(currentProject);
            performanceChartControl.AssignNewProject(currentProject);
        }

    }
}
