using Telegram.Bot.Types;
using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    public class RepliedToMeFilter : RepliedMessageFilter
    {
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
        {
            if (Reply.From == null)
                return false;

            return Reply.From.Id == (context.BotInfo?.User.Id ?? throw new ArgumentNullException(nameof(context), "MentionedFilter requires BotInfo to be initialized"));
        }
    }
}
