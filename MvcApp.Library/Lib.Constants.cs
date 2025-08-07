namespace MvcApp.Library
{
    static public partial class Lib
    {
        /// <summary>
        /// Authenticated Visitor/User cookie name.
        /// </summary>
        static public readonly string SAuthCookieName = $"{Assembly.GetEntryAssembly().GetName().Name}.AuthCookie";
        /// <summary>
        /// Not authenticated Visitor cookie name.
        /// </summary>
        static public readonly string SNoAuthCookieName = $"{Assembly.GetEntryAssembly().GetName().Name}.NoAuthCookie";
        /// <summary>
        /// Session cookie name. We store the selected language only
        /// </summary>
        static public readonly string SSessionCookieName = $"{Assembly.GetEntryAssembly().GetName().Name}.SessionCookie";

        /// <summary>
        /// Constant
        /// </summary>
        public const string PolicyAuthenticated = "Authenticated";

        /// <summary>
        /// The default culture code, e.g. en-US
        /// </summary>
        public const string SDefaultCultureCode = "en-US";
        /// <summary>
        /// The default currency code, e.g. EUR
        /// </summary>
        public const string SDefaultCurrencyCode = "EUR";
        /// <summary>
        /// The default currency symbol, e.g. €
        /// </summary>
        public const string SDefaultCurrencySymbol = "€";

    }
}
 