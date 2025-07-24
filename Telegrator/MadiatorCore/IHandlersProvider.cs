using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Handlers.Components;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.MadiatorCore
{
    /// <summary>
    /// Provides methods to retrieve and describe handler information for updates.
    /// </summary>
    public interface IHandlersProvider
    {
        /// <summary>
        /// Gets the collection of <see cref="UpdateType"/>'s allowed by registered handlers
        /// </summary>
        public IEnumerable<UpdateType> AllowedTypes { get; }

        /// <summary>
        /// Gets the handlers for the specified update and context.
        /// </summary>
        /// <param name="updateRouter">The update router.</param>
        /// <param name="client">The Telegram bot client.</param>
        /// <param name="update">The update to handle.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An enumerable of described handler info.</returns>
        public IEnumerable<DescribedHandlerInfo> GetHandlers(IUpdateRouter updateRouter, ITelegramBotClient client, Update update, CancellationToken cancellationToken = default);

        /// <summary>
        /// Describes all handler descriptors in the list for the given context.
        /// </summary>
        /// <param name="descriptors">The handler descriptor list.</param>
        /// <param name="updateRouter">The update router.</param>
        /// <param name="client">The Telegram bot client.</param>
        /// <param name="update">The update to handle.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An enumerable of described handler info.</returns>
        public IEnumerable<DescribedHandlerInfo> DescribeDescriptors(HandlerDescriptorList descriptors, IUpdateRouter updateRouter, ITelegramBotClient client, Update update, CancellationToken cancellationToken = default);

        /// <summary>
        /// Describes a single handler descriptor for the given context.
        /// </summary>
        /// <param name="descriptor">The handler descriptor.</param>
        /// <param name="updateRouter">The update router.</param>
        /// <param name="client">The Telegram bot client.</param>
        /// <param name="update">The update to handle.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The described handler info, or null if not applicable.</returns>
        public DescribedHandlerInfo? DescribeHandler(HandlerDescriptor descriptor, IUpdateRouter updateRouter, ITelegramBotClient client, Update update, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an instance of the handler for the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The handler descriptor.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The handler instance.</returns>
        public UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the list of bot commands supported by the provider.
        /// </summary>
        /// <returns>An enumerable of bot commands.</returns>
        public IEnumerable<BotCommand> GetBotCommands(CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether the provider contains any handlers.
        /// </summary>
        /// <returns>True if the provider is empty; otherwise, false.</returns>
        public bool IsEmpty();
    }
}
