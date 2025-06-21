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
        ulong Counter = 0;
        WriteLineFile LogFile;

        /// <summary>
        /// Returns the settings
        /// </summary>
        FileLoggerOptions Settings => Options as FileLoggerOptions; 
 
        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public FileLoggerProvider(FileLoggerOptions Settings)
            : base(Settings)
        {
        }

        // ● public
        /// <summary>
        /// Writes the specified log information to a log file.
        /// </summary>
        public override async Task WriteLogAsync(LogEntry Entry)
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

            await Task.CompletedTask;
        } 

    }
}


 
 