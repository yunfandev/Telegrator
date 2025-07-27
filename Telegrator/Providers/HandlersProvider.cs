using System.Collections.ObjectModel;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Configuration;
using Telegrator.Handlers.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Providers
{
    /// <summary>
    /// Provides handler resolution and instantiation logic for Telegram bot updates.
    /// Responsible for mapping update types to handler descriptors, filtering handlers based on update context,
    /// and creating handler instances with appropriate lifecycle management.
    /// </summary>
    public class HandlersProvider : IHandlersProvider
    {
        /// <inheritdoc/>
        public IEnumerable<UpdateType> AllowedTypes { get; }

        /// <summary>
        /// Read-only dictionary mapping <see cref="UpdateType"/> to lists of handler descriptors.
        /// Each descriptor list is frozen to prevent modification after initialization.
        /// </summary>
        public readonly ReadOnlyDictionary<UpdateType, HandlerDescriptorList> HandlersDictionary;

        /// <summary>
        /// Configuration options for the bot and handler execution behavior.
        /// </summary>
        protected readonly TelegramBotOptions Options;

        /// <summary>
        /// Initializes a new instance of <see cref="HandlersProvider"/> with the specified handler collections and configuration.
        /// </summary>
        /// <param name="handlers">Collection of handler descriptor lists organized by update type</param>
        /// <param name="options">Configuration options for the bot and handler execution</param>
        /// <exception cref="ArgumentNullException">Thrown when options or botInfo is null</exception>
        public HandlersProvider(IHandlersCollection handlers, TelegramBotOptions options)
        {
            AllowedTypes = handlers.AllowedTypes;
            HandlersDictionary = handlers.Values.ForEach(list => list.Freeze()).ToReadOnlyDictionary(list => list.HandlingType);
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HandlersProvider"/> with the specified handler collections and configuration.
        /// </summary>
        /// <param name="handlers">Collection of handler descriptor lists organized by update type</param>
        /// <param name="options">Configuration options for the bot and handler execution</param>
        /// <exception cref="ArgumentNullException">Thrown when options or botInfo is null</exception>
        public HandlersProvider(IEnumerable<HandlerDescriptorList> handlers, TelegramBotOptions options)
        {
            AllowedTypes = Update.AllTypes;
            HandlersDictionary = handlers.ForEach(list => list.Freeze()).ToReadOnlyDictionary(list => list.HandlingType);
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc/>
        /// <exception cref="Exception">Thrown when the descriptor type is not recognized</exception>
        public virtual UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            switch (descriptor.Type)
            {
                case DescriptorType.Implicit:
                case DescriptorType.Singleton:
                    {
                        return descriptor.SingletonInstance ??= (descriptor.InstanceFactory != null
                            ? descriptor.SingletonInstance = descriptor.InstanceFactory.Invoke()
                            : descriptor.SingletonInstance = (UpdateHandlerBase)Activator.CreateInstance(descriptor.HandlerType, [descriptor.UpdateType]));
                    }

                case DescriptorType.Keyed:
                case DescriptorType.General:
                    {
                        return descriptor.InstanceFactory == null
                            ? (UpdateHandlerBase)Activator.CreateInstance(descriptor.HandlerType, [descriptor.UpdateType])
                            : descriptor.InstanceFactory.Invoke();
                    }

                default:
                    throw new Exception();
            }
        }

        /// <inheritdoc/>
        public virtual bool TryGetDescriptorList(UpdateType updateType, out HandlerDescriptorList? list)
        {
            return HandlersDictionary.TryGetValue(updateType, out list);
        }

        /// <inheritdoc/>
        public virtual bool IsEmpty()
        {
            return HandlersDictionary.Count == 0;
        }
    }
}
