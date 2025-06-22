# Asp.Net Core Plugins with Views, Javascript and CSS 

> This text is part of a group of texts describing [Asp.Net Core](../Index.md).

A [Plugin](https://en.wikipedia.org/wiki/Plug-in_(computing)) is a software module, external to an application and **dynamically loadable**. A plugin adds or extends the functionality of an application, without requiring an application rebuild.

Asp.Net Core offers [Razor Class Library](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/ui-class) template project, which may contain Views, Controllers and static files, such as Javascript and CSS, as a type of a resusable application component. But Razor Class Libraries are not designed to be dynamically loadable.

This text describes a way to have dynamically loadable external class libraries containing Views, Controllers and static files, such as Javascript and CSS, as plugins to an Asp.Net Core MVC application.

## Required Projects

The projects required to create a plugin MVC application, are

- the MVC application
- a class library, where both the MVC application and the plugin libraries, depend on.
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
- set the output folder, for both Debut and Release, to `MvcApp\Plugins\Plugin.Test` folder
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
        <PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="9.0.4" />
        <PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="9.0.4" />          
        <PackageReference Include="Microsoft.Extensions.FileProviders.Abstractions" Version="9.0.4" />
        <PackageReference Include="Microsoft.Extensions.FileProviders.Embedded" Version="9.0.4" />
        <PackageReference Include="Microsoft.Extensions.FileProviders.Physical" Version="9.0.4" />
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
        <ProjectReference Include="..\tp.Web\tp.Web.csproj" />
        <ProjectReference Include="..\tp\tp.csproj" />
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

Here is the procedure.

First a reference to an [ApplicationPartManager](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.applicationparts.applicationpartmanager) instance is needed.

An `ApplicationPart` allows Asp.Net Core to discover Controllers and other application parts.

```
IMvcBuilder MvcBuilder = builder.Services.AddControllersWithViews();

...

LoadPlugins(MvcBuilder.PartManager);
```

After that the `LoadPlugins()` method is called.

```
static void LoadPlugins(ApplicationPartManager PartManager)
{ 
    string RootPluginFolder = Path.Combine(App.BinPath, "Plugins");
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
            //LoadPlugin(PartManager, PluginFolderPath);
            // load the plugin definition
            CleanPluginFolder(PluginFolderPath);
            string PluginDefFilePath = Path.Combine(PluginFolderPath, "plugin-def.json");
            PluginDef Def = new PluginDef(PluginFolderPath);
            Json.LoadFromFile(Def, PluginDefFilePath);

            // find plugin assembly file path  
            string[] FilePaths = Directory.GetFiles(PluginFolderPath, "Plugin.*.dll");
            if (FilePaths == null || FilePaths.Length == 0)
                Sys.Throw($"No Plugin Assembly found in folder: {PluginFolderPath}");
            Def.PluginAssemblyFilePath = FilePaths[0];
            Def.Id = Path.GetFileName(Def.PluginAssemblyFilePath);

            PluginDefList.Add(Def);
        }
    }

    // sort definition list
    PluginDefList = PluginDefList.OrderBy(item => item.LoadOrder).ToList();

    // create plugins
    List<Type> ImplementorClassTypes;
    foreach (PluginDef Def in PluginDefList)
    {
        // load the assembly and the application part for that assembly
        Def.PluginAssembly = Assembly.LoadFrom(Def.PluginAssemblyFilePath);
        ApplicationPart Part = new AssemblyPart(Def.PluginAssembly);
        PartManager.ApplicationParts.Add(Part);

        ImplementorClassTypes = TypeFinder.FindImplementorClasses(typeof(IAppPlugin), Def.PluginAssembly);
        if (ImplementorClassTypes.Count == 0)
            Sys.Throw($"Plugin: {Def.Id} does not implement IAppPlugin");
        if (ImplementorClassTypes.Count > 1)
            Sys.Throw($"Plugin: {Def.Id} implements more than one IAppPlugin");

        IAppPlugin Plugin = (IAppPlugin)Activator.CreateInstance(ImplementorClassTypes[0]);
        Plugin.Descriptor = Def;

        PluginList.Add(Plugin);
    }
}

 
```

The `LoadPlugins()` method

- gets a list of folders found at `OutputPath\Plugins`. 
- if a folder name starts with `Plugin.` it loads the `plugin-def.json` plugin definition file into a `PluginDef` class instance and then gets the plugin assembly file path.
- adds the `PluginDef` instance to a list
- sorts the `PluginDef` list according to `LoadOrder` property
- loads the plugin assembly using `Assembly.LoadFrom()`
- adds the loaded assembly as `ApplicationPart` to `ApplicationPartManager`
- searches the assembly for a class `Type` implementing the `IAppPlugin` interface
- creates an instance of the `IAppPlugin` and adds it to a list.

## Plugin Views

For plugin Views to be discoverable by Asp.Net the developer has to register an implementation of the [IViewLocationExpander](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.razor.iviewlocationexpander) interface.

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
    static public void AddPluginViewLocations(PluginDef Def);
}
```

The `ExpandViewLocations()` and `PopulateValues()` are required by the `IViewLocationExpander`.

The next two static methods, `AddViewLocation()` and `AddPluginViewLocations()` are our own helpers for easily adding view locations to the expander.

After plugins are loaded by the `LoadPlugins()` method the `MvcApp` calls the `InitializePlugins()`. 

```
static void InitializePlugins()
{
    foreach (IAppPlugin Plugin in PluginList)
    {
        Plugin.Initialize();
        Plugin.AddViewLocations();
    }
}
```

The `Plugin.AddViewLocations()` is called which then just calls the static `ViewLocationExpander.AddPluginViewLocations()` passing its `PluginDef` instance. This call adds plugin view locations to the location expander.

## Plugin Static files

For plugin static files, such as Javascript and CSS files, to be discoverable by Asp.Net the developer has to register the appropriate [PhysicalFileProvider](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/file-providers)s, pointing to the write paths.

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