namespace MvcApp.Library
{
 
    /// <summary>
    /// Request context for MVC users.
    /// <para>NOTE: This is a Scoped Service (i.e. one instance per HTTP Request) </para>
    /// </summary>
    public class MvcUserContext
    {
 
        List<Claim> UserClaimList => this.HttpContext.User.Claims.ToList();

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public MvcUserContext(IHttpContextAccessor HttpContextAccessor)
        {
            this.HttpContext = HttpContextAccessor.HttpContext;
        }

        // ● public 
        /// <summary>
        /// Sign-in. Authenticates a specified, already validated, Visitor
        /// </summary>
        public async Task SignInAsync(IAppUser User, bool IsPersistent, bool IsImpersonation)
        {
            // await Task.CompletedTask;
            this.IsImpersonation = IsImpersonation;

            ClaimsPrincipal Principal = UserClaims.CreateUserPrincipal(User, CookieAuthenticationDefaults.AuthenticationScheme, IsImpersonation);

            // properties
            AuthenticationProperties AuthProperties = new AuthenticationProperties();
            AuthProperties.AllowRefresh = true;
            AuthProperties.IssuedUtc = DateTime.UtcNow;
            //AuthProperties.ExpiresUtc = DateTime.UtcNow.AddDays(30); // overrides the ExpireTimeSpan option of CookieAuthenticationOptions set with AddCookie
            AuthProperties.IsPersistent = IsPersistent;

            // authenticate the principal under the scheme
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, Principal, AuthProperties);
        }
        /// <summary>
        /// Sign-out.
        /// </summary>
        public async Task SignOutAsync()
        {
            IsImpersonation = false;

            string Scheme = CookieAuthenticationDefaults.AuthenticationScheme;

            await HttpContext.SignOutAsync(Scheme);
        }


        // ● properties
        /// <summary>
        /// The http context
        /// </summary>
        public HttpContext HttpContext { get; }
        /// <summary>
        /// The http request
        /// </summary>
        public HttpRequest Request => HttpContext.Request;
        /// <summary>
        /// The query string as a collection of key-value pairs
        /// </summary>
        public IQueryCollection Query => Request.Query;
 
        /// <summary>
        /// The culture (language) of the current request specified as a culture code (en-US, el-GR)
        /// The <see cref="CultureInfo.Name"/> culture of the current request.
        /// <para>CAUTION: The culture of each HTTP Request is set by a lambda in ConfigureServices().
        /// This property here uses that setting to return its value.
        /// </para>
        /// </summary>
        public string CultureCode => Lib.Culture.Name;
 
        /// <summary>
        /// True when the request is authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                bool Result = HttpContext.User.Identity.IsAuthenticated;
                if (Result)
                {
                    Result = HttpContext.User.Identity.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme;
                }

                return Result;
            }
        }
        /// <summary>
        /// True when the Visitor has loged-in usin the SuperUserPassword
        /// </summary>
        public bool IsImpersonation
        {
            get { return UserClaims.GetIsUserImpersonation(UserClaimList); }
            private set { Session.Set<bool>("IsImpersonation", value); }
        }
    }
}
