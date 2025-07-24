using Telegram.Bot.Types.Enums;
using Telegrator.Handlers.Components;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.MadiatorCore
{
    /// <summary>
    /// Collection class for managing handler descriptors organized by update type.
    /// Provides functionality for collecting, adding, and organizing handlers.
    /// </summary>
    public interface IHandlersCollection
    {
        /// <summary>
        /// Gets the collection of <see cref="UpdateType"/>'s allowed by registered handlers
        /// </summary>
        public IEnumerable<UpdateType> AllowedTypes { get; }

        /// <summary>
        /// Gets the collection of <see cref="UpdateType"/> keys for the handler lists.
        /// </summary>
        public IEnumerable<UpdateType> Keys { get; }
        
        /// <summary>
        /// Gets the collection of <see cref="HandlerDescriptorList"/> values.
        /// </summary>
        public IEnumerable<HandlerDescriptorList> Values { get; }
        
        /// <summary>
        /// Gets the <see cref="HandlerDescriptorList"/> for the specified <see cref="UpdateType"/>.
        /// </summary>
        /// <param name="updateType">The update type key.</param>
        /// <returns>The handler descriptor list for the given update type.</returns>
        public HandlerDescriptorList this[UpdateType updateType] { get; }

        /// <summary>
        /// Collects all handlers domain-wide and returns a new <see cref="IHandlersCollection"/>.
        /// </summary>
        /// <returns>A new <see cref="IHandlersCollection"/> with all handlers collected.</returns>
        public IHandlersCollection CollectHandlersDomainWide();

        /// <summary>
        /// Adds a <see cref="HandlerDescriptor"/> to the collection and returns the updated collection.
        /// </summary>
        /// <param name="descriptor">The handler descriptor to add.</param>
        /// <returns>The updated <see cref="IHandlersCollection"/>.</returns>
        public IHandlersCollection AddDescriptor(HandlerDescriptor descriptor);

        /// <summary>
        /// Adds a handler of the specified type to the collection and returns the updated collection.
        /// </summary>
        /// <typeparam name="THandler">The type of handler to add, must inherit from <see cref="UpdateHandlerBase"/>.</typeparam>
        /// <returns>The updated <see cref="IHandlersCollection"/>.</returns>
        public IHandlersCollection AddHandler<THandler>() where THandler : UpdateHandlerBase;

        /// <summary>
        /// Adds a handler of the specified type to the collection and returns the updated collection.
        /// </summary>
        /// <param name="handlerType">The type of handler to add.</param>
        /// <returns>The updated <see cref="IHandlersCollection"/>.</returns>
        /// <exception cref="Exception">Thrown if the handler type is invalid.</exception>
        public IHandlersCollection AddHandler(Type handlerType);

        /// <summary>
        /// Gets the <see cref="HandlerDescriptorList"/> for the specified <see cref="HandlerDescriptor"/>.
        /// </summary>
        /// <param name="descriptor">The handler descriptor.</param>
        /// <returns>The handler descriptor list containing the descriptor.</returns>
        public HandlerDescriptorList GetDescriptorList(HandlerDescriptor descriptor);
    }
}
