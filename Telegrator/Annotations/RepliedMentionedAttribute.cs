using Telegram.Bot.Types.Enums;
using Telegrator.Filters;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute for filtering messages that are replies to messages containing mentions.
    /// Allows handlers to respond to messages that reply to messages with specific mentions.
    /// </summary>
    public class RepliedMentionedAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the RepliedMentionedAttribute that matches replies to any mention.
        /// </summary>
        /// <param name="replyDepth">The depth of the reply chain to check (default: 1).</param>
        public RepliedMentionedAttribute(int replyDepth = 1)
            : base(new RepliedMessageHasEntityFilter(MessageEntityType.Mention, 0, null, replyDepth), new RepliedMentionedFilter(replyDepth)) { }

        /// <summary>
        /// Initializes a new instance of the RepliedMentionedAttribute that matches replies to mentions at a specific offset.
        /// </summary>
        /// <param name="offset">The offset position where the mention should occur in the replied message.</param>
        /// <param name="replyDepth">The depth of the reply chain to check (default: 1).</param>
        public RepliedMentionedAttribute(int offset, int replyDepth = 1)
            : base(new RepliedMessageHasEntityFilter(MessageEntityType.Mention, offset, null, replyDepth), new RepliedMentionedFilter(replyDepth)) { }

        /// <summary>
        /// Initializes a new instance of the RepliedMentionedAttribute that matches replies to a specific mention.
        /// </summary>
        /// <param name="mention">The specific mention text to match in the replied message.</param>
        /// <param name="replyDepth">The depth of the reply chain to check (default: 1).</param>
        public RepliedMentionedAttribute(string mention, int replyDepth = 1)
            : base(new RepliedMessageHasEntityFilter(MessageEntityType.Mention, replyDepth), new RepliedMentionedFilter(mention, replyDepth)) { }

        /// <summary>
        /// Initializes a new instance of the RepliedMentionedAttribute that matches replies to a specific mention at a specific offset.
        /// </summary>
        /// <param name="mention">The specific mention text to match in the replied message.</param>
        /// <param name="offset">The offset position where the mention should occur in the replied message.</param>
        /// <param name="replyDepth">The depth of the reply chain to check (default: 1).</param>
        public RepliedMentionedAttribute(string mention, int offset, int replyDepth = 1)
            : base(new RepliedMessageHasEntityFilter(MessageEntityType.Mention, offset, null, replyDepth), new RepliedMentionedFilter(mention, replyDepth)) { }
    }
}
