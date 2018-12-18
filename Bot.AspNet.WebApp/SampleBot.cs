using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace HackedBrain.BotBuilder.Samples.IdiomaticNetCore.BotWebApp
{
    public class SampleBot : IBot
    {
        private readonly ConversationState _conversationState;
        private readonly IStatePropertyAccessor<SampleBotState> _statePropertyAccessor;

        public SampleBot(ConversationState conversationState, IStatePropertyAccessor<SampleBotState> statePropertyAccessor)
        {
            _conversationState = conversationState;
            _statePropertyAccessor = statePropertyAccessor;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var state = await _statePropertyAccessor.GetAsync(
                turnContext,
                () => new SampleBotState());

            if (!state.HasIssuedGreeting)
            {
                state.HasIssuedGreeting = true;

                await turnContext.SendActivityAsync("Hello World!");
            }

            var activity = turnContext.Activity;

            if (activity.Type == ActivityTypes.Message)
            {
                state.EchoCount++;

                await turnContext.SendActivityAsync($"[{state.EchoCount}] You said: {activity.Text}", cancellationToken: cancellationToken);
            }

            await _conversationState.SaveChangesAsync(turnContext);
        }
    }
}