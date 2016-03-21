using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DynamicMethodsLibrary
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MethodAttribute : Attribute
    {
        public string Name
        { 
            get; 
            private set; 
        }

        public BitmapImage Icon
        {
            get;
            private set;
        }

        private string iconPath;

        public MethodAttribute(string name, string  iconPath)
        {
            this.Name = name;
            this.iconPath = iconPath;
        }

        public void ResolveResource(Assembly assembly)
        {
            if (assembly != null && iconPath != null)
            {
                var stream = assembly.GetManifestResourceStream(string.Format("{0}.{1}",assembly.GetName().Name,this.iconPath));

                if (stream != null)
                {
                    Icon = new BitmapImage();
                    Icon.BeginInit();
                    Icon.StreamSource = stream;
                    Icon.EndInit();
                }
            }
        }

    }
}
