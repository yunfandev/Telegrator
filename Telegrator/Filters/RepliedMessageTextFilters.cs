using Telegram.Bot.Types;
using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Base class for filters that operate on the text of a replied message (the message being replied to).
    /// The replyDepth parameter determines how many levels up the reply chain to search for the target message.
    /// </summary>
    public abstract class RepliedMessageTextFilters : RepliedMessageFilter
    {
        /// <summary>
        /// Gets the text of the replied message (the message being replied to at the specified depth).
        /// </summary>
        public string Text { get; private set; } = null!;
        
        /// <summary>
        /// The content to match in the replied message.
        /// </summary>
        protected readonly string Content;
        
        /// <summary>
        /// The string comparison to use for matching.
        /// </summary>
        protected readonly StringComparison Comparison;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedMessageTextFilters"/> class.
        /// </summary>
        /// <param name="content">The content to match.</param>
        /// <param name="comparison">The string comparison to use.</param>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        protected RepliedMessageTextFilters(string content, StringComparison comparison = StringComparison.InvariantCulture, int replyDepth = 1)
            : base(replyDepth)
        {
            Content = content;
            Comparison = comparison;
        }

        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            Text = Reply.Text ?? string.Empty;
            return CanPassNext(context);
        }
    }

    /// <summary>
    /// Filters replied messages (the message being replied to at the specified depth) whose text starts with the specified content.
    /// </summary>
    public class RepliedTextStartsWithFilter : RepliedMessageTextFilters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedTextStartsWithFilter"/> class.
        /// </summary>
        /// <param name="content">The content to match.</param>
        /// <param name="comparison">The string comparison to use.</param>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        public RepliedTextStartsWithFilter(string content, StringComparison comparison = StringComparison.InvariantCulture, int replyDepth = 1)
            : base(content, comparison, replyDepth) { }
        
        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Text.StartsWith(Content, Comparison);
    }

    /// <summary>
    /// Filters replied messages (the message being replied to at the specified depth) whose text ends with the specified content.
    /// </summary>
    public class RepliedTextEndsWithFilter : RepliedMessageTextFilters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedTextEndsWithFilter"/> class.
        /// </summary>
        /// <param name="content">The content to match.</param>
        /// <param name="comparison">The string comparison to use.</param>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        public RepliedTextEndsWithFilter(string content, StringComparison comparison = StringComparison.InvariantCulture, int replyDepth = 1)
            : base(content, comparison, replyDepth) { }
        
        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Text.EndsWith(Content, Comparison);
    }

    /// <summary>
    /// Filters replied messages (the message being replied to at the specified depth) whose text contains the specified content.
    /// </summary>
    public class RepliedTextContainsFilter : RepliedMessageTextFilters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedTextContainsFilter"/> class.
        /// </summary>
        /// <param name="content">The content to match.</param>
        /// <param name="comparison">The string comparison to use.</param>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        public RepliedTextContainsFilter(string content, StringComparison comparison = StringComparison.InvariantCulture, int replyDepth = 1)
            : base(content, comparison, replyDepth) { }
        
        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Text.IndexOf(Content, Comparison) >= 0;
    }

    /// <summary>
    /// Filters replied messages (the message being replied to at the specified depth) whose text equals the specified content.
    /// </summary>
    public class RepliedTextEqualsFilter : RepliedMessageTextFilters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepliedTextEqualsFilter"/> class.
        /// </summary>
        /// <param name="content">The content to match.</param>
        /// <param name="comparison">The string comparison to use.</param>
        /// <param name="replyDepth">The reply depth to search up the reply chain for the target message.</param>
        public RepliedTextEqualsFilter(string content, StringComparison comparison = StringComparison.InvariantCulture, int replyDepth = 1)
            : base(content, comparison, replyDepth) { }
        
        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Text.Equals(Content, Comparison);
    }
}
