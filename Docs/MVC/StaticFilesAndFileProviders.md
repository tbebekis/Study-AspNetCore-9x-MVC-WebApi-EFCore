# Static Files and File Providers

> This text is part of a group of texts describing [Asp.Net Core](..\Index.md).

`Static Files` are files that they are not pre-processed by the Razor pre-processor, but they are served to the client, i.e. the browser, as they are. Such static files are the `HTML`, `CSS` and `Javascript` files.

## Root Paths

In recent Asp.Net Core versions the application startup code in `Program.cs` file is something like the following

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);
 
    // add services to the Dependency Injection container
    // ...

    var app = builder.Build();

    // add middlewares into the app pipeline
    // ...

    app.Run();
}
```

The `app` variable is of type [WebApplication](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.webapplication) and provides the `Environment` property of type [IWebHostEnvironment](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.hosting.iwebhostenvironment).

The `IWebHostEnvironment` interface provides the following properties.

- `ContentRootPath`. A string. By default it is the absolute physical path to the folder containing the application executable, e.g. `C:\MyApp`.
- `ContentRootFileProvider`. A [IFileProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.fileproviders.ifileprovider) interface of a `File Provider` pointing at `ContentRootPath`.
- `WebRootPath`. A string. By default it is the absolute physical path to the `wwwroot` folder of the application, e.g. `C:\MyApp\wwwwroot`.
- `WebRootFileProvider`. A [IFileProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.fileproviders.ifileprovider) interface of a `File Provider` pointing at `WebRootPath`.

The above root paths can be changed using the `UseContentRoot()` and `UseWebRoot` extension methods found in [HostingAbstractionsWebHostBuilderExtensions](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.hosting.hostingabstractionswebhostbuilderextensions) class.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseContentRoot(...);
 
    // ...

    app.Run();
}
```

## File Providers

File Providers provide access to the file system. A file provider is a class implementing the `IFileProvider` interface. 

```
public interface IFileProvider
{
    IFileInfo GetFileInfo(string subpath);
    IDirectoryContents GetDirectoryContents(string subpath);
    IChangeToken Watch(string filter);
}
```

Asp.Net Core provides three built-in implementations of the `IFileProvider` interface.
 
- [PhysicalFileProvider](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/file-providers#physical-file-provider). Provides access to the physical file system.
- [ManifestEmbeddedFileProvider](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/file-providers#manifest-embedded-file-provider). Provides access to files embedded as resources into assemblies.
- [CompositeFileProvider](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/file-providers#composite-file-provider). Provides access to files of multiple `IFileProvider` instances through a single interface.
 
## The PhysicalFileProvider

The PhysicalFileProvider provides access to the physical file system.

Here is how this template application uses `PhysicalFileProvider` with static files.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);
 
    // add services to the Dependency Injection container
    // ...

    var app = builder.Build();

    // ...

    // ● static files - wwwroot
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = StaticFileResponseProc
    });

    // ● static files - OutputPath\Plugins
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(App.BinPath, "Plugins")),
        RequestPath = new PathString("/Plugins"),
        OnPrepareResponse = StaticFileResponseProc
    });

    // ● static files - Themes folder
    if (!string.IsNullOrWhiteSpace(App.AppSettings.Theme))
    {
        string ThemesFolderPhysicalPath = Path.Combine(App.ContentRootPath, "Themes");
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(ThemesFolderPhysicalPath),
            RequestPath = new PathString("/Themes"),
            OnPrepareResponse = StaticFileResponseProc
        });
    }
 
    // ...

    app.Run();
}
```

The `StaticFileResponseProc` is method using in adding the `Cache-Control` HTTP header in the response of a static file.

```
static void StaticFileResponseProc(StaticFileResponseContext StaticFilesContext)
{
    StaticFilesContext.Context.Response.Headers.Append(HeaderNames.CacheControl, "no-store,no-cache");
}
```

In the above code

- the `PhysicalFileProvider` constructor single parameter is the root physical path the provider represents, e.g. `C:\MyApp\bin\Debug\net9.0\Plugins`
- the `App.BinPath` used above is a property of the `App` static class returning the physical path of the output folder, e.g. `C:\MyApp\bin\Debug\net9.0\`. It uses the [System.AppContext.BaseDirectory](https://learn.microsoft.com/en-us/dotnet/api/system.appcontext).
- the `RequestPath` is the relative request path of the provider, e.g. the physical path `C:\MyApp\bin\Debug\net9.0\Plugins` becomes the virtual path `/Plugins`.


To serve static files found at `wwwroot` folder, the `WebRoot`, is enough to call
```
app.UseStaticFiles();
```

To serve static files found outside the `WebRoot`

```
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider("C:\MyApp\StaticContent")),
    RequestPath = "/StaticFiles"
});
```

## The ManifestEmbeddedFileProvider 

The `ManifestEmbeddedFileProvider` provides access to files embedded as resources into assemblies. It uses a manifest compiled into the assembly in order to simulate the original file paths of the embedded files.

### Prerequisites to use a ManifestEmbeddedFileProvider

- The project `.csproj` file should contain 

```<GenerateEmbeddedFilesManifest>true</GenerateEmbeddedFilesManifest>``` 

in a `<PropertyGroup>`.

- The [Microsoft.Extensions.FileProviders.Embedded](https://www.nuget.org/packages/Microsoft.Extensions.FileProviders.Embedded) NuGet package should be installed.

- An `EmbeddedResource` entry of the folder containing the files as embedded resources should be added to the project `.csproj` file.

```
<EmbeddedResource Include="StaticContent\**" />
```

### Create a ManifestEmbeddedFileProvider instance
Here is how to create a `ManifestEmbeddedFileProvider` instance from the code of a project that uses the Assembly library with the embedded files. 

The first parameter is the Assmbly containing the embedded files while the second parameter is the folder inside that Assembly.

```
Assembly A = GetAssemblyContainingEmbeddedFiles();
ManifestEmbeddedFileProvider FileProvider = new ManifestEmbeddedFileProvider(A, "StaticContent");
```

## The CompositeFileProvider 

The `CompositeFileProvider` provides access to files of multiple `IFileProvider` instances through a single interface.

```
List<IFileProvider> FileProviderList = new List<IFileProvider>();

IFileProvider FileProvider;
foreach (Assembly A in ExternalAssemblyList)
{
    FileProvider = new ManifestEmbeddedFileProvider(A, "StaticContent")
    FileProviderList.Add(FileProvider);
}

CompositeFileProvider ComboFileProvider = new CompositeFileProvider(FileProviderList);

...

// wwwroot
app.UseStaticFiles();  

// the other providers
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = ComboFileProvider
});
```