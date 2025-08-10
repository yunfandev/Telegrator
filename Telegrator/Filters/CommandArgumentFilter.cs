using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using Telegrator.Handlers;

namespace Telegrator.Filters
{
    public abstract class CommandArgumentFilterBase(int index) : Filter<Message>
    {
        protected string Target { get; private set; } = null!;

        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            CommandHandlerAttribute attr = context.CompletedFilters.Get<CommandHandlerAttribute>(0);
            string[] args = attr.Arguments ??= context.Input.SplitArgs();
            Target = args.ElementAtOrDefault(index);

            if (Target == null)
                return false;

            return CanPassNext(context);
        }

        protected abstract bool CanPassNext(FilterExecutionContext<Message> context);
    }

    public class ArgumentStartsWithFilter(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0) : CommandArgumentFilterBase(index)
    {
        protected readonly string Content = content;
        protected readonly StringComparison Comparison = comparison;

        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Target.StartsWith(Content, Comparison);
    }

    public class ArgumentEndsWithFilter(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0) : CommandArgumentFilterBase(index)
    {
        protected readonly string Content = content;
        protected readonly StringComparison Comparison = comparison;

        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Target.EndsWith(Content, Comparison);
    }

    public class ArgumentContainsFilter(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0) : CommandArgumentFilterBase(index)
    {
        protected readonly string Content = content;
        protected readonly StringComparison Comparison = comparison;

        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Target.IndexOf(Content, Comparison) >= 0;
    }

    public class ArgumentEqualsFilter(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0) : CommandArgumentFilterBase(index)
    {
        protected readonly string Content = content;
        protected readonly StringComparison Comparison = comparison;

        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Target.Equals(Content, Comparison);
    }

    public class ArgumentRegexFilter : CommandArgumentFilterBase
    {
        private readonly Regex _regex;

        public Match Match { get; private set; } = null!;

        public ArgumentRegexFilter(Regex regex, int index = 0)
            : base(index) => _regex = regex;

        public ArgumentRegexFilter(string pattern, RegexOptions options = RegexOptions.None, TimeSpan matchTimeout = default, int index = 0)
            : base(index) => _regex = new Regex(pattern, options, matchTimeout);

        protected override bool CanPassNext(FilterExecutionContext<Message> context)
        {
            Match = _regex.Match(Target);
            return Match.Success;
        }
    }
}
