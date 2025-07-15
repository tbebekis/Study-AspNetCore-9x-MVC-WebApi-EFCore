# Filters

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

In Asp.Net Core [filters](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters) are code components that run immediately after Asp.Net Core selects the action to execute. The execution of filters forms its own pipeline known as _action invocation pipeline_ or _filter pipeline_.

## Filter Types and Execution Order

Filters belong to types, i.e. categories, such as action filters or exception filters. 

The execution order of the various filters is based on filter types and is the following.

- [Authorization filters](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#authorization-filters). They determine if the user is authorized to access the requested resource.
- [Resource filters](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#resource-filters). They used in performing any necessary action on the requested resource.
- [Action filters](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#action-filters) and [Endpoint filters](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/min-api-filters). They used in performing any necessary action on the controller action being selected for execution.
- [Exception filters](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#exception-filters). They used in handling any exception that may thrown during controller action execution.
- [Result filters](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#result-filters). They used in performing any necessary action on the result of the just executed controller action.

The **filter pipeline** resemples that of the **middleware pipeline** in that filters are executed **before** and **after** a controller action is executed.

Also because there are filters applied in the **global** level while other filters applied in the **controller** or **action** level, there is another one order of filter execution dictated by that level, known also as `filter scope`. Global filters are executed first.

### Filter Pipeline Short-circuiting and Cancellation

A `Resource filter` and an `Action filter` may opt to short-circuit the filter pipeline by setting either the `Result` or the `Cancelled` property of the `execution context` provided to the filter method, thus effectively [terminating the filter pipeline](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#cancellation-and-short-circuiting).

## Global Filter Scope

Filters registered using the [MvcOptions.Filters.Add()](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.mvcoptions) method are **global** filters. That registration applies a filter to all requests across the application. 

```
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CustomActionFilter>();
});
```

## Local Filter Scope

Filters applied to controller classes or controller actions using attributes are **local** filters.

```
[TypeFilter(typeof(CustomExceptionFilter))]
public class HomeController : ControllerBase
{
    [ServiceFilter(typeof(CustomActionFilter))]
    public IActionResult Index()
    {
        // ...
    }

    // ...
}
```

## Filter Implementation

Regarding [filter implementation](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#implementation) different filter types support different interface definitions, i.e. there is not a common interface type for all filter types.

Besides that these different interface definitions provide synchronous and asynchronous versions.

The next example, taken from the official documentation, shows a synchronous action filter. The `OnActionExecuting()` is called before the action method is executed while the `OnActionExecuted()` is called after the action method is executed.

```
public class SampleActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Do something before the action executes.
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Do something after the action executes.
    }
}
```

The next example, again taken from the official documentation, shows an asynchronous action filter.

```
public class SampleAsyncActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Do something before the action executes.

        await next();

        // Do something after the action executes.
    }
}
```

## The Filter Context

Filter methods, such are the methods in the examples of this text, accept a `context` parameter, specialized for each filter type.

All these `contexts` are subclasses of the [FilterContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.filtercontext) which is a subclass of the [ActionContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.actioncontext).

As a result these `contexts` provide properties and access to useful properties and methods such as the following.

- [ActionContext.HttpContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.actioncontext.httpcontext) property.
- [ActionContext.ActionDescriptor](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.actioncontext.actiondescriptor) property.
- [ActionContext.ModelState](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.actioncontext.modelstate) property.
- [ActionContext.RouteData](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.actioncontext.routedata) property.
- [FilterContext.Filters](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.filtercontext.filters) property.
- [FilterContext.FindEffectivePolicy()](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.filtercontext.findeffectivepolicy) method.
- [FilterContext.IsEffectivePolicy()](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.filtercontext.iseffectivepolicy) method.

## Authorization filters

[Authorization filters](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#authorization-filters). They determine if the user is authorized to access the requested resource.

`Authorization filters` run before everything else.

An application rarely needs to create a custom authorization filter because there are built-in attributes like the [AuthorizeAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.authorizeattribute) and [AllowAnonymousAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.allowanonymousattribute) attributes.

A custom authorization filter should implement either the [IAuthorizationFilter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.iauthorizationfilter) or the [IAsyncAuthorizationFilter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.iasyncauthorizationfilter) interface.

```
public class CustomAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        bool IsAuthorized = GetIsUserAuthorized(context); // a fictional method

        if (!IsAuthorized)
        {
            context.Result = new UnauthorizedResult();  
        }
    }
}
```

## Resource filters

[Resource filters](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#resource-filters). They used in performing any necessary action on the requested resource.

`Resource filters` run before model binding. The can short-circuit the filter pipeline by assigning the [ResourceExecutingContext.Result](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.resourceexecutingcontext) property.

A custom resource filter should implement either the [IResourceFilter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.iresourcefilter) or the [IAsyncResourceFilter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.iasyncresourcefilter) interface.


```
public class CustomResourceFilter : IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        bool CanContinue = CheckIfCanContinue(context); // a fictional method

        if (!CanContinue)
        {
            context.Result = new ContentResult
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Content = "Your request is invalid"
            };
        }
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
        // this code is executed after the action method
    }
}
```


## Action filters
[Action filters](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#action-filters) and [Endpoint filters](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/min-api-filters). They used in performing any necessary action on the controller action being selected for execution.

`Action filters` run before and after the execution of a controller action method. The model binding is already completed when an action filter is executed.

A custom action filter should implement either the [IActionFilter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.iactionfilter) or the [IAsyncActionFilter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.iasyncactionfilter) interface.

Most probably the `ActionDescriptor` property is of type [ControllerActionDescriptor](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllers.controlleractiondescriptor).

```
public class CustomActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
        {
            ControllerActionDescriptor Descriptor = context.ActionDescriptor as ControllerActionDescriptor;

            string ControllerName = Descriptor.ControllerName;
            string ActionName = Descriptor.ActionName;
            TypeInfo ControllerTypeInfo = Descriptor.ControllerTypeInfo;
            MethodInfo MethodInfo = Descriptor.MethodInfo;

            // ...
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // this code is executed after the action method

        if (context.Exception == null && context.Result is ObjectResult result)
        {
            if (result.Value is ProductModel)
            {
                // ...
            }
        }
    }
}
```



## Exception filters
[Exception filters](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#exception-filters). They used in handling any exception that may thrown during controller action execution.

An `Exception filters` runs when an exception is thrown. It is the right place to handle exceptions thrown during controller action execution or action filters **globally**.

A custom exception filter should implement either the [IExceptionFilter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.iexceptionfilter) or the [IAsyncExceptionFilter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.iasyncexceptionfilter) interface.

```
public class CustomExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var Result = new ObjectResult(new
            {
                Error = "Unexpected Error",
                Message = context.Exception.Message  
            });

        Result.StatusCode = StatusCodes.Status500InternalServerError;

        // inform Asp.Net Core that the exception is handled
        context.ExceptionHandled = true; 
    }
}
``` 

## Result filters

[Result filters](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#result-filters). They used in performing any necessary action on the result of the just executed controller action.

`Result filters` run after the controller action method is executed, but before the response is sent to the client. It is a chance to post-process the action result.

A custom result filter should implement either the [IResultFilter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.iresultfilter) or the [IAsyncResultFilter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.iasyncresultfilter) interface.

```
public class CustomResultFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    { 
        if (context.Result is ObjectResult objectResult)
        {
            if (objectResult.Value is ProductModel)
            {
                // ...
            }           
         }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        // this code is executed after the response is sent to the client
    }
}
```


## Attributes 
ServiceFilterAttribute
TypeFilterAttribute

 ActionFilterAttribute
ExceptionFilterAttribute
ResultFilterAttribute
FormatFilterAttribute
 


### Controller level filters