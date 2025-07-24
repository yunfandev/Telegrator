using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator;
using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Abstract base class for filters that operate on replied messages.
    /// Provides functionality to traverse reply chains and access replied message content.
    /// </summary>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public abstract class RepliedMessageFilter(int replyDepth = 1) : Filter<Message>
    {
        /// <summary>
        /// Gets the replied message at the specified depth in the reply chain.
        /// </summary>
        public Message Reply { get; private set; } = null!;
        
        /// <summary>
        /// Gets the depth of reply chain traversal.
        /// </summary>
        public int ReplyDepth { get; private set; } = replyDepth;
        
        /// <summary>
        /// Validates that the message has a valid reply chain at the specified depth.
        /// </summary>
        /// <param name="context">The filter execution context containing the message.</param>
        /// <returns>True if the reply chain is valid at the specified depth; otherwise, false.</returns>
        protected bool CanPassReply(FilterExecutionContext<Message> context)
        {
            Message reply = context.Input;
            for (int i = ReplyDepth; i > 0; i--)
            {
                if (reply.ReplyToMessage is not { Id: > 0 } replyMessage)
                    return false;

                reply = replyMessage;
            }

            Reply = reply;
            return true;
        }

        /// <summary>
        /// Determines if the message can pass through the filter by first validating
        /// the reply chain and then applying specific filter logic.
        /// </summary>
        /// <param name="context">The filter execution context containing the message.</param>
        /// <returns>True if the message passes both reply validation and specific filter criteria; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            if (!CanPassReply(context))
                return false;

            return CanPassNext(context);
        }

        /// <summary>
        /// Abstract method that must be implemented by derived classes to perform
        /// specific filtering logic on the replied message.
        /// </summary>
        /// <param name="context">The filter execution context.</param>
        /// <returns>True if the replied message passes the specific filter criteria; otherwise, false.</returns>
        protected abstract bool CanPassNext(FilterExecutionContext<Message> context);
    }

    /// <summary>
    /// Filter that checks if a message is a reply to another message at the specified depth.
    /// </summary>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class MessageRepliedFilter(int replyDepth = 1) : RepliedMessageFilter(replyDepth)
    {
        /// <summary>
        /// Always returns true if the reply chain is valid at the specified depth.
        /// </summary>
        /// <param name="context">The filter execution context (unused).</param>
        /// <returns>True if the reply chain is valid; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
            => true;
    }

    /// <summary>
    /// Filter that checks if the replied message was sent by the bot itself.
    /// </summary>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class MeRepliedFilter(int replyDepth = 1) : RepliedMessageFilter(replyDepth)
    {
        /// <summary>
        /// Checks if the replied message was sent by the bot.
        /// </summary>
        /// <param name="context">The filter execution context containing bot information.</param>
        /// <returns>True if the replied message was sent by the bot; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
            => context.BotInfo.User == Reply.From;
    }

    /// <summary>
    /// Filter that checks if the replied message has non-empty text content.
    /// </summary>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class RepliedTextNotNullOrEmptyFilter(int replyDepth = 1) : RepliedMessageFilter(replyDepth)
    {
        /// <summary>
        /// Checks if the replied message text is not null or empty.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the replied message has non-empty text; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => !string.IsNullOrEmpty(Reply.Text);
    }

    /// <summary>
    /// Filter that checks if the replied message is of a specific type.
    /// </summary>
    /// <param name="type">The message type to check for.</param>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class RepliedMessageTypeFilter(MessageType type, int replyDepth = 1) : RepliedMessageFilter(replyDepth)
    {
        /// <summary>
        /// Checks if the replied message is of the specified type.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the replied message is of the specified type; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Reply.Type == type;
    }

    /// <summary>
    /// Filter that checks if the replied message is an automatic forward.
    /// </summary>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class RepliedIsAutomaticFormwardMessageFilter(int replyDepth = 1) : RepliedMessageFilter(replyDepth)
    {
        /// <summary>
        /// Checks if the replied message is an automatic forward.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the replied message is an automatic forward; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Reply.IsAutomaticForward;
    }

    /// <summary>
    /// Filter that checks if the replied message is from an offline user.
    /// </summary>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class RepliedIsFromOfflineMessageFilter(int replyDepth = 1) : RepliedMessageFilter(replyDepth)
    {
        /// <summary>
        /// Checks if the replied message is from an offline user.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the replied message is from an offline user; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Reply.IsFromOffline;
    }

    /// <summary>
    /// Filter that checks if the replied message is a service message.
    /// </summary>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class RepliedIsServiceMessageMessageFilter(int replyDepth = 1) : RepliedMessageFilter(replyDepth)
    {
        /// <summary>
        /// Checks if the replied message is a service message.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the replied message is a service message; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Reply.IsServiceMessage;
    }

    /// <summary>
    /// Filter that checks if the replied message is a topic message.
    /// </summary>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class RepliedIsTopicMessageMessageFilter(int replyDepth = 1) : RepliedMessageFilter(replyDepth)
    {
        /// <summary>
        /// Checks if the replied message is a topic message.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the replied message is a topic message; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Reply.IsTopicMessage;
    }

    /// <summary>
    /// Filter that checks if the replied message contains a dice with a specific value.
    /// </summary>
    /// <param name="value">The dice value to check for.</param>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class RepliedDiceThrowedFilter(int value, int replyDepth = 1) : RepliedMessageFilter(replyDepth)
    {
        /// <summary>
        /// The dice type to check for (optional).
        /// </summary>
        private readonly DiceType? Dice = null;
        
        /// <summary>
        /// The dice value to check for.
        /// </summary>
        private readonly int Value = value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedDiceThrowedFilter"/> class with a specific dice type and value.
        /// </summary>
        /// <param name="diceType">The dice type to check for.</param>
        /// <param name="value">The dice value to check for.</param>
        /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
        public RepliedDiceThrowedFilter(DiceType diceType, int value, int replyDepth = 1)
            : this(value, replyDepth) => Dice = diceType;

        /// <summary>
        /// Checks if the replied message contains a dice with the specified value and optionally the specified type.
        /// </summary>
        /// <param name="context">The filter execution context containing the message.</param>
        /// <returns>True if the replied message contains a dice with the specified criteria; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
        {
            if (context.Input.Dice == null)
                return false;

            if (Dice != null && context.Input.Dice.Emoji != GetEmojyForDiceType(Dice))
                return false;

            return context.Input.Dice.Value == Value;
        }

        /// <summary>
        /// Gets the emoji representation for a specific dice type.
        /// </summary>
        /// <param name="diceType">The dice type to get the emoji for.</param>
        /// <returns>The emoji string for the dice type, or null if not found.</returns>
        private static string? GetEmojyForDiceType(DiceType? diceType) => diceType switch
        {
            DiceType.Dice => "🎲",
            DiceType.Darts => "🎯",
            DiceType.Bowling => "🎳",
            DiceType.Basketball => "🏀",
            DiceType.Football => "⚽",
            DiceType.Casino => "🎰",
            _ => null
        };
    }

    /// <summary>
    /// Filter that checks if the replied message contains specific message entities.
    /// </summary>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class RepliedMessageHasEntityFilter(int replyDepth = 1) : RepliedMessageFilter(replyDepth)
    {
        /// <summary>
        /// The string comparison type to use for content matching.
        /// </summary>
        private readonly StringComparison _stringComparison = StringComparison.CurrentCulture;
        
        /// <summary>
        /// The entity type to filter by (optional).
        /// </summary>
        private readonly MessageEntityType? EntityType;
        
        /// <summary>
        /// The content to match in the entity (optional).
        /// </summary>
        private readonly string? Content;
        
        /// <summary>
        /// The offset position to check (optional).
        /// </summary>
        private readonly int? Offset;
        
        /// <summary>
        /// The length to check (optional).
        /// </summary>
        private readonly int? Length;

        /// <summary>
        /// Gets the found entities that match the filter criteria.
        /// </summary>
        public MessageEntity[]? FoundEntities { get; set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageHasEntityFilter"/> class with a specific entity type.
        /// </summary>
        /// <param name="type">The entity type to filter by.</param>
        /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
        public RepliedMessageHasEntityFilter(MessageEntityType type, int replyDepth = 1) : this(replyDepth)
        {
            EntityType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageHasEntityFilter"/> class with position criteria.
        /// </summary>
        /// <param name="type">The entity type to filter by.</param>
        /// <param name="offset">The offset position to check.</param>
        /// <param name="length">The length to check (optional).</param>
        /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
        public RepliedMessageHasEntityFilter(MessageEntityType type, int offset, int? length, int replyDepth = 1) : this(replyDepth)
        {
            EntityType = type;
            Offset = offset;
            Length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageHasEntityFilter"/> class with content criteria.
        /// </summary>
        /// <param name="type">The entity type to filter by.</param>
        /// <param name="content">The content to match in the entity.</param>
        /// <param name="stringComparison">The string comparison type to use.</param>
        /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
        public RepliedMessageHasEntityFilter(MessageEntityType type, string content, StringComparison stringComparison = StringComparison.CurrentCulture, int replyDepth = 1) : this(replyDepth)
        {
            EntityType = type;
            Content = content;
            _stringComparison = stringComparison;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageHasEntityFilter"/> class with all criteria.
        /// </summary>
        /// <param name="type">The entity type to filter by.</param>
        /// <param name="offset">The offset position to check.</param>
        /// <param name="length">The length to check (optional).</param>
        /// <param name="content">The content to match in the entity.</param>
        /// <param name="stringComparison">The string comparison type to use.</param>
        /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
        public RepliedMessageHasEntityFilter(MessageEntityType type, int offset, int? length, string content, StringComparison stringComparison = StringComparison.CurrentCulture, int replyDepth = 1) : this(replyDepth)
        {
            EntityType = type;
            Offset = offset;
            Length = length;
            Content = content;
            _stringComparison = stringComparison;
        }

        /// <summary>
        /// Checks if the replied message contains entities that match the specified criteria.
        /// </summary>
        /// <param name="context">The filter execution context containing the message.</param>
        /// <returns>True if matching entities are found; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
        {
            if (context.Input is not { Entities.Length: > 0 })
                return false;

            FoundEntities = context.Input.Entities.Where(entity => FilterEntity(context.Input.Text, entity)).ToArray();
            return FoundEntities.Length != 0;
        }

        /// <summary>
        /// Filters an entity based on the specified criteria.
        /// </summary>
        /// <param name="text">The message text containing the entity.</param>
        /// <param name="entity">The entity to filter.</param>
        /// <returns>True if the entity matches all specified criteria; otherwise, false.</returns>
        private bool FilterEntity(string? text, MessageEntity entity)
        {
            if (EntityType != null && entity.Type != EntityType)
                return false;

            if (Offset != null && entity.Offset != Offset)
                return false;

            if (Length != null && entity.Length != Length)
                return false;

            if (Content != null)
            {
                if (text is not { Length: > 0 })
                    return false;

                if (!text.Substring(entity.Offset, entity.Length).Equals(Content, _stringComparison))
                    return false;
            }

            return true;
        }
    }
}
