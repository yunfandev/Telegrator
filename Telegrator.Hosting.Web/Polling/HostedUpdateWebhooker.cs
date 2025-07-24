using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegrator.Hosting.Web.Components;
using Telegrator.MadiatorCore;

namespace Telegrator.Hosting.Web.Polling
{
    public class HostedUpdateWebhooker : IHostedService
    {
        private readonly ITelegramBotWebHost _botHost;
        private readonly ITelegramBotClient _botClient;
        private readonly IUpdateRouter _updateRouter;
        private readonly TelegramBotWebOptions _options;

        public HostedUpdateWebhooker(ITelegramBotWebHost botHost, ITelegramBotClient botClient, IUpdateRouter updateRouter, IOptions<TelegramBotWebOptions> options)
        {
            if (string.IsNullOrEmpty(options.Value.WebhookUri))
                throw new ArgumentNullException(nameof(options), "Option \"WebhookUrl\" must be set to subscribe for update recieving");

            if (string.IsNullOrEmpty(options.Value.WebhookPattern))
                throw new ArgumentNullException(nameof(options), "Option \"WebhookPattern\" must be set to subscribe for update recieving");

            _botHost = botHost;
            _botClient = botClient;
            _updateRouter = updateRouter;
            _options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _botClient.SetWebhook(
                url: _options.WebhookUri,
                maxConnections: _options.MaxConnections,
                allowedUpdates: _botHost.UpdateRouter.HandlersProvider.AllowedTypes,
                dropPendingUpdates: _options.DropPendingUpdates,
                cancellationToken: cancellationToken);

            //botHost.MapGet(_options.WebhookPattern, async (Update update) => await _updateRouter.HandleUpdateAsync(_botClient, update, cancellationToken));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _botClient.DeleteWebhook(_options.DropPendingUpdates, cancellationToken);
            return Task.CompletedTask;
        }
    }
}
