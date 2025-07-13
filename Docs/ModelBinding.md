# Model Binding

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

In Asp.Net Core [Model Binding](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding) is a built-in procedure which 
- converts incoming [HTTP](https://en.wikipedia.org/wiki/HTTP) Request data into .Net types
- and binds that data 
- - to controller action method parameters 
- - or Razor Page method parameters and public properties.

## Sources
Model binding retrieves data from various sources in the form of `Key-Value` pairs. 

Model binding sources are checked in the following order

- Form fields. `application/x-www-form-urlencoded` or `multipart/form-data` content types.
- Request body. `application/json` or `application/xml` content types. With controllers annotated with the `ApiControllerAttribute`.
- Route data.
- Query string parameters.

### Source - Form fields

Suppose the next model.

```
public class TestModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```
Consider the next MVC view containing a form tag.

```
@model TestModel

<h1>@ViewData["Title"]</h1>
 
<form asp-route="Test.Post" method="post">
    <label>Id: <input asp-for="Id" /></label> <br />
    <label>Name: <input asp-for="Name" /></label> <br />
    <input type="submit" value="Submit">
</form>
```

The default [enctype](https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormElement/enctype) of a HTML Form element is `application/x-www-form-urlencoded`.

Next is the HTTP message the server receives when that form is submitted. 

```
POST /test HTTP/1.1
Host: example.com
Content-Type: application/x-www-form-urlencoded
Content-Length: 27

Id=123&Name=John Doe
```

If the Form's `enctype` is set to `multipart/form-data`

```
<form asp-route="Test.Endpoint.Post" method="post" enctype="multipart/form-data">
```

then the HTTP message the server receives when that form is submitted is as following.  

```
POST /test HTTP/1.1
Host: example.com
Content-Type: multipart/form-data;boundary="delimiter12345"

--delimiter12345
Content-Disposition: form-data; name="Id"

123
--delimiter12345
Content-Disposition: form-data; name="Name";  

John Doe
--delimiter12345--
```

In both cases Asp.Net Core Model Binding automatically reads that data, converts it to an instance of the `TestModel` and passes that instance to the proper controller action.

```
[HttpPost("/test", Name = "Test.Post")]
public async Task<IActionResult> Action1(TestModel Model)
{    
}
```
 
### Source - Request body 

The next is the HTTP message with the request body when content type is `application/json`.

This is the case with controllers annotated with the `ApiControllerAttribute`, i.e. with [WebApi](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api) applications.

```
POST /test HTTP/1.1
Host: example.com
Accept: application/json
Content-Type: application/json
Content-Length: 81

{
  "Id": 123,
  "Name": "John Doe"
}
```

Asp.Net Core Model Binding automatically reads that request body data, converts it to an instance of the `TestModel` and passes that instance to the proper controller action.

```
[HttpPost("/test")]
public async Task<IActionResult> Action1(TestModel Model)
{    
}
```

### Source - Route data

Given the next [route template](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing#route-templates)

``` /segment1/{param1}/segment2/{param2} ```

the next controller action

```
[HttpGet("/segment1/{param1}/segment2/{param2}", Name = "ParamTest")]
public IActionResult PatternTest(int Param1, int Param2)
{
    return View();
}
```

and the next `GET` call

```
GET /segment1/123/segment2/456 HTTP/1.1
Host: example.com
Accept: */*
```

Asp.Net Core Model Binding automatically parses that route data, converts it to .Net types and passes that values as parameters to the proper controller action

### Source - Query string parameters

The `https://example.com/query-test?Param1=123&Param2=345` sends the following HTTP message to the server.

```
GET /query-test?Param1=123&Param2=345 HTTP/1.1
Host: example.com
Accept: */*
```

Asp.Net Core Model Binding automatically parses that query string, converts it to .Net types and passes that values as parameters to the proper controller action.

```
[HttpGet("/query-test")]
public IActionResult QueryTest(int Param1, int Param2)
{
    return View();
}
```

### From Attributes

The following attributes are available that can be used to dictate the source of the Model Binding.

- [FromFormAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.fromformattribute) - Binds values from posted form fields.
- [FromBodyAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.frombodyattribute) - Binds values from the request body.
- [FromRouteAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.fromrouteattribute) - Binds values from route data.
- [FromQueryAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.fromqueryattribute) - Binds values from the query string.
- [FromHeaderAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.fromheaderattribute) - Binds values from HTTP headers.

Asp.Net Core Model Binding is smart enough so an application rarely needs to use these attributes.

Docs offer some [examples](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding#sources).

```
[HttpPost("/test/save")]
public async Task<IActionResult> Save([FromBody]TestModel Model)
{    
}

[HttpGet("/get-products", Name = "Test.Post")]
public async Task<IActionResult> GetProducts([FromHeader(Name = "Accept-Language")] string CultureCode)
{    
}
```

## Binding Attributes

### [ModelBinderAttribute]()
### [BindPropertyAttribute]()
### [BindAttribute]()
### [BindRequiredAttribute]()
### [BindNeverAttribute]()


## Deep Diving to Model Binding

Next is a list of terms and links that may help the developer in a deep diving into Asp.Net Core Model Binding.

- [Source](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding#sources). Sources such as Form fields or Query string used by Model Binding to get incoming data.
- [Target](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding#targets). Targets are code entities, such as controller action parameters, Model Binding tries to find values for.
- [Simple Types](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding#simple-types). Primitive and built-in target data types, such as `int`, `bool` and `DateTime`.
- [Complex Types](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding#complex-types). Target data types that are classes, structs and records.
- [Input Formatter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.formatters). An input formatter handles a particular content-type such as `JSON`, `XML`, etc. Implements the [IInputFormatter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.formatters.iinputformatter) interface.
- [Value Provider](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding#additional-sources). Provides source data for model binding. It decouples the source of data from the model binding process. Implements the [IValueProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.ivalueprovider) interface. There are classes such as `FormValueProvider`, `RouteValueProvider` and `QueryStringValueProvider`                 .
- [Value Provider Factory](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding#additional-sources). Provides `IValueProvider` instances. Implements the [IValueProviderFactory](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.ivalueproviderfactory) interface.        
- [Model Binder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.binders). Binds data from an HTTP request to a .NET object. Implements the [IModelBinder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.imodelbinder) interface. The [ComplexObjectModelBinder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.binders.complexobjectmodelbinder) class is the model binder for complex types.
- [Model Binder Provider](). Creates `IModelBinder` instances. Implements the [IModelBinderProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.imodelbinderprovider) interface.
- [Model Binder Factory](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding). A factory for creating `IModelBinder` instances. Implements the [IModelBinderFactory](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.imodelbinderfactory) interface.        
- [Model Metadata](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.modelmetadata). A metadata representation of a model type, property or parameter.               
- [Model Metadata Provider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.metadata.defaultmodelmetadataprovider). Provides instances of `ModelMetadata`. Implements the [IModelMetadataProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.imodelmetadataprovider) interface.      
- [Model Binding Context](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.defaultmodelbindingcontext). A context that contains operating information for model binding.     
- [Custom Model Binding](https://learn.microsoft.com/en-us/aspnet/core/mvc/advanced/custom-model-binding). Docs offer a chapter about `Custom Model Binding`.

 




 
