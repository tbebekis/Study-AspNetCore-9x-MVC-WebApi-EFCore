namespace CommonLib.Logging.Providers
{
    public interface IDatabaseLoggerService
    {
        Task InsertLogEntryAsync(LogEntry Entry);
        Task ApplyRetainPolicyAsync(int Days);
    }
}
