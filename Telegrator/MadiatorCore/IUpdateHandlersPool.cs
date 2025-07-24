using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.MadiatorCore
{
    /// <summary>
    /// Represents a delegate for when a handler is enqueued.
    /// </summary>
    /// <param name="args">The <see cref="DescribedHandlerInfo"/> for the enqueued handler.</param>
    public delegate void HandlerEnqueued(DescribedHandlerInfo args);
    /// <summary>
    /// Represents a delegate for when a handler is executing.
    /// </summary>
    /// <param name="args">The <see cref="DescribedHandlerInfo"/> for the executing handler.</param>
    public delegate void HandlerExecuting(DescribedHandlerInfo args);

    /// <summary>
    /// Provides a pool for managing the execution and queuing of update handlers.
    /// </summary>
    public interface IUpdateHandlersPool : IDisposable
    {
        /// <summary>
        /// Occurs when a handler is enqueued.
        /// </summary>
        public event HandlerEnqueued? HandlerEnqueued;
        
        /// <summary>
        /// Occurs when a handler is executing.
        /// </summary>
        public event HandlerExecuting? HandlerExecuting;

        /// <summary>
        /// Enqueues a collection of handlers for execution.
        /// </summary>
        /// <param name="handlers">The handlers to enqueue.</param>
        public void Enqueue(IEnumerable<DescribedHandlerInfo> handlers);

        /// <summary>
        /// Enqueues a single handler for execution.
        /// </summary>
        /// <param name="handlerInfo">The handler to enqueue.</param>
        public void Enqueue(DescribedHandlerInfo handlerInfo);

        /// <summary>
        /// Dequeues a handler using its lifetime token.
        /// </summary>
        /// <param name="token">The <see cref="HandlerLifetimeToken"/> of the handler to dequeue.</param>
        public void Dequeue(HandlerLifetimeToken token);
    }
}
