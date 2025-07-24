using Telegrator.Filters;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute for filtering messages with reply to messages of this bot.
    /// </summary>
    public class MeRepliedAttribute()
        : MessageFilterAttribute(new MeRepliedFilter())
    { }

    /// <summary>
    /// Attribute for filtering messages in reply chain.
    /// </summary>
    public class MessageRepliedAttribute(int replyDepth = 1)
        : MessageFilterAttribute(new MessageRepliedFilter(replyDepth))
    { }
}
