using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegrator.Hosting.Components;
using Telegrator.MadiatorCore;
using Telegrator.Polling;

namespace Telegrator.Hosting.Polling
{
    public class HostedUpdateReceiver(ITelegramBotHost botHost, ITelegramBotClient botClient, IUpdateRouter updateRouter, IOptions<ReceiverOptions> options, ILogger<HostedUpdateReceiver> logger) : BackgroundService
    {
        private readonly ReceiverOptions ReceiverOptions = options.Value;
        private readonly IUpdateRouter UpdateRouter = updateRouter;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting receiving updates via long-polling");
            ReceiverOptions.AllowedUpdates = botHost.UpdateRouter.HandlersProvider.AllowedTypes.ToArray();
            ReactiveUpdateReceiver updateReceiver = new ReactiveUpdateReceiver(botClient, ReceiverOptions);
            await updateReceiver.ReceiveAsync(UpdateRouter, stoppingToken).ConfigureAwait(false);
        }
    }
}
