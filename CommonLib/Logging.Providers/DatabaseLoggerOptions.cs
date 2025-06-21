namespace CommonLib.Logging.Providers
{

    /// <summary>
    /// Options for the logger.
    /// <para>There are two ways to configure this logger: </para>
    /// <para></para>
    /// <para> ● 1. using the <c>builder.Logging</c> in Program.cs</para>
    /// <code>
    ///   builder.Logging.ClearProviders()
    ///     .AddConsole()
    ///     //.AddFileLogger()           
    ///     .AddDatabaseLogger(options =&gt; {  
    ///       options.MaxSizeInKiloBytes = 2048;
    ///       options.RetainPolicyInDays = 7;
    ///   });
    /// </code>
    /// <para> ● 2. using the <c>appsettings.json</c> file </para>
    /// <code>
    ///   {
    ///     "Logging": {
    ///         "LogLevel": {
    ///             "Default": "Information",
    ///             "Microsoft.AspNetCore": "Warning"
    ///         },
    ///         "Database": {
    ///             "LogLevel": {
    ///                 "Default": "Information",
    ///                 "Microsoft": "Error"
    ///             },
    ///             "RetainPolicyInDays": 7
    ///         }
    ///     },
    /// 
    ///   ...
    /// 
    ///  }
    /// </code>
    /// </summary>
    public class DatabaseLoggerOptions: LoggerOptions
    {
        int fRetainPolicyInDays;

        // ● construction 
        /// <summary>
        /// Constructor
        /// </summary>
        public DatabaseLoggerOptions()
        {
        }

        // ● properties  
        /// <summary>
        /// The maximum days of log files to retain. Defaults to 7.
        /// </summary>
        public int RetainPolicyInDays
        {
            get { return fRetainPolicyInDays < 7 ? 7 : fRetainPolicyInDays; }
            set { fRetainPolicyInDays = value; }
        }
    }
 
}
