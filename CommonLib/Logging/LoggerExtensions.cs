#nullable enable

namespace CommonLib.Logging
{
    static public class LoggerExtensions
    { 
        static public void Debug(this ILogger logger, string Text, EventId eventId, params object?[] args)
        {
            logger.LogDebug(eventId, Text, args);
        }
        static public void Debug(this ILogger logger, string Text, params object?[] args)
        {
            logger.LogDebug(0, Text, args);
        }
        
        static public void Trace(this ILogger logger, string Text, EventId eventId, params object?[] args)
        {
            logger.LogTrace(eventId, Text, args);
        }
        static public void Trace(this ILogger logger, string Text, params object?[] args)
        {
            logger.LogTrace(0, Text, args);
        }
        
        static public void Info(this ILogger logger, string Text, EventId eventId, params object?[] args)
        {
            logger.LogInformation(eventId, Text, args);
        }
        static public void Info(this ILogger logger, string Text, params object?[] args)
        {
            logger.LogInformation(0, Text, args);
        }
        
        static public void Warn(this ILogger logger, string Text, EventId eventId, params object?[] args)
        {
            logger.LogWarning(eventId, Text, args);
        }
        static public void Warn(this ILogger logger, string Text, params object?[] args)
        {
            logger.LogWarning(0, Text, args);
        }
        
        static public void Error(this ILogger logger, string Text, EventId eventId, params object?[] args)
        {
            logger.LogError(eventId, Text, args);
        }
        static public void Error(this ILogger logger, string Text, params object?[] args)
        {
            logger.LogError(0, Text, args);
        }
        static public void Error(this ILogger logger, Exception Ex, EventId eventId)
        {
            logger.LogError(eventId, Ex, string.Empty);
        }
        static public void Error(this ILogger logger, Exception Ex)
        {
            logger.LogError(0, Ex, string.Empty);
        }
        
        static public void Critical(this ILogger logger, string Text, EventId eventId, params object?[] args)
        {
            logger.LogCritical(eventId, Text, args);
        }
        static public void Critical(this ILogger logger, string Text, params object?[] args)
        {
            logger.LogCritical(0, Text, args);
        }
        static public void Critical(this ILogger logger, Exception Ex, EventId eventId)
        {
            logger.LogCritical(eventId, Ex, string.Empty);
        }
        static public void Critical(this ILogger logger, Exception Ex)
        {
            logger.LogCritical(0, Ex, string.Empty);
        }
     }
}
