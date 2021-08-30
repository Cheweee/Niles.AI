using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Niles.AI.Models.Settings;
using Niles.AI.Services;
using Niles.AI.Worker.Services;
using Niles.AI.Worker.Extensions;

namespace Niles.AI.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<RabbitMQConnectionOptions>(hostContext.Configuration.GetSection("RabbitMQConnectionSettings"));
                    services.AddSingleton<RabbitMQService>(provider => {
                        var settings = hostContext.Configuration.GetSection("RabbitMQConnectionSettings").Get<RabbitMQConnectionOptions>();
                        var logger = provider.GetService<ILogger<RabbitMQService>>();
                        return new RabbitMQService(settings, logger);
                    });
                    services.AddSingleton<INeuralNetworkTyped, SigmoidNeuralNetwork>();
                    services.AddSingleton<INeuralNetworkTyped, HyperbolicTangentNeuralNetwork>();
                    services.AddSingleton<WorkerRabbitMQService>(provider => {
                        var rabbitMQService = provider.GetService<RabbitMQService>();
                        return new WorkerRabbitMQService(rabbitMQService, provider.ComposeNeuralNetworks());
                    });
                    services.AddSingleton<ComputeService>();
                    services.AddHostedService<Worker>();
                });
    }
}
