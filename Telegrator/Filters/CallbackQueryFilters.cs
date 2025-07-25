using Telegram.Bot.Types;
using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Filter thet checks <see cref="CallbackQuery"/>'s data
    /// </summary>
    public class CallbackDataFilter : Filter<CallbackQuery>
    {
        private readonly string _data;

        /// <summary>
        /// Initialize new instance of <see cref="CallbackDataFilter"/>
        /// </summary>
        /// <param name="data"></param>
        public CallbackDataFilter(string data)
        {
            _data = data;
        }

        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<CallbackQuery> context)
        {
            return context.Input.Data == _data;
        }
    }

    /// <summary>
    /// Filter that checks if <see cref="CallbackQuery"/> belongs to a specific message
    /// </summary>
    public class CallbackInlineIdFilter : Filter<CallbackQuery>
    {
        private readonly string _inlineMessageId;

        /// <summary>
        /// Initialize new instance of <see cref="CallbackInlineIdFilter"/>
        /// </summary>
        /// <param name="inlineMessageId"></param>
        public CallbackInlineIdFilter(string inlineMessageId)
        {
            _inlineMessageId = inlineMessageId;
        }

        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<CallbackQuery> context)
        {
            return context.Input.InlineMessageId == _inlineMessageId;
        }
    }
}
