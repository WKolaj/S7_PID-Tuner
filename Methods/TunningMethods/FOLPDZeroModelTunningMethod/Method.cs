using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FOLPDZeroModelTunningMethod
{
    [Method("Methods for FOLPD with zero model", "FOLPDZero.png")]
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
            window.AssingPlantObject(TransferFunction);

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
            if (TuningMethod.LengthOf(TransferFunction.Nominator) != 2 || TuningMethod.LengthOf(TransferFunction.Denominator) != 2 || TransferFunction.TimeDelay <= 0)
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
