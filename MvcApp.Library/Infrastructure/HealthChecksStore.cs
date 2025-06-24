namespace MvcApp.Library
{
    static public class HealthChecksStore
    {
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
                    return new HealthCheckResult(HealthStatus.Unhealthy, $"GetAllProducts() operation exceeds the specified timeout. Max Seconds: {MaxSeconds}");

                return new HealthCheckResult(HealthStatus.Healthy, $"GetAllProducts() operation timeout not exceeded. Max Seconds: {MaxSeconds}");

            }
            catch (Exception ex)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, "An exception is thrown", ex);
            }
        }


        static public IHealthChecksBuilder AddHealthChecks(IHealthChecksBuilder HealthChecksBuilder)
        { 
            HealthChecksBuilder.AddCheck("Environment", new EnvironmentHealthCheck(), tags: ["host"])
                .AddCheck("Url Check", new UrlHealthCheck("https://google.com", () => Lib.GetService<IHttpClientFactory>()))
                .AddCheck("IP Check", new PingHealthCheck("8.8.8.8"))
                .AddCheck("DataContext GetAllProducts Check", new GenericHealthCheck(DataContextGetAllProductsCheckFunc, 60), tags: ["DbContext"]);

            return HealthChecksBuilder;
        }

    }
}
