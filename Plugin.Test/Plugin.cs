namespace Plugin.Test
{
    /// <summary>
    /// Represents this plugin
    /// </summary>
    public class Plugin: IMvcAppPlugin
    {
        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public Plugin()
        {
            TestLib.Plugin = this;
            
        }

        // ● public
        /// <summary>
        /// Initializes the plugin.
        /// <para>Here plugin may call the <c>Lib.ObjectMapper.Add(Type Source, Type Dest, bool TwoWay)</c> in order to add mappings to the <see cref="Lib.ObjectMapper"/></para>
        /// </summary>
        public void Initialize()
        {
        }
        /// <summary>
        /// Use the <c>static</c> <see cref="ViewLocationExpander.AddViewLocation(string)"/> method to add locations.
        /// </summary>
        public void AddViewLocations()
        {
            Descriptor.AddPluginViewLocations();
        }

        // ● properties
        /// <summary>
        /// The plugin descriptor
        /// </summary>
        public MvcAppPluginDef Descriptor { get; set; }
    }
}
