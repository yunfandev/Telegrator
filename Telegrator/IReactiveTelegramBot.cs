using Telegrator.MadiatorCore;

namespace Telegrator
{
    /// <summary>
    /// Interface for reactive Telegram bot implementations.
    /// Defines the core properties and capabilities of a reactive bot.
    /// </summary>
    public interface IReactiveTelegramBot
    {
        /// <summary>
        /// Gets the update router for handling incoming updates.
        /// </summary>
        public IUpdateRouter UpdateRouter { get; }
    }
}
