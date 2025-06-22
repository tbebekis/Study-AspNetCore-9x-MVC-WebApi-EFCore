# Claims and Identity

> This text is part of a group of texts describing [Asp.Net Core](Index.md).
 
[**Identity**](https://en.wikipedia.org/wiki/Digital_identity) is a software entity comprised of information about a person, an application, a device, etc. It is a collection of attributes and characteristics that identify an entity. 

[**Principal**](https://en.wikipedia.org/wiki/Principal_(computer_security)) is a software entity that can be authenticated by an application, computer or network and it represents the identity of a user, application, computer, etc.

[**Identity Provider**](https://en.wikipedia.org/wiki/Identity_provider) is a software component that issues, maintains and manages identity information for `Principals`.

[**Identity Information**](https://en.wikipedia.org/wiki/Identity_and_access_management) is information, i.e. data, that identifies an entity. That entity could be a person, an application, a device, etc. 

[**Authentication**](https://en.wikipedia.org/wiki/Authentication) is a process that verifies the identity of a user, application, etc.

[**Authorization**](https://en.wikipedia.org/wiki/Authorization) is a process that decides if a user has the required permissions in order to access a certain resource provided by an application.  



## .Net Core Identity

As `Identity Information`, the .Net Core and Asp.Net Core, uses [Claims-based Identity](https://en.wikipedia.org/wiki/Claims-based_identity). The relevant .Net Core classes are part of the [System.Security.Claims](https://learn.microsoft.com/en-us/dotnet/api/system.security.claims) namespace.

- [ClaimsPrincipal](https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal) class. Implements the [IPrincipal](https://learn.microsoft.com/en-us/dotnet/api/system.security.principal.iprincipal) interface. 
- [ClaimsIdentity](https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimsidentity) class. Implements the [IIdentity](https://learn.microsoft.com/en-us/dotnet/api/system.security.principal.iidentity) interface. 
- [Claim](https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claim) class.

> In Asp.Net Core the [HttpContext.User](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext.user) property is a `ClaimsPrincipal`.

## Claim class
The `Claim` class provides the `Type` and the `Value` properties both of the string type. 

A `Claim` is actually a `Key-Value` pair. The `Claim.Type` property is the `Key` where the `Claim.Value` property is the `Value`.

The `Type` property contains the *name* of the claim, such as `Role`, where a possible `Value` could be `Administrator`.  

A claim is a statement that an entity, such as a user or an application, makes about itself. Claims are used in both `Authentication` and `Authorization`.

A claim may store the Id, Name, Email, Role, etc. of a user, but **never a secret** such as a password.

## ClaimsIdentity class

The `ClaimsIdentity` class is a **Claims-based Identity** and provides a collection of claims.

The `ClaimsIdentity.Claims` property is where these claims are kept.

One or more `Claim` instances may be assigned to a `ClaimsIdentity`. 

## ClaimsPrincipal class

The `ClaimsPrincipal` class is an authentication and authorization entity.

The `ClaimsPrincipal` class is actually a collection of `ClaimsIdentity` identities. One or more `ClaimsIdentity` instances may be assigned to a `ClaimsPrincipal`. The `ClaimsPrincipal.Identities` property is where these identities are kept. 

One of these identities is considered the **primary Identity** and is accessible through the `ClaimsPrincipal.Identity` property.

The `ClaimsPrincipal` class also provides a collection of all the claims of all of its Identities. The `ClaimsPrincipal.Claims` property is a consolidation of all the claims of all the identities of a principal.

## The process

- a requestor user or a service wants to access an application
- the application presents a login UI to a user or an authentication [End Point](https://en.wikipedia.org/wiki/Web_API#Endpoints) to a service
- the requestor user/service provides authentication credentials
- the application goes on to verify the provided credentials. That verification maybe performed by the application itself or an external `Identity Provider`
- on success the application permits the requestor to enter and prepares a collection of claims about the requestor. These claims may come from the application itself or from an external provider
- after entering the requestor wants to access an application resource, such as a list of Invoices. The application decides if the requestor is authorized to access the resource based on its claims.

## A claims example

A `using System.Security.Claims;` directive is required when working with claims.

Consider the following `Main()` method of a console application.

```
internal class Program
{
    static void Main(string[] args)
    {
        // ● create a claims list
        List<Claim> ClaimList = new List<Claim>();

        ClaimList.Add(new Claim(ClaimTypes.NameIdentifier, "E7807E26-E5D2-4698-8E96-3F0C5B9BBBFE"));
        ClaimList.Add(new Claim(ClaimTypes.Name, "John Doe"));
        ClaimList.Add(new Claim(ClaimTypes.Email, "jdoe@company.com"));
        ClaimList.Add(new Claim(ClaimTypes.Role, "Admin"));

        // ● private claims
        ClaimList.Add(new Claim("FavoriteColor", "Blue"));

        // ● create the identity
        string AuthenticationType = "MyAuthType";
        ClaimsIdentity Identity = new ClaimsIdentity(ClaimList, AuthenticationType);   

        Console.WriteLine($"IsAuthenticated: {Identity.IsAuthenticated}");

        // ● create the principal
        ClaimsPrincipal Principal = new ClaimsPrincipal(Identity);

        // ● make it the current principal in the current thread
        Thread.CurrentPrincipal = Principal;

        Console.WriteLine($"Thread.CurrentPrincipal.Identity.Name: {Thread.CurrentPrincipal.Identity.Name}");
        Console.ReadLine();
    }
}
```
The code first creates a claim list and adds claims to it.

The [ClaimTypes](https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimtypes) is a .Net built-in class which defines constants for the *well-known* claim `Types`. 

> In `JWT` token authentication the [JwtRegisteredClaimNames](https://learn.microsoft.com/en-us/dotnet/api/system.identitymodel.tokens.jwt.jwtregisteredclaimnames) is used instead of `ClaimTypes`.

Besides these *well-known* claim types an application is free to use its own claim types, as is the `FavoriteColor` in the above example.

The `ClaimsIdentity` class provides a number of constructors. Some of these constructors accept a string parameter under the name `authenticationType`.

```
public ClaimsIdentity(IEnumerable<Claim>? claims, string? authenticationType)
```

If an `authenticationType` is passed when a `ClaimsIdentity` is constructed then the `ClaimsIdentity.IsAuthenticated` property returns `true`. 

> In Asp.Net Core passing an `authenticationType` is not enough for authentication. Also in Asp.Net Core an [Authentication Scheme](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/#authentication-scheme) is passed to these constructors where an `authenticationType` parameter is expected.

The list of claims is passed to the `ClaimsIdentity` constructor. Also there are constructors where the application may define what is the `Name` and the `Role` claims.

```
string AuthenticationType = "MyAuthType";
ClaimsIdentity Identity = new ClaimsIdentity(ClaimList, AuthenticationType, ClaimTypes.Email, ClaimTypes.Role);  
```

Following is the defaults as they defined by the `ClaimsIdentity` class.

```
public const string DefaultNameClaimType = ClaimTypes.Name;
public const string DefaultRoleClaimType = ClaimTypes.Role;
```

The `ClaimsPrincipal` class implements the `IPrincipal` interface. Because of that a `ClaimsPrincipal` instance may assigned to `Thread.CurrentPrincipal` property.

```
Thread.CurrentPrincipal = Principal;
```

After that the Principal is available on the current thread.

```
Console.WriteLine($"Thread.CurrentPrincipal.Identity.Name: {Thread.CurrentPrincipal.Identity.Name}");
```

The `Identity.Name` in the above, returns the value of the claim that is defined as the name claim.

## Useful method and properties

### ClaimsPrincipal
- `ClaimsPrincipal.Current`.
- `ClaimsPrincipal.AddIdentity()`.
- `ClaimsPrincipal.IsInRole()`. 
- `ClaimsPrincipal.HasClaim()`. 
- `ClaimsPrincipal.FindFirst()`. 
- `ClaimsPrincipal.FindAll()`. 

### ClaimsIdentity
- `ClaimsIdentity.IsAuthenticated`.
- `ClaimsIdentity.AuthenticationType`.
- `ClaimsIdentity.AddClaim()`.
- `ClaimsIdentity.AddClaims()`.
- `ClaimsIdentity.RemoveClaim()`.
- `ClaimsIdentity.HasClaim()`. 
- `ClaimsIdentity.FindFirst()`. 
- `ClaimsIdentity.FindAll()`. 

 


