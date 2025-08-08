using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Configuration;
using Telegrator.Hosting.Components;
using Telegrator.Hosting.Configuration;
using Telegrator.Hosting.Logging;
using Telegrator.Hosting.Polling;
using Telegrator.Hosting.Providers;
using Telegrator.Logging;
using Telegrator.MadiatorCore;

namespace Telegrator.Hosting
{
    /// <summary>
    /// Contains extensions for <see cref="IServiceCollection"/>
    /// Provides method to configure <see cref="ITelegramBotHost"/>
    /// </summary>
    public static class ServicesCollectionExtensions
    {
        /// <summary>
        /// Registers a configuration instance that strongly-typed <typeparamref name="TOptions"/> will bind against using <see cref="ConfigureOptionsProxy{TOptions}"/>.
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="optionsProxy"></param>
        /// <returns></returns>
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, IConfiguration configuration, ConfigureOptionsProxy<TOptions> optionsProxy) where TOptions : class
        {
            optionsProxy.Configure(services, configuration);
            return services;
        }

        /// <summary>
        /// Registers <see cref="TelegramBotHost"/> default services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTelegramBotHostDefaults(this IServiceCollection services)
        {
            services.AddLogging(builder => builder.AddConsole().AddDebug());
            services.AddSingleton<IUpdateHandlersPool, HostUpdateHandlersPool>();
            services.AddSingleton<IAwaitingProvider, HostAwaitingProvider>();
            services.AddSingleton<IHandlersProvider, HostHandlersProvider>();
            services.AddSingleton<IUpdateRouter, HostUpdateRouter>();
            //services.AddSingleton<ITelegramBotInfo, TelegramBotInfo>(services => new TelegramBotInfo(services.GetRequiredService<ITelegramBotClient>().GetMe().Result));
            services.AddSingleton<ITelegramBotInfo, HostedTelegramBotInfo>();

            return services;
        }

        /// <summary>
        /// Registers <see cref="ITelegramBotClient"/> service with <see cref="HostedUpdateReceiver"/> to receive updates using long polling
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTelegramReceiver(this IServiceCollection services)
        {
            services.AddHttpClient<ITelegramBotClient>("tgreceiver").RemoveAllLoggers().AddTypedClient(TypedTelegramBotClientFactory);
            services.AddHostedService<HostedUpdateReceiver>();
            return services;
        }

        /// <summary>
        /// <see cref="ITelegramBotClient"/> factory method
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        private static ITelegramBotClient TypedTelegramBotClientFactory(HttpClient httpClient, IServiceProvider provider)
            => new TelegramBotClient(provider.GetRequiredService<IOptions<TelegramBotClientOptions>>().Value, httpClient);
    }

    /// <summary>
    /// Provides useful methods to adjust <see cref="ITelegramBotHost"/>
    /// </summary>
    public static class TelegramBotHostExtensions
    {
        /// <summary>
        /// Configures bots available commands depending on what handlers was registered
        /// </summary>
        /// <param name="botHost"></param>
        /// <returns></returns>
        public static ITelegramBotHost SetBotCommands(this ITelegramBotHost botHost)
        {
            ITelegramBotClient client = botHost.Services.GetRequiredService<ITelegramBotClient>();
            IEnumerable<BotCommand> aliases = botHost.UpdateRouter.HandlersProvider.GetBotCommands();
            client.SetMyCommands(aliases).Wait();
            return botHost;
        }

        /// <summary>
        /// Adds a Microsoft.Extensions.Logging adapter to Alligator using a logger factory.
        /// </summary>
        /// <param name="host"></param>
        public static ITelegramBotHost AddLoggingAdapter(this ITelegramBotHost host)
        {
            ILoggerFactory loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger("Telegrator");
            MicrosoftLoggingAdapter adapter = new MicrosoftLoggingAdapter(logger);
            Alligator.AddAdapter(adapter);
            return host;
        }
    }

    /// <summary>
    /// Provides extension methods for reflection and type inspection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Checks if a type implements the <see cref="IPreBuildingRoutine"/> interface.
        /// </summary>
        /// <param name="handlerType">The type to check.</param>
        /// <param name="routineMethod"></param>
        /// <returns>True if the type implements IPreBuildingRoutine; otherwise, false.</returns>
        public static bool IsPreBuildingRoutine(this Type handlerType, [NotNullWhen(true)] out MethodInfo? routineMethod)
        {
            routineMethod = null;
            if (handlerType.GetInterface(nameof(IPreBuildingRoutine)) == null)
                return false;

            routineMethod = handlerType.GetMethod(nameof(IPreBuildingRoutine.PreBuildingRoutine), BindingFlags.Static | BindingFlags.Public);
            return routineMethod != null;
        }
    }
}
