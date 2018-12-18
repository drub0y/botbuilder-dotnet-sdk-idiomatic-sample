using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;

namespace HackedBrain.BotBuilder.Samples.IdiomaticNetCore.BotWebApp
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment ?? throw new System.ArgumentNullException(nameof(hostingEnvironment));
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
