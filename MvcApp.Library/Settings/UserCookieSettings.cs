namespace MvcApp.Library
{
    /// <summary>
    /// User cookie settings
    /// </summary>
    public class UserCookieSettings
    {
 
        /// <summary>
        /// Indicates if the cookie is essential for the application to function correctly.
        /// <para>If true then consent policy checks may be bypassed.</para>
        /// <para>Default is true.</para>
        /// <para>
        /// NOTE: If the CookiePolicyOptions.CheckConsentNeeded is set to true in the ConfigureServices()
        /// then the CookieOptions.IsEssential must be set to true too.
        /// Otherwise the cookie is considered a non-essential one
        /// and it will not being sent to the browser (no Set-Cookie header) without the user's explicit permission.
        /// SEE: https://stackoverflow.com/questions/52456388/net-core-cookie-will-not-be-set 
        /// </para>
        /// </summary>
        public bool IsEssential { get; set; } = true;
        /// <summary>
        /// Indicates whether a cookie is inaccessible by client-side script.
        /// <para>True means the cookie must not be accessible.</para>
        /// <para>Default is true.</para>
        /// </summary>
        public bool HttpOnly { get; set; } = true;

        /// <summary>
        /// Incicates whether the cookie should be included by the client in cross-site requests.
        /// <para>Default is SameSiteMode.Strict, meaning that the cookie will not be included by the client in cross-site requests.</para>
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Microsoft.AspNetCore.Http.SameSiteMode SameSite { get; set; } = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
        /// <summary>
        /// How many hours to keep visitor cookie valid.
        /// <para> -1 = never expires, 0 = expire immediately, nnn = expire after nnn hours</para>
        /// <para>Default is -1 (never expires)</para>
        /// </summary>
        public int ExpirationHours { get; set; } = -1;
    }
}
