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
    ///     .AddFileLogger(options =&gt; {  
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
    ///         "File": {
    ///             "LogLevel": {
    ///                 "Default": "Information",
    ///                 "Microsoft": "Error"
    ///             },
    ///             "MaxSizeInKiloBytes": 2048,
    ///             "RetainPolicyInDays": 7,
    ///             "Folder": "BIN_PATH\\Logs",
    ///             "FileName": "Log.log"
    ///         }
    ///     },
    /// 
    ///   ...
    /// 
    ///  }
    /// </code>
    /// </summary>
    public class FileLoggerOptions
    {
        string fFolder;
        string fFileName;
        int fMaxSizeInKiloBytes;
        int fRetainPolicyInDays;

        // ● construction 
        /// <summary>
        /// Constructor
        /// </summary>
        public FileLoggerOptions()
        {
        }

        // ● properties 
        /// <summary>
        /// The active log level. Defaults to LogLevel.Information
        /// </summary>
        public LoggerLevel LogLevel { get; set; } = new LoggerLevel();
        /// <summary>
        /// The folder where log files should be placed. 
        /// <para>Defaluts to <c>BIN_PATH\Logs</c> where <c>BIN_PATH</c> is the <see cref="System.AppContext.BaseDirectory"/></para>
        /// </summary>
        public string Folder
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(fFolder))
                {
                    return Path.Combine(System.AppContext.BaseDirectory, "Logs");
                }
                if (fFolder.StartsWith("BIN_PATH")) // e.g. "Folder": "BIN_PATH\Logs"
                {
                    return fFolder.Replace("BIN_PATH", System.AppContext.BaseDirectory);
                }

                return fFolder; 
            }
            set { fFolder = value; }
        }
        /// <summary>
        /// The log file name.
        /// <para>NOTE: When a new log file is created its filename is prefixed by a datetime, e.g. <c>yyyy-MM-dd_HH_mm_ss__fff_Log.log</c></para>
        /// </summary>
        public string FileName
        {
            get { return !string.IsNullOrWhiteSpace(fFileName) ? fFileName : "Log.log"; }
            set { fFileName = value; }
        }
        /// <summary>
        /// The maximum number in KB of a single log file. Defaults to 1024 * 2.
        /// </summary>
        public int MaxSizeInKiloBytes
        {
            get { return fMaxSizeInKiloBytes > 0 ? fMaxSizeInKiloBytes : 1024 * 2; }
            set { fMaxSizeInKiloBytes = value; }
        }
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

 