namespace CommonLib.Logging
{
    public class LoggerLevel
    {
        public LoggerLevel()
        {
        }
        public LoggerLevel(LogLevel Level)
        {
            Default = Level;
        }
        /// <summary>
        /// The active log level. Defaults to LogLevel.Information
        /// </summary>
        public LogLevel Default { get; set; } = LogLevel.Information;
        public LogLevel Microsoft { get; set; } = LogLevel.Error;
    }
}
