using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace HackedBrain.BotBuilder.Samples.IdiomaticNetCore.BotWebApp
{
    public class SampleBot : IBot
    {
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await turnContext.SendActivityAsync("Hello World!", cancellationToken: cancellationToken);
        }
    }
}