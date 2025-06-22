# Dependency Injection

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

[Dependency Injection](https://en.wikipedia.org/wiki/Dependency_injection) is a subtle matter.

A dependency is an object that a client object depends on, in order to operate. A code mechanism **injects** the dependency object to the client object.

In theory a client object or function that requires other objects in order to operate, should **not** be responsible of creating those needed objects. Rather there should be a code mechanism, a provider, which somehow provides, i.e. **injects**, these needed and required objects to the client object.

## Asp.Net Core Dependency Injection

[Asp.Net Core Dependency Injection](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) has its own intricacies.

Dependency objects, that ones that are injected to client objects, are called `Services`.

The pattern used widely demands service classes to be interface implementors.

```
public interface IDataService
{
    ...
}

public class DataService: IDataService
{
    ...
}
```

Services should be registered with the Asp.Net's `Dependency Container`, which is an [IServiceProvider](https://learn.microsoft.com/en-us/dotnet/api/system.iserviceprovider) interface.

> Asp.Net comes with its own Dependency Container. To configure some aspects of that default provider the developer has to call `builder.Host.UseDefaultServiceProvider()`.
> Another option that can be used as Dependency Container is the excellent [AutoFac](https://autofac.org/) library.

That registration happens in the early stages of application initialization.

The `builder.Services` in the following example is an `IServiceProvider` instance.

```
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // NOTE: add your own services here
        ...

        var app = builder.Build();

        // add middlewares here
        ...

        app.Run();
    }
}
```

## Service lifetime

A service is created when some client code asks for it. If no client code asks for a certain service, no service instance is created.

[Service lifetime](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes) is a term denoting 

- when a service starts to exist
- when a service ceases to exist

Here are the available lifetimes

- **Singleton**. The service's lifetime is of that of the application.
- **Scoped**. The service's lifetime is of that of the current HTTP Request.
- **Transient**. The service instance is created each time the service is requested.

Again. According to its lifetime a service is created
- **Singleton**: once per application.
- **Scoped**: once per HTTP Request.
- **Transient** : each time is requested.

There are service registration methods according to available lifetimes.

- `AddSingleton()`
- `AddScoped()`
- `AddTransient()`

These methods are provided as `IServiceProvider` extension methods by the [ServiceCollectionServiceExtensions](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollectionserviceextensions) class. **Not** by the `IServiceProvider`.

## Service registration

Service registration happens in the early stages of application initialization.

Here is an example.

```
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        ...

        // NOTE: add your own services here.
        // Following are some of the available options in adding a service

        // using the ServiceDescriptor
        builder.Services.Add(new ServiceDescriptor(typeof(IDataService), typeof(DataService), ServiceLifetime.Singleton));

        // using the interface type and its implementor class type
        builder.Services.AddSingleton<IDataService, DataService>();

        // using just a class type
        services.AddSingleton<DataService>();

        ...

        app.Run();
    }
}
```

## Injecting Dependencies to Controllers

After a service registration, and regarding a `Controller`, the service can be injected 

- to a constructor 
- to an action method
- to a public property
- or obtained using `HttpContext.RequestServices.GetService()`

```
public class HomeController : Controller
{
    IDataService dataService;

    public HomeController(IDataService dataService)
    {
        this.dataService = dataService;
    }

    [HttpGet]
    public IActionResult Index([FromServices] IDataService dataService)
    {
        ...

        return View();
    }

    [HttpGet]
    public IActionResult Index2()
    {
        IDataService dataService = HttpContext.RequestServices.GetService<IDataService>();
        
        ...
        
        return View();
    }

    [HttpGet]
    public IActionResult Index3()
    {
        PublicDataService.DoSomething();
        
        ...
        
        return View();
    }    

    // this service is injected
    public IDataService PublicDataService { get; set; }

}
```
The [FromServicesAttribute]( https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.fromservicesattribute) allows to [inject services into action methods](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/dependency-injection#action-injection-with-fromservices).


## Injecting Dependencies elsewere

Constructor Dependency Injection works also with:

- Middlewares
- Razor Views
- View Components
- Tag Helpers

Here are some examples.

### Middleware

```
public class MyMiddleware
{
    RequestDelegate nextDelegate;
    IDataService dataService;

    public LoggingMiddleware(RequestDelegate next, IDataService dataService)
    {
        nextDelegate = next;
        this.dataService = dataService;
    }

    ...
}
```


### Razor View.

```
@inject IDataService DataService
```

### View Component.
```
public class MyViewComponent : ViewComponent
{
    IDataService dataService;

    public MyViewComponent(IDataService dataService)
    {
        this.dataService = dataService;
    }

    ...
}
```

### Tag Helper.

```
[HtmlTargetElement("MyTag")]
public class MyTestTagHelper : TagHelper
{
    IDataService dataService;

    public MyTestTagHelper(IDataService dataService)
    {
        this.dataService = dataService;
    }
}
```

## Scoped Services

A scoped service is unique per HTTP request and its lifetime is the HTTP request's lifetime. This requires some type of [Scope Validation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host#scope-validation) which is done by Asp.Net Core. So scoped services require special attention.

Although not recommenced, the developer may deactivate the `Scope Validation`.

```
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseDefaultServiceProvider(options => options.ValidateScopes = false);
        ...

        app.Run();
    }
}
```

An `IServiceProvider` is created when the application starts executing and it is called the `Root Service Provider`. Its lifetime is the application's lifetime.

Scoped services are **not** created by the `Root Service Provider`.

Scoped services are created by the `HttpContext.RequestServices` Service Provider which is considered a scoped Service Provider.

If a scoped service is created by the `Root Service Provider` and not by the `HttpContext.RequestServices` scoped Service Provider, then **the scoped service becomes a singleton one**.

This is because a scoped service gets the lifetime of the service provider which created the scoped service. The `Root Service Provider` lasts to the application shutdown.

When `Scope Validation` is enabled, at it is by default, it catches situations like this invalid scope service creation.

## Service scopes

The `Root Service Provider` imposes a **service scope**.

Apart of that a new service scope is created in every HTTP request. 

Scoped services belong to this, per HTTP request, scope. This means that scoped services are resolved by that per HTTP request scope. When the request ends his life, its scope is disposed along with all scoped services created by that scope.

The developer may use the `CreateScope()` method, provided by the [ServiceProviderServiceExtensions](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.serviceproviderserviceextensions), in creating a new scope.

The `CreateScope()` method returns a [IServiceScope](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicescope)


```
public interface IServiceScope : IDisposable
{
    IServiceProvider ServiceProvider { get; }
}
```

Then that `IServiceScope.ServiceProvider` property can be used in resolving services. 

Here is an example taken from [Asp.Net Core documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection#resolve-a-service-at-app-startup) on how to resolve a scoped service at application startup.

```
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var myDependency = services.GetRequiredService<IMyDependency>();
    myDependency.WriteMessage("Call services from main");
}
```

## Resolving Services

There are some [official quidelines](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection#recommendations) dictating **not** to use the so-called _service locator pattern_, that is do not use the `GetService()` method in resolving services.

But frequently there are situations where the developer has to resort to the `GetService()` method.

Here is how it can be done.

First a static class, like the following is needed.

```
static public partial class App
{
    static public IServiceProvider GetServiceProvider(IServiceScope Scope = null)
    {
        if (Scope != null)
            return Scope.ServiceProvider;
        HttpContext HttpContext = HttpContextAccessor?.HttpContext;
        return HttpContext?.RequestServices ?? RootServiceProvider;
    }

    static public T GetService<T>(IServiceScope Scope = null)
    {
        IServiceProvider ServiceProvider = GetServiceProvider(Scope);
        return ServiceProvider.GetService<T>();
    }

    static public HttpContext GetHttpContext() => HttpContextAccessor.HttpContext;

    static internal IHttpContextAccessor HttpContextAccessor { get; set; }
    static internal IServiceProvider RootServiceProvider { get; set; }    
}
```

And some code has to be added to the `Main()` which does the following

- registers a `IHttpContextAccessor` service
- stores a reference to the `ApplicationServices` which is the `Root Service Provider`
- stores a reference to the `IHttpContextAccessor` service.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // add the IHttpContextAccessor service, it is singleton
    builder.Services.AddHttpContextAccessor();

    ...

    var app = builder.Build();

    App.RootServiceProvider = (app as IApplicationBuilder).ApplicationServices;
    App.HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
 
    ...

    app.Run();
}
```

After that any service can be resolved as 

```
var dataservice = App.GetService<IDataService>();
```

## Some Guidelines

- not every class deserves to be a service registered with the Service Provider. Static classes are sometimes easier to use in Controllers, Razor Views and View Components.
- more than three or four services in a Controller constructor is bad design. Use public property injection instead.
- avoid writing extension methods for service registration. Asp.Net Core has already a huge number of extension methods.
- prefer clean an easily understandable design everywhere. Writing code is not a championship. It's an art.
- Proper naming is crucial.
 
 