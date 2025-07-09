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

The route template `{controller=Home}/{action=Index}/{id?}` matches any controller action. It just sets some defaults, the `Home` controller and the `Index` action and an optional `id` parameter.

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

Using `Route Attributes` makes controller name and action name irrelevant.

The [HttpRouteAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.routeattribute) route attribute can be used either on a contoller or on an action.

The following route attributes are also available. These attributes map to [HTTP Methods](https://en.wikipedia.org/wiki/HTTP#Request_methods) and can be used with controller actions only. Docs refer to the these attributes as [verb templates](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing#http-verb-templates).
 
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
        // returns a view with a list of products that match the search criteria specified by the passed-in parameters
    }

    [HttpGet("list", Name = "Product.List")]            // GET product/list
    public async Task<ActionResult> Index()
    {
        // returns a view with all products
    }

    [HttpGet("paging", Name = "Product.Paging")]        // GET product/paging
    public async Task<ActionResult> Paging()
    {
        // returns a view with a list of products where the list is a part of the total products
        // the query string must contain a pageindex and a pagesize parameter
    }

    [HttpGet("insert")]                                 // GET product/insert
    public async Task<ActionResult> Insert()
    {
        // returns a view for creating a new product
    }

    [ValidateAntiForgeryToken]
    [HttpPost("insert", Name = "Product.Insert")]       // POST product/insert     
    public async Task<ActionResult> Insert(ProductModel Model)
    {
        // creates a new product and returns the Index or Edit view
    }

    [HttpGet("edit/{Id}")]                              // GET product/edit/123
    public async Task<ActionResult> Edit(string Id)
    {
        // returns a view for editing a product
    }

    [ValidateAntiForgeryToken]
    [HttpPost("edit", Name = "Product.Edit")]           // POST product/edit     
    public async Task<ActionResult> Edit(ProductModel Model)
    {
        // updates a product and returns the Index or Edit view
    }

    [HttpGet("delete/{Id}", Name = "Product.Delete")]   // GET product/delete/123
    public async Task<ActionResult> Delete(string Id)
    {
       // deletes a product and returns the Index view
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


## Route Templates

A [Route Template](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing#route-templates) defines the structure of a URL.

 ● A route template consists of segments separated by `/`

```product/list```

 ● Segments could be **literal** segments and **placeHolder** segments
 
In the following route template the `product` is a literal segment where the `{id}` is a placeholder segment.

``` product/{id} ```

 ● A placeholder segment defines the name of a **route parameter**. 

 In the next example `id` is the name of a route parameter.

 ``` product/{id} ```

 ● Reserved keywords for route parameters

The following [keywords](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing#reserved-routing-names) are reserved and have a special meaning when used with route parameters.

- action
- area
- controller
- handler
- page

 ● Using keywords, route parameters can be matched to controllers and actions 

The next route template
```
{controller}/{action}
```

can match a request URL such as the following

```
// GET https://company.com/home/index
```
to the following controller and action

```
public class HomeController : Controller
{
    public IActionResult Index()
    {
        ...
    }
}
```
 ● Route parameters can have default values

 ```
{controller=Home}/{action=Index}
 ```

 ● Route parameters can match to action method parameters

 ```
// GET product/123

[Route("product/{id}")]
public IActionResult GetProduct(int Id)
{
    string sId = Id.ToString();     // 123
}
```

 ● In a route template literal segments and placeholder segments, i.e. parameters, can be in any order.

Given the next route template

``` /segment1/{param1}/segment2/{param2} ```

and the next controller action

```
[HttpGet("/segment1/{param1}/segment2/{param2}", Name = "ParamTest")]
public IActionResult PatternTest(string Param1, string Param2)
{
    return View();
}
```

the following in a MVC view

``` @Html.RouteLink("Param Test", "ParamTest", new { Param1 = "123", Param2 = "456" }) ```

generates the next anchor tag

``` <a href="/segment1/123/segment2/456">Param Test</a> ```

which is perfectly routable.


 ● The `*` and `**` prefixes define a **catch-all** parameter

A **catch-all** parameter binds to the rest of a URL path

The following

``` library/docs/{**slug} ```

matches any URL path starting with `library/docs/` and having anything after that.

Anything that comes after `library/docs/` is assigned to a route parameter under the name `slug`.
 
Special characters in the route parameter value are escaped. That includes the path separator `/` character.

``` library/docs/getting/started/guide```

becomes

``` library/docs/getting%2Fstarted%2Fguide```

 ● The `?` suffix makes a route parameter optional

The following route template

``` {controller}/{action}/{id?} ```

can match to a number of paths.

```
product/edit/123

product/delete/123

product/list

home/index
```

## Route Template Tokens

A route template may contain [tokens](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing#token-replacement-in-route-templates-controller-action-area).

A token is one of the following reserved keywords enclosed in square brackets.

- [area]
- [controller]
- [action]

Route template tokens are replaced with the values of the area name, controller name and action name based on the action where the route is defined.

```
[Route("[controller]/[action]")]
public class HomeController : Controller
{

    public IActionResult Index()
    {
        ...
    }

    public IActionResult About()
    {
        ...
    }
}
```

## Route Constraints

A route parameter may have [constraints](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing#route-constraints). A constraint is defined using the `:` character.

> Route constraints are **not** input validation tools. They used by the system in order to disambiguate similar routes. 

``` {id:int} ```

The last example defines a route parameter with name `id` and constraint `int`, i.e. it matches only when the route value is an integer.

Please consider the route constraints table in [docs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing#route-constraints).
 






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

#### The LinkGenerator class

The [LinkGenerator](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing#url-generation-concepts) class is used internally by `IUrlHelper` and `IHtmlHelper` implementations in generating URLs.

`LinkGenerator` is automatically registered with the Dependency Injection container as a singleton service and is available even outside of the context of an executing request.

## Model Binding Attributes

The [Microsoft.AspNetCore.Mvc](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc) namespace provides a groupd or `From` attributes designed to be used with action method parameters.

These `From` attributes indicate that an action method parameter is bound to value coming from a specific source, e.g. route, querey, body, etc.

- [FromRouteAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.fromrouteattribute)
- [FromQueryAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.fromqueryattribute)
- [FromHeaderAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.fromheaderattribute)
- [FromBodyAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.frombodyattribute)
- [FromFormAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.fromformattribute)
- [FromServicesAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.fromservicesattribute)

The next is an example use of the `FromRouteAttribute`.

```
[HttpGet("edit/{Id}")]                               
public async Task<ActionResult> Edit([FromRoute] string Id)
{
    // returns a view for editing a product
}        

// a variation

[HttpGet("edit/{Id}")]                               
public async Task<ActionResult> Edit([FromRoute(Name ="Id")] string ProductId)
{   
}  
```

The next is an example use of the `FromQueryAttribute`.

```
[HttpGet("paging", Name = "Product.Paging")]        
public async Task<ActionResult> Paging([FromQuery]PageIndex, [FromQuery]PageSize)
{ 
}
```
 
## Display EndPoint List

The [EndpointDataSource](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.endpointdatasource) provides the enumerable `EndPoints` property.

The `EndpointDataSource` is automatically registered with Dependency Injection.

It is easy to display end point information using the `EndpointDataSource` service by writing a method like the following.

```
static public List<string> GetEndPointList(EndpointDataSource endpointDataSource)
{
    List<string> EndPointList = new();

    RouteEndpoint REP;
    string DisplayName;
    string Pattern;
    string S;

    foreach (var EP in endpointDataSource.Endpoints)
    {
        REP = EP as RouteEndpoint;
        if (REP != null)
        {
            DisplayName = !string.IsNullOrWhiteSpace(REP.DisplayName) ? REP.DisplayName : "no name";
            Pattern = !string.IsNullOrWhiteSpace(REP.RoutePattern.RawText) ? REP.RoutePattern.RawText : "no pattern";
            S = $"DisplayName = {DisplayName}, Pattern = {Pattern}";
            EndPointList.Add(S);
        }
    }

    return EndPointList;
}
```

