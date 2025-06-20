namespace CommonLib.Logging.Providers
{
    /// <summary>
    /// File logger extension methods.
    /// </summary>
    static public class FileLoggerExtensions
    {
        /// <summary>
        /// Adds the file logger provider, aliased as 'File', in the available services as singleton and binds the file logger options class to the 'File' section of the appsettings.json file.
        /// </summary>
        static public ILoggingBuilder AddFileLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.AddSingleton<FileLoggerOptions>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions<FileLoggerOptions, FileLoggerProvider>(builder.Services);

            return builder;
        }
        /// <summary>
        /// Adds the file logger provider, aliased as 'File', in the available services as singleton and binds the file logger options class to the 'File' section of the appsettings.json file.
        /// </summary>
        static public ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<FileLoggerOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            builder.AddFileLogger();
            builder.Services.Configure(options);

            return builder;
        }
    }
}
