# Entity Framework Core

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

[Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) is an [Object Relational Mapping (ORM)](https://en.wikipedia.org/wiki/Object%E2%80%93relational_mapping) framework.

`EF Core` is open-source, lightweight, extensible and supports [a great number of databases](https://learn.microsoft.com/en-us/ef/core/providers/).

## The EF Core Model

In order to access data in a database with `EF Core` two elements are required

- an Entity. A mostly [POCO](https://en.wikipedia.org/wiki/Plain_old_CLR_object) class that maps to a database table.
- a Data Context. A [DbContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext) derived class which represents a session with the database.

In `EF Core` terminology these two elements, the Entities and the Data Context, are called [Model](https://learn.microsoft.com/en-us/ef/core/modeling/).

The `EF Core Model` is mainly a configuration on how entity types are mapped to tables of the underlying database.
 

The `DbContext` provides the [Model](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.model) property of type [IModel](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.imodel) which provides properties and methods that return information about the `EF Core Model`.

Also the [DbSet<T>](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbset-1), explained later in this text, provides the [EntityType](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbset-1.entitytype) property which provides `EF Core Model` information about the entity associated to the `DbSet<T>`.

## The Entities and the DbContext

Consider the following entities.

```
[PrimaryKey(nameof(Id))]
public class BaseEntity
{
    public BaseEntity()
    {
    }

    static public string GenId()
    {
        string format = "D";
        return Guid.NewGuid().ToString(format).ToUpper();
    }

    public virtual void SetId()
    {
        this.Id = GenId();
    }

    [Key, MaxLength(40), DefaultValue(null), JsonPropertyOrder(-1)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; }
}

[Table(nameof(Product))]
[Index(nameof(Name), IsUnique = true)]
public class Product : BaseEntity
{
    public Product() 
    { 
    }

    [MaxLength(128), Required]
    public string Name { get; set; }
    [Precision(18, 4), Required]
    public decimal Price { get; set; }
}

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
    }
}
```

`Product` is derived from the `BaseEntity` class.

Next is the `DbContext`.

```
public class DataContext: DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string DatabasePath = Path.Combine(System.AppContext.BaseDirectory, "Sqlite.db3");
        optionsBuilder.UseSqlite($"Data Source={DatabasePath}", SqliteOptionsBuilder => { });
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Product).Assembly);
        DemoData.AddData(modelBuilder);
    } 

    public DataContext()
        : this(new DbContextOptions<DataContext>())
    {
    }
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    static public void EnsureDatabase()
    {
        using (var context = new DataContext())
        {
            context.Database.EnsureCreated();
        }
    }

    public DbSet<Product> Products { get; set; }

}
```

- It is not a bad idea to have an ultimate base entity class, such as the `BaseEntity` in this example. That way it is easy to create generic repositories or generic services that handle [CRUD operations](https://en.wikipedia.org/wiki/Create,_read,_update_and_delete) in a generic way.
- The advocates of [generic repositories or generic services](https://kenslearningcurve.com/tutorials/the-generic-repository-in-c/) argue that they are ideal for `CRUD` operations.
- The opposite party presents the strong argument that `DbContext` is already a kind of a generic repository.
- The `[Table(nameof(Product))]` attribute instructs `EF Core` to use a certain table name for an entity. Attributes are explained later in this text. 
- `DbContext` does not provide a public default constructor, i.e. a constructor without any parameter. The example provides a public default constructor.
- The `OnConfiguring()` method instructs the `EF Core` to use a [Sqlite](https://sqlite.org/) database.
- The line `context.Database.EnsureCreated()` of the `static public void EnsureDatabase()` ensures that a database is created.
- The `ApplyConfigurationsFromAssembly()` call in the `OnModelCreating()` method forces `EF Core` to search a specified Assembly for entity classes and include them in its `Model`. This is done because our entity classes are accompanied by a `IEntityTypeConfiguration` interface implementation. The `ApplyConfigurationsFromAssembly()` searches the Assembly for types implementing the `IEntityTypeConfiguration` interface.
- The line `DemoData.AddData(modelBuilder)`, in the `OnModelCreating()` method, uses the static `DemoData` class to add initial data in the database.
- [DbContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext) is used for `CRUD` operations and is a generic repository too.
- [DbSet](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbset-1), like the `DbContext`, is used for `CRUD` operations and is a generic repository too.
- `DbContext` and `DbSet` provide `CRUD` methods such as `Add()`, `Remove()` and `Update()`.
- `DbContext` provides the `SaveChanges()` method which [commits](https://en.wikipedia.org/wiki/Commit_(data_management)) the changes to the database.
- It is a good idea to review the documentation for both of these clasess and check the provided properties and methods. 
- `DbContext` is **not** thread safe.
- `DbContext` is [IDisposable](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose). Its lifetime should be as short as possible.
- `DbContext` represents a session with the database, i.e. a [Transaction](https://en.wikipedia.org/wiki/Database_transaction).
- In other words `DbContext` implements the [Unit of Work](https://en.wikipedia.org/wiki/Unit_of_work) pattern. 
- An `EF Core` application may call many times `Add()`, `Remove()` or `Update()` methods, against one or more entities, and then, in the end, call the `DbContext.SaveChanges()` method in order to [commit](https://en.wikipedia.org/wiki/Commit_(data_management)) the changes to the database.
 
## Insert, Update, Delete

> There are `Async()` versions of most of the methods used in the next example.

### Insert.

```
using (var Context = new DataContext())
{
    Product product = new()
    {
        Id = BaseEntity.GenId(),
        Name = "Laptop",
        Price = 1200.5M,
    };

    Context.Add(product);
    Context.SaveChanges();
}
```

### Update.

```
using (var Context = new DataContext())
{
    var product = Context.Products.FirstOrDefault(x => x.Name == "Laptop");

    product.Name = "Tablet"; 

    Context.Update(product);
    Context.SaveChanges();
}
```

### Delete.

```
using (var Context = new DataContext())
{
    var product = Context.Products.FirstOrDefault(x => x.Name == "Tablet");

    Context.Remove(product);
    Context.SaveChanges();
}
```

- The `Context.Products` used in update and delete is the `DbSet<Product> Products` property of the `DataContext`.
- The `Products` property is actually a [IQueryable<Product>](https://learn.microsoft.com/en-us/dotnet/api/system.linq.iqueryable-1) instance.
- A `DbSet<T>` is a `IQueryable<T>` instance.


## Quering Data

In `EF Core` [Language-Integrated Query (LINQ)](https://learn.microsoft.com/en-us/dotnet/csharp/linq/) is used in querying data from the database.

`EF Core` provides a [great number of facilities](https://learn.microsoft.com/en-us/dotnet/csharp/linq/standard-query-operators/) in querying data.

> There are `Async()` versions of most of the methods used in the next example.


### Loading a single entity

```
using (var Context = new DataContext())
{
    Product product = Context.Products.Single(p => p.Name == "Tablet");
}
```

### Loading a list of entities

```
using (var Context = new DataContext())
{
    List<Product> List = Context.Products
                          .ToList();
}
```

### Filtering

```
using (var Context = new DataContext())
{
    List<Product> List = Context.Products
      .Where(p => p.Name.Contains("blet"))
      .ToList();
}
```

> Filtering limits the result size. There maybe entities, such as a `SalesOrder` entity, with millions of rows that not always needed.

### Ordering

```
using (var Context = new DataContext())
{
    List<Product> List = Context.Products
      .OrderBy(p => p.Name)
      .ToList();
}
```

### Query result types

```
IQueryable<Product> Q1 = Context.Products;

IQueryable<Product> Q2 = Q1.Where(p => p.Name.Contains("blet"));

IOrderedQueryable<Product> Q3 = Q2.OrderBy(p => p.Name);

// until now no query is executed against the database.
// calling ToList() executes the SQL statement in the database
List<Product> List = Q3.ToList();
```

Chained all together.

```
using (var Context = new DataContext())
{
    List<Product> List = Context.Products
      .Where(p => p.Name.Contains("blet"))
      .OrderBy(p => p.Name)  
      .ToList();
}
```

### Projections

Mapping one set of properties to another is called `Projection`.

The next example creates an [Anonymous Type](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types) with just two of the properties of the `Product` class.

```
using (var Context = new DataContext())
{ 
    var List = Context.Products
        .Select(p => new { p.Id, p.Name })
        .ToList();
}
```

Projection with a specific type.

```
public class ProjectSimple
{
    public string Id { get; set; }
    public string Name { get; set; }
}

...
using (var Context = new DataContext())
{ 
    var List = Context.Products
        .Select(p => new ProjectSimple() { p.Id, p.Name })
        .ToList();
}
```

> Projections limit the result size. There maybe entities with tenths of fields that not always needed.


## Change Tracking

`DbContext` uses [Change Tracking](https://learn.microsoft.com/en-us/ef/core/change-tracking/), that is it tracks changes made to entities. 

An entity is tracked when

- it is returned from a query, e.g. `Context.Products.FirstOrDefault(x => x.Name == "Laptop")` 
- the `Add()`, `Update()` or `Attach()` methods of the `DbContext` or `DbSet` are used with an entity. 

The `DbContext.SaveChanges()` detects any changes made to an entity and updates the underlying database.

### The EntityEntry

The `DbContext.Entry()` and the `DbSet.Entry()` methods return an [EntityEntry](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.changetracking.entityentry) instance which provides change tracking information regarding a specified entity.

The `EntityEntry.State` read-write property is an [EntityState](https://learn.microsoft.com/en-us/ef/core/change-tracking/#entity-states) enum type indicating the `tracking` state of an entity.

### The ChangeTracker

The [DbContext.ChangeTracker](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.changetracker) property, is an instance of [ChangeTracker](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.changetracking.changetracker), which provides methods and properties to configure `change tracking`.


### Disabling Change Tracking

In `EF Core`, by default, entities returned from queries are [tracked entities](https://learn.microsoft.com/en-us/ef/core/querying/tracking).

Change tracking can be disabled 

- in the `DbContext.OnConfiguring()` method in order to affect any `DbContext` instance

```
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    string DatabasePath = Path.Combine(System.AppContext.BaseDirectory, "Sqlite.db3");
    
    optionsBuilder.UseSqlite($"Data Source={DatabasePath}", SqliteOptionsBuilder => { })
      .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
}
```

- using the `DbContext.ChangeTracker.QueryTrackingBehavior` or setting the `DbContext.ChangeTracker.AutoDetectChangesEnabled` to **false**, in order to affect every query in a certain `DbContext`

```
DataContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

DataContext.ChangeTracker.AutoDetectChangesEnabled = false;
```

- using the `AsNoTracking()` method to affect a certain query 

```
DataContext.Products.AsNoTracking(); 
```

- using the `DbContext.ChangeTracker.Clear()` method to clear all change tracking information from a `DbContext`

```
DataContext.ChangeTracker.Clear(); 
```

- using the `EntityEntry.State` property to detach an entity

```
var Entry = Context.Entry(product);
Entry.State = EntityState.Detached;
```

## The DbContext

[DbContext](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/) represents a session with the database, i.e. a [Transaction](https://en.wikipedia.org/wiki/Database_transaction) and implements the [Unit of Work](https://en.wikipedia.org/wiki/Unit_of_work) pattern.

All the `CRUD` operation methods, such as `Add()`, `Remove()` and `Update()`, are executed inside a transaction. The `SaveChanges()` method [commits](https://en.wikipedia.org/wiki/Commit_(data_management)) the changes to the database. 

`DbContext` is [IDisposable](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose). Its lifetime should be as short as possible.

 
### DbContext in Dependency Injection
 
`DbContext` can be registered as a [Scoped Service](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) using

```
builder.Services.AddDbContext<DataContext>();
```
There is variation that accepts an options instance.

```
string ConnectionString = "...";

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(ConnectionString));
```

`DbContext` does not provide a public default constructor, i.e. a constructor without any parameter. 

For a `DbContext` to be used in Dependency Injection a public constructor is required with a single `DbContextOptions` parameter. This is the constructor used by Dependency Injection container when an instance of a `DbContext` is constructed.

```
public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
}
``` 

After that the `DbContext` can be injected wherever Dependency Injection can be used.

```
public class MyController
{
    private readonly DataContext DataContext;

    public MyController(DataContext context)
    {
        DataContext = context;
    }
}
```

> Using `DbContext` as a scope service it is useful when an application needs to use the same `DbContext` instance in performing multiple `units of work` within the scope of a single HTTP request.


### Creating a DbContext with `new()`
 
```
public class DataContext: DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string DatabasePath = Path.Combine(System.AppContext.BaseDirectory, "Sqlite.db3");
        optionsBuilder.UseSqlite($"Data Source={DatabasePath}", SqliteOptionsBuilder => { });
    }

    public DataContext()
        : this(new DbContextOptions<DataContext>())
    {
    }
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
}
```
This example uses a dummy `DbContextOptions<DataContext>` parameter in the default constructor but the truth is that this parameter is not actually required. 

The actual configuration takes place in the `OnConfiguring()` method using the passed-in [DbContextOptionsBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder-1)

Now a `DbContext` instance can be created using the `new()` operator.

```
using (var Context = new DataContext())
{ 
    ...
}
```
### Creating a DbContext with a DbContext factory

A call to `AddDbContextFactory()` is required. That call registers a [IDbContextFactory<DbContext>](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#use-a-dbcontext-factory) **scoped** service.

```
builder.Services.AddDbContextFactory<DataContext>();
```

After that a `DbContext` instance can be created using the `IDbContextFactory` service.

 
```
public class MyController
{ 
    private IDbContextFactory<DataContext> DbContextFactory;

    public MyController(IDbContextFactory<DataContext> DbContextFactory)
    {
        this.DbContextFactory = DbContextFactory;
    }

    public IActionResult Action1()
    {
        using (var DataContext = DbContextFactory.CreateDbContext())
        {
            ...
        }
    }
} 
```

## Configuring a DbContext Database Provider

A `DbContext` uses a single [Database Provider](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#configuring-the-database-provider).

The configuration of a `database provider` is done using a `DbContextOptionsBuilder.UseXXX()` extension method, such as the `UseSqlite()`.

```
string DatabasePath = Path.Combine(System.AppContext.BaseDirectory, "Sqlite.db3");
optionsBuilder.UseSqlite($"Data Source={DatabasePath}", SqliteOptionsBuilder => { });
```
For the needed `UseXXX()` method to be available the proper [NuGet package](https://learn.microsoft.com/en-us/ef/core/providers) must be installed.

All these `UseXXX()` method accept a connection string as a sole parameter.


## Configuring Entities

The `EF Core Model` is mainly a configuration on how entity types are mapped to tables of the underlying database.

In configuring entities the `EF Core` uses a lot of built-in [conventions](https://learn.microsoft.com/en-us/ef/core/modeling/#built-in-conventions) in order to spare a lot of work from the developer.

### Built-in Conventions

Most common are the following.

- **Table Name**. The name of an entity class is used in mapping the entity to a table with the same name in the underlying database.
- **Column Name**. The name of a property is used in mapping the property to a table column with the same name in the underlying database.
- **Primary Key**. A property named `ID`, `Id`, `id` or `EntityNameId` (not case-sensitive), is configured as the primary key.
- **Foreign Key**. If a property is named `ForeignEntityId` and there is an entity having `ForeignName` as name and a primary key of the same data type then that property is configured as a foreign key.
- **Data Types**. The Database Provider decides the appropriate mapping. 
 
### Using Data Annotation Attributes

https://www.learnentityframeworkcore.com/configuration/data-annotation-attributes
 



## XXX
 
 

## ZZZ


 