# Configuration

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

`Configuration` in Asp.Net Core is a term referring to a sub-system which provides configuration options, i.e. settings, in the form of `Key-Value` pairs, required for application configuration.
 
## .Net and Asp.Net Core Configuration documentation

.Net and especially Asp.Net provide a whole universe of classes and interfaces that comprising its configuration system.

- [ConfigurationBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.configurationbuilder)
- [IConfiguration](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfiguration)
- [Configuration Providers](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration#configuration-providers)
- [ConfigurationBinder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.configurationbinder)
- [IOptions<TOptions>](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptions)
- [IOptionsSnapshot<TOptions>](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptionssnapshot-1)
- [IOptionsMonitor<TOptions>](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptionsmonitor-1)
- and much more...

Documentation for the above configuration sub-system can be found at
- [Configuration in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration)

## Asp.Net Core Configuration providers

In Asp.Net Core a `Configuration Provider` is a software entity which reads information, in the form of `Key-Value` pairs, stored in a wide number of `Configuration Sources`, such as JSON or XML files, Environment Variables, Command Line parameters, Azure Key Vault, etc.

In Asp.Net Core there is number of built-in [configuration providers](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/#configuration-providers) available, representing these various configuration sources.

The most commonly used configuration provider in Asp.Net Core applications is the [JSON configuration provider](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/#json-configuration-provider).

Asp.Net Core merges the various configuration sources into a single configuration system. The configuration sources are consolidated into a hierarchical structure of `Key-Value` pairs, similar to a file system. That consolidated result has a tree structure where `Nodes` are the `Configuration Sections`.

## The internals of Asp.Net Core configuration

Consider the following code of an Asp.Net Core application startup.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    var Configuration = builder.Configuration;
 
    // ...

    app.Run();
}
```

The `builder` variable here is of type [WebApplicationBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.webapplicationbuilder).

`WebApplicationBuilder` provides the `Configuration` property of type [ConfigurationManager](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.configurationmanager).

`ConfigurationManager` implements an number of interfaces. One of them is the [IConfiguration](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfiguration) interface.

Also the `ConfigurationManager.GetSection(string Key)` method returns a [IConfigurationSection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfigurationsection) interface which is a `IConfiguration` interface too.

The `key` parameter here identifies a `Configuration Section`. In a `JSON` file such a section is the name of a `JSON` object.

```
{
    "MySection": {
        "Option1": true,
        "Option2: "a string here"
    }
}
```

The `IConfiguration` has a number of extension methods such as `Bind()`, `Get()` and `GetValue()`.

## Accessing Configuration values

Creating an Asp.Net Core application with Visual Studio or VSCode, creates an `appsettings.json` file too, with the following content.

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Here is how to access configuration values from that file.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddOptions();
 
    // access a top level section
    IConfigurationSection LoggingSection = builder.Configuration.GetSection("Logging");

    // using a section variable to access a sub-section
    IConfigurationSection LoggingLevelSection = LoggingSection.GetSection("LogLevel");

    // using a section variable to read a value
    string DefaultLogLevel = LoggingLevelSection.GetValue<string>("Default");

    // another way in accessing sections is to use a path, i.e. Logging:LogLevel
    IConfigurationSection LoggingLevelSection2 = builder.Configuration.GetSection("Logging:LogLevel");

    // ...

    app.Run();
}
```

## Binding a user defined class to a Configuration Section

Let us say that the `appsettings.json` file has the following content.

```
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",

    "AppSettings": {
        "SessionTimeoutMinutes": 30,
        "SupportedCultures": [ "en-US", "el-GR" ]
    }
}
```

Accordingly the application uses a class like the following.

```
public class AppSettings
{ 
    public int SessionTimeoutMinutes { get; set; }
    public List<string> SupportedCultures { get; set; } = new List<string>();
}
```

Here is how to `bind` an instance of the `AppSettings` class to `appsettings.json` file using Asp.Net Core configuration.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddOptions();
 
    AppSettings Settings = new AppSettings();
    builder.Configuration.Bind(nameof(AppSettings), Settings);

    // or
    IConfigurationSection SettingsSection = builder.Configuration.GetSection(nameof(AppSettings));
    AppSettings Settings2 = SettingsSection.Get<AppSettings>();    

    // ...

    app.Run();
}
```
 
## Bind a user defined class and add it to the Dependency Injection

Asp.Net Core docs refer to binding a user defined class to a `Configuration Section` and then adding it to the Dependency Injection container as the [Options Pattern](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options#the-options-pattern).

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // no need for AddOptions() any more, 
    // the builder.Services.Configure<IOptions>(IConfigurationSection)
    // ends up calling AddOptions() too
    // builder.Services.AddOptions();

    IConfigurationSection SettingsSection = builder.Configuration.GetSection(nameof(AppSettings));
    builder.Services.Configure<AppSettings>(SettingsSection); 
 
    // ...

    app.Run();
}
```

> NOTE: The `AddOptions()` method installs monitors that get notifications when a  **file-based** configuration source, e.g. a JSON, XML or INI file, is altered. Only **file-based** configuration sources support dynamic changes. The `builder.Services.Configure<IOptions>(IConfigurationSection)` ends up calling `AddOptions()` too. If `AddOptions()` is called, directly or indirectly, a second time, then the application gets notified **twice** for a single change in a configuration file. There is a section below on how to monitor changes in JSON configuration files.

After that the user defined Configuration class may injected wherever a Dependency Injection is allowed.

```
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    AppSettings settings;

    public HomeController(ILogger<HomeController> logger, IOptionsSnapshot<AppSettings> SettingsSnapshot)
    {
        _logger = logger;
        settings = SettingsSnapshot.Value;
    }

    public IActionResult Index()
    {
        return View();
    } 
}
```

The [IOptionsSnapshot](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options#use-ioptionssnapshot-to-read-updated-data) is used here in order to read changes in the `appsettings.json` file **after** the application is started and maybe the file is already altered. 

`IOptionsSnapshot` is a **scoped** service. It provides a snapshot of the configuration source, i.e. the options file, at the time its instance is constructed. 

## Monitoring Configuration changes

Using the [IOptionsMonitor](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options#ioptionsmonitor) an application may notified every time the underlying file configuration source, i.e. the `appsettings.json` file, changes.

> **NOTE**: when debugging use the `appsettings.json` found in the root application folder. **Not** the one in the bin folder.

This example uses a static `App` class with an `AppSettings` property. That `App.Settings` property is re-assigned each time the underlying `appsettings.json` file changes. In that way, and because `App` is a static class, its `Settings` property is available to any code in the application. 

```
static public class App
{
    static public void SetupAppSettingsMonitor(IOptionsMonitor<AppSettings> AppSettingsMonitor)
    { 
        AppSettingsMonitor.OnChange(NewAppSettings =>
        {
            Settings = NewAppSettings;
        });
    }

    static public AppSettings Settings { get; set; } = new AppSettings();
}
```

Here is the application startup.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // ...

    // add AppSettings as a service
    IConfigurationSection AppSettingsSection = builder.Configuration.GetRequiredSection(nameof(AppSettings));
    builder.Services.Configure<AppSettings>(AppSettingsSection);
    builder.Configuration.Bind(nameof(AppSettings), App.Settings);

    // ...

    var app = builder.Build();

    // get an IOptionsMonitor<AppSettings> service instance
    // IOptionsMonitor is a singleton service
    IOptionsMonitor<AppSettings> AppSettingsMonitor = app.Services.GetRequiredService<IOptionsMonitor<AppSettings>>();

    // call App.SetupAppSettingsMonitor to hook into IOptionsMonitor<AppSettings>.OnChange()
    App.SetupAppSettingsMonitor(AppSettingsMonitor);

    // ...

    app.Run();
}
```
`IOptionsMonitor` is a **singleton** service. It always provides, at any time, the current values of the configuration source, i.e. the options file.

> **NOTE**: for some reason the `OnChange()` is called twice for a signle change in the file. This is a well-known *feature*.


## Adding a file as source to Configuration sub-system

Except of the `appsettings.json` file, where the application may have added some custom sections, the developer may want to use additional configuration files with the Asp.Net Core Configuration sub-system.

Here is an `appoptions.json` file added to the project as `Content` and `Copy if newer` in its properties.

```
{
    "AppOptions": {
        "Option1": 123,
        "Option2": "Hello"
    }
}
```

The class used in binding the file.

```
public class AppOptions
{
    public int Option1 { get; set; }
    public string Option2 { get; set; }
}
```

The updated `App` static class.

```
static public class App
{
    static public void SetupAppSettingsMonitor(IOptionsMonitor<AppSettings> AppSettingsMonitor)
    { 
        AppSettingsMonitor.OnChange(NewAppSettings =>
        {
            Settings = NewAppSettings;
        });
    }

    static public void SetupAppOptionsMonitor(IOptionsMonitor<AppOptions> AppOptionsMonitor)
    {
        AppOptionsMonitor.OnChange(NewAppOptions =>
        {
            Options = NewAppOptions;                
        });
    }    

    static public AppSettings Settings { get; set; } = new AppSettings();
    static public AppOptions Options { get; set; } = new AppOptions();
}
```

And the updated startup code.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // ...

    // add AppSettings as a service
    IConfigurationSection AppSettingsSection = builder.Configuration.GetRequiredSection(nameof(AppSettings));
    builder.Services.Configure<AppSettings>(AppSettingsSection);
    builder.Configuration.Bind(nameof(AppSettings), App.Settings);

    // add the additional file to Asp.Net Core Configuration
    builder.Configuration.AddJsonFile("appoptions.json", optional: false, reloadOnChange: true);

    // the rest is the same as with AppSettings above
    IConfigurationSection AppOptionsSection = builder.Configuration.GetRequiredSection(nameof(AppOptions));
    builder.Services.Configure<AppOptions>(AppOptionsSection);
    builder.Configuration.Bind(nameof(AppOptions), App.Options);    

    // ...

    var app = builder.Build();

    // get an IOptionsMonitor<AppSettings> service instance
    // IOptionsMonitor is a singleton service
    IOptionsMonitor<AppSettings> AppSettingsMonitor = app.Services.GetRequiredService<IOptionsMonitor<AppSettings>>();

    // call App.SetupAppSettingsMonitor to hook into IOptionsMonitor<AppSettings>.OnChange()
    App.SetupAppSettingsMonitor(AppSettingsMonitor);

    // the same as with AppSettings above
    IOptionsMonitor<AppOptions> AppOptionsMonitor = app.Services.GetRequiredService<IOptionsMonitor<AppOptions>>();
    App.SetupAppOptionsMonitor(AppOptionsMonitor);    

    // ...

    app.Run();
}
```

Adding a file as `builder.Configuration.AddJsonFile(...)` adds the file as one of the Asp.Net Core configuration sources, just like the default `appsettings.json` file. 

Although Asp.Net Core merges the configuration sources into a single configuration system, the application has to install a specific `IOptionsMonitor` for each configuration file if that file is going to be monitored for changes, and respond accordingly.

> In the above code both the `OnChange()` methods will be called even if just one of the files changes. It looks like the consolidation of the configuration sources fails to distinguish the changed file and thus to call just the corresponding `OnChange()`. Or I have done something wrong which is very possible too.

Because `AppSettings` and `AppOptions` classes are registerd as Dependency Injection services too they can be injected wherever a Dependency Injection is allowed.

## Other Configuration Sources

The application may use the `builder.Configuration` in adding various configuration sources and even different configuration files depending on the [Environment](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments).

```
builder.Configuration
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: 
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    ;
```
It maybe is easier to have an `appsettings.Development.json` (and a `appsettings.Production.json` for that matter) file, excluding the development file from source control, than to deal with the [Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets).