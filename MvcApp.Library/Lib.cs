using Microsoft.EntityFrameworkCore;

namespace MvcApp.Library
{
    /// <summary>
    /// Represents this library
    /// </summary>
    static public partial class Lib
    {
        public const string SDefaultId = "00000000-0000-0000-0000-000000000000";

        static IAppCache fCache;

        // ● public
        /// <summary>
        /// Initializes this class
        /// </summary>
        static public void Initialize(IServiceProvider RootServiceProvider,
                                        IHttpContextAccessor HttpContextAccessor,
                                        IWebHostEnvironment WebHostEnvironment,
                                        ConfigurationManager Configuration)
        {
            if (Lib.RootServiceProvider == null)
            {
                Lib.RootServiceProvider = RootServiceProvider;
                Lib.HttpContextAccessor = HttpContextAccessor;
                Lib.Configuration = Configuration;
                Lib.WebHostEnvironment = WebHostEnvironment;

                Session.Initialize(HttpContextAccessor);
                Caches.DefaultEvictionTimeoutMinutes = Settings.Defaults.CacheTimeoutMinutes;
            }
        }

        // ● public
        /// <summary>
        /// Returns a service specified by a type argument. If the service is not registered an exception is thrown.
        /// <para>WARNING: "Scoped" services can NOT be resolved from the "root" service provider. </para>
        /// <para>There are two solutions to the "Scoped" services problem:</para>
        /// <para> ● Use <c>HttpContext.RequestServices</c>, a valid solution since we use a "Scoped" service provider to create the service,  </para>
        /// <para> ● or add <c> .UseDefaultServiceProvider(options => options.ValidateScopes = false)</c> in the <c>CreateHostBuilder</c>() of the Program class</para>
        /// <para>see: https://github.com/dotnet/runtime/issues/23354 and https://devblogs.microsoft.com/dotnet/announcing-ef-core-2-0-preview-1/ </para>
        /// <para>SEE: https://www.milanjovanovic.tech/blog/using-scoped-services-from-singletons-in-aspnetcore</para>
        /// <para>SEE: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines#scoped-service-as-singleton</para>
        /// </summary>
        static public T GetService<T>(IServiceScope Scope = null)
        {
            IServiceProvider ServiceProvider = GetServiceProvider(Scope);
            return ServiceProvider.GetService<T>();
        }
        /// <summary>
        /// Returns the current <see cref="HttpContext"/>
        /// </summary>
        static public HttpContext GetHttpContext() => HttpContextAccessor.HttpContext;
        /// <summary>
        ///  Returns the current <see cref="Microsoft.AspNetCore.Http.HttpContext.Request"/>
        /// </summary>
        static public HttpRequest GetHttpRequest() => GetHttpContext().Request;
        /// <summary>
        /// Returns a <see cref="IServiceProvider"/>.
        /// <para>If a <see cref="IServiceScope"/> is specified, then the <see cref="IServiceScope.ServiceProvider"/> is returned.</para>
        /// <para>Otherwise, the <see cref="IServiceProvider"/> is returned from the <see cref="IHttpContextAccessor.HttpContext.RequestServices"/> property.</para>
        /// <para>Finally, and if not a <see cref="HttpContext"/> is available, the <see cref="RootServiceProvider"/> is returned.</para>
        /// </summary>
        static public IServiceProvider GetServiceProvider(IServiceScope Scope = null)
        {
            if (Scope != null)
                return Scope.ServiceProvider;

            HttpContext HttpContext = HttpContextAccessor?.HttpContext;
            return HttpContext?.RequestServices ?? RootServiceProvider;
        }

        /// <summary>
        /// Call-back for monitoring chandes in settings.
        /// </summary>
        static public void SetupAppSettingsMonitor(IOptionsMonitor<AppSettings> AppSettingsMonitor)
        {
            AppSettingsMonitor.OnChange(NewAppSettings =>
            {
                Lib.Settings = NewAppSettings;
            });
        }

        /// <summary>
        /// Returns application's <see cref="DbContext"/>
        /// </summary>
        static public DbContext GetDataContext()
        {
            DbContext Result = null;
            HttpContext HttpContext = Lib.GetHttpContext();

            if (HttpContext != null)
            {
                IServiceScope Scope = HttpContext.RequestServices.CreateScope();
                Result = Scope.ServiceProvider.GetService<DataContext>();
            }
            else
            {
                Result = new DataContext();
            }

            return Result;
        }
        /// <summary>
        /// Returns a data service instance
        /// </summary>
        static public EFDataService<T> GetDataService<T>() where T : BaseEntity
        {
            EFDataService<T> Result = new EFDataService<T>(typeof(DataContext));
            return Result;
        }

        /// <summary>
        /// Creates and returns a logger.
        /// </summary>
        static public ILogger CreateLogger(string Category = "")
        {
            ILoggerFactory LoggerFactory = Lib.GetService<ILoggerFactory>();
            Category = !string.IsNullOrWhiteSpace(Category) ? Category : "Global";
            ILogger Result = LoggerFactory.CreateLogger(Category);
            return Result;
        }

