# Api Data Results

> This text is part of a group of texts describing [Asp.Net Core](../Index.md).

A WebApi application should return consistent responses.

## Problem description

Let us consider a WebApi application which registers an exception handler in the application startup code in order to trap unhandled exceptions. 

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args); 
    builder.Services.AddControllers();      

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
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
```

Furthermore the application contains a controller, something like the following, which returns a list of products.

```
[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet("list")]
    public IEnumerable<ProductModel> List()
    {
        List<ProductModel> products = new List<ProductModel>()
        {
            new Product() { Name = "Product 1", Price = 12.34M },
            new Product() { Name = "Product 2", Price = 56.78M }
        };

        return products;
    }
}
```

The model used is the following.

```
public class ProductModel
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

Opening a browser and calling the Url `https://localhost:7134/product/list` results in the following `JSON` text.

```
[
  {
    "name": "Product 1",
    "price": 12.34
  },
  {
    "name": "Product 2",
    "price": 56.78
  }
]
```

The client application expects a `JSON` array of products in success. 

But in order to deal with a possible failure of its request it has to **first** check the [HTTP Status](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Status) of the [HttpResponse](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpresponse), before attempting to deserialize the `JSON` data to a product array. 

And it has to perform this check in every request.

There are ways to have a WebApi application returning structured results even when an exception is thrown.

## Api Data Result class hierarchy.

Consider the following hierarchy of result (response) classes.

```
/// <summary>
/// Result base class
/// </summary>
public class DataResult
{
    protected const string SCheckErrorList = "Check the Error list";
    List<string> fErrors;

    public DataResult()
    {
    }

    public virtual void AddError(string ErrorText)
    {
        if (!string.IsNullOrWhiteSpace(ErrorText))
        {
            if (fErrors == null)
                fErrors = new List<string>();

            if (!fErrors.Contains(ErrorText))
                fErrors.Add(ErrorText);

            Message = SCheckErrorList;
        }
    }
    public virtual void ClearErrors()
    {
        if (fErrors != null)
            fErrors.Clear();
    }
    public virtual void CopyErrors(DataResult Source)
    {
        this.Status = Source.Status;
        foreach (var error in Source.Errors) 
            this.AddError(error);
    }

    public virtual void SetResult(int Status = -1, string ErrorMessage = null, string Message = null)
    {
        if (Status >= StatusCodes.Status100Continue)
            this.Status = Status;

        if (!string.IsNullOrWhiteSpace(ErrorMessage))
            Errors.Add(ErrorMessage); 

        if (!string.IsNullOrWhiteSpace(Message))
            this.Message = Message;
    }
    public virtual void ErrorResult(int Status, string ErrorMessage = null)
    {
        if (!string.IsNullOrWhiteSpace(ErrorMessage))
            ErrorMessage = $"Error: {ErrorMessage}";
        else if (Status >= 400 && Status <= 599)
            ErrorMessage = Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(Status);
        else if (Status >= 1000)
            ErrorMessage = ApiStatusCodes.StatusCodeToMessage[Status];
        else
            ErrorMessage = $"Error: {Status}";

        SetResult(Status, ErrorMessage, SCheckErrorList);
    }
    public virtual void ExceptionResult(Exception Ex)
    {
        string ErrorMessage = Ex == null? "Unspecified Exception": $"Exception: {Ex.GetType().Name}";
        ErrorResult(ApiStatusCodes.Exception, ErrorMessage);

        if (Ex != null)
        {
            List<string> ExceptionErrorList = Ex.GetErrors();
            Errors.AddRange(ExceptionErrorList);
        }
 
    }
    public virtual void NoDataResult(string Message = null)
    {
        Message = !string.IsNullOrWhiteSpace(Message) ? $"No data: {Message}" : "No data.";
        SetResult(Status: ApiStatusCodes.NoData, Message: Message);
    }


    public int Status { get; set; } = StatusCodes.Status200OK;
    public string Message { get; set; } = "OK";
    public List<string> Errors
    {
        get
        {
            if (fErrors == null)
                fErrors = new List<string>();
            return fErrors;
        }
        set { fErrors = value; }
    }
}

/// <summary>
/// Result for a single item
/// </summary>
public class ItemResult<T> : DataResult
{
    public T Item { get; set; }
}

/// <summary>
/// Result for a list of items
/// </summary>
public class ListResult<T> : DataResult
{
    public List<T> List { get; set; } = new List<T>();
}

/// <summary>
/// Paging information interface
/// </summary>
public interface IPaging
{
    int TotalItems { get; set; }
    int PageSize { get; set; }
    int PageIndex { get; set; }
}

/// <summary>
/// Result for a paged list of items
/// </summary>
public class ListResultPaged<T>: ListResult<T>, IPaging
{
    int IPaging.TotalItems { get => Paging.TotalItems; set => Paging.TotalItems = value; }
    int IPaging.PageSize { get => Paging.PageSize; set => Paging.PageSize = value; }
    int IPaging.PageIndex { get => Paging.TotalItems; set => Paging.PageIndex = value; }

    public ListResultPaged() 
    { 
    }
    public ListResultPaged(IPaging SourcePaging, List<T> SourceList = null)
    {
        SetFrom(SourcePaging, SourceList);
    }


    public void SetFrom(IPaging SourcePaging, List<T> SourceList = null)
    {
        Paging.TotalItems = SourcePaging.TotalItems;
        Paging.PageIndex = SourcePaging.PageIndex;
        Paging.PageSize = SourcePaging.PageSize;

        List = SourceList;
    }

    public Paging Paging { get; set; } = new(); 
}
```

