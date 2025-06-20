namespace CommonLib.Logging.Providers
{
    /// <summary>
    /// A logger provider that writes log entries to a text file.
    /// <para><c>File</c> is the provider alias of this provider and can be used in the Logging section of the appsettings.json.</para>
    /// <para>Example use:</para>
    /// <code>
    ///  int CustomerId = 123;
    ///  int OrderId = 456;
    ///  
    ///  using (Logger.BeginScope("THIS IS A SCOPE"))
    ///  {
    ///      Logger.LogCritical("Customer {CustomerId} order {OrderId} is completed.", CustomerId, OrderId);
    ///      Logger.LogWarning("Just a warning");
    ///  } 
    /// </code>
    /// </summary>
    [Microsoft.Extensions.Logging.ProviderAlias("File")]
    public class FileLoggerProvider : LoggerProvider
    {
        // ● private
        bool Terminated;
        WriteLineFile LogFile;
        ConcurrentQueue<LogEntry> LogEntryQueue = new ConcurrentQueue<LogEntry>();
 
        /// <summary>
        /// A thread proc handling the writing to the log file
        /// </summary>
        void ThreadProc()
        {
            Task.Run(() => {
                ulong Counter = 0;
                while (!Terminated)
                {
                    try
                    {
                        LogEntry Entry = null;
                        if (LogEntryQueue.TryDequeue(out Entry))
                        {
                            if (LogFile == null)
                            {
                                LogFile = new WriteLineFile(Settings.Folder, Settings.FileName, LogEntry.LineCaptions, Settings.MaxSizeInKiloBytes);
                            }

                            string Line = Entry.AsLine;
                            LogFile.WriteLine(Line);

                            Counter = Interlocked.Increment(ref Counter);
                            if (Counter % 100 == 0)
                            {
                                LogFile.DeleteFilesOlderThan(Settings.RetainPolicyInDays);
                            }

                            if (Counter > 10000 && (Counter >= (ulong.MaxValue - 1000)))
                                Counter = 0;
                        }

                        System.Threading.Thread.Sleep(300);
                    }
                    catch // (Exception ex)
                    {
                    }
                }

            });
        }

        // ● overrides
        /// <summary>
        /// Disposes the options change toker. IDisposable pattern implementation.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            Terminated = true;
            base.Dispose(disposing);
        }

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public FileLoggerProvider(FileLoggerOptions Settings)
        {
            this.Settings = Settings;
            ThreadProc();
        }

        // ● public
        /// <summary>
        /// Checks if the given logLevel is enabled. It is called by the Logger.
        /// </summary>
        public override bool IsEnabled(LogLevel logLevel)
        {
            bool Result = logLevel != LogLevel.None
               && this.Settings.LogLevel.Default != LogLevel.None
               && Convert.ToInt32(logLevel) >= Convert.ToInt32(this.Settings.LogLevel.Default);

            return Result;
        }
        /// <summary>
        /// Writes the specified log information to a log file.
        /// </summary>
        public override async Task WriteLogAsync(LogEntry Entry)
        {
            LogEntryQueue.Enqueue(Entry);
            await Task.CompletedTask;
        }

        // ● properties
        /// <summary>
        /// Returns the settings
        /// </summary>
        internal FileLoggerOptions Settings { get; private set; }
    }
}


 
 