using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Bot.Builder.Integration.AspNet.Core
{
    public static class BotFrameworkOptionsExtensions
    {
        public static BotFrameworkOptions UseBotConfigurationEndpointCredentialsFromFolder(this BotFrameworkOptions options, string botConfigurationFileFolder, string botFileSecretKey = null, string endpointName = null)
        {
            options.CredentialProvider = BotConfigurationEndpointServiceCredentialProvider.LoadFromFolder(botConfigurationFileFolder, botFileSecretKey, endpointName);

            return options;
        }

        public static BotFrameworkOptions UseBotConfigurationEndpointCredentials(this BotFrameworkOptions options, string botConfigurationFilePath, string botFileSecretKey = null, string endpointName = null)
        {
            options.CredentialProvider = BotConfigurationEndpointServiceCredentialProvider.Load(botConfigurationFilePath, botFileSecretKey, endpointName);

            return options;
        }
    }
}