## Using structure results

With the result classes in place the WebApi application can now return consistent responses even in the case of an error.

A `DataResult` instance is returned even in a case of an error.

Furthermore because all result classes are `DataResult` subclasses the deserialization and the check of the returned information becomes easier.

Here is a new version of the previous error handler.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args); 
    builder.Services.AddControllers();      

    var app = builder.Build();

    //if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler(AppBuilder => {
            AppBuilder.Run(async HttpContext =>
            {
                string Message = "Unknown error";

                var ExceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                if (ExceptionHandlerFeature != null && ExceptionHandlerFeature.Error != null)
                    Message = ExceptionHandlerFeature.Error.Message;

                DataResult Result = new();
                Result.ErrorResult(StatusCodes.Status500InternalServerError, Message)
                string JsonText = JsonSerializer.Serialize(Result);

                await HttpContext.Response.WriteAsync(JsonText);
            });
        });

        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
```

Controllers now can return a consistent structured result.

```
[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet("list")]
    public ListResult<Product> List()
    {
        List<ProductModel> products = new List<ProductModel>()
        {
            new Product() { Name = "Product 1", Price = 12.34M },
            new Product() { Name = "Product 2", Price = 56.78M }
        };

        ListResult<Product> ListResult = new();
        ListResult.List = products.

        return ListResult;
    }
}
```

Now the client application knows that it always has to expect at least the following `JSON`, even in the case of an error, for any reason.

It has just to examine the `Message` string in order to know if its request was a success or a failure.

```
{
  "Status": 200,
  "Message": "OK",
  "Errors": [
    "string"
  ]
}
```

And in a case of success it gets the full list of products.

```
{
  "Status": 200,
  "Message": "OK",
  "Errors": [
    "string"
  ],
  "List": [
    {
      "Id": null,
      "Name": null,
      "Price": 1
    }
  ]
}
```

## Avoiding automatic Asp.Net Core WebApi error responses

The idea is that the client application, even in a case of an error, any kind of error, it has to examine just a single unit of information: the `DataResult`.

Not even the [HttpResponse.StatusCode](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpresponse.statuscode) property.

The problem is that Asp.Net Core may send error responses automatically. For example Asp.Net Core WebApi applications return `HTTP 400`, i.e. `BadRequest`, responses [automatically](https://learn.microsoft.com/en-us/aspnet/core/web-api/#automatic-http-400-responses) when a model validation fails.

Here is how to avoid this automatic behavior.

Three code components are required.

### An IExceptionFilter

We code a class that implements the [IExceptionFilter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.iexceptionfilter) interface.

An action exception filter 
- catches exceptions thrown in a Razor Page or controller creation, model binding, action filters, or action methods.
- **does not catch** exceptions thrown in resource filters, result filters, MVC result execution or **a razor view**.

The following `ActionExceptionFilter` class is a global exception filter for controller actions. Instead of using `try-catch` blocks inside action methods this exception filter catches all exceptions thrown inside a controller action.  

```
public class ActionExceptionFilter : IExceptionFilter
{
    IWebHostEnvironment HostEnvironment;
    IModelMetadataProvider ModelMetadataProvider;
    ILogger Logger;

