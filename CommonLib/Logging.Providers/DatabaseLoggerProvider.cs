namespace CommonLib.Logging.Providers
{
    /// <summary>
    /// A logger provider that writes log entries to a database table.
    /// <para><c>Database</c> is the provider alias of this provider and can be used in the Logging section of the appsettings.json.</para>
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
    [Microsoft.Extensions.Logging.ProviderAlias("Database")]
    public class DatabaseLoggerProvider : LoggerProvider
    {
        // ● private
        ulong Counter = 0;
        IDatabaseLoggerService Service;

        /// <summary>
        /// Returns the settings
        /// </summary>
        DatabaseLoggerOptions Settings => Options as DatabaseLoggerOptions;


        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public DatabaseLoggerProvider(DatabaseLoggerOptions Settings)
            : base(Settings)
        {
        }

        // ● public
        /// <summary>
        /// Writes the specified log information to a log file.
        /// </summary>
        public override async Task WriteLogAsync(LogEntry Entry)
        {
            if (Service == null && GetServiceFunc != null)
                Service = GetServiceFunc();

            if (Service != null)
            {
                await Service.InsertLogEntryAsync(Entry);

                Counter = Interlocked.Increment(ref Counter);
                if (Counter % 100 == 0)
                {
                    await Service.ApplyRetainPolicyAsync(Settings.RetainPolicyInDays);
                }

                if (Counter > 10000 && (Counter >= (ulong.MaxValue - 1000)))
                    Counter = 0;
            }
 
        }

        // ● properties
        /// <summary>
        /// A call-back returning the data service used in writing to the database.
        /// </summary>
        static public Func<IDatabaseLoggerService> GetServiceFunc { get; set; }
    }
}
