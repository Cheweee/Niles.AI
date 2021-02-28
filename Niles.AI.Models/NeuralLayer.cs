using System;
using System.Collections.Generic;

///<summary> Модель слой нейронов в сети </summary>
public class NeuralLayer
{
    public int Id { get; set; }
    ///<summary> Название слоя </summary>
    public string Name { get; set; }
    ///<summary> Нейроны в слое </summary>
    public List<Neuron> Neurons { get; set; } = new List<Neuron>();
}

///<summary> Опции построения нейронного слоя </summary>
public class NeuralLayerBuildOptions
{
    ///<summary> Название слоя </summary>
    public string Name { get; set; }
    ///<summary> Количество нейронов в слое </summary>
    public int NeuronsCount { get; set; } = 1;
}