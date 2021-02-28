using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Niles.AI.API.Hubs
{
    public class NeuralNetworkHub : Hub
    {
        protected IHubContext<NeuralNetworkHub> _context;
        public NeuralNetworkHub(IHubContext<NeuralNetworkHub> context)
        {
            this._context = context;
        }

        public async Task Send(string message)
        {
            await this._context.Clients.All.SendAsync("niles.neuralnetwork", message);
        }
    }
}