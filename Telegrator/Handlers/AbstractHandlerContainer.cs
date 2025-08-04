using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Handlers
{
    /// <summary>
    /// Container class that holds the context and data for handler execution.
    /// Provides access to the update, client, filters, and other execution context.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update being handled.</typeparam>
    public class AbstractHandlerContainer<TUpdate>(DescribedHandlerInfo handlerInfo) : IAbstractHandlerContainer<TUpdate> where TUpdate : class
    {
        /// <summary>
        /// Gets the actual update object of type TUpdate.
        /// </summary>
        public TUpdate ActualUpdate { get; } = handlerInfo.HandlingUpdate.GetActualUpdateObject<TUpdate>();

        /// <inheritdoc/>
        public Update HandlingUpdate { get; } = handlerInfo.HandlingUpdate;

        /// <inheritdoc/>
        public ITelegramBotClient Client { get; } = handlerInfo.Client;

        /// <inheritdoc/>
        public Dictionary<string, object> ExtraData { get; } = handlerInfo.ExtraData;

        /// <inheritdoc/>
        public CompletedFiltersList CompletedFilters { get; } = handlerInfo.CompletedFilters;

        /// <inheritdoc/>
        public IAwaitingProvider AwaitingProvider { get; } = handlerInfo.AwaitingProvider;
    }
}
