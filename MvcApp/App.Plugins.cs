namespace MvcApp
{
    static public partial class App
    { 
        /// <summary>
        /// Removes files from a specified plugin folder
        /// </summary>
        static void CleanPluginFolder(string PluginFolderPath)
        {
            string[] Patterns = {"Plug.WebLib.*", "*.pdb", "*.deps.json" };
            string[] FilePaths;
            foreach (string Pattern in Patterns)
            {
                FilePaths = Directory.GetFiles(PluginFolderPath, Pattern);
                if (FilePaths != null && FilePaths.Length > 0)
                {
                    foreach (string FilePath in FilePaths)
                    {
                        if (File.Exists(FilePath))
                            File.Delete(FilePath);
                    }
                }
            }      
        }

        /// <summary>
        /// Loads plugin definitions into <see cref="PluginDefList"/> list.
        /// </summary>
        static void LoadPluginDefinitions()
        {
            string RootPluginFolder = Path.Combine(Lib.BinPath, "Plugins");
            string[] PluginFolders = Directory.GetDirectories(RootPluginFolder);

            // load plugin definitions and assemblies
            DirectoryInfo DI;
            string FolderName;
            foreach (string PluginFolderPath in PluginFolders)
            {
                DI = new DirectoryInfo(PluginFolderPath);
                FolderName = DI.Name;
                if (FolderName.StartsWith("Plugin."))
                {
                    // load the plugin definition
                    CleanPluginFolder(PluginFolderPath);
                    string PluginDefFilePath = Path.Combine(PluginFolderPath, "plugin-def.json");
                    MvcAppPluginDef Def = new MvcAppPluginDef(PluginFolderPath);

                    Json.LoadFromFile(Def, PluginDefFilePath);

                    // find plugin assembly file path  
                    string[] FilePaths = Directory.GetFiles(PluginFolderPath, "Plugin.*.dll");
                    if (FilePaths == null || FilePaths.Length == 0)
                        throw new Exception($"No Plugin Assembly found in folder: {PluginFolderPath}");
                    Def.PluginAssemblyFilePath = FilePaths[0];
                    Def.Id = Path.GetFileName(Def.PluginAssemblyFilePath);

                    PluginDefList.Add(Def);
                }
            }

            // sort definition list
            PluginDefList = PluginDefList.OrderBy(item => item.LoadOrder).ToList();
        }
        /// <summary>
        /// Loads plugin assemblies and creates the <see cref="IMvcAppPlugin"/> instances and adds them to the <see cref="PluginList"/>.
        /// <para>After this method the <see cref="MvcAppPluginDef.PluginAssembly"/> property is set with the loaded plubin <see cref="Assembly"/></para>
        /// </summary>
        static void LoadPluginAssemblies()
        {
            // create plugins
            List<Type> ImplementorClassTypes;
            foreach (MvcAppPluginDef Def in PluginDefList)
            {
                // load the assembly and the application part for that assembly
                Def.PluginAssembly = Assembly.LoadFrom(Def.PluginAssemblyFilePath);
 
                ImplementorClassTypes = TypeFinder.FindImplementorClasses(typeof(IMvcAppPlugin), Def.PluginAssembly);
                if (ImplementorClassTypes.Count == 0)
                    throw new Exception($"Plugin: {Def.Id} does not implement IAppPlugin");

                if (ImplementorClassTypes.Count > 1)
                    throw new Exception($"Plugin: {Def.Id} implements more than one IAppPlugin");

                IMvcAppPlugin Plugin = (IMvcAppPlugin)Activator.CreateInstance(ImplementorClassTypes[0]);
                Plugin.Descriptor = Def;
                PluginList.Add(Plugin);
            }
        }
        /// <summary>
        /// Adds plugin assemblies to the <see cref="ApplicationPartManager"/>
        /// </summary>
        static void AddPluginsToApplicationPartManager()
        {
            foreach (var Def in PluginDefList)
            {
                ApplicationPart Part = new AssemblyPart(Def.PluginAssembly);
                PartManager.ApplicationParts.Add(Part);
            }
        }

        /// <summary>
        /// Load, create and initialize plugins
        /// </summary>
        static void LoadPlugins()
        {
            LoadPluginDefinitions();
            LoadPluginAssemblies();
            AddPluginsToApplicationPartManager();

            foreach (IMvcAppPlugin Plugin in PluginList)
            {
                Plugin.Initialize();
                Plugin.AddViewLocations();
            }
        }

        // ● properties
        /// <summary>
        /// List of plugin definitions
        /// </summary>
        static List<MvcAppPluginDef> PluginDefList { get; set; } = new List<MvcAppPluginDef>();
        /// <summary>
        /// List of plugins
        /// </summary>
        static List<IMvcAppPlugin> PluginList { get; } = new List<IMvcAppPlugin>();
        /// <summary>
        /// Gets the <see cref="ApplicationPartManager"/> where <see cref="ApplicationPart"/>s
        /// are configured.
        /// </summary>
        static ApplicationPartManager PartManager { get; set; }
    }
}
