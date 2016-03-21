using DynamicMethodsLibrary;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TransferFunctionLib;

namespace LSMethod
{
    public enum LSMethodType
    {
        Normal, Extended
    }

    public class LSFactory : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Mechanizm wiazania danych WPF
        /// </summary>
        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event Action<Double> AutoDelayCalculationPercentageChanged;
        public event Action<Double> IdentificationProgressChanged;

        public event Action IdentificationStarted;
        public event Action IdentificationStopped;

        public event Action CalculationAutoDelayStarted;
        public event Action CalculationAutoDelayStopped;

        private LSMethodType typeOfMethod;
        public LSMethodType TypeOfMethod
        {
            get
            {
                return typeOfMethod;
            }

            set
            {
                typeOfMethod = value;
                NotifyPropertyChanged("TypeOfMethod");
            }
        }

        private PointCollection uPoints = new PointCollection();
        private PointCollection yPoints = new PointCollection();

        private Boolean onIdentification = false;

        private Int32 uRank;
        public Int32 URank
        {
            get
            {
                return uRank;
            }

            set
            {
                uRank = value;
                CheckRanks();
                NotifyPropertyChanged("URank");
            }
        }

        private Int32 yRank;
        public Int32 YRank
        {
            get
            {
                return yRank;
            }

            set
            {
                yRank = value;
                CheckRanks();
                NotifyPropertyChanged("YRank");
            }
        }

        public void CheckRanks()
        {
            if (URank > YRank)
            {
                YRank = URank;
            }
        }

        private Int32 discreteDelay;
        public Int32 DiscreteDelay
        {
            get
            {
                return discreteDelay;
            }

            set
            {
                discreteDelay = value;
                NotifyPropertyChanged("DiscreteDelay");
            }
        }

        private Double sampleTime;
        public Double SampleTime
        {
            get
            {
                return sampleTime;
            }

            set
            {
                sampleTime = value;
                NotifyPropertyChanged("SampleTime");
            }
        }

        private Int32 maxIterationNumber = 10;
        public Int32 MaxIterationNumber
        {
            get
            {
                return maxIterationNumber;
            }

            set
            {
                maxIterationNumber = value;
                NotifyPropertyChanged("MaxIterationNumber");
            }
        }

        private Int32 startKForAutoTimeDelay = 0;
        public Int32 StartKForAutoTimeDelay
        {
            get
            {
                return startKForAutoTimeDelay;
            }

            set
            {
                startKForAutoTimeDelay = value;
                NotifyPropertyChanged("StartKForAutoTimeDelay");
            }
        }

        private Int32 endKForAutoTimeDelay = 10;
        public Int32 EndKForAutoTimeDelay
        {
            get
            {
                return endKForAutoTimeDelay;
            }

            set
            {
                endKForAutoTimeDelay = value;
                NotifyPropertyChanged("EndKForAutoTimeDelay");
            }
        }

        private Boolean autoTimeDelay;
        public Boolean AutoTimeDelay
        {
            get
            {
                return autoTimeDelay;
            }

            set
            {
                autoTimeDelay = value;
                NotifyPropertyChanged("AutoTimeDelay");
            }
        }

        public TransferFunctionClass GetTransferFunctiomFactors()
        {
            if (IdentificationStarted != null)
            {
                IdentificationStarted();
            }

            if (IdentificationProgressChanged != null)
            {
                IdentificationProgressChanged(0);
            }

            if (AutoTimeDelay)
            {
                CalculateAutoDiscreteTime();

            }
            onIdentification = true;

            if (IdentificationProgressChanged != null)
            {
                IdentificationProgressChanged(50);
            }
            Matrix<Double> calculatedFactors = null;

            if (TypeOfMethod == LSMethodType.Normal)
            {
                calculatedFactors = GetLSFactors();

            }
            else if (TypeOfMethod == LSMethodType.Extended)
            {
                calculatedFactors = GetELSFactors();
            }
            if (IdentificationProgressChanged != null)
            {
                IdentificationProgressChanged(90);
            }
            Double[] nomFactors = GetNominatorFactors(calculatedFactors);
            Double[] denFactors = GetDenominatorFactors(calculatedFactors);

            if (IdentificationProgressChanged != null)
            {
                IdentificationProgressChanged(100);
            }
            onIdentification = false;

            if (IdentificationStopped != null)
            {
                IdentificationStopped();
            }
            return new TransferFunctionClass(nomFactors, denFactors, Convert.ToDouble(DiscreteDelay), Convert.ToInt32(SampleTime), TransferFunctionType.Discrete);

        }

