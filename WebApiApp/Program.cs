namespace WebApiApp
{
    public partial class Program
    {
        static public void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            App.AddServices(builder);

            var app = builder.Build();

            App.AddMiddlewares(app);

            app.Run();
        }
    }
}
