using Microsoft.Bot.Builder;

namespace HackedBrain.BotBuilder.Samples.IdiomaticNetCore.BotWebApp
{
    public class SampleBotStateAccessors
    {
        public ConversationState ConversationState { get; set; }

        public IStatePropertyAccessor<SampleBotState> SampleBotStateAccessor { get; set; }
    }
}