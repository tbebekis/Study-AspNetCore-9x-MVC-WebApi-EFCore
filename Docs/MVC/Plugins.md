# Asp.Net Core Plugins with Views, Javascript and CSS 

> This text is part of a group of texts describing [Asp.Net Core](../Index.md).

A [Plugin](https://en.wikipedia.org/wiki/Plug-in_(computing)) is a software module, external to an application and **dynamically loadable**. A plugin adds or extends the functionality of an application, without requiring an application rebuild.

Asp.Net Core offers the [Razor Class Library](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/ui-class) template project, which may contain Views, Controllers and static files, such as Javascript and CSS. A `Razor Class Library` is a type of a resusable application component. 

But Razor Class Libraries are not designed to be dynamically loadable.

This text describes a way to have dynamically loadable external class libraries containing Views, Controllers and static files, such as Javascript and CSS, as plugins to an Asp.Net Core MVC application.

## Required Projects

The projects required to create a plugin MVC application, are

- the MVC application
- a common class library, where both the MVC application and the plugin libraries, depend on.
- one or more plain class libraries as plugins

The sample application accompanying this text has the following projects, accordingly

- MvcApp
- MvcApp.Library
- Plugin.Test

## The Plugin Library

The plugin library is a plain C# class library with the following folders.

```
Controllers
Views
    Shared
wwwroot
    js
    css
```

## Plugin Project File (*.csproj)

Here is the `Plugin.Test` project file.

```
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net9.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <!-- <Nullable>enable</Nullable> -->
        <AddRazorSupportForMvc>true</AddRazorSupportForMvc>   
        <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
        <AppendRuntimeIdentifierToOutputPath>false</AppendRuntimeIdentifierToOutputPath>
        
        <CopyLocalLockFileAssemblies>false</CopyLocalLockFileAssemblies>
        <OutputPath>$(SolutionDir)\MvcApp\Plugins\$(MSBuildProjectName)</OutputPath>  
    </PropertyGroup>

    <ItemGroup>
        <Content Include="Views\**">
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </Content>
        <Content Include="wwwroot\**">
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>                   
        </Content>
        <Content Include="plugin-def.json">
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </Content>
    </ItemGroup>
 
    <ItemGroup>
        <ProjectReference Include="..\MvcApp.Library\MvcApp.Library.csproj" />
    </ItemGroup>

</Project>
```
 
This project file instructs the build system to

- not append the target framework and runtime identifier to output folders (i.e. not create a net9.0, linux, win-x64, etc. folder)
- set the output folder, for both Debug and Release, to `MvcApp\Plugins\Plugin.Test` folder
- include as `Content` the folders `Views`, `wwwroot` and their contents
- include as `Content` the `plugin-def.json` file
 
## Plugin Definition File

A plugin should contain a `json` file, named `plugin-def.json`, directly under the project folder, with the following content.

```
{
    "Id": "Plugin.Test.dll",
    "Group": "Test Plugins",
    "Description": "Test Plugin",
    "Author": "Test",
    "LoadOrder": 1
}
```

The `plugin-def.json` is loaded and mapped to an instance of the `PluginDef` class contained by the `MvcApp.Library`.

```
    public class PluginDef
    {
        public PluginDef(string PluginFolderPath);
       
        public string Id { get; set; }         
        public string Group { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int LoadOrder { get; set; }

        [JsonIgnore]
        public string PluginFolderPath { get; } 
        [JsonIgnore]
        public string PluginAssemblyFilePath { get; set; }
        [JsonIgnore]
        public Assembly PluginAssembly { get; set; }        

        [JsonIgnore]
        public string ContentRootUrl { get; } 
        [JsonIgnore]
        public string WebRootUrl { get; } 
    }
```

The `MvcApp` uses the `plugin-def.json` files to identify and load plugin libraries dynamically.

## The MvcApp Project File (*.csproj)

```
<Project Sdk="Microsoft.NET.Sdk.Web">

    <PropertyGroup>
        <TargetFramework>net9.0</TargetFramework>
        <!-- <Nullable>enable</Nullable> -->
        <ImplicitUsings>enable</ImplicitUsings>
        <BaseOutputPath>$(SolutionDir)\BinMvc</BaseOutputPath>
    </PropertyGroup>

    <!--  Disable the default behavior of appending the target framework and runtime identifier to the output path. -->
    <PropertyGroup>
        <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
        <AppendRuntimeIdentifierToOutputPath>false</AppendRuntimeIdentifierToOutputPath>
    </PropertyGroup>

    <!--  Disable the default behavior of compiling views into assembly on publish. -->
    <PropertyGroup>
        <MvcRazorCompileOnPublish>false</MvcRazorCompileOnPublish>
        <RazorCompileOnBuild>false</RazorCompileOnBuild>
    </PropertyGroup>

    <ItemGroup>
        <!-- Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation is needed if we are to have Views not embedded into assembly -->
        <PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="9.0.5" />
        <PackageReference Include="Microsoft.AspNetCore.Routing" Version="2.3.0" />
        <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.5" />
        <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.5" />
        <PackageReference Include="Microsoft.Extensions.FileProviders.Abstractions" Version="9.0.5" />
        <PackageReference Include="Microsoft.Extensions.FileProviders.Embedded" Version="9.0.5" />
        <PackageReference Include="Microsoft.Extensions.FileProviders.Physical" Version="9.0.5" />
    </ItemGroup>

    <ItemGroup>
        <!-- For the CopyToOutputDirectory to work just right click on the project | Build Dependencies | Project Dependencies 
             and add all Plugin projects as dependencies.
             Plugin projects have as their output directory the Plugins folder.
             So Plugins must be built first.
             -->
        <None Include="Plugins\**" CopyToOutputDirectory="PreserveNewest" />
        <None Include="Plugins\**" CopyToPublishDirectory="PreserveNewest" />
    </ItemGroup>    

    <ItemGroup>
      <ProjectReference Include="..\MvcApp.Library\MvcApp.Library.csproj" />
    </ItemGroup>    

    <ItemGroup>
      <Folder Include="Plugins\" />
    </ItemGroup>

</Project>

```

This project file instructs the build system to

- output Debug and Release builds to `$(SolutionDir)\BinMvc` folder
- not append the target framework and runtime identifier to output folders (i.e. not create a net9.0, linux, win-x64, etc. folder)
- not compile Views on Publish, so Views, along with Javascript and CSS files can be found as file system files in the output and Publish folders
- copy the `MvcApp\Plugins` folder to output and Publish folders

## How to not let Asp.Net Core to compile Views into assembly on Publish

- Add the package `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation` to the project.
- Add the following in the project file.

```
<!--  Disable the default behavior of compiling views on publish. -->
<PropertyGroup>
    <MvcRazorCompileOnPublish>false</MvcRazorCompileOnPublish>
    <RazorCompileOnBuild>false</RazorCompileOnBuild>
</PropertyGroup>
```
- Add the following in application startup
```
IMvcBuilder MvcBuilder = builder.Services.AddControllersWithViews();
MvcBuilder.AddRazorRuntimeCompilation();
```


## Loading Plugins

An [ApplicationPart](https://learn.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts) allows Asp.Net Core to discover Controllers and other application parts.

The [AssemblyPart](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.applicationparts.assemblypart) is an `ApplicationPart` representing an `Assembly`, which exposes types and resources.  

The [ApplicationPartManager](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.applicationparts.applicationpartmanager) is the central code entity, in this application part system, that manages the parts and features of an MVC application.

Following is a part of a user-defined `App` static class that handles plugin discovering, loading, creation and configuration.
 

```
static public partial class App
{ 

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
    static void AddPluginsToApplicationPartManager()
    {
        foreach (var Def in PluginDefList)
        {
            ApplicationPart Part = new AssemblyPart(Def.PluginAssembly);
            PartManager.ApplicationParts.Add(Part);
        }
    }


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
    static List<MvcAppPluginDef> PluginDefList { get; set; } = new List<MvcAppPluginDef>();
    static List<IMvcAppPlugin> PluginList { get; } = new List<IMvcAppPlugin>();
    static ApplicationPartManager PartManager { get; set; }
}
 
```

The `LoadPlugins()` method

- gets a list of folders found at `OutputPath\Plugins`. 
- if a folder name starts with `Plugin.` it loads the `plugin-def.json` plugin definition file into a `PluginDef` class instance and then gets the plugin assembly file path.
- adds the `PluginDef` instance to a list
- sorts the `PluginDef` list according to `LoadOrder` property
- loads the plugin assembly using `Assembly.LoadFrom()`
- searches the assembly for a class `Type` implementing the `IAppPlugin` interface
- creates an instance of the `IAppPlugin` and adds it to a list.
- adds the loaded assembly as `ApplicationPart` to `ApplicationPartManager`
- calls the `IMvcAppPlugin.Initialize()` for each loaded plugin
- calls the `IMvcAppPlugin.AddViewLocations()` permitting the plugin to add its own view locations to a custom `IViewLocationExpander`

Following is the `IMvcAppPlugin` interface.

```
public interface IMvcAppPlugin
{
    public void Initialize();
    void AddViewLocations();

    MvcAppPluginDef Descriptor { get; set; }
}
```


## Plugin Views

For plugin Views to be discoverable by Asp.Net the developer has to register an custom implementation of the [IViewLocationExpander](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.razor.iviewlocationexpander) interface.

```
IMvcBuilder MvcBuilder = builder.Services.AddControllersWithViews();
builder.Services.Configure<RazorViewEngineOptions>(options => { options.ViewLocationExpanders.Add(new ViewLocationExpander()); });
```

A view location expander gives the developer a chance to add view locations in the view search locations of the Asp.Net.

Here is our `IViewLocationExpander`.

```
public class ViewLocationExpander : IViewLocationExpander
{
    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations);
    public void PopulateValues(ViewLocationExpanderContext context);

    static public void AddViewLocation(string Location);        
}
```

The `ExpandViewLocations()` and `PopulateValues()` are required by the `IViewLocationExpander`.

The  static methods, `AddViewLocation()` are our own helper for easily adding view locations to the expander.

After plugins are loaded by the `LoadPlugins()` method the `MvcApp` calls the `Plugin.AddViewLocations()` for each plugin telling it to add any view locations it might have, to the `IViewLocationExpander`.

```
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
```
 

## Plugin Static files

For plugin static files, such as Javascript and CSS files, to be discoverable by Asp.Net the developer has to register the appropriate [PhysicalFileProvider](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/file-providers)s, in the main MVC application startup code, pointing to the write paths.

```
app.UseStaticFiles(new StaticFileOptions 
{ 
    OnPrepareResponse = StaticFileResponseProc 
});

app.UseStaticFiles(new StaticFileOptions 
{
    FileProvider = new PhysicalFileProvider(Path.Combine(App.BinPath, "Plugins")),
    RequestPath = new PathString("/Plugins"),
    OnPrepareResponse = StaticFileResponseProc 
});
```

## Conclusion

Finally the `MvcApp` can load plugins with Views, Javascript, CSS and everything.

A plugin can even have its own `Layout`s defined in its `Views\Shared` folder or use `MvcApp` `Layout`s for its Views.

When the the `MvcApp` is published its own Views, Javascript, CSS end up as files in the publish folder. The same stands true for Plugins too.