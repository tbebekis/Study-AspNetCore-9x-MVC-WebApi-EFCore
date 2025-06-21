namespace CommonLib.HealthChecks
{

    /// <summary>
    /// A health check using a callback which executes the check and returns a <see cref="HealthCheckResult"/> instance.
    /// <para>Usage</para>
    /// <code>.AddCheck("DataContext GetAllProducts Check", new GenericHealthCheck(DataContextGetAllProductsCheckFunc, 60), tags: ["DbContext"])</code>
    /// </summary>
    public class GenericHealthCheck : IHealthCheck
    {
        // ● Example call-back
        /*
        static HealthCheckResult DataContextGetAllProductsCheckFunc(object Info)
        {     
            try
            {
                long StartTime = Stopwatch.GetTimestamp();

                using (var DataContext = new DataContext())
                {
                    DbSet<Product> DbSet = DataContext.Set<Product>();
                    var List = DbSet.AsNoTracking().ToList();
                }

                int MaxSeconds = Convert.ToInt32(Info);

                TimeSpan ElapsedTime = Stopwatch.GetElapsedTime(StartTime);
                if (ElapsedTime.TotalSeconds > MaxSeconds)
                    return new HealthCheckResult(HealthStatus.Unhealthy, "Operation exceeds the specified timeout.");

                return new HealthCheckResult(HealthStatus.Healthy, "Operation timeout not exceeded.");

            }
            catch (Exception ex)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, "An exception is thrown", ex);
            }   
        }

        */

        Func<object, HealthCheckResult> HealthCheckFunc;
        object Info;


        /// <summary>
        /// Constructor
        /// </summary>
        public GenericHealthCheck(Func<object, HealthCheckResult> HealthCheckFunc, object Info = null)
        {
            if (HealthCheckFunc == null)
                throw new Exception($"No HealthCheckFunc callback is passed to {this.GetType().Name}");

            this.HealthCheckFunc = HealthCheckFunc;
            this.Info = Info;
        }

        /// <summary>
        /// Runs the health check, returning the status of the component being checked.
        /// </summary>
        /// <param name="context">A context object associated with the current execution.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the health check.</param>
        /// <returns>A <see cref="Task{HealthCheckResult}"/> that completes when the health check has finished, yielding the status of the component being checked.</returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return HealthCheckFunc(Info);
            }
            catch (Exception ex)
            {
                HealthCheckResult Failure = HealthCheckResult.Unhealthy("Health check failed", ex, null);
                return await Task.FromResult(Failure);
            }
        }
    }
}
