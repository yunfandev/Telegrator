using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegrator.Hosting.Components;
using Telegrator.Hosting.Configuration;
using Telegrator.Hosting.Providers;
using Telegrator.MadiatorCore;

#pragma warning disable IDE0001
namespace Telegrator.Hosting.Web
{
    /// <summary>
    /// Represents a web hosted telegram bots and services builder that helps manage configuration, logging, lifetime, and more.
    /// </summary>
    public class TelegramBotWebHostBuilder : ITelegramBotHostBuilder
    {
        private readonly WebApplicationBuilder _innerBuilder;
        private readonly TelegramBotWebOptions _settings;
        private readonly HostHandlersCollection _handlers;

        /// <inheritdoc/>
        public IHandlersCollection Handlers => _handlers;

        /// <inheritdoc/>
        public IConfigurationManager Configuration => _innerBuilder.Configuration;

        /// <inheritdoc/>
        public ILoggingBuilder Logging => _innerBuilder.Logging;

        /// <inheritdoc/>
        public IServiceCollection Services => _innerBuilder.Services;

        /// <inheritdoc/>
        public IHostEnvironment Environment => _innerBuilder.Environment;

        internal TelegramBotWebHostBuilder(WebApplicationBuilder webApplicationBuilder, TelegramBotWebOptions settings)
        {
            _innerBuilder = webApplicationBuilder;
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _handlers = new HostHandlersCollection(Services, _settings);
        }

        /// <summary>
        /// Builds the host.
        /// </summary>
        /// <returns></returns>
        public TelegramBotWebHost Build()
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
                Services.Configure<TelegratorWebOptions>(Configuration.GetSection(nameof(TelegratorWebOptions)));
                Services.Configure<TelegratorOptions>(Configuration.GetSection(nameof(TelegratorOptions)));
                Services.Configure<TelegramBotClientOptions>(Configuration.GetSection(nameof(TelegramBotClientOptions)), new TelegramBotClientOptionsProxy());
            }

            Services.AddSingleton<IConfigurationManager>(Configuration);
            Services.AddSingleton<IOptions<TelegratorOptions>>(Options.Create(_settings));
            return new TelegramBotWebHost(_innerBuilder, _handlers);
        }
    }
}
