using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicMethodsLibrary;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace DelayModelTunningMethods
{
    [Method("Methods for delay model","FirstRankTimeDelay.png")]
    public class Method:TuningMethod
    {
        public override PIDControllerClass Tuning()
        {
            if(!CheckTransferFunction())
            {
                System.Windows.MessageBox.Show("Given transfer function does not corespond to this method", "Wrong transfer function", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            SettingsWindow window = new SettingsWindow();
            window.AssingPlantObject(TransferFunction);

            bool? result = window.ShowDialog();

            if((bool)result)
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
            if (TuningMethod.LengthOf(TransferFunction.Nominator) != 1 || TuningMethod.LengthOf(TransferFunction.Denominator) != 1 || TransferFunction.TimeDelay <= 0)
            {
                return false;
            }

            else
            {
                return true;
            }
        }
    }
}
