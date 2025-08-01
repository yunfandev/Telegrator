using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Configuration;

namespace Telegrator.Hosting
{
    public class HostedTelegramBotInfo(ITelegramBotClient client, IServiceProvider services, IConfigurationManager configuration) : ITelegramBotInfo
    {
        /// <inheritdoc/>
        public User User { get; } = client.GetMe().Result;
        
        public IServiceProvider Services { get; } = services;

        public IConfigurationManager Configuration { get; } = configuration;
    }
}
