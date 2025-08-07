namespace CommonLib
{
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.Primitives;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Web and Asp.Net helper
    /// </summary>
    static public class WebLib
    {
        /* ●  private */
        /// <summary>
        /// FROM: https://stackoverflow.com/questions/13086856/mobile-device-detection-in-asp-net
        /// </summary>
        static Regex MobileCheck = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        /// <summary>
        /// FROM: https://stackoverflow.com/questions/13086856/mobile-device-detection-in-asp-net
        /// </summary>
        static Regex MobileVersionCheck = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        /// <summary>
        /// FROM: https://stackoverflow.com/questions/7576508/how-to-detect-crawlers-in-asp-net-mvc
        /// </summary>
        static Regex CrawlerCheck = new Regex(@"bot|crawler|baiduspider|80legs|ia_archiver|voyager|curl|wget|yahoo! slurp|mediapartners-google", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);


        // ● initialization
        /// <summary>
        /// Initializes this class
        /// </summary>
        static public void Initialize(IServiceProvider RootServiceProvider,
                                        IHttpContextAccessor HttpContextAccessor,
                                        IWebHostEnvironment WebHostEnvironment,
                                        ConfigurationManager Configuration)
        {
            if (WebLib.RootServiceProvider == null)
            {
                WebLib.RootServiceProvider = RootServiceProvider;
                WebLib.HttpContextAccessor = HttpContextAccessor;
                WebLib.Configuration = Configuration;
                WebLib.WebHostEnvironment = WebHostEnvironment;

    
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
        /// Returns the current <see cref="ActionContext"/>.
        /// <para>WARNING: It should be called only when a valid <see cref="Microsoft.AspNetCore.Http.HttpRequest"/> exists. </para>
        /// </summary>
        static public ActionContext GetActionContext()
        {
            IActionContextAccessor service = GetService<IActionContextAccessor>();
            return (service != null) ? service.ActionContext : null;
        }
        /// <summary>
        /// Returns an <see cref="IUrlHelper"/>.
        /// <para>WARNING: It should be called only when a valid <see cref="Microsoft.AspNetCore.Http.HttpRequest"/> exists. </para>
        /// </summary>
        static public IUrlHelper GetUrlHelper()
        {
            ActionContext context = GetActionContext();
            IUrlHelperFactory factory = GetService<IUrlHelperFactory>();
            return (context != null && factory != null) ? factory.GetUrlHelper(context) : null;
        }

        // ● query string  
        /// <summary>
        /// Returns the current <see cref="Microsoft.AspNetCore.Http.HttpContext.Request.Query"/>
        /// </summary>
        static public IQueryCollection GetQuery() => GetHttpRequest().Query;
        /// <summary>
        /// Returns a value from query string, if any, else returns a default value.
        /// </summary>
        static public string GetQueryValue(string Key, string Default = "")
        {
            try
            {
                IQueryCollection QS = GetQuery();
                return QS != null && QS.ContainsKey(Key) ? QS[Key].ToString() : Default;
            }
            catch
            {
            }

            return Default;

        }
        /// <summary>
        /// Returns a value from query string, if any, else returns a default value.
        /// </summary>
        static public int GetQueryValue(string Key, int Default = 0)
        {
            try
            {
                string S = GetQueryValue(Key, "");
                return !string.IsNullOrWhiteSpace(S) ? Convert.ToInt32(S) : Default;
            }
            catch
            {
            }

            return Default;
        }
        /// <summary>
        /// Returns a value from query string, if any, else returns a default value.
        /// </summary>
        static public bool GetQueryValue(string Key, bool Default = false)
        {
            try
            {
                string S = GetQueryValue(Key, "");
                return !string.IsNullOrWhiteSpace(S) ? Convert.ToBoolean(S) : Default;
            }
            catch
            {
            }

            return Default;

        }

        /// <summary>
        /// Returns the value of a query string parameter.
        /// <para>NOTE: When a parameter is included more than once, e.g. ?page=1&amp;page=2 then the result will be 1,2 hence this function returns an array.</para>
        /// </summary>
        static public string[] GetQueryValueArray(string Key)
        {
            try
            {
                IQueryCollection QS = GetQuery();
                return QS[Key].ToArray();
            }
            catch
            {
            }

            return new string[0];
        }

        // ● URL combine 
        /// <summary>
        /// Combines the url base and the relative url into one, consolidating the '/' between them
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="relativeUrl">The relative path to combine</param>
        /// <returns>The merged url</returns>
        static public string UrlCombine(string baseUrl, string relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (string.IsNullOrWhiteSpace(relativeUrl))
                return baseUrl;

            baseUrl = baseUrl.TrimEnd('/');
            relativeUrl = relativeUrl.TrimStart('/');

            return $"{baseUrl}/{relativeUrl}";
        }
        /// <summary>
        /// Combines the url base and the array of relatives urls into one, consolidating the '/' between them
        /// </summary>
        /// <returns>The merged url</returns>
        static public string UrlCombine(string baseUrl, params string[] relativePaths)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (relativePaths.Length == 0)
                return baseUrl;

            var currentUrl = UrlCombine(baseUrl, relativePaths[0]);

            return UrlCombine(currentUrl, relativePaths.Skip(1).ToArray());
        }

        // ● URL encode 
        /// <summary>
        /// Encodes a URL string.
        /// </summary>
        static public string UrlEncode(string Url)
        {
            return System.Web.HttpUtility.UrlEncode(Url);
        }

        // ● URL escape 
        /// <summary>
        /// Escapes a url using the <see cref="Uri.EscapeUriString(string)"/>.
        /// <para>The <see cref="Uri.EscapeUriString(string)"/> escapes unreserved characters only..</para>
        /// <para>The <see cref="Uri.EscapeDataString(string)"/> escapes unreserved AND reserved charactes.</para>
        /// <para>Reserved Characters: :/?#[]@!$&amp;'()*+,;=  </para>
        /// <para>Unreserved Characters: alphanumeric and -._~ </para>
        /// <para>SEE: https://tools.ietf.org/html/rfc3986#section-2 </para>
        /// </summary>
        static public string UrlEscapeOld(string Url)
        {
#pragma warning disable SYSLIB0013
            return Uri.EscapeUriString(Url);
#pragma warning restore SYSLIB0013
        }
        /// <summary>
        /// Escapes a url using the <see cref="Uri.EscapeDataString(string)"/>.
        /// <para>The <see cref="Uri.EscapeUriString(string)"/> escapes unreserved characters only..</para>
        /// <para>The <see cref="Uri.EscapeDataString(string)"/> escapes unreserved AND reserved charactes.</para>
        /// <para>Reserved Characters: :/?#[]@!$&amp;'()*+,;= </para>
        /// <para>Unreserved Characters: alphanumeric and -._~ </para>
        /// <para>SEE: https://tools.ietf.org/html/rfc3986#section-2 </para>
        /// </summary>
        static public string UrlEscapes(string Url)
        {
            return Uri.EscapeDataString(Url);
        }

        // ● URLs
        /// <summary>
        /// Returns the referrer Url if any, else null.
        /// <para>NOTE: The HTTP referer is an optional HTTP header field that identifies the address of the webpage which is linked to the resource being requested. 
        /// By checking the referrer, the new webpage can see where the request originated</para>
        /// </summary>
        static public string GetReferrerUrl(HttpRequest R = null)
        {
            R = R ?? GetHttpRequest();

            if (R != null)
                return R.Headers[HeaderNames.Referer];

            return null;
        }
        /// <summary>
        /// Returns the client IP address, that is the IP address of the visitor, if any, else null
        /// </summary>
        static public string GetClientIpAddress(HttpRequest R = null)
        {
            string Result = null;

            R = R ?? GetHttpRequest();

            if (R != null)
            {
                // first try to get the IP address from the X-Forwarded-For header
                // SEE: https://en.wikipedia.org/wiki/X-Forwarded-For
                var SV = R.Headers["X-Forwarded-For"];
                if (!StringValues.IsNullOrEmpty(SV))
                    Result = SV.FirstOrDefault();

                // next try the remote IP address
                if (string.IsNullOrWhiteSpace(Result) && R.HttpContext.Connection.RemoteIpAddress != null)
                    Result = R.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            // check to see if it is the IPv6 Loopback address
            if (!string.IsNullOrWhiteSpace(Result) && Result.Equals(IPAddress.IPv6Loopback.ToString(), StringComparison.InvariantCultureIgnoreCase))
                Result = IPAddress.Loopback.ToString();

            // remove the port if there
            if (!string.IsNullOrWhiteSpace(Result) && Result.Contains(':'))
                Result = Result.Split(':').FirstOrDefault();

            return Result;
        }
        /// <summary>
        /// Returns the domain name of the server and the TCP port number on which the server is listening. 
        /// The port number may be omitted if the port is the standard port for the service requested. 
        /// </summary>
        static public string GetHostDomainName(HttpRequest R = null)
        {
            string Result = null;

            R = R ?? GetHttpRequest();

            if (R != null)
                Result = R.Headers[HeaderNames.Host];

            return Result;
        }

        /// <summary>
        /// Returns the scheme of the current request, i.e. https or http
        /// </summary>
        static public string GetRequestProtocol(HttpRequest R = null)
        {
            R = R ?? GetHttpRequest();
            return IsHttps(R) ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
        }

        /// <summary>
        /// Returns the raw relative Url path and query string of a specified request
        /// <note>SEE: https://stackoverflow.com/questions/28120222/get-raw-url-from-microsoft-aspnet-http-httprequest </note>
        /// </summary>
        static public string GetRelativeRawUrl(HttpRequest R = null)
        {
            string Result = null;

            R = R ?? GetHttpRequest();

            Result = R.HttpContext.Features.Get<IHttpRequestFeature>()?.RawTarget;

            // if is empty create it manually
            if (string.IsNullOrWhiteSpace(Result))
                Result = $"{R.PathBase}{R.Path}{R.QueryString}";

            return Result;
        }
        /// <summary>
        /// Returns the relative Url of a request, along with the Query String, url-encoded.
        /// <note>SEE: https://stackoverflow.com/questions/28120222/get-raw-url-from-microsoft-aspnet-http-httprequest </note>
        /// </summary>
        static public string GetRelativeRawUrlEncoded(HttpRequest R = null)
        {
            R = R ?? GetHttpRequest();

            // use the IHttpRequestFeature   
            // the returned value is not UrlDecoded
            string Result = R.HttpContext.Features.Get<IHttpRequestFeature>()?.RawTarget;

            // if empty string, then build the URL manually
            if (string.IsNullOrEmpty(Result))
                Result = $"{R.PathBase}{R.Path}{R.QueryString}";

            return Result;
        }

        /// <summary>
        /// Returns the absolute Url of a request, along with the Query String, url-encoded.
        /// <para>Suitable for use in HTTP headers and other HTTP operations.</para>
        /// </summary>
        static public string GetAbsoluteUrlEncoded(HttpRequest R = null)
        {
            R = R ?? GetHttpRequest();

            return R.GetEncodedUrl();
        }
        /// <summary>
        /// Returns the combined components of the request URL in a fully un-escaped form (except for the QueryString) suitable only for display. 
        /// <para>This format should not be used in HTTP headers or other HTTP operations.</para>
        /// </summary>
        static public string GetAbsoluteDisplayUrl(HttpRequest R = null)
        {
            R = R ?? GetHttpRequest();

            return R.GetDisplayUrl();
        }

        /// <summary>
        /// Returns the absolute Url (e.g. containing scheme and host name) of a specified Route name
        /// </summary>
        static public string GetAbsoluteRouteUrl(IUrlHelper UrlHelper, string RouteName, object RouteValues = null)
        {
            string Scheme = GetHttpRequest().Scheme;
            return UrlHelper.RouteUrl(RouteName, RouteValues, Scheme);
        }


        // ● Ajax
        /// <summary>
        /// Returns true when the request is an Ajax request
        /// </summary>
        static public bool IsAjaxRequest(HttpRequest R = null)
        {

            /// X-Requested-With -> XMLHttpRequest is invalid in cross-domain call
            /// 
            /// Only the following headers are allowed across origins:
            ///     Accept
            ///     Accept-Language
            ///     Content-Language
            ///     Last-Event-ID
            ///     Content-Type


            R = R ?? GetHttpRequest();
            return string.Equals(R.Query[HeaderNames.XRequestedWith], "XMLHttpRequest", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(R.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.InvariantCultureIgnoreCase);
        }
        /// <summary>
        /// Returns true if the RequestScheme is https.
        /// </summary>
        static public bool IsHttps(HttpRequest R = null)
        {
            R = R ?? GetHttpRequest();

            if (R != null)
            {
                return R.Headers["X-Forwarded-Proto"].ToString().Equals("https", StringComparison.OrdinalIgnoreCase)
                    || R.IsHttps;
            }

            return false;
        }
        /// <summary>
        /// Gets whether the specified HTTP request URI references the local host.
        /// </summary>
        /// <param name="R">HTTP request</param>
        /// <returns>True, if HTTP request URI references to the local host</returns>
        static public bool IsLocalRequest(HttpRequest R = null)
        {
            R = R ?? GetHttpRequest();

            if (R != null)
            {
                // SEE: https://stackoverflow.com/questions/35240586/in-asp-net-core-how-do-you-check-if-request-is-local/
                ConnectionInfo CI = R.HttpContext.Connection;
                if (CI.RemoteIpAddress != null)
                {
                    return CI.LocalIpAddress != null ? CI.RemoteIpAddress.Equals(CI.LocalIpAddress) : IPAddress.IsLoopback(CI.RemoteIpAddress);
                }
            }


            return true;
        }
        /// <summary>
        /// Returns true if we are dealing with a mobile device/browser
        /// <para>FROM: https://stackoverflow.com/questions/13086856/mobile-device-detection-in-asp-net </para>
        /// </summary>
        static public bool IsMobile(HttpRequest R = null)
        {
            R = R ?? GetHttpRequest();

            if (R != null)
            {
                string S = R.Headers[Microsoft.Net.Http.Headers.HeaderNames.UserAgent].ToString();
                return S.Length >= 4 && (MobileCheck.IsMatch(S) || MobileVersionCheck.IsMatch(S.Substring(0, 4)));
            }

            return false;
        }
        /// <summary>
        /// Returns true if we are dealing with a search endine bot.
        /// <para>FROM: https://stackoverflow.com/questions/7576508/how-to-detect-crawlers-in-asp-net-mvc  </para>
        /// </summary>
        static public bool IsCrawler(HttpRequest R = null)
        {
            R = R ?? GetHttpRequest();

            if (R != null)
            {
                string S = R.Headers[Microsoft.Net.Http.Headers.HeaderNames.UserAgent].ToString();
                return S.Length >= 4 && CrawlerCheck.IsMatch(S);
            }

            return false;
        }

        // ● miscs
        /// <summary>
        /// Returns the MIME Content type based on a filename
        static public string GetMimeType(string FileName)
        {
            string Result = "";
            var Provider = new FileExtensionContentTypeProvider();

            if (!Provider.TryGetContentType(FileName, out Result))
                Result = "application/octet-stream";

            return Result;
        }

        // ● file results
        /// <summary>
        /// <see cref="FileStreamResult"/> represents an <see cref="ActionResult"/> that when executed will
        /// write a file from a stream to the response.
        /// <para><see cref="FileStreamResult"/> inherits from <see cref="FileResult"/>.</para>
        /// </summary>
        static public FileStreamResult ToFileStreamResult(string PhysicalFilePath)
        {
            string ContentType = GetMimeType(PhysicalFilePath);
            FileStream fileStream = new FileStream(PhysicalFilePath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fileStream, ContentType);
        }
        /// <summary>
        /// <see cref="FileContentResult"/> represents an <see cref="ActionResult"/> that when executed will
        /// write a binary file to the response.
        /// <para><see cref="FileContentResult"/> inherits from <see cref="FileResult"/>.</para>
        /// </summary>
        static public FileContentResult ToFileContentResult(string PhysicalFilePath)
        {
            string ContentType = GetMimeType(PhysicalFilePath);
            byte[] Buffer = System.IO.File.ReadAllBytes(PhysicalFilePath);
            return new FileContentResult(Buffer, ContentType);
        }
        /// <summary>
        /// <see cref="VirtualFileResult"/> represents an <see cref="ActionResult"/> that on execution 
        /// writes the file specified using a virtual path to the response
        /// using mechanisms provided by the host.
        /// <para><see cref="VirtualFileResult"/> inherits from <see cref="FileResult"/>.</para>
        /// </summary>
        static public VirtualFileResult ToVirtualFileResult(string VirtualFilePath)
        {
            string ContentType = GetMimeType(VirtualFilePath);
            return new VirtualFileResult(VirtualFilePath, ContentType);
        }


        /// <summary>
        /// Returns a <see cref="FileContentResult"/> for downloading a file or null.
        /// <para>Null is returned when no data is passed (null or length = 0) and the file does not exist.</para>
        /// <para>NOTE: If no binary Data is specified then the function tries to load the binary data from the specified file path. </para>
        /// <para>CAUTION: FilePath is mandatory.</para>
        /// </summary>
        static public FileContentResult GetFileContentResult(string FilePath, byte[] Data = null)
        {
            FileContentResult Result = null;

            if (!string.IsNullOrWhiteSpace(FilePath))
            {
                if (Data == null || Data.Length <= 0)
                {
                    if (File.Exists(FilePath))
                        Data = File.ReadAllBytes(FilePath);
                }

                if (Data != null && Data.Length > 0)
                {
                    string FileName = Path.GetFileName(FilePath);
                    string ContentType;
                    new FileExtensionContentTypeProvider().TryGetContentType(FileName, out ContentType);
                    ContentType = ContentType ?? "application/octet-stream";

                    Result = new FileContentResult(Data, ContentType);
                    Result.FileDownloadName = FileName;
                }
            }

            return Result;

        }
        /// <summary>
        /// Returns a <see cref="FileContentResult"/> for downloading a file or null.
        /// <para>Null is returned when no data is passed (null or length = 0) and the file does not exist.</para>
        /// <para>NOTE: If no binary Data is specified then the function tries to load the binary data from the specified file path. </para>
        /// <para>CAUTION: FilePath is mandatory.</para>
        /// </summary>
        static public FileContentResult GetFileContentResult(string FilePath, Stream Stream)
        {
            byte[] Data = Stream.ToArray();
            return GetFileContentResult(FilePath, Data);
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
        /// Returns the base url of this application.
        /// <para>CAUTION: There should be a valid HttpContext in order to be able to return the base url.</para>
        /// </summary>
        static public string BaseUrl
        {
            get
            {
                HttpContext HttpContext = GetHttpContext();
                if (HttpContext != null)
                {
                    string Scheme = HttpContext.Request.Scheme;
                    string Host = HttpContext.Request.Host.Host;
                    string Port = HttpContext.Request.Host.Port.HasValue && HttpContext.Request.Host.Port != 80 && HttpContext.Request.Host.Port != 443 ? $":{HttpContext.Request.Host.Port}" : "";

                    return $"{Scheme}://{Host}{Port}";
                }

                return string.Empty;
            }
        }

        // ● properties - paths
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
    }
}
