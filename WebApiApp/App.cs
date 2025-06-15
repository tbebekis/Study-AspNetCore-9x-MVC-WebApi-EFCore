namespace WebApiApp
{
    static public partial class App
    {

        static public ConfigurationManager Configuration { get; private set; }
        /// <summary>
        /// Application settings, coming from appsettings.json
        /// </summary>
        static public AppSettings Settings => Lib.Settings;

        

    }
}
