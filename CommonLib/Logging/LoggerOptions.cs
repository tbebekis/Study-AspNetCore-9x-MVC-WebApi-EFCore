namespace CommonLib.Logging
{

    /// <summary>
    /// A base class for logger option classes.
    /// <para>SEE: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging#configure-logging</para>
    /// </summary>
    public class LoggerOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LoggerOptions()
        {
        }

        // ● properties 
        /// <summary>
        /// The <c>LogLevel</c> entry in the <c>appsettings.json</c> file is a <c>JSON</c> object.
        /// <para>This property binds to that <c>LogLevel</c> object  in the <c>appsettings.json</c> file. </para>
        /// </summary>
        public virtual LoggerLevel LogLevel { get; set; } = new LoggerLevel();
    }
}
