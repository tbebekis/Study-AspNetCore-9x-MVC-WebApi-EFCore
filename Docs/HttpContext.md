# HttpContext

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

[HttpContext](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/use-http-context) encapsulates information regarding the current [HttpRequest](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httprequest) and [HttpResponse](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpresponse).

The `HttpContext` is **not** thread safe.

## Notable HttpContext Properties

- **Connection**. Information about the local and remote IP addresses and ports.
- **Features**. A list of [feature interfaces](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/request-features) providing various functionality. 
- **Items**. A `Key-Value` collection of user defined data having **the same lifetime as the request** that can be used in sharing data between code components and maintaining state.
- **Request**. The `HttpRequest` instance.
- **RequestServices**. A `IServiceProvider` providing access the the request's service container.
- **Response**. The `HttpResponse` instance.
- **Session**. A `Key-Value` collection of user defined data having **the same lifetime as the session** that can be used in sharing data between code components and maintaining state.
- **User**. A `ClaimsPrincipal` instance representing the identity of the current request.
 
## Accessing HttpContext

- **Razor View**. Provides the `Context` property.
- **Razor Page**. Provides the `HttpContext` property.
- **Controller**. Provides the `HttpContext` property.
- **Middlewares**. The `InvokeAsync()` method of a custom middleware gets a `HttpContext` parameter.

The [IHttpContextAccessor](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.ihttpcontextaccessor) interface provides a `HttpContext` property.

The `IHttpContextAccessor` should be registered as a service in application's startup code.

```
builder.Services.AddHttpContextAccessor();
```

After that the `IHttpContextAccessor` service can be injected anywhere a service can be injected, i.e. constructors, properties, `[FromServices]` controller action parameters, etc. Check the `Dependency Injection` text of this group of texts.