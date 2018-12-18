using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HackedBrain.BotBuilder.Samples.IdiomaticNetCore.BotWebApp
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            _hostingEnvironment = hostingEnvironment ?? throw new System.ArgumentNullException(nameof(hostingEnvironment));
            _loggerFactory = loggerFactory ?? throw new System.ArgumentNullException(nameof(loggerFactory));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBotState(botStateConfigBuilder =>
            {
                var storage = new MemoryStorage();

                botStateConfigBuilder
                    .UseConversationState(botStateBuilder =>
                    {
                        botStateBuilder
                            .UseStorage(storage)
                            .WithProperty<SampleBotState>();
                    });
            });

            services.AddBot<SampleBot>(options =>
            {
                options.UseBotConfigurationEndpointCredentialsFromFolder(_hostingEnvironment.ContentRootPath, endpointName: _hostingEnvironment.EnvironmentName);

                var logger = _loggerFactory.CreateLogger<SampleBot>();

                options.OnTurnError = async (context, exception) =>
                {
                    logger.LogError("An unhandled exception occurred in this turn: {Exception}", exception);

                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseBotFramework();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
