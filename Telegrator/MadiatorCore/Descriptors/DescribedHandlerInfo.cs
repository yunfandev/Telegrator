using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using Telegrator.Handlers;
using Telegrator.Handlers.Components;

namespace Telegrator.MadiatorCore.Descriptors
{
    /// <summary>
    /// Contains information about a described handler, including its context, client, and execution logic.
    /// </summary>
    public class DescribedHandlerInfo
    {
        /// <summary>
        /// descriptor from that handler was described from
        /// </summary>
        public readonly HandlerDescriptor From;

        /// <summary>
        /// The update router associated with this handler.
        /// </summary>
        public readonly IUpdateRouter UpdateRouter;

        /// <summary>
        /// The awaiting provider to fetch new updates inside handler
        /// </summary>
        public readonly IAwaitingProvider AwaitingProvider;

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
        /// Display string for the handler (for debugging or logging).
        /// </summary>
        public string DisplayString { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescribedHandlerInfo"/> class.
        /// </summary>
        /// <param name="fromDescriptor">descriptor from that handler was described from</param>
        /// <param name="awaitingProvider"></param>
        /// <param name="updateRouter">The update router.</param>
        /// <param name="client">The Telegram bot client.</param>
        /// <param name="handlerInstance">The handler instance.</param>
        /// <param name="filterContext">The filter execution context.</param>
        /// <param name="displayString">Optional display string.</param>
        public DescribedHandlerInfo(HandlerDescriptor fromDescriptor, IUpdateRouter updateRouter, IAwaitingProvider awaitingProvider, ITelegramBotClient client, UpdateHandlerBase handlerInstance, FilterExecutionContext<Update> filterContext, string? displayString)
        {
            From = fromDescriptor;
            UpdateRouter = updateRouter;
            AwaitingProvider = awaitingProvider;
            Client = client;
            HandlerInstance = handlerInstance;
            ExtraData = filterContext.Data;
            CompletedFilters = filterContext.CompletedFilters;
            HandlingUpdate = filterContext.Update;
            DisplayString = displayString ?? fromDescriptor.HandlerType.Name;
        }

        /// <inheritdoc/>
        public override string ToString()
            => DisplayString ?? From.HandlerType.Name;
    }
}
