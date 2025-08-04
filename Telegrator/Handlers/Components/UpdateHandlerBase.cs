using System.ComponentModel;
using System.Threading;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;
using Telegrator.Polling;

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
        /// <param name="described"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Result> Execute(DescribedHandlerInfo described, CancellationToken cancellationToken = default)
        {
            if (LifetimeToken.IsEnded)
                throw new Exception();

            try
            {
                // Creating container
                IHandlerContainer container = GetContainer(described);
                DescriptorAspectsSet? aspects = described.From.Aspects;

                // Executing pre processor
                if (aspects != null)
                {
                    Result? preResult = await aspects.ExecutePre(this, container, cancellationToken).ConfigureAwait(false);
                    if (!preResult.Positive)
                        return preResult;
                }

                // Executing handler
                Result execResult = await ExecuteInternal(container, cancellationToken).ConfigureAwait(false);
                if (!execResult.Positive)
                    return execResult;

                // Executing post processor
                if (aspects != null)
                {
                    Result postResult = await aspects.ExecutePost(this, container, cancellationToken).ConfigureAwait(false);
                    if (!postResult.Positive)
                        return postResult;
                }

                // Success
                return Result.Ok();
            }
            catch (OperationCanceledException)
            {
                // Cancelled
                _ = 0xBAD + 0xC0DE;
                return Result.Ok();
            }
            catch (Exception exception)
            {
                await described.UpdateRouter
                    .HandleErrorAsync(described.Client, exception, HandleErrorSource.HandleUpdateError, cancellationToken)
                    .ConfigureAwait(false);

                return Result.Fault();
            }
            finally
            {
                LifetimeToken.LifetimeEnded();
            }
        }

        private IHandlerContainer GetContainer(DescribedHandlerInfo handlerInfo)
        {
            if (this is IHandlerContainerFactory handlerDefainedContainerFactory)
                return handlerDefainedContainerFactory.CreateContainer(handlerInfo);

            if (handlerInfo.UpdateRouter.DefaultContainerFactory is not null)
                return handlerInfo.UpdateRouter.DefaultContainerFactory.CreateContainer(handlerInfo);

            throw new Exception();
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
