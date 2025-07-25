using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Configuration;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;
using Telegrator.Providers;

namespace Telegrator.Hosting.Providers
{
    /// <inheritdoc/>
    public class HostAwaitingProvider(IOptions<TelegramBotOptions> options, ITelegramBotInfo botInfo, ILogger<HostAwaitingProvider> logger) : AwaitingProvider(options.Value, botInfo)
    {
        /// <inheritdoc/>
        public override IEnumerable<DescribedHandlerInfo> GetHandlers(IUpdateRouter updateRouter, ITelegramBotClient client, Update update, CancellationToken cancellationToken = default)
        {
            IEnumerable<DescribedHandlerInfo> handlers = base.GetHandlers(updateRouter, client, update, cancellationToken).ToArray();
            logger.LogInformation("Described awaiting handlers : {handlers}", string.Join(", ", handlers.Select(hndlr => hndlr.HandlerInstance.GetType().Name)));
            return handlers;
        }
    }
}
