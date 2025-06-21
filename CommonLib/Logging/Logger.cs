namespace CommonLib.Logging
{
    /// <summary>
    /// Represents an object that handles log information.
    /// <para></para>
    /// <para>Implementation of the <see cref="ILogger"/> interface.</para>
    /// <para>This class does <strong>NOT</strong> write or display log information in a medium. </para>
    /// <para>Its responsibility is to create just a <see cref="LogEntry"/> instance, fill the properties of that log entry, 
    /// and then pass it to the associated <see cref="LoggerProvider"/> logger provider.</para>
    /// <para>There is no need to have any other implementation of the <see cref="ILogger"/> interface.</para>
    /// <para>The <c>Log()</c> method of this logger just prepares a <see cref="LogEntry"/> instance, representing a unit of log information,
    /// and then it calls the <see cref="LoggerProvider.WriteLogAsync(LogEntry)"/> abstract method.</para>
    /// <para>The <see cref="LoggerProvider.WriteLogAsync(LogEntry)"/> writes or displays the <see cref="LogEntry"/> according to its own logic. </para>
    /// </summary>
    internal class Logger : ILogger
    {
        // ● construction 
        /// <summary>
        /// Constructor.
        /// <para>CAUTION: You never create a logger directly. This is a responsibility of the logging framework by calling the provider's CreateLogger().</para>
        /// </summary>
        public Logger(LoggerProvider Provider, string Category)
        {
            this.Provider = Provider;
            this.Category = Category;
        }

        // ● ILogger implementation 
        /// <summary>
        /// Begins a logical operation scope. Returns an IDisposable that ends the logical operation scope on dispose.
        /// </summary>
        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return Provider.ScopeProvider.Push(state);
        }
        /// <summary>
        /// Checks if the given logLevel is enabled.
        /// </summary>
        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return Provider.IsEnabled(logLevel);
        }
        /// <summary>
        /// Creates a log entry, actually a log info instance, fill the properties of that log info, and then passes it to the associated logger provider.
        /// <para>WARNING: It's easier to use the ILogger extension methods than this method, since it requires a lot of parameters, so calling it could be a very complicated action.</para>
        /// </summary>
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if ((this as ILogger).IsEnabled(logLevel))
            {
                LogEntry Entry = new LogEntry();
                Entry.Category = this.Category;
                Entry.Level = logLevel;
                // well, the passed default formatter function does not takes the exception into account
                // SEE:  https://github.com/aspnet/Extensions/blob/master/src/Logging/Logging.Abstractions/src/LoggerExtensions.cs
                Entry.Text = exception?.Message ?? state.ToString(); // formatter(state, exception)
                Entry.Exception = exception;
                Entry.EventId = eventId;
                Entry.State = state;

                // well, you never know what it really is
                if (state is string)
                {
                    Entry.StateText = state.ToString();
                }
                // in case we have to do with a message template, lets get the keys and values (for Structured Logging providers)
                // SEE: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging#log-message-template
                // SEE: https://softwareengineering.stackexchange.com/questions/312197/benefits-of-structured-logging-vs-basic-logging
                else if (state is IEnumerable<KeyValuePair<string, object>> Properties)
                {
                    Entry.StateProperties = new Dictionary<string, object>();

                    foreach (KeyValuePair<string, object> item in Properties)
                    {
                        Entry.StateProperties[item.Key] = item.Value;
                    }
                }

                // gather info about scope(s), if any
                if (Provider.ScopeProvider != null)
                {
                    Provider.ScopeProvider.ForEachScope((value, loggingProps) =>
                    {
                        if (Entry.Scopes == null)
                            Entry.Scopes = new List<LogScopeInfo>();

                        LogScopeInfo Scope = new LogScopeInfo();
                        Entry.Scopes.Add(Scope);

                        if (value is string)
                        {
                            Scope.Text = value.ToString();
                        }
                        else if (value is IEnumerable<KeyValuePair<string, object>> props)
                        {
                            if (Scope.Properties == null)
                                Scope.Properties = new Dictionary<string, object>();

                            foreach (var pair in props)
                            {
                                Scope.Properties[pair.Key] = pair.Value;
                            }
                        }
                    },
                    state);

                }

                Task.Run(async () => {
                    try
                    {
                        await Provider.WriteLogAsync(Entry);
                    }
                    catch  
                    {
                    }
                });
 
            }
        }

        // ● properties 
        /// <summary>
        /// The logger provider who created this instance
        /// </summary>
        public LoggerProvider Provider { get; private set; }
        /// <summary>
        /// The category this instance serves.
        /// <para>The category is usually the fully qualified class name of a class asking for a logger, e.g. MyNamespace.MyClass </para>
        /// </summary>
        public string Category { get; private set; }
    }
}
