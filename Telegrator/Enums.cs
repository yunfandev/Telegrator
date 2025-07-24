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
}
