# Plugin.Test

For a full description of how to create an Asp.Net Core MVC application with Plugins containing Controllers, Views, Javascript and CSS files please consult the text [Asp.Net Core Plugins with Views, Javascript and CSS](../MvcApp/Docs/Plugins.md)

## Plugin Class Library

A plugin is a plain C# class library.

The plugin project name should have the form `Plugin.PLUGIN_NAME_`.

The plugin project folders should be as the following. 

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

## The IAppPlugin implementation

A plugin library should implement the `IAppPlugin` interface.

```
public class Plugin: IAppPlugin
{
    public Plugin()
    {
        TestLib.Plugin = this;
    }


    public void Initialize()
    {
    }
    public void AddViewLocations()
    {
        ViewLocationExpander.AddPluginViewLocations(Descriptor);
    }

    public PluginDef Descriptor { get; set; }
}
```

