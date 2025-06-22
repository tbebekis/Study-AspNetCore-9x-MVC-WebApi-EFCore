# Authorization

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

[**Authorization**](https://en.wikipedia.org/wiki/Authorization) is a process that decides if a user has the required permissions in order to access a certain protected resource provided by an application. 

## Authorization types

Regarding authorization Asp.Net Core provides a number of options.

- [Simple Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/simple).
- [Policy-based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies).
- [Role-based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles).
- [Claims-based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/claims).
- [Scheme-based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme).
- [Resource-based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased).
- [Custom Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/iard). 

## Simple Authorization

The [AuthorizeAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.authorizeattribute) attribute controls authorization.

When placed on a controller then all controller actions require an authenticated request.

```
[Authorize]
public class MyController : Controller
{
    public ActionResult Action1() 
    {
    }

    public ActionResult Action2()
    {
    }
}
```

All the actions of the above controller, and its descendants, require authenticated requests.

The `AuthorizeAttribute` can be placed on an `Action` too.

```
public class MyController : Controller
{
    public ActionResult Action1() 
    {
    }

    [Authorize]
    public ActionResult Action2()
    {
    }
}
```

Only the `Action2` of the above controller require authenticated requests.

The [AllowAnonymousAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.allowanonymousattribute) attribute allows access to an `Action` of a protected controller, by a non-authenticated request.

```
[Authorize]
public class MyController : Controller
{
    public ActionResult Action1() 
    {
    }

    [AllowAnonymous]
    public ActionResult Action2()
    {
    }
}
```

All the actions of the above controller, and its descendants, require authenticated requests, except of the `Action2`.

The `AuthorizeAttribute` provides the following properties.

- `AuthenticationSchemes`. A comma-delimited list of strings of the authentication schemes that required in order to gain access. They are applied using an **or** logic.
- `Policy`. A string with the name of the policy that required in order to gain access.
- `Roles`. A comma-delimited list of strings of the roles that required in order to gain access. They are applied using an **or** logic.

The `Simple Authentication` does not uses these properties.

## Policy-based Authorization.

In Asp.Net Core all authorization methods end up to be implemented as policies.

A `Policy` is a code entity which represents **a collection of requirements**. For a policy evaluation to succeed all of its requirements must meet.

A `Requirement` is a code entity which represents a condition that the current `Identity`, i.e. user, must meet, in order to access a protected resource.

Common requirements are
- the `Identity` contains a claim of a specified type
- the `Identity` contains a claim of a specified type having a specified value
- the `Identity` contains a role claim
- the `Identity` contains a role claim having as value a particular role

Besides the above nothing prohibits a requirement from specifying a condition that does not involve claims. That is a requirement may specify a condition that comes from any other source and not from a claim.


#### Applying a Policy

A policy is applied using the `AuthorizeAttribute`.

```
[Authorize(Policy = "Policy1")]
public class MyController : Controller
{
    public ActionResult Action1() 
    {
    }

    [Authorize(Policy = "Policy2")]
    public ActionResult Action2() 
    {
    }

    [Authorize(Policy = "Policy3")]
    [Authorize(Policy = "Policy4")]
    public ActionResult Action3() 
    {
    }        
}
```
When multiple policies are applied to a controller or action, then all of them are required to be satisfied. Multiple policies work like boolean conditions with the **and** operator.

In the above code

- `Action1` requires `Policy1`
- `Action2` requires `Policy1` and `Policy2`
- `Action3` requires `Policy1`, `Policy3` and `Policy4`


Policies can also be used in [Minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview) end points.

```
app.MapGet("/get-product", () => { ...})
    .RequireAuthorization("Product.View");
```

#### Creating a Policy

A policy can be created using the [AuthorizationPolicy](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.authorizationpolicy) class. This is a totally valid but not a common way.

In the following example a policy created using the `AuthorizationPolicy` class requires users to be authenticated. 

```
List<IAuthorizationRequirement> Requirements = new() { new DenyAnonymousAuthorizationRequirement() };
List<string> RequiredAuthenticationSchemes = new();

var MyPolicy = new AuthorizationPolicy(Requirements, RequiredAuthenticationSchemes);
```

A common way in creating a policy is to use the [AuthorizationPolicyBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.authorizationpolicybuilder
) class.

The `AuthorizationPolicyBuilder` class provides properties and methods allowing to easily add requirements regarding authentication schemes, claims, roles and even use assertion functions of the form `Func<AuthorizationHandlerContext, bool>`.

```
AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .RequireRole("Admininstrator")
    .RequireClaim("Department", "IT")  
    .RequireAssertion(ctx =>
    {
        return ctx.User.HasClaim("Specialty", "SystemEngineer");
    })
    .Build();
```

The `Build()` method returns an `AuthorizationPolicy` instance.

#### Registering a Policy

A policy must be registered with the authorization middleware **under a unique name**.

In the following example 
- the `options` parameter is an instance of the [AuthorizationOptions](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.authorizationoptions) class, while
-  the `policy` parameter is an instance of the `AuthorizationPolicyBuilder` class.

The following example illustrates the most common way in creating a policy and registering it at the same time.

```
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("POLICY_NAME", policy => { 
        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .RequireRole("Admininstrator")
        .RequireClaim("Department", "IT")  
        .RequireAssertion(ctx =>
        {
            return ctx.User.HasClaim("Specialty", "SystemEngineer");
        })
    });
});
```

#### Checking a Policy using code

It is possible to check programmatically if a policy is met using the [IAuthorizationService.AuthorizeAsync(ClaimsPrincipal, Object, String)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.iauthorizationservice.authorizeasync) method.

That method gets the following parameters.

- `ClaimsPrincipal user`. The `Identity` to check.
- `object? resource`. Optional. A resouce that may required in policy evaluation.
- `string policyName`. The policy name.


A policy check can be done in a controller.

```
public class MyController : Controller
{
    IAuthorizationService AuthService;
    public MyController(IAuthorizationService authorizationService)
    {
        AuthService = authorizationService;
    }

    public async Task<IActionResult> MyAction1() 
    {
        AuthorizationResult AuthResult = await AuthService.AuthorizeAsync(User, null, "MyPolicy");
        if (!AuthResult.Succeeded)
            return new ForbiddenResult();

        // ...
    }  
}
```
A policy check can be done in a view.

```
@inject IAuthorizationService AuthService
@{
    AuthorizationResult AuthResult = await AuthService.AuthorizeAsync(User, null, "MyPolicy");
    if (!AuthResult.Succeeded)
    {
        //...
    }
}
<div>
   <!--  
   ...
   -->
</div>
```

The above example requires a `using` in the `_ViewImports.cshtml` as 

```
@using Microsoft.AspNetCore.Authorization
```

#### Custom Policy Requirements

A policy requirement is comprised of two components.

- a requirement class, which is just a plain class, implementing the [IAuthorizationRequirement](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.iauthorizationrequirement), containing some data used when evaluating the requirement
- an [AuthorizationHandler](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.authorizationhandler-1) authorization handler that evaluates that requirement data against the current `Identity` 

A custom requirement class is an [IAuthorizationRequirement](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.iauthorizationrequirement) interface  implementor. The truth is that regarding the `IAuthorizationRequirement` there is not much to implement as it merely acts as just a marker. 

```
public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string Permission)
    {
        this.Permission = Permission;
    }

    public string Permission { get; }
}
```

The following example illustrates the registration of a policy with the requirement implemented above.

```
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Product.Create", policy => { policy.Requirements.Add(new PermissionRequirement("Product.Create")); });
});
```

A requirement class has to be associated with an `AuthorizationHandler`. The handler provides the `HandleRequirementAsync()` overridable method where the requirement evaluation takes place. 

The `context` parameter in the `HandleRequirementAsync()` method is an [AuthorizationHandlerContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.authorizationhandlercontext) instance.

In the following example the `DataStore.GetUserPermissions(Id)` returns a list of permissions, from a database table, associated with an `Identity`, i.e. user or application.

```
public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var User = context.User;
        bool IsAuthenticated = User != null && User.Identity != null ? User.Identity.IsAuthenticated : false;

        if (IsAuthenticated)
        {
            var IdClaim = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
            if (IdClaim != null)
            {
                string UserId = IdClaim.Value;
                List<string> Permissions = DataStore.GetUserPermissions(UserId);

                foreach (var Permission in Permissions)
                {
                    if (string.Compare(Permission, requirement.Permission, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        context.Succeed(requirement);
                        break;
                    }
                } 
            }
        }

        return Task.CompletedTask;
    }
}
```

When a controller action is called then all applied policies are evaluated which is that all associated authorization handlers are called.

By default all authorization handlers will be called even if a previous policy fails. This allows authorization handlers to execute side effects, such as logging.

To change that long-circuit behavior to a short-circuit one the `InvokeHandlersAfterFailure` should be set to `false`.

```
builder.Services.AddAuthorization(options =>
{
    options.InvokeHandlersAfterFailure = false;

    // ...
});
```

The `AuthorizationHandler` must be registered with the Dependency Injection container.
 
```
services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
```


#### Policy considerations

- There is no  guarantee that the policy evaluation order is that of the policy registration order.
- There can be [multiple](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?#why-would-i-want-multiple-handlers-for-a-requirement) `AuthorizationHandler`s for a single requirement.
- There can be a single `AuthorizationHandler` for [multiple](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?#use-a-handler-for-multiple-requirements) requirements.
- the `AuthorizationOptions.DefaultPolicy` property defines what policy should be evaluated when the `AuthorizeAttribute` is used without specifying a policy. By default requires authenticated users.
- the `AuthorizationOptions.FallbackPolicy` property defines what policy should be evaluated when no policies are specified for a resource. Defaults to `null`. 
- When a policy is applied to a controller action it seems that its associated authorization handler will be called even if the request, i.e. the user, is **not** authenticated. The `policy.RequireAuthenticatedUser()` when registering a policy ensures that the policy will fail if the user is **not** authenticated. Adding the `AuthorizeAttribute` to a controller class or using the `RequireAuthorization()` with a Minimal API end point seems that it has the same effect.


## Role-based Authorization.

Role-based Authorization in Asp.Net Core is very simple. It just requires the existence of a claim type in the authenticated request, before access is granted.

An `Identity`, i.e. a user, may have one or more roles, e.g. `Administrator`, `User`, `Guest`, etc.

In the Asp.Net Core it is very easy to enforce authorization when a claim of the `ClaimTypes.Role` type exists in the claim list of an identity.

```
[Authorize(Roles = "Administrator")]
public class MyController : Controller
{
    public IActionResult Action1()
    {        
    }
}
```

Multiple roles may specified

- in a single `AuthorizeAttribute` attribute as a comma separated list. In this case roles are used as conditions defined with the **or** operator.
- using multiple `AuthorizeAttribute` attributes. In this case roles are used as conditions defined with the **and** operator.

```

[Authorize(Roles = "Administrator, Manager")]
public class MyController : Controller
{
    public IActionResult Action1()
    {        
    } 

    [Authorize(Roles = "Administrator, CEO")]
    public IActionResult Action2()
    {        
    }

    [Authorize(Roles = "Administrator")]
    [Authorize(Roles = "Manager")]    
    public IActionResult Action3()
    {        
    }     
}
```
In the above code

- the `Action1` can be accessed by an identity having any of the `Administrator` **or** `Manager` roles
- the `Action2` can be accessed by an identity having any of the `Administrator` **or** `Manager` **or** `CEO` roles
- the `Action3` can be accessed by an identity having both the `Administrator` **and** `Manager` roles

A policy can be used to enforce Role-base authorization using the `policy.RequireRole()` when registering a policy. The `RequireRole()` accepts a number of roles and enforces them with the **or** operator.

```
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Policy1", policy =>
          policy.RequireRole("Role1", "Role2", "Role3"));
});
```

#### Role-based access control (RBAC)

[Role-based access control (RBAC)](https://en.wikipedia.org/wiki/Role-based_access_control) or `Role-based access security` is a widely used method in allowing access to protected resources to authorized requestors only, based on Roles **and** Permissions.

Asp.Net Core's `Role-based` authorization is **not** `RBAC`. 

It is possible to apply `RBAC` authorization in Asp.Net Core using `Policy-based` authorization.

Generally an `RBAC` system involves the following.

- `Permission`. A code entity with a name, such as `Product.Create`, `Product.Edit`, `Product.Delete`, etc. denoting what an identity can do. 
- `Role`. A code entity with a name, such as `Administrator`, `Manager`.
- `RolePermissions`. A list of permissions assigned to a particular role.
- `User`. An identity.
- `UserRoles`. A list of roles assigned to a perticular identity.

Permissions are not assigned directly to users. A user aquires permissions indirectlry via the roles he is a member of.

> The term `Scope`, used in [OAuth](https://en.wikipedia.org/wiki/OAuth), defines a specific action an application is allowed to do on behalf of a user. A `Scope` is actually a `Permission`


## Claims-based Authorization

Claims authorization is policy based. The application has to register a policy expressing the claims requirements.

```
var builder = WebApplication.CreateBuilder(args);

// some services here
// ... 

// ● Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrator", policy => policy.RequireClaim(ClaimTypes.Role, "Administrator"));

    options.AddPolicy("Manager", policy => policy.RequireClaim(ClaimTypes.Role, "Manager"));
});

//...

var app = builder.Build();

// some middlewares here
// ...

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(...);

app.Run();
```

Although the above example uses the `ClaimTypes.Role` claim type, claims-based authorization works with any claim type, as long as a policy is registered expressing a requirement about a claim.

Here an overload of the `RequireClaim(string claimType, params string[] allowedValues)` method is used which accepts both the claim type and its possible values. In this overload both the claim type and its possible values is the requirement.

There is another overload as `RequireClaim(string claimType)` where only the existence of a specified claim type is the requirement. 

Here is how to authorize access based on claims enforced by policies.

When multiple policies are applied to a controller or action, then all of them are required to be satisfied. Multiple policies work like boolean conditions with the **and** operator.

```
[Authorize(Policy = "Administrator")]
[Authorize(Policy = "Manager")]
public class AllController : Controller
{
    //...
}

[Authorize(Policy = "Administrator")]
public class AdminOnlyController : Controller
{
    //...
} 
```

## Scheme-based Authorization

There are cases where an application has to use multiple authentication methods, i.e. [Authentication Schemes](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/#authentication-scheme).

An application may need
- a cookie-base authentication with a basic identity
- a cookie-base authentication with [Multi-Factor Authentication](https://en.wikipedia.org/wiki/Multi-factor_authentication)
- a [JWT bearer authentication](https://en.wikipedia.org/wiki/JSON_Web_Token).

Consider the following.

```
var builder = WebApplication.CreateBuilder(args);

// some services here
// ...

// ● Authentication
AuthenticationBuilder AuthBuilder = services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = options.DefaultScheme;
    options.DefaultChallengeScheme = options.DefaultScheme;
});

// ● Cookie authentication
AuthBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
    // ...
});

// ● JWT authentication
AuthBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
    // ...
});

// ...

var app = builder.Build();

// some middlewares here
// ...

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(...);

app.Run();
```

Every authentication scheme added to authentication service has its own discrete name.
 
The above code adds two authentication schemes under specific names.

- [CookieAuthenticationDefaults.AuthenticationScheme](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.cookies.cookieauthenticationdefaults)
- [JwtBearerDefaults.AuthenticationScheme](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer.jwtbearerdefaults)

There are two ways to enforce authorization using an `Authentication Scheme`.
- using the `AuthorizeAttribute`
- using a `Policy`

#### Enforce Scheme-based Authorization using the AuthorizeAttribute attribute

Here is how to authorize access based on an authentication scheme.

```
[Authorize(AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme}, {JwtBearerDefaults.AuthenticationScheme}")]
public class AllSchemesController : Controller
{
    //...
}

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class CookieOnlyController : Controller
{
    //...
}

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class JwtOnlyController : Controller
{
    //...
}
```
#### Enforce Scheme-based Authorization using Policies 

Policies may added using the `AddAuthorization()`.

```
var builder = WebApplication.CreateBuilder(args);

// some services here
// ...

// ● Authentication
AuthenticationBuilder AuthBuilder = services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = options.DefaultScheme;
    options.DefaultChallengeScheme = options.DefaultScheme;
});

// ● Cookie authentication
AuthBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
    // ...
});

// ● JWT authentication
AuthBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
    // ...
});

// ● Authorization
builder.Services.AddAuthorization(options =>
{
    var DefaultAuthPolicyBuilder = new AuthorizationPolicyBuilder(
        CookieAuthenticationDefaults.AuthenticationScheme,
        JwtBearerDefaults.AuthenticationScheme);

    DefaultAuthPolicyBuilder = DefaultAuthPolicyBuilder.RequireAuthenticatedUser();

    options.DefaultPolicy = DefaultAuthPolicyBuilder.Build();

    var CookieOnlyPolicyBuilder = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme);
    options.AddPolicy("CookieOnlyPolicy", CookieOnlyPolicyBuilder
        .RequireAuthenticatedUser()
        .Build());    

    var JwtOnlyPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
    options.AddPolicy("JwtOnlyPolicy", JwtOnlyPolicyBuilder
        .RequireAuthenticatedUser()
        .Build());
});

//...

var app = builder.Build();

// some middlewares here
// ...

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(...);

app.Run();
```

Here is how to authorize access based on authentication schemes enforced by policies.

```
[Authorize(Policy = "CookieOnlyPolicy")]
public class CookieOnlyController : Controller
{
    //...
}

[Authorize(Policy = "JwtOnlyPolicy")]
public class JwtOnlyController : Controller
{
    //...
}
```


## Resource-based Authorization

Resource-based authorization depends in both, the requestor identity **and** the resource.

This means that the resource must first be retrieved from the data store and then the authorization evaluation must be applied.

Resource-based authorization can not be applied using `declarative` authorization, i.e. the `AuthorizeAttribute`.

Instead it is applied using `imperative` authorization, i.e. using an authorization handler.

The following examples use a `BlogPost` entity, as resource, and its `AuthorIdList` property in the evaluation process.

```
public class BlogPost
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }

    // ...

    public List<string> AuthorIdList { get; set; }
}
```

A requirement is needed, which is just an empty class.

```
public class BlogPostAuthorRequirement : IAuthorizationRequirement 
{     
}
```

A policy with that requirement should be registered.

```
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BlogPostAuthorPolicy", policy => policy.Requirements.Add(new BlogPostAuthorRequirement()));
});
```

The evaluation is performed by a custom authorization handler. In this handler the second overload of the `HandleRequirementAsync()` method should be used, the one that accepts a resource too.

```
public class BlogPostAuthorAuthorizationHandler : AuthorizationHandler<BlogPostAuthorRequirement, BlogPost>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BlogPostAuthorRequirement requirement,
        BlogPost resource)
    {

        var User = context.User;
        bool IsAuthenticated = User != null && User.Identity != null ? User.Identity.IsAuthenticated : false;

        if (IsAuthenticated)
        {
            var IdClaim = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
            if (IdClaim != null)
            {
                string UserId = IdClaim.Value;

                foreach (string AuthorId in resource.AuthorIdList)
                {
                    if (string.Compare(UserId, AuthorId, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        context.Succeed(requirement);
                        break;
                    }
                }  
            }
        }

        return Task.CompletedTask; 
    }
}
```

The authorization handler has to be registered too.

```
builder.Services.AddTransient<IAuthorizationHandler, BlogPostAuthorAuthorizationHandler>();
```

The controller action method that returns the resource for editing has to use the `IAuthorizationService` and call its `AuthorizeAsync()` passing the resource too. That way the authorization handler is called and the evaluation takes place.

```
public class BlogController : Controller
{
    IAuthorizationService AuthService;

    public BlogController(IAuthorizationService authorizationService)
    {
        AuthService = authorizationService;
    }

    [HttpGet("/blog/update/{blogpostid}", Name = "UpdateBlogPost")]
    public async Task<IActionResult> UpdateBlogPost(string BlogPostId) 
    {
        BlogPost Entity =  DataStore.GetBlogPost(BlogPostId);

        AuthorizationResult AuthResult = await AuthService.AuthorizeAsync(User, Entity, "BlogPostAuthorPolicy");
        if (!AuthResult.Succeeded)
            return new ForbiddenResult();

        // ...

        return View(Entity);
    }  
}
```


## Custom Authorization

In Asp.Net Core is possible to apply [Custom Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/iard) using an `AuthorizeAttribute` derived class.

 That custom `AuthorizeAttribute` derived class
 
 - is also a requirement, because it implements the `IAuthorizationRequirement` interface
 - and should also implement the [IAuthorizationRequirementData](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.iauthorizationrequirementdata) interface which returns the requirement.

```
public class PermissionAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthorizationRequirementData
{
    public PermissionAttribute(string permissionName)
    {
        PermissionName = permissionName;
    }

    public string PermissionName { get; set; }

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}
```

An authorization handler is required.

```
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAttribute>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAttribute requirement)
    {
        var User = context.User;
        bool IsAuthenticated = User != null && User.Identity != null ? User.Identity.IsAuthenticated : false;

        if (IsAuthenticated)
        {
            var IdClaim = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
            if (IdClaim != null)
            {
                string UserId = IdClaim.Value;
                List<AppPermission> Permissions = DataStore.GetUserPermissions(UserId);

                foreach (var Permission in Permissions)
                {
                    if (string.Compare(Permission.Name, requirement.PermissionName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        context.Succeed(requirement);
                        break;
                    }
                }
            }
        }

        return Task.CompletedTask;
    }
}
```

Handler registration is required.

```
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
```

The custom authorization attribute can be used everywhere an `AuthorizeAttribute` can be used.

```
 
public class MyController : Controller
{
    [Permission("SomePermission")]
    public ActionResult Action1() 
    {
    }
}
```

> Using Asp.Net Core `Custom Authorization` authorization makes it easy to implement [RBAC](https://en.wikipedia.org/wiki/Role-based_access_control).
> The above code example may serve as a starting point.
>
> The `GetUserPermissions(UserId)` would be get a list of User Permissions from a database having the required `RBAC` tables, as noted earlier in this text.
























