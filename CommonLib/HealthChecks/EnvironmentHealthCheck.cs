namespace CommonLib.HealthChecks
{


    /// <summary>
    /// Displays information about the <see cref="Environment"/>.
    /// <para>Usage</para>
    /// <code>.AddCheck("Environment", new EnvironmentHealthCheck(), tags: ["host"])</code>
    /// </summary>
    public class EnvironmentHealthCheck : IHealthCheck
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public EnvironmentHealthCheck()
        {
        }

        /// <summary>
        /// Runs the health check, returning the status of the component being checked.
        /// </summary>
        /// <param name="context">A context object associated with the current execution.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the health check.</param>
        /// <returns>A <see cref="Task{HealthCheckResult}"/> that completes when the health check has finished, yielding the status of the component being checked.</returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                int MB = 1024 * 1024;
                double Megabytes = Math.Round((double)Environment.WorkingSet / MB, 2);

                var Metadata = new Dictionary<string, object>()
                {
                    { "OS", Environment.OSVersion.Platform.ToString() },
                    { "OSVersion", Environment.OSVersion.VersionString }, //MachineName
                    { "Is64BitOS", Environment.Is64BitOperatingSystem},
                    { "Is64BitApp", Environment.Is64BitProcess},
                    { "MachineName", Environment.MachineName },
                    { "CLRVersion", Environment.Version.ToString() },
                    { "Processors", Environment.ProcessorCount },
                    { "MappedMemory", $"{Megabytes} MB" },
                    { "CpuUsagePrivilegedTime", Environment.CpuUsage.PrivilegedTime },  
                    { "CpuUsageUserTime", Environment.CpuUsage.UserTime },                  
                };

                return await Task.FromResult(HealthCheckResult.Healthy("Environment is healthy", Metadata));
            }
            catch (Exception ex)
            {
                HealthCheckResult Failure = HealthCheckResult.Unhealthy("Environment check failed", ex, null);
                return await Task.FromResult(Failure);
            }
        }
 
    }
}
