using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TOSPDWithNomModelTunningMethods
{
    [Method("Methods for third order system with nominator plus time delay model", "TOSPDWithNom.png")]
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
            if (TuningMethod.LengthOf(TransferFunction.Nominator) != 4 || TuningMethod.LengthOf(TransferFunction.Denominator) != 4 || TransferFunction.TimeDelay <= 0)
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
