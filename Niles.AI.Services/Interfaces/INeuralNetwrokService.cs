using Niles.AI.Models;

namespace Niles.AI.Services.Interfaces
{
    ///<summary> Сервис для управления нейронной сетью </summary>
    public interface INeuralNetworkService
    {
        ///<summary> Построение нейронной сети </summary>
        void Build(NeuralNetworkBuildOptions options);
        ///<summary> Обучение нейронной сети </summary>
        void Train(NeuralNetworkTrainOptions options);
        ///<summary> Функция активации нейронной сети </summary>
        void Activate(NeuralNetworkActivateOptions options);
        void ClearInstance();
    }
}