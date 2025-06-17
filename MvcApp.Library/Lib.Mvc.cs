namespace MvcApp.Library
{
    static public partial class Lib
    {
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

        // ● Ajax
        /// <summary>
        /// Returns true when the request is an Ajax request
        /// </summary>
        static public bool IsAjaxRequest(HttpRequest R = null)
        {
            /*
            X-Requested-With -> XMLHttpRequest is invalid in cross-domain call
           
            Only the following headers are allowed across origins:
                Accept
                Accept-Language
                Content-Language
                Last-Event-ID
                Content-Type
            */

            R = R ?? GetHttpRequest();
            return string.Equals(R.Query[HeaderNames.XRequestedWith], "XMLHttpRequest", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(R.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.InvariantCultureIgnoreCase);
        }
        
        // ● Url
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

    }
}
