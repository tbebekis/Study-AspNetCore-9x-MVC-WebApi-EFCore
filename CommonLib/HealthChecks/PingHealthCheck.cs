namespace CommonLib.HealthChecks
{
    /// <summary>
    /// Checks a specified <see cref="IPAddress"/>.
    /// <para>Usage</para>
    /// <code>.AddCheck("IP Check", new PingHealthCheck("8.8.8.8"))</code>
    /// </summary>
    public class PingHealthCheck : IHealthCheck
    {
        string IP;
        IPAddress IPAddress;


        /// <summary>
        /// Constructor
        /// </summary>
        public PingHealthCheck(string IP)
        {
            if (string.IsNullOrWhiteSpace(IP))
                throw new Exception($"An empty IP is passed to {this.GetType().Name}");
 

            if (!System.Net.IPAddress.TryParse(IP, out var ip))
                throw new Exception($"An invalid Url {IP} is passed to {this.GetType().Name}");

            this.IP = IP;
            this.IPAddress = ip;
        }

        /// <summary>
        /// Runs the health check, returning the status of the component being checked.
        /// </summary>
        /// <param name="context">A context object associated with the current execution.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the health check.</param>
        /// <returns>A <see cref="Task{HealthCheckResult}"/> that completes when the health check has finished, yielding the status of the component being checked.</returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var Ping = new Ping())
            {
                PingReply Reply = await Ping.SendPingAsync(IPAddress);

                if (Reply.Status == IPStatus.Success)
                    return HealthCheckResult.Healthy($"The IP {IP} is reachable.");

                return HealthCheckResult.Unhealthy($"The IP {IP} is unreachable.");
            }      

        }
    }
}
