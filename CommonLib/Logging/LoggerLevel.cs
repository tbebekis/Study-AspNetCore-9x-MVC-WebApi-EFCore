namespace CommonLib.Logging
{
    /// <summary>
    /// Helper. Serves as a property to a <see cref="LoggerOptions"/> derived class,
    /// in order to bind the options instance to the <c>appsettings.json</c> file.
    /// <para>SEE: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging#configure-logging</para>
    /// </summary>
    public class LoggerLevel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LoggerLevel()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public LoggerLevel(LogLevel Level)
        {
            Default = Level;
        }
        /// <summary>
        /// The active log level. Defaults to <see cref="LogLevel.Information"/>
        /// </summary>
        public LogLevel Default { get; set; } = LogLevel.Information;
        /// <summary>
        /// The <c>Microsoft</c> log level. Defaults to <see cref="LogLevel.Error"/>
        /// </summary>
        public LogLevel Microsoft { get; set; } = LogLevel.Error;
    }
}
