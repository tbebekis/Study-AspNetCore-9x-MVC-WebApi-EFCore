namespace Plugin.Test
{
    /// <summary>
    /// Represents the test library
    /// </summary>
    static public partial class TestLib
    {

        /// <summary>
        /// Returns the url to a static file in the plugin folder.
        /// <para>e.g. <c>~/Plugins/PLUGIN_NAME/wwwroot/css/plugin.css</c></para>
        /// <para>where FilePath is <c>css/plugin.css</c></para>
        /// </summary>
        static public string GetStaticFileUrl(string FilePath)
        {
            return Plugin.Descriptor.GetStaticFileUrl(FilePath);
        }
        
        /// <summary>
        /// Add resource providers to <see cref="tp.Res"/> static class.
        /// </summary>
        static public void AddResourceProviders()
        {
            // NOTE: use Res.Add() to add a resource provider for this library
        }

        static public IMvcAppPlugin Plugin { get; internal set; }
 
    }
}
