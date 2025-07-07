# Routing

In Asp.Net Core [Routing](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing) is a procedure that matches an incoming HTTP request to a request handler known as [EndPoint](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.routeendpoint).

Incoming HTTP request handling can be configured to be delegated to

- [MVC controllers](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions)
- [WebApi controllers](https://learn.microsoft.com/en-us/aspnet/core/web-api)
- [Minimal Api End Points](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview)
- [Razor Pages](https://learn.microsoft.com/en-us/aspnet/core/razor-pages)
- [SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction)
- [gRPC Services](https://learn.microsoft.com/en-us/aspnet/core/grpc)
- Built-in middlewares such as [Static Files](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files)
- [Custom Middlewares and Request Delegates](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware)

This text covers routing in general and routing regarding MVC and WebApi controllers.

## Components in Asp.Net Core Routing

- [Route Template](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing#route-templates). Defines the structure of a URL and it may contain placeholders for route values, e.g. `/customer/{id}` where `{id}` is a placeholder.
- [Route Values](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.routevaluedictionary). A dictionary of `Key-Value` pairs constructed from placeholders contained in the route template where `Key` is the placeholder, e.g. `id` and `Value` the actual placeholder value, e.g. `1234` in the request URL.
- [Route Constraints](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing#route-constraints).
- Route Parameters. Parameters in controller actions having the same name as a placeholder in the associated route template. They get their values from the `Route Values`, e.g. `public IActionResult GetCustomer(int id)`.

## Controller Routing

The built-in `Routing Middleware` is used by Asp.Net Core in matching incoming request URLs to controller actions. That matching is done using the defined `Route Templates`. 

`Route Templates` can be defined

- in application startup code (`Conventional Routing`)
- using attributes in controllers and actions (`Attribute Routing`)

## Mixing Conventional and Attribute Routing

An application can mix the use of conventional routing and attribute routing.

Actions are either conventionally routed or attribute routed.

Placing a route attribute on a controller class makes all of its actions attribute routed.

Placing a route attribute on a controller action makes only that action attribute routed.

If an action is marked with a route attribute, then no conventional routing is used in mapping that action.

## Conventional Routing

Conventional Routing is mostly used with MVC applications.

Conventional routes are defined in application's startup code. 

The `MapControllerRoute()` and `MapAreaControllerRoute()` are used in defining [one or more](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing#multiple-conventional-routes) **conventional** controller routes.

The ASP.NET Core MVC template generates the following.

```
// ...

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

The route template `{controller=Home}/{action=Index}/{id?}` can match any controller action. It just sets some defaults, the `Home` controller and the `Index` action and an optional `id` parameter.

With the above configuration even the `/` URL path finds a match to `Home/Index`.

The conventional route matching

- uses controller and action names only
- does not use namespaces or method parameters.

> The use of `MapControllerRoute()` does not prohibit the use of attribute routing in an MVC application.


## Attribute Routing

Attribute Routing is mostly used with [REST](https://en.wikipedia.org/wiki/REST) Web Apis but it can be used with MVC applications too.
 
Attribute routing is done by marking controller classes and controller actions with route attributes.

The `MapControllers()` is used with Web Api applications to map attribute routed controllers.

```

// ...

app.MapControllers();

app.Run();
```

## Route Attributes

> Using `Route Attributes` makes controller name and action name irrelevant.

The [HttpRouteAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.routeattribute) can be used either on a contoller or on an action.

The following attributes are also available. These attributes map to [HTTP Methods](https://en.wikipedia.org/wiki/HTTP#Request_methods) and can be used with controller actions only. Docs refer to the above attributes as [verb templates](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing#http-verb-templates).
 
- [HttpGetAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.httpgetattribute)
- [HttpPostAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.httppostattribute)
- [HttpPutAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.httpputattribute)
- [HttpDeleteAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.httpdeleteattribute)
- [HttpHeadAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.httpheadattribute)
- [HttpPatchAttribute](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing)

Consider the next example. Side comments display the route.

```
public class HomeController : Controller
{
    [Route("")]                     // ""
    [Route("Home")]                 // Home
    [Route("Home/Index")]           // Home/Index
    [Route("Home/Index/{id?}")]     // Home/Index/123
    public IActionResult Index(int? id)
    {
        ...
    }

    [Route("Home/About")]           // Home/About
    public IActionResult About()
    {
        ...
    }
}
```

The `HttpRouteAttribute`, when it is placed on a controller, it acts as a prefix for all action routes.

Consider the next example.

```
[Route("Home")]  // route prefix for all action routes
public class HomeController: Controller
{
    [Route("")]                     // Home
    [Route("/")]                    // ""
    [Route("Index")]                // Home/Index
    [Route("Index/{id?}")]          // Home/Index/123
    public IActionResult Index(int? id)
    {
        ...
    }

    [Route("About")]                // Home/About
    public IActionResult About()
    {
        ...
    }
}
```

## Attribute Routing with Http Verb Attributes
 
[Verb Attributes](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-9.0#attribute-routing-with-http-verb-attributes) impose a `HTTP` method.

```
[Route("product")]
public class ProductController : Controller
{ 
    [HttpGet("search", Name = "Product.Search")]        // GET product/search
    public async Task<ActionResult> Search(string Term, string CategoryId = "")
    {
        ...
    }

    [HttpGet("list", Name = "Product.List")]            // GET product/list
    public async Task<ActionResult> Index()
    {
        ...
    }

    [HttpGet("paging", Name = "Product.Paging")]        // GET product/paging
    public async Task<ActionResult> Paging()
    {
        ...
    }

    [HttpGet("insert")]                                 // GET product/insert
    public async Task<ActionResult> Insert()
    {
        ...
    }

    [ValidateAntiForgeryToken]
    [HttpPost("insert", Name = "Product.Insert")]       // POST product/insert     
    public async Task<ActionResult> Insert(ProductModel Model)
    {
        ...
    }

    [HttpGet("edit/{Id}")]                              // GET product/edit/123
    public async Task<ActionResult> Edit(string Id)
    {
        ...
    }

    [ValidateAntiForgeryToken]
    [HttpPost("edit", Name = "Product.Edit")]           // POST product/edit     
    public async Task<ActionResult> Edit(ProductModel Model)
    {
        ...
    }

    [HttpGet("delete/{Id}", Name = "Product.Delete")]   // GET product/delete/123
    public async Task<ActionResult> Delete(string Id)
    {
        ...
    }

}
```

The `GET product/insert` and `POST product/insert` are two different routes.

## Route Names

As the last example shows a route can have a name.

```
[HttpGet("list", Name = "Product.List")]  
```

Route names

- must be unique in the whole application
- have no impact on the route construction and mapping
- are only use in generating URLs.


## Generating URLs

#### [IUrlHelper](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iurlhelper)

The `IUrlHelper` provides methods that generate URLs to MVC actions, pages, and routes. 

Controllers and views provide the `Url` property of type `IUrlHelper`.

Actully URL generating methods are provided as [extension methods](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.urlhelperextensions).

 ● `Url.Action(ActionName, ControllerName, RouteValues)`

The `UrlHelper.Action()` uses the controller name, the action name and route values.

```
// Home/Index
Url.Action(action: "Home", controller: "Index")

// Home/Index
Url.Action("Index", "Home")

// Product/Delete/123
Url.Action("Delete", "Product", new { id = 123 })
```

● `Url.RouteUrl(RouteName, RouteValues)` 

The `UrlHelper.RouteUrl()` uses the route name and route values.

```
// Product/Delete/123
Url.RouteUrl(routeName: "Product.Delete", values: new { id = 123 })

// Product/Delete/123
Url.RouteUrl("Product.Delete", new { id = 123 })
```

#### [IHtmlHelper](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.rendering.ihtmlhelper)

A MVC views provide the `Html` property of type [IHtmlHelper](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.rendering.ihtmlhelper).

`IHtmlHelper` provides [extension methods](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.rendering.htmlhelperlinkextensions) for generating [anchor tags](https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/a) in MVC views.

● `Html.ActionLink(LinkText, ActionName, ControllerName, RouteValues)`
 
The `Html.ActionLink()` uses the controller name, the action name and route values. 


```
// generates <a href="/product/delete/123">Delete Product</a>
@Html.ActionLink("Delete Product", "Delete", "Product", new { id = 123 }) 
```

● `Html.RouteLink(LinkText, RouteName, RouteValues)` 

The `Html.RouteLink()` uses the route name and route values.

```
// generates <a href="/product/delete/123">Delete Product</a>
@Html.ActionLink("Delete Product", "Product.Delete", new { id = 123 }) 
```
 
## Route Templates

## Route Tokens

## Route Constraints