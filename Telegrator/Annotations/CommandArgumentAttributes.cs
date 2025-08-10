using System.Text.RegularExpressions;
using Telegrator.Filters;

namespace Telegrator.Annotations
{
    public class ArgumentStartsWithAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0)
        : MessageFilterAttribute(new ArgumentStartsWithFilter(content, comparison, index))
    { }

    public class ArgumentEndsWithAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0)
        : MessageFilterAttribute(new ArgumentEndsWithFilter(content, comparison, index))
    { }

    public class ArgumentContainsAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0)
        : MessageFilterAttribute(new ArgumentContainsFilter(content, comparison, index))
    { }

    public class ArgumentEqualsAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0)
        : MessageFilterAttribute(new ArgumentEqualsFilter(content, comparison, index))
    { }

    public class ArgumentRegexAttribute(string pattern, RegexOptions options = RegexOptions.None, TimeSpan matchTimeout = default, int index = 0)
        : MessageFilterAttribute(new ArgumentRegexFilter(pattern, options, matchTimeout, index))
    { }
}
