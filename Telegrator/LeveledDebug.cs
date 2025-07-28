using System.Diagnostics;

namespace Telegrator
{
    /// <summary>
    /// Telegrator's Debug logger helper
    /// </summary>
    public static class LeveledDebug
    {
        /// <summary>
        /// Gets or sets flags of what debug messages to write
        /// </summary>
        public static DebugLevel IndentFlags { get; set; } = DebugLevel.None;

        /// <summary>
        /// Writes debug message if Indent level has Router flag
        /// </summary>
        /// <param name="message"></param>
        public static void RouterWriteLine(string message)
        {
            if (IndentFlags.HasFlag(DebugLevel.Router))
                Debug.WriteLine(message);
        }

        /// <summary>
        /// Writes debug message if Indent level has Router flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void RouterWriteLine(string message, params object[] args)
        {
            if (IndentFlags.HasFlag(DebugLevel.Router))
                Debug.WriteLine(message, args);
        }

        /// <summary>
        /// Writes debug message if Indent level has Providers flag
        /// </summary>
        /// <param name="message"></param>
        public static void ProviderWriteLine(string message)
        {
            if (IndentFlags.HasFlag(DebugLevel.Providers))
                Debug.WriteLine(message);
        }

        /// <summary>
        /// Writes debug message if Indent level has Providers flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void ProviderWriteLine(string message, params object[] args)
        {
            if (IndentFlags.HasFlag(DebugLevel.Providers))
                Debug.WriteLine(message, args);
        }

        /// <summary>
        /// Writes debug message if Indent level has Filters flag
        /// </summary>
        /// <param name="message"></param>
        public static void FilterWriteLine(string message)
        {
            if (IndentFlags.HasFlag(DebugLevel.Filters))
                Debug.WriteLine(message);
        }

        /// <summary>
        /// Writes debug message if Indent level has Filters flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void FilterWriteLine(string message, params object[] args)
        {
            if (IndentFlags.HasFlag(DebugLevel.Filters))
                Debug.WriteLine(message, args);
        }

        /// <summary>
        /// Writes debug message if Indent level has Pool flag
        /// </summary>
        /// <param name="message"></param>
        public static void PoolWriteLine(string message)
        {
            if (IndentFlags.HasFlag(DebugLevel.HandlersPool))
                Debug.WriteLine(message);
        }

        /// <summary>
        /// Writes debug message if Indent level has Pool flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void PoolWriteLine(string message, params object[] args)
        {
            if (IndentFlags.HasFlag(DebugLevel.HandlersPool))
                Debug.WriteLine(message, args);
        }
    }
}
