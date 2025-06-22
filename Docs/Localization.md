# Localization

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

A web application, MVC or WebApi, may opt to provide its content in more than one languages. 

We live in a multilingual world and a multilingual MVC website has the potential to attract much more visitors.

[Localization](https://en.wikipedia.org/wiki/Internationalization_and_localization) in software is a process of making an application to display its content in the language requested by a visitor and to use number formatting, date formatting, etc. in the culture of the visitor.

[Culture](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo) in .Net is a term and software entity which specifies a language and, optionally, a region. A culture which specifies both the language and the region, e.g. `en-US`, is called `Specific Culture`. A culture that specifies the language only,  e.g. `en`, is called `Neutral Culture`. A `Specific Culture`, e.g. `en-US`,  belongs to a `Parent Culture`, e.g. `en`, which is always a `Neutral Culture`.

## Startup Configuration
 
```
var builder = WebApplication.CreateBuilder(args);

// some services here
// ...

// add localization service and specify the Resources folder
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var app = builder.Build();

// some middlewares here
// ...

// cultures supported by this application
var Cultures = new[] { new CultureInfo("en-US"), new CultureInfo("en-GR") };
 
// Request Localization middleware
app.UseRequestLocalization((RequestLocalizationOptions options) =>
{
    options.DefaultRequestCulture = new RequestCulture("en-US"),  // Default culture
    options.SupportedCultures = Cultures;
    options.SupportedUICultures = Cultures;
    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});
 
// some middlewares here
// ...

app.MapControllerRoute(...);

app.Run();
```

The above code first adds the localization service with `AddLocalization()`.

In the middlewares section it defines the cultures supported by this application and adds the request localization middleware with `UseRequestLocalization()`.

The code uses the [CookieRequestCultureProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.localization.cookierequestcultureprovider) class, which determines the culture information for a request using the value of a cookie.

## Request Culture Providers

A `Request Culture Provider` is a class used in [determining the culture](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization/select-language-culture) to be used with a request.

The [RequestLocalizationOptions](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.requestlocalizationoptions) used with the `UseRequestLocalization()` method provides the `RequestCultureProviders` property which is a list of the registered request culture providers.

An application may register more than one request culture providers. 

In every request the `RequestCultureProviders` list is enumerated in order to find a provider that has successfully determined the request culture. The first one wins the battle.

Asp.Net core provides the following built-in request culture providers.

- [QueryStringRequestCultureProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.localization.querystringrequestcultureprovider). Is registered by default. Determines the request culture using the query string, e.g. `https://mysite.com/?culture=en-US` or `https://mysite.com/?culture=en-US&ui-culture=en-US`.
- [CookieRequestCultureProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.localization.cookierequestcultureprovider). Determines the request culture using a cookie value.
- [AcceptLanguageHeaderRequestCultureProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.localization.acceptlanguageheaderrequestcultureprovider). Determines the request culture using the `Accept-Language` HTTP header of the request.
- [RouteDataRequestCultureProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.localization.routing.routedatarequestcultureprovider). Determines the request culture using the culture route value, i.e. using controller or action attributes as `[Route("{culture}/[controller]")]`. Check [this article](https://andrewlock.net/applying-the-routedatarequest-cultureprovider-globally-with-middleware-as-filters/).

All request culture providers inherit from the abstract [RequestCultureProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.localization.requestcultureprovider) class and must provide implementation for the abstract method `DetermineProviderCultureResult()` which returns a [ProviderCultureResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.localization.providercultureresult) instance.

Asp.Net Core also provides the [CustomRequestCultureProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.localization.customrequestcultureprovider) which can be used in providing [a custom solution to the request culture selection](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization/select-language-culture?#use-a-custom-provider).

## Localization Resources

Localization resources are files with `.resx` extension. The [file name](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization/provide-resources?#resource-file-naming) 
- may indicate the **full type name** of the class, controller, view or view component which consumes the resource
- always indicate the culture it is for.

`MyApp.Controllers.HomeController.el-GR.resx` may be the file name of a resource file for the `HomeController` and Greek culture. 

Neutral cultures can be used, e.g. `MyApp.Controllers.HomeController.el.resx`. Asp.Net Core uses a [culture fallback logic](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization/provide-resources?#culture-fallback-behavior).

Resource files may be placed in the `Resources` folder or in special sub-folders, such as `Resources/Controllers/HomeController.el-GR.resx` for a controller with full type name as `Controllers.HomeController`.

Simpler solutions are always preferable. It is easier to have a general resource file, e.g. `Resources.el-GR.resx` and use proper resource keys, e.g. `Home.Wellcome` or `Login.UserName`.
 
## Asp.Net Core content localization

Asp.Net Core [recommends using](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization/make-content-localizable) the following interfaces.

- [IStringLocalizer](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization/make-content-localizable?#istringlocalizer) for generally providing localized strings that may reside in a `resx` file or in any other medium.
- [IHtmlLocalizer](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization/make-content-localizable?#ihtmllocalizer) for resources that contain HTML code.
- [IViewLocalizer](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization/make-content-localizable?#view-localization) for providing localized strings to a View.
 
The projects accompanying this text use a different approach.

## Another way to provide string localization

Create an interface which represents an object that provides localized resources such as string and images.

```
public interface IResourceProvider
{
    string GetString(string Key, CultureInfo Culture = null);
    object GetObject(string Key, CultureInfo Culture = null);
    byte[] GetBinary(string Key, CultureInfo Culture = null);
    object GetImage(string Key, CultureInfo Culture = null);

    string Name { get; }
}
```

One `IResourceProvider` may use a `resx` file while another may use some other medium, such as a database or a `JSON` or `XML` file.

Here is the base implementation.

```
public class ResourceProviderBase : IResourceProvider
{
    // ● construction
    ResourceProviderBase() { }
    protected ResourceProviderBase(string Name = null)
    {
        this.Name = !string.IsNullOrWhiteSpace(Name) ? Name : this.GetType().FullName;
    }

    // ● public
    public override string ToString()
    {
        return Name;
    }

    public virtual string GetString(string Key, CultureInfo Culture = null)
    {
        return null;
    }
    public virtual object GetObject(string Key, CultureInfo Culture = null)
    {
        return null;
    }
    public virtual byte[] GetBinary(string Key, CultureInfo Culture = null)
    {
        return null;
    }
    public virtual object GetImage(string Key, CultureInfo Culture = null)
    {
        return null;
    }

    // ● properties
    public virtual string Name { get; }
 
}
```

An here is an implementation that uses the [ResourceManager](https://learn.microsoft.com/en-us/dotnet/api/system.resources.resourcemanager) .Net class as its source of localized resources. 

The `ResourceManager` class reads resources from `resx` files.

```
public class ResourceProviderWithResourceManager: ResourceProviderBase
{
    ResourceManager Manager;

    // ● construction
    public ResourceProviderWithResourceManager(ResourceManager Manager, string Name)
        : base(Name)
    {
        if (Manager == null)
            throw new ArgumentNullException("Manager");   

        this.Manager = Manager;  
    }

    // ● public
    public override string GetString(string Key, CultureInfo Culture = null)
    {
        return Manager.GetString(Key, Culture);
    }
    public override object GetObject(string Key, CultureInfo Culture = null)
    {
        return Manager.GetObject(Key, Culture);
    }
    public override byte[] GetBinary(string Key, CultureInfo Culture = null)
    {
        return GetObject(Key, Culture) as byte[];
    }
    public override object GetImage(string Key, CultureInfo Culture = null)
    {
        return GetObject(Key, Culture);
    }
}
```


Create a **static** class to serve as the registry of the `IResourceProvider` resource providers and as the central point of the whole resource system.

```
static public class Res
{
    static List<IResourceProvider> ResourceProviders = new List<IResourceProvider>();


    static Res()
    {
    }

    static public void Add(IResourceProvider Provider)
    {
        if (Provider != null && !ResourceProviders.Contains(Provider) && !string.IsNullOrWhiteSpace(Provider.Name))
        {
            IResourceProvider Item = ResourceProviders.FirstOrDefault(x => string.Compare(x.Name, Provider.Name, StringComparison.OrdinalIgnoreCase) == 0);
            if (Item == null)
                ResourceProviders.Insert(0, Provider);
        }
    }
    static public void Add(ResourceManager Manager, string Name)
    {
        if ((Manager != null) && !string.IsNullOrWhiteSpace(Name))
        {
            Add(new ResourceProviderWithResourceManager(Manager, Name));
        }
    }

    // ● strings
    static public string GetString(string Key, string Default, CultureInfo Culture = null)
    {
        string Result;

        foreach (IResourceProvider RP in ResourceProviders)
        {
            Result = RP.GetString(Key, Culture);
            if (!string.IsNullOrWhiteSpace(Result))
                return Result;
        }

        return Default;
    }
    static public string GetString(string Key, CultureInfo Culture = null)
    {
        return GetString(Key, Key, Culture);
    }        
    static public string L(string Key, string Default, CultureInfo Culture = null)
    {
        return GetString(Key, Default, Culture);
    }
    static public string L(string Key, CultureInfo Culture = null)
    {
        return GetString(Key, Key, Culture);
    }

    // ● non-strings 
    static public object GetObject(string Key, CultureInfo Culture = null)
    {
        object Result;
        foreach (IResourceProvider RP in ResourceProviders)
        {
            Result = RP.GetObject(Key, Culture);
            if (Result != null)
                return Result;
        }

        return null;
    }
    static public byte[] GetBinary(string Key, CultureInfo Culture = null)
    {
        byte[] Data;
        foreach (IResourceProvider RP in ResourceProviders)
        {
            Data = RP.GetBinary(Key, Culture);
            if (Data != null)
                return Data;
        }

        return null;
    }
    static public object GetImage(string Key, CultureInfo Culture = null)
    {
        object Data;
        foreach (IResourceProvider RP in ResourceProviders)
        {
            Data = RP.GetImage(Key, Culture);
            if (Data != null)
                return Data;
        }

        return null;
    }
 
}
```

Any client code in the application has access to this static `Res` class and may ask for a localized resource using a resource key, e.g. `Home.Wellcome`. 

The `Res` class enumerates its registered resource providers and returns the first resource found under the specified resource key.

Any Assembly in the application may use the `Res.Add()` to register one or more `IResourceProvider` instances. 

Another approach in `IResourceProvider` registration would be to use an `Attribute` to mark all resource providers and have the central code to collect these classes.

### View Localization

A View derived class is always useful to have. The base class is the Asp.Net Core's [RazorPage](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.razor.razorpage).

```
public abstract class ViewBase<TModel> : RazorPage<TModel>
{
    public HtmlString L(string Key, params object[] Args)
    {
        string S = Res.GetString(Key);  
        if ((Args != null) && (Args.Length > 0))
            S = string.Format(S, Args);
        return new HtmlString(S);
    }  

    // other code here
}
```

A custom View class should be declared in the `_ViewImports.cshtml` file.

```
@inherits ViewBase<TModel>
```

This view class provides the `L()` method, stands for `Localize()`, which then is used in `*.cshtml` view files to ask for localized strings.

```
<span class="...">@L("Login")</span>
```

The application may also provide base controller classes containing the exact same `L()` method.

```
var ErrorMessage = L("LoginFailed");
```














 