        // ● miscs
        /// <summary>
        /// Returns the list of supported cultures.
        /// <para>This setting may come from application settings, a database or elsewhere.</para>
        /// </summary>
        static public List<string> GetSupportedCultureCodes()
        {
            List<string> Result = new List<string>();
            if (Settings.Defaults.SupportedCultures != null && Settings.Defaults.SupportedCultures.Count > 0)
                Result.AddRange(Settings.Defaults.SupportedCultures);
            else
                Result.Add(Settings.Defaults.CultureCode);

            return Result;
        }
        /// <summary>
        /// Returns the list of supported cultures.
        /// <para>This setting may come from application settings, a database or elsewhere.</para>
        /// </summary>
        static public List<CultureInfo> GetSupportedCultures()
        {
            List<string> List =  GetSupportedCultureCodes();
            List<CultureInfo> Result = new List<CultureInfo>();

            foreach (string CultureCode in List)
                Result.Add(CultureInfo.GetCultureInfo(CultureCode));   

            return Result;
        }

        // ● properties
        /// <summary>
        /// This <see cref="IServiceProvider"/> is the root service provider and is assigned in <see cref="AppStartUp.AddMiddlewares(WebApplication)"/>
        /// <para><strong>WARNING</strong>: do <strong>NOT</strong> use this service provider to resolve "Scoped" services.</para>
        /// </summary>
        static public IServiceProvider RootServiceProvider { get; private set; }
        /// <summary>
        /// <see cref="IHttpContextAccessor"/> is a singleton service and this property is assigned in <see cref="AppStartUp.AddMiddlewares(WebApplication)"/>
        /// </summary>
        static public IHttpContextAccessor HttpContextAccessor { get; private set; }
        /// <summary>
        /// The configuration manager.
        /// </summary>
        static public ConfigurationManager Configuration { get; private set; }
        /// <summary>
        /// The <see cref="IWebHostEnvironment"/>
        /// </summary>
        static public IWebHostEnvironment WebHostEnvironment { get; private set; }

        /// <summary>
        /// True when the current user/requestor is authenticated with the cookie authentication scheme.
        /// </summary>
        static public bool IsAuthenticated => GetService<MvcUserContext>().IsAuthenticated;
        /// <summary>
        /// True when the application is running in development mode
        /// </summary>
        static public bool InDevMode => WebHostEnvironment.IsDevelopment();
 
        /// <summary>
        /// Application settings, coming from appsettings.json
        /// </summary>
        static public AppSettings Settings { get; private set; } = new AppSettings();
        /// <summary>
        /// Returns the application cache
        /// </summary>
        static public IAppCache Cache
        {
            get
            {
                if (fCache == null)
                {
                    var MemCache = Lib.GetService<IMemoryCache>();
                    if (MemCache != null)
                    {
                        fCache = new AppMemCache(MemCache);                       
                    }
                }

                if (fCache == null)
                {
                    var DistCache = Lib.GetService<IDistributedCache>();
                    if (DistCache != null)
                    {
                        fCache = new AppDistCache(DistCache);
                    }
                }

                if (fCache == null)
                    throw new ApplicationException("No Cache Service is registered");


                return fCache;
            }
        }
        /// <summary>
        /// Returns the <see cref="IObjectMapper"/> object mapper.
        /// <para>The application may provide its own mapper.</para>
        /// <para>The default mapper is a wrapper to the excellent AutoMaper library.</para>
        /// <para>SEE: https://automapper.org/ </para>
        /// </summary>
        static public IObjectMapper ObjectMapper { get; } = new ObjectMapper();

        /// <summary>
        /// The physical "root path", i.e. the root folder of the application
        /// <para> e.g. C:\MyApp</para>
        /// </summary>
        static public string ContentRootPath => WebHostEnvironment.ContentRootPath;
        /// <summary>
        /// The physical "web root" path, i.e. the path to the "wwwroot" folder
        /// <para>e.g. C:\MyApp\wwwwroot</para>
        /// </summary>
        static public string WebRootPath => WebHostEnvironment.WebRootPath;
        /// <summary>
        /// The physical path of the output folder
        /// <para>e.g. C:\MyApp\bin\Debug\net9.0\  </para>
        /// </summary>
        static public string BinPath => System.AppContext.BaseDirectory;

        /// <summary>
        /// Returns the Id of the current HTTP request
        /// </summary>
        static public string RequestId
        {
            get
            {
                string Result = Activity.Current?.Id ?? GetHttpContext().TraceIdentifier;
                if (!string.IsNullOrWhiteSpace(Result) && Result.StartsWith('|'))
                    Result = Result.Remove(0, 1);
                return Result;

            }
        }
        /// <summary>
        /// The <see cref="CultureInfo"/> culture of the current request.
        /// <para>CAUTION: The culture of each HTTP Request is set by a lambda in ConfigureServices().
        /// This property here uses that setting to return its value.
        /// </para>
        /// </summary>
        static public CultureInfo Culture
        {
            get
            {
                CultureInfo Result = CultureInfo.GetCultureInfo("en-US");

                HttpContext HttpContext = GetHttpContext();
                if (HttpContext != null)
                {
                    IRequestCultureFeature Feature = HttpContext.Features.Get<IRequestCultureFeature>();
                    if (Feature != null)
                        Result = Feature.RequestCulture.Culture;
                }

                return Result;

            }
        }

    }
}
