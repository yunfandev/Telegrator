namespace Telegrator.Configuration
{
    /// <summary>
    /// Configuration options for Telegram bot behavior and execution settings.
    /// Controls various aspects of bot operation including concurrency, routing, and execution policies.
    /// </summary>
    public class TelegramBotOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether only the first found handler should be executed for each update.
        /// </summary>
        public bool ExecuteOnlyFirstFoundHanlder { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of parallel working handlers. Null means no limit.
        /// </summary>
        public int? MaximumParallelWorkingHandlers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether awaiting handlers should be routed separately from regular handlers.
        /// </summary>
        public bool ExclusiveAwaitingHandlerRouting { get; set; }

        /// <summary>
        /// Gets or sets the global cancellation token for all bot operations.
        /// </summary>
        public CancellationToken GlobalCancellationToken { get; set; }
    }
}
