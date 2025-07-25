using System;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Configuration;
using Telegrator.Handlers.Components;
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
        /// <summary>
        /// The bot configuration options.
        /// </summary>
        private readonly TelegramBotOptions _options;
        
        /// <summary>
        /// The provider for regular handlers.
        /// </summary>
        private readonly IHandlersProvider _handlersProvider;
        
        /// <summary>
        /// The provider for awaiting handlers.
        /// </summary>
        private readonly IAwaitingProvider _awaitingProvider;
        
        /// <summary>
        /// The pool for managing handler execution.
        /// </summary>
        private readonly IUpdateHandlersPool _HandlersPool;

        /// <inheritdoc/>
        public IHandlersProvider HandlersProvider => _handlersProvider;

        /// <inheritdoc/>
        public IAwaitingProvider AwaitingProvider => _awaitingProvider;

        /// <inheritdoc/>
        public TelegramBotOptions Options => _options;

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
        public UpdateRouter(IHandlersProvider handlersProvider, IAwaitingProvider awaitingProvider, TelegramBotOptions options)
        {
            _options = options;
            _handlersProvider = handlersProvider;
            _awaitingProvider = awaitingProvider;
            _HandlersPool = new UpdateHandlersPool(_options, _options.GlobalCancellationToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRouter"/> class with a custom handlers pool.
        /// </summary>
        /// <param name="handlersProvider">The provider for regular handlers.</param>
        /// <param name="awaitingProvider">The provider for awaiting handlers.</param>
        /// <param name="options">The bot configuration options.</param>
        /// <param name="handlersPool">The custom handlers pool to use.</param>
        public UpdateRouter(IHandlersProvider handlersProvider, IAwaitingProvider awaitingProvider, TelegramBotOptions options, IUpdateHandlersPool handlersPool)
        {
            _options = options;
            _handlersProvider = handlersProvider;
            _awaitingProvider = awaitingProvider;
            _HandlersPool = handlersPool;
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
            LeveledDebug.RouterWriteLine("Handling exception {0}", exception.GetType().Name);
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
        public virtual Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Logging
            LeveledDebug.RouterWriteLine("Received Update ({0}) of type \"{1}\"", update.Id, update.Type);
            LogUpdate(update);

            // Queuing handlers for execution
            foreach (DescribedHandlerInfo handler in GetHandlers(botClient, update, cancellationToken))
                HandlersPool.Enqueue(handler);

            LeveledDebug.RouterWriteLine("Receiving Update ({0}) finished", update.Id);
            return Task.CompletedTask;
        }

        private IEnumerable<DescribedHandlerInfo> GetHandlers(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                // Getting handlers in update awaiting pool
                IEnumerable<DescribedHandlerInfo> handlers = AwaitingProvider.GetHandlers(this, botClient, update, cancellationToken);
                if (handlers.Any() && Options.ExclusiveAwaitingHandlerRouting)
                    return handlers;

                return handlers.Concat(HandlersProvider.GetHandlers(this, botClient, update, cancellationToken));
            }
            catch (OperationCanceledException)
            {
                _ = 0xBAD + 0xC0DE;
                return [];
            }
            catch (Exception ex)
            {
                ExceptionHandler?.HandleException(botClient, ex, HandleErrorSource.PollingError, cancellationToken);
                return [];
            }
        }

        private static void LogUpdate(Update update)
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
                            sb.AppendFormat("'{0}'", msg.Text);

                        LeveledDebug.RouterWriteLine(sb.ToString());
                        break;
                    }
            }
        }
    }
}
