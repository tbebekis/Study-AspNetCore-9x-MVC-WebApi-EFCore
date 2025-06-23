# Session

> This text is part of a group of texts describing [Asp.Net Core](../Index.md).

[HTTP](https://en.wikipedia.org/wiki/HTTP) is a [stateless protocol](https://en.wikipedia.org/wiki/Stateless_protocol).

Asp.Net Core provides [a number of ways](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state) for application state management.

One such a way is [Session state](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state#session-state).

## The ISession interface
In Asp.Net Core session services are provided through the [HttpContext.Session](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext.session) property of type [ISession](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.isession).

## Session is stored in a cookie
Asp.Net Core uses a [Cookie](https://en.wikipedia.org/wiki/HTTP_cookie) for storing session state to the client, i.e. the browser. That cookie contains a session Id which identifies the session.

A session cookie is deleted when the browser session ends by the user.

A session has a default timeout of 20 minutes. The developer may define a more suitable timeout value. 

Session is mostly used in storing data specific to the user, such as some user preferences, that are not required to be permanent accross sessions.

Since session data are stored in a cookie there is a limit in the cookie size, which is less than 4096 bytes.

## Session configuration

```
var builder = WebApplication.CreateBuilder(args);

// some services here
// ...

// required
// see https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state#configure-session-state
builder.Services.AddDistributedMemoryCache();

// session
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "MyApp.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.Minutes(30);    
});

var app = builder.Build();

// some middlewares here
// ...

app.UseSession();

app.MapControllerRoute(...);

app.Run();
```

The order of session middleware is important. Is should be after `UseRouting()` and before `MapControllerRoute()` or `MapRazorPages()`.

## Read and Write to Session

The [Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller) and [PageModel](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.razorpages.pagemodel) classes provide access to session data through the [HttpContext.Session](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext.session) property of type [ISession](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.isession).

But sometimes a need arises to access session data outside of a `Controller` or `PageModel` context.

It is easy to write up a class that provides session services to any code in an application.

```

static public class Session
{
    static IHttpContextAccessor HttpContextAccessor;
    static public HttpContext GetHttpContext() => HttpContextAccessor.HttpContext;

    // ● private
    static T Get<T>(this ISession Instance, string Key)
    {
        Key = Key.ToLowerInvariant();
        string JsonText = Instance.GetString(Key);
        if (JsonText == null)
            return default(T);

        return JsonSerializer.Deserialize<T>(JsonText);
    }
    static T Get<T>(this ISession Instance, string Key, T Default)
    {
        Key = Key.ToLowerInvariant();
        string JsonText = Instance.GetString(Key);
        if (JsonText == null)
            return Default;

        return JsonSerializer.Deserialize<T>(JsonText);
    }
    static void Set<T>(this ISession Instance, string Key, T Value)
    {
        Key = Key.ToLowerInvariant();
        string JsonText = JsonSerializer.Serialize(Value);
        Instance.SetString(Key, JsonText);
    }

    // ● public
    static public void Initialize(IHttpContextAccessor HttpContextAccessor)
    {
        Session.HttpContextAccessor = HttpContextAccessor;
    }

    static public ISession GetSession()
    {
        return GetHttpContext().Session;
    }

    static public T Get<T>(string Key)
    {
        ISession Session = GetSession();
        return Session != null ? Session.Get<T>(Key) : default(T);
    }
    static public T Get<T>(string Key, T Default)
    {
        ISession Session = GetSession();
        return Session != null ? Session.Get<T>(Key, Default) : default(T);
    }
    static public void Set<T>(string Key, T Value)
    {
        ISession Session = GetSession();
        if (Session != null)
            Session.Set(Key, Value);
    }

    static public T Pop<T>(string Key)
    {
        T Result = Get<T>(Key);
        Remove(Key);
        return Result;
    }

    static public string GetString(string Key)
    {
        return Get<string>(Key, null);
    }
    static public void SetString(string Key, string Value)
    {
        Set(Key, Value);
    }

    static public void Clear()
    {
        ISession Session = GetSession();
        if (Session != null)
            Session.Clear();
    }
    static public void Remove(string Key)
    {
        ISession Session = GetSession();
        if (Session != null)
        {
            Key = Key.ToLowerInvariant();
            Session.Remove(Key);
        }
    }
    static public bool ContainsKey(string Key)
    {
        ISession Session = GetSession();
        if (Session != null)
        {
            Key = Key.ToLowerInvariant();
            byte[] Buffer = null;
            return Session.TryGetValue(Key, out Buffer);
        }

        return false;
    }


    // ● properties
    static public IDictionary<object, object> Request
    {
        get
        {
            HttpContext HttpContext = GetHttpContext();
            return HttpContext != null ? HttpContext.Items : new Dictionary<object, object>();
        }
    }

}
```

Some notes on the above class.

- a static class is available to any code in an application without the need to use Dependency Injection.
- the above static class requires an [IHttpContextAccessor](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.ihttpcontextaccessor) to be passed in its `Initialize()` method.
- using this static `Session` class the developer may store any object in the session, since the data is serialized to `json`. Avoid saving large data amounts to session, since the storage medium is a cookie with a limited size.
- the `Request` property of this static `Session` class provides access to `HttpContext.Items` property. That property is a generic dictionary and it is used in storing data while processing a single request. The lifetime of that dictionary is the lifetime of the HTTP request. The dictionary's contents are discarded after the request is processed. 
