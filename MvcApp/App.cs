namespace MvcApp
{
    /// <summary>
    /// Represetns this application
    /// </summary>
    static public partial class App
    {
#pragma warning disable CS0649  
        static ConfigurationManager Configuration;
#pragma warning restore CS0649  

        /// <summary>
        /// Initializes other libraries and plugins
        /// </summary>
        static void Initialize()
        {
            AjaxRequestHandlers.Initialize();
        }
 
    }
}
