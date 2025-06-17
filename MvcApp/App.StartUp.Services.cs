namespace MvcApp
{
    static public partial class App
    {
        static void GlobalErrorHandlerMvc(ActionExceptionFilterContext Context)
        {
            ErrorViewModel Model = new ErrorViewModel();
            Model.ErrorMessage = Context.InDevMode?  Context.Exception.GetText(): Context.Exception.Message;
            Model.RequestId = Context.RequestId;

            /* SEE: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#exception-filters */
            var Result = new ViewResult();
            Result.ViewName = "Error";
            Result.ViewData = new ViewDataDictionary(Context.ModelMetadataProvider, Context.ExceptionContext.ModelState);
            Result.ViewData.Model = Model;
            Result.ViewData.Add("Exception", Context.ExceptionContext.Exception);
            Result.ViewData.Add("RequestId", Context.RequestId);
            Context.ExceptionContext.Result = Result;
        }
        static void GlobalErrorHandlerAjax(ActionExceptionFilterContext Context)
        {
            DataResult Result = new();
            Result.ExceptionResult(Context.ExceptionContext.Exception);

            // NO, we do NOT want an invalid HTTP StatusCode. It is a valid HTTP Response.
            // We just have an action result with errors, so any error should be recorded by our HttpActionResult and delivered to the client.
            // context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError; 
            Context.ExceptionContext.HttpContext.Response.ContentType = "application/json";
            Context.ExceptionContext.Result = new JsonResult(Result);
        }

        static void SetupJsonSerializerOptions(JsonSerializerOptions JsonOptions)
        {
            Json.SetupJsonOptions(
                JsonOptions,
                Decimals: 2
                );
        }


        // ● public
        /// <summary>
        /// Add services to the container.
        /// </summary>
        static public void AddServices(WebApplicationBuilder builder)
        {
            // ● AppSettings
            App.Configuration = builder.Configuration;
            builder.Configuration.Bind(nameof(AppSettings), Lib.Settings);

            // ● DbContext
            // AddDbContextPool() singleton service
            // AddDbContext() scoped servicea
            // SEE: https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics
            //builder.Services.AddDbContextPool<AppDbContext>(context => context.UseSqlite(), poolSize: 1024);
            builder.Services.AddDbContext<DataContext>();
            RBAC.Initialize(() => new DataContext());

            // ● custom services
            builder.Services.AddScoped<PageBuilderService>();
            builder.Services.AddScoped<MvcUserContext>();           // current Identity context            
            builder.Services.AddScoped(typeof(AppDataService<>));
            builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            PermissionAuthorizationHandler.GetUserPermissionsFunc = RBAC.GetUserPermissionListForMvc;

            // ● HttpContext - NOTE: is singleton
            builder.Services.AddHttpContextAccessor();

            // ● ActionContext - see: https://github.com/aspnet/mvc/issues/3936
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();  // see: https://github.com/aspnet/mvc/issues/3936

            // ● Memory Cache - NOTE: is singleton
            // NOTE: Distributed Cache is required for Session to function properly
            // SEE: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state#configure-session-state
            builder.Services.AddDistributedMemoryCache(); // AddMemoryCache(); 

            // ● Authentication (Cookie) 
            if (Lib.Settings.UseAuthentication)
            {
                builder.Services.AddScoped<UserCookieAuthEvents>();

                AuthenticationBuilder AuthBuilder = builder.Services.AddAuthentication(options => {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = options.DefaultScheme;
                    options.DefaultChallengeScheme = options.DefaultScheme;
                });

                AuthBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {

                    TimeSpan Expiration = Lib.Settings.UserCookie.ExpirationHours <= 0 ? TimeSpan.FromDays(365) : TimeSpan.FromHours(Lib.Settings.UserCookie.ExpirationHours);

                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.AccessDeniedPath = "/access-denied";
                    options.ReturnUrlParameter = "ReturnUrl";
                    options.EventsType = typeof(UserCookieAuthEvents);
                    options.ExpireTimeSpan = Expiration;
                    //options.SlidingExpiration = true;

                    options.Cookie.Name = App.SAuthCookieName;       // cookie name
                    options.Cookie.IsEssential = Lib.Settings.UserCookie.IsEssential;
                    options.Cookie.HttpOnly = Lib.Settings.UserCookie.HttpOnly;
                    options.Cookie.SameSite = Lib.Settings.UserCookie.SameSite;
                    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
                });
 
                //● authorization
                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy(App.PolicyAuthenticated, policy => { policy.RequireAuthenticatedUser(); });
                });
            }
 
            // ● Session
            builder.Services.AddSession(options => {
                options.Cookie.Name = App.SSessionCookieName;       // cookie name
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;                  // Make the session cookie essential
                options.IdleTimeout = TimeSpan.FromMinutes(Lib.Settings.SessionTimeoutMinutes);
            });

            // ● Localization
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            // ● IHttpClientFactory
            builder.Services.AddHttpClient();

            // ● HSTS
            if (!builder.Environment.IsDevelopment())
            {
                HSTSSettings HSTS = Lib.Settings.HSTS;
                builder.Services.AddHsts(options =>
                {
                    options.Preload = HSTS.Preload;
                    options.IncludeSubDomains = HSTS.IncludeSubDomains;
                    options.MaxAge = TimeSpan.FromHours(HSTS.MaxAgeHours >= 1 ? HSTS.MaxAgeHours : 1);
                    if (HSTS.ExcludedHosts != null && HSTS.ExcludedHosts.Count > 0)
                    {
                        foreach (string host in HSTS.ExcludedHosts)
                            options.ExcludedHosts.Add(host);
                    }
                });
            }

            // ● Themes support
            if (ViewLocationExpander.UseThemes)
            {
                builder.Services.Configure<RazorViewEngineOptions>(options => { options.ViewLocationExpanders.Add(new ViewLocationExpander()); });
            }

            // ● MVC
            IMvcBuilder MvcBuilder = builder.Services.AddControllersWithViews(options => {                
                options.ModelBinderProviders.Insert(0, new ModelBinderProvider());
                options.Filters.Add<ActionExceptionFilter>();
                ActionExceptionFilter.MvcHandlerFunc = GlobalErrorHandlerMvc;
                ActionExceptionFilter.AjaxHandlerFunc = GlobalErrorHandlerAjax;
            });
            MvcBuilder.AddJsonOptions(options => SetupJsonSerializerOptions(options.JsonSerializerOptions));
            builder.Services.ConfigureHttpJsonOptions(options => SetupJsonSerializerOptions(options.SerializerOptions));

            MvcBuilder.AddRazorRuntimeCompilation();



            // ● Plugins
            LoadPluginDefinitions();
            LoadPluginAssemblies();
            AddPluginsToApplicationPartManager(MvcBuilder.PartManager);
        }
    }
}
