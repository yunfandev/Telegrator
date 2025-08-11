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
    public class AbstractHandlerContainer<TUpdate> : IAbstractHandlerContainer<TUpdate> where TUpdate : class
    {
        /// <summary>
        /// Gets the actual update object of type TUpdate.
        /// </summary>
        public TUpdate ActualUpdate { get; }

        /// <inheritdoc/>
        public Update HandlingUpdate { get; }

        /// <inheritdoc/>
        public ITelegramBotClient Client { get; }

        /// <inheritdoc/>
        public Dictionary<string, object> ExtraData { get; }

        /// <inheritdoc/>
        public CompletedFiltersList CompletedFilters { get; }

        /// <inheritdoc/>
        public IAwaitingProvider AwaitingProvider { get; }

        public AbstractHandlerContainer(DescribedHandlerInfo handlerInfo)
        {
            ActualUpdate = handlerInfo.HandlingUpdate.GetActualUpdateObject<TUpdate>();
            HandlingUpdate = handlerInfo.HandlingUpdate;
            Client = handlerInfo.Client;
            ExtraData = handlerInfo.ExtraData;
            CompletedFilters = handlerInfo.CompletedFilters;
            AwaitingProvider = handlerInfo.AwaitingProvider;
        }

        public AbstractHandlerContainer(TUpdate actualUpdate, Update handlingUpdate, ITelegramBotClient client, Dictionary<string, object> extraData, CompletedFiltersList filters, IAwaitingProvider awaitingProvider)
        {
            ActualUpdate = actualUpdate;
            HandlingUpdate = handlingUpdate;
            Client = client;
            ExtraData = extraData;
            CompletedFilters = filters;
            AwaitingProvider = awaitingProvider;
        }

        public AbstractHandlerContainer<QUpdate> CreateChild<QUpdate>() where QUpdate : class
        {
            return new AbstractHandlerContainer<QUpdate>(
                HandlingUpdate.GetActualUpdateObject<QUpdate>(),
                HandlingUpdate, Client, ExtraData,
                CompletedFilters, AwaitingProvider);
        }

        public static AbstractHandlerContainer<TUpdate> From<QUpdate>(IAbstractHandlerContainer<QUpdate> other) where QUpdate : class
        {
            return new AbstractHandlerContainer<TUpdate>(
                other.HandlingUpdate.GetActualUpdateObject<TUpdate>(),
                other.HandlingUpdate, other.Client, other.ExtraData,
                other.CompletedFilters, other.AwaitingProvider);
        }
    }
}
