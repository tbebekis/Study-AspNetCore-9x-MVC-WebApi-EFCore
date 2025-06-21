using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.HealthChecks
{
    public class HealthCheckReport
    {
        public class HealthCheckItem
        {
            internal HealthCheckItem(string Name, string Description, TimeSpan Duration, string Status, IReadOnlyDictionary<string, object> Data)
            {
                this.Name = Name;
                this.Description = Description;
                this.Duration = Duration;
                this.Status = Status;
                this.Data = Data;
            }

            public string Name { get; }
            public string Description { get; }            
            public TimeSpan Duration { get; }
            public string Status { get; }
            public IReadOnlyDictionary<string, object> Data { get; }

        }

        // ●
        internal HealthCheckReport(HttpContext HttpContext, HealthReport Report)
        {
            TotalChecks = Report.Entries.Count;
            Healthy = Report.Entries.Count(x => x.Value.Status == HealthStatus.Healthy);
            Unhealthy = Report.Entries.Count(x => x.Value.Status == HealthStatus.Unhealthy);
            Degraded = Report.Entries.Count(x => x.Value.Status == HealthStatus.Degraded);
            TotalDuration = Report.TotalDuration;
            Status = Report.Status.ToString();

            HealthChecks = Report.Entries.Select(x => new HealthCheckItem(
                x.Key,
                x.Value.Description,
                x.Value.Duration,
                x.Value.Status.ToString(),
                x.Value.Data
            ))
            .ToList();
        }

        // ●
        public override string ToString()
        {
            string JsonText = JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
            return JsonText;
        }

        static public string GetReportText(HttpContext HttpContext, HealthReport Report)
        {
            HealthCheckReport R = new HealthCheckReport(HttpContext, Report);
            return R.ToString();
        }

        // ●
        public int TotalChecks { get; }
        public int Healthy { get; }
        public int Unhealthy { get; }
        public int Degraded { get; }
        public TimeSpan TotalDuration { get; }
        public string Status { get; }
        public List<HealthCheckItem> HealthChecks { get; }
        
    }
}

/*
var result = JsonSerializer.Serialize(new
            {
                report.Entries.Count,
                Unhealthy = report.Entries.Count(x => x.Value.Status == HealthStatus.Unhealthy),
                Degraded = report.Entries.Count(x => x.Value.Status == HealthStatus.Degraded),
                Status = report.Status.ToString(),
                report.TotalDuration,
                Checks = report.Entries.Select(e => new
                {
                    Check = e.Key,
                    e.Value.Description,
                    e.Value.Duration,
                    Status = e.Value.Status.ToString()
                })
            }); 
 */
