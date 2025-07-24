using Telegram.Bot.Types.Enums;
using Telegrator.Filters;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute for filtering message with command "start" in bot's private chats.
    /// Allows handlers to respond to "welcome" bot commands.
    /// </summary>
    public class WelcomeAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Creates new instance of <see cref="WelcomeAttribute"/>
        /// </summary>
        public WelcomeAttribute() : base(new MessageChatTypeFilter(ChatType.Private), new CommandAlliasFilter("start"))
        { }
    }
}
