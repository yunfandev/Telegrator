using System.Collections.Concurrent;
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
        /// Event that signals when awaiting handlers are queued.
        /// </summary>
        protected ManualResetEventSlim AwaitingHandlersQueuedEvent = null!;

        /// <summary>
        /// Semaphore for controlling the number of concurrently executing handlers.
        /// </summary>
        protected SemaphoreSlim ExecutingHandlersSemaphore = null!;

        /// <summary>
        /// Queue for storing awaiting handlers.
        /// </summary>
        protected readonly ConcurrentQueue<DescribedHandlerInfo> AwaitingHandlersQueue = [];

        /// <summary>
        /// Dictionary for tracking currently executing handlers.
        /// </summary>
        protected readonly ConcurrentDictionary<HandlerLifetimeToken, Task> ExecutingHandlersPool = [];

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
                ExecutingHandlersSemaphore = new SemaphoreSlim(options.MaximumParallelWorkingHandlers ?? 0);
                AwaitingHandlersQueuedEvent = new ManualResetEventSlim(false);
            }

            if (Options.MaximumParallelWorkingHandlers != null)
                HandlersCheckpoint();
        }

        /// <inheritdoc/>
        public void Enqueue(IEnumerable<DescribedHandlerInfo> handlers)
        {
            handlers.ForEach(Enqueue);
        }

        /// <inheritdoc/>
        public void Enqueue(DescribedHandlerInfo handlerInfo)
        {
            if (Options.MaximumParallelWorkingHandlers == null)
            {
                Task.Run(async () => await ExecuteHandlerWrapper(handlerInfo));
                return;
            }

            lock (SyncObj)
            {
                AwaitingHandlersQueue.Enqueue(handlerInfo);
                HandlerEnqueued?.Invoke(handlerInfo);
                AwaitingHandlersQueuedEvent.Set();
            }
        }

        /// <inheritdoc/>
        public void Dequeue(HandlerLifetimeToken token)
        {
            if (Options.MaximumParallelWorkingHandlers == null)
                return;

            lock (SyncObj)
            {
                ExecutingHandlersPool.TryRemove(token, out _);
                ExecutingHandlersSemaphore.Release(1);
            }
        }

        /// <summary>
        /// Main checkpoint method that manages handler execution in a loop.
        /// Continuously processes queued handlers while respecting concurrency limits.
        /// </summary>
        protected virtual async void HandlersCheckpoint()
        {
            await Task.Yield();
            while (!GlobalCancellationToken.IsCancellationRequested)
            {
                if (!CanEnqueueHandler())
                {
                    await ExecutingHandlersSemaphore.WaitAsync(GlobalCancellationToken);
                    if (!CanEnqueueHandler())
                        continue;
                }

                if (!TryDequeueHandler(out DescribedHandlerInfo? enqueuedHandler))
                {
                    AwaitingHandlersQueuedEvent.Reset();
                    AwaitingHandlersQueuedEvent.Wait(GlobalCancellationToken);

                    if (!TryDequeueHandler(out enqueuedHandler))
                        continue;
                }

                if (enqueuedHandler == null)
                    continue;

                ExecuteHandler(enqueuedHandler);
            }
        }

        /// <summary>
        /// Executes a handler by creating a lifetime token and tracking the execution.
        /// </summary>
        /// <param name="enqueuedHandler">The handler to execute.</param>
        protected virtual void ExecuteHandler(DescribedHandlerInfo enqueuedHandler)
        {
            HandlerLifetimeToken lifetimeToken = enqueuedHandler.HandlerLifetime;
            lifetimeToken.OnLifetimeEnded += Dequeue;

            Task executingHandler = ExecuteHandlerWrapper(enqueuedHandler);
            lock (SyncObj)
                ExecutingHandlersPool.TryAdd(lifetimeToken, executingHandler);

            HandlerExecuting?.Invoke(enqueuedHandler);
        }

        /// <summary>
        /// Wrapper method that executes a handler and handles exceptions.
        /// </summary>
        /// <param name="enqueuedHandler">The handler to execute.</param>
        /// <returns>A task representing the asynchronous execution.</returns>
        /// <exception cref="HandlerFaultedException">Thrown when the handler execution fails.</exception>
        protected virtual async Task ExecuteHandlerWrapper(DescribedHandlerInfo enqueuedHandler)
        {
            try
            {
                await enqueuedHandler.Execute(GlobalCancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                throw new HandlerFaultedException(enqueuedHandler, ex);
            }
        }

        /// <summary>
        /// Checks if a new handler can be enqueued based on the current execution count.
        /// </summary>
        /// <returns>True if a new handler can be enqueued; otherwise, false.</returns>
        protected virtual bool CanEnqueueHandler()
        {
            lock (SyncObj)
            {
                return ExecutingHandlersPool.Count < Options.MaximumParallelWorkingHandlers;
            }
        }

        /// <summary>
        /// Attempts to dequeue a handler from the awaiting queue.
        /// </summary>
        /// <param name="enqueuedHandler">The dequeued handler, if successful.</param>
        /// <returns>True if a handler was successfully dequeued; otherwise, false.</returns>
        protected virtual bool TryDequeueHandler(out DescribedHandlerInfo? enqueuedHandler)
        {
            lock (SyncObj)
            {
                return AwaitingHandlersQueue.TryDequeue(out enqueuedHandler);
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

            if (AwaitingHandlersQueuedEvent != null)
            {
                AwaitingHandlersQueuedEvent.Dispose();
                AwaitingHandlersQueuedEvent = null!;
            }

            if (SyncObj != null)
                SyncObj = null!;

            GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}
