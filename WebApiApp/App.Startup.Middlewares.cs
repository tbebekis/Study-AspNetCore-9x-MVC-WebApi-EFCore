namespace WebApiApp
{
    static public partial class App
    {

        static public void AddMiddlewares(WebApplication app)
        {
            var RootServiceProvider = (app as IApplicationBuilder).ApplicationServices;
            var HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
            var WebHostEnvironment = app.Environment;
 
            // ● Lib initialization
            Lib.Initialize(RootServiceProvider, HttpContextAccessor, WebHostEnvironment, Configuration);

            App.Initialize();

            // ● AppSettings
            // get an IOptionsMonitor<AppSettings> service instance
            // IOptionsMonitor is a singleton service
            IOptionsMonitor<AppSettings> AppSettingsMonitor = app.Services.GetRequiredService<IOptionsMonitor<AppSettings>>();

            // call Lib.SetupAppSettingsMonitor to hook into IOptionsMonitor<AppSettings>.OnChange()
            Lib.SetupAppSettingsMonitor(AppSettingsMonitor);

            //----------------------------------------------------------------------------------------
            // Middlewares
            //----------------------------------------------------------------------------------------

            app.UseStatusCodePages(Context => StatusCodeErrorHandlerWebApi.Handle(Context)); 


            // ● global exception handler
            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.MapScalarApiReference(options =>
                {
                    var documents = new[]
                    {
                        new ScalarDocument("v1", "Production API", "/openapi/{documentName}.json")
                    };
                        
                    options.AddDocuments(documents);

                    //options.AddDocument("v1");
                    //options.OpenApiRoutePattern = "/openapi/{documentName}.json";
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            Lib.AddObjectMappings();
            Lib.ObjectMapper.Configure();

            if (DataContext.UseInMemoryDatabase)
            {
                DemoData.AddDataInMemory(() => new DataContext());
            }
            else
            {
                DataContext.EnsureDatabase(); 
            }
        }
    }
}
