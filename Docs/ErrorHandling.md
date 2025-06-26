# Error Handling

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

Error handling is an important issue in any kind of application. 

The most common approach is handling errors locally with the `try-catch` block.

```
try
{
   ...
}  
catch (Exception ex)
{
   ...
}
```

Although handling errors locally, as above, is a common practice and the preferable one in many cases, in Asp.Net Core applications, there are ways to handle errors **globally**.

Following are the available options.

- `UseExceptionHandler()` with an error handling path (MVC only)
- `UseExceptionHandler()` with a callback function
- `UseStatusCodePages()` with a callback function
- implement the `IExceptionHandler`  interface
- implement the `IExceptionFilter`  interface
- use an exception handling [custom middleware](#using-an-exception-handling-custom-middleware) 


## UseExceptionHandler() with an error handling path (MVC only)

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    //if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthorization();

    app.MapStaticAssets();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.Run();
}
```
This is an **MVC only** solution.

In the case of an exception the above will display the `Error` [view](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling#exception-handler-page) of the `Home` controller.

The effect of the statement 

```
  app.UseExceptionHandler("/Home/Error");
```
is that any unhandled exception ends up in the `Error` view. And the `Error` view becomes a sort of an exception handler.

It is possible to customize the `Error` view and in this text there is an example of how to do it.


## UseExceptionHandler() with a callback function

Using another overload of the `UseExceptionHandler()` extension method it is possible to use a [global error handler function](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling#exception-handler-lambda), as the next example displays.

The [IExceptionHandlerPathFeature](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.diagnostics.iexceptionhandlerpathfeature), used in the example, provides the `Error` property which is a reference to the current exception.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    //if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler(AppBuilder => {
            AppBuilder.Run(async HttpContext =>
            {
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                HttpContext.Response.ContentType = Text.Plain;

                string Message = "Unknown error";

                var ExceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                if (ExceptionHandlerFeature != null && ExceptionHandlerFeature.Error != null)
                    Message = ExceptionHandlerFeature.Error.Message;

                await HttpContext.Response.WriteAsync(Message);
            });
        });

        app.UseHsts();
    } 

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthorization();

    app.MapStaticAssets();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.Run();
}
```




## `UseStatusCodePages()` with a callback function

When an application sets the `Response.StatusCode` to a value from [`400` to `599`](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Status), for example

```
httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
```
then the Asp.Net Core returns the status code and an **empty response body**.

