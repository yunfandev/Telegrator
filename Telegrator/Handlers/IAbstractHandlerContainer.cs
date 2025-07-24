using Telegrator.Handlers.Components;

namespace Telegrator.Handlers
{
    /// <summary>
    /// Represents a handler container for a specific update type.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update handled by the container.</typeparam>
    public interface IAbstractHandlerContainer<TUpdate> : IHandlerContainer where TUpdate : class
    {
        /// <summary>
        /// Gets the actual update object of type <typeparamref name="TUpdate"/>.
        /// </summary>
        public TUpdate ActualUpdate { get; }
    }
}
