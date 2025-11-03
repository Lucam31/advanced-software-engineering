namespace Shared.Logger;

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