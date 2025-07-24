using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Base class for filters that operate on the chat of a replied message (the message being replied to).
    /// The replyDepth parameter determines how many levels up the reply chain to search for the target message.
    /// </summary>
    public abstract class RepliedMessageChatFilter : RepliedMessageFilter
    {
        /// <summary>
        /// Gets the chat of the replied message (the message being replied to at the specified depth).
        /// </summary>
        public Chat Chat { get; private set; } = null!;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageChatFilter"/> class.
        /// </summary>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        protected RepliedMessageChatFilter(int replyDepth = 1) : base(replyDepth) { }
        
        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            if (!CanPassReply(context))
                return false;

            Chat = Reply.Chat;
            return CanPassNext(context);
        }
    }

    /// <summary>
    /// Filters replied messages (the message being replied to at the specified depth) whose chat is a forum.
    /// </summary>
    public class RepliedMessageChatIsForumFilter : RepliedMessageChatFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageChatIsForumFilter"/> class.
        /// </summary>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        public RepliedMessageChatIsForumFilter(int replyDepth = 1)
            : base(replyDepth) { }

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Chat.IsForum;
    }

    /// <summary>
    /// Filters replied messages (the message being replied to at the specified depth) whose chat ID matches the specified value.
    /// </summary>
    public class RepliedMessageChatIdFilter : RepliedMessageChatFilter
    {
        private readonly long Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageChatIdFilter"/> class.
        /// </summary>
        /// <param name="id">The chat ID to match.</param>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        public RepliedMessageChatIdFilter(long id, int replyDepth = 1) : base(replyDepth) => Id = id;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Chat.Id == Id;
    }

    /// <summary>
    /// Filters replied messages (the message being replied to at the specified depth) whose chat type matches the specified value.
    /// </summary>
    public class RepliedMessageChatTypeFilter : RepliedMessageChatFilter
    {
        private readonly ChatType Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageChatTypeFilter"/> class.
        /// </summary>
        /// <param name="type">The chat type to match.</param>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        public RepliedMessageChatTypeFilter(ChatType type, int replyDepth = 1) : base(replyDepth) => Type = type;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Chat.Type == Type;
    }

    /// <summary>
    /// Filters replied messages (the message being replied to at the specified depth) whose chat title matches the specified value.
    /// </summary>
    public class RepliedMessageChatTitleFilter : RepliedMessageChatFilter
    {
        private readonly string? Title;
        private readonly StringComparison Comparison = StringComparison.InvariantCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageChatTitleFilter"/> class.
        /// </summary>
        /// <param name="title">The chat title to match.</param>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        public RepliedMessageChatTitleFilter(string? title, int replyDepth = 1) : base(replyDepth) => Title = title;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageChatTitleFilter"/> class with a specific string comparison.
        /// </summary>
        /// <param name="title">The chat title to match.</param>
        /// <param name="comparison">The string comparison to use.</param>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        public RepliedMessageChatTitleFilter(string? title, StringComparison comparison, int replyDepth = 1)
            : this(title, replyDepth) => Comparison = comparison;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
        {
            if (Chat.Title == null)
                return false;
            return Chat.Title.Equals(Title, Comparison);
        }
    }

    /// <summary>
    /// Filters replied messages (the message being replied to at the specified depth) whose chat username matches the specified value.
    /// </summary>
    public class RepliedMessageChatUsernameFilter : RepliedMessageChatFilter
    {
        private readonly string? UserName;
        private readonly StringComparison Comparison = StringComparison.InvariantCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageChatUsernameFilter"/> class.
        /// </summary>
        /// <param name="userName">The chat username to match.</param>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        public RepliedMessageChatUsernameFilter(string? userName, int replyDepth = 1) : base(replyDepth) => UserName = userName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageChatUsernameFilter"/> class with a specific string comparison.
        /// </summary>
        /// <param name="userName">The chat username to match.</param>
        /// <param name="comparison">The string comparison to use.</param>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        public RepliedMessageChatUsernameFilter(string? userName, StringComparison comparison, int replyDepth = 1)
            : this(userName, replyDepth) => Comparison = comparison;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
        {
            if (Chat.Username == null)
                return false;
            return Chat.Username.Equals(UserName, Comparison);
        }
    }

    /// <summary>
    /// Filters replied messages (the message being replied to at the specified depth) whose chat first and/or last name matches the specified values.
    /// </summary>
    public class RepliedMessageChatNameFilter : RepliedMessageChatFilter
    {
        private readonly string? FirstName;
        private readonly string? LastName;
        private readonly StringComparison Comparison = StringComparison.InvariantCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageChatNameFilter"/> class.
        /// </summary>
        /// <param name="firstName">The chat first name to match.</param>
        /// <param name="lastName">The chat last name to match.</param>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        public RepliedMessageChatNameFilter(string? firstName, string? lastName, int replyDepth = 1) : base(replyDepth)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageChatNameFilter"/> class with a specific string comparison.
        /// </summary>
        /// <param name="firstName">The chat first name to match.</param>
        /// <param name="lastName">The chat last name to match.</param>
        /// <param name="comparison">The string comparison to use.</param>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        public RepliedMessageChatNameFilter(string? firstName, string? lastName, StringComparison comparison, int replyDepth = 1)
            : this(firstName, lastName, replyDepth) => Comparison = comparison;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
        {
            if (LastName != null)
            {
                if (Chat.LastName == null)
                    return false;

                if (Chat.LastName.Equals(LastName, Comparison))
                    return false;
            }

            if (FirstName != null)
            {
                if (Chat.FirstName == null)
                    return false;

                if (Chat.FirstName.Equals(FirstName, Comparison))
                    return false;
            }

            return true;
        }
    }
}
