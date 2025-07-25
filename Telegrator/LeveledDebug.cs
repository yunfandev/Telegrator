using System.Diagnostics;

namespace Telegrator
{
    /// <summary>
    /// Telegrator's Debug logger helper
    /// </summary>
    public static class LeveledDebug
    {
        /// <summary>
        /// Writes debug message if Indent level has Router flag
        /// </summary>
        /// <param name="message"></param>
        public static void RouterWriteLine(string message)
        {
            if (Debug.IndentLevel.HasFlag(DebugLevel.Router))
                Debug.WriteLine(message);
        }

        /// <summary>
        /// Writes debug message if Indent level has Router flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void RouterWriteLine(string message, params object[] args)
        {
            if (Debug.IndentLevel.HasFlag(DebugLevel.Router))
                Debug.WriteLine(message, args);
        }

        /// <summary>
        /// Writes debug message if Indent level has Providers flag
        /// </summary>
        /// <param name="message"></param>
        public static void ProviderWriteLine(string message)
        {
            if (Debug.IndentLevel.HasFlag(DebugLevel.Providers))
                Debug.WriteLine(message);
        }

        /// <summary>
        /// Writes debug message if Indent level has Providers flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void ProviderWriteLine(string message, params object[] args)
        {
            if (Debug.IndentLevel.HasFlag(DebugLevel.Providers))
                Debug.WriteLine(message, args);
        }

        /// <summary>
        /// Writes debug message if Indent level has Filters flag
        /// </summary>
        /// <param name="message"></param>
        public static void FilterWriteLine(string message)
        {
            if (Debug.IndentLevel.HasFlag(DebugLevel.Filters))
                Debug.WriteLine(message);
        }

        /// <summary>
        /// Writes debug message if Indent level has Filters flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void FilterWriteLine(string message, params object[] args)
        {
            if (Debug.IndentLevel.HasFlag(DebugLevel.Filters))
                Debug.WriteLine(message, args);
        }

        /// <summary>
        /// Writes debug message if Indent level has Pool flag
        /// </summary>
        /// <param name="message"></param>
        public static void PoolWriteLine(string message)
        {
            if (Debug.IndentLevel.HasFlag(DebugLevel.HandlersPool))
                Debug.WriteLine(message);
        }

        /// <summary>
        /// Writes debug message if Indent level has Pool flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void PoolWriteLine(string message, params object[] args)
        {
            if (Debug.IndentLevel.HasFlag(DebugLevel.HandlersPool))
                Debug.WriteLine(message, args);
        }
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
