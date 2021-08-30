using System;
using System.Collections.Generic;
using System.Linq;
using Niles.AI.Services.Interfaces;

namespace Niles.AI.Worker.Services
{
    public interface INeuralNetworkTyped : INeuralNetworkService
    {
        ActivateFunctions ActivateFunction { get; }
        NeuralNetwork Instance { get; }
    }

    public interface INeuralNetwork
    {
        ///<summary> Построение нейронной сети </summary>
        void Build(NeuralNetworkBuildOptions options, ActivateFunctions activateFunction);

        ///<summary> Обучение нейронной сети </summary>
        void Train(NeuralNetworkTrainOptions options, ActivateFunctions activateFunction);

        ///<summary> Функция активации нейронной сети </summary>
        void Activate(NeuralNetworkActivateOptions options, ActivateFunctions activateFunction);

        void ClearInstance(ActivateFunctions activateFunction);

        NeuralNetwork GetInstance(ActivateFunctions activateFunctions);
    }

    public static class NeuralNetworkExtensions
    {
        public static void BuildExtensions(this IEnumerable<INeuralNetworkTyped> members, NeuralNetworkBuildOptions options, ActivateFunctions activateFunction = ActivateFunctions.Sigmoid)
        {
            var membersList = members.ToList();
            if (membersList != null)
            {
                for (int i = 0; i < membersList.Count; i++)
                    if (membersList[i].ActivateFunction == activateFunction)
                        membersList[i].Build(options);
            }
            else
            {
                foreach (var member in members)
                    if (member.ActivateFunction == activateFunction)
                        member.Build(options);
            }
        }

        public static void TrainExtensions(this IEnumerable<INeuralNetworkTyped> members, NeuralNetworkTrainOptions options, ActivateFunctions activateFunction = ActivateFunctions.Sigmoid)
        {
            var membersList = members.ToList();
            if (membersList != null)
            {
                for (int i = 0; i < membersList.Count; i++)
                    if (membersList[i].ActivateFunction == activateFunction)
                        membersList[i].Train(options);
            }
            else
            {
                foreach (var member in members)
                    if (member.ActivateFunction == activateFunction)
                        member.Train(options);
            }
        }

        public static void ActivateExtensions(this IEnumerable<INeuralNetworkTyped> members, NeuralNetworkActivateOptions options, ActivateFunctions activateFunction = ActivateFunctions.Sigmoid)
        {
            var membersList = members.ToList();
            if (membersList != null)
            {
                for (int i = 0; i < membersList.Count; i++)
                    if (membersList[i].ActivateFunction == activateFunction)
                        membersList[i].Activate(options);
            }
            else
            {
                foreach (var member in members)
                    if (member.ActivateFunction == activateFunction)
                        member.Activate(options);
            }
        }

        public static void ClearExtensions(this IEnumerable<INeuralNetworkTyped> members, ActivateFunctions activateFunction = ActivateFunctions.Sigmoid)
        {
            var membersList = members.ToList();
            if (membersList != null)
            {
                for (int i = 0; i < membersList.Count; i++)
                    if (membersList[i].ActivateFunction == activateFunction)
                        membersList[i].ClearInstance();
            }
            else
            {
                foreach (var member in members)
                    if (member.ActivateFunction == activateFunction)
                        member.ClearInstance();
            }
        }

        public static NeuralNetwork GetInstance(this IEnumerable<INeuralNetworkTyped> members, ActivateFunctions activateFunction = ActivateFunctions.Sigmoid)
        {
            var membersList = members.ToList();
            if (membersList != null)
            {
                for (int i = 0; i < membersList.Count; i++)
                    if (membersList[i].ActivateFunction == activateFunction)
                        return membersList[i].Instance;
            }
            else
            {
                foreach (var member in members)
                    if (member.ActivateFunction == activateFunction)
                        return member.Instance;
            }

            return null;
        }
    }

    public class NeuralNetworksComposer : INeuralNetwork
    {
        private readonly Func<IEnumerable<INeuralNetworkTyped>> _membersFactory;

        public NeuralNetworksComposer(Func<IEnumerable<INeuralNetworkTyped>> membersFactory)
        {
            _membersFactory = membersFactory ?? throw new System.ArgumentNullException(nameof(membersFactory));
        }

        public void Build(NeuralNetworkBuildOptions options, ActivateFunctions activateFunction) =>  _membersFactory().BuildExtensions(options, activateFunction);

        public void Train(NeuralNetworkTrainOptions options, ActivateFunctions activateFunction) => _membersFactory().TrainExtensions(options, activateFunction);

        public void Activate(NeuralNetworkActivateOptions options, ActivateFunctions activateFunction) => _membersFactory().ActivateExtensions(options, activateFunction);

        public void ClearInstance(ActivateFunctions activateFunction) => _membersFactory().ClearExtensions(activateFunction);

        public NeuralNetwork GetInstance(ActivateFunctions activateFunction) => _membersFactory().GetInstance(activateFunction);
    }
}