    public ActionExceptionFilter(IWebHostEnvironment HostEnvironment, IModelMetadataProvider ModelMetadataProvider, ILogger<ActionExceptionFilter> logger)
    {
        this.HostEnvironment = HostEnvironment;
        this.ModelMetadataProvider = ModelMetadataProvider;
        this.Logger = logger;
    }


    public void OnException(ExceptionContext context)
    {
        ActionExceptionFilterContext FilterContext = new(context, ModelMetadataProvider, HostEnvironment.IsDevelopment());

        if (FilterContext.IsWebApi && WebApiHandlerFunc != null)
        {
            WebApiHandlerFunc(FilterContext);
        }
        else if (FilterContext.IsMvc && MvcHandlerFunc != null)
        {
            MvcHandlerFunc(FilterContext);
        }
        else if (FilterContext.IsAjax && AjaxHandlerFunc != null)
        {
            AjaxHandlerFunc(FilterContext);
        }
        else
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
 
        context.ExceptionHandled = true;

        string Controller = FilterContext.ActionDescriptor.ControllerName;
        string Action = FilterContext.ActionDescriptor.ActionName;

        Logger.LogError(context.Exception, "Exception in Controller: {Controller}, Action: {Action}", Controller, Action);
    }

    static public Action<ActionExceptionFilterContext> WebApiHandlerFunc { get; set; }
    static public Action<ActionExceptionFilterContext> MvcHandlerFunc { get; set; }
    static public Action<ActionExceptionFilterContext> AjaxHandlerFunc { get; set; }
}
```

That `ActionExceptionFilter` class has no idea what to do with an exception. It just delegates the exception handling to a proper callback function.

Before calling the proper callback function, it gathers all the possible information using a custom context class, which then passes it to the callback function.

```
public class ActionExceptionFilterContext
{
    Type BaseControllerType = typeof(ControllerBase);
    Type ControllerType = typeof(Controller);

    string fRequestId;

    static public bool IsAjaxRequest(HttpRequest R)
    {
        /*
        X-Requested-With -> XMLHttpRequest is invalid in cross-domain call
       
        Only the following headers are allowed across origins:
            Accept
            Accept-Language
            Content-Language
            Last-Event-ID
            Content-Type
        */

        return string.Equals(R.Query[HeaderNames.XRequestedWith], "XMLHttpRequest", StringComparison.InvariantCultureIgnoreCase)
            || string.Equals(R.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.InvariantCultureIgnoreCase);
    }


    public ActionExceptionFilterContext(ExceptionContext ExceptionContext, IModelMetadataProvider ModelMetadataProvider, bool IsDevelopment)
    {
        this.ExceptionContext = ExceptionContext;
        this.ModelMetadataProvider = ModelMetadataProvider;
        this.InDevMode = IsDevelopment;
        this.Exception = ExceptionContext.Exception;

        IsWebApi = ControllerTypeInfo.IsSubclassOf(BaseControllerType) && !ControllerTypeInfo.IsSubclassOf(ControllerType);
        if (!IsWebApi)
        {
            IsMvc = ControllerTypeInfo.IsSubclassOf(BaseControllerType) && ControllerTypeInfo.IsSubclassOf(ControllerType);
            if (IsMvc)
            {
                IsAjax = IsAjaxRequest(ExceptionContext.HttpContext.Request);
                if (IsAjax)
                    IsMvc = false;
            }
        } 
    }


