using System.Collections.Generic;
using System.Linq;
///<summary> Модель многослойной нейронной сети </summary>
public class NeuralNetwork
{
    ///<summary>Функция активации нейронной сети</summary>
    public ActivateFunctions ActivateFunction { get; set; } = ActivateFunctions.Sigmoid;
    
    ///<summary> Слои нейронной сети </summary>
    public List<NeuralLayer> Layers { get; set; } = new List<NeuralLayer>();

    ///<symmary> Выходные данные нейронной сети </summary>
    public IReadOnlyList<double> Output
    {
        get
        {
            if (Layers.Count == 0)
                return new List<double>();

            return Layers.LastOrDefault().Neurons.Select(o => o.Axon).ToList();
        }
    }

    public bool TrainEnded { get; set; } = false;
}

///<summary> Опции построения нейронной сети </summary>
public class NeuralNetworkBuildOptions
{
    ///<summary> Опции для построения слоев </summary>
    public IReadOnlyList<NeuralLayerBuildOptions> LayersBuildOptions { get; set; }

    ///<summary> Использовать нейроны смещения </summary>
    public bool UseBias { get; set; }

    ///<summary>Функция активации нейронной сети</summary>
    public ActivateFunctions ActivateFunction { get; set; } = ActivateFunctions.Sigmoid;
}

///<summary> Опции обучения нейронной сети </summary>
public class NeuralNetworkTrainOptions
{
    ///<summary> Количество эпох при обучении </summary>
    public int Epoch { get; set; } = 1;

    ///<summary> Скорость обучения </summary>
    public double LearningRate { get; set; } = 0.5;

    ///<summary> Момент </summary>
    public double Moment { get; set; } = 0.1;

    ///<summary> Наборы для обучения </summary>
    public List<TrainSet> TrainSets { get; set; }

    ///<summary> Функция активации </summary>
    public ActivateFunctions ActivateFunction { get; set; } = ActivateFunctions.Sigmoid;
}

///<summary> Опции активации нейронной сети </summary>
public class NeuralNetworkActivateOptions
{
    ///<symmary> Входные данные нейронной сети </summary>
    public IReadOnlyList<double> Input { get; set; } = new List<double>();

    ///<summary> Функция активации </summary>
    public ActivateFunctions ActivateFunction { get; set; } = ActivateFunctions.Sigmoid;
}

///<summary> Набор для обучения </summary>
public class TrainSet
{
    ///<summary> Входные данные </summary>
    public IReadOnlyList<double> Input { get; set; }

    ///<summary> Идеальный ответ </summary>
    public IReadOnlyList<double> IdealOutput { get; set; }

    ///<summary> Вероятность ошибки </summary>
    public double ErrorRate { get; set; } = 100;
}

///<summary> Варианты функций активации нейронной сети </summary>
public enum ActivateFunctions
{
    ///<summary> Сигмоида </summary>
    Sigmoid,

    ///<summary> Гиперболический тангенс </summary>
    HyperbolicTangent
}