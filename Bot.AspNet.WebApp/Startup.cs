using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace HackedBrain.BotBuilder.Samples.IdiomaticNetCore.BotWebApp
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment ?? throw new System.ArgumentNullException(nameof(hostingEnvironment));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var botConfiguration = BotConfiguration.LoadFromFolder(_hostingEnvironment.ContentRootPath);

            var endpoint = botConfiguration.Services.OfType<EndpointService>().FirstOrDefault(ep => ep.Name == _hostingEnvironment.EnvironmentName);

            services.AddBot<SampleBot>(options =>
            {
                options.CredentialProvider = new SimpleCredentialProvider(
                    endpoint.AppId,
                    endpoint.AppPassword);
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
