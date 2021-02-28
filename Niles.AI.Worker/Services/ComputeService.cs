using System;
using System.Collections.Generic;

namespace Niles.AI.Worker.Services
{
    public class ComputeService
    {
        ///<summary> Сигмоид </summary>
        public double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-1 * x));
        }

        ///<summary> Производная сигмоида </summary>
        public double SigmoidDerivative(double x)
        {
            return Sigmoid(x) * (1 - Sigmoid(x));
        }

        ///<summary> Гиперболический тангенс </summary>
        public double HyperbolicTangent(double x)
        {
            return (Math.Pow(Math.E, 2 * x) - 1) / (Math.Pow(Math.E, 2 * x) + 1);
        }

        ///<summary> Производная гиперболического тангенса </summary>
        public double HyperbolicTangentDerivative(double x)
        {
            return 1 - Math.Pow(x, 2);
        }

        ///<summary> Функция проверки процента ошибки по формуле MSE </summary>
        ///<param name="idealOutput"> Необходимый результат </param>
        ///<param name="actualOutput"> Фактический результат </param>
        public double MSEErrorRate(IReadOnlyList<double> idealOutput, IReadOnlyList<double> actualOutput)
        {
            double errorRate = 0.0;
            for (int i = 0; i < actualOutput.Count; i++)
            {
                var actual = actualOutput[i];
                var ideal = idealOutput[i];

                errorRate += Math.Pow(ideal - actual, 2);
            }

            return errorRate / actualOutput.Count;
        }

        ///<summary> Функция проверки процента ошибки по формуле Root MSE </summary>
        ///<param name="idealOutput"> Необходимый результат </param>
        ///<param name="actualOutput"> Фактический результат </param>
        public double RootMSEErrorRate(IReadOnlyList<double> idealOutput, IReadOnlyList<double> actualOutput)
        {
            double errorRate = 0.0;
            for (int i = 0; i < actualOutput.Count; i++)
            {
                var actual = actualOutput[i];
                var ideal = idealOutput[i];

                errorRate += Math.Pow(ideal - actual, 2);
            }

            return Math.Pow(errorRate / actualOutput.Count, .5);
        }

        ///<summary> Функция проверки процента ошибки по формуле Arctan </summary>
        ///<param name="idealOutput"> Необходимый результат </param>
        ///<param name="actualOutput"> Фактический результат </param>
        public double ArctanErrorRate(IReadOnlyList<double> idealOutput, IReadOnlyList<double> actualOutput)
        {
            double errorRate = 0.0;
            for (int i = 0; i < actualOutput.Count; i++)
            {
                var actual = actualOutput[i];
                var ideal = idealOutput[i];

                errorRate += Math.Pow(Math.Atan(ideal - actual), 2);
            }

            return errorRate / actualOutput.Count;
        }
    
        public double OutputDeltaForSigmoid(in double ideal, in double actual)
        {
            // delta = (ideal - actual) * df(x)
            // df(x) = (1 - actual) * actual
            return (ideal - actual) * SigmoidDerivative(actual);
        }

        public double OutputDeltaForHyperbolicTangent(in double ideal, in double actual)
        {
            // delta = (ideal - actual) * df(x)
            // df(x) = 1 - actual^2
            return (ideal - actual) * HyperbolicTangentDerivative(actual);
        }

        public double HiddenDeltaForSigmoid(in double output, IReadOnlyList<double> weights, IReadOnlyList<double> prevDeltas)
        {
            // delta = df(x) * E(wi * deltai)
            // df(x) = (1 - actual) * actual

            double sum = 0.0;
            for(int i = 0; i < weights.Count; i++)
                sum += weights[i] * prevDeltas[i];

            return (1 - output) * sum;
        }

        public double HiddenDeltaForHyperbolicTangent(in double output, IReadOnlyList<double> weights, IReadOnlyList<double> prevDeltas)
        {
            // delta = df(x) * E(wi * deltai)
            // df(x) = 1 - actual^2

            double sum = 0.0;
            for(int i = 0; i < weights.Count; i++)
                sum += weights[i] * prevDeltas[i];

            return (1 - Math.Pow(output, 2)) * sum;
        }

        public double GRADAB(double deltaB, double outA)
        {
            return deltaB * outA;
        }
    }
}