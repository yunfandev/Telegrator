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
    /// <inheritdoc/>
    public class HostUpdateRouter : UpdateRouter
    {
        /// <summary>
        /// <see cref="ILogger"/> of this router
        /// </summary>
        protected readonly ILogger<HostUpdateRouter> Logger;

        // Ehat a mess :/
        /// <inheritdoc/>
        public HostUpdateRouter(
            IHandlersProvider handlersProvider,
            IAwaitingProvider awaitingProvider,
            IOptions<TelegramBotOptions> options,
            IUpdateHandlersPool handlersPool,
            ILogger<HostUpdateRouter> logger) : base(handlersProvider, awaitingProvider, options.Value, handlersPool)
        {
            Logger = logger;
            ExceptionHandler = new HostExceptionHandler(logger);
        }

        /// <inheritdoc/>
        public override Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Received update of type \"{type}\"", update.Type);
            return base.HandleUpdateAsync(botClient, update, cancellationToken);
        }

        /// <summary>
        /// Default exception handler of this router
        /// </summary>
        /// <param name="logger"></param>
        private class HostExceptionHandler(ILogger<HostUpdateRouter> logger) : IRouterExceptionHandler
        {
            public void HandleException(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
            {
                if (exception is HandlerFaultedException handlerFaultedException)
                {
                    logger.LogError("\"{handler}\" handler's execution was faulted :\n{exception}",
                        handlerFaultedException.HandlerInfo.ToString(),
                        handlerFaultedException.InnerException?.ToString() ?? "No inner exception");
                    return;
                }

                logger.LogError("Exception was thrown during update routing faulted :\n{exception}", exception.ToString());
            }
        }
    }
}
