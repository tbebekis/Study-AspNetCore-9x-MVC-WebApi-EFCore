# Middlewares and the Request pipeline

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

A web application handles HTTP requests and returns HTTP responses. When a request arrives it is examined and handled by a number of software components, sequentially.

In Asp.Net Core these components are called `Middlewares` or `Request Delegates`.

A `Middleware`, in Asp.Net Core, is a term referring to a software component that examines and handles a request.

`Request Delegate` is another term for a middleware.

[`Pipeline`](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/#create-a-middleware-pipeline-with-webapplication) is a term referring to the sequence of middlewares. 

In Asp.Net Core that pipeline is a sequence of middlewares which are called one after the other, giving them a chance to examine and perhaps handle the request.

A middleware may do some work with the request before passing it to the next middleware and/or after passing it to the next middleware.

A middleware may opt to not pass the request to the next middleware in the pipeline, thus terminating the handling or the request and sending a response to the client. Such a  middleware is called `terminal middleware`.

The order a middleware is added to the pipeline is **very important**.

## Request Delegates

`Request delegate` is another term for a middleware.

A request delegate is actually a [delegate type](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.requestdelegate).

```
public delegate System.Threading.Tasks.Task RequestDelegate(HttpContext context);
```

A request delegate

- is an anonymous method that is chained into the pipeline
- is considered an *inline middleware*
- is added to the pipeline using the `Use`, `Map` and `Run` extension methods
- is executed in the order it is added to the pipeline. 
 

## The `Run` extension method

A `Run` request delegate is a `terminal middleware` and it terminates the pipeline returning a response.

```
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// other middlewares here
// ...

app.Run(async context =>
{
    // do some work here
    // and then terminate the pipeline
    await context.Response.WriteAsync("This is the response.");
});

app.Run();
```
 
## The `Use` extension method

The `Use` request delegate is used in chaining multiple request delegates, i.e. middlewares, which are going to be executed one after ther other.

```
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// first middleware
app.Use(async (context, next) =>
{
    // do some work before calling the next middleware
    // ...

    // call next middleware  
    await next.Invoke();

    // do some work after calling the next middleware
    // ...
});

// second middleware
app.Use(async (context, next) =>
{
    // call next middleware  
    await next.Invoke();
});

// other middlewares here
// ...

// terminal middleware
app.Run(async context =>
{
    await context.Response.WriteAsync("This is the response.");
});

app.Run();
```

The `next` parameter represents the next request delegate, i.e. middleware, in the pipeline. If `next` is not called, the pipeline terminates.

The `Run` extension method does not receive a `next` parameter as it is always a `terminal middleware`.

## The `Map` extension method

In Asp.Net Core Web application `Startup` code the `app` variable is of type [WebApplication](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.webapplication).

The `WebApplication` implements, among others, the following two interfaces.
- [IApplicationBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.iapplicationbuilder)
- [IEndpointRouteBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.iendpointroutebuilder)

There is number of `Map` extension methods on these two interfaces. Here are the links.

- [MapExtensions](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.mapextensions)
- [EndpointRouteBuilderExtensions](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.endpointroutebuilderextensions)

The `Map` request delegate is used in branching the pipeline based on a specified request path. If the incoming request path starts with the path of the `Map` delegate, then that branch is executed.

```
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Map("/get_data", (app) => {
    var Data = SomeService.GetData();
    return Results.Ok(Data);
});

app.Map("/get_data_list", (app) => {
    var DataList = SomeService.GetDataList();
    return Results.Ok(DataList);
});

// other middlewares here
// ...

app.Run(async context =>
{
    await context.Response.WriteAsync("This is the response.");
});

app.Run();
```

There are `MapGet()`, `MapPost()`, etc. variations of the `Map` method.

`Map` request delegates are mostly `terminal middlewares`.

## The `UseWhen` and `MapWhen` extension methods

Here are the links for these extensions

- [UseWhen](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.usewhenextensions)
- [MapWhen](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.mapwhenextensions)
 
The `UseWhen` and `MapWhen` extension methods are branching the pipeline based on the outcome of a condition, which is actually a predicate.

The `UseWhen` branches are usually rejoined back to the pipeline, provided that they do not contain a `terminal middleware`.

The `MapWhen` branches are mostly `terminal`.

Both  `UseWhen` and `MapWhen` extension methods accept two parameters:

- the predicate, with the signature `Func<HttpContext, bool>`, which is used in evaluating a condition
- a function to be called when the predicate returns `true`, with the signature `Action<IApplicationBuilder>`
 
```
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWhen(context => {
        return context.Request.Query.ContainsKey("SOME_KEY");
    },
    appBuilder => {

        app.Use(async (context, next) =>
        {
            // ...

            // call next middleware  
            await next.Invoke();

            // ...
        });

    });

app.MapWhen(context => {
        return context.Request.Query.ContainsKey("SOME_OTHER_KEY");
    },
    appBuilder => {

        app.Run(async context =>
        {
            await context.Response.WriteAsync("This is the response.");
        });
        
    });    


// other middlewares here
// ...    

app.Run(async context =>
{
    await context.Response.WriteAsync("This is the response.");
});

app.Run();

```

## Custom Middleware class

A wise thought on the developer's part would be to not use inline middlewares at all.

The developer may opt to write a custom middleare class. There are two options:

- implement the [IMiddleware](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.imiddleware) interface
- write a public class following some conventions.

Here is an example of the first case, implementing `IMiddleware`.

```
public class MyMiddleware: IMiddleware
{
    private readonly ISomeService someService;

    public MyMiddleware(ISomeService someService)
    {
        this.someService = someService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // do some work before calling the next middleware
        // ...

        // call next middleware  
        await next(context);

        // do some work after calling the next middleware
        // ...
    }
}
```

Here is an example with a public class following some conventions.

```
public class MyMiddleware
{
    private readonly RequestDelegate next;

    public MyMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // do some work before calling the next middleware
        // ...

        // call next middleware  
        await next(context);

        // do some work after calling the next middleware
        // ...
    }
}
```

The conventions are the following. The middleware class must have

- a public constructor with a parameter of type [RequestDelegate](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.requestdelegate).
- a public method named InvokeAsync as `public async Task InvokeAsync(HttpContext context)`.
- **or** a public method named Invoke as `public Task Invoke(HttpContext context)`

There can be additional parameters for both the constructor and the `InvokeAsync` or `Invoke` methods that are populated by the Dependency Injection system.

## Registering a Custom Middleware class

Middlewares implementing the `IMiddleware` interface [should be added](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/extensibility) to the Dependency Injection container.

```
services.AddTransient<MyMiddleware>();
```

A middleware class, custom or not, is exposed with one or more extension methods.

```
static public class MyMiddlewareExtensions
{
    public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MyMiddleware>();
    }
}
```

These extension methods help in chaining a middleware into the pipeline.

```
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// other middlewares here
// ...    

app.UseMyMiddleware();

// other middlewares here
// ...    

app.Run(async (context) =>
{
    await context.Response.WriteAsync("This is the response.");
});

app.Run();
```

There are situations where a custom middleware needs to read from the request body or write to the response body. For situations like these please consider the relevant [link](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/request-response) in docs.


## Asp.Net Core built-in Middlewares

Asp.Net Core comes with a number of [built-in](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/#built-in-middleware) Middlewares.

A wide range of needs is covered by these ready-made Middlewares including Authentication, Logging, Session, Static files and MVC.

## The importance of Middleware order

The [order of a middleware in the pipeline](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/#middleware-order) is **very important**.

Custom middlewares go to a certain order in the pipeline.

Here is the order the documentation dictates.

- Exception Handling
- HSTS
- HTTPS Redirection
- Static Files
- Routing
- CORS
- Authentication
- Authorization
- Custom middlewares
- EndPoint (such as MVC)