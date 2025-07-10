# Routing

- [Routing](#routing)
  - [Components in Asp.Net Core Routing](#components-in-aspnet-core-routing)
  - [Controller Routing](#controller-routing)
    - [Mixing Conventional and Attribute Routing](#mixing-conventional-and-attribute-routing)
  - [Conventional Routing](#conventional-routing)
    - [Multiple conventional routes](#multiple-conventional-routes)
  - [Attribute Routing](#attribute-routing)
    - [Route Attributes](#route-attributes)
    - [Attribute Routing with Http Verb Attributes](#attribute-routing-with-http-verb-attributes)
  - [Route Names](#route-names)
  - [Route Templates](#route-templates)
  - [Route Template Tokens](#route-template-tokens)
  - [Route Constraints](#route-constraints)
  - [Generating URLs](#generating-urls)
      - [IUrlHelper](#iurlhelper)
      - [IHtmlHelper](#ihtmlhelper)
      - [The LinkGenerator class](#the-linkgenerator-class)
  - [Model Binding Attributes](#model-binding-attributes)
  - [Display Controller Endpoint List](#display-controller-endpoint-list)
  - [Display a List of all Endpoints](#display-a-list-of-all-endpoints)
  - [Dynamic Endpoints - Using a custom EndpointDataSource](#dynamic-endpoints---using-a-custom-endpointdatasource)
    - [Dynamic Endpoints - Return an MVC View](#dynamic-endpoints---return-an-mvc-view)
    - [Dynamic Endpoints - A POST Endpoint](#dynamic-endpoints---a-post-endpoint)
  - [Dynamic Endpoints - Using a Multi Endpoint Middleware](#dynamic-endpoints---using-a-multi-endpoint-middleware)
  - [Routes for a REST Api](#routes-for-a-rest-api)


In Asp.Net Core [Routing](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing) is a procedure that matches an incoming HTTP request to a request handler known as [Endpoint](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.routeendpoint).

Incoming HTTP request handling can be configured to be delegated to

- [MVC controllers](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions)
- [WebApi controllers](https://learn.microsoft.com/en-us/aspnet/core/web-api)
- [Minimal Api End Points](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview)
- [Razor Pages](https://learn.microsoft.com/en-us/aspnet/core/razor-pages)
- [SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction)
- [gRPC Services](https://learn.microsoft.com/en-us/aspnet/core/grpc)
- Built-in middlewares such as [Static Files](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files)
- [Custom Middlewares and Request Delegates](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware)

> This text covers routing in general and routing regarding MVC and WebApi controllers.

## Components in Asp.Net Core Routing

- [Route Template](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing#route-templates). Defines the structure of a URL and it may contain placeholders for route values, e.g. `/customer/{id}` where `{id}` is a placeholder.
- [Route Values](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.routevaluedictionary). A dictionary of `Key-Value` pairs constructed from placeholders contained in the route template where `Key` is the placeholder, e.g. `id` and `Value` the actual placeholder value, e.g. `1234` in the request URL.
- [Route Constraints](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing#route-constraints). A constraint applied to a route placeholder value, e.g. `{id:int}`.


## Controller Routing

The built-in `Routing Middleware` is used by Asp.Net Core in matching incoming request URLs to controller actions. That matching is done using the defined `Route Templates`. 

`Route Templates` can be defined

- in application startup code and is known as `Conventional Routing`
- using attributes in controllers and actions and is known as `Attribute Routing`

### Mixing Conventional and Attribute Routing

An application can mix the use of conventional routing and attribute routing.

Placing a route attribute on a controller class makes all of its actions attribute routed.

Placing a route attribute on a controller action makes only that action attribute routed.

Controller actions are either conventionally routed or attribute routed.

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

### Multiple conventional routes

It is perfectly valid to add more than one call to `MapControllerRoute()` and `MapAreaControllerRoute()`, provided that the pattern is different.

Multiple conventional routes added that way cover more specific cases, such as when they explicitly define the controller and the action in the pattern. These more specific configurations should be placed before the `default` general route.

The next example is from the official [docs](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing#multiple-conventional-routes).

```
app.MapControllerRoute(name: "blog",
                pattern: "blog/{*article}",
                defaults: new { controller = "Blog", action = "Article" });
app.MapControllerRoute(name: "default",
               pattern: "{controller=Home}/{action=Index}/{id?}");
```


## Attribute Routing

Attribute Routing is mostly used with [REST](https://en.wikipedia.org/wiki/REST) Web Apis but it can be used with MVC applications too.
 
Attribute routing is done by marking controller classes and controller actions with route attributes.

The `MapControllers()` is used with Web Api applications to map attribute routed controllers.

```

// ...

app.MapControllers();

app.Run();
```

### Route Attributes

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

### Attribute Routing with Http Verb Attributes
 
[Verb Attributes](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-9.0#attribute-routing-with-http-verb-attributes) impose a `HTTP` method.

```
[Route("product")]
public class ProductController : Controller
{ 
    // ● List
    [Permission("Product.View")]
    [HttpGet("list", Name = "Product.List")]
    public async Task<ActionResult> Index()                     // GET product/list
    {
        // returns a view with all products
    }

    // ● Insert
    [Permission("Product.Insert")]
    [HttpGet("insert", Name = "Product.Insert")]
    public async Task<ActionResult> Insert()                    // GET product/insert
    {
        // returns a view for creating a new product
    }

    // ● Edit
    [Permission("Product.Edit")]
    [HttpGet("edit/{Id}", Name = "Product.Edit")]
    public async Task<ActionResult> Edit(string Id)             // GET product/edit/123
    {
        // returns a view for editing an existing product
    }

    // ● Delete
    [Permission("Product.Delete")]
    [HttpGet("delete/{Id}", Name = "Product.Delete")]           // GET product/delete/123
    public async Task<ActionResult> Delete(string Id)           
    {
        // deletes a product and returns the Index view
    }

    // ● Save
    [ValidateAntiForgeryToken]
    [Permission("Product.Edit")]
    [HttpPost(Name = "Product.Save")]                           // POST product
    public async Task<ActionResult> Save(ProductModel Model)    
    {
        // either creates a new product
        // or udpates an existing product
        // and returns the Edit view
    }

    // ● Search (with paging)
    [Permission("Product.View")]
    [HttpGet("search", Name = "Product.Search")]                // GET product/search
    public async Task<ActionResult> Search(string Term = "", string CategoryId = "")
    {
        // returns a view with a list of products 
        // that match the search criteria specified by the passed-in parameters
    }

    // ● Paging
    [Permission("Product.View")]
    [HttpGet("paging", Name = "Product.Paging")]                // GET product/paging
    public async Task<ActionResult> Paging()                    
    {
        // returns a view with a list of products 
        // where the list is a part, known as page, of the total products
        // NOTE: the query string must contain a pageindex and a pagesize parameter
    }
}
```

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

 ● Segments could be **literal** segments and **placeholder** segments
 
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

> [From MDN](https://developer.mozilla.org/en-US/docs/Glossary/Slug): _A Slug is the unique identifying part of a web address, typically at the end of the URL_. 

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

MVC views provide the `Html` property of type [IHtmlHelper](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.rendering.ihtmlhelper).

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
 
## Display Controller Endpoint List

The [IActionDescriptorCollectionProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.infrastructure.iactiondescriptorcollectionprovider) is registered automatically as a service by Asp.Net Core.

The `IActionDescriptorCollectionProvider` provides access to the currently cached collection of [ActionDescriptor](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.abstractions.actiondescriptor) instances by its `ActionDescriptors.Items` property.

The `ActionDescriptor` class describes an action.

Most probably the items of the `ActionDescriptors.Items` collection are [ControllerActionDescriptor](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllers.controlleractiondescriptor) instances.

The `ControllerActionDescriptor` is a subclass of the `ActionDescriptor` class and describes a controller action.

Both `ActionDescriptor`  and  `ControllerActionDescriptor` classes provide a lot of information about controller actions.

Consider the next example.

```
...

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

RootServiceProvider = (app as IApplicationBuilder).ApplicationServices;
IActionDescriptorCollectionProvider Provider = RootServiceProvider.GetService<IActionDescriptorCollectionProvider>();
foreach (ActionDescriptor Item in Provider.ActionDescriptors.Items)
{
    if (Item is ControllerActionDescriptor)
    {
        ControllerActionDescriptor Descriptor = Item as ControllerActionDescriptor;
        string ControllerName = Descriptor.ControllerName;
        string ActionName = Descriptor.ActionName;                    
    }
}

app.Run();

```

## Display a List of all Endpoints

Asp.Net Core template projects use the `app` variable in application startup code.

That `app` variable is of type [WebApplication](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.webapplication).

The `WebApplication` class implements the [IEndpointRouteBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.iendpointroutebuilder) interface.

The `IEndpointRouteBuilder` provides access to a collection  of [EndpointDataSource](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.endpointdatasource) instances by its `DataSources` property.

In an Asp.Net Core application there maybe more than one `EndpointDataSource`, thus the need for a collection.

The developer may add custom `EndpointDataSource` subclasses to that `IEndpointRouteBuilder.DataSources` collection. Perhaps for adding **dynamic** `Endpoint` instances.

The `EndpointDataSource` provides a collection of `Endpoint` instances by its `Endpoints` property.

Most probably the items of the `EndpointDataSource.Endpoints` collection are [RouteEndpoint](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.routeendpoint) instances.

Both `Endpoint`  and  `RouteEndpoint` classes provide a lot of information about application end-points.

The [Endpoint.Metadata](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.endpoint.metadata) property is a [EndpointMetadataCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.endpointmetadatacollection) instance.

`EndpointMetadataCollection` is a `IReadOnlyList<object>` implementor, that is it is a read only list of `object` instances.

Each `object` in the `EndpointMetadataCollection` is an instance of some class with a specific meaning to Asp.Net Core.

In that collection there may be `metadata` classes, such as [RouteNameMetadata](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.routenamemetadata), [HttpMethodMetadata](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.httpmethodmetadata).

Other classes that can be found in that `metadata` collection are

- AuthorizeAttribute
- ControllerAttribute
- RouteAttribute
- HttpGetAttribute or HttpPostAttribute
- TypeFilterAttribute
- SaveTempDataAttribute
- Custom Authorize Attributes
- EndpointNameMetadata
- ControllerActionDescriptor
- HttpMethodActionConstraint
- etc.

Consider the next example.

```
...

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

EndPointDataSources = (app as IEndpointRouteBuilder).DataSources;
foreach (EndpointDataSource DS in EndPointDataSources)
{
    foreach (Endpoint Item in DS.Endpoints)
    {
        if (Item is RouteEndpoint)
        {
            RouteEndpoint routeEndpoint = Item as RouteEndpoint;
            string Pattern = routeEndpoint.RoutePattern.RawText;
            string Description = routeEndpoint.DisplayName;

            RouteNameMetadata routeNameMetadata = routeEndpoint.Metadata.FirstOrDefault(x => x.GetType() == typeof(RouteNameMetadata)) as RouteNameMetadata;
            HttpMethodMetadata httpMethodMetadata = routeEndpoint.Metadata.FirstOrDefault(x => x.GetType() == typeof(HttpMethodMetadata)) as HttpMethodMetadata;

            string RouteName = routeNameMetadata != null ? routeNameMetadata.RouteName : string.Empty;
            string HttpMethods = httpMethodMetadata != null ? string.Join(", ", httpMethodMetadata.HttpMethods) : string.Empty;
        }
    }
}

app.Run();
```

## Dynamic Endpoints - Using a custom EndpointDataSource

A subclass of the [EndpointDataSource](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.endpointdatasource) is required.

That custom `EndpointDataSource` should be registered as singleton service and should be added to the `IEndpointRouteBuilder.DataSources` collection. `IEndpointRouteBuilder` is discussed earlier in this text.

The `EndpointDataSource` class has two abstract members

- `public abstract IChangeToken GetChangeToken();`
- `public abstract IReadOnlyList<Endpoint> Endpoints { get; }`
 
Whenever a new `Endpoint` is added to `EndpointDataSource.Endpoints` a new `IChangeToken` should be generated. 

That `IChangeToken` is the mechanism through which Asp.Net Core gets informed that new `Endpoints` are added to that `Endpoints` list and as a result it re-reads that list.

Next is a custom `EndpointDataSource` along with some utility classes.

The `EndPointInfo` is used in adding dynamically new `Endpoint` instances.

```
public class EndPointInfo
{
    RequestDelegate requestDelegate;
    string httpMethod;

    public EndPointInfo(string Pattern) 
    { 
        Pattern = Pattern ?? throw new ArgumentNullException(nameof(Pattern));             
        this.Pattern = Pattern; 
    }

    protected virtual Task Handler(HttpContext context)
    {
        context.Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
        return context.Response.WriteAsync("Not Found");
    }

    public string Pattern { get; }
    public RequestDelegate RequestDelegate
    {
        get => requestDelegate != null ? requestDelegate : Handler;
        set => requestDelegate = value;
    } 
    public string HttpMethod
    {
        get => !string.IsNullOrWhiteSpace(httpMethod) ? httpMethod : "GET";
        set => httpMethod = value;
    }
    public string RouteName { get; set; }        
    public int Order { get; set; }
    public string Description { get; set; }
    public List<object> Metadata { get; } = new List<object>();
}
```

Consider the `DynamicEndpointDataSource` class.

```
public class DynamicEndpointDataSource : EndpointDataSource
{
    static readonly System.Threading.Lock syncLock = new();
    static internal ICollection<EndpointDataSource> EndPointDataSources;
    static List<EndPointInfo> InfoList = new List<EndPointInfo>();

    List<Endpoint> EndPointList = new List<Endpoint>();
    CancellationTokenSource CancellationTokenSource;
    IChangeToken ChangeToken;

    static bool IsOrInheritsFrom(object Instance, Type T)
    {
        return Instance.GetType() == T || Instance.GetType().IsSubclassOf(T);
    }

    Endpoint CreateEndPoint(EndPointInfo Info)
    {
        if (!string.IsNullOrWhiteSpace(Info.RouteName)
            && Info.Metadata.FirstOrDefault(x => IsOrInheritsFrom(x, typeof(RouteNameMetadata))) == null)
        {
            Info.Metadata.Add(new RouteNameMetadata(Info.RouteName));
        }

        if (!string.IsNullOrWhiteSpace(Info.RouteName)
            && Info.Metadata.FirstOrDefault(x => IsOrInheritsFrom(x, typeof(EndpointNameMetadata))) == null)
        {
            Info.Metadata.Add(new EndpointNameMetadata(Info.RouteName));
        }

        if (!string.IsNullOrWhiteSpace(Info.HttpMethod)
            && Info.Metadata.FirstOrDefault(x => IsOrInheritsFrom(x, typeof(HttpMethodMetadata))) == null)
        {
            Info.Metadata.Add(new HttpMethodMetadata(new string[] { Info.HttpMethod }));
        }

        string DisplayName = !string.IsNullOrWhiteSpace(Info.Description) ? Info.Description : null;


        EndpointMetadataCollection MetadataCollection = new EndpointMetadataCollection(Info.Metadata);
        RoutePattern Pattern = RoutePatternFactory.Parse(Info.Pattern);

        RouteEndpoint Result = new RouteEndpoint(
            Info.RequestDelegate,
            Pattern,
            Info.Order,
            MetadataCollection,
            DisplayName
            );

        return Result;
    }


    public DynamicEndpointDataSource() 
    {
        CancellationTokenSource = new CancellationTokenSource();
        ChangeToken = new CancellationChangeToken(CancellationTokenSource.Token);
    }


    public override IChangeToken GetChangeToken() => ChangeToken;

    public void Add(string Pattern, RequestDelegate RequestDelegate, string HttpMethod = "GET", string RouteName = "")
    {
        EndPointInfo Info = new EndPointInfo(Pattern);
        Info.RequestDelegate = RequestDelegate;
        Info.RouteName = RouteName;
        Info.HttpMethod = HttpMethod; 
        Add(Info);
    }

    public void Add(EndPointInfo Info)
    {
        EndPointInfo[] Infos = { Info };
        Add(Infos);
    }

    public void Add(IEnumerable<EndPointInfo> Infos)
    {
        lock(syncLock)
        {
            int Count = 0;
            foreach (var Info in Infos)
            {
                if (InfoList.FirstOrDefault(x => string.Compare(x.Pattern, Info.Pattern, StringComparison.InvariantCultureIgnoreCase) == 0) == null)
                {
                    InfoList.Add(Info);
                    Endpoint EndPoint = CreateEndPoint(Info);
                    EndPointList.Add(EndPoint);
                    Count++;
                }
            }

            if (Count > 0)
            {
                var OldCancellationTokenSource = CancellationTokenSource;
                CancellationTokenSource = new CancellationTokenSource();
                ChangeToken = new CancellationChangeToken(CancellationTokenSource.Token);
                OldCancellationTokenSource?.Cancel();
            }
        }

    }

    static public Endpoint[] GetAllEndpoints()
    {
        List<Endpoint> List = new();

        if (EndPointDataSources != null)
        {
            foreach (EndpointDataSource DS in EndPointDataSources)
            {
                List.AddRange(DS.Endpoints.ToArray());
            }
        }

        return List.ToArray();
    }

    public override IReadOnlyList<Endpoint> Endpoints => EndPointList;
}

```

Extension methods for wiring-up the `DynamicEndpointDataSource` class to Asp.Net Core machinery and binding an `IFormCollection` to a model.

```
static public class DynamicEndpointDataSourceExtensions
{
    static public void AddDynamicEndpoints(this IServiceCollection Services)
    {
        Services.AddSingleton<DynamicEndpointDataSource>();
    }

    static public void UseDynamicEndPoints(this WebApplication app)
    {
        IEndpointRouteBuilder RouteBuilder = app as IEndpointRouteBuilder;
        DynamicEndpointDataSource.EndPointDataSources = RouteBuilder.DataSources;

        var dataSource = RouteBuilder.ServiceProvider.GetService<DynamicEndpointDataSource>();

        if (dataSource is null)
            throw new Exception("Did you forget to add the DynamicEndpointDataSource service?");

        RouteBuilder.DataSources.Add(dataSource);
    }

    static public async Task<T> BindFromRequestForm<T>(this HttpContext httpContext) where T : class
    {
        // Some guidance from docs on custom model binding
        // SEE: https://learn.microsoft.com/en-us/aspnet/core/mvc/advanced/custom-model-bindingpolymorphic-model-binding
        //
        // also check the following on how internal Asp.Net Core performs model and parameter binding
        // ● ControllerBinderDelegateProvider, used in Controller binding
        // SEE: https://source.dot.net/#Microsoft.AspNetCore.Mvc.Core/Controllers/ControllerBinderDelegateProvider.cs,0336994e1dd319ff
        // ● PageBinderFactory, used in Pages binding
        // SEE: https://source.dot.net/#Microsoft.AspNetCore.Mvc.RazorPages/Infrastructure/PageBinderFactory.cs,ce30817932de7a8c

        // ● ServiceProvider, we use a service provider below
        IServiceProvider ServiceProvider = httpContext.RequestServices;

        // ● DefaultModelMetadataProvider is the actual instance
        // a metadata provider provides instances of ModelMetadata
        // SEE: https://source.dot.net/#Microsoft.AspNetCore.Mvc.Core/ModelBinding/Metadata/DefaultModelMetadataProvider.cs,be000e7f2e9425ff
        // SEE: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.metadata.defaultmodelmetadataprovider
        IModelMetadataProvider MetadataProvider = ServiceProvider.GetRequiredService<IModelMetadataProvider>();

        // ● ModelMetadata, inherits DefaultModelMetadata 
        // a metadata representation of a model type, property or parameter.
        // SEE: https://source.dot.net/#Microsoft.AspNetCore.Mvc.Abstractions/ModelBinding/ModelMetadata.cs,3ca33b8f1a31957c
        // SEE: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.modelmetadata
        ModelMetadata Metadata = MetadataProvider.GetMetadataForType(typeof(T));

        // ● ModelBinderFactory is the actual instance
        // a factory abstraction for creating IModelBinder instances.
        // SEE: https://source.dot.net/#Microsoft.AspNetCore.Mvc.Core/ModelBinding/ModelBinderFactory.cs,0f77e84648e06fa9
        // SEE: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.imodelbinderfactory
        IModelBinderFactory ModelBinderFactory = ServiceProvider.GetRequiredService<IModelBinderFactory>();

        // ● ComplexObjectModelBinder is the actual instance
        // a IModelBinder implementation for binding complex types
        // SEE: https://source.dot.net/#Microsoft.AspNetCore.Mvc.Core/ModelBinding/Binders/ComplexObjectModelBinder.cs,35ac9f0653157305
        // SEE: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.binders.complexobjectmodelbinder
        IModelBinder ModelBinder = ModelBinderFactory.CreateBinder(new ModelBinderFactoryContext() { Metadata = Metadata });

        // ● DefaultModelBindingContext, inherits ModelBindingContext
        // a context that contains operating information for model binding and validation
        // SEE: https://source.dot.net/#Microsoft.AspNetCore.Mvc.Core/ModelBinding/DefaultModelBindingContext.cs,265cda16e15d7110
        // SEE: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.defaultmodelbindingcontext
        // ● FormValueProvider, inherits BindingSourceValueProvider
        // a IValueProvider adapter for data stored in an IFormCollection
        // SEE: https://source.dot.net/#Microsoft.AspNetCore.Mvc.Core/ModelBinding/FormValueProvider.cs,490aec4c70c4b022
        // SEE: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.formvalueprovider
        var BindingContext = new DefaultModelBindingContext
        {
            ModelMetadata = Metadata,
            ModelName = string.Empty,
            ModelState = new ModelStateDictionary(),

            ValueProvider = new FormValueProvider(
                BindingSource.Form,
                httpContext.Request.Form,
                CultureInfo.InvariantCulture
            ),

            ActionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
                )                
        };

        // attempts to bind a model
        await ModelBinder.BindModelAsync(BindingContext);

        // return the model
        return BindingContext.Result.Model as T;
    }   
}
```

Next is an Asp.Net Core `Main()` startup method containing calls to registration extension methods `AddDynamicEndpoints()` and `UseDynamicEndPoints()`.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddDynamicEndpoints();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
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

    app.UseDynamicEndPoints();

    app.Run();
}
```

Next two examples of dynamically adding new `Endpoint` instances to an Asp.Net Core application using the `DynamicEndpointDataSource` class.


```
var DynamicEndpoints = RootServiceProvider.GetService<DynamicEndpointDataSource>();

// using a lambda as request delegate
DynamicEndpoints.Add("dyn", context =>
{
    var List = GetEndPointList();
    return context.Response.WriteAsync("This is a result from a dynamic end point.");
});

// using an EndPointInfo instance
EndPointInfo Info = new EndPointInfo("dyn2");
DynamicEndpoints.Add(Info);
```

The developer may subclass the `EndPointInfo` class and override the 

``` protected virtual Task Handler(HttpContext context) ```

method or just provide a value to the `EndPointInfo.RequestDelegate` property.

The static `DynamicEndpointDataSource.GetAllEndpoints()` returns an empty `Endpoint` list if the `UseDynamicEndPoints()` extension method is not called. 

The `UseDynamicEndPoints()` method, among other things, aquire a reference to the `IEndpointRouteBuilder.DataSources` which is used by the `GetAllEndpoints()` to return the end-point list.

### Dynamic Endpoints - Return an MVC View
 

Consider the next example.

```
using System.Net;
using System.Globalization;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing.Patterns;

public class TestModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class TestGetEndPoint : EndPointInfo
{
    public TestGetEndPoint()  
        : base("test-endpoint/{Id?}")
    {
        RouteName = "Test.Endpoint.Get";
        HttpMethod = HttpMethods.Get;  
    }

    protected override async Task Handler(HttpContext context)
    {
        // read request route data values
        RouteData ReqRouteData = context.GetRouteData();
        var Id = ReqRouteData.Values["Id"];

        // prepare response
        var ActionResultExecutor = context.RequestServices.GetRequiredService<IActionResultExecutor<ViewResult>>();
        var RouteData = context.GetRouteData() ?? new RouteData();
        var ActionContext = new ActionContext(context, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

        TestModel Model = new TestModel() { Id = 123, Name = "John Doe" };

        var ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
        ViewData.Model = Model;
        ViewData["Title"] = "Test Endpoint XXX";

        ITempDataProvider TempDataProvider = context.RequestServices.GetRequiredService<ITempDataProvider>();
        var TempData = new TempDataDictionary(context, TempDataProvider);

        var ViewResult = new ViewResult();
        ViewResult.ViewName = "TestEndpoint"; // or full path @"~/Views/Shared/TestEndpoint.cshtml";
        ViewResult.ViewData = ViewData;
        ViewResult.TempData = TempData;

        await ActionResultExecutor.ExecuteAsync(ActionContext, ViewResult);
    }
}
```

There is tiny `TestModel` just for test purposes.

The `TestGetEndPoint` class derives from `EndPointInfo` and overrides the virtual `Handler()` which is the request delegate.

The `Handler()` prepares everything that is required in order to display a MVC view along with its model and finally executes the [ViewResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.viewresult).

The `Handler()` even prepares the `ViewData` and `TempData` dictionaries which can be used in passing data to the view.

The `TestGetEndPoint` should be added to the `DynamicEndpointDataSource`.

```
var DynamicEndpoints = RootServiceProvider.GetService<DynamicEndpointDataSource>();

EndPointInfo Info = new TestGetEndPoint();  
DynamicEndpoints.Add(Info);
```
### Dynamic Endpoints - A POST Endpoint

Consider the next MVC view containing a form tag.

```
@model TestModel

<h1>@ViewData["Title"]</h1>
 
<form asp-route="Test.Endpoint.Post" method="post">
    <label>Id: <input asp-for="Id" /></label> <br />
    <label>Name: <input asp-for="Name" /></label> <br />
    <input type="submit" value="Submit">
</form>
```

The `asp-route` adds an [action](https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/form#action) attribute of the [HTML Form Element](https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/form) using a route name registered by the dynamic endpoint of the next example.

Next is the dynamic endpoint for the view form.

The `Handler()` method gets a reference to the `HttpRequest.Form` which is an `IFormCollection` instance. That `IFormCollection` instance can be further processed by application code.

```
public class TestPostEndPoint : EndPointInfo
{
    public TestPostEndPoint()
        : base("test-endpoint-post")
    {
        RouteName = "Test.Endpoint.Post";
        HttpMethod = HttpMethods.Post;
    }

    protected override async Task Handler(HttpContext context)
    {
        // read form collection
        IFormCollection Form = await context.Request.ReadFormAsync(); 

        await context.Response.WriteAsync("This is a result from a post.");
    }
}
```

Another way is to bind the `HttpRequest.Form` to an actual model using the `BindFromRequestForm()` extension method presented earlier.

```
public class TestPostEndPoint : EndPointInfo
{
    public TestPostEndPoint()
        : base("test-endpoint-post")
    {
        RouteName = "Test.Endpoint.Post";
        HttpMethod = HttpMethods.Post;
    }

    protected override async Task Handler(HttpContext context)
    {
        // read form collection
        // IFormCollection Form = await context.Request.ReadFormAsync(); 

        TestModel Model = await context.BindFromRequestForm<TestModel>();

        await context.Response.WriteAsync("This is a result from a post.");
    }
}
```
## Dynamic Endpoints - Using a Multi Endpoint Middleware

Consider the next example.

```
using Microsoft.AspNetCore.Routing.Template;

public class EndPointInfo
{
    RequestDelegate requestDelegate;
    string httpMethod;

    public EndPointInfo(string Pattern)
    {
        Pattern = Pattern ?? throw new ArgumentNullException(nameof(Pattern));
        this.Pattern = Pattern;
    }

    protected virtual Task Handler(HttpContext context)
    {
        context.Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
        return context.Response.WriteAsync("Not Found");
    }

    public string Pattern { get; }
    public RequestDelegate RequestDelegate
    {
        get => requestDelegate != null ? requestDelegate : Handler;
        set => requestDelegate = value;
    }
    public string HttpMethod
    {
        get => !string.IsNullOrWhiteSpace(httpMethod) ? httpMethod : "GET";
        set => httpMethod = value;
    }
    public string Key => $"{HttpMethod}:{Pattern}";
}

static public class EndPoints
{
    static readonly System.Threading.Lock syncLock = new();
    static Dictionary<string, EndPointInfo> Handlers = new Dictionary<string, EndPointInfo>();

    static public void Add(EndPointInfo Info)
    {
        if (!Handlers.ContainsKey(Info.Key))
            Handlers[Info.Key] = Info;
    }
    static public void Remove(EndPointInfo Info)
    {
        if (Handlers.ContainsKey(Info.Key))
            Handlers.Remove(Info.Key);
    }

    static public void Add(string HttpMethod, string Pattern, RequestDelegate Handler)
    {
        HttpMethod = HttpMethod ?? throw new ArgumentNullException(nameof(HttpMethod));
        Pattern = Pattern ?? throw new ArgumentNullException(nameof(Pattern));
        Handler = Handler ?? throw new ArgumentNullException(nameof(Handler));

        EndPointInfo Info = new EndPointInfo(Pattern);
        Info.HttpMethod = HttpMethod;
        Info.RequestDelegate = Handler;

        Add(Info);
    }
    static public void Remove(string HttpMethod, string Pattern)
    {
        string Key = $"{HttpMethod}:{Pattern}";
        if (Handlers.ContainsKey(Key))
            Handlers.Remove(Key);
    }

    static public RequestDelegate FindHandler(HttpRequest Request)
    {
        foreach (EndPointInfo Info in Handlers.Values)
        {
            var templateParser = TemplateParser.Parse(Info.Pattern);
            var matcher = new TemplateMatcher(templateParser, null);
            if (matcher.TryMatch(Request.Path, Request.RouteValues))
            {
                return Info.RequestDelegate;
            }
        }

        return null;
    }
    static public bool HandlerExists(HttpRequest Request)
    {
        return FindHandler(Request) != null;
    }
}


public class MultiEndPointMiddleware : IMiddleware
{
    public MultiEndPointMiddleware()
    { 
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        RequestDelegate Handler = EndPoints.FindHandler(context.Request);
        if (Handler != null)
        {
            await Handler(context);
        }
        else
        {
            await next(context);
        }
    }
}
```

The `EndPointInfo` of this example is very similar to the one presented earlier.

The static `EndPoints` class is actually a registry of `EndPointInfo` instances. 

The `MultiEndPointMiddleware` should be configured to run just before the middleware registered by calls such as the `MapControllerRoute()`.

The `MultiEndPointMiddleware` checks if there is an `EndPointInfo` instance registered with the  static `EndPoints` class which can handle the current request. 

If there is one it calls its `RequestDelegate` in order to process the request and then terminates the pipeline. The request pipeline concludes.

If no `EndPointInfo` handler is found it passes the request to the next delegate and the pipeline continues.

The next example demonstrates how to register the `MultiEndPointMiddleware` middleware.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllersWithViews();
    builder.Services.AddTransient<MultiEndPointMiddleware>();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthorization();

    MultiMiddlewareTest.AddTests();

    app.UseMiddleware<MultiEndPointMiddleware>();

    app.MapStaticAssets();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.Run();
}
```

Here is how to add a test `EndPointInfo`.

```
static public class MultiMiddlewareTest
{
    static public void AddTests()
    {
        EndPoints.Add("GET", "multi/{Id?}", context => {
            return context.Response.WriteAsync("This is a result from a handler of MultiEndPointMiddleware.");
        });
    }
}
```

## Routes for a REST Api

| HTTP Method | Path            | Controller.Action   | Notes                                  |
| ----------- | --------------- | ------------------- | -------------------------------------- |
| GET         | /product        | Product.Index()     | Display a View with the list of Items  |
| GET         | /product/insert | Product.Insert()    | Display a View for creating a new Item |
| GET         | /product/{id}   | Product.Edit(Id)    | Display a View for editing an Item     |
| DELETE      | /product/{id}   | Product.Delete(Id)  | Deletes an Item                        |
| POST        | /product        | Product.Save(Model) | Saves a newly created or edited Item   |

