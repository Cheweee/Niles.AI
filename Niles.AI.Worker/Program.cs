using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Niles.AI.Models.Settings;
using Niles.AI.Services;
using Niles.AI.Worker.Services;

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
                    services.AddSingleton<NeuralNetworkService>();
                    services.AddSingleton<NeuralNetworkRabbitMQService>();
                    services.AddSingleton<ComputeService>();
                    services.AddHostedService<Worker>();
                });
    }
}
