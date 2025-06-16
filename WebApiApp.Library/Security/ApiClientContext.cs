namespace WebApiApp.Library
{
    /// <summary>
    /// Request context for JWT clients.
    /// <para>NOTE: This is a Scoped Service (i.e. one instance per HTTP Request) </para>
    /// </summary>
    public class ApiClientContext
    {
        string fCultureCode;

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public ApiClientContext(IHttpContextAccessor HttpContextAccessor)
        {
            this.HttpContext = HttpContextAccessor.HttpContext;
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
        /// </summary>
        public string CultureCode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fCultureCode))
                {
                    // read the token from HTTP headers
                    JwtSecurityToken Token = TokenHelper.ReadTokenFromRequestHeader(HttpContext);
                    if (Token != null)
                        fCultureCode = TokenHelper.GetCultureCode(Token);
                }

                return !string.IsNullOrWhiteSpace(fCultureCode) ? fCultureCode : Lib.Settings.Defaults.CultureCode;
            }

        }
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
                    Result = HttpContext.User.Identity.AuthenticationType == JwtBearerDefaults.AuthenticationScheme;
                }

                return Result;
            }
        }
    }
}
