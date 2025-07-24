namespace Telegrator.Configuration
{
    /// <summary>
    /// Interface for configuring handler collection behavior.
    /// Defines options that control how handlers are collected and processed during initialization.
    /// </summary>
    public interface IHandlersCollectingOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to descend the indexr of handler's index on register. ('false' by default)
        /// </summary>
        public bool DescendDescriptorIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to exclude intersecting command aliases.
        /// </summary>
        public bool ExceptIntersectingCommandAliases { get; set; }
    }
}
