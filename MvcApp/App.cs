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

            /*
            if (App.MvcAppContext == null)
            {
                
                App.AppContext = new AppContext();
                WLib.Initialize(App.AppContext);
                Lib.Initialize(App.AppContext, App.AppContext.Cache);
                App.InitializePlugins();

                AjaxRequestHandlers.Initialize();
            }
            */
        }

        // ● properties
 
    }
}
