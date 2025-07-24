using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;
using Telegrator.MadiatorCore;

namespace Telegrator.MadiatorCore.Descriptors
{
    /// <summary>
    /// Contains information about a described handler, including its context, client, and execution logic.
    /// </summary>
    public class DescribedHandlerInfo
    {
        /// <summary>
        /// The update router associated with this handler.
        /// </summary>
        public readonly IUpdateRouter UpdateRouter;

        /// <summary>
        /// The Telegram bot client used for this handler.
        /// </summary>
        public readonly ITelegramBotClient Client;

        /// <summary>
        /// The handler instance being described.
        /// </summary>
        public readonly UpdateHandlerBase HandlerInstance;

        /// <summary>
        /// Extra data associated with the handler execution.
        /// </summary>
        public readonly Dictionary<string, object> ExtraData;

        /// <summary>
        /// List of completed filters for this handler.
        /// </summary>
        public readonly CompletedFiltersList CompletedFilters;

        /// <summary>
        /// The update being handled.
        /// </summary>
        public readonly Update HandlingUpdate;

        /// <summary>
        /// Lifetime token for the handler instance.
        /// </summary>
        public HandlerLifetimeToken HandlerLifetime => HandlerInstance.LifetimeToken;

        /// <summary>
        /// The handler container created during execution.
        /// </summary>
        public IHandlerContainer? HandlerContainer { get; private set; }

        /// <summary>
        /// Display string for the handler (for debugging or logging).
        /// </summary>
        public string DisplayString { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescribedHandlerInfo"/> class.
        /// </summary>
        /// <param name="updateRouter">The update router.</param>
        /// <param name="client">The Telegram bot client.</param>
        /// <param name="handlerInstance">The handler instance.</param>
        /// <param name="filterContext">The filter execution context.</param>
        /// <param name="displayString">Optional display string.</param>
        public DescribedHandlerInfo(IUpdateRouter updateRouter, ITelegramBotClient client, UpdateHandlerBase handlerInstance, FilterExecutionContext<Update> filterContext, string? displayString)
        {
            UpdateRouter = updateRouter;
            Client = client;
            HandlerInstance = handlerInstance;
            ExtraData = filterContext.Data;
            CompletedFilters = filterContext.CompletedFilters;
            HandlingUpdate = filterContext.Update;
            DisplayString = displayString ?? handlerInstance.GetType().Name;
        }

        /// <summary>
        /// Executes the handler logic asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown if the handler lifetime has ended or the handler is not a container factory.</exception>
        public async Task Execute(CancellationToken cancellationToken)
        {
            if (HandlerLifetime.IsEnded)
                throw new Exception();

            IHandlerContainerFactory? containerFactory = HandlerInstance is IHandlerContainerFactory handlerDefainedContainerFactory
                ? handlerDefainedContainerFactory
                : UpdateRouter.DefaultContainerFactory is not null
                    ? UpdateRouter.DefaultContainerFactory
                    : throw new Exception();

            try
            {
                HandlerContainer = containerFactory.CreateContainer(UpdateRouter.AwaitingProvider,  this);
                await HandlerInstance.Execute(HandlerContainer, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Cancelled
                _ = 0xBAD + 0xC0DE;
                return;
            }
            catch (Exception exception)
            {
                await UpdateRouter
                    .HandleErrorAsync(Client, exception, HandleErrorSource.HandleUpdateError, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
