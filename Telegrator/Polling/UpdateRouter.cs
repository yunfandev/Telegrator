using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Configuration;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;
using Telegrator.Logging;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Polling
{
    /// <summary>
    /// Implementation of <see cref="IUpdateRouter"/> that routes updates to appropriate handlers.
    /// Manages the distribution of updates between regular handlers and awaiting handlers.
    /// </summary>
    public class UpdateRouter : IUpdateRouter
    {
        private readonly TelegratorOptions _options;
        private readonly IHandlersProvider _handlersProvider;
        private readonly IAwaitingProvider _awaitingProvider;
        private readonly IUpdateHandlersPool _HandlersPool;
        private readonly ITelegramBotInfo _botInfo;

        /// <inheritdoc/>
        public IHandlersProvider HandlersProvider => _handlersProvider;

        /// <inheritdoc/>
        public IAwaitingProvider AwaitingProvider => _awaitingProvider;

        /// <inheritdoc/>
        public TelegratorOptions Options => _options;

        /// <inheritdoc/>
        public IUpdateHandlersPool HandlersPool => _HandlersPool;

        /// <inheritdoc/>
        public IRouterExceptionHandler? ExceptionHandler { get; set; }

        /// <inheritdoc/>
        public IHandlerContainerFactory? DefaultContainerFactory { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRouter"/> class.
        /// </summary>
        /// <param name="handlersProvider">The provider for regular handlers.</param>
        /// <param name="awaitingProvider">The provider for awaiting handlers.</param>
        /// <param name="options">The bot configuration options.</param>
        /// <param name="botInfo"></param>
        public UpdateRouter(IHandlersProvider handlersProvider, IAwaitingProvider awaitingProvider, TelegratorOptions options, ITelegramBotInfo botInfo)
        {
            _options = options;
            _handlersProvider = handlersProvider;
            _awaitingProvider = awaitingProvider;
            _HandlersPool = new UpdateHandlersPool(_options, _options.GlobalCancellationToken);
            _botInfo = botInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRouter"/> class with a custom handlers pool.
        /// </summary>
        /// <param name="handlersProvider">The provider for regular handlers.</param>
        /// <param name="awaitingProvider">The provider for awaiting handlers.</param>
        /// <param name="options">The bot configuration options.</param>
        /// <param name="handlersPool">The custom handlers pool to use.</param>
        /// <param name="botInfo"></param>
        public UpdateRouter(IHandlersProvider handlersProvider, IAwaitingProvider awaitingProvider, TelegratorOptions options, IUpdateHandlersPool handlersPool, ITelegramBotInfo botInfo)
        {
            _options = options;
            _handlersProvider = handlersProvider;
            _awaitingProvider = awaitingProvider;
            _HandlersPool = handlersPool;
            _botInfo = botInfo;
        }

        /// <summary>
        /// Handles errors that occur during update processing.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="exception">The exception that occurred.</param>
        /// <param name="source">The source of the error.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous error handling operation.</returns>
        public virtual Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Alligator.LogDebug("Handling exception {0}", exception.GetType().Name);
            ExceptionHandler?.HandleException(botClient, exception, source, cancellationToken);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles incoming updates by routing them to appropriate handlers.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="update">The update to handle.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous update handling operation.</returns>
        public virtual async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Logging
            Alligator.LogDebug("Received Update ({0}) of type \"{1}\"", update.Id, update.Type);
            LogUpdate(update);

            try
            {
                // Getting handlers in update awaiting pool
                IEnumerable<DescribedHandlerInfo> handlers = GetHandlers(AwaitingProvider, botClient, update, cancellationToken);
                if (handlers.Any())
                {
                    // Enqueuing found awiting handlers
                    await HandlersPool.Enqueue(handlers);

                    // Chicking if awaiting handlers has exclusive routing
                    if (Options.ExclusiveAwaitingHandlerRouting)
                    {
                        Alligator.LogDebug("Receiving Update ({0}) completed with only awaiting handlers", update.Id);
                        return;
                    }
                }

                // Queuing reagular handlers for execution
                await HandlersPool.Enqueue(GetHandlers(HandlersProvider, botClient, update, cancellationToken));
                Alligator.LogDebug("Receiving Update ({0}) finished", update.Id);
            }
            catch (OperationCanceledException)
            {
                Alligator.LogDebug("Receiving Update ({0}) cancelled", update.Id);
            }
            catch (Exception ex)
            {
                Alligator.LogDebug("Receiving Update ({0}) finished with exception {1}", update.Id, ex.Message);
                ExceptionHandler?.HandleException(botClient, ex, HandleErrorSource.PollingError, cancellationToken);
            }
        }

        /// <summary>
        /// Gets the handlers that match the specified update, using the provided router and client.
        /// Searches for handlers by update type, falling back to Unknown type if no specific handlers are found.
        /// </summary>
        /// <param name="provider">The privode used to get handlers instance</param>
        /// <param name="client">The Telegram bot client instance</param>
        /// <param name="update">The incoming Telegram update to process</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A collection of described handler information for the update</returns>
        protected virtual IEnumerable<DescribedHandlerInfo> GetHandlers(IHandlersProvider provider, ITelegramBotClient client, Update update, CancellationToken cancellationToken = default)
        {
            Alligator.LogDebug("Requested handlers for UpdateType.{0}", update.Type);
            if (!provider.TryGetDescriptorList(update.Type, out HandlerDescriptorList? descriptors))
            {
                Alligator.LogDebug("No registered, providing Any");
                provider.TryGetDescriptorList(UpdateType.Unknown, out descriptors);
            }

            if (descriptors == null || descriptors.Count == 0)
            {
                Alligator.LogDebug("No handlers provided");
                return [];
            }

            //IEnumerable<DescribedHandlerInfo> described = DescribeDescriptors(provider, descriptors, updateRouter, client, update, cancellationToken);
            //Alligator.RouterWriteLine("Described total of {0} handlers for Update ({1}) from {2} provider", described.Count(), update.Id, provider.GetType().Name);
            //Alligator.RouterWriteLine("Described handlers : {0}", string.Join(", ", described));

            return DescribeDescriptors(provider, descriptors, client, update, cancellationToken);
        }

        /// <summary>
        /// Describes all handler descriptors for a given update context.
        /// Processes descriptors in reverse order and respects the ExecuteOnlyFirstFoundHanlder option.
        /// </summary>
        /// <param name="provider">The privode used to get handlers instance</param>
        /// <param name="descriptors">The list of handler descriptors to process</param>
        /// <param name="client">The Telegram bot client instance</param>
        /// <param name="update">The incoming Telegram update to process</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A collection of described handler information</returns>
        protected virtual IEnumerable<DescribedHandlerInfo> DescribeDescriptors(IHandlersProvider provider, HandlerDescriptorList descriptors, ITelegramBotClient client, Update update, CancellationToken cancellationToken = default)
        {
            try
            {
                Alligator.LogDebug("Describing descriptors of descriptorsList.HandlingType.{0} for Update ({1})", descriptors.HandlingType, update.Id);
                foreach (HandlerDescriptor descriptor in descriptors.Reverse())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    DescribedHandlerInfo? describedHandler = DescribeHandler(provider, descriptor, client, update, cancellationToken);
                    if (describedHandler == null)
                        continue;

                    yield return describedHandler;
                }
            }
            finally
            {
                Alligator.LogDebug("Describing for Update ({0}) finished", update.Id);
            }
        }

        /// <summary>
        /// Describes a single handler descriptor for a given update context.
        /// Validates the handler's filters against the update and creates a handler instance if validation passes.
        /// </summary>
        /// <param name="provider">The privode used to get handlers instance</param>
        /// <param name="descriptor">The handler descriptor to process</param>
        /// <param name="client">The Telegram bot client instance</param>
        /// <param name="update">The incoming Telegram update to process</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The described handler info if validation passes; otherwise, null</returns>
        public virtual DescribedHandlerInfo? DescribeHandler(IHandlersProvider provider, HandlerDescriptor descriptor, ITelegramBotClient client, Update update, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "handler_name", descriptor.ToString() }
            };

            FilterExecutionContext<Update> filterContext = new FilterExecutionContext<Update>(_botInfo, update, update, data, []);
            if (descriptor.Filters != null && !descriptor.Filters.Validate(filterContext))
                return null;

            UpdateHandlerBase handlerInstance = provider.GetHandlerInstance(descriptor, cancellationToken);
            return new DescribedHandlerInfo(descriptor, this, AwaitingProvider, client, handlerInstance, filterContext, descriptor.DisplayString);
        }

        /// <summary>
        /// Methos used to log received <see cref="Update"/> object
        /// </summary>
        /// <param name="update"></param>
        /// <exception cref="NullReferenceException"></exception>
        protected static void LogUpdate(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        Message msg = update.Message ?? throw new NullReferenceException();
                        StringBuilder sb = new StringBuilder("Update.Message");

                        if (msg.From != null)
                            sb.AppendFormat(" from {0} ({1})", msg.From.Username, msg.From.Id);

                        if (msg.Text != null)
                            sb.AppendFormat(" with text '{0}'", msg.Text);

                        if (msg.Sticker != null)
                            sb.AppendFormat(" with sticker '{0}'", msg.Sticker.Emoji);

                        Alligator.LogDebug(sb.ToString());
                        break;
                    }

                case UpdateType.CallbackQuery:
                    {
                        CallbackQuery cq = update.CallbackQuery ?? throw new NullReferenceException();
                        StringBuilder sb = new StringBuilder("Update.CallbackQuery");

                        if (cq.From != null)
                            sb.AppendFormat(" from {0} ({1})", cq.From.Username, cq.From.Id);

                        if (cq.From != null)
                            sb.AppendFormat(" with data '{0}'", cq.Data);

                        Alligator.LogDebug(sb.ToString());
                        break;
                    }
            }
        }
    }
}
