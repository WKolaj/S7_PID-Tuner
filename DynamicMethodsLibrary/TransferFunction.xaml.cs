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
using TransferFunctionLib;

namespace DynamicMethodsLibrary
{
    /// <summary>
    /// Kontrolka ukladu dynamicznego
    /// </summary>
    public partial class TransferFunction : UserControl
    {
        private DynamicSystem dynamicSystem;
        public DynamicSystem DynamicSystemObject
        {
            get
            {
                return dynamicSystem;
            }

            set
            {
                dynamicSystem = value;
                RefreshTransferFunctionDisplaying();
            }
        }

        private SystemType type;
        public SystemType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
                RefreshTransferFunctionDisplaying();
            }
        }

        /// <summary>
        /// Konstruktor kontrolki transmitancji
        /// </summary>
        public TransferFunction()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Metoda wyczyszczajaca wyswietlanie transmitancji
        /// </summary>
        void ClearTransferFunction()
        {
            if(Type == SystemType.Continues)
            {
                nominatorFactorsLabel.Content = "Nom(s)";
                denominatorFactorsLabel.Content = "Den(s)";
                timeDelayLabel.Content = "e⁻ᵀˢ";
            }
            else if(Type == SystemType.Discrete)
            {
                nominatorFactorsLabel.Content = "Nom(z)";
                denominatorFactorsLabel.Content = "Den(z)";
                timeDelayLabel.Content = "z⁻ᵀ";
            }
        }


        /// <summary>
        /// Odswiezenie wsywietlanych wspolczynnikow
        /// </summary>
        public void RefreshTransferFunctionDisplaying()
        {
            if (DynamicSystemObject == null)
            {
                ClearTransferFunction();
                return;
            }

            //Nastepnie nalezy sprawdzic, czy aktualny obiekt jest ciagly czy dyskretny i przypisac wektory transmitancji jego licznika i mianownika do wyswietlania
            if (Type == SystemType.Continues)
            {
                nominatorFactorsLabel.Content = DynamicSystemObject.ContinousNominatorString;
                denominatorFactorsLabel.Content = DynamicSystemObject.ContinousDenominatorString;
                timeDelayLabel.Content = DynamicSystemObject.ContinousTimeDelayString;
            }
            else if (Type == SystemType.Discrete)
            {
                nominatorFactorsLabel.Content = DynamicSystemObject.DiscreteNominatorString;
                denominatorFactorsLabel.Content = DynamicSystemObject.DiscreteDenominatorString;
                timeDelayLabel.Content = DynamicSystemObject.DiscreteTimeDelayString;
            }
        }

        /// <summary>
        /// Metoda zmieniajaca kolor tła transmitancji - sluzace do zaznaczania
        /// </summary>
        /// <param name="colorCode">
        /// lancuch znakow reprezentujacy wartosci koloru w kodzie szesnastkowym
        /// </param>
        public void ChangeColor(String colorCode)
        {   
            MainBorder.Background = (Brush)(new BrushConverter()).ConvertFrom(colorCode);
        }


        public void Normalize()
        {
            if(dynamicSystem != null)
            {
                if (type == SystemType.Continues)
                {
                    dynamicSystem.NormalizeContinous();
                }
                else if (type == SystemType.Discrete)
                {
                    dynamicSystem.NormalizeDiscrete();
                }

                RefreshTransferFunctionDisplaying();
            }
        }

    }
}