        private void CalculateAutoDiscreteTime()
        {
            if (CalculationAutoDelayStarted != null)
            {
                CalculationAutoDelayStarted();
            }


            if (AutoDelayCalculationPercentageChanged != null)
            {
                AutoDelayCalculationPercentageChanged(0);
            }

            DiscreteDelay = StartKForAutoTimeDelay;

            Double[] errors = new Double[EndKForAutoTimeDelay - StartKForAutoTimeDelay];

            Matrix<Double> calculatedFactors = null;

            if (TypeOfMethod == LSMethodType.Normal)
            {
                calculatedFactors = GetLSFactors();
            }
            else if (TypeOfMethod == LSMethodType.Extended)
            {
                calculatedFactors = GetELSFactors();
            }

            Matrix<Double> Y = GetY();

            errors[0] = GetModelError(Y, calculatedFactors);


            if (AutoDelayCalculationPercentageChanged != null)
            {
                AutoDelayCalculationPercentageChanged(10);
            }

            for (int i = 1; i < errors.Length; i++)
            {

                DiscreteDelay++;

                if (TypeOfMethod == LSMethodType.Normal)
                {
                    calculatedFactors = GetLSFactors();
                }
                else if (TypeOfMethod == LSMethodType.Extended)
                {
                    calculatedFactors = GetELSFactors();
                }

                errors[i] = GetModelError(Y, calculatedFactors); ;

                if (AutoDelayCalculationPercentageChanged != null)
                {
                    AutoDelayCalculationPercentageChanged(10 + 80 * Convert.ToDouble(i) / Convert.ToDouble(errors.Length));
                }
            }

            Double minError = errors.Min();

            if (AutoDelayCalculationPercentageChanged != null)
            {
                AutoDelayCalculationPercentageChanged(100);
            }

            DiscreteDelay = StartKForAutoTimeDelay + Array.IndexOf(errors, minError);

            if (CalculationAutoDelayStopped != null)
            {
                CalculationAutoDelayStopped();
            }
        }

        private Double[] GetNominatorFactors(Matrix<Double> Factors)
        {
            Double[] factors = new Double[uRank];

            for (int i = 0; i < uRank; i++)
            {
                factors[i] = Factors[i + yRank - 1, 0];
            }

            return factors;
        }

        private Double[] GetDenominatorFactors(Matrix<Double> Factors)
        {
            Double[] factors = new Double[yRank];

            factors[0] = 1;

            for (int i = 0; i < yRank - 1; i++)
            {
                factors[i + 1] = Factors[i, 0];
            }

            return factors;
        }

        public LSFactory(Int32 uRank, Int32 yRank, Double[] uPoints, Double[] yPoints, Int32 discreteDelay, Double sampleTime)
        {
            this.URank = uRank;
            this.YRank = yRank;
            this.DiscreteDelay = discreteDelay;
            this.SampleTime = sampleTime;

            AssignPoints(uPoints, yPoints);
        }

        public void AssignPoints(Double[] CV, Double[] PV)
        {
            this.uPoints.AssignArray(CV);
            this.yPoints.AssignArray(PV);
        }

        private Vector<Double> GetVForK(int k, Matrix<Double> ErrorMatrix)
        {
            var vector = Vector<Double>.Build.Dense(uRank + yRank + yRank - 2);

            int vectorNumber = 0;

            for (int i = k - 1; i > k - yRank; i--)
            {
                vector[vectorNumber] = -yPoints[i];
                vectorNumber++;
            }

            for (int i = k - discreteDelay; i > k - discreteDelay - uRank; i--)
            {
                vector[vectorNumber] = uPoints[i];
                vectorNumber++;

            }

            for (int i = k - 1; i > k - yRank; i--)
            {
                if (i < 0)
                {
                    vector[vectorNumber] = ErrorMatrix[0, 0];
                }
                else
                {
                    vector[vectorNumber] = ErrorMatrix[i, 0];
                }

                vectorNumber++;
            }

            return vector;
        }

        private Matrix<Double> GetV(Matrix<Double> ErrorMatrix)
        {
            var V = Matrix<Double>.Build.Dense(uPoints.Count(), uRank + yRank + yRank - 2);

            for (int i = 0; i < uPoints.Count(); i++)
            {
                V.SetRow(i, GetVForK(i, ErrorMatrix));
            }

            return V;
        }

        private Vector<Double> GetV0ForK(int k)
        {
            var vector = Vector<Double>.Build.Dense(uRank + yRank - 1);

            int vectorNumber = 0;

            for (int i = k - 1; i > k - yRank; i--)
            {
                vector[vectorNumber] = -yPoints[i];
                vectorNumber++;
            }

            for (int i = k - discreteDelay; i > k - discreteDelay - uRank; i--)
            {
                vector[vectorNumber] = uPoints[i];
                vectorNumber++;

            }

            return vector;
        }

