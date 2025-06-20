namespace WebApiApp.Library
{
    public class DatabaseLoggerService : AppDataService<LogEntryEntity>, IDatabaseLoggerService
    {
        public async Task ApplyRetainPolicyAsync(int Days)
        {
            DateTime MaxDateTime = DateTime.UtcNow.Date;
            ListResult<LogEntryEntity> ListResult = await GetListWithFilterAsync(e => e.EntryTimeUtc >= MaxDateTime, null);
            if (ListResult.Succeeded && ListResult.List.Count > 0)
                await DeleteRangeAsync(ListResult.List);
        }

        public async Task InsertLogEntryAsync(LogEntry Entry)
        {
            LogEntryEntity Entity = new LogEntryEntity(Entry);
            await InsertAsync(Entity);
        }
    }
}
