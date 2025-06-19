namespace MvcApp
{
    /*
    Service Lifetime:           
        ● Singleton : once per application
        ● Scoped    : once per HTTP Request
        ● Transient : each time is requested     
     */


    static public partial class App
    {


        /// <summary>
        /// Static file response callback.
        /// </summary>
        static void StaticFileResponseProc(StaticFileResponseContext StaticFilesContext)
        {
            StaticFilesContext.Context.Response.Headers.Append(HeaderNames.CacheControl, Lib.Settings.Http.StaticFilesCacheControl);
        }

        // ● public
        /// <summary>
        /// Add middlewares the the pipeline
        /// </summary>
        static public void AddMiddlewares(WebApplication app)
        {
            var RootServiceProvider = (app as IApplicationBuilder).ApplicationServices;
            var HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
            var WebHostEnvironment = app.Environment;

            // ● Lib initialization
            Lib.Initialize(RootServiceProvider, HttpContextAccessor, WebHostEnvironment, Configuration);
     
 
            App.Initialize();
            //Test();

            //----------------------------------------------------------------------------------------
            // Middlewares
            //----------------------------------------------------------------------------------------

            // ● error handling
            // we handle errors in the same page for all environments
            app.UseExceptionHandler("/Home/Error");

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();            


            /*
            app.UseHttpsRedirection();
           app.UseStaticFiles();
           // app.UseCookiePolicy();

           app.UseRouting();
           // app.UseRateLimiter();
           // app.UseRequestLocalization();
           // app.UseCors();

           app.UseAuthentication();
           app.UseAuthorization();
           // app.UseSession();
           // app.UseResponseCompression();
           // app.UseResponseCaching(); 
           */

            app.UseHttpsRedirection();
            

            // ● static files - wwwroot
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = StaticFileResponseProc
            });
            // ● static files - OutputPath\Plugins
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Lib.BinPath, "Plugins")),
                RequestPath = new PathString("/Plugins"),
                OnPrepareResponse = StaticFileResponseProc
            });
            // ● static files - Themes folder
            if (!string.IsNullOrWhiteSpace(Lib.Settings.Theme))
            {
                string ThemesFolderPhysicalPath = Path.Combine(Lib.ContentRootPath, ViewLocationExpander.ThemesFolder);
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(ThemesFolderPhysicalPath),
                    RequestPath = new PathString("/" + ViewLocationExpander.ThemesFolder),
                    OnPrepareResponse = StaticFileResponseProc
                });
            }

            // ● Cookie Policy
            app.UseCookiePolicy(new CookiePolicyOptions {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                // If CheckConsentNeeded is set to true, then the IsEssential should be also set to true, for any Cookie's CookieOptions setting.
                // SEE: https://stackoverflow.com/questions/52456388/net-core-cookie-will-not-be-set
                CheckConsentNeeded = context => true,

                // Set the secure flag, which Chrome's changes will require for SameSite none.
                // Note this will also require you to be running on HTTPS.
                MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None,

                // Set the cookie to HTTP only which is good practice unless you really do need
                // to access it client side in scripts.
                HttpOnly = HttpOnlyPolicy.Always,

                // Add the SameSite attribute, this will emit the attribute with a value of none.
                Secure = app.Environment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always
            });

            // ● endpoint resolution middlware
            app.UseRouting();

            if (Lib.Settings.UseAuthentication)
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }

 
            // ● Request Localization 
            // UseRequestLocalization initializes a RequestLocalizationOptions object. 
            // On every request the list of RequestCultureProvider in the RequestLocalizationOptions is enumerated 
            // and the first provider that can successfully determine the request culture is used.
            // SEE: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization#localization-middleware
            app.UseRequestLocalization((RequestLocalizationOptions options) =>
            {
                var Cultures = Lib.GetSupportedCultures();
                options.DefaultRequestCulture = new RequestCulture(Lib.Settings.Defaults.CultureCode);
                options.SupportedCultures = Cultures;
                options.SupportedUICultures = Cultures;
                options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
            });

            // ● Cors
            //app.UseCors();

            // ● Session
            app.UseSession();

            // ● MVC 
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            

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
