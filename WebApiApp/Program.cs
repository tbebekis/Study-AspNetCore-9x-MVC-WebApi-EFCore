namespace WebApiApp
{
    public class Program
    {
        static public void Main(string[] args)
        {
            /// the WebApplication.CreateBuilder(args) adds the default logging providers, 
            ///  Console
            ///  Debug
            ///  EventSource
            /// SEE: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/#logging-providers
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseDefaultServiceProvider(options => options.ValidateScopes = false);
            App.AddServices(builder);
            var app = builder.Build();
            App.AddMiddlewares(app);
            app.Run();
        }
    }
}