        private Double GetModelError(Matrix<Double> Y, Matrix<Double> Factors)
        {
            Matrix<Double> ErrorMatrix = GetModelErrorMatrix(Y, Factors);

            return ErrorMatrix.Column(0).Sum(new Func<Double, Double>((error) => { return error * error; }));
        }


        private Matrix<Double> GetV0()
        {
            var V = Matrix<Double>.Build.Dense(uPoints.Count(), uRank + yRank - 1);

            for (int i = 0; i < uPoints.Count(); i++)
            {
                V.SetRow(i, GetV0ForK(i));
            }

            return V;
        }

        private Matrix<Double> GetY()
        {
            var Y = Matrix<Double>.Build.Dense(yPoints.Count(), 1);

            Y.SetColumn(0, yPoints.ToArray());

            return Y;
        }

        private Matrix<Double> LSMethod(Matrix<Double> V, Matrix<Double> Y)
        {
            return (((V.TransposeThisAndMultiply(V)).Inverse()).Multiply(V.Transpose())).Multiply(Y);
        }

        private Matrix<Double> GetLSFactors()
        {
            Matrix<Double> Y = GetY();
            Matrix<Double> VK1 = GetV0();

            return LSMethod(VK1, Y);
        }

        public Matrix<Double> GetELSFactors()
        {
            Matrix<Double> Y = GetY();

            Matrix<Double> VK1 = GetV0();
            Matrix<Double> ELSFactorsK1 = LSMethod(VK1, Y);
            Matrix<Double> ErrorMatrixK1 = GetErrorMatrix(VK1, Y, ELSFactorsK1);
            Double VarianceK1 = GetVariance(ErrorMatrixK1);

            Matrix<Double> VK2 = GetV(ErrorMatrixK1);
            Matrix<Double> ELSFactorsK2 = LSMethod(VK2, Y);
            Matrix<Double> ErrorMatrixK2 = GetErrorMatrix(VK2, Y, ELSFactorsK2);
            Double VarianceK2 = GetVariance(ErrorMatrixK2);

            for (int i = 0; (i < maxIterationNumber) || (VarianceK2 > VarianceK1); i++)
            {
                VK1 = VK2;
                ELSFactorsK1 = ELSFactorsK2;
                ErrorMatrixK1 = ErrorMatrixK2;
                VarianceK1 = VarianceK2;

                VK2 = GetV(ErrorMatrixK1);
                ELSFactorsK2 = LSMethod(VK2, Y);
                ErrorMatrixK2 = GetErrorMatrix(VK2, Y, ELSFactorsK2);
                VarianceK2 = GetVariance(ErrorMatrixK2);

                if (onIdentification)
                {
                    if (IdentificationProgressChanged != null)
                    {
                        IdentificationProgressChanged(50 + 40 * Convert.ToDouble(i) / Convert.ToDouble(maxIterationNumber));
                    }
                }
            }


            return ELSFactorsK1;

        }

        public Matrix<Double> GetModelErrorMatrix(Matrix<Double> Y, Matrix<Double> LSFactors)
        {
            var errors = Matrix<Double>.Build.Dense(yPoints.Count(), 1);
            var modelOutputs = Matrix<Double>.Build.Dense(yPoints.Count(), 1);
            var modelVector = Matrix<Double>.Build.Dense(1, uRank + yRank - 1);
            var modelFactors = Matrix<Double>.Build.Dense(uRank + yRank - 1, 1 );

            for( int i=0; i< modelFactors.RowCount; i++)
            {
                modelFactors[i, 0] = LSFactors[i, 0];
            }

            Int32 vectorNumber = 0;

            for (int j = 0; j < yPoints.Count(); j++)
            {

                vectorNumber = 0;

                for (int i = j - 1; i > j - yRank; i--)
                {

                    if (i <= 0)
                    {
                        modelVector[0, vectorNumber] = -yPoints[0];
                    }
                    else
                    {
                        modelVector[0, vectorNumber] = -modelOutputs[i, 0];
                    }

                    vectorNumber++;

                }

                for (int i = j - discreteDelay; i > j - discreteDelay - uRank; i--)
                {

                    modelVector[0, vectorNumber] = uPoints[i];

                    vectorNumber++;

                }



                modelOutputs[j, 0] = (modelVector.Multiply(modelFactors))[0, 0];

                errors[j, 0] = (modelOutputs[j, 0] - Y[j, 0]);
            }


            return errors;
        }

        private Matrix<Double> GetErrorMatrix(Matrix<Double> V, Matrix<Double> Y, Matrix<Double> LSFactors)
        {
            return Y.Subtract(V.Multiply(LSFactors));
        }

        private Double GetVariance(Matrix<Double> ErrorMatrix)
        {
            return (from error in ErrorMatrix.Column(0)
                    select error * error).Sum();
        }

    }
}
