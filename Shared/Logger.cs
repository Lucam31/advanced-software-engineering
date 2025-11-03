using System.Text;

namespace Shared
{
    /// <summary>
    /// Defines the severity of a log message.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Detailed information for debugging.
        /// </summary>
        Debug,

        /// <summary>
        /// Normal application events.
        /// </summary>
        Info,

        /// <summary>
        /// Unexpected, but non-critical problems.
        /// </summary>
        Warning,

        /// <summary>
        /// An error occurred, but the application can continue.
        /// </summary>
        Error,

        /// <summary>
        /// An error that forces the application to stop.
        /// </summary>
        Fatal
    }

    /// <summary>
    /// A simple, static logger for the game.
    /// Provides colored console output and file logging.
    /// <remarks>
    /// This class is thread-safe.
    /// </remarks>
    /// </summary>
    public static class GameLogger
    {
        // --- Configuration ---
        private static LogLevel _minLevel = LogLevel.Info;
        private static bool _logToConsole = true;
        private static bool _logToFile = true;
        private static string _logFilePath = "logs/default_log.txt";

        // --- Thread-Safety ---
        private static readonly object _consoleLock = new object();
        private static readonly object _fileLock = new object();

        /// <summary>
        /// Configures the logger once at application startup.
        /// This will also clear the log file if logToFile is true.
        /// </summary>
        /// <param name="minLevel">The minimum level to log.</param>
        /// <param name="logToConsole">Should messages be logged to the console?</param>
        /// <param name="logToFile">Should messages be logged to a file?</param>
        /// <param name="logFilePath">Path to the log file (optional).</param>
        public static void Configure(
            LogLevel minLevel,
            bool logToConsole,
            bool logToFile,
            string? logFilePath = null)
        {
            _minLevel = minLevel;
            _logToConsole = logToConsole;
            _logToFile = logToFile;
            if (logFilePath != null)
            {
                _logFilePath = logFilePath;
            }

            if (!_logToFile) return;
            lock (_fileLock)
            {
                try
                {
                    var directory = Path.GetDirectoryName(_logFilePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory ?? throw new InvalidOperationException());
                    }

                    File.WriteAllText(_logFilePath, string.Empty);
                }
                catch (Exception ex)
                {
                    lock (_consoleLock)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"FATAL LOGGER ERROR: Failed to clear log file {_logFilePath}");
                        Console.WriteLine(ex);
                        Console.ResetColor();
                    }
                }
            }
        }

        /// <summary>
        /// Logs a Debug message (lowest priority).
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Logs an Informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        /// <summary>
        /// Logs a Warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// Logs an Error, optionally with an Exception.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The exception that occurred (optional).</param>
        public static void Error(string message, Exception? ex = null)
        {
            Log(LogLevel.Error, message, ex);
        }

        /// <summary>
        /// Logs a Fatal error that should terminate the program.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The exception that occurred (optional).</param>
        public static void Fatal(string message, Exception? ex = null)
        {
            Log(LogLevel.Fatal, message, ex);
        }


        // --- Private Core Logic ---

        /// <summary>
        /// The central log method that formats and dispatches to targets.
        /// </summary>
        private static void Log(LogLevel level, string message, Exception? ex = null)
        {
            // 1. Level Check: Only log if the level is important enough.
            if (level < _minLevel)
            {
                return;
            }

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var levelString = level.ToString().ToUpper();

            // StringBuilder is more efficient than string + string + ...
            var fileMessage = new StringBuilder();
            fileMessage.Append($"{timestamp} [{levelString}] {message}");
            if (ex != null)
            {
                // Add exception details (including stack trace)
                fileMessage.Append($"\n--- Exception Details ---\n{ex}\n-------------------------");
            }

            // 3. Dispatch to targets
            if (_logToConsole)
            {
                WriteToConsole(level, timestamp, levelString, message, ex);
            }

            if (_logToFile)
            {
                WriteToFile(fileMessage.ToString());
            }
        }

        /// <summary>
        /// Writes the formatted message to the console (thread-safe).
        /// </summary>
        private static void WriteToConsole(
            LogLevel level,
            string timestamp,
            string levelString,
            string message,
            Exception? ex)
        {
            // Lock to prevent colors and lines from mixing up
            lock (_consoleLock)
            {
                // Timestamp
                Console.Write($"{timestamp} ");

                // Level (Colored)
                Console.Write("[");
                Console.ForegroundColor = GetLevelColor(level);
                Console.Write(levelString);
                Console.ResetColor();
                Console.Write("] ");

                // Message
                Console.WriteLine(message);

                // Exception (Red)
                if (ex != null)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex);
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// Writes the formatted message to the log file (thread-safe).
        /// </summary>
        private static void WriteToFile(string message)
        {
            // Lock to prevent two threads from writing to the file simultaneously
            lock (_fileLock)
            {
                try
                {
                    // Ensure the "logs/" directory exists
                    var directory = Path.GetDirectoryName(_logFilePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Appends the line to the end of the file
                    File.AppendAllText(_logFilePath, message + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    // Fallback: If writing fails, at least scream to the console.
                    lock (_consoleLock)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"FATAL LOGGER ERROR: Failed to write to log file {_logFilePath}");
                        Console.WriteLine(ex);
                        Console.ResetColor();
                    }
                }
            }
        }

        /// <summary>
        /// Helper method to get the correct color for the log level.
        /// </summary>
        private static ConsoleColor GetLevelColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug: return ConsoleColor.Gray;
                case LogLevel.Info: return ConsoleColor.Cyan;
                case LogLevel.Warning: return ConsoleColor.Yellow;
                case LogLevel.Error: return ConsoleColor.Red;
                case LogLevel.Fatal: return ConsoleColor.Magenta;
                default: return ConsoleColor.White;
            }
        }
    }
}