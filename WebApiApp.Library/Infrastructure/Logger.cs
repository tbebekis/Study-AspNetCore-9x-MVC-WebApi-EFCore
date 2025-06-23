#nullable enable

namespace WebApiApp.Library
{
    static public class Logger
    {
        static public void Debug(string Source, string Text, EventId eventId, params object?[] args)
        {
            Lib.CreateLogger(Source).LogDebug(eventId, Text, args);
        }
        static public void Debug(string Source, string Text, params object?[] args)
        {
            Lib.CreateLogger(Source).LogDebug(0, Text, args);
        }

        static public void Trace(string Source, string Text, EventId eventId, params object?[] args)
        {
            Lib.CreateLogger(Source).LogTrace(eventId, Text, args);
        }
        static public void Trace(string Source, string Text, params object?[] args)
        {
            Lib.CreateLogger(Source).LogTrace(0, Text, args);
        }

        static public void Info(string Source, string Text, EventId eventId, params object?[] args)
        {
            Lib.CreateLogger(Source).LogInformation(eventId, Text, args);
        }
        static public void Info(string Source, string Text, params object?[] args)
        {
            Lib.CreateLogger(Source).LogInformation(0, Text, args);
        }

        static public void Warn(string Source, string Text, EventId eventId, params object?[] args)
        {
            Lib.CreateLogger(Source).LogWarning(eventId, Text, args);
        }
        static public void Warn(string Source, string Text, params object?[] args)
        {
            Lib.CreateLogger(Source).LogWarning(0, Text, args);
        }

        static public void Error(string Source, string Text, EventId eventId, params object?[] args)
        {
            Lib.CreateLogger(Source).LogError(eventId, Text, args);
        }
        static public void Error(string Source, string Text, params object?[] args)
        {
            Lib.CreateLogger(Source).LogError(0, Text, args);
        }
        static public void Error(string Source, Exception Ex, EventId eventId)
        {
            Lib.CreateLogger(Source).LogError(eventId, Ex, string.Empty);
        }
        static public void Error(string Source, Exception Ex)
        {
            Lib.CreateLogger(Source).LogError(0, Ex, string.Empty);
        }

        static public void Critical(string Source, string Text, EventId eventId, params object?[] args)
        {
            Lib.CreateLogger(Source).LogCritical(eventId, Text, args);
        }
        static public void Critical(string Source, string Text, params object?[] args)
        {
            Lib.CreateLogger(Source).LogCritical(0, Text, args);
        }
        static public void Critical(string Source, Exception Ex, EventId eventId)
        {
            Lib.CreateLogger(Source).LogCritical(eventId, Ex, string.Empty);
        }
        static public void Critical(string Source, Exception Ex)
        {
            Lib.CreateLogger(Source).LogCritical(0, Ex, string.Empty);
        }
    }
}
