namespace CommonLib.HealthChecks
{

    /// <summary>
    /// Helper
    /// </summary>
    static public class HealthCheckCenter
    {
        /// <summary>
        /// Serializes to <c>JSON</c> text a <see cref="HealthReport"/> instance.
        /// </summary>
        static public string SerializeReport(HttpContext HttpContext, HealthReport Report)
        {           

            object ReportData = new
            {
                TotalChecks = Report.Entries.Count,
                Healthy = Report.Entries.Count(x => x.Value.Status == HealthStatus.Healthy),
                Unhealthy = Report.Entries.Count(x => x.Value.Status == HealthStatus.Unhealthy),
                Degraded = Report.Entries.Count(x => x.Value.Status == HealthStatus.Degraded),
                TotalDuration = Report.TotalDuration,
                Status = Report.Status.ToString(),

                HealthChecks = Report.Entries.Select(x => new
                {
                    Name = x.Key,
                    Description = x.Value.Description,
                    Duration = x.Value.Duration,
                    Status = x.Value.Status.ToString(),
                    Data = x.Value.Data
                })
                .ToList()
            };

            JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
            string JsonText = JsonSerializer.Serialize(ReportData, JsonOptions);            
            return JsonText;          
        } 
    }
}

 