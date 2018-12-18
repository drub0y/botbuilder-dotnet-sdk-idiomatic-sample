using Microsoft.Bot.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Microsoft.Bot.Builder.Integration.AspNet.Core
{
    public static class BotStateServiceCollectionExtensions
    {
        public static IServiceCollection AddBotState(this IServiceCollection services, Action<BotStateConfigurationBuilder> config)
        {
            config(new BotStateConfigurationBuilder(services));

            return services;
        }
    }

    public class BotStateConfigurationBuilder
    {
        private IServiceCollection _services;

        internal BotStateConfigurationBuilder(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get => _services; }

        public BotStateConfigurationBuilder UseBotState(BotState botState)
        {
            _services.AddSingleton<BotState>(botState);
            _services.AddSingleton(botState.GetType(), botState);

            return this;
        }
    }

    public static class BotStateConfigurationBuilderExtensions
    {
        public static BotStateConfigurationBuilder UseUserState(this BotStateConfigurationBuilder builder, Action<BotStateBuilder> config) =>
            UseBotState(builder, config, typeof(UserState));

        public static BotStateConfigurationBuilder UseConversationState(this BotStateConfigurationBuilder builder, Action<BotStateBuilder> config) =>
            UseBotState(builder, config, typeof(ConversationState));

        private static BotStateConfigurationBuilder UseBotState(BotStateConfigurationBuilder builder, Action<BotStateBuilder> config, Type botStateType)
        {
            var stateBuilder = new BotStateBuilder(builder, typeof(ConversationState));

            config(stateBuilder);

            var botState = stateBuilder.Build();

            return builder.UseBotState(botState);
        }
    }

    public class BotStateBuilder
    {
        private readonly BotStateConfigurationBuilder _botStateConfigurationBuilder;
        private readonly Type _botStateType;
        private IStorage _storage;
        private Dictionary<string, Type> _properties;

        internal BotStateBuilder(BotStateConfigurationBuilder botStateConfigurationBuilder, Type botStateType)
        {
            _botStateConfigurationBuilder = botStateConfigurationBuilder ?? throw new ArgumentNullException(nameof(botStateConfigurationBuilder));
            _botStateType = botStateType ?? throw new ArgumentNullException(nameof(botStateType));
            _storage = null;
            _properties = new Dictionary<string, Type>();
        }

        public BotStateBuilder UseStorage(IStorage storage)
        {
            _storage = storage;

            return this;
        }

        public BotStateBuilder WithProperty<TValue>(string name = null)
        {
            _properties.Add(name ?? typeof(TValue).Name, typeof(TValue));

            return this;
        }

        internal BotState Build()
        {
            // WARNING: taming of reflection dragons ahead

            var botState = (BotState)Activator.CreateInstance(_botStateType, _storage);

            // TODO: should be an overload of CreateProperty that takes Type!
            var createPropertyGenericMethodInfo = typeof(BotState).GetMethod("CreateProperty").GetGenericMethodDefinition();
            var statePropertyAccessorGenericInterface = typeof(IStatePropertyAccessor<>);

            var services = _botStateConfigurationBuilder.Services;

            foreach (var entry in _properties)
            {
                var createPropertyMethodInfo = createPropertyGenericMethodInfo.MakeGenericMethod(entry.Value);

                var propertyAccessor = createPropertyMethodInfo.Invoke(botState, new[] { entry.Key });

                services.AddSingleton(statePropertyAccessorGenericInterface.MakeGenericType(entry.Value), propertyAccessor);
            }

            _botStateConfigurationBuilder.UseBotState(botState);

            return botState;
        }
    }

    public static class BotStateBuilderExtensions
    {
        public static BotStateBuilder UseMemoryStorage(this BotStateBuilder builder) =>
            builder.UseStorage(new MemoryStorage());
    }
}
