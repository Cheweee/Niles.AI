using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Niles.AI.API.Services;
using Niles.AI.Models.Settings;
using Niles.AI.Services;
using Niles.AI.Services.Interfaces;
using Niles.AI.API.Extensions;
using Niles.AI.API.Hubs;

namespace Niles.AI.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins("http://localhost:8080", "http://localhost:4200")
                    .AllowCredentials();
                });
            });
            services.AddSignalR();
            services.AddControllers();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSingleton<NeuralNetworkHub>();

            services.AddSingleton<RabbitMQService>(provider =>
            {
                var settings = Configuration.GetSection("RabbitMQConnectionSettings").Get<RabbitMQConnectionOptions>();
                var logger = provider.GetService<ILogger<RabbitMQService>>();
                return new RabbitMQService(settings, logger);
            });

            services.AddSingleton<NeuralNetworkService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NeuralNetworkHub>("/neuralnetworkhub");
            });

            app.UseRabbitListener();
        }
    }
}
