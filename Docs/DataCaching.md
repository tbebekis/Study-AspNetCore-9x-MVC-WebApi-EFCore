# Data Caching

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

[Data Caching](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview) is a term that describes a process in which requested data is returned by retrieving it from a temporary storage location known as `cache`, e.g. in memory, where it is stored under a unique string key. 

If that key is not present in the cache then data is retrieved from a persistent storage location, e.g. a relational database, stored in the cache, under that unique string key, for a limited period of time, and returned to the caller. 

## Terminology

- **Cache Store**. The medium used as cache. Could be the memory or a table in a relational database. It is actually a dictionary of `Key-Value` pairs, where a `Key` is a unique string and `Value` is the data.
- **Cache Entry**. An entry stored in the cache which is identified by its unique `Key` and has a `Value` which is the data. An entry could be optionally associated with information which specifies the time of its eviction from cache, i.e. its expiration.
- **Cache Key**. A string that uniquelly identifies a cache entry.
- **Absolute Expiration Time**. Indicates that the cache entry will be removed at an explicit date and time.
- **Sliding Expiration Time**. Indicates that the cache entry will be removed if is remains idle (not accessed) for a certain amount of time.

## Procedure

- a layer of an application requests data
- the data layer constructs a unique key for a cache entry
- checks if the cache contains data under that key
- if there is no data in cache under that key, it retrieves data from the persistent data store, i.e. a database, adds the data in cache under that key, and returns the data
- if there is data in cache under that key it gets and returns the data to the caller.

## Asp.Net Core Data Caching Options

Regarding data caching Asp.Net Core offers the following options.

- [Memory Cache](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory). 
- [Distributed Cache](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed).
- [Hybrid Cache](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid).

## Memory Cache

[Memory Cache](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory) uses memory as the `Cache Store`.

Memory cache should be used when the whole application runs on a single server.

