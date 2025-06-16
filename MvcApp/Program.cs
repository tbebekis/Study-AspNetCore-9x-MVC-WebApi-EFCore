namespace MvcApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseDefaultServiceProvider(options => options.ValidateScopes = false);
            App.AddServices(builder);
            var app = builder.Build();
            App.AddMiddlewares(app);
            app.Run();
        }
    }
}
