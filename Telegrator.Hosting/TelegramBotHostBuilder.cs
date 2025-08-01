using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegrator.Hosting;
using Telegrator.Hosting.Components;
using Telegrator.Hosting.Configuration;
using Telegrator.Hosting.Providers;
using Telegrator.MadiatorCore;

#pragma warning disable IDE0001
namespace Telegrator.Hosting
{
    /// <summary>
    /// Represents a hosted telegram bots and services builder that helps manage configuration, logging, lifetime, and more.
    /// </summary>
    public class TelegramBotHostBuilder : ITelegramBotHostBuilder
    {
        private readonly HostApplicationBuilder _innerBuilder;
        private readonly TelegramBotHostBuilderSettings _settings;
        private readonly HostHandlersCollection _handlers;

        /// <inheritdoc/>
        public IHandlersCollection Handlers => _handlers;

        /// <inheritdoc/>
        public IServiceCollection Services => _innerBuilder.Services;

        /// <inheritdoc/>
        public IConfigurationManager Configuration => _innerBuilder.Configuration;

        /// <inheritdoc/>
        public ILoggingBuilder Logging => _innerBuilder.Logging;

        /// <inheritdoc/>
        public IHostEnvironment Environment => _innerBuilder.Environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
        /// </summary>
        /// <param name="hostApplicationBuilder"></param>
        /// <param name="settings"></param>
        internal TelegramBotHostBuilder(HostApplicationBuilder hostApplicationBuilder, TelegramBotHostBuilderSettings? settings = null)
        {
            _innerBuilder = hostApplicationBuilder;
            _settings = settings ?? new TelegramBotHostBuilderSettings();
            _handlers = new HostHandlersCollection(Services, _settings);

            _innerBuilder.Logging.ClearProviders();
        }

        /// <summary>
        /// Builds the host.
        /// </summary>
        /// <returns></returns>
        public TelegramBotHost Build()
        {
            foreach (PreBuildingRoutine preBuildRoutine in _handlers.PreBuilderRoutines)
            {
                try
                {
                    preBuildRoutine.Invoke(this);
                }
                catch (NotImplementedException)
                {
                    _ = 0xBAD + 0xC0DE;
                }
            }

            if (!_settings.DisableAutoConfigure)
            {
                Services.Configure<TelegratorOptions>(Configuration.GetSection(nameof(TelegratorOptions)));
                Services.Configure<ReceiverOptions>(Configuration.GetSection(nameof(ReceiverOptions)));
                Services.Configure<TelegramBotClientOptions>(Configuration.GetSection(nameof(TelegramBotClientOptions)), new TelegramBotClientOptionsProxy());
            }

            return new TelegramBotHost(_innerBuilder, _handlers);
        }
    }
}
