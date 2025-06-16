namespace MvcApp.Library
{
    /// <summary>
    /// Represents a plugin
    /// </summary>
    public interface IMvcAppPlugin
    {

        /// <summary>
        /// Initializes the plugin.
        /// <para>Here plugin may call the <c>Lib.ObjectMapper.Add(Type Source, Type Dest, bool TwoWay)</c> in order to add mappings to the <see cref="Lib.ObjectMapper"/></para>
        /// </summary>
        public void Initialize();

        /// <summary>
        /// Use the <c>static</c> <see cref="ViewLocationExpander.AddViewLocation(string)"/> method to add locations.
        /// </summary>
        void AddViewLocations();
        /// <summary>
        /// The plugin descriptor
        /// </summary>
        MvcAppPluginDef Descriptor { get; set; }
    }
}
