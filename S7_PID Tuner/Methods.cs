using DynamicMethodsLibrary;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using TransferFunctionLib;

namespace S7_PID_Tuner
{
    public abstract class Method
    {

        protected Assembly assembly;

        protected Type methodClassType;

        public String AssemblyPath
        {
            get;
            set;
        }

        public String Name
        {
            get;
            set;
        }

        public BitmapImage Icon
        {
            get;
            set;
        }

        public abstract void Init(String assemblyPath);


        public Method()
        {
        }

        public static string GetFullPath(String path)
        {
            String newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path);
            return newPath;
        }

        public static Assembly LoadWithoutCache(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var rawAssembly = new byte[fs.Length];
                fs.Read(rawAssembly, 0, rawAssembly.Length);
                return Assembly.Load(rawAssembly);
            }
        }
    }

    public class IdentificationMethod : Method
    {
        DynamicMethodsLibrary.IdentificationWindow identificationWindowMethod;

        PIDControllerDevice controllerDevice;

        public override void Init(String AssemblyPath)
        {
           assembly = Method.LoadWithoutCache(Method.GetFullPath(AssemblyPath));

            methodClassType = assembly.GetTypes().Single(t => t.Name == "MainWindow");

            MethodAttribute attribute = (MethodAttribute) methodClassType.GetCustomAttribute(typeof(MethodAttribute));

            attribute.ResolveResource(assembly);

            Name = attribute.Name;

            Icon = attribute.Icon;

            this.AssemblyPath = AssemblyPath;
        }

        public IdentificationMethod(String AssemblyPath)
        {
            Init(AssemblyPath);
        }

        public DynamicSystem Start( PIDControllerDevice controllerDevice, out SystemType typeOfDS )
        {
            this.controllerDevice = controllerDevice;

            ConnectionWindow connectionWindow = new ConnectionWindow();
            connectionWindow.InitDevice( controllerDevice );
            
            if((bool)connectionWindow.ShowDialog())
            {
                DynamicSystem ds = StartMethod(out typeOfDS);
                
                if(controllerDevice.Connected)
                {
                    try
                    {
                        controllerDevice.TurnOffTuningMode();
                    }
                    catch
                    {

                    }

                    controllerDevice.Disconnect();
                }

                return ds;
            }
            else
            {
                typeOfDS = SystemType.Continues;
                return null;
            }
        }

        public void OnTuningModeChange()
        {
            if(controllerDevice != null)
            {
                if(!controllerDevice.TuningMode)
                {
                    identificationWindowMethod.StopIdentification("Tunning mode was set to false");
                }
            }
        }

        private void OnPVUpdated()
        {
            UpdatePV(controllerDevice.ProcessValue);
        }

        public void OnCvUpdated()
        {
            UpdateCV(controllerDevice.ManualValue);
        }

        public void OnSampleTimeUpdated()
        {
            UpdateSampleTime(controllerDevice.IdentificationSampleTime);
        }

        public void MethodSettingCV(Double newValue)
        {
            if(controllerDevice != null)
            {
                controllerDevice.ManualValue = newValue;
            }
        }

        public void MethodSettingSampleTime(Int32 newValue)
        {
            if (controllerDevice != null)
            {
                controllerDevice.IdentificationSampleTime = newValue;
            }
        }

        public void ConnectMethodToControllerEvents()
        {
            identificationWindowMethod.SetCVOutside = MethodSettingCV;
            identificationWindowMethod.SetSampleTimeOutside = MethodSettingSampleTime;

            controllerDevice.TuningModeStateChanged += OnTuningModeChange;
            controllerDevice.ProcessValueUpdated += OnPVUpdated;
            controllerDevice.PIDSampleTimeUpdated += OnSampleTimeUpdated;
            controllerDevice.ManualOutputUpdated += OnCvUpdated;
        }

        public void DisconnectMethodToControllerEvents()
        {
            identificationWindowMethod.SetCVOutside = null;
            identificationWindowMethod.SetSampleTimeOutside = null;

            controllerDevice.TuningModeStateChanged -= OnTuningModeChange;
            controllerDevice.ProcessValueUpdated -= OnPVUpdated;
            controllerDevice.PIDSampleTimeUpdated -= OnSampleTimeUpdated;
            controllerDevice.ManualOutputUpdated -= OnCvUpdated;
        }

        private DynamicSystem StartMethod( out SystemType type)
        {
            assembly = Method.LoadWithoutCache(Method.GetFullPath(AssemblyPath));

            Type windowType = assembly.GetTypes().Single(t => t.Name == "MainWindow");

            identificationWindowMethod = (DynamicMethodsLibrary.IdentificationWindow)Activator.CreateInstance(windowType);

            ConnectMethodToControllerEvents();

            identificationWindowMethod.SetSampleTimeFromOutside(controllerDevice.IdentificationSampleTime);

            identificationWindowMethod.Icon = Icon;

            bool? result = identificationWindowMethod.ShowDialog();

            DisconnectMethodToControllerEvents();

            if((bool) result)
            {
                if(identificationWindowMethod.isDiscrete)
                {
                    type = SystemType.Discrete;
                    return DynamicSystem.FromDiscreteTransferFuntion(identificationWindowMethod.nominator, identificationWindowMethod.denominator, Convert.ToInt32(identificationWindowMethod.timeDelay), Convert.ToDouble(identificationWindowMethod.SampleTime) / 1000);
                }
                else
                {
                    type = SystemType.Continues;
                    return DynamicSystem.FromContinousTransferFuntion(identificationWindowMethod.nominator, identificationWindowMethod.denominator, identificationWindowMethod.timeDelay, Convert.ToDouble(identificationWindowMethod.simulationSampleTime) / 1000);
                }
                                
            }
            else
            {
                type = SystemType.Continues;
                return null;
            }
        }

        public void StopMethod(String cause)
        {
            if (identificationWindowMethod != null)
            {
                identificationWindowMethod.StopIdentification(cause);
            }
        }

        public void UpdatePV(Double newPVValue)
        {
            if (identificationWindowMethod != null)
            {
                identificationWindowMethod.SetPVFromOutside(newPVValue);
            }
        }

        public void UpdateSampleTime(Int32 newSampleTime)
        {
            if (identificationWindowMethod != null)
            {
                identificationWindowMethod.SetSampleTimeFromOutside(newSampleTime);
            }
        }

        public void UpdateCV(Double newCV)
        {
            if (identificationWindowMethod != null)
            {
                identificationWindowMethod.SetCVFromOutside(newCV);
            }
        }

        public static bool CheckAssemlby(String path)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(Method.GetFullPath(path));

                Type classType = assembly.GetTypes().Single(t => t.Name == "MainWindow");

                if (!typeof(DynamicMethodsLibrary.IdentificationWindow).IsAssignableFrom(classType))
                {
                    throw new Exception("MainWindow class in assembly does not inherite from IndetificationWindow");
                }

                MethodAttribute attribute = (MethodAttribute)classType.GetCustomAttribute(typeof(MethodAttribute));

                attribute.ResolveResource(assembly);

                assembly = null;

                return true;
            }
            catch
            {
                return false;
            }
    
        }


    }

    public class IdentificationMethodCollection
    {
        private List<Method> methodList = new List<Method>();
        public List<Method> MethodList
        {
            get
            {
                return methodList;
            }

            private set
            {
                methodList = value;
            }
        }

        public XDocument ToXML()
        {
            XDocument doc = new XDocument();

            //String identMethodsPath = System.IO.Path.Combine(System.IO.Path.c)
            doc.Add(new XElement("IdentificationMethods", from method in MethodList
                                             select new XElement("IdentificationMethod", method.AssemblyPath)));

            return doc;
        }

        public void FromXML(XDocument xmlDocument)
        {
            XElement rootElement = xmlDocument.Root;
            
            if(rootElement == null)
            {
                throw new InvalidOperationException("Wrong file ");
            }

            if(rootElement.Name != "IdentificationMethods")
            {
                throw new InvalidOperationException("Wrong root name element - root name is "+rootElement.Name);
            }

            methodList.Clear();

            List<String> assemblyPaths = (from path in rootElement.Elements("IdentificationMethod")
                                         select path.Value).ToList();

            foreach(string path in assemblyPaths)
            {
                try
                {
                    MethodList.Add(new IdentificationMethod(path));
                }
                catch
                {
                }
                    
            }

        }

    }

    public class TuningMethod : Method
    {
        DynamicMethodsLibrary.TuningMethod tuningMethod;

        Project currentProject;

        public override void Init(String AssemblyPath)
        {
            assembly = Method.LoadWithoutCache(Method.GetFullPath(AssemblyPath));

            methodClassType = assembly.GetTypes().Single(t => t.Name == "Method");

            MethodAttribute attribute = (MethodAttribute)methodClassType.GetCustomAttribute(typeof(MethodAttribute));

            attribute.ResolveResource(assembly);

            Name = attribute.Name;

            Icon = attribute.Icon;

            this.AssemblyPath = AssemblyPath;
        }

        public TuningMethod(String AssemblyPath)
        {
            Init(AssemblyPath);
        }

        public void AssignProject(Project currentProject)
        {
            this.currentProject = currentProject;
        }

        public PIDController Start()
        {
            if (currentProject != null)
            {
                if (currentProject.PlantObject != null)
                {
                    assembly = Method.LoadWithoutCache(Method.GetFullPath(AssemblyPath));

                    Type methodType = assembly.GetTypes().Single(t => t.Name == "Method");

                    tuningMethod = (DynamicMethodsLibrary.TuningMethod)Activator.CreateInstance(methodType);

                    try
                    {
                        return tuningMethod.StartTuning(new TransferFunctionClass(currentProject.PlantObject.ContinousNumerator, currentProject.PlantObject.ContinousDenumerator, currentProject.PlantObject.ContinousTimeDelay, Convert.ToInt32(currentProject.PlantObject.SimulationSampleTime * 1000), TransferFunctionType.Continous));
                    }
                    catch (Exception exception)
                    {
                        System.Windows.MessageBox.Show(exception.Message, "Error in tuning method", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            return null;
        }

        public static bool CheckAssemlby(String path)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(Method.GetFullPath(path));

                Type classType = assembly.GetTypes().Single(t => t.Name == "Method");

                if (!typeof(DynamicMethodsLibrary.TuningMethod).IsAssignableFrom(classType))
                {
                    throw new Exception("MainWindow class in assembly does not inherite from IndetificationWindow");
                }

                MethodAttribute attribute = (MethodAttribute)classType.GetCustomAttribute(typeof(MethodAttribute));

                attribute.ResolveResource(assembly);

                assembly = null;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    

    public class TuningMethodCollection
    {
        private List<TuningMethod> methodList = new List<TuningMethod>();
        public List<TuningMethod> MethodList
        {
            get
            {
                return methodList;
            }

            private set
            {
                methodList = value;
            }
        }

        public XDocument ToXML()
        {
            XDocument doc = new XDocument();

            //String identMethodsPath = System.IO.Path.Combine(System.IO.Path.c)
            doc.Add(new XElement("TuningMethods", from method in MethodList
                                                  select new XElement("TuningMethod", method.AssemblyPath)));

            return doc;
        }

        public void FromXML(XDocument xmlDocument)
        {
            XElement rootElement = xmlDocument.Root;

            if (rootElement == null)
            {
                throw new InvalidOperationException("Wrong file ");
            }

            if (rootElement.Name != "TuningMethods")
            {
                throw new InvalidOperationException("Wrong root name element - root name is " + rootElement.Name);
            }

            methodList.Clear();

            List<String> assemblyPaths = (from path in rootElement.Elements("TuningMethod")
                                          select path.Value).ToList();

            foreach (string path in assemblyPaths)
            {
                try
                {
                    MethodList.Add(new TuningMethod(path));
                }
                catch
                {
                    
                }

            }

        }



        public TuningMethodCollection()
        {
        }

        public void AssignProject(Project currentProject)
        {
            foreach (var method in MethodList)
            {
                method.AssignProject(currentProject);
            }
        }
    }

    public class PerformanceIndex : Method,INotifyPropertyChanged
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

        DynamicMethodsLibrary.PerfomanceIndexMethod perfomanceIndexMethod;

        public override void Init(String AssemblyPath)
        {
            assembly = Method.LoadWithoutCache(Method.GetFullPath(AssemblyPath));

            methodClassType = assembly.GetTypes().Single(t => t.Name == "CustomPerformanceIndex");

            MethodAttribute attribute = (MethodAttribute)methodClassType.GetCustomAttribute(typeof(MethodAttribute));

            this.perfomanceIndexMethod = (DynamicMethodsLibrary.PerfomanceIndexMethod)Activator.CreateInstance(methodClassType);

            Name = attribute.Name;

            Icon = null;

            this.AssemblyPath = AssemblyPath;
        }

        private Double value;
        public Double Value
        {
            get
            {
                return value;
            }

            private set
            {
                this.value = value;
                NotifyPropertyChanged("Value");
            }
        }

        public PerformanceIndex(String AssemblyPath)
        {
            Init(AssemblyPath);
        }

        public void RefreshValue(Double[] pv, Double[] sp,Int32 SampleTime)
        {
            if (perfomanceIndexMethod != null)
            {
                this.Value = perfomanceIndexMethod.GetValue(pv, sp,SampleTime);
            }
        }

        public static bool CheckAssemlby(String path)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(Method.GetFullPath(path));

                Type classType = assembly.GetTypes().Single(t => t.Name == "CustomPerformanceIndex");

                if (!typeof(DynamicMethodsLibrary.PerfomanceIndexMethod).IsAssignableFrom(classType))
                {
                    throw new Exception("CustomPerformanceIndex index class in assembly does not inherite from PerformanceIndexMethod");
                }

                MethodAttribute attribute = (MethodAttribute)classType.GetCustomAttribute(typeof(MethodAttribute));

                assembly = null;

                return true;
            }
            catch
            {
                return false;
            }

        }
    }


    public class PerformanceIndexCollection
    {
        private List<PerformanceIndex> performanceIndexList = new List<PerformanceIndex>();
        public List<PerformanceIndex> PerformanceIndexList
        {
            get
            {
                return performanceIndexList;
            }

            private set
            {
                performanceIndexList = value;
            }
        }

        public XDocument ToXML()
        {
            XDocument doc = new XDocument();

            //String identMethodsPath = System.IO.Path.Combine(System.IO.Path.c)
            doc.Add(new XElement("PerfomanceIndexes", from index in PerformanceIndexList
                                                  select new XElement("PerformanceIndex", index.AssemblyPath)));

            return doc;
        }

        public void FromXML(XDocument xmlDocument)
        {
            XElement rootElement = xmlDocument.Root;

            if (rootElement == null)
            {
                throw new InvalidOperationException("Wrong file ");
            }

            if (rootElement.Name != "PerfomanceIndexes")
            {
                throw new InvalidOperationException("Wrong root name element - root name is " + rootElement.Name);
            }

            PerformanceIndexList.Clear();

            List<String> assemblyPaths = (from path in rootElement.Elements("PerformanceIndex")
                                          select path.Value).ToList();

            foreach (string path in assemblyPaths)
            {
                try
                {
                    PerformanceIndexList.Add(new PerformanceIndex(path));
                }
                catch
                {

                }

            }

        }


    }
}
