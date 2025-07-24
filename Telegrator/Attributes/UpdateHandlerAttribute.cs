using Telegram.Bot.Types.Enums;
using Telegrator.Attributes.Components;
using Telegrator.Handlers.Components;

namespace Telegrator.Attributes
{
    /// <summary>
    /// Abstract base attribute for marking update handler classes.
    /// Provides a type-safe way to associate handler types with specific update types and concurrency settings.
    /// </summary>
    /// <typeparam name="T">The type of the update handler that this attribute is applied to.</typeparam>
    /// <param name="updateType">The type of update that this handler can process.</param>
    /// <param name="concurrency">The concurrency level for this handler (default: 0 for unlimited).</param>
    public abstract class UpdateHandlerAttribute<T>(UpdateType updateType, int concurrency = 0)
        : UpdateHandlerAttributeBase([typeof(T)], updateType, concurrency) where T : UpdateHandlerBase
    {
    }
}
