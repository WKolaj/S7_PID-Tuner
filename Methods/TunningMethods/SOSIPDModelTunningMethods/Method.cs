using DynamicMethodsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SOSIPDModelTunningMethod
{
    [Method("Methods for second order system integral plus time delay model", "SOSIPD.png")]
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
            if (TuningMethod.LengthOf(TransferFunction.Nominator) != 1 || TuningMethod.LengthOf(TransferFunction.Denominator) != 4 || TransferFunction.TimeDelay <= 0)
            {
                return false;
            }
            else if (Math.Abs(Math.Max(TransferFunction.Denominator[2], TransferFunction.Denominator[1])) / 1000 < Math.Abs(TransferFunction.Denominator[0]))
            {
                return false;
            }
            else if (!HasPlantObjectRoots(TransferFunction))
            {
                return false;
            }

            return true;
        }

        private Boolean HasPlantObjectRoots(TransferFunctionClass plantObject)
        {
            Double a = plantObject.Denominator[3];
            Double b = plantObject.Denominator[2];
            Double c = plantObject.Denominator[1];

            Double delta = b * b - 4 * a * c;

            if (delta < 0)
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
