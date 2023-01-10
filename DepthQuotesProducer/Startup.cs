using Abstractions.Interfaces;
using DepthQuotesProducer.AppConfiguration;
using DepthQuotesProducer.Binance;
using DepthQuotesProducer.NatsProducer;
using DepthQuotesProducer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NATS.Client;

namespace DepthQuotesProducer
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
            services.AddLogging(cfg =>
                cfg.AddConsole()
                .AddFilter((category, level) =>
                {
                    if (category.Contains("Microsoft.AspNetCore") || category.Contains("Microsoft.Hosting"))
                    {
                        if (level >= LogLevel.Warning)
                            return true;
                        else
                            return false;
                    }

                    return true;
                }));

            services.AddControllers();
            services.Configure<NatsConfiguration>(Configuration.GetSection("Nats"));
            services.Configure<SymbolsConfiguration>(Configuration.GetSection("Symbols"));

            services
                .AddSingleton<IConnectionFactory, ConnectionFactory>()
                .AddSingleton<IExchangeClient, BinanceClient>()
                .AddSingleton<IProducer, NatsProducer.NatsProducer>()
                .AddSingleton<IProducerService, ProducerService>();

            services.AddHostedService<ProducerHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
