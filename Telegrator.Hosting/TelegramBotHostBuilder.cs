using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegrator.Hosting;
using Telegrator.Hosting.Components;
using Telegrator.Hosting.Configuration;
using Telegrator.Hosting.Providers;
using Telegrator.Hosting.Providers.Components;
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
        private readonly IHandlersCollection _handlers;

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
        public TelegramBotHostBuilder(HostApplicationBuilder hostApplicationBuilder, TelegramBotHostBuilderSettings? settings = null)
        {
            _innerBuilder = hostApplicationBuilder ?? throw new ArgumentNullException(nameof(hostApplicationBuilder));
            _settings = settings ?? new TelegramBotHostBuilderSettings();
            _handlers = new HostHandlersCollection(Services, _settings);

            _innerBuilder.Logging.ClearProviders();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
        /// </summary>
        /// <param name="hostApplicationBuilder"></param>
        /// <param name="handlers"></param>
        /// <param name="settings"></param>
        public TelegramBotHostBuilder(HostApplicationBuilder hostApplicationBuilder, IHandlersCollection handlers, TelegramBotHostBuilderSettings? settings = null)
        {
            _innerBuilder = hostApplicationBuilder ?? throw new ArgumentNullException(nameof(hostApplicationBuilder));
            _settings = settings ?? new TelegramBotHostBuilderSettings();
            _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));

            _innerBuilder.Logging.ClearProviders();
        }

        /// <summary>
        /// Builds the host.
        /// </summary>
        /// <returns></returns>
        public TelegramBotHost Build()
        {
            if (_handlers is IHostHandlersCollection hostHandlers)
            {
                foreach (PreBuildingRoutine preBuildRoutine in hostHandlers.PreBuilderRoutines)
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
            }

            if (!_settings.DisableAutoConfigure)
            {
                Services.Configure<ReceiverOptions>(Configuration.GetSection(nameof(ReceiverOptions)));
                Services.Configure<TelegramBotClientOptions>(Configuration.GetSection(nameof(TelegramBotClientOptions)), new TelegramBotClientOptionsProxy());
            }
            else
            {
                /*
                if (null == Services.SingleOrDefault(srvc => srvc.ImplementationType == typeof(IOptions<ReceiverOptions>)))
                    throw new MissingMemberException("Auto configuration disabled, yet no options of type 'ReceiverOptions' wasn't registered. This configuration is runtime required!");
                */

                if (null == Services.SingleOrDefault(srvc => srvc.ImplementationType == typeof(IOptions<TelegramBotClientOptions>)))
                    throw new MissingMemberException("Auto configuration disabled, yet no options of type 'TelegramBotClientOptions' wasn't registered. This configuration is runtime required!");
            }

            Services.AddSingleton<IOptions<TelegratorOptions>>(Options.Create(_settings));
            Services.AddSingleton<IConfigurationManager>(Configuration);
            return new TelegramBotHost(_innerBuilder, _handlers);
        }
    }
}
