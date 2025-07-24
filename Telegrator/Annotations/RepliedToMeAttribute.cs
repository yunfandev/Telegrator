using Telegrator.Filters;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute for filtering messages with reply to messages of this bot.
    /// </summary>
    public class RepliedToMeAttribute()
        : MessageFilterAttribute(new RepliedToMeFilter())
    { }
}
