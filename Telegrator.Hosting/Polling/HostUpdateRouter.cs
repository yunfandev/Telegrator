using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegrator;
using Telegrator.Configuration;
using Telegrator.MadiatorCore;
using Telegrator.Polling;

namespace Telegrator.Hosting.Polling
{
    public class HostUpdateRouter : UpdateRouter
    {
        protected readonly ILogger<HostUpdateRouter> Logger;

        public HostUpdateRouter(IHandlersProvider handlersProvider, IAwaitingProvider awaitingProvider, IOptions<TelegramBotOptions> options, IUpdateHandlersPool handlersPool, ILogger<HostUpdateRouter> logger)
            : base(handlersProvider, awaitingProvider, options.Value, handlersPool)
        {
            Logger = logger;
            ExceptionHandler = new HostExceptionHandler(logger);
        }

        public override Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Received update of type \"{type}\"", update.Type);
            return base.HandleUpdateAsync(botClient, update, cancellationToken);
        }

        private class HostExceptionHandler(ILogger<HostUpdateRouter> logger) : IRouterExceptionHandler
        {
            public void HandleException(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
            {
                if (exception is HandlerFaultedException handlerFaultedException)
                {
                    logger.LogError("\"{handler}\" handler's execution was faulted :\n{exception}",
                        handlerFaultedException.HandlerInfo.DisplayString,
                        handlerFaultedException.InnerException?.ToString() ?? "No inner exception");
                    return;
                }

                logger.LogError("Exception was thrown during update routing faulted :\n{exception}", exception.ToString());
            }
        }
    }
}
