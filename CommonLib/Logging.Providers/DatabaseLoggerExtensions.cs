namespace CommonLib.Logging.Providers
{
 
    /// <summary>
    /// Database logger extension methods.
    /// </summary>
    static public class DatabaseLoggerExtensions
    {
        /// <summary>
        /// Adds the database logger provider, aliased as 'Database', in the available services as singleton 
        /// and binds the database logger options class to the 'Database' section of the appsettings.json file.
        /// </summary>
        static public ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.AddSingleton<DatabaseLoggerOptions>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, DatabaseLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions<DatabaseLoggerOptions, DatabaseLoggerProvider>(builder.Services);

            return builder;
        }
        /// <summary>
        /// Adds the database logger provider, aliased as 'Database', in the available services as singleton 
        /// and binds the database logger options class to the 'Database' section of the appsettings.json file.
        /// </summary>
        static public ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder builder, Action<DatabaseLoggerOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            builder.AddDatabaseLogger();
            builder.Services.Configure(options);

            return builder;
        }
    }






}
