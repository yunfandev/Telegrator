using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Configuration;
using Telegrator.Handlers.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;
using Telegrator.Providers;

namespace Telegrator.Hosting.Providers
{
    public class HostHandlersProvider : HandlersProvider
    {
        private readonly IServiceProvider Services;
        private readonly ILogger<HostHandlersProvider> Logger;

        public HostHandlersProvider(IHandlersCollection handlers, IOptions<TelegramBotOptions> options, ITelegramBotInfo botInfo, IServiceProvider serviceProvider, ILogger<HostHandlersProvider> logger)
            : base(handlers, options.Value, botInfo)
        {
            Services = serviceProvider;
            Logger = logger;
        }

        public override IEnumerable<DescribedHandlerInfo> GetHandlers(IUpdateRouter updateRouter, ITelegramBotClient client, Update update, CancellationToken cancellationToken = default)
        {
            IEnumerable<DescribedHandlerInfo> handlers = base.GetHandlers(updateRouter, client, update, cancellationToken).ToArray();
            Logger.LogInformation("Described handlers : {handlers}", string.Join(", ", handlers.Select(hndlr => hndlr.DisplayString ?? hndlr.HandlerInstance.GetType().Name)));
            return handlers;
        }

        public override UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IServiceScope scope = Services.CreateScope();
            object handlerInstance = descriptor.ServiceKey == null
                ? scope.ServiceProvider.GetRequiredService(descriptor.HandlerType)
                : scope.ServiceProvider.GetRequiredKeyedService(descriptor.HandlerType, descriptor.ServiceKey);

            if (handlerInstance is not UpdateHandlerBase updateHandler)
                throw new InvalidOperationException();

            updateHandler.LifetimeToken.OnLifetimeEnded += _ => scope.Dispose();
            return updateHandler;
        }
    }
}
