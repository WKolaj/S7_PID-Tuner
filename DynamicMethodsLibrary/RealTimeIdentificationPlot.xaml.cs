using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DynamicMethodsLibrary
{
    public enum IdentificationMode
    {
        Normal,Identification,Stop
    }

    /// <summary>
    /// Interaction logic for RealTimeIdentificationPlot.xaml
    /// </summary>
    public partial class RealTimeIdentificationPlot : UserControl, INotifyPropertyChanged
    {
        internal void RefreshMode()
        {
            if(Mode != IdentificationMode.Stop)
            {
                ClearAll();
            }
        }

        private IdentificationControl identificationControl;

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

        private Int32 pointsLength = 500;
        public Int32 PointsLength
        {
            get
            {
                return pointsLength;
            }

            set
            {
                ClearAll();

                pointsLength = value;

                NotifyPropertyChanged("PointsLength");
            }
        }

        private IdentificationMode Mode
        {
            get
            {
                return identificationControl.Mode;
            }

            set
            {
                identificationControl.Mode = value;
            }
        }



        internal void AssignControl(IdentificationControl control)
        {
            this.identificationControl = control;
        }

        public RealTimeIdentificationPlot()
        {
            InitializeComponent();
            InitPointContext();
        }


        private void InitPointContext()
        {
            CVSeries.DataContext = this;
            PVSeries.DataContext = this;
            ModelSeries.DataContext = this;
        }

        private List<DataPoint> cVPoints = new List<DataPoint>();
        public List<DataPoint> CVPoints
        {
            get
            {
                return cVPoints;
            }

            private set
            {
                cVPoints = value;
            }
        }

        private List<DataPoint> pVPoints = new List<DataPoint>();
        public List<DataPoint> PVPoints
        {
            get
            {
                return pVPoints;
            }

            private set
            {
                pVPoints = value;
            }
        }

        private List<DataPoint> modelPoints = new List<DataPoint>();
        public List<DataPoint> ModelPoints
        {
            get
            {
                return modelPoints;
            }

            private set
            {
                modelPoints = value;
            }
        }

        private void RefreshPlot()
        {
            Dispatcher.BeginInvoke(new Action(()=>
            { 
                  
                        identificationChart.InvalidatePlot(true);
            }));

        }

      

        public void AddCVPoint(DateTime time, Double value)
        {
            switch (Mode)
            {
                case IdentificationMode.Identification:
                    {
                        CVPoints.Add(DateTimeAxis.CreateDataPoint(time, value));
                        RefreshPlot();
                        break;
                    }

                case IdentificationMode.Normal:
                    {
                        if (CVPoints.Count >= PointsLength)
                        {
                            CVPoints.RemoveAt(0);
                        }

                        CVPoints.Add(DateTimeAxis.CreateDataPoint(time, value));
                        RefreshPlot();
                        break;
                    }
            }

        }

        public void AddPVPoint(DateTime time, Double value)
        {
            switch (Mode)
            {
                case IdentificationMode.Identification:
                    {
                        PVPoints.Add(DateTimeAxis.CreateDataPoint(time, value));
                        RefreshPlot();
                        break;
                    }

                case IdentificationMode.Normal:
                    {
                        if (PVPoints.Count >= PointsLength)
                        {
                            PVPoints.RemoveAt(0);
                        }

                        PVPoints.Add(DateTimeAxis.CreateDataPoint(time, value));
                        RefreshPlot();
                        break;
                    }
            }

        }

        public void AddModelPoint(DateTime time, Double value)
        {
            switch (Mode)
            {
                case IdentificationMode.Stop:
                    {
                        ModelPoints.Add(DateTimeAxis.CreateDataPoint(time, value));
                        RefreshPlot();
                        break;
                    }
            }
        }

        public void AddRealTimeCVPoint(Double value)
        {
            switch (Mode)
            {
                case IdentificationMode.Identification:
                    {
                        CVPoints.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, value));
                        RefreshPlot();
                        break;
                    }

                case IdentificationMode.Normal:
                    {
                        if (CVPoints.Count >= PointsLength)
                        {
                            CVPoints.RemoveAt(0);
                        }

                        CVPoints.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, value));
                        RefreshPlot();
                        break;
                    }
            }

        }

        public void AddRealTimePVPoint(Double value)
        {
            switch (Mode)
            {
                case IdentificationMode.Identification:
                    {
                        PVPoints.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, value));
                        RefreshPlot();
                        break;
                    }

                case IdentificationMode.Normal:
                    {
                        if (PVPoints.Count >= PointsLength)
                        {
                            PVPoints.RemoveAt(0);
                        }

                        PVPoints.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, value));
                        RefreshPlot();
                        break;
                    }
            }
        }

        public void AddRealTimeModelPoint(Double value)
        {
            switch (Mode)
            {
                case IdentificationMode.Stop:
                    {
                        ModelPoints.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, value));
                        RefreshPlot();
                        break;
                    }
            }
        }

        private void ClearCVPoints()
        {
            CVPoints.Clear();
            RefreshPlot();
        }

        private void ClearPVPoints()
        {
            PVPoints.Clear();
            RefreshPlot();
        }

        private void ClearModelPoints()
        {
            ModelPoints.Clear();
            RefreshPlot();
        }

        public void ClearAll()
        {
            CVPoints.Clear();
            PVPoints.Clear();
            ModelPoints.Clear();
            RefreshPlot();
        }

        public void ReadFromFile(Stream file)
        {
            ClearAll();

            try
            {
            StreamReader reader = new StreamReader(file);

            while(!reader.EndOfStream)
            {
                String line = reader.ReadLine();

                String[] values = line.Split(';');

                if(!String.IsNullOrEmpty(values[0]) && !String.IsNullOrEmpty(values[1]))
                {
                    CVPoints.Add(DateTimeAxis.CreateDataPoint(DateTimeAxis.ToDateTime(Convert.ToDouble(values[0])), Convert.ToDouble(values[1])));
                }

                if(!String.IsNullOrEmpty(values[2]) && !String.IsNullOrEmpty(values[3]))
                {
                    PVPoints.Add(DateTimeAxis.CreateDataPoint(DateTimeAxis.ToDateTime(Convert.ToDouble(values[2])), Convert.ToDouble(values[3])));
                }
            }
                file.Close();
                reader.Close();
                RefreshPlot();

            }
            catch(Exception exception)
            {
                ClearAll();
                System.Windows.MessageBox.Show(exception.Message, "Cannot read from file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public DateTime StartModelDateTimePoint
        {
            get;
            private set;
        }

        public Double[][] GetDiscretePoints()
        {
            DateTime startDateTimePoint;
            PointApproximator pointApproximator = new PointApproximator(CVPoints, PVPoints, identificationControl.mainWindow.SampleTime);
            Double[][] points = pointApproximator.GetNormalizedCollections(out startDateTimePoint);
            StartModelDateTimePoint = startDateTimePoint;
            return points;
        }


        public StringBuilder PointsToString()
        {
            StringBuilder builder = new StringBuilder();

            long length = (new Int64[] { CVPoints.Count, PVPoints.Count, ModelPoints.Count }).Max();

            for(int i=0; i<length; i++)
            {
                if( i < CVPoints.Count )
                {
                    builder.Append(CVPoints[i].X.ToString("R") + ";" + CVPoints[i].Y.ToString("G6") + ";");
                }
                else
                {
                    builder.Append(";;");
                }

                if (i < PVPoints.Count)
                {
                    builder.Append(PVPoints[i].X.ToString("R") + ";" + PVPoints[i].Y.ToString("G6") + ";");
                }
                else
                {
                    builder.Append(";;");
                }

                if (i < ModelPoints.Count)
                {
                    builder.Append(ModelPoints[i].X.ToString("R") + ";" + ModelPoints[i].Y.ToString("G6") + ";");
                }
                else
                {
                    builder.Append(";;");
                }

                builder.Append("\n");
            }

            return builder;
        }

        public void ShowModelCollection(List<DataPoint> modelPoints)
        {
            ModelPoints.Clear();
            ModelPoints.AddRange(modelPoints);
            RefreshPlot();
        }

        public void HideModelCollection()
        {
            ModelPoints.Clear();
            RefreshPlot();
        }

    }

}