When there are multiple servers running a copy of the same application, probaly using [Load Balancers](https://en.wikipedia.org/wiki/Load_balancing_(computing)), then Session Affinity, known as [Sticky Sessions](https://en.wikipedia.org/wiki/Load_balancing_(computing)#Persistence) too, must be used.

> Sticky Session is a technique used in conjunction with load balancing which ensures that requests of a certain client are serviced by the same server until the session ends.

A memory cache is an implementation of the [IMemoryCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.imemorycache) interface.

The configuration is very easy.

```
builder.Services.AddMemoryCache();  
```

The above adds the `IMemoryCache` interface as a **singleton** service.

After that the application may use that cache service either injecting it or just using the [service locator pattern](https://en.wikipedia.org/wiki/Service_locator_pattern).

`IMemoryCache` cache can store any object.

```
var Cache = GetService<IMemoryCache>();

string Key = "...";
string Data = "...";

// set
var o = new MemoryCacheEntryOptions();
o.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15);  
o.SlidingExpiration = TimeSpan.FromMinutes(15);      

Cache.Set<string>(Key, Value, o);

// get
Data = Cache.Get<string>(Key);
```
 

## Distributed Cache

[Distributed Cache](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed) uses an external shared service as the `Cache Store`. 

That shared service could be a [Redis](https://www.nuget.org/packages/Microsoft.Extensions.Caching.StackExchangeRedis) cache service, a [NCache](https://www.nuget.org/packages/Alachisoft.NCache.OpenSource.SDK/) cache service, or [MsSql server](https://www.nuget.org/packages/Microsoft.Extensions.Caching.SqlServer) cache service.

Multiple servers, or containers, running a copy of the same application access the same external cache service running in some other server.

A distributed cache is an implementation of the [IDistributedCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache) interface.

The Asp.Net Core [Session](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.isession) needs an implementation of the `IDistributedCache` interface, as a backing store, in order to function properly. Asp.Net Core offers a default implementation of the `IDistributedCache` interface which is not a distributed cache but actually a memory cache.

The configuration of that default `IDistributedCache` is very easy.

```
builder.Services.AddDistributedMemoryCache();  
```

The above adds the `IDistributedCache` interface as a **singleton** service.

After that the application may use that cache service either injecting it or just using the [service locator pattern](https://en.wikipedia.org/wiki/Service_locator_pattern).

For configurations regarding [Redis](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed#distributed-redis-cache), [NCache](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed#distributed-ncache-cache) and [MsSql](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-9.0#distributed-sql-server-cache) please consult the [docs](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed).

`IDistributedCache` cache can store `byte[]` array objects only. Everything must be converted to and from `byte[]` in order to be used.

So `IDistributedCache` cache requires serialization.

```
var Cache = GetService<IDistributedCache>();

string Key = "...";
string Data = "...";
byte[] Buffer = null;

// set
byte[] Buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Data, JsonOptions));

var o = new DistributedCacheEntryOptions();
o.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15); 
o.SlidingExpiration = TimeSpan.FromMinutes(15);  

Cache.Set(Key, Buffer, o); 

// get
Buffer = Cache.Get(Key);
Data = JsonSerializer.Deserialize<T>(Buffer, JsonOptions);
```

## Hybrid Cache

[Hybrid Cache](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid) is a combination of both Memory and Distributed cache. Offers the speed of a Memory cache and the scalability of a Distributed cache.

The requirements in order to use `Hybrid Cache` are
- the [Microsoft.Extensions.Caching.Hybrid](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Hybrid/) package to be installed
- a `using Microsoft.Extensions.Caching.Hybrid;` statement.

`Hybrid Cache` adds two-level caching
 
- L1: Memory cache, which offers fast reads
- L2: Distributed cache (Redis, NCache, MsSql Server, etc.), which offers scalability

`IDistributedCache` is **not** required. If it is not present then `Hybrid Cache`  behaves just like the `Memory Cache`.

The `Hybrid Cache` working procedure is as following

- when data is required the cache looks-up for the entry key in L1 and then in the L2 cache
- if the key exists then its data is returned to the caller
- if the key is not present then a callback function is executed which returns the data from a persistent medium, e.g. a database
- when data is retrieved from the persistent medium, it is stored in both L1 and L2 caches, under the key and an expiration policy.
 
The above logic is encapsulated in the [HybridCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.hybrid.hybridcache) abstract class. There is no `IHybridCache` interface.

Consider the signature of the `HybridCache.GetOrCreateAsync<T>()` method.

```
public ValueTask<T> GetOrCreateAsync<T>(
    string key, 
    Func<CancellationToken, ValueTask<T>> factory,
    HybridCacheEntryOptions? options = null, 
    IEnumerable<string>? tags = null, 
    CancellationToken cancellationToken = default)
```

- `key` is the entry key
- `factory` is the callback function to be executed if the data is not available in the cache
- `options` is options for the cache entry about expiration, etc
- `tags` is a collection of string tags to associate with this cache entry.

The configuration of the `Hybrid Cache` could be very easy.

```
builder.Services.AddHybridCache();
```

Or not so easy.

```
builder.Services.AddHybridCache(options =>
{
    options.MaximumPayloadBytes = 1024 * 10 * 10; 
    options.MaximumKeyLength = 256;

    options.ReportTagMetrics = true;
    options.DisableCompression = true;

    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        LocalCacheExpiration = TimeSpan.FromMinutes(15),
        Expiration = TimeSpan.FromMinutes(15)      
    };
});
```

- `MaximumPayloadBytes`. The maximum size in bytes of a cache entry.
- `MaximumKeyLength`. The maximum length of a cache key.
- `ReportTagMetrics`. When true then tags are used in metric reports.
- `DisableCompression`. When true then compression is disabled.
- `DefaultEntryOptions.Expiration`. Default value for L1 cache entries expiration.
- `DefaultEntryOptions.LocalCacheExpiration`. Default value for L2 cache entries expiration.

The `AddHybridCache()` adds the `HybridCache` class as a **singleton** service.

After that the application may use that cache service either injecting it or just using the [service locator pattern](https://en.wikipedia.org/wiki/Service_locator_pattern).
 
The `AddHybridCache()` call adds automatically the first level cache, L1, which is the `IMemoryCache`.

The L2 cache should be registered explicitly. Also any other requirements specific to L2 cache must be fulfilled.

```
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6789";  
});
```

Following is an example of use.

```
var Cache = GetService<HybridCache>();

string Key = "...";
string Data = "...";
string[] Tags = { "..." };
CancellationToken cancellationToken = default; 
var Options = new HybridCacheEntryOptions
{
    LocalCacheExpiration = TimeSpan.FromMinutes(15),
    Expiration = TimeSpan.FromMinutes(15)  
}; 

// set
await cache.SetAsync(Key, Data, Options, Tags, cancellationToken);

// get
cache.GetOrCreateAsync(
    Key, 
    async token =>
    {
        var Result = await DbService.GetDataAsync();            
        return Result;
    },
    options: Options,
    tags: Tags,
    cancellationToken: cancellationToken
); 
```

## Unified access to different caches

`Hybrid Cache` offers unified access to both `Memory Cache` and `Distributed Cache`.

Another way of unified access is the following.

`IDataCache` interface unifies access to caches.

```
public interface IDataCache
{
    T Get<T>(string Key);
    bool TryGetValue<T>(string Key, out T Value);
    T Pop<T>(string Key);

    void Set<T>(string Key, T Value);
    void Set<T>(string Key, T Value, int TimeoutMinutes);

    bool ContainsKey(string Key);
    void Remove(string Key);

    Task<T> Get<T>(string Key, Func<Task<CacheLoaderResult<T>>> LoaderFunc);
}
```

The result value of the `LoaderFunc` callback function is a `CacheLoaderResult<T>` instance. 

The `LoaderFunc` callback function is called in order to retrieve fresh data from the database only when the specified key is not found in the cache.
 

```
public class CacheLoaderResult<T>
{
    public CacheLoaderResult(T Value)
        : this(Value, Caches.DefaultEvictionTimeoutMinutes)
    {
    }
    public CacheLoaderResult(T Value, int TimeoutMinutes = 0)
    {
        this.Value = Value;
        this.TimeoutMinutes = Caches.GetTimeoutMinutes(TimeoutMinutes);
    }

    public T Value { get; set; }
    public int TimeoutMinutes { get; set; }
}
```

A class that implements `IDataCache` using `IMemoryCache`.

```
public class MemDataCache : IDataCache
{
    IMemoryCache Cache;

    public MemDataCache(IMemoryCache Cache)
    {
        this.Cache = Cache;
    }

    public T Get<T>(string Key)
    {
        return Cache.Get<T>(Key);
    }
    public bool TryGetValue<T>(string Key, out T Value)
    {
        return Cache.TryGetValue(Key, out Value);
    }
    public T Pop<T>(string Key)
    {
        T Result = Get<T>(Key);
        Remove(Key);
        return Result;
    }

    public void Set<T>(string Key, T Value)
    {
        Set(Key, Value, Caches.DefaultEvictionTimeoutMinutes);
    }
    public void Set<T>(string Key, T Value, int TimeoutMinutes)
    {
        Remove(Key);

        if (TimeoutMinutes > 0)
        {
            var o = new MemoryCacheEntryOptions();
            o.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(TimeoutMinutes); 
            o.SlidingExpiration = TimeSpan.FromMinutes(TimeoutMinutes);

            Cache.Set(Key, Value, o);
        }
        else
        {
            Cache.Set(Key, Value);
        }
    }

    public bool ContainsKey(string Key)
    {
        return Cache.TryGetValue(Key, out var Item);
    }
    public void Remove(string Key)
    {
        if (ContainsKey(Key))
            Cache.Remove(Key);
    }

    /// <summary>
    /// Returns a value found under a specified key.
    /// <para>If the key does not exist, it calls the specified loader call-back function </para>
    /// </summary>
    public async Task<T> Get<T>(string Key, Func<Task<CacheLoaderResult<T>>> LoaderFunc)
    {
        T Value;
        if (TryGetValue(Key, out Value))
            return Value;

        CacheLoaderResult<T> Result = await LoaderFunc();
        Set(Key, Result.Value, Result.TimeoutMinutes);
        return Result.Value;
    }
}
```

A class that implements `IDataCache` using `IDistributedCache`.

```
public class DistDataCache: IDataCache
{
    IDistributedCache Cache;

    static JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = null,
        WriteIndented = true,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public DistDataCache(IDistributedCache Cache)
    {
        this.Cache = Cache;
    }

    public T Get<T>(string Key)
    {
        T Value;
        return TryGetValue<T>(Key, out Value) ? Value : default(T);
    }
    public bool TryGetValue<T>(string Key, out T Value)
    {
        byte[] Buffer = Cache.Get(Key);
        Value = default;

        if (Buffer == null)
            return false;

        Value = JsonSerializer.Deserialize<T>(Buffer, JsonOptions);
        return true;
    }
    public T Pop<T>(string Key)
    {
        T Result = Get<T>(Key);
        Remove(Key);
        return Result;
    }

    public void Set<T>(string Key, T Value)
    {
        Set(Key, Value, Caches.DefaultEvictionTimeoutMinutes);
    }
    public void Set<T>(string Key, T Value, int TimeoutMinutes)
    {
        Remove(Key);

        byte[] Buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Value, JsonOptions));

        if (TimeoutMinutes > 0)
        {
            var o = new DistributedCacheEntryOptions();
            o.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(TimeoutMinutes);
            o.SlidingExpiration = TimeSpan.FromMinutes(TimeoutMinutes);

            Cache.Set(Key, Buffer, o);
        }
        else
        {
            Cache.Set(Key, Buffer);
        }
    }

    public bool ContainsKey(string Key)
    {
        return Cache.Get(Key) != null;
    }
    public void Remove(string Key)
    {
        if (ContainsKey(Key))
            Cache.Remove(Key);
    }

    /// <summary>
    /// Returns a value found under a specified key.
    /// <para>If the key does not exist, it calls the specified loader call-back function </para>
    /// </summary>
    public async Task<T> Get<T>(string Key, Func<Task<CacheLoaderResult<T>>> LoaderFunc)
    {
        T Value;
        if (TryGetValue(Key, out Value))
            return Value;

        CacheLoaderResult<T> Result = await LoaderFunc();
        Set(Key, Result.Value, Result.TimeoutMinutes);
        return Result.Value;
    }
}
```

And here is an example of use.

```
public class AppDataService<T>:  EFDataService<T> where T : BaseEntity
{
    public AppDataService()
        : base(typeof(DataContext))
    {
    }

    public override async Task<List<SelectListItem>> GetSelectList(string SelectedId = "", bool AddDefaultItem = false)
    {
        string CultureCode = Lib.Culture.Name;
        string CacheKey = $"{typeof(T).Name}-{nameof(GetSelectList)}-{CultureCode}";

        /// get from cache
        /// This code is here just for demonstration purposes.
        /// NOTE: in a real-world application information such as products, most probably, is NOT cached. 
        /// Other types (tables) such as Countries, Currencies, Measure Units, etc. used in look-ups, may be a better fit.
        List<SelectListItem> ResultList = await Lib.Cache.Get(CacheKey, async () => {
            List<SelectListItem> InnerResultList = await base.GetSelectList(SelectedId, AddDefaultItem);
            CacheLoaderResult<List<SelectListItem>> CacheResult = new(InnerResultList, Lib.Settings.Defaults.CacheTimeoutMinutes);
            return CacheResult;
        });

        return ResultList;
    }
}
```

The `AppDataService.GetSelectList()` of the above example returns a `List<SelectListItem>` instance. 

[SelectListItem](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.rendering.selectlistitem) is used in preparing drop-down lists in Asp.Net Core MVC views.

The code of the `AppDataService.GetSelectList()` method uses the `Lib.Cache` which is a `IDataCache` instance. 

The `IDataCache.Get()` call will return the `List<SelectListItem>` instance immediately if the entry key exists in cache.

If the key is not in the cache then the lambda `async => () { ... }`, which is the `LoaderFunc` callback function, is called and it returns fresh data from the database as a `CacheLoaderResult<List<SelectListItem>>` instance.

The `IDataCache.Get()` implementation sets the returned `List<SelectListItem>` to the cache, setting the expiration timeouts too.

```
public async Task<T> Get<T>(string Key, Func<Task<CacheLoaderResult<T>>> LoaderFunc)
{
    T Value;
    if (TryGetValue(Key, out Value))
        return Value;

    CacheLoaderResult<T> Result = await LoaderFunc();
    Set(Key, Result.Value, Result.TimeoutMinutes);
    return Result.Value;
}
```