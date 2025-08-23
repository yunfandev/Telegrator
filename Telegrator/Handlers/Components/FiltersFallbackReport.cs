using Telegram.Bot.Types;
using Telegrator.Attributes.Components;
using Telegrator.Filters.Components;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Handlers.Components
{
    /// <summary>
    /// Represents a report of filter fallback information for debugging and error handling.
    /// Contains detailed information about which filters failed and why during handler execution.
    /// </summary>
    /// <param name="descriptor">The handler descriptor that generated this report.</param>
    /// <param name="context">The filter execution context.</param>
    public class FiltersFallbackReport(HandlerDescriptor descriptor, FilterExecutionContext<Update> context)
    {
        /// <summary>
        /// Gets the handler descriptor associated with this fallback report.
        /// </summary>
        public HandlerDescriptor Descriptor { get; } = descriptor;

        /// <summary>
        /// Gets the filter execution context that generated this report.
        /// </summary>
        public FilterExecutionContext<Update> Context { get; } = context;

        /// <summary>
        /// Gets or sets the fallback information for the update validator filter.
        /// </summary>
        public FilterFallbackInfo? UpdateValidator { get; set; }

        /// <summary>
        /// Gets or sets the fallback information for the state keeper validator filter.
        /// </summary>
        public FilterFallbackInfo? StateKeeperValidator { get; set; }

        /// <summary>
        /// Gets the list of fallback information for update filters that failed.
        /// </summary>
        public List<FilterFallbackInfo> UpdateFilters { get; } = [];

        /// <summary>
        /// Checks if the failure is due to a specific filter.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Only(string name, int index = 0)
        {
            FilterFallbackInfo? info = UpdateFilters.SingleSafe(info => info.Failed);
            if (info != null && info.Name != name)
                return false;

            FilterFallbackInfo? target = UpdateFilters.ElementAtOrDefault(index);
            return ReferenceEquals(target, info);
        }

        /// <summary>
        /// Checks if the failure is due to a specific filter.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public bool Only(string[] names)
        {
            return UpdateFilters
                .Where(info => info.Failed)
                .Select(info => info.Name)
                .SequenceEqual(names);
        }

        /// <summary>
        /// Checks if the failure is due to all filters except one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Except(string name, int index = 0)
        {
            FilterFallbackInfo? info = UpdateFilters.SingleSafe(info => !info.Failed);
            if (info != null && info.Name != name)
                return false;

            FilterFallbackInfo? target = UpdateFilters.ElementAtOrDefault(index);
            return ReferenceEquals(target, info);
        }

        /// <summary>
        /// Checks if the failure is due to all filters except one.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public bool Except(string[] names)
        {
            return UpdateFilters
                .Where(info => !info.Failed)
                .Select(info => info.Name)
                .SequenceEqual(names);
        }

        /// <summary>
        /// Checks if the failure is due to aall attribute type, excluding one.
        /// </summary>
        /// <typeparam name="T">The attribute type to check for.</typeparam>
        /// <param name="index">The index of the filter to check (default: 0).</param>
        /// <returns>True if the failure is exclusively due to the specified attribute type; otherwise, false.</returns>
        public bool ExceptAttribute<T>(int index = 0) where T : UpdateFilterAttributeBase
            => Except(nameof(T), index);

        /// <summary>
        /// Checks if the failure is due to a specific attribute type, excluding other failures.
        /// </summary>
        /// <typeparam name="T">The attribute type to check for.</typeparam>
        /// <param name="index">The index of the filter to check (default: 0).</param>
        /// <returns>True if the failure is exclusively due to the specified attribute type; otherwise, false.</returns>
        public bool OnlyAttribute<T>(int index = 0) where T : UpdateFilterAttributeBase
            => Only(nameof(T), index);
    }

    /// <summary>
    /// Contains information about a filter that failed during execution.
    /// Provides details about the filter, its failure status, and any associated exception.
    /// </summary>
    /// <param name="name">The name of the filter.</param>
    /// <param name="filter">The filter instance that failed.</param>
    /// <param name="failed">Whether the filter failed.</param>
    /// <param name="exception">The exception that occurred during filter execution, if any.</param>
    public class FilterFallbackInfo(string name, IFilter<Update> filter, bool failed, Exception? exception)
    {
        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Gets the filter instance that failed.
        /// </summary>
        public IFilter<Update> Filter { get; } = filter;

        /// <summary>
        /// Gets a value indicating whether the filter failed.
        /// </summary>
        public bool Failed { get; } = failed;

        /// <summary>
        /// Gets the exception that occurred during filter execution, if any.
        /// </summary>
        public Exception? Exception { get; } = exception;
    }

    /// <summary>
    /// Specifies the reason for a filter fallback.
    /// </summary>
    public enum FallbackReason
    {
        /// <summary>
        /// The filter target was null.
        /// </summary>
        NullTarget,

        /// <summary>
        /// The filter failed to pass.
        /// </summary>
        FailedFilter
    }
}
