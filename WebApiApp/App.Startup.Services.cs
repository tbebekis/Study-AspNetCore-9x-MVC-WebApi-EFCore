namespace WebApiApp
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
            builder.Services.AddScoped<ApiClientContext>();
            builder.Services.AddScoped<ApiClientService>();
            builder.Services.AddScoped(typeof(AppDataService<>));
            builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();  

            PermissionAuthorizationHandler.GetUserPermissionsFunc = RBAC.GetUserPermissionListForWebApi;
            

            // ● global exception handler
            builder.Services.AddExceptionHandler<WebApiExceptionHandler>();
            builder.Services.AddProblemDetails();

            // ● HttpContext - NOTE: is singleton
            builder.Services.AddHttpContextAccessor();

            // ● ActionContext - see: https://github.com/aspnet/mvc/issues/3936
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();  // see: https://github.com/aspnet/mvc/issues/3936

            // ● Memory Cache - NOTE: is singleton
            // NOTE: Distributed Cache is required for Session to function properly
            // SEE: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state#configure-session-state
            builder.Services.AddDistributedMemoryCache(); // AddMemoryCache(); 

            // ● Authentication  
            AuthenticationBuilder AuthBuilder = builder.Services.AddAuthentication(o => {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = o.DefaultScheme;
                o.DefaultChallengeScheme = o.DefaultScheme;
            });

            AuthBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o => {

                JwtSettings Jwt = Lib.Settings.Jwt;

                TokenValidationParameters ValidationParams = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = Jwt.Issuer,                  
                    ValidAudiences = new List<string> { Jwt.Audience },                    
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.EncryptionKey)), 
                };
            
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = ValidationParams;

                o.Events = new ApiClientJwtBearerEvents();

                /// <para>The JWT token handler of Asp.Net Core, by default, maps inbound claims using a certain logic.</para>
                /// <para>This default mapping happens when MapInboundClaims = true; which is the default.</para>
                /// <para>By default the JWT token handler, maps, for example, the JwtRegisteredClaimNames.Sub 
                /// to http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier claim.</para>
                /// <para>Setting <see cref="JwtBearerOptions.MapInboundClaims"/> to false disables that default claim mapping.</para>
                /// <para>Another way to check the inbound claims, as they are, i.e. without any mapping applied,
                /// is to read the Token string from HTTP Authorization header
                /// as the Tokens.ReadTokenFromRequestHeader() does.</para>
                /// <para>SEE: https://stackoverflow.com/a/68253821/1779320</para>
                /// <para>SEE: https://stackoverflow.com/a/62477483/1779320</para>
                // o.MapInboundClaims = false;
            });

            // ● Authorization  
            builder.Services.AddAuthorization();

            // ● OpenApi
            /// in the launchSettings.json
            /// set "launchBrowser": true,
            /// add "launchUrl": "scalar/v1",
            builder.Services.AddOpenApi();
            //builder.Services.AddEndpointsApiExplorer(); // https://localhost:7025/swagger
            // The Microsoft.AspNetCore.OpenApi document generator doesn't use the MVC JSON options.
            // SEE: https://github.com/scalar/scalar/discussions/5828
            builder.Services.ConfigureHttpJsonOptions(options => SetupJsonSerializerOptions(options.SerializerOptions));

            // ● Controllers
            IMvcBuilder MvcBuilder = builder.Services.AddControllers(options => {
                options.Filters.Add<ActionExceptionFilter>();
                options.ModelBinderProviders.Insert(0, new ModelBinderProvider());
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = false;                    // https://learn.microsoft.com/en-us/aspnet/core/web-api/#disable-automatic-400-response
                options.SuppressInferBindingSourcesForParameters = false;           // https://learn.microsoft.com/en-us/aspnet/core/web-api/#disable-inference-rules
                options.SuppressConsumesConstraintForFormFileParameters = true;     // https://learn.microsoft.com/en-us/aspnet/core/web-api/#multipartform-data-request-inference
                options.SuppressMapClientErrors = true;                             // https://learn.microsoft.com/en-us/aspnet/core/web-api/#disable-problemdetails-response
                options.ClientErrorMapping[StatusCodes.Status404NotFound].Link =
                    "https://httpstatuses.com/404"; 
            });

            MvcBuilder.AddJsonOptions(options => SetupJsonSerializerOptions(options.JsonSerializerOptions));
        }
    }
}
 