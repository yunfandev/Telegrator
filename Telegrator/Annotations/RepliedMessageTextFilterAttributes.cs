using Telegrator.Filters;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute for filtering updates where the replied-to message's text starts with the specified content.
    /// </summary>
    /// <param name="content">The string that the replied message's text should start with</param>
    /// <param name="comparison">The string comparison type</param>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class RepliedTextStartsWithAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture, int replyDepth = 1)
        : MessageFilterAttribute(new RepliedTextStartsWithFilter(content, comparison, replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering updates where the replied-to message's text ends with the specified content.
    /// </summary>
    /// <param name="content">The string that the replied message's text should end with</param>
    /// <param name="comparison">The string comparison type</param>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class RepliedTextEndsWithAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture, int replyDepth = 1)
        : MessageFilterAttribute(new RepliedTextEndsWithFilter(content, comparison, replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering updates where the replied-to message's text contains the specified content.
    /// </summary>
    /// <param name="content">The string that the replied message's text should contain</param>
    /// <param name="comparison">The string comparison type</param>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class RepliedTextContainsAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture, int replyDepth = 1)
        : MessageFilterAttribute(new RepliedTextContainsFilter(content, comparison, replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering updates where the replied-to message's text equals the specified content.
    /// </summary>
    /// <param name="content">The string that the replied message's text should equal</param>
    /// <param name="comparison">The string comparison type</param>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class RepliedTextEqualsAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture, int replyDepth = 1)
        : MessageFilterAttribute(new RepliedTextEqualsFilter(content, comparison, replyDepth))
    { }

    /// <summary>
    /// Attribute for filtering updates where the replied-to message contains any non-empty text.
    /// </summary>
    /// <param name="replyDepth">How many levels up the reply chain to check (default: 1)</param>
    public class RepliedHasTextAttribute(int replyDepth = 1)
        : MessageFilterAttribute(new RepliedTextNotNullOrEmptyFilter(replyDepth))
    { }
}
