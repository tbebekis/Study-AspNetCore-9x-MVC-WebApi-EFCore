namespace MvcApp.Library
{

    /// <summary>
    /// Application settings.
    /// </summary>
    public class AppSettings  
    {
        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public AppSettings()
        {
        }

        // ● properties
        /// <summary>
        /// Whether or not to use Cookie authentication
        /// </summary>
        public bool UseAuthentication { get; set; } = true;
        /// <summary>
        /// The theme currently in use. Defaults to null or empty string, meaning no themes at all.
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// The session timeout in minutes
        /// </summary>
        public int SessionTimeoutMinutes { get; set; } = 30;
 
 
        /// <summary>
        /// Default settings
        /// </summary>
        public DefaultSettings Defaults { get; set; } = new DefaultSettings();
        /// <summary>
        /// User cookie settings
        /// </summary>
        public UserCookieSettings UserCookie { get; set; } = new UserCookieSettings();
        /// <summary>
        /// Http related settings
        /// </summary>
        public HttpSettings Http { get; set; } = new HttpSettings();
        /// <summary>
        /// HSTS settings
        /// <para>SEE: https://en.wikipedia.org/wiki/HTTP_Strict_Transport_Security </para>
        /// </summary>
        public HSTSSettings HSTS { get; set; } = new HSTSSettings();
        /// <summary>
        /// SEO related settings
        /// </summary>
        public SeoSettings Seo { get; set; } = new SeoSettings();

 
    }
}
