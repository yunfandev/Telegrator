using Telegram.Bot.Types.Enums;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Handlers.Components
{
    /// <summary>
    /// Base class for update handlers, providing execution and lifetime management for Telegram updates.
    /// </summary>
    public abstract class UpdateHandlerBase(UpdateType handlingUpdateType)
    {
        /// <summary>
        /// Gets the <see cref="UpdateType"/> that this handler processes.
        /// </summary>
        public UpdateType HandlingUpdateType { get; } = handlingUpdateType;

        /// <summary>
        /// Gets the <see cref="HandlerLifetimeToken"/> associated with this handler instance.
        /// </summary>
        public HandlerLifetimeToken LifetimeToken { get; } = new HandlerLifetimeToken();

        /// <summary>
        /// Executes the handler logic and marks the lifetime as ended after execution.
        /// </summary>
        /// <param name="container">The <see cref="IHandlerContainer"/> for the update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Result> Execute(IHandlerContainer container, CancellationToken cancellationToken = default)
        {
            try
            {
                return await ExecuteInternal(container, cancellationToken);
            }
            finally
            {
                LifetimeToken.LifetimeEnded();
            }
        }

        /// <summary>
        /// Executes the handler logic for the given container and cancellation token.
        /// </summary>
        /// <param name="container">The <see cref="IHandlerContainer"/> for the update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task<Result> ExecuteInternal(IHandlerContainer container, CancellationToken cancellationToken);
    }
}
