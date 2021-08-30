using System.Linq;
using Microsoft.Extensions.Logging;
using Niles.AI.Services.Interfaces;

namespace Niles.AI.Worker.Services
{
    public class SigmoidNeuralNetwork : NeuralNetworkService, INeuralNetworkTyped
    {
        public ActivateFunctions ActivateFunction { get => ActivateFunctions.Sigmoid; }

        public SigmoidNeuralNetwork(ComputeService _computeService, ILogger<NeuralNetworkService> _logger) : base(_computeService, _logger)
        {
        }

        protected override void ActivateNeuron(Neuron neuron)
        {
            double input = 0.0;
            foreach (var synapse in neuron.Dendrites)
            {
                var biasNeuronWeight = neuron.Dendrites.FirstOrDefault(d => d.Input.IsHidden)?.Weight;
                input += synapse.Input.Axon * synapse.Weight + (biasNeuronWeight.HasValue ? biasNeuronWeight.Value : 0);
            }

            neuron.Axon = _computeService.Sigmoid(input);
        }

        ///<summary> Вычисляет дельты нейронов </summary>
        protected override void ComputeDeltas(TrainSet trainSet)
        {
            for (int i = Instance.Layers.Count - 1; i > 0; i--)
            {
                var layer = Instance.Layers[i];
                if (i == Instance.Layers.Count - 1)
                {
                    for (int oi = 0; oi < layer.Neurons.Count; oi++)
                    {
                        var neuron = layer.Neurons[oi];
                        neuron.Delta = _computeService.OutputDeltaForSigmoid(trainSet.IdealOutput[oi], neuron.Axon);
                    }
                    continue;
                }

                var prevLayer = Instance.Layers[i + 1];

                for (int j = 0; j < layer.Neurons.Count; j++)
                {
                    var neuron = layer.Neurons[j];
                    var neuronDendrites = prevLayer.Neurons.SelectMany(o => o.Dendrites.Where(d => d.Input.Id == neuron.Id));
                    neuron.Delta = _computeService.HiddenDeltaForSigmoid(neuron.Axon, neuronDendrites.Select(d => d.Weight).ToList(), prevLayer.Neurons.Select(o => o.Delta).ToList());
                }
            }
        }
    }
}