using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegrator.Hosting.Web.Components;
using Telegrator.Hosting.Web.Polling;

namespace Telegrator.Hosting.Web
{
    /// <summary>
    /// Contains extensions for <see cref="IServiceCollection"/>
    /// Provides method to configure <see cref="ITelegramBotWebHost"/>
    /// </summary>
    public static class ServicesCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="ITelegramBotClient"/> service with <see cref="HostedUpdateWebhooker"/> to receive updates using webhook
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTelegramWebhook(this IServiceCollection services)
        {
            services.AddHttpClient<ITelegramBotClient>("tgwebhook").RemoveAllLoggers().AddTypedClient(TypedTelegramBotClientFactory);
            services.AddHostedService<HostedUpdateWebhooker>();
            return services;
        }

        private static ITelegramBotClient TypedTelegramBotClientFactory(HttpClient httpClient, IServiceProvider provider)
            => new TelegramBotClient(provider.GetRequiredService<IOptions<TelegramBotClientOptions>>().Value, httpClient);
    }
}
