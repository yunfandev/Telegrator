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
        /// <param name="serviceProvider">The service provider.</param>
        internal TelegramBotHost(HostApplicationBuilder hostApplicationBuilder, HostHandlersCollection handlers)
        {
            RegisterHostServices(hostApplicationBuilder, handlers);
            _innerHost = hostApplicationBuilder.Build();

            _updateRouter = Services.GetRequiredService<IUpdateRouter>();
            _logger = Services.GetRequiredService<ILogger<TelegramBotHost>>();

            LogHandlers(handlers);
        }

        public static TelegramBotHostBuilder CreateBuilder()
        {
            TelegramBotHostBuilder builder = new TelegramBotHostBuilder(null);
            builder.Services.AddTelegramBotHostDefaults();
            builder.Services.AddTelegramReceiver();
            return builder;
        }

        public static TelegramBotHostBuilder CreateBuilder(TelegramBotHostBuilderSettings? settings)
        {
            TelegramBotHostBuilder builder = new TelegramBotHostBuilder(settings);
            builder.Services.AddTelegramBotHostDefaults();
            builder.Services.AddTelegramReceiver();
            return builder;
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
                logBuilder.AppendLine("\n\tUpdateType." + updateType + " :");

                foreach (HandlerDescriptor descriptor in descriptors.Reverse())
                {
                    string indexerString = descriptor.Indexer.ToString();
                    logBuilder.AppendLine("* " + indexerString + " " + (descriptor.DisplayString ?? descriptor.HandlerType.Name));
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
