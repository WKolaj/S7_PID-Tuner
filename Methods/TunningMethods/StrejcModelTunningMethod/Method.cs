using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StrejcModelTunningMethod
{
    [Method("Methods for Strejsc model", "Strejc.png")]
    public class Method : TuningMethod
    {
        public override PIDControllerClass Tuning()
        {
            if (!CheckTransferFunction())
            {
                System.Windows.MessageBox.Show("Given transfer function does not corespond to this method", "Wrong transfer function", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            SettingsWindow window = new SettingsWindow();
            window.AssignPlantObject(TransferFunction);

            bool? result = window.ShowDialog();

            if ((bool)result)
            {
                return window.PIDController;
            }
            else
            {
                return null;
            }
        }

        public bool CheckTransferFunction()
        {
            if (TuningMethod.LengthOf(TransferFunction.Nominator)!= 1 || TransferFunction.TimeDelay <= 0)
            {
                return false;
            }

            RootFinder rootFinder = new RootFinder(TransferFunction.Denominator,0.001);

            if(rootFinder.GetRealRoots().Length != TuningMethod.LengthOf(TransferFunction.Denominator) - 1)
            {
                return false;
            }
            
            return true;
        }
    }
}
