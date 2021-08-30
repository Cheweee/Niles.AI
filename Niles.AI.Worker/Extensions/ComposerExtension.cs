using System;
using Microsoft.Extensions.DependencyInjection;
using Niles.AI.Worker.Services;

namespace Niles.AI.Worker.Extensions
{
    public static class ExtensibilityComposer
    {
        public static INeuralNetwork ComposeNeuralNetworks(this IServiceProvider provider)
        {
            return new NeuralNetworksComposer(provider.GetServices<INeuralNetworkTyped>);
        }
    }
}