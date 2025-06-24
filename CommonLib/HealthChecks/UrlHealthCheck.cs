namespace CommonLib.HealthChecks
{
    /// <summary>
    /// Checks a specified url.
    /// <para>Usage</para>
    /// <code>.AddCheck("Url Check", new UrlHealthCheck("https://google.com", () => Lib.GetService&lt;IHttpClientFactory&gt;()))</code>
    /// </summary>
    public class UrlHealthCheck : IHealthCheck
    {
        string Url;
        Uri fUri;
        Func<IHttpClientFactory> GetHttpClientFactory;


        /// <summary>
        /// Constructor
        /// </summary>
        public UrlHealthCheck(string Url, Func<IHttpClientFactory> GetHttpClientFactoryFunc)
        {
            if (GetHttpClientFactoryFunc == null)
                throw new Exception($"No GetHttpClientFactoryFunc callback is passed to {this.GetType().Name}");

            if (string.IsNullOrWhiteSpace(Url))
                throw new Exception($"An empty Url is passed to {this.GetType().Name}");

            if (!Uri.TryCreate(Url, UriKind.Absolute, out var uri))
                throw new Exception($"An invalid Url {Url} is passed to {this.GetType().Name}");

            this.Url = Url;
            this.fUri = uri;
            this.GetHttpClientFactory = GetHttpClientFactoryFunc;
        }

        /// <summary>
        /// Runs the health check, returning the status of the component being checked.
        /// </summary>
        /// <param name="context">A context object associated with the current execution.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the health check.</param>
        /// <returns>A <see cref="Task{HealthCheckResult}"/> that completes when the health check has finished, yielding the status of the component being checked.</returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            IHttpClientFactory Factory = GetHttpClientFactory();
            using (var Client = Factory.CreateClient())
            {
                var Response = await Client.GetAsync(fUri);

                if (Response.StatusCode < HttpStatusCode.BadRequest)
                    return HealthCheckResult.Healthy($"The Url {Url} is accessible.");

                return HealthCheckResult.Unhealthy($"The Url {Url} is NOT accessible.");
            }     
            
        }
    }
}
