namespace MvcApp
{
    /// <summary>
    /// Represetns this application
    /// </summary>
    static public partial class App
    {
        static ConfigurationManager Configuration;

        /// <summary>
        /// Initializes other libraries and plugins
        /// </summary>
        static void Initialize()
        {
            App.InitializePlugins();
            AjaxRequestHandlers.Initialize();
        }
 
    }
}
