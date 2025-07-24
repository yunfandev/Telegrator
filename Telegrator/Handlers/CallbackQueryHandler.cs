using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;

namespace Telegrator.Handlers
{
    /// <summary>
    /// Attribute that marks a handler to process callback query updates.
    /// This handler will be triggered when users interact with inline keyboards or other callback mechanisms.
    /// </summary>
    /// <param name="concurrency">The maximum number of concurrent executions allowed (default: 0 for unlimited).</param>
    public sealed class CallbackQueryHandlerAttribute(int concurrency = 0) : UpdateHandlerAttribute<CallbackQueryHandler>(UpdateType.CallbackQuery, concurrency)
    {
        /// <summary>
        /// Always returns true, allowing any callback query update to pass through this filter.
        /// </summary>
        /// <param name="context">The filter execution context (unused).</param>
        /// <returns>Always returns true to allow any callback query update.</returns>
        public override bool CanPass(FilterExecutionContext<Update> context) => true;
    }

    /// <summary>
    /// Abstract base class for handlers that process callback query updates.
    /// Provides a foundation for creating handlers that respond to user interactions with inline keyboards.
    /// </summary>
    public abstract class CallbackQueryHandler() : AbstractUpdateHandler<CallbackQuery>(UpdateType.CallbackQuery)
    {
        /// <summary>
        /// Gets the type-specific data from the callback query.
        /// Returns the data string, chat instance, or game short name depending on the callback query type.
        /// </summary>
        protected string TypeData
        {
            get => Input switch
            {
                { Data: { } data } => data,
                { ChatInstance: { } chatInstance } => chatInstance,
                { GameShortName: { } gameShortName } => gameShortName
            };
        }
    }
}
