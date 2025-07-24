using Telegrator.Filters;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute for filtering messages based on the username of the sender of the replied-to message.
    /// </summary>
    public class RepliedFromUsernameAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message is from a specific username.
        /// </summary>
        /// <param name="username">The username to match</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedFromUsernameAttribute(string username, int replyDepth = 1)
            : base(new RepliedUsernameFilter(username, replyDepth)) { }

        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message is from a specific username with custom comparison.
        /// </summary>
        /// <param name="username">The username to match</param>
        /// <param name="comparison">The string comparison method</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedFromUsernameAttribute(string username, StringComparison comparison, int replyDepth = 1)
            : base(new RepliedUsernameFilter(username, comparison, replyDepth)) { }
    }

    /// <summary>
    /// Attribute for filtering messages based on the name of the sender of the replied-to message.
    /// </summary>
    public class RepliedFromUserAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message is from a user with specific names.
        /// </summary>
        /// <param name="firstName">The first name to match</param>
        /// <param name="lastName">The last name to match (optional)</param>
        /// <param name="comparison">The string comparison method</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedFromUserAttribute(string firstName, string? lastName, StringComparison comparison, int replyDepth = 1)
            : base(new RepliedUserFilter(firstName, lastName, comparison, replyDepth)) { }

        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message is from a user with specific names.
        /// </summary>
        /// <param name="firstName">The first name to match</param>
        /// <param name="lastName">The last name to match</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedFromUserAttribute(string firstName, string lastName, int replyDepth = 1)
            : base(new RepliedUserFilter(firstName, lastName, StringComparison.InvariantCulture, replyDepth)) { }

        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message is from a user with a specific first name.
        /// </summary>
        /// <param name="firstName">The first name to match</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedFromUserAttribute(string firstName, int replyDepth = 1)
            : base(new RepliedUserFilter(firstName, null, StringComparison.InvariantCulture, replyDepth)) { }

        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message is from a user with a specific first name and custom comparison.
        /// </summary>
        /// <param name="firstName">The first name to match</param>
        /// <param name="comparison">The string comparison method</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedFromUserAttribute(string firstName, StringComparison comparison, int replyDepth = 1)
            : base(new RepliedUserFilter(firstName, null, comparison, replyDepth)) { }
    }

    /// <summary>
    /// Attribute for filtering messages based on the user ID of the sender of the replied-to message.
    /// </summary>
    /// <param name="userId">The user ID to match</param>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class RepliedUserIdAttribute(long userId, int replyDepth = 1)
        : MessageFilterAttribute(new RepliedUserIdFilter(userId, replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering messages where the replied-to message was sent by a bot.
    /// </summary>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class ReplyFromBotAttribute(int replyDepth = 1)
        : MessageFilterAttribute(new ReplyFromBotFilter(replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering messages where the replied-to message was sent by a premium user.
    /// </summary>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class ReplyFromPremiumUserAttribute(int replyDepth = 1)
        : MessageFilterAttribute(new ReplyFromPremiumUserFilter(replyDepth))
    { }
}
