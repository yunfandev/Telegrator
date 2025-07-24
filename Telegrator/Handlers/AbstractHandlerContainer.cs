using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;
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
        private readonly TUpdate _actualUpdate;
        private readonly Update _handlingUpdate;
        private readonly ITelegramBotClient _client;
        private readonly Dictionary<string, object> _extraData;
        private readonly CompletedFiltersList _completedFilters;
        private readonly IAwaitingProvider _awaitingProvider;

        /// <summary>
        /// Gets the actual update object of type TUpdate.
        /// </summary>
        public TUpdate ActualUpdate => _actualUpdate;

        /// <inheritdoc/>
        public Update HandlingUpdate => _handlingUpdate;

        /// <inheritdoc/>
        public ITelegramBotClient Client => _client;

        /// <inheritdoc/>
        public Dictionary<string, object> ExtraData => _extraData;

        /// <inheritdoc/>
        public CompletedFiltersList CompletedFilters => _completedFilters;

        /// <inheritdoc cref="IHandlerContainer.AwaitingProvider"/>
        public IAwaitingProvider AwaitingProvider => _awaitingProvider;

        /// <inheritdoc/>
        IAwaitingProvider IHandlerContainer.AwaitingProvider => AwaitingProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractHandlerContainer{TUpdate}"/> class.
        /// </summary>
        /// <param name="awaitingProvider">The awaiting provider for managing async operations.</param>
        /// <param name="handlerInfo">The handler information containing execution context.</param>
        public AbstractHandlerContainer(IAwaitingProvider awaitingProvider, DescribedHandlerInfo handlerInfo)
        {
            _actualUpdate = handlerInfo.HandlingUpdate.GetActualUpdateObject<TUpdate>();
            _handlingUpdate = handlerInfo.HandlingUpdate;
            _client = handlerInfo.Client;
            _extraData = handlerInfo.ExtraData;
            _completedFilters = handlerInfo.CompletedFilters;
            _awaitingProvider = awaitingProvider;
        }
    }
}
