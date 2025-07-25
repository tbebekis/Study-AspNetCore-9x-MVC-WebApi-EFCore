﻿namespace CommonLib.Logging
{

    /// <summary>
    /// A base logger provider class. 
    /// <para>A logger provider essentialy represents the medium where log information is saved or displayed.</para>
    /// <para>This class may serve as base class in writing a file or a database logger provider.</para>
    /// </summary>
    public abstract class LoggerProvider : IDisposable, ILoggerProvider, ISupportExternalScope
    {
        ConcurrentDictionary<string, Logger> loggers = new ConcurrentDictionary<string, Logger>();
        IExternalScopeProvider fScopeProvider;


        // ● interface implementations 
        /// <summary>
        /// Called by the logging framework in order to set external scope information source for the logger provider. 
        /// <para>ISupportExternalScope implementation</para>
        /// </summary>
        void ISupportExternalScope.SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            fScopeProvider = scopeProvider;
        }
        /// <summary>
        /// Returns an ILogger instance to serve a specified category.
        /// <para>Asp.Net Core calls it <c>Category</c> but in reality it is the <c>Source</c> code entity that creates log entries.</para>
        /// <para>The source is usually the fully qualified class name of a class using a logger, e.g. MyNamespace.MyClass </para>
        /// </summary>
        ILogger ILoggerProvider.CreateLogger(string Category)
        {
            return loggers.GetOrAdd(Category, (category) => {
                return new Logger(this, category);
            });
        }
        /// <summary>
        /// Disposes this instance
        /// </summary>
        void IDisposable.Dispose()
        {
            if (!this.IsDisposed)
            {
                try
                {
                    Dispose(true);
                }
                catch
                {
                }

                this.IsDisposed = true;
                GC.SuppressFinalize(this);  // instructs GC not bother to call the destructor                
            }
        }


        // ● protected 
        /// <summary>
        /// Disposes the options change toker. IDisposable pattern implementation.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
        }
        protected LoggerOptions Options { get; }

        // ● construction 
        /// <summary>
        /// Constructor
        /// </summary>
        public LoggerProvider(LoggerOptions Options)
        {
            this.Options = Options;
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~LoggerProvider()
        {
            if (!this.IsDisposed)
            {
                Dispose(false);
            }
        }

        // ● public 
        /// <summary>
        /// Returns true if a specified log level is enabled.
        /// <para>Called by logger instances created by this provider.</para>
        /// </summary>
        /// <summary>
        /// Checks if the given logLevel is enabled. It is called by the Logger.
        /// </summary>
        public virtual bool IsEnabled(LogLevel logLevel)
        {
            bool Result = logLevel != LogLevel.None
               && this.Options.LogLevel.Default != LogLevel.None
               && Convert.ToInt32(logLevel) >= Convert.ToInt32(this.Options.LogLevel.Default);

            return Result;
        }
        /// <summary>
        /// The loggers do not actually log the information in any medium.
        /// Instead the call their provider WriteLog() method, passing the gathered log information.
        /// </summary>
        public virtual async Task WriteLogAsync(LogEntry Entry)
        {
            await Task.CompletedTask;
        }

        // ● properties 
        /// <summary>
        /// Returns the scope provider. 
        /// <para>Called by logger instances created by this provider.</para>
        /// </summary>
        internal IExternalScopeProvider ScopeProvider
        {
            get
            {
                if (fScopeProvider == null)
                    fScopeProvider = new LoggerExternalScopeProvider();
                return fScopeProvider;
            }
        }
        /// <summary>
        /// Returns true when this instance is disposed.
        /// </summary> 
        public bool IsDisposed { get; protected set; }
        
    }
}
