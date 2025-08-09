using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using Telegrator.Handlers;

namespace Telegrator.Filters
{
    public class CommandArgumentFilter : Filter<Message>
    {
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            CommandHandlerAttribute attr = context.CompletedFilters.Get<CommandHandlerAttribute>(0);
            string[] args = attr.Arguments ??= context.Input.SplitArgs();

            return alliases.Contains(ReceivedCommand, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
