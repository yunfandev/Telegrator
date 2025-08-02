using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using Telegram.Bot.Types.Enums;
using Telegrator.Configuration;
using Telegrator.Hosting.Components;
using Telegrator.Hosting.Providers;
using Telegrator.Hosting.Web.Components;
using Telegrator.Hosting.Logging;
using Telegrator.Logging;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Hosting.Web
{
    /// <summary>
    /// Represents a web hosted telegram bot
    /// </summary>
    public class TelegramBotWebHost : ITelegramBotWebHost
    {
        private readonly WebApplication _innerApp;
        private readonly IUpdateRouter _updateRouter;
        private readonly ILogger<TelegramBotWebHost> _logger;

        private bool _disposed;

        /// <inheritdoc/>
        public IServiceProvider Services => _innerApp.Services;

        /// <inheritdoc/>
        public IUpdateRouter UpdateRouter => _updateRouter;

        /// <inheritdoc/>
        public ICollection<EndpointDataSource> DataSources => ((IEndpointRouteBuilder)_innerApp).DataSources;

        /// <summary>
        /// Allows consumers to be notified of application lifetime events.
        /// </summary>
        public IHostApplicationLifetime Lifetime => _innerApp.Lifetime;

        /// <summary>
        /// This application's logger
        /// </summary>
        public ILogger<TelegramBotWebHost> Logger => _logger;

        // Private interface fields
        IServiceProvider IEndpointRouteBuilder.ServiceProvider => Services;
        IServiceProvider IApplicationBuilder.ApplicationServices { get => Services; set => throw new NotImplementedException(); }
        IFeatureCollection IApplicationBuilder.ServerFeatures => ((IApplicationBuilder)_innerApp).ServerFeatures;
        IDictionary<string, object?> IApplicationBuilder.Properties => ((IApplicationBuilder)_innerApp).Properties;

        internal TelegramBotWebHost(WebApplicationBuilder webApplicationBuilder, HostHandlersCollection handlers)
        {
            // Registering this host in services for easy access
            RegisterHostServices(webApplicationBuilder, handlers);

            // Building proxy application
            _innerApp = webApplicationBuilder.Build();

            // Initializing bot info, as it requires to make a request via tg bot
            Services.GetRequiredService<ITelegramBotInfo>();

            // Reruesting services for this host
            _updateRouter = Services.GetRequiredService<IUpdateRouter>();
            _logger = Services.GetRequiredService<ILogger<TelegramBotWebHost>>();

            // Logging registering handlers in DEBUG purposes
            LogHandlers(handlers);
        }

        /// <summary>
        /// Creates new <see cref="TelegramBotHostBuilder"/> with default services and webhook update receiving scheme
        /// </summary>
        /// <returns></returns>
        public static TelegramBotWebHostBuilder CreateBuilder(TelegramBotWebOptions settings)
        {
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));
            WebApplicationBuilder innerApp = WebApplication.CreateBuilder(settings.ToWebApplicationOptions());
            TelegramBotWebHostBuilder builder = new TelegramBotWebHostBuilder(innerApp, settings);
            builder.Services.AddTelegramBotHostDefaults();
            builder.Services.AddTelegramWebhook();
            return builder;
        }

        /// <summary>
        /// Creates new SLIM <see cref="TelegramBotHostBuilder"/> with default services and webhook update receiving scheme
        /// </summary>
        /// <returns></returns>
        public static TelegramBotWebHostBuilder CreateSlimBuilder(TelegramBotWebOptions settings)
        {
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));
            WebApplicationBuilder innerApp = WebApplication.CreateSlimBuilder(settings.ToWebApplicationOptions());
            TelegramBotWebHostBuilder builder = new TelegramBotWebHostBuilder(innerApp, settings);
            builder.Services.AddTelegramBotHostDefaults();
            builder.Services.AddTelegramWebhook();
            return builder;
        }

        /// <summary>
        /// Creates new EMPTY <see cref="TelegramBotHostBuilder"/> WITHOUT any services or update receiving schemes
        /// </summary>
        /// <returns></returns>
        public static TelegramBotWebHostBuilder CreateEmptyBuilder(TelegramBotWebOptions settings)
        {
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));
            WebApplicationBuilder innerApp = WebApplication.CreateEmptyBuilder(settings.ToWebApplicationOptions());
            return new TelegramBotWebHostBuilder(innerApp, settings);
        }

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _innerApp.StartAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _innerApp.StopAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public IApplicationBuilder CreateApplicationBuilder()
            => ((IEndpointRouteBuilder)_innerApp).CreateApplicationBuilder();

        /// <inheritdoc/>
        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
            => _innerApp.Use(middleware);

        /// <inheritdoc/>
        public IApplicationBuilder New()
            => ((IApplicationBuilder)_innerApp).New();

        /// <inheritdoc/>
        public RequestDelegate Build()
            => ((IApplicationBuilder)_innerApp).Build();

        /// <summary>
        /// Disposes the host.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            await _innerApp.DisposeAsync();

            GC.SuppressFinalize(this);
            _disposed = true;
        }

        /// <summary>
        /// Disposes the host.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            // Sorry for this, i really dont know how to handle such cases
            ValueTask disposeTask = _innerApp.DisposeAsync();
            while (!disposeTask.IsCompleted)
                Thread.Sleep(100);

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

        private void RegisterHostServices(WebApplicationBuilder hostApplicationBuilder, HostHandlersCollection handlers)
        {
            //hostApplicationBuilder.Services.RemoveAll<IHost>();
            //hostApplicationBuilder.Services.AddSingleton<IHost>(this);

            hostApplicationBuilder.Services.AddSingleton<ITelegramBotHost>(this);
            hostApplicationBuilder.Services.AddSingleton<ITelegramBotWebHost>(this);
            hostApplicationBuilder.Services.AddSingleton<ITelegratorBot>(this);
            hostApplicationBuilder.Services.AddSingleton<IHandlersCollection>(handlers);
        }
    }
}
