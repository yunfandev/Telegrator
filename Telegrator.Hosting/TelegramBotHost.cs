using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using Telegram.Bot.Types.Enums;
using Telegrator.Hosting.Components;
using Telegrator.Hosting.Providers;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Hosting
{
    /// <summary>
    /// Represents a hosted telegram bot
    /// </summary>
    public class TelegramBotHost : ITelegramBotHost
    {
        private readonly IHost _innerHost;
        private readonly IUpdateRouter _updateRouter;
        private readonly ILogger<TelegramBotHost> _logger;

        private bool _disposed;

        /// <inheritdoc/>
        public IServiceProvider Services => _innerHost.Services;

        /// <inheritdoc/>
        public IUpdateRouter UpdateRouter => _updateRouter;

        /// <summary>
        /// This application's logger
        /// </summary>
        public ILogger<TelegramBotHost> Logger => _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotHost"/> class.
        /// </summary>
        /// <param name="hostApplicationBuilder">The service provider.</param>
        /// <param name="handlers"></param>
        internal TelegramBotHost(HostApplicationBuilder hostApplicationBuilder, HostHandlersCollection handlers)
        {
            RegisterHostServices(hostApplicationBuilder, handlers);
            _innerHost = hostApplicationBuilder.Build();

            _updateRouter = Services.GetRequiredService<IUpdateRouter>();
            _logger = Services.GetRequiredService<ILogger<TelegramBotHost>>();

            LogHandlers(handlers);
        }

        /// <summary>
        /// Creates new <see cref="TelegramBotHostBuilder"/> with default configuration, services and long-polling update receiving scheme
        /// </summary>
        /// <returns></returns>
        public static TelegramBotHostBuilder CreateBuilder()
        {
            HostApplicationBuilder innerBuilder = new HostApplicationBuilder(settings: null);
            TelegramBotHostBuilder builder = new TelegramBotHostBuilder(innerBuilder, null);
            builder.Services.AddTelegramBotHostDefaults();
            builder.Services.AddTelegramReceiver();
            return builder;
        }

        /// <summary>
        /// Creates new <see cref="TelegramBotHostBuilder"/> with default services and long-polling update receiving scheme
        /// </summary>
        /// <returns></returns>
        public static TelegramBotHostBuilder CreateBuilder(TelegramBotHostBuilderSettings? settings)
        {
            HostApplicationBuilder innerBuilder = new HostApplicationBuilder(settings?.ToApplicationBuilderSettings());
            TelegramBotHostBuilder builder = new TelegramBotHostBuilder(innerBuilder, settings);
            builder.Services.AddTelegramBotHostDefaults();
            builder.Services.AddTelegramReceiver();
            return builder;
        }

        /// <summary>
        /// Creates new EMPTY <see cref="TelegramBotHostBuilder"/> WITHOUT any services or update receiving schemes
        /// </summary>
        /// <returns></returns>
        public static TelegramBotHostBuilder CreateEmptyBuilder()
        {
            HostApplicationBuilder innerBuilder = Host.CreateEmptyApplicationBuilder(null);
            return new TelegramBotHostBuilder(innerBuilder, null);
        }

        /// <summary>
        /// Creates new EMPTY <see cref="TelegramBotHostBuilder"/> WITHOUT any services or update receiving schemes
        /// </summary>
        /// <returns></returns>
        public static TelegramBotHostBuilder CreateEmptyBuilder(TelegramBotHostBuilderSettings? settings)
        {
            HostApplicationBuilder innerBuilder = Host.CreateEmptyApplicationBuilder(null);
            return new TelegramBotHostBuilder(innerBuilder, settings);
        }

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _innerHost.StartAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _innerHost.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Disposes the host.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _innerHost.Dispose();

            GC.SuppressFinalize(this);
            _disposed = true;
        }

        private void LogHandlers(HostHandlersCollection handlers)
        {
            StringBuilder logBuilder = new StringBuilder("Registered handlers : ");
            if (!handlers.Keys.Any())
                throw new Exception();

            foreach (UpdateType updateType in handlers.Keys)
            {
                HandlerDescriptorList descriptors = handlers[updateType];
                logBuilder.Append("\n\tUpdateType." + updateType + " :");

                foreach (HandlerDescriptor descriptor in descriptors.Reverse())
                {
                    logBuilder.AppendFormat("\n\t* {0} - {1}",
                        descriptor.Indexer.ToString(),
                        descriptor.ToString());
                }
            }

            Logger.LogInformation(logBuilder.ToString());
        }

        private void RegisterHostServices(HostApplicationBuilder hostApplicationBuilder, HostHandlersCollection handlers)
        {
            //hostApplicationBuilder.Services.RemoveAll<IHost>();
            //hostApplicationBuilder.Services.AddSingleton<IHost>(this);

            hostApplicationBuilder.Services.AddSingleton<ITelegramBotHost>(this);
            hostApplicationBuilder.Services.AddSingleton<ITelegratorBot>(this);
            hostApplicationBuilder.Services.AddSingleton<IHandlersCollection>(handlers);
        }
    }
}
