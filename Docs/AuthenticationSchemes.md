# Authentication Schemes
> This text is part of a group of texts describing [Asp.Net Core](Index.md).

An `Authentication Scheme` is a specific way of authenticating users and client applications. 

An `Authentication Scheme` consists of the following:

- an authenction scheme name, which is just a string
- an authentication handler, which should inherit from [AuthenticationHandler](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationhandler-1) class or implement the [IAuthenticationHandler](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.iauthenticationhandler) interface
- an authentication options class, which should inherit from [AuthenticationSchemeOptions](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationschemeoptions) class.

> If `Cookie` or `JWT` authentication is all an application needs then there is no need to implement the three above elements. Asp.Net Core provides everything for authentication types like these.

An Asp.Net Core application should have set the application's **default** authentication scheme. All the `AddAuthentication()` method does is just this, it adds a scheme name as the application's **default** authentication scheme.

> **NOTE**: always set a default authentication scheme, especially if working with Asp.Net Core 6 and earlier.

An application may implement **more than one** authentication types, each one with its own authentication scheme.

```
AuthenticationBuilder AuthBuilder = builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

AuthBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
    // ...
});

AuthBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
    // ...
});
```

### Custom Authentication Scheme

A Custom Authentication Scheme has to fullfil the three requirements of an authentication scheme as described above:

- an authentication scheme name
- an authentication handler class
- an authentication options class.