Calling [UseStatusCodePages()](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling#usestatuscodepages) in the application startup code enables default text-only handlers for common error status codes. 

In such cases, the `UseStatusCodePages()` extension method can be used in order to customize the text of the response message, in a number or ways   .

One such a way is by passing a handler callback function as in the next example which uses a fictional `GetDescriptiveStatusMessage()`.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();

    var app = builder.Build();

    app.UseStatusCodePages(Context => {

        int HttpStatus = Context.HttpContext.Response.StatusCode;

        if (HttpStatus >= 400 && HttpStatus <= 599)
        {
            string Message = GetDescriptiveStatusMessage(HttpStatus);
            Context.HttpContext.Response.WriteAsync(Message);
        }

        return Task.CompletedTask;
    });

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
```


## Implementing the `IExceptionHandler` interface

This approach is similar to passing a global exception handler callback to the `UseExceptionHandler()` extension method. 

An implementation of the [IExceptionHandler](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling#iexceptionhandler) is involved.

```
public class GlobalExceptionHandler : IExceptionHandler
{
    public GlobalExceptionHandler()
    {
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = Text.Plain;

        string  Message = exception.Message;
        await httpContext.Response.WriteAsync(Message);
        return true;
    }
}
```

In case the error is handled, the handler must return **true**.

Registration of the handler is required.

```
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
```

## Implementing the `IExceptionFilter` interface

A [IExceptionFilter](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?#exception-filters) implementor class is a global **action exception filter**.

Such an action exception filter 
- catches exceptions thrown in a Razor Page or controller creation, model binding, action filters, or action methods.
- **does not catch** exceptions thrown in resource filters, result filters, MVC result execution or **a razor view**.


The next example is an action exception filter for an MVC application. 

The filter creates an `ErrorViewModel`, prepares a [ViewResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.viewresult) and sets the `Error` view as the view to render the response.

```
public class ActionExceptionFilter : IExceptionFilter
{
    IWebHostEnvironment HostEnvironment;
    IModelMetadataProvider ModelMetadataProvider;

    public ActionExceptionFilter(IWebHostEnvironment HostEnvironment, IModelMetadataProvider ModelMetadataProvider)
    {
        this.HostEnvironment = HostEnvironment;
        this.ModelMetadataProvider = ModelMetadataProvider;
    }

    public void OnException(ExceptionContext context)
    {
        ErrorViewModel Model = new ErrorViewModel();
        Model.Exception = context.Exception;
        Model.ErrorMessage = context.Exception.Message;  

        var Result = new ViewResult();
        Result.ViewName = "Error";
        Result.ViewData = new ViewDataDictionary(ModelMetadataProvider, context.ModelState);
        Result.ViewData.Model = Model;
 
        context.ExceptionHandled = true; 
    }
}
```
 
Next is a possible implementation of an action exception filter for a WebApi application.

```
public class ActionExceptionFilter : IExceptionFilter
{

    public ActionExceptionFilter()
    {
    }

    public void OnException(ExceptionContext context)
    {
        ApiResultModel Model = new ApiResultModel();
        Model.Exception = context.Exception;
        Model.ErrorMessage = context.Exception.Message;  

        context.HttpContext.Response.ContentType = "application/json";
        context.Result = new JsonResult(Model);
 
        context.ExceptionHandled = true; 
    }
}
```

The `ActionExceptionFilter` must be registered as a filter.

```
  builder.Services.AddControllersWithViews(options => {                
    options.Filters.Add<ActionExceptionFilter>();
  });
```

## Using an exception handling custom middleware

A possible implementation of an exception handling custom middleware

```
public class ExceptionHandlerMiddleware: IMiddleware
{
    Task HandleExceptionAsync(HttpContext context, Exception exception)
    { 
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        string Message = "Unknown error";
        return context.Response.WriteAsync(Message);
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

And the required registration.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllersWithViews();
    builder.Services.AddSingleton<ExceptionHandlerMiddleware>();

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlerMiddleware>();

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthorization();

    app.MapStaticAssets();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.Run();
}
```


## The Error view (MVC only)

When the application startup code contains a statement like

```
   app.UseExceptionHandler("/Home/Error");
```

then the `Error` view becomes a sort of an exception handler.

The effect of that statement is that any unhandled exception ends up in the `Error` view.

That means that, in a case of an error, the `Error` view maybe displayed even **without** the `HomeController.Error()` action to be called.

So because the `HomeController.Error()` may not be called we have to gather all the exception information and prepare the view model **inside** the view.

So here is such an `Error` view.

```
@model ErrorViewModel

@using System.Text
@using System.Diagnostics
@using Microsoft.AspNetCore.Diagnostics
@using Microsoft.AspNetCore.Hosting
@using Microsoft.Extensions.Hosting

@inject IWebHostEnvironment HostEnvironment

@{
    ViewData["Title"] = "Error";
    ErrorViewModel ErrorModel = Model != null ? Model : new ErrorViewModel();
    ErrorModel.Update(Context, ViewData, HostEnvironment);
}

<div class="page">
 
    <h1>Error</h1>

    <p><strong>Oïps! something went wrong.</strong></p>
    <p><strong>An error occurred while processing your request.</strong></p>
    <p>We apologize for the error. </p>
    <p>Our staff will be immediately informed regarding the problem and we will try to resolved it as soon as possible. </p>

    @if (!string.IsNullOrWhiteSpace(ErrorModel.RequestId))
    {
        <p><strong>Reference Id: </strong> <code>@ErrorModel.RequestId</code></p>
    }

    @if (!string.IsNullOrWhiteSpace(ErrorModel.ErrorMessage))
    {
        <p><strong>Error Message: </strong><code>@ErrorModel.ErrorMessage</code></p>
    }

    @if (!string.IsNullOrWhiteSpace(ErrorModel.StackTrace))
    {
        <p><strong>Full Stack</strong></p>
        <div style="overflow: scroll;">
            <pre>@ErrorModel.StackTrace</pre>
        </div>        
    }
</div>

```
 

## The Developer exception page

In Asp.Net Core applications, when running in the **Development environment**, the [`Developer Exception Page`](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling#developer-exception-page) is enabled by default in both types of applications, MVC or WebApi.

The `Developer Exception Page` displays very detailed information regarding an unhandled error, such as the full stack, the query string, the headers and the routing.

The `Developer Exception Page` is used by the system when no other exception handlers exist that handled the error.

To see the `Developer Exception Page` just disable any other exception handler and generate an exception in a controller or a view.

```
public class MyController : Controller
{
    public IActionResult Action1()
    {
        throw new Exception("This is a test error from a Controller.");
    } 
}
```