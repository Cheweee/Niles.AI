using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Niles.AI.Services.Interfaces;

namespace Niles.AI.Worker.Services
{
    public abstract class NeuralNetworkService : INeuralNetworkService
    {
        protected readonly ComputeService _computeService;
        protected readonly ILogger<NeuralNetworkService> _logger;

        public NeuralNetwork Instance { get; } = new NeuralNetwork();

        public NeuralNetworkService(
            ComputeService computeService,
            ILogger<NeuralNetworkService> logger
        )
        {
            _computeService = computeService ?? throw new ArgumentNullException(nameof(computeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        ///<summary> Построение нейронной сети с учетом опций </summary>
        ///<param name="options"> Опции построения нейронной сети </param>
        public void Build(NeuralNetworkBuildOptions options)
        {
            ClearInstance();
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
                    Activate(new NeuralNetworkActivateOptions { Input = trainSet.Input });

                    // Оцениваем ошибку
                    trainSet.ErrorRate = _computeService.MSEErrorRate(trainSet.IdealOutput, Instance.Output);
                    if (epoch == 0 || epoch % 1000 == 0)
                        _logger.LogInformation($"Current network instance error rate: {(trainSet.ErrorRate * 100).ToString("0")}% on {epoch} epoch");

                    ComputeDeltas(trainSet);
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

                    ActivateNeuron(neuron);
                }
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
        public void ClearInstance()
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

        ///<summary> Функция активации нейрона с помощью сигмоида </summary>
        ///<param name="neuron"> Нейрон, который необходимо активировать </param>
        protected abstract void ActivateNeuron(Neuron neuron);

        ///<summary> Вычисляет дельты нейронов </summary>
        protected abstract void ComputeDeltas(TrainSet trainSet);

        ///<summary> Оптимизирует веса нейронов </summary>
        protected virtual void OptimizeWeights(double learningRate, double moment)
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
        protected virtual void UpdateSynapsesWeights(Neuron neuron, double learningRate, double moment)
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