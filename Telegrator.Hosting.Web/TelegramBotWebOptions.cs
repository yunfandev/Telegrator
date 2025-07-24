using Telegrator.Configuration;

namespace Telegrator.Hosting.Web
{
    public class TelegramBotWebOptions : TelegramBotOptions
    {
        /// <summary>
        /// Gets or sets uri for webhook update receiving
        /// </summary>
        public required string WebhookUri { get; set; }

        public required string WebhookPattern { get; set; }

        public int MaxConnections { get; set; }

        public bool DropPendingUpdates { get; set; }
    }
}
