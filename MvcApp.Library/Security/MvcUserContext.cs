namespace MvcApp.Library
{
 
    /// <summary>
    /// Request context for MVC users.
    /// <para>NOTE: This is a Scoped Service (i.e. one instance per HTTP Request) </para>
    /// </summary>
    public class MvcUserContext
    {
        // ● private
        Visitor fVisitor;
        VisitorService fVisitorService;
        List<Claim> fUserClaimList;

        List<Claim> UserClaimList => fUserClaimList ?? (fUserClaimList = this.HttpContext.User.Claims.ToList());
        VisitorService VisitorService => fVisitorService ?? (fVisitorService = new VisitorService());




        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public MvcUserContext(IHttpContextAccessor HttpContextAccessor)
        {
            this.HttpContext = HttpContextAccessor.HttpContext;

            GetVisitor();
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

        /// <summary>
        /// Returns the current visitor
        /// </summary>
        public Visitor GetVisitor()
        {
            // ● Registered Visitor
            // check if we have a registered and authenticated Visitor
            // Authenticated AppUser.Id is stored in the authentication cookie (Lib.SAuthCookieName)
            // Visitor.UserId is an AppUser.Id, if not null or empty.
            if (fVisitor == null)
            {
                string UserId = UserClaims.GetUserId(UserClaimList); // it is the AppUser.Id and the Visitor.UserId
                if (!string.IsNullOrWhiteSpace(UserId))
                    fVisitor = VisitorService.GetVisitorByAppUser(UserId);
            }

            // ● Un-registered Visitor
            // check if we have an un-registered Visitor
            // Non-authenticated Visitor.Code is stored in the authentication cookie (Lib.SNoAuthCookieName)
            if (fVisitor == null)
            {
                string Code = Request.Cookies[Lib.SNoAuthCookieName];
                if (!string.IsNullOrWhiteSpace(Code))
                    fVisitor = VisitorService.GetVisitor(Code);
            }

            // ● still no Visitor, 
            // create a new one with the default Visitor properties
            // save it to the DataStore
            // and save it to the not authenticated cookie (Lib.SNoAuthCookieName) too
            if (fVisitor == null)
            {
                fVisitor = VisitorService.CreateNewVisitor();

                this.HttpContext.Response.Cookies.Append(
                    Lib.SNoAuthCookieName,
                    fVisitor.Code,
                    new CookieOptions
                    {
                        Secure = true,
                        SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                        HttpOnly = true,
                        IsEssential = true,
                        Expires = DateTimeOffset.UtcNow.AddYears(1)
                    }
                );
            }

            return fVisitor;
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



        /// <summary>
        /// Returns the <see cref="Baufox.Visitor"/> which is the actual user.
        /// <para><see cref="Baufox.Visitor"/> implements the <see cref="IAppUser"/> interface. </para>
        /// </summary>
        public Visitor Visitor => GetVisitor();


    }
}
