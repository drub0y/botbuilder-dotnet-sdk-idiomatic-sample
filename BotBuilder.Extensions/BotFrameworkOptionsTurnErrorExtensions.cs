using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Microsoft.Bot.Builder.Integration.AspNet.Core
{
    public static class BotFrameworkOptionsTurnErrorExtensions
    {
        public static BotFrameworkOptions LogUnhandledTurnExceptions(this BotFrameworkOptions options, ILoggerFactory loggerFactory) =>
            options.LogUnhandledTurnExceptions(loggerFactory.CreateLogger("Bot.TurnError"));

        public static BotFrameworkOptions LogUnhandledTurnExceptions(this BotFrameworkOptions options, ILogger logger)
        {
            options.OnTurnError = (context, exception) =>
            {
                logger.LogError("An unexpected exception occurred while processing turn: {Exception}", exception);

                return Task.CompletedTask;
            };

            return options;
        }

        public static BotFrameworkOptions LogUnhandledTurnExceptions(this BotFrameworkOptions options, ILoggerFactory loggerFactory, string friendlyErrorMessage) =>
            options.LogUnhandledTurnExceptions(loggerFactory.CreateLogger("Bot.TurnError"), friendlyErrorMessage);

        public static BotFrameworkOptions LogUnhandledTurnExceptions(this BotFrameworkOptions options, ILogger logger, string friendlyErrorMessage) =>
            options.LogUnhandledTurnExceptions(logger, MessageFactory.Text(friendlyErrorMessage));

        public static BotFrameworkOptions LogUnhandledTurnExceptions(this BotFrameworkOptions options, ILoggerFactory loggerFactory, Activity errorActivity) =>
            options.LogUnhandledTurnExceptions(loggerFactory.CreateLogger("Bot.TurnError"), errorActivity);

        public static BotFrameworkOptions LogUnhandledTurnExceptions(this BotFrameworkOptions options, ILogger logger, Activity errorActivity) =>
            options.LogUnhandledTurnExceptions(logger, _ => errorActivity);

        public static BotFrameworkOptions LogUnhandledTurnExceptions(this BotFrameworkOptions options, ILogger logger, Func<ITurnContext, Activity> errorActivityProvider)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (errorActivityProvider == null)
            {
                throw new ArgumentNullException(nameof(errorActivityProvider));
            }

            options.OnTurnError = async (context, exception) =>
            {
                logger.LogError("An unexpected exception occurred while processing turn: ChannelId={ChannelId};From={FromId};Exception={Exception}", context.Activity.ChannelId, context.Activity.From.Id, exception);

                await context.SendActivityAsync(errorActivityProvider(context));
            };

            return options;
        }
    }
}
