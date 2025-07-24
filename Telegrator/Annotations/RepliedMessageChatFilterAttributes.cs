using Telegram.Bot.Types.Enums;
using Telegrator.Filters;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute for filtering messages where the replied-to message was sent in a forum chat.
    /// </summary>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class RepliedChatIsForumAttribute(int replyDepth = 1)
        : MessageFilterAttribute(new RepliedMessageChatIsForumFilter(replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering messages where the replied-to message was sent in a specific chat by ID.
    /// </summary>
    /// <param name="id">The chat ID to match</param>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class RepliedChatIdAttribute(long id, int replyDepth = 1)
        : MessageFilterAttribute(new RepliedMessageChatIdFilter(id, replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering messages where the replied-to message was sent in a chat of a specific type.
    /// </summary>
    /// <param name="type">The chat type to match</param>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class RepliedChatTypeAttribute(ChatType type, int replyDepth = 1)
        : MessageFilterAttribute(new RepliedMessageChatTypeFilter(type, replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering messages based on the chat title of the replied-to message.
    /// </summary>
    public class RepliedChatTitleAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message is from a chat with a specific title and comparison method.
        /// </summary>
        /// <param name="title">The chat title to match</param>
        /// <param name="comparison">The string comparison method</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedChatTitleAttribute(string? title, StringComparison comparison, int replyDepth = 1)
            : base(new RepliedMessageChatTitleFilter(title, comparison, replyDepth)) { }

        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message is from a chat with a specific title.
        /// </summary>
        /// <param name="title">The chat title to match</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedChatTitleAttribute(string? title, int replyDepth = 1)
            : base(new RepliedMessageChatTitleFilter(title, StringComparison.InvariantCulture, replyDepth)) { }
    }

    /// <summary>
    /// Attribute for filtering messages based on the chat username of the replied-to message.
    /// </summary>
    public class RepliedChatUsernameAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message is from a chat with a specific username and comparison method.
        /// </summary>
        /// <param name="userName">The chat username to match</param>
        /// <param name="comparison">The string comparison method</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedChatUsernameAttribute(string? userName, StringComparison comparison, int replyDepth = 1)
            : base(new RepliedMessageChatUsernameFilter(userName, comparison, replyDepth)) { }

        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message is from a chat with a specific username.
        /// </summary>
        /// <param name="userName">The chat username to match</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedChatUsernameAttribute(string? userName, int replyDepth = 1)
            : base(new RepliedMessageChatUsernameFilter(userName, replyDepth)) { }
    }

    /// <summary>
    /// Attribute for filtering messages based on the chat name of the replied-to message.
    /// </summary>
    public class RepliedChatNameAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message is from a chat with specific first and last names.
        /// </summary>
        /// <param name="firstName">The first name to match</param>
        /// <param name="lastName">The last name to match (optional)</param>
        /// <param name="comparison">The string comparison method</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedChatNameAttribute(string? firstName, string? lastName, StringComparison comparison, int replyDepth = 1)
            : base(new RepliedMessageChatNameFilter(firstName, lastName, comparison, replyDepth)) { }

        /// <summary>
        /// Initializes the attribute to filter messages where the replied-to message is from a chat with specific first and last names.
        /// </summary>
        /// <param name="firstName">The first name to match</param>
        /// <param name="lastName">The last name to match (optional)</param>
        /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
        public RepliedChatNameAttribute(string? firstName, string? lastName, int replyDepth = 1)
            : base(new RepliedMessageChatNameFilter(firstName, lastName, StringComparison.InvariantCulture, replyDepth)) { }
    }
}
