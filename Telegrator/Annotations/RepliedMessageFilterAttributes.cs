using Telegram.Bot.Types.Enums;
using Telegrator.Filters;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute for filtering messages that are replies to other messages.
    /// </summary>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class MessageRepliedAttribute(int replyDepth = 1)
        : MessageFilterAttribute(new MessageRepliedFilter(replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering messages where the replied-to message contains dice throws with specific values.
    /// </summary>
    public class RepliedDiceThrowedAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message contains a dice throw with a specific value.
        /// </summary>
        /// <param name="value">The dice value to match</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedDiceThrowedAttribute(int value, int replyDepth = 1)
            : base(new RepliedDiceThrowedFilter(value, replyDepth)) { }

        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message contains a dice throw with a specific type and value.
        /// </summary>
        /// <param name="diceType">The type of dice</param>
        /// <param name="value">The dice value to match</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedDiceThrowedAttribute(DiceType diceType, int value, int replyDepth = 1)
            : base(new RepliedDiceThrowedFilter(diceType, value, replyDepth)) { }
    }

    /// <summary>
    /// Attribute for filtering messages where the replied-to message was automatically forwarded.
    /// </summary>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class RepliedIsAutomaticFormwardMessageAttribute(int replyDepth = 1)
        : MessageFilterAttribute(new RepliedIsAutomaticFormwardMessageFilter(replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering messages where the replied-to message was sent while the user was offline.
    /// </summary>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class RepliedIsFromOfflineMessageAttribute(int replyDepth = 1)
        : MessageFilterAttribute(new RepliedIsFromOfflineMessageFilter(replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering messages where the replied-to message is a service message.
    /// </summary>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class RepliedIsServiceMessageMessageAttribute(int replyDepth = 1)
        : MessageFilterAttribute(new RepliedIsServiceMessageMessageFilter(replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering messages where the replied-to message is a topic message in forum chats.
    /// </summary>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class RepliedIsTopicMessageMessageAttribut(int replyDepth = 1)
        : MessageFilterAttribute(new RepliedIsServiceMessageMessageFilter(replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering messages based on entities in the replied-to message.
    /// </summary>
    public class RepliedMessageHasEntityAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message has a specific entity type.
        /// </summary>
        /// <param name="type">The entity type to match</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedMessageHasEntityAttribute(MessageEntityType type, int replyDepth = 1)
            : base(new RepliedMessageHasEntityFilter(type, replyDepth)) { }

        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message has a specific entity type at a specific position.
        /// </summary>
        /// <param name="type">The entity type to match</param>
        /// <param name="offset">The starting position of the entity</param>
        /// <param name="length">The length of the entity (optional)</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedMessageHasEntityAttribute(MessageEntityType type, int offset, int? length, int replyDepth = 1)
            : base(new RepliedMessageHasEntityFilter(type, offset, length, replyDepth)) { }

        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message has a specific entity type with specific content.
        /// </summary>
        /// <param name="type">The entity type to match</param>
        /// <param name="content">The content that the entity should contain</param>
        /// <param name="stringComparison">The string comparison method</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedMessageHasEntityAttribute(MessageEntityType type, string content, StringComparison stringComparison = StringComparison.CurrentCulture, int replyDepth = 1)
            : base(new RepliedMessageHasEntityFilter(type, content, stringComparison, replyDepth)) { }

        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message has a specific entity type at a specific position with specific content.
        /// </summary>
        /// <param name="type">The entity type to match</param>
        /// <param name="offset">The starting position of the entity</param>
        /// <param name="length">The length of the entity (optional)</param>
        /// <param name="content">The content that the entity should contain</param>
        /// <param name="stringComparison">The string comparison method</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedMessageHasEntityAttribute(MessageEntityType type, int offset, int? length, string content, StringComparison stringComparison = StringComparison.CurrentCulture, int replyDepth = 1)
            : base(new RepliedMessageHasEntityFilter(type, offset, length, content, stringComparison, replyDepth)) { }
    }
}
