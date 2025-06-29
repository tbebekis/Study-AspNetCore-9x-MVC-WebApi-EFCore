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

## Entities and the DbContext

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

### Query Projections

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

## DbContext

[DbContext](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/) represents a session with the database, i.e. a [Transaction](https://en.wikipedia.org/wiki/Database_transaction) and implements the [Unit of Work](https://en.wikipedia.org/wiki/Unit_of_work) pattern.

All the `CRUD` operation methods, such as `Add()`, `Remove()` and `Update()`, are executed inside a transaction. The `SaveChanges()` method [commits](https://en.wikipedia.org/wiki/Commit_(data_management)) the changes to the database. 

`DbContext` is [IDisposable](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose). Its lifetime should be as short as possible.

 
## DbContext in Dependency Injection
 
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


## Creating a DbContext with `new()`
 
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
## Creating a DbContext with a DbContext factory

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

## DbSet&lt;T&gt;

TODO: DbSet

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

Configuration is done

- with `EF Core` built-in configuration conventions
- using Data Annotation attributes
- using fluent syntax in `DbContext.OnModelCreating()` or `IEntityTypeConfiguration<T>.Configure()`.



## `EF Core` built-in Configuration Conventions

In configuring entities the `EF Core` uses a lot of built-in [conventions](https://learn.microsoft.com/en-us/ef/core/modeling/#built-in-conventions) in order to spare a lot of work from the developer.

Most common are the following.

- **Table Name**. The name of an entity class is used in mapping the entity to a table with the same name in the underlying database.
- **Column Name**. The name of a property is used in mapping the property to a table column with the same name in the underlying database.
- **Primary Key**. A property named `ID`, `Id`, `id` or `EntityNameId` (not case-sensitive), is configured as the primary key.
- **Foreign Key**. If a property is named `ForeignEntityId` and there is an entity having `ForeignName` as name and a primary key of the same data type then that property is configured as a foreign key.
- **Data Types**. The Database Provider decides the appropriate mapping. 
- **Nullable Types**. Properties with nullable data types can be null, e.g. `string? Name { get; set; }`, otherwise a value is required, e.g. `string Name { get; set; }`.
 

By convention, an alternate key is introduced for you when you identify a property which isn't the primary key as the target of a relationship.

## Configuration using Data Annotation Attributes

Attributes can be placed on a class or property and specify metadata about that class or property.

Data annotation attributes modify or override configuration which is imposed by `EF Core` built-in conventions.

There is a great number of data annotation attributes. Some of them are `mapping` attributes while others are `validation` attributes.

Data annotation attributes can be found in many namespaces such as [System.ComponentModel.DataAnnotations](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations),  [System.ComponentModel.DataAnnotations.Schema](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema) or [Microsoft.EntityFrameworkCore](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore).

Next is a list of frequently used attributes.

- [**TableAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema.tableattribute). Annotates an entity class specifying the database table the entity is mapped to.
```
[Table(nameof(Product))]
public class Product : BaseEntity
{
    ...
}
```
- [**PrimaryKeyAttribute**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.primarykeyattribute). Annotates an entity class specifying the primary key which can be comprised of a single property or multiple properties when compound keys are used.

```
[PrimaryKey(nameof(Id))]
public class BaseEntity 
{
    ...
}
```

- [**IndexAttribute**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.indexattribute). Annotates an entity class specifying an index that should be generated in the database.

```
[Index(nameof(ProductId), nameof(MeasureUnitId), IsUnique = true)]
public class ProductMeasureUnit : BaseEntity
{
    public string ProductId { get; set; }
    public string MeasureUnitId { get; set; }
    ...
}
```

- [**KeyAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.keyattribute). Annotates a property specifying the primary key.

```
public class BaseEntity 
{
    [Key]
    public string Id { get; set; }
    ...
}
```

- [**ForeignKeyAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema.foreignkeyattribute). Annotates a property denoting that the property is used as a foreign key in a relationship between two entites.

> In `EF Core` terminology the `foreign` entity is called `Principal` entity while the entity that depends on the principal entity is called `Dependent` entity.

Next two examples are taken from `EF Core` [docs](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/mapping-attributes#foreignkeyattribute).

```
public class Blog
{
    public string Id { get; set; }
    public List<Post> Posts { get; } = new();
}

public class Post
{
    public int Id { get; set; }

    [ForeignKey(nameof(Blog))]
    public string BlogKey { get; set; }

    public Blog Blog { get; init; }
}
```

An alternative way with the same result.

```
public class Blog
{
    public string Id { get; set; }
    public List<Post> Posts { get; } = new();
}

public class Post
{
    public int Id { get; set; }

    public string BlogKey { get; set; }

    [ForeignKey(nameof(BlogKey))]
    public Blog Blog { get; init; }
}
```

- [**KeylessAttibute**](https://learn.microsoft.com/en-us/ef/core/modeling/keyless-entity-types). Annotates an entity specifying that the entity has no primary key. Can be used to execute database queries returning keyless entities.

```
[Keyless]
public class ProductOrdersTotal  
{
    public ProductOrdersTotal()
    {
    }

    public string Name { get; set; }
    public decimal OrdersTotal { get; set; } 
}
```

- [**ColumnAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema.columnattribute). Annotates a property. Can be used to specify the column name in the database or the database provider specific data type of the column or both.

```
public class MeasureUnit : BaseEntity 
{
    [Column("UnitOfMeasure")]
    public string Name { get; set; }
    ...
}

public class Product : BaseEntity
{
    [Column(Name = "UnitPrice", TypeName = "decimal(18, 4)")]
    public decimal Price { get; set; }
    ...
}
```

- [**RequiredAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.requiredattribute). Annotates a property specifying that a value is required, i.e. it cannot be **null**.

```
public class MeasureUnit: BaseEntity 
{
    [Required]
    public string Name { get; set; }
    ...
}
```

- [**MaxLengthAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.maxlengthattribute). Annotates a property specifying the maximum allowed length of array or string data. There is `MinLengthAttribute` counterpart too.

```
public class BaseEntity 
{
    [MaxLength(40)]
    public string Id { get; set; }
    ...
}
```

- [**StringLengthAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.stringlengthattribute). Annotates a string property specifying min and max length at once.

```
public class Product
{
    [StringLength(128, MinimumLength = 4)]
    public string Name { get; set; } 
    ...
}
```

- [**RangeAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.rangeattribute). Annotates a numeric property specifying a range of valid values.

```
public class Product : BaseEntity
{        
    [Range(0, 50000)]
    public decimal Price { get; set; }
}
```

- [**PrecisionAttribute**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.precisionattribute). Annotates a decimal or double property specifying the precision of data.

```
public class Product : BaseEntity
{
    [Precision(18, 4)]
    public decimal Price { get; set; }
    ...
}
```

- [**UnicodeAttribute**](https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties?tabs=data-annotations%2Cwith-nrt#unicode). Annotates a string property enabling or disabling the storing of data as unicode. Has no effect for database providers that do not support this feature.

```
public class Product : BaseEntity
{
    [Unicode(true)]
    public string Name { get; set; }
    ...
}
``` 

- [**CommentAttribute**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.commentattribute). Annotates a property specifying a comment about the column. For database providers that support this feature.

```
public class Product : BaseEntity
{
    [Comment("Product name in the default language, i.e. english.")]
    public string Name { get; set; }
    ...
}
```

- [**DefaultValueAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.defaultvalueattribute). Annotates a property specifying a default value.

```
public class Category : BaseEntity 
{
    [DefaultValue(false)]
    public bool IsPublic { get; set; } 
}
```

- [**DatabaseGeneratedAttribute**](https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=data-annotations#explicitly-configuring-value-generation). Annotates a property specifying if and how the database provider generates values the property.

```
public class BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; }
    ...
}
```

- [**TimestampAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.timestampattribute). Annotates a byte array , i.e. byte[], property. Can only be applied once per entity. Used when the database provider is the MsSql server.   

```
public class Product : BaseEntity
{        
    [Timestamp]
    public byte[] RowTimestamp { get; set; }
}
```

- [**NotMappedAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema.notmappedattribute). Annotates a property specifying that it should be not mapped to a database column.

```
public class AppUser: BaseEntity 
{
    public string UserName { get; set; }
    public string Password { get; set; }

    [NotMapped]
    public string ClientId 
    {
        get => UserName;
        set => UserName = value;
    }
    [NotMapped]
    public string Secret 
    {
        get => Password;
        set => Password = value;
    }
    ...
}
```

- [**ComplexTypeAttibute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema.complextypeattribute). Specifies that a type is a [complex type](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/whatsnew#value-objects-using-complex-types).

```
[ComplexType]
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string PostCode { get; set; }
}

public class SalesOrder
{
    public int Id { get; set; }
    public Address ShippingAddress { get; set; }
    public Address BillingAddress { get; set; }
    ...
}
```

A discussion on complex types can be found in this [announcement](https://devblogs.microsoft.com/dotnet/announcing-ef8-rc1/).

A complex type

- it is not an entity per se, i.e. no `DbSet<MyComplexType>`
- it is not identified by a key value and is not tracked 
- should be used as a property of an entity type
- it is not autonomously saved by a `DbContext` or a `DbSet`, but instead is saved as a part of an entity
- can be a reference or a value type, i.e. either class or record
- can be shared by multiple properties in the same entity.
- can be used by multipte entities
- must be defined as a **required** value in the `OnModelCreating()` method.

## Configuration using Fluent Syntax

Fluent API methods reside in the [Microsoft.EntityFrameworkCore.Metadata.Builders](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders) as members in various `builder` classes.

Configuration using fluent syntax can be done in `IEntityTypeConfiguration<T>.Configure()` or `DbContext.OnModelCreating()`.

Configuration using fluent syntax modify or override configuration which is imposed by `EF Core` built-in conventions or added using Data annotation attributes.

It is not a bad idea to implement the `IEntityTypeConfiguration<T>` for every entity.

```
public class Product : BaseEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        ...
    }
}
```

That way the application can use the `ApplyConfigurationsFromAssembly()`. 

The `ApplyConfigurationsFromAssembly()` instructs `EF Core` to search an Assembly for types implementing the `IEntityTypeConfiguration` interface and include entities and entity configurations in the `Model`.

It's a matter of preference but having an implementation of `IEntityTypeConfiguration` interface leads to less code in the `DbContext.OnModelCreating()` and easy management of entity configuration.

The other way is to use the `DbContext.OnModelCreating()` in entity configuration.

```
public class DataContext: DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ...
    } 
    ...
}
```

### Fluent API Methods - Model Configuration

The following methods are members of the [ModelBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.modelbuilder) class. The presented list is not exhaustive.

- [**Entity()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.modelbuilder.entity). Performs configuration of a specified entity type.

```
modelBuilder.Entity<Product>().HasKey(p => p.Id);
```

- [**Owned()**](https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities). Specifies that an entity is an owned one.

```
TODO: Owned()
```

- [**HasDbFunction()**](https://learn.microsoft.com/en-us/ef/core/querying/user-defined-function-mapping). Maps a CLR function to a database [UDF](https://learn.microsoft.com/en-us/sql/relational-databases/user-defined-functions/user-defined-functions) function.

A database UDF function.
 
```
CREATE FUNCTION get_product_salesorders_total(@ProductId nvarchar(40))
RETURNS DECIMAL AS 
BEGIN
    RETURN (SELECT SUM(LineAmount) FROM SalesOrderLine WHERE ProductId = ProductId);
END;
```

A **not-mapped** entity.

```
[NotMapped]
public class ProductOrdersTotal : BaseEntity
{
    public ProductOrdersTotal() { }
    public ProductOrdersTotal(string Id, string Name, decimal OrdersTotal)
    {
        this.Id = Id;
        this.Name = Name;
        this.OrdersTotal = OrdersTotal;
    }

    public string Name { get; set; }
    public decimal OrdersTotal { get; set; } 
}
```

The `DbContext`.

```
public class DataContext: DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(
            typeof(DataContext).GetMethod(nameof(GetProductSalesOrderTotalMap), 
            new[] { typeof(string) })
        ).HasName("get_product_salesorders_total");
    }
    
    // this method is never called, 
    // it is here just for the mapping to a database function
    public static decimal GetProductSalesOrderTotalMap(string ProductId)
        => throw new NotImplementedException();

    // a method that uses the mapped function
    public List<ProductOrdersTotal> GetSalesOrderTotals()
    { 
        return this.Products.Select
          (p => new ProductOrdersTotal(p.Id, 
                                       p.Name,
                                       GetProductSalesOrderTotalMap(p.Id))
          ).ToList();
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<SalesOrderLine> SalesOrderLines { get; set; }
    ...
}
```

- [**HasDefaultSchema()**](https://learn.microsoft.com/en-us/ef/core/modeling/entity-types#table-schema). Specifies the default database schema.


```
modelBuilder.HasDefaultSchema("dbo");
```

- [**HasSequence()**](https://learn.microsoft.com/en-us/ef/core/modeling/sequences).	Configures a database sequence. Valid with relational databases that support sequences.

```
modelBuilder.HasSequence<int>("SalesOrderCodeNo");

modelBuilder.Entity<SalesOrder>()
    .Property(o => o.OrderNo)
    .HasDefaultValueSql("NEXT VALUE FOR SalesOrderCodeNo");
```

- [**HasAnnotation()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.modelbuilder.hasannotation). A `Key-Value` pair that attaches arbitrary information to the `Model` which can later be read from the `Model` using its key. `EF Core` uses `HasAnnotation()` internally to configure things such as the constraint name of a foreign key. It has little value to a developer unless he implements something that uses it. Check [this discussion](https://github.com/dotnet/efcore/issues/13028).

```
modelBuilder.HasAnnotation("MyKey", "MyValue");
```

- [**HasChangeTrackingStrategy**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.modelbuilder.haschangetrackingstrategy). Specifies the [ChangeTrackingStrategy](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.changetrackingstrategy) to be used with this `EF Core Model`. A `ChangeTrackingStrategy` indicates how the `DbContext` detects changes happened in entity properties.

```
modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
```

- [**Ignore()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.modelbuilder.ignore). When used with a [ModelBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.modelbuilder) specifies that an entity is not-mapped to a database table.

```
modelBuilder.Ignore("ProductOrdersTotal");

modelBuilder.Ignore(typeof(ProductOrdersTotal));

modelBuilder.Ignore<ProductOrdersTotal>();
```

### Fluent API Methods - Entity Configuration

The following methods are members of the [EntityTypeBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1) class. The presented list is not exhaustive.

- [**ToTable()**](https://learn.microsoft.com/en-us/dotnet/api/system.data.entity.modelconfiguration.entitytypeconfiguration-1.totable). Specifies the name of a database table that the entity maps to.
```
modelBuilder.Entity<SalesOrder>().ToTable("Sales_Order");
```
- [**HasKey()**](https://learn.microsoft.com/en-us/ef/core/modeling/keys#configuring-a-primary-key). Configures one or more properties as the primary key.
```
modelBuilder.Entity<Product>().HasKey(p => p.Id);
modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
```
- [**HasNoKey()**]()
Annotates an entity specifying that the entity has no primary key. Can be used to execute database queries returning keyless entities.
```
modelBuilder.Entity<ProductOrdersTotal>().HasNoKey();
```


- [**HasAlternateKey()**](https://learn.microsoft.com/en-us/ef/core/modeling/keys?tabs=data-annotations#alternate-keys). Configures an alternate key for an entity. An alternate key, just like a primary key, uniquely identifies an entity. It can be a multi-column key and it is useful in establishing relationships between entities.

```
modelBuilder.Entity<Company>().HasAlternateKey(c => c.TaxPayerId);  // a TIN
modelBuilder.Entity<Car>().HasAlternateKey(c => new { c.PlateNo, c.ChassisNo });
```

- [**HasIndex()**](https://learn.microsoft.com/en-us/ef/core/modeling/indexes). Creates an index on one or more properties. The index can be a unique one.

```
modelBuilder.Entity<Product>().HasIndex(p => p.Name });
modelBuilder.Entity<User>().HasIndex(u => new { u.FirstName, u.LastName });
```
- [**Ignore()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1.ignore). When used with an [EntityTypeBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1) specifies one or more properties that are excluded from mapping.

```
modelBuilder.Entity<User>().Ignore(u => u.FullName);
```

- [**ComplexProperty()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1.complexproperty). Specifies that a property of a class or structure type is a `Complex Type`. Check the `ComplexTypeAttribute` presented earlier.

```
modelBuilder.Entity<SalesOrder>(so => {
		so.ComplexProperty(sa => sa.ShippingAddress, sa => { sa.IsRequired(); });
		so.ComplexProperty(ba => ba.BillingAddress, ba => { ba.IsRequired(); });
	});
```

- [**HasData()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1.hasdata). Used in adding initial data to the database.

```
modelBuilder.Entity<Product>().HasData(new List<Product>()
{
    new Product(){ ... }
    ...
});
```

- [**ToView()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.relationalentitytypebuilderextensions.toview). Specifies that an entity maps to a database view.

```
modelBuilder.Entity<ProductOrdersTotal>().ToView("v_product_salesorders_total");
```
- [**ToSqlQuery()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.relationalentitytypebuilderextensions.tosqlquery). Maps an entity to a `SELECT` Sql statement.

```
modelBuilder.Entity<ProductOrdersTotal>(builder => { builder.ToSqlQuery("select * from v_product_salesorders_total"); });

...

// example usage
var Totals = MyDataContext.Set<ProductOrdersTotal>().ToList();
```

- [**HasQueryFilter()**](https://learn.microsoft.com/en-us/ef/core/querying/filters). Specifies that the entity has a global query filter that should be automatically applied to queries of this entity type.

```
modelBuilder.Entity<User>().HasQueryFilter(p => p.UserType == "ClientApplication");
```

- [**Property()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1.property). Returns a [PropertyBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1) which is use in configuring a property of the entity.

```
modelBuilder.Entity<User>().Property(u => u.Salt).HasColumnName("PasswordSalt");
```

### Fluent API Methods - Property Configuration

- [**HasColumnType()**]().	Configures the data type of the corresponding column in the database for the property.
- [**HasComputedColumnSql()**]().	Configures the property to map to computed column in the database when targeting a relational database.
- [**HasDefaultValue()**]().	Configures the default value for the column that the property maps to when targeting a relational database.
- [**HasDefaultValueSql()**]().	Configures the default value expression for the column that the property maps to when targeting relational database.
- [**HasField()**]().	Specifies the backing field to be used with a property.
- [**HasMaxLength()**]().	Configures the maximum length of data that can be stored in a property.
- [**IsConcurrencyToken()**]().	Configures the property to be used as an optimistic concurrency token.
- [**IsRequired()**]().	Configures whether the valid value of the property is required or whether null is a valid value.
- [**IsRowVersion()**]().	Configures the property to be used in optimistic concurrency detection.
- [**IsUnicode()**]().	Configures the string property which can contain unicode characters or not.
- [**ValueGeneratedNever()**]().	Configures a property which cannot have a generated value when an entity is saved.
- [**ValueGeneratedOnAdd()**]().	Configures that the property has a generated value when saving a new entity.
- [**ValueGeneratedOnAddOrUpdate()**]().	Configures that the property has a generated value when saving new or existing entity.
- [**ValueGeneratedOnUpdate()**]().	Configures that a property has a generated value when saving an existing entity. 

 
## XXX
 
 - [**HasOne()**]().
- [**HasMany()**]().
- [**OwnsOne()**]().

## ZZZ


 