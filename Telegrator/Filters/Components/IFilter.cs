namespace Telegrator.Filters.Components
{
    public interface INamedFilter
    {
        public string Name { get; }
    }

    /// <summary>
    /// Interface for filters that can be collected into a completed filters list.
    /// Provides information about whether a filter should be tracked during execution.
    /// </summary>
    public interface IFilterCollectable
    {
        /// <summary>
        /// Gets if filter can be collected to <see cref="CompletedFiltersList"/>
        /// </summary>
        public bool IsCollectible { get; }
    }

    /// <summary>
    /// Represents a filter for a specific update type.
    /// </summary>
    /// <typeparam name="T">The type of the update to filter.</typeparam>
    public interface IFilter<T> : IFilterCollectable where T : class
    {
        /// <summary>
        /// Determines whether the filter can pass for the given context.
        /// </summary>
        /// <param name="info">The filter execution context.</param>
        /// <returns>True if the filter passes; otherwise, false.</returns>
        public bool CanPass(FilterExecutionContext<T> info);
    }
}
