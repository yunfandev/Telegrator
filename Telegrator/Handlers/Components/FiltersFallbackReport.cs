using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Attributes.Components;
using Telegrator.Filters.Components;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Handlers.Components
{
    public class FiltersFallbackReport(HandlerDescriptor descriptor, FilterExecutionContext<Update> context)
    {
        public HandlerDescriptor Descriptor { get; } = descriptor;

        public FilterExecutionContext<Update> Context { get; } = context;

        public FilterFallbackInfo? UpdateValidator { get; set; }

        public FilterFallbackInfo? StateKeeperValidator { get; set; }

        public List<FilterFallbackInfo> UpdateFilters { get; } = [];

        /*
        public FilterFallbackInfo OfType<T>(int index = 0) where T : IFilter<Update>
        {
            return UpdateFilters.Where(info => info.Failed is T).ElementAt(index);
        }

        public FilterFallbackInfo OfAttribute<T>(int index = 0) where T : UpdateFilterAttributeBase
        {
            return UpdateFilters.Where(info => info.Name == typeof(T).Name).ElementAt(index);
        }

        public FilterFallbackInfo OfName(string name, int index = 0)
        {
            return UpdateFilters.Where(info => info.Name == name).ElementAt(index);
        }
        */

        public bool ExceptAttribute<T>(int index = 0) where T : UpdateFilterAttributeBase
        {
            string name = typeof(T).Name;
            IEnumerable<FilterFallbackInfo> failed = UpdateFilters.Where(info => info.Failed);
            if (failed.Count() > 1)
                return false;

            return failed.SingleOrDefault()?.Name == name;
        }
    }

    public class FilterFallbackInfo(string name, IFilter<Update> filter, bool failed, Exception? exception)
    {
        public string Name { get; } = name;

        public IFilter<Update> Filter { get; } = filter;

        public bool Failed { get; } = failed;

        public Exception? Exception { get; } = exception;
    }

    public enum FallbackReason
    {
        NullTarget,

        FailedFilter
    }
}
