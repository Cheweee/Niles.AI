using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Niles.AI.Services.Interfaces;

namespace Niles.AI.Worker.Services
{
    public class NeuralNetworkService : INeuralNetworkService
    {
        private readonly ComputeService _computeService;
        private readonly ILogger<NeuralNetworkService> _logger;

        public NeuralNetwork Instance { get; } = new NeuralNetwork();

        public NeuralNetworkService(
            ComputeService computeService,
            ILogger<NeuralNetworkService> logger
        )
        {
            _computeService = computeService ?? throw new ArgumentNullException(nameof(computeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        ///<summary> Инициализирует нейронную сеть </summary>
        public void InitializeNetworkInstance(NeuralNetworkBuildOptions options)
        {
            ClearInstance();

            Build(options);
        }

        ///<summary> Очищает нейронную сеть </summary>
        public void ClearNetworkInstance()
        {
            ClearInstance();
        }

        ///<summary> Построение нейронной сети с учетом опций </summary>
        ///<param name="options"> Опции построения нейронной сети </param>
        public void Build(NeuralNetworkBuildOptions options)
        {
            int layerId = 1;
            int neuronId = 1;
            foreach (var layerOptions in options.LayersBuildOptions)
            {
                var layer = new NeuralLayer
                {
                    Id = layerId,
                    Name = layerOptions.Name
                };
                for (int i = 0; i < layerOptions.NeuronsCount; i++)
                {
                    layer.Neurons.Add(new Neuron { Id = neuronId });
                    neuronId++;
                }
                if (layerId < options.LayersBuildOptions.Count && options.UseBias)
                    layer.Neurons.Add(new Neuron { Id = neuronId++, IsHidden = true });
                Instance.Layers.Add(layer);
                layerId++;
            }
            ConnectLayers();
            RandomizeWeights();
        }

        ///<summary> Обучение нейронной сети </summary>
        ///<param name="options"> Опции обучения нейронной сети </summary>
        public void Train(NeuralNetworkTrainOptions options)
        {
            ClearInstanceDataForCompute();
            int maxEpoch = options.Epoch;
            double learningRate = options.LearningRate;

            for (int epoch = 0; epoch < maxEpoch; epoch++)
            {
                foreach (var trainSet in options.TrainSets)
                {
                    // Прогоняем данные
                    Activate(new NeuralNetworkActivateOptions { Input = trainSet.Input, ActivateFunction = options.ActivateFunction });

                    // Оцениваем ошибку
                    trainSet.ErrorRate = _computeService.MSEErrorRate(trainSet.IdealOutput, Instance.Output);
                    if (epoch == 0 || epoch % 1000 == 0)
                        _logger.LogInformation($"Current network instance error rate: {(trainSet.ErrorRate * 100).ToString("0")}% on {epoch} epoch");

                    ComputeDeltas(trainSet, options.ActivateFunction);
                    OptimizeWeights(learningRate, options.Moment);
                }
                learningRate += 0.0000001;

                if (options.TrainSets.TrueForAll(o => o.ErrorRate <= .1))
                {
                    Instance.TrainEnded = true;
                    return;
                }
            }
        }

        ///<summary> Функция активации нейронной сети </summary>
        public void Activate(NeuralNetworkActivateOptions options)
        {
            for (int i = 0; i < Instance.Layers.Count; i++)
            {
                var layer = Instance.Layers[i];
                for (int j = 0; j < layer.Neurons.Count; j++)
                {
                    var neuron = layer.Neurons[j];

                    if (neuron.IsHidden)
                        continue;

                    if (i == 0)
                    {
                        neuron.Axon = options.Input[j];
                        continue;
                    }

                    ActivateNeuron(neuron, options.ActivateFunction);
                }
            }
        }

        ///<summary> Функция активации нейрона с помощью сигмоида </summary>
        ///<param name="neuron"> Нейрон, который необходимо активировать </param>
        private void ActivateNeuron(Neuron neuron, ActivateFunctions function)
        {
            double input = 0.0;
            foreach (var synapse in neuron.Dendrites)
            {
                var biasNeuronWeight = neuron.Dendrites.FirstOrDefault(d => d.Input.IsHidden)?.Weight;
                input += synapse.Input.Axon * synapse.Weight + (biasNeuronWeight.HasValue ? biasNeuronWeight.Value : 0);
            }

            switch (function)
            {
                default:
                    neuron.Axon = _computeService.Sigmoid(input);
                    break;
                case ActivateFunctions.HyperbolicTangent:
                    neuron.Axon = _computeService.HyperbolicTangent(input);
                    break;
            }
        }

        ///<summary> Соединение слоев нейронов </summary>
        private void ConnectLayers()
        {
            for (int i = 1; i < Instance.Layers.Count; i++)
            {
                var currentLayer = Instance.Layers[i];
                var prevLayer = Instance.Layers[i - 1];

                foreach (var to in currentLayer.Neurons)
                {
                    foreach (var from in prevLayer.Neurons)
                    {
                        if (!to.IsHidden)
                            to.Dendrites.Add(new Synapse { Input = from });
                    }
                }
            }
        }

        ///<summary> Случайное выставление весов синапсов нейронов </summary>
        private void RandomizeWeights()
        {
            var randomizer = new Random();
            foreach (var layer in Instance.Layers.OrderByDescending(o => o.Id))
            {
                foreach (var neuron in layer.Neurons)
                {
                    neuron.Dendrites.ForEach(d => d.Weight = randomizer.NextDouble());
                }
            }
        }

        ///<summary> Очищает нейронную сеть </summary>
        private void ClearInstance()
        {
            Instance.Layers = new List<NeuralLayer>();
        }

        ///<summary> Удаляет лишние данные, которые необходимы для расчетов </summary>
        private void ClearInstanceDataForCompute()
        {
            for (int i = Instance.Layers.Count - 1; i >= 0; i--)
            {
                var layer = Instance.Layers[i];
                foreach (var neuron in layer.Neurons)
                {
                    neuron.Delta = 0;
                    foreach (var dendrite in neuron.Dendrites)
                    {
                        dendrite.Delta = 0;
                        dendrite.Gradient = 0;
                        dendrite.PreviousDelta = 0;
                    }
                }
            }
        }

        ///<summary> Вычисляет дельты нейронов </summary>
        private void ComputeDeltas(TrainSet trainSet, ActivateFunctions function)
        {
            for (int i = Instance.Layers.Count - 1; i > 0; i--)
            {
                var layer = Instance.Layers[i];
                if (i == Instance.Layers.Count - 1)
                {
                    for (int oi = 0; oi < layer.Neurons.Count; oi++)
                    {
                        var neuron = layer.Neurons[oi];
                        switch (function)
                        {
                            default:
                                neuron.Delta = _computeService.OutputDeltaForSigmoid(trainSet.IdealOutput[oi], neuron.Axon);
                                break;
                            case ActivateFunctions.HyperbolicTangent:
                                neuron.Delta = _computeService.OutputDeltaForHyperbolicTangent(trainSet.IdealOutput[oi], neuron.Axon);
                                break;
                        }
                    }
                    continue;
                }

                var prevLayer = Instance.Layers[i + 1];

                for (int j = 0; j < layer.Neurons.Count; j++)
                {
                    var neuron = layer.Neurons[j];
                    var neuronDendrites = prevLayer.Neurons.SelectMany(o => o.Dendrites.Where(d => d.Input.Id == neuron.Id));
                    switch (function)
                    {
                        default:
                            neuron.Delta = _computeService.HiddenDeltaForSigmoid(neuron.Axon, neuronDendrites.Select(d => d.Weight).ToList(), prevLayer.Neurons.Select(o => o.Delta).ToList());
                            break;
                        case ActivateFunctions.HyperbolicTangent:
                            neuron.Delta = _computeService.HiddenDeltaForHyperbolicTangent(neuron.Axon, neuronDendrites.Select(d => d.Weight).ToList(), prevLayer.Neurons.Select(o => o.Delta).ToList());
                            break;
                    }
                }
            }
        }

        ///<summary> Оптимизирует веса нейронов </summary>
        private void OptimizeWeights(double learningRate, double moment)
        {
            for (int i = 1; i < Instance.Layers.Count; i++)
            {
                var layer = Instance.Layers[i];
                foreach (var neuron in layer.Neurons)
                {
                    foreach (var synapse in neuron.Dendrites)
                        synapse.Gradient = _computeService.GRADAB(neuron.Delta, synapse.Input.Axon);

                    UpdateSynapsesWeights(neuron, learningRate, moment);
                }
            }
        }

        ///<summary> Обновляет веса нейронов </summary>
        private void UpdateSynapsesWeights(Neuron neuron, double learningRate, double moment)
        {
            foreach (var synapse in neuron.Dendrites)
            {
                synapse.PreviousDelta = synapse.Delta;
                synapse.Delta = (learningRate * synapse.Gradient) + (moment * synapse.PreviousDelta);

                synapse.Weight += synapse.Delta;
            }
        }
    }
}