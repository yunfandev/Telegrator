using Telegrator.Handlers;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Polling
{
    /// <summary>
    /// Implementation of <see cref="IUpdateHandlersPool"/> that manages the execution of handlers.
    /// Provides thread-safe queuing and execution of handlers with configurable concurrency limits.
    /// </summary>
    public class UpdateHandlersPool : IUpdateHandlersPool
    {
        /// <summary>
        /// Synchronization object for thread-safe operations.
        /// </summary>
        protected object SyncObj = new object();

        /// <summary>
        /// Semaphore for controlling the number of concurrently executing handlers.
        /// </summary>
        protected SemaphoreSlim ExecutingHandlersSemaphore = null!;

        /// <summary>
        /// The bot configuration options.
        /// </summary>
        protected readonly TelegratorOptions Options;

        /// <summary>
        /// The global cancellation token for stopping all operations.
        /// </summary>
        protected readonly CancellationToken GlobalCancellationToken;

        /// <summary>
        /// Flag indicating whether the pool has been disposed.
        /// </summary>
        protected bool disposed = false;

        /// <inheritdoc/>
        public event HandlerEnqueued? HandlerEnqueued;

        /// <inheritdoc/>
        public event HandlerExecuting? HandlerExecuting;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateHandlersPool"/> class.
        /// </summary>
        /// <param name="options">The bot configuration options.</param>
        /// <param name="globalCancellationToken">The global cancellation token.</param>
        public UpdateHandlersPool(TelegratorOptions options, CancellationToken globalCancellationToken)
        {
            Options = options;
            GlobalCancellationToken = globalCancellationToken;

            if (options.MaximumParallelWorkingHandlers != null)
            {
                ExecutingHandlersSemaphore = new SemaphoreSlim(options.MaximumParallelWorkingHandlers.Value);
            }
        }

        /// <inheritdoc/>
        public async Task Enqueue(IEnumerable<DescribedHandlerInfo> handlers)
        {
            Result? lastResult = null;
            foreach (DescribedHandlerInfo handlerInfo in handlers)
            {
                if (lastResult?.NextType != null)
                {
                    if (lastResult.NextType != handlerInfo.HandlerInstance.GetType())
                        continue;
                }

                if (ExecutingHandlersSemaphore != null)
                {
                    await ExecutingHandlersSemaphore.WaitAsync();
                }

                try
                {
                    HandlerExecuting?.Invoke(handlerInfo);
                    lastResult = await handlerInfo.Execute(GlobalCancellationToken);
                    ExecutingHandlersSemaphore?.Release(1);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                if (!lastResult.RouteNext)
                    break;
            }
        }

        /// <summary>
        /// Disposes of the handlers pool and releases all resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (disposed)
                return;

            if (ExecutingHandlersSemaphore != null)
            {
                ExecutingHandlersSemaphore.Dispose();
                ExecutingHandlersSemaphore = null!;
            }

            /*
            if (AwaitingHandlersQueuedEvent != null)
            {
                AwaitingHandlersQueuedEvent.Dispose();
                AwaitingHandlersQueuedEvent = null!;
            }
            */

            if (SyncObj != null)
                SyncObj = null!;

            GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}
