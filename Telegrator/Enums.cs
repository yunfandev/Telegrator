namespace Telegrator
{
    /// <summary>
    /// Enumeration of dice types supported by Telegram.
    /// Used for filtering dice messages and determining dice emoji representations.
    /// </summary>
    public enum DiceType
    {
        /// <summary>
        /// Standard dice (🎲).
        /// </summary>
        Dice,
        
        /// <summary>
        /// Darts (🎯).
        /// </summary>
        Darts,
        
        /// <summary>
        /// Bowling (🎳).
        /// </summary>
        Bowling,
        
        /// <summary>
        /// Basketball (🏀).
        /// </summary>
        Basketball,
        
        /// <summary>
        /// Football (⚽).
        /// </summary>
        Football,
        
        /// <summary>
        /// Casino slot machine (🎰).
        /// </summary>
        Casino
    }

    /// <summary>
    /// Flags version of <see cref="Telegram.Bot.Types.Enums.ChatType"/>
    /// Type of the <see cref="Telegram.Bot.Types.Chat"/>, from which the message or inline query was sent
    /// </summary>
    [Flags]
    public enum ChatTypeFlags
    {
        /// <summary>
        /// Normal one-to-one chat with a user or bot
        /// </summary>
        Private = 0x1,

        /// <summary>
        /// Normal group chat
        /// </summary>
        Group = 0x2,

        /// <summary>
        /// A channel
        /// </summary>
        Channel = 0x4,

        /// <summary>
        /// A supergroup
        /// </summary>
        Supergroup = 0x8,

        /// <summary>
        /// Value possible only in <see cref="Telegram.Bot.Types.InlineQuery.ChatType"/>: private chat with the inline query sender
        /// </summary>
        Sender
    }

    /// <summary>
    /// Levels of debug writing
    /// </summary>
    [Flags]
    public enum DebugLevel
    {
        /// <summary>
        /// Write debug messages from filters execution
        /// </summary>
        Filters = 0x1,

        /// <summary>
        /// Write debug messages from handlers providers execution
        /// </summary>
        Providers = 0x2,

        /// <summary>
        /// Write debug messages from update router's execution
        /// </summary>
        Router = 0x4,

        /// <summary>
        /// Write debug messages from handlers pool execution
        /// </summary>
        HandlersPool = 0x8
    }
}
