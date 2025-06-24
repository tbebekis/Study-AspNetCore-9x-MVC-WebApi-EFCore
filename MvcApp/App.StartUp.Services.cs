namespace MvcApp
{
    static public partial class App
    {

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

            /// Service Lifetime:           
            /// ● Singleton : once per application
            /// ● Scoped    : once per HTTP Request
            /// ● Transient : each time is requested   


            // ● AppSettings
            IConfigurationSection AppSettingsSection = builder.Configuration.GetRequiredSection(nameof(AppSettings));
            builder.Services.Configure<AppSettings>(AppSettingsSection);
            builder.Configuration.Bind(nameof(AppSettings), Lib.Settings);

            // ● logging
            builder.Logging.ClearProviders()
                .AddConsole()
                .AddFileLogger()
                .AddDatabaseLogger()
                ;

            // ● health checks
            IHealthChecksBuilder HealthChecksBuilder = builder.Services.AddHealthChecks();
            HealthChecksStore.AddHealthChecks(HealthChecksBuilder);
 

            // ● global exception handler
            /// NOTE: we do NOT need this.
            /// Actually any error ends up in the Error view, because of the app.UseExceptionHandler("/Home/Error"); setting.
            // builder.Services.AddExceptionHandler<GlobalExceptionHandlerMvc>();
            // builder.Services.AddProblemDetails();

            // ● DbContext
            /// AddDbContextPool() singleton service
            /// AddDbContext() scoped servicea
            /// SEE: https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics
            /// builder.Services.AddDbContextPool<AppDbContext>(context => context.UseSqlite(), poolSize: 1024);
            builder.Services.AddDbContext<DataContext>();
            RBAC.Initialize(() => new DataContext());

            // ● custom services
            builder.Services.AddScoped<PageBuilderService>();
            builder.Services.AddScoped<MvcUserContext>();           // current Identity context            
            builder.Services.AddScoped(typeof(AppDataService<>));
            builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            // ● provide the proper service to the database logger
            DatabaseLoggerProvider.GetServiceFunc = () => new DatabaseLoggerService();

            // ● provide the call-back for getting user permissions
            PermissionAuthorizationHandler.GetUserPermissionsFunc = RBAC.GetUserPermissionListForMvc;
 
            // ● HttpContext - NOTE: is singleton
            builder.Services.AddHttpContextAccessor();

            // ● ActionContext - see: https://github.com/aspnet/mvc/issues/3936
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();  // see: https://github.com/aspnet/mvc/issues/3936

            // ● Memory Cache - NOTE: is singleton
            /// NOTE: Distributed Cache is required for Session to function properly
            /// SEE: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state#configure-session-state
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
                ActionExceptionFilter.MvcHandlerFunc = GlobalErrorHandlers.MvcActionErrorHandler;
                ActionExceptionFilter.AjaxHandlerFunc = GlobalErrorHandlers.AjaxActionErrorHandler;
            });
            MvcBuilder.AddJsonOptions(options => SetupJsonSerializerOptions(options.JsonSerializerOptions));
            builder.Services.ConfigureHttpJsonOptions(options => SetupJsonSerializerOptions(options.SerializerOptions));

            MvcBuilder.AddRazorRuntimeCompilation();

            // ● Plugins 
            App.PartManager = MvcBuilder.PartManager;
        }
    }
}
