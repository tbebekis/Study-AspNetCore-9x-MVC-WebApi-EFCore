namespace MvcApp.Library
{
    /// <summary>
    /// Plugin Definition extensions
    /// </summary>
    static public class MvcAppPluginDefExtensions
    {
        /// <summary>
        /// Returns the content root url to the plugin folder, <strong>without leading slash</strong>.
        /// <para>For example, the path: </para>
        /// <para><c>C:\PROJECT-PATH\bin\Debug\Plugins\Plugin.Test </c></para>
        /// <para>becomes: </para>
        /// <para><c>Plugins/Plugin.Test</c></para>
        /// </summary>
        static public string GetContentRootUrl(this MvcAppPluginDef Def)
        {
            string Result = Def.PluginFolderPath.Replace(Lib.BinPath, string.Empty)
                                      .TrimStart('\\')
                                      .TrimEnd('\\')
                                      .TrimStart('/')
                                      .TrimEnd('/')
                                      .Replace('\\', '/');    

            return Result;
        }
        /// <summary>
        /// Returns the url to a static file in the plugin folder.
        /// <para>e.g. <c>~/Plugins/PLUGIN_NAME/wwwroot/css/plugin.css</c></para>
        /// <para>where FilePath is <c>css/plugin.css</c></para>
        /// </summary>
        static public string GetStaticFileUrl(this MvcAppPluginDef Def, string FilePath)
        {
            // <link rel="stylesheet" href="~/Plugins/PLUGIN_NAME/wwwroot/css/plugin.css" />
            string Result = $"{Def.WebRootUrl}/{FilePath}";
            return Result;
        }
    }
}
