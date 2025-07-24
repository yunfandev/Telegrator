using Telegram.Bot.Types;
using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Abstract base class for filters that operate on the sender of replied messages.
    /// Provides functionality to access and validate the user who sent the replied message.
    /// </summary>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public abstract class RepliedMessageSenderFilter(int replyDepth = 1) : RepliedMessageFilter(replyDepth)
    {
        /// <summary>
        /// Gets the user who sent the replied message.
        /// </summary>
        public User User { get; private set; } = null!;

        /// <summary>
        /// Determines if the message can pass through the filter by validating the reply chain
        /// and ensuring the replied message has a valid sender.
        /// </summary>
        /// <param name="context">The filter execution context containing the message.</param>
        /// <returns>True if the reply chain is valid and has a sender; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            if (!CanPassReply(context))
                return false;

            if (Reply.From is not { Id: > 0 } from)
                return false;

            User = from;
            return CanPassNext(context);
        }
    }

    /// <summary>
    /// Filter that checks if the replied message sender has a specific username.
    /// </summary>
    /// <param name="username">The username to check for.</param>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class RepliedUsernameFilter(string username, int replyDepth = 1) : RepliedMessageSenderFilter(replyDepth)
    {
        /// <summary>
        /// The username to check for.
        /// </summary>
        private readonly string _username = username;
        
        /// <summary>
        /// The string comparison type to use for username matching.
        /// </summary>
        private readonly StringComparison _comparison = StringComparison.InvariantCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedUsernameFilter"/> class with custom string comparison.
        /// </summary>
        /// <param name="username">The username to check for.</param>
        /// <param name="comparison">The string comparison type to use.</param>
        /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
        public RepliedUsernameFilter(string username, StringComparison comparison, int replyDepth = 1)
            : this(username, replyDepth) => _comparison = comparison;

        /// <summary>
        /// Checks if the replied message sender has the specified username.
        /// </summary>
        /// <param name="context">The filter execution context (unused).</param>
        /// <returns>True if the sender has the specified username; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
            => User.Username != null && User.Username.Equals(_username, _comparison);
    }

    /// <summary>
    /// Filter that checks if the replied message sender has specific first and/or last name.
    /// </summary>
    /// <param name="firstName">The first name to check for.</param>
    /// <param name="lastName">The last name to check for (optional).</param>
    /// <param name="comparison">The string comparison type to use.</param>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class RepliedUserFilter(string firstName, string? lastName, StringComparison comparison, int replyDepth = 1) : RepliedMessageSenderFilter(replyDepth)
    {
        /// <summary>
        /// The first name to check for.
        /// </summary>
        private readonly string _firstName = firstName;
        
        /// <summary>
        /// The last name to check for (optional).
        /// </summary>
        private readonly string? _lastName = lastName;
        
        /// <summary>
        /// The string comparison type to use for name matching.
        /// </summary>
        private readonly StringComparison _comparison = comparison;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedUserFilter"/> class with first and last name.
        /// </summary>
        /// <param name="firstName">The first name to check for.</param>
        /// <param name="lastName">The last name to check for.</param>
        /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
        public RepliedUserFilter(string firstName, string lastName, int replyDepth = 1)
            : this(firstName, lastName, StringComparison.InvariantCulture, replyDepth) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedUserFilter"/> class with first name only.
        /// </summary>
        /// <param name="firstName">The first name to check for.</param>
        /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
        public RepliedUserFilter(string firstName, int replyDepth = 1)
            : this(firstName, null, StringComparison.InvariantCulture, replyDepth) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedUserFilter"/> class with first name and custom comparison.
        /// </summary>
        /// <param name="firstName">The first name to check for.</param>
        /// <param name="comparison">The string comparison type to use.</param>
        /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
        public RepliedUserFilter(string firstName, StringComparison comparison, int replyDepth = 1)
            : this(firstName, null, comparison, replyDepth) { }

        /// <summary>
        /// Checks if the replied message sender has the specified first and/or last name.
        /// </summary>
        /// <param name="context">The filter execution context (unused).</param>
        /// <returns>True if the sender has the specified name(s); otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
        {
            if (User.LastName != null)
            {
                if (_lastName == null)
                    return false;

                if (!_firstName.Equals(User.LastName, _comparison))
                    return false;
            }

            return User.FirstName.Equals(_firstName, _comparison);
        }
    }

    /// <summary>
    /// Filter that checks if the replied message sender has a specific user ID.
    /// </summary>
    /// <param name="userId">The user ID to check for.</param>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class RepliedUserIdFilter(long userId, int replyDepth = 1) : RepliedMessageSenderFilter(replyDepth)
    {
        /// <summary>
        /// The user ID to check for.
        /// </summary>
        private readonly long _userId = userId;

        /// <summary>
        /// Checks if the replied message sender has the specified user ID.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the sender has the specified user ID; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => User.Id == _userId;
    }

    /// <summary>
    /// Filter that checks if the replied message was sent by a bot.
    /// </summary>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class ReplyFromBotFilter(int replyDepth = 1) : RepliedMessageSenderFilter(replyDepth)
    {
        /// <summary>
        /// Checks if the replied message was sent by a bot.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the replied message was sent by a bot; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => User.IsBot;
    }

    /// <summary>
    /// Filter that checks if the replied message was sent by a premium user.
    /// </summary>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class ReplyFromPremiumUserFilter(int replyDepth = 1) : RepliedMessageSenderFilter(replyDepth)
    {
        /// <summary>
        /// Checks if the replied message was sent by a premium user.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the replied message was sent by a premium user; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => User.IsPremium;
    }
}
