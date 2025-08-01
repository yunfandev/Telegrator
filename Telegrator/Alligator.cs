using System.Diagnostics;

namespace Telegrator
{
    /// <summary>
    /// Telegrator's FUNNY debug logger helper
    /// </summary>
    public static class Alligator
    {
        /// <summary>
        /// Gets or sets flags of what trace messages to write
        /// </summary>
        public static DebugLevel Allowed { get; set; } = DebugLevel.None;

        /// <summary>
        /// Writes trace message if Indent level has Router flag
        /// </summary>
        /// <param name="message"></param>
        public static void RouterWriteLine(string message)
        {
            if (Allowed.HasFlag(DebugLevel.Router))
                Trace.WriteLine(message);
        }

        /// <summary>
        /// Writes debug message if Indent level has Router flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void RouterWriteLine(string message, params object[] args)
        {
            if (Allowed.HasFlag(DebugLevel.Router))
                Trace.WriteLine(string.Format(message, args));
        }

        /// <summary>
        /// Writes trace message if Indent level has Providers flag
        /// </summary>
        /// <param name="message"></param>
        public static void ProviderWriteLine(string message)
        {
            if (Allowed.HasFlag(DebugLevel.Providers))
                Trace.WriteLine(message);
        }

        /// <summary>
        /// Writes trace message if Indent level has Providers flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void ProviderWriteLine(string message, params object[] args)
        {
            if (Allowed.HasFlag(DebugLevel.Providers))
                Trace.WriteLine(string.Format(message, args));
        }

        /// <summary>
        /// Writes trace message if Indent level has Filters flag
        /// </summary>
        /// <param name="message"></param>
        public static void FilterWriteLine(string message)
        {
            if (Allowed.HasFlag(DebugLevel.Filters))
                Trace.WriteLine(message);
        }

        /// <summary>
        /// Writes trace message if Indent level has Filters flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void FilterWriteLine(string message, params object[] args)
        {
            if (Allowed.HasFlag(DebugLevel.Filters))
                Trace.WriteLine(string.Format(message, args));
        }

        /// <summary>
        /// Writes trace message if Indent level has Pool flag
        /// </summary>
        /// <param name="message"></param>
        public static void PoolWriteLine(string message)
        {
            if (Allowed.HasFlag(DebugLevel.HandlersPool))
                Trace.WriteLine(message);
        }

        /// <summary>
        /// Writes trace message if Indent level has Pool flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void PoolWriteLine(string message, params object[] args)
        {
            if (Allowed.HasFlag(DebugLevel.HandlersPool))
                Trace.WriteLine(string.Format(message, args));
        }
        
        /// <summary>
        /// Writes trace message if flag was set
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public static void WriteLine(DebugLevel level, string message)
        {
            if (Allowed.HasFlag(level))
                Trace.WriteLine(message);
        }

        /// <summary>
        /// Writes trace message if flag was set
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLine(DebugLevel level, string message, params object[] args)
        {
            if (Allowed.HasFlag(level))
                Trace.WriteLine(string.Format(message, args));
        }
    }
}