    public bool IsWebApi { get; }
    public bool IsMvc { get; }
    public bool IsAjax { get; }
    public ExceptionContext ExceptionContext { get; }
    public Exception Exception { get; }
    public ControllerActionDescriptor ActionDescriptor => ExceptionContext.ActionDescriptor as ControllerActionDescriptor;
    public TypeInfo ControllerTypeInfo => ActionDescriptor.ControllerTypeInfo;
    public IModelMetadataProvider ModelMetadataProvider { get; }
    public string RequestId
    {
        get
        {
            if (string.IsNullOrWhiteSpace(fRequestId))
            {
                fRequestId = Activity.Current?.Id ?? ExceptionContext.HttpContext.TraceIdentifier;
                if (!string.IsNullOrWhiteSpace(fRequestId) && fRequestId.StartsWith('|'))
                    fRequestId = fRequestId.Remove(0, 1);
            }

            return fRequestId;
        }
    }
    public bool InDevMode { get; }
}
```

The `ActionExceptionFilter` is required to be registered with the Dependency Injection container.

```
IMvcBuilder MvcBuilder = builder.Services.AddControllers(options => {                
    options.Filters.Add<ActionExceptionFilter>();
    ActionExceptionFilter.WebApiHandlerFunc = GlobalErrorHandlerWebApi;
});
```

Also the registration code provides the callback function too.

The callback function returns a `DataResult` instance to the client with all the available information.

```
static void GlobalErrorHandlerWebApi(ActionExceptionFilterContext Context)
{
    DataResult Result = new();
    Result.ExceptionResult(Context.ExceptionContext.Exception);

    // We do NOT want an invalid HTTP StatusCode.
    // We want to return a valid HTTP Response.
    // We want to have a DataResult delivered to the client, even in a case of an error.
    // context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError; 
    Context.ExceptionContext.HttpContext.Response.ContentType = "application/json";

    Context.ExceptionContext.Result = new JsonResult(Result);
    Context.ExceptionContext.ExceptionHandled = true;
}
```

### An IExceptionHandler

Just to be sure we need to trap any exception thrown outside of a controller action, such as views, pages, etc.

To achieve that an implementation of the [IExceptionHandler](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.diagnostics.iexceptionhandler) interface is required.

The `GlobalExceptionHandlerWebApi` class is a global exception handler which catches any exception thrown by an application except of exceptions thrown by the application startup code.

As done previously so this handler too just prepares and returns a `DataResult` instance to the client.

```
public class GlobalExceptionHandlerWebApi : IExceptionHandler
{
    public GlobalExceptionHandlerWebApi()
    {
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        DataResult Result = new();
        Result.ExceptionResult(exception);

        string JsonText = JsonSerializer.Serialize(Result);
        await httpContext.Response.WriteAsync(JsonText);

        return true;
    }
}
```

A registration of the `GlobalExceptionHandlerWebApi`  class with the Dependency Injection container is required.

```
builder.Services.AddExceptionHandler<GlobalExceptionHandlerWebApi>();
```
### A StatusCode Error Handler

Lastly and because of the Asp.Net Core WebApi framework automatic handling of model validation failures responding with `HTTP 400`, i.e. `BadRequest`, even before the request reaches the controller action, an error status code handler is required.

As done previously so this handler too just prepares and returns a `DataResult` instance to the client.

```
static public class ErrorStatusCodeHandlerWebApi
{
    static public Task Handle(StatusCodeContext Context)
    {
        int HttpStatus = Context.HttpContext.Response.StatusCode;

        if (HttpStatus >= 400 && HttpStatus <= 599)
        {
            DataResult Result = new();
            Result.ErrorResult(HttpStatus);
            string JsonText = JsonSerializer.Serialize(Result);
            Context.HttpContext.Response.WriteAsync(JsonText);
        }

        return Task.CompletedTask;
    }
}
```

The `ErrorStatusCodeHandlerWebApi` is not a handler per se. It is just a static class with a static method. That method needs to be registered as the error handler in the `app.UseStatusCodePages()` call in the application startup.

```
app.UseStatusCodePages(Context => ErrorStatusCodeHandlerWebApi.Handle(Context)); 
```

### Conclusion

That's it. Now a client application will always receive a `DataResult` instance even in a case of an error.
 



