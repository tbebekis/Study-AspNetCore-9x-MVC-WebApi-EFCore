# Application Startup

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

The startup of an Asp.Net Core application, **be it MVC or WebApi**, is a complicated subject.

Everything starts in the `Program.cs` file.

In earlier Asp.Net Core versions the `Program.cs` file was something like the following

```
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```

> The `Main()` creates a [`host`](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/#host) which then is used in creating a `builder` which then is used in calling a `Startup` class.

Here is such a `Startup` class.

```
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // add services to the Dependency Injection container
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // add middlewares into the app pipeline
    }
}
```

In recent Asp.Net Core versions the application startup code in the `Program.cs` file is something like the following

```
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // add services to the Dependency Injection container
        // builder.Services.Add...
        // ...

        var app = builder.Build();

        // add middlewares into the app pipeline
        // app.Use...
        // ...

        // start the application
        app.Run();
    }
}
```

> If the developer leaves un-checked the *Do not use top-level statements*, when creating the project in Visual Studio, then the `Program` class and the `Main()` method is omitted and the content of the `Program.cs` file is just the content of the above `Main()` method.

I'm not perfectly sure what is going on here and I'll try my best to explain it.

- the [WebApplication](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.webapplication) creates a `builder` which is a[`WebApplicationBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.webapplicationbuilder)
- the `builder` provides the `Services` property which is then used in adding services to [Dependency Injection](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) container
- the `builder` then builds the `app` which is a `WebApplication` instance (lol)
- the `app` is then used in adding [Middlewares](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware) into the app request pipeline to handle requests and send back responses.
- [docs say](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/#host) *"On startup, an ASP.NET Core app builds a host"*. 
- Well `WebApplication` implements the `IHost` interface which provides a `Services` property, but it is **not** used in adding services to DI container. Instead the `WebApplication.Services` is used, which is the `builder`. 
- I'm at a loss.

> Nobody says being a developer is an easy or at least reasonable job and we all understand why.

## The two phases of Application Startup

The above is the history and the evolution.

The summary is that Application Startup has two phases

- add services to Dependency Injection container
- add middlewares into the app request pipeline to handle requests and send back responses.

Both applications in this solution have a `Program.cs` file with the following class.

```
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseDefaultServiceProvider(options => options.ValidateScopes = false);
        
        App.AddServices(builder);
        
        var app = builder.Build();
        App.AddMiddlewares(app);
        
        app.Run();
    }
}
```

The above code delegates the two phases of Application Startup to two methods of a static `App` class

- `App.AddServices()`, adds services to Dependency Injection container
- `App.AddMiddlewares()`, adds middlewares into the app pipeline to handle requests and responses.

Next we have to understand what Dependency Injection is.
