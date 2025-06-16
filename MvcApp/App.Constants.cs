namespace MvcApp
{
    static public partial class App
    {
        static public readonly string SAuthCookieName = $"{Assembly.GetEntryAssembly().GetName().Name}.UserCookie";
        static public readonly string SSessionCookieName = $"{Assembly.GetEntryAssembly().GetName().Name}.SessionCookie";
 
        /// <summary>
        /// Constant
        /// </summary>
        public const string PolicyAuthenticated = "Authenticated";
    }
}
