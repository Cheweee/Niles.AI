using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Niles.AI.Services.Interfaces;
using Niles.AI.API.Services;

namespace Niles.AI.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static NeuralNetworkService NeuralNetworkService { get; set; }

        public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                NeuralNetworkService = scope.ServiceProvider.GetService<NeuralNetworkService>();

                var lifecicle = app.ApplicationServices.GetService<IHostApplicationLifetime>();

                lifecicle.ApplicationStarted.Register(OnApplicationStarted);

                return app;
            }
        }

        private static void OnApplicationStarted()
        {
            NeuralNetworkService.SubscribeOnGetInstanceResponse();
        }
    }
}