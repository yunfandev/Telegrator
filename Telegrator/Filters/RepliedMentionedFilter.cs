using Telegram.Bot.Types;
using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Filter that checks if a replied message contains a mention of the bot or a specific user.
    /// Requires a <see cref="MessageHasEntityFilter"/> to be applied first to identify mention entities.
    /// </summary>
    public class RepliedMentionedFilter : RepliedMessageFilter
    {
        /// <summary>
        /// The username to check for in the mention (null means check for bot's username).
        /// </summary>
        private readonly string? Mention;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMentionedFilter"/> class that checks for bot mentions.
        /// </summary>
        /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
        public RepliedMentionedFilter(int replyDepth = 1) : base(replyDepth)
        {
            Mention = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMentionedFilter"/> class that checks for specific user mentions.
        /// </summary>
        /// <param name="mention">The username to check for in the mention.</param>
        /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
        public RepliedMentionedFilter(string mention, int replyDepth = 1) : base(replyDepth)
        {
            Mention = mention;
        }

        /// <summary>
        /// Checks if the replied message contains a mention of the specified user or bot.
        /// This filter requires a <see cref="MessageHasEntityFilter"/> to be applied first
        /// to identify mention entities in the replied message.
        /// </summary>
        /// <param name="context">The filter execution context containing the message and completed filters.</param>
        /// <returns>True if the replied message contains the specified mention; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the bot username is null and no specific mention is provided.</exception>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
        {
            if (Reply.Text == null)
                return false;

            string userName = Mention ?? context.BotInfo.User.Username ?? throw new ArgumentNullException(nameof(context), "RepliedMentionedFilter requires BotInfo to be initialized");
            MessageEntity entity = context.CompletedFilters.Get<MessageHasEntityFilter>(0).FoundEntities.ElementAt(0);

            string mention = Reply.Text.Substring(entity.Offset + 1, entity.Length - 1);
            return userName == mention;
        }
    }
}
