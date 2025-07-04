# Entity Framework Core

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

## Table of Contents

- [Entity Framework Core](#entity-framework-core)
  - [Table of Contents](#table-of-contents)
  - [Introduction](#introduction)
  - [The EF Core Model](#the-ef-core-model)
  - [DbContext](#dbcontext)
    - [Configuring a DbContext Database Provider](#configuring-a-dbcontext-database-provider)
    - [DbContext in Dependency Injection](#dbcontext-in-dependency-injection)
    - [Creating a DbContext with `new()`](#creating-a-dbcontext-with-new)
    - [Creating a DbContext with a DbContext factory](#creating-a-dbcontext-with-a-dbcontext-factory)
    - [Entities and the DbContext](#entities-and-the-dbcontext)
  - [Saving Data](#saving-data)
    - [Insert.](#insert)
    - [Update.](#update)
    - [Delete.](#delete)
  - [Quering Data](#quering-data)
    - [Loading a single entity](#loading-a-single-entity)
    - [Loading a list of entities](#loading-a-list-of-entities)
    - [Filtering](#filtering)
    - [Ordering](#ordering)
    - [Paging](#paging)
    - [Query result types](#query-result-types)
    - [Query Projections](#query-projections)
  - [DbSet\<T\>](#dbsett)
  - [Change Tracking](#change-tracking)
    - [The EntityEntry](#the-entityentry)
    - [The ChangeTracker](#the-changetracker)
    - [Disabling Change Tracking](#disabling-change-tracking)
  - [Configuration](#configuration)
  - [Configuration - built-in Conventions](#configuration---built-in-conventions)
  - [Configuration - Data Annotation Attributes](#configuration---data-annotation-attributes)
    - [**TableAttribute**.](#tableattribute)
    - [**PrimaryKeyAttribute**.](#primarykeyattribute)
    - [**IndexAttribute**.](#indexattribute)
    - [**KeyAttribute**.](#keyattribute)
    - [**ForeignKeyAttribute**.](#foreignkeyattribute)
    - [**KeylessAttibute**.](#keylessattibute)
    - [**ColumnAttribute**.](#columnattribute)
    - [**RequiredAttribute**.](#requiredattribute)
    - [**MaxLengthAttribute**.](#maxlengthattribute)
    - [**StringLengthAttribute**.](#stringlengthattribute)
    - [**RangeAttribute**.](#rangeattribute)
    - [**PrecisionAttribute**.](#precisionattribute)
    - [**UnicodeAttribute**.](#unicodeattribute)
    - [**CommentAttribute**.](#commentattribute)
    - [**DefaultValueAttribute**.](#defaultvalueattribute)
    - [**DatabaseGeneratedAttribute**.](#databasegeneratedattribute)
    - [**TimestampAttribute**.](#timestampattribute)
    - [**NotMappedAttribute**.](#notmappedattribute)
    - [**InversePropertyAttribute**](#inversepropertyattribute)
    - [**DeleteBehaviorAttribute**](#deletebehaviorattribute)
    - [**ComplexTypeAttibute**.](#complextypeattibute)
    - [**OwnedAttribute**](#ownedattribute)
  - [Configuration - Fluent API](#configuration---fluent-api)
  - [Configuration - Fluent API - Model Configuration](#configuration---fluent-api---model-configuration)
    - [**Entity()**](#entity)
    - [**Owned()**](#owned)
    - [**HasDbFunction()**](#hasdbfunction)
    - [**HasDefaultSchema()**](#hasdefaultschema)
    - [**HasSequence()**](#hassequence)
    - [**HasAnnotation()**](#hasannotation)
    - [**HasChangeTrackingStrategy**](#haschangetrackingstrategy)
    - [**Ignore()**](#ignore)
  - [Configuration - Fluent API - Entity Configuration](#configuration---fluent-api---entity-configuration)
    - [**ToTable()**](#totable)
    - [**HasKey()**](#haskey)
    - [**HasNoKey()**](#hasnokey)
    - [**HasAlternateKey()**](#hasalternatekey)
    - [**HasIndex()**](#hasindex)
    - [**Ignore()**](#ignore-1)
    - [**ComplexProperty()**](#complexproperty)
    - [**HasData()**](#hasdata)
    - [**ToView()**](#toview)
    - [**ToSqlQuery()**](#tosqlquery)
    - [**HasQueryFilter()**](#hasqueryfilter)
    - [**Property()**.](#property)
  - [Configuration - Fluent API - Property Configuration](#configuration---fluent-api---property-configuration)
    - [**HasConversion()**](#hasconversion)
    - [**HasColumnType()**](#hascolumntype)
    - [**HasComputedColumnSql()**](#hascomputedcolumnsql)
    - [**HasDefaultValue()**](#hasdefaultvalue)
    - [**HasDefaultValueSql()**](#hasdefaultvaluesql)
    - [**HasField()**](#hasfield)
    - [**HasMaxLength()**](#hasmaxlength)
    - [**HasPrecision()**](#hasprecision)
    - [**HasValueGenerator()**](#hasvaluegenerator)
    - [**IsConcurrencyToken()**](#isconcurrencytoken)
    - [**IsFixedLength()**](#isfixedlength)
    - [**IsRowVersion()**](#isrowversion)
    - [**IsRequired()**](#isrequired)
    - [**IsUnicode()**](#isunicode)
    - [**UseCollation()**](#usecollation)
    - [**ValueGeneratedNever()**](#valuegeneratednever)
    - [**ValueGeneratedOnAdd()**](#valuegeneratedonadd)
    - [**ValueGeneratedOnUpdate()**](#valuegeneratedonupdate)
    - [**ValueGeneratedOnAddOrUpdate()**](#valuegeneratedonaddorupdate)
  - [Relationships](#relationships)
    - [Relationship Mapping](#relationship-mapping)
    - [Relationship Types](#relationship-types)
  - [One-to-one Relationship](#one-to-one-relationship)
    - [The common case: both entities contain reference to each other](#the-common-case-both-entities-contain-reference-to-each-other)
    - [Principal without reference to dependent](#principal-without-reference-to-dependent)
    - [No references at all](#no-references-at-all)
    - [More one-to-one cases](#more-one-to-one-cases)
  - [One-to-many Relationship](#one-to-many-relationship)
    - [The common case: both entities contain reference to each other](#the-common-case-both-entities-contain-reference-to-each-other-1)
    - [Principal without reference to dependent collection](#principal-without-reference-to-dependent-collection)
    - [No references at all](#no-references-at-all-1)
    - [Dependent without reference to principal](#dependent-without-reference-to-principal)
    - [More one-to-many cases](#more-one-to-many-cases)
  - [Many-to-many Relationship](#many-to-many-relationship)
    - [The common case: without defining a join table](#the-common-case-without-defining-a-join-table)
    - [Defining a join table](#defining-a-join-table)
    - [More many-to-many cases](#more-many-to-many-cases)
  - [Loading Related Data](#loading-related-data)
    - [Eager Loading](#eager-loading)
    - [Explicit Loading](#explicit-loading)
    - [Lazy Loading](#lazy-loading)
      - [Lazy Loading using proxies](#lazy-loading-using-proxies)
      - [Lazy Loading without proxies](#lazy-loading-without-proxies)
  - [Migrations](#migrations)
    - [Visual Studio Migration Operations](#visual-studio-migration-operations)
    - [NET Core CLI Migration Operations](#net-core-cli-migration-operations)
    - [Migrations procedure](#migrations-procedure)
    - [Migration considerations](#migration-considerations)
    - [Easy way to explore Migrations with a Windows.Forms application](#easy-way-to-explore-migrations-with-a-windowsforms-application)
  - [Further Reading](#further-reading)


## Introduction

[Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) is an [Object Relational Mapping (ORM)](https://en.wikipedia.org/wiki/Object%E2%80%93relational_mapping) framework.

`EF Core` is open-source, lightweight, extensible and supports [a great number of databases](https://learn.microsoft.com/en-us/ef/core/providers/).

## The EF Core Model

In order to access data in a database with `EF Core` two elements are required

- a Data Context. A [DbContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext) derived class which represents a connection and a session with the database.
- Entities. Mostly [POCO](https://en.wikipedia.org/wiki/Plain_old_CLR_object) classes where each Entity maps to a database table.

In `EF Core` terminology these two elements, the Data Context and the Entities, along with their configuration, are known as [Model](https://learn.microsoft.com/en-us/ef/core/modeling/).

The `EF Core Model` is mainly a configuration on how entity types are mapped to tables of the underlying database.

The `DbContext` provides the [Model](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.model) property of type [IModel](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.imodel) which provides properties and methods that return information about the `EF Core Model`.

Also the [`DbSet<T>`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbset-1), which represents a collection of entities of the same type and is explained later in this text, provides the [EntityType](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbset-1.entitytype) property which provides `EF Core Model` information about the entity associated to the `DbSet<T>`.

## DbContext

[DbContext](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/) represents a session with the database, i.e. a [Transaction](https://en.wikipedia.org/wiki/Database_transaction) and implements the [Unit of Work](https://en.wikipedia.org/wiki/Unit_of_work) pattern.

`DbContext` provides `CRUD` methods such as `Add()`, `Update()` and `Remove()`.

All that `CRUD` methods are executed inside a transaction. The `DbContext.SaveChanges()` method [commits](https://en.wikipedia.org/wiki/Commit_(data_management)) the changes to the database.

`DbContext` is [IDisposable](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose). Its lifetime should be as short as possible.

### Configuring a DbContext Database Provider

A `DbContext` connects to a database using a single [Database Provider](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#configuring-the-database-provider).

The configuration of a `database provider` is done using a `DbContextOptionsBuilder.UseXXX()` extension method, where `XXX` is the provider name, such as the `UseSqlite()`.

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
        ...
    } 

    public DataContext()
        : this(new DbContextOptions<DataContext>())
    {
    }
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
}

```

For the required `UseXXX()` method to be available the proper [NuGet package](https://learn.microsoft.com/en-us/ef/core/providers) must be installed.

All these `UseXXX()` methods accept a connection string as a sole parameter.

### DbContext in Dependency Injection

`DbContext` can be used either as a regular class or as a service.

The `AddDbContext()` method is used in registering a `DbContext`. `AddDbContext()` registers `DbContext` as a [Scoped Service](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection).

```
builder.Services.AddDbContext<DataContext>();
```

There is a variation that accepts an `options` instance.

```
string ConnectionString = "...";

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(ConnectionString));
```

`DbContext` does not provide a public default constructor, i.e. a constructor without any parameter.

For a `DbContext` to be used in Dependency Injection a public constructor is required with a single `DbContextOptions` parameter. That constructor is used by Dependency Injection container when an instance of a `DbContext` is constructed.

```
public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
}
```

Having a constructor like that the `DbContext` can be injected anywhere Dependency Injection can be used.

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

`DbContext` can be used as a normal class too.

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

A call to `AddDbContextFactory()` is required. That call registers an [IDbContextFactory<DbContext>](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#use-a-dbcontext-factory) **scoped** service.

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

### Entities and the DbContext

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

Next is a `DbContext`.

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
- The `[Table(nameof(Product))]` attribute instructs `EF Core` to use a certain database table to map the entity. Attributes are explained later in this text.
- `DbContext` does not provide a public default constructor, i.e. a constructor without any parameter. The example provides a public default constructor.
- The `OnConfiguring()` method instructs the `EF Core` to use a [Sqlite](https://sqlite.org/) database.
- The line `context.Database.EnsureCreated()` of the `static public void EnsureDatabase()` method ensures that a database is created.
- The `ApplyConfigurationsFromAssembly()` call in the `OnModelCreating()` method forces `EF Core` to search a specified Assembly for entity classes and include them in its `Model`. This is done because our entity classes are accompanied by a `IEntityTypeConfiguration` interface implementation. The `ApplyConfigurationsFromAssembly()` searches the Assembly for types implementing the `IEntityTypeConfiguration` interface.
- The line `DemoData.AddData(modelBuilder)`, in the `OnModelCreating()` method, uses the static `DemoData` class to add initial data in the database.
- [DbContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext) is used for `CRUD` operations and is a generic repository too.
- [`DbSet<T>`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbset-1), like the `DbContext`, is used for `CRUD` operations and is a generic repository too.
- `DbContext` and `DbSet<T>` provide `CRUD` methods such as `Add()`, `Remove()` and `Update()`.
- `DbContext` provides the `SaveChanges()` method which [commits](https://en.wikipedia.org/wiki/Commit_(data_management)) the changes to the database.
- It is a good idea to review the documentation for both of these clasess and check the provided properties and methods.
- `DbContext` is **not** thread safe.
- `DbContext` is [IDisposable](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose). Its lifetime should be as short as possible.
- `DbContext` represents a session with the database, i.e. a [Transaction](https://en.wikipedia.org/wiki/Database_transaction).
- In other words `DbContext` implements the [Unit of Work](https://en.wikipedia.org/wiki/Unit_of_work) pattern.
- An `EF Core` application may call many times `Add()`, `Remove()` or `Update()` methods, against one or more entities, and then, in the end, call the `DbContext.SaveChanges()` method in order to [commit](https://en.wikipedia.org/wiki/Commit_(data_management)) the changes to the database.

## Saving Data

> There are `Async()` versions of most of the methods used in the next examples.

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

In `EF Core` queries are conducted through the `IQueryable<T>` interface implemented by `DbSet<T>`.

There is a great number of **very useful** `IQueryable<T>` extension methods in the [EntityFrameworkQueryableExtensions](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions) and it is **very important for a developer to explore it**.

> There are `Async()` versions of most of the methods used in the next examples.

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

### Paging

```
// a helper class for paging
public class Paging<T> where T : class
{
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
    public int PageIndex { get; set; }

    public List<T> List { get; set; }
}

...

var Paging = new Paging<Product>();
Paging.PageSize = 10;
Paging.PageSize = 0;

using (var Context = new DataContext())
{
    IQueryable<Product> Q = Context.Products;

    Paging.TotalItems = Q.Count();

    Q = Q.Skip(Paging.PageIndex * Paging.PageSize)
        .Take(Paging.PageSize);

    Paging.List = Q.ToList();
}
```

### Query result types

```
IQueryable<Product> Q1 = Context.Products;

IQueryable<Product> Q2 = Q1.Where(p => p.Price > 0.5M);

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
      .Where(p => p.Price > 0.5M)
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
public class ProductSimple
{
    public string Id { get; set; }
    public string Name { get; set; }
}

...
using (var Context = new DataContext())
{ 
    var List = Context.Products
        .Select(p => new ProductSimple() { p.Id, p.Name })
        .ToList();
}
```

> Projections limit the result size. There maybe entities with tenths of fields that not always needed.



## DbSet&lt;T&gt;

[DbSet&lt;T&gt;](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbset-1) represents a collection of entities of the same type. Provides `CRUD` methods such as `Add()`, `Update()` and `Remove()`. Also it is used in issuing queries to the database using [LINQ](https://learn.microsoft.com/en-us/dotnet/csharp/linq/) queries.

`DbSet<T>` is used in declaring properties in a `DbContext` representing collections of entities.

```
public class DataContext: DbContext
{
    ...
    public DbSet<Product> Products { get; set; }
}
```

Another way to get a `DbSet<T>` is to use the `DbContext.Set<T>()` method.

```
DbSet<Product> DbSet = DataContext.Set<Product>();
```

`DbSet<T>` is used in adding, updating and deleting an entity.

```
using (var Context = new DataContext())
{ 
    var P = new Product() { ... };

    Context.Products.Add(P);
    Context.Products.Update(P);
    Context.Products.Remove(P);

    Context.SaveChanges();
}
```

A `DbSet<T>` is also an `IEnumerable<T>` and `IQueryable<T>` instance.

```
using (var Context = new DataContext())
{ 
    var List = Context.Products
        .Select(p => new { p.Id, p.Name })
        .ToList(); 
}
```

`DbSet<T>` can execute [Sql statements](https://learn.microsoft.com/en-us/ef/core/querying/sql-queries).

`DbSet<T>` provides a lot of useful methods, such as [FromSql](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.relationalqueryableextensions.fromsql) and [FromSqlRaw](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.relationalqueryableextensions.fromsqlraw).

```
using (var Context = new DataContext())
{ 
    string SqlText = "select * from Products"
    var List = Context.Products
        .FromSqlRaw(SqlText)
        .ToList(); 
}
```


## Change Tracking

`DbContext` uses [Change Tracking](https://learn.microsoft.com/en-us/ef/core/change-tracking/), that is it tracks changes made to entities.

An entity is tracked when

- it is returned from a query, e.g. `Context.Products.FirstOrDefault(x => x.Name == "Laptop")`
- the `Add()`, `Update()` or `Attach()` methods of the `DbContext` or `DbSet` are used with an entity.

The `DbContext.SaveChanges()` detects any changes made to an entity and updates the underlying database.

### The EntityEntry

The `DbContext.Entry()` and the `DbSet.Entry()` methods return an [EntityEntry](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.changetracking.entityentry) instance which provides change tracking information regarding a specified entity.

The `EntityEntry.State` **read-write** property is an [EntityState](https://learn.microsoft.com/en-us/ef/core/change-tracking/#entity-states) enum type indicating the `tracking` state of an entity.

### The ChangeTracker

The [DbContext.ChangeTracker](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.changetracker) property, is an instance of [ChangeTracker](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.changetracking.changetracker), which provides methods and properties to configure `change tracking`.

### Disabling Change Tracking

In `EF Core`, by default, entities returned from queries are [tracked entities](https://learn.microsoft.com/en-us/ef/core/querying/tracking).

Change tracking can be disabled

 ● in the `DbContext.OnConfiguring()` method in order to affect any `DbContext` instance

```
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    string DatabasePath = Path.Combine(System.AppContext.BaseDirectory, "Sqlite.db3");
  
    optionsBuilder.UseSqlite($"Data Source={DatabasePath}", SqliteOptionsBuilder => { })
      .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
}
```

 ● using the `DbContext.ChangeTracker.QueryTrackingBehavior` or setting the `DbContext.ChangeTracker.AutoDetectChangesEnabled` to **false**, in order to affect every query in a certain `DbContext`

```
DataContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

DataContext.ChangeTracker.AutoDetectChangesEnabled = false;
```

 ● using the `AsNoTracking()` method in order to affect a certain query

```
DataContext.Products.AsNoTracking(); 
```

 ● using the `DbContext.ChangeTracker.Clear()` method to clear all change tracking information from a `DbContext`

```
DataContext.ChangeTracker.Clear(); 
```

 ● using the `EntityEntry.State` property to detach an entity

```
var Entry = Context.Entry(product);
Entry.State = EntityState.Detached;
```

## Configuration

The `EF Core Model` is mainly a configuration on how entity types are mapped to tables of the underlying database.

Configuration is done

- with `EF Core` built-in configuration conventions
- using Data Annotation attributes
- using Fluent API syntax in `DbContext.OnModelCreating()` or `IEntityTypeConfiguration<T>.Configure()`.

## Configuration - built-in Conventions

In configuring entities the `EF Core` uses a lot of built-in [conventions](https://learn.microsoft.com/en-us/ef/core/modeling/#built-in-conventions) in order to spare a lot of work from the developer.

Most common are the following.

- **Table Name**. The name of an entity class is used in mapping the entity to a table with the same name in the underlying database.
- **Column Name**. The name of a property is used in mapping the property to a table column with the same name in the underlying database.
- **Primary Key**. A property named `ID`, `Id`, `id` or `EntityNameId` (not case-sensitive), is configured as the primary key.
- **Foreign Key**. If a property is named `ForeignEntityId` and there is another entity having `ForeignName` as name and a primary key of the same data type then that property is configured as a foreign key.
- **Alternate Key**. When a property, which is not the primary key, is used as the target of a relationship then an [alternate key](https://learn.microsoft.com/en-us/ef/core/modeling/keys#alternate-keys) is created.
- **Data Types**. The Database Provider decides the appropriate mapping.
- **Nullable Types**. Properties with nullable data types can be null, e.g. `string? Name { get; set; }`, otherwise a value is required, e.g. `string Name { get; set; }`.

## Configuration - Data Annotation Attributes

Attributes can be placed on a class or property to specify metadata about that class or property.

Data annotation attributes modify or override configuration which is imposed by `EF Core` built-in conventions.

There is a great number of data annotation attributes. Some of them are `mapping` attributes while others are `validation` attributes.

Data annotation attributes can be found in many namespaces such as [System.ComponentModel.DataAnnotations](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations),  [System.ComponentModel.DataAnnotations.Schema](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema) or [Microsoft.EntityFrameworkCore](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore).

Next is a list of frequently used attributes. 

The presented list is not exhaustive.

### [**TableAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema.tableattribute). 
Annotates an entity class specifying the database table the entity is mapped to.

```
[Table(nameof(Product))]
public class Product : BaseEntity
{
    ...
}
```

### [**PrimaryKeyAttribute**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.primarykeyattribute). 
Annotates an entity class specifying the primary key which can be comprised of a single property or multiple properties when compound keys are used.

```
[PrimaryKey(nameof(Id))]
public class BaseEntity 
{
    public string Id { get; set; }
}

...

[PrimaryKey(nameof(RoleId), nameof(PermissionId))]
public class AppRolePermission  
{
    public AppRolePermission() 
    { 
    }
 
    public string RoleId { get; set; }
    public string PermissionId { get; set; }
}
```

### [**IndexAttribute**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.indexattribute). 

Annotates an entity class specifying an index that should be generated in the database.

```
[Index(nameof(ProductId), nameof(MeasureUnitId), IsUnique = true)]
public class ProductMeasureUnit : BaseEntity
{
    public string ProductId { get; set; }
    public string MeasureUnitId { get; set; }
    ...
}
```

### [**KeyAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.keyattribute). 

Annotates a property specifying the primary key.

```
public class BaseEntity 
{
    [Key]
    public string Id { get; set; }
}
```

### [**ForeignKeyAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema.foreignkeyattribute). 

Annotates a property denoting that the property is used as a foreign key in a relationship between two entites.

> In `EF Core` terminology the `foreign` entity is called `Principal` entity while the entity that depends on the principal entity is called `Dependent` entity.

The `ForeignKey` attribute accepts a single string parameter which is the name either of an associated navigation property (i.e. a property of an entity type) or the name of a property of a primitive type which is the actual foreign key.

In other words, in `EF Core`, in order to define a foreign key two things are required:

- an associated navigation property of an entity type, as the `Category` in the next example
- a property of a primitive type which is the actual foreign key, as the `CategoryId` in the next example.

```
public class Product : BaseEntity
{
    public Product() { }

    public string Name { get; set; }
    public decimal Price { get; set; }

    public string CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public virtual Category Category { get; set; }
}
```

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

### [**KeylessAttibute**](https://learn.microsoft.com/en-us/ef/core/modeling/keyless-entity-types). 

Annotates an entity specifying that the entity has no primary key  at all. Can be used to execute database queries returning keyless entities.

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

### [**ColumnAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema.columnattribute). 

Annotates a property. Can be used to specify the column name in the database or the database provider specific data type of the column or both.

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

### [**RequiredAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.requiredattribute). 

Annotates a property specifying that a value is required, i.e. it cannot be **null**.

```
public class MeasureUnit: BaseEntity 
{
    [Required]
    public string Name { get; set; }
    ...
}
```

### [**MaxLengthAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.maxlengthattribute). 

Annotates a property specifying the maximum allowed length of array or string data. There is `MinLengthAttribute` counterpart too.

```
public class BaseEntity 
{
    [MaxLength(40)]
    public string Id { get; set; }
    ...
}
```

### [**StringLengthAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.stringlengthattribute). 

Annotates a string property specifying min and max length at once.

```
public class Product
{
    [StringLength(128, MinimumLength = 4)]
    public string Name { get; set; } 
    ...
}
```

### [**RangeAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.rangeattribute). 

Annotates a numeric property specifying a range of valid values.

```
public class Product : BaseEntity
{      
    [Range(0, 50000)]
    public decimal Price { get; set; }
}
```

### [**PrecisionAttribute**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.precisionattribute). 

Annotates a decimal or double property specifying the precision of data.

```
public class Product : BaseEntity
{
    [Precision(18, 4)]
    public decimal Price { get; set; }
    ...
}
```

### [**UnicodeAttribute**](https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties?tabs=data-annotations%2Cwith-nrt#unicode). 

Annotates a string property enabling or disabling the storing of data as unicode. Has no effect for database providers that do not support this feature.

```
public class Product : BaseEntity
{
    [Unicode(true)]
    public string Name { get; set; }
    ...
}
```

### [**CommentAttribute**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.commentattribute). 

Annotates a property specifying a comment about the column. For database providers that support this feature.

```
public class Product : BaseEntity
{
    [Comment("Product name in the default language, i.e. english.")]
    public string Name { get; set; }
    ...
}
```

### [**DefaultValueAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.defaultvalueattribute). 

Annotates a property specifying a default value.

```
public class Category : BaseEntity 
{
    [DefaultValue(false)]
    public bool IsPublic { get; set; } 
}
```

### [**DatabaseGeneratedAttribute**](https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=data-annotations#explicitly-configuring-value-generation). 

Annotates a property that specifies whether and how the database provider generates values ​​for the property.

```
public class BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; }
    ...
}
```

### [**TimestampAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.timestampattribute). 

Annotates a byte array , i.e. `byte[]`, property. Can only be applied once per entity. Used when the database provider is the MsSql server.

```
public class Product : BaseEntity
{      
    [Timestamp]
    public byte[] RowTimestamp { get; set; }
}
```

### [**NotMappedAttribute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema.notmappedattribute). 

Annotates a property specifying that it should be not mapped to a database column.

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

### [**InversePropertyAttribute**](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/mapping-attributes#inversepropertyattribute)

When an entity has more than one relational properties to the same type then `EF Core` cannot automatically detect the right relationship. 

The `InversePropertyAttribute` attibute is used to address this situation.

Following is the example taken from the official documentation.

```
public class Blog
{
    public int Id { get; set; }

    [InverseProperty("Blog")]
    public List<Post> Posts { get; } = new();

    public int FeaturedPostId { get; set; }
    public Post FeaturedPost { get; set; }
}

public class Post
{
    public int Id { get; set; }
    public int BlogId { get; set; }

    public Blog Blog { get; init; }
}
```

### [**DeleteBehaviorAttribute**](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/mapping-attributes#deletebehaviorattribute)

The [DeleteBehavior](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.deletebehavior) enumeration type indicates what happens to dependent entities when the [principal entity is deleted](https://learn.microsoft.com/en-us/ef/core/saving/cascade-delete).

`EF Core` by convention uses  
- the `DeleteBehavior.ClientSetNull` for optional relationships and
- the `DeleteBehavior.Cascade` for required relationships.

The `DeleteBehaviorAttribute` attribute can be used to change this default behavior.

```
// Principal
public class Driver 
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Car Car { get; set; }
}

// Dependent
public class Car  
{
    public string Id { get; set; }
    public string PlateNumber { get; set; }

    public string DriverId { get; set; }

    [DeleteBehavior(DeleteBehavior.ClientCascade)]
    public Driver Driver { get; set; }
}

``` 

### [**ComplexTypeAttibute**](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema.complextypeattribute). 

Specifies that a type is a [complex type](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/whatsnew#value-objects-using-complex-types).

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
- it is not identified by a key value, i.e. is keyless, and it is not tracked
- should be used as a property of an entity type
- it is not autonomously saved by a `DbContext` or a `DbSet`, but instead is saved as a part of its container entity
- can be a reference or a value type, i.e. either class or struct
- can be shared by multiple properties in the same entity.
- can be used by multipte entities
- must be defined as a **required** value in the `OnModelCreating()` method.


### [**OwnedAttribute**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.ownedattribute)

Specifies that an entity is an [owned](https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities) one.

An owned entity is not an autonomous entity and it is only used as a navigation property contained by another entity which is called the _owner_ entity.

An owned entity cannot exist without its container _owner_ entity.

```
[Owned]
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

## Configuration - Fluent API

> Configuration using Fluent API syntax **modifies or overrides** configuration which is imposed by `EF Core` built-in conventions or added using `Data Annotation` attributes.

Fluent API methods reside in the [Microsoft.EntityFrameworkCore.Metadata.Builders](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders) as members in various `builder` classes.

Configuration using Fluent API syntax can be done in `IEntityTypeConfiguration<T>.Configure()` or `DbContext.OnModelCreating()`.

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

The `ApplyConfigurationsFromAssembly()` instructs `EF Core` to search an Assembly for types implementing the `IEntityTypeConfiguration` interface and include entities and entity configurations in the `EF Core Model`.

It's a matter of preference but having an implementation of `IEntityTypeConfiguration` interface leads to less code in the `DbContext.OnModelCreating()` and easy management of entity configuration.

The other way is to use the `DbContext.OnModelCreating()` in entity configuration.

```
public class DataContext: DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ...
    } 
}
```

## Configuration - Fluent API - Model Configuration

The following methods are members of the [ModelBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.modelbuilder) class. 

The presented list is not exhaustive.

### [**Entity()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.modelbuilder.entity)

Performs configuration of a specified entity type.

```
modelBuilder.Entity<Product>().HasKey(p => p.Id);
```

### [**Owned()**](https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities)

Specifies that an entity is an [owned](https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities) one.

An owned entity is not an autonomous entity and it is only used as a navigation property contained by another entity which is called the _owner_ entity.

An owned entity cannot exist without its container _owner_ entity.

```
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

```
modelBuilder.Entity<SalesOrder>().OwnsOne(p => p.ShippingAddress);
modelBuilder.Entity<SalesOrder>().OwnsOne(p => p.BillingAddress);
```

### [**HasDbFunction()**](https://learn.microsoft.com/en-us/ef/core/querying/user-defined-function-mapping)

Maps a CLR function to a database [UDF](https://learn.microsoft.com/en-us/sql/relational-databases/user-defined-functions/user-defined-functions) function.

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

### [**HasDefaultSchema()**](https://learn.microsoft.com/en-us/ef/core/modeling/entity-types#table-schema)

Specifies the default database schema.

```
modelBuilder.HasDefaultSchema("dbo");
```

### [**HasSequence()**](https://learn.microsoft.com/en-us/ef/core/modeling/sequences)

Configures a database sequence. Valid with relational databases that support sequences.

```
modelBuilder.HasSequence<int>("SalesOrderCodeNo");

modelBuilder.Entity<SalesOrder>()
    .Property(o => o.OrderNo)
    .HasDefaultValueSql("NEXT VALUE FOR SalesOrderCodeNo");
```

### [**HasAnnotation()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.modelbuilder.hasannotation)

Creates a `Key-Value` pair object that attaches arbitrary information to the `Model` which can later be read from the `Model` using its key. `EF Core` uses `HasAnnotation()` internally to configure things such as the constraint name of a foreign key. It has little value to a developer unless he implements something that uses it. Check [this discussion](https://github.com/dotnet/efcore/issues/13028).

```
modelBuilder.HasAnnotation("MyKey", "MyValue");
```

### [**HasChangeTrackingStrategy**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.modelbuilder.haschangetrackingstrategy)

Specifies the [ChangeTrackingStrategy](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.changetrackingstrategy) to be used with this `EF Core Model`. A `ChangeTrackingStrategy` indicates how the `DbContext` detects changes happened in entity properties.

```
modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
```

### [**Ignore()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.modelbuilder.ignore)

When used with a [ModelBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.modelbuilder) specifies that an entity is not-mapped to a database table.

```
modelBuilder.Ignore("ProductOrdersTotal");

modelBuilder.Ignore(typeof(ProductOrdersTotal));

modelBuilder.Ignore<ProductOrdersTotal>();
```

## Configuration - Fluent API - Entity Configuration

The following methods are members of the [EntityTypeBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1) class. 

The presented list is not exhaustive.

### [**ToTable()**](https://learn.microsoft.com/en-us/dotnet/api/system.data.entity.modelconfiguration.entitytypeconfiguration-1.totable)

Specifies the name of a database table that the entity maps to.

```
modelBuilder.Entity<SalesOrder>().ToTable("Sales_Order");
```

### [**HasKey()**](https://learn.microsoft.com/en-us/ef/core/modeling/keys#configuring-a-primary-key)

Configures one or more properties as the primary key.

```
modelBuilder.Entity<Product>().HasKey(p => p.Id);
modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
```

### [**HasNoKey()**](https://learn.microsoft.com/en-us/ef/core/modeling/keyless-entity-types)
  
  Annotates an entity specifying that the entity has no primary key. Can be used to execute database queries returning keyless entities.

```
modelBuilder.Entity<ProductOrdersTotal>().HasNoKey();
```

### [**HasAlternateKey()**](https://learn.microsoft.com/en-us/ef/core/modeling/keys#alternate-keys)

Configures an [alternate key](https://learn.microsoft.com/en-us/ef/core/modeling/keys#alternate-keys) for an entity. An alternate key, just like a primary key, uniquely identifies an entity. It can be a multi-column key and it is useful in establishing relationships between entities.

```
modelBuilder.Entity<Company>().HasAlternateKey(c => c.TaxPayerId);  // a TIN
modelBuilder.Entity<Car>().HasAlternateKey(c => new { c.PlateNo, c.ChassisNo });
```

### [**HasIndex()**](https://learn.microsoft.com/en-us/ef/core/modeling/indexes)

Creates an index on one or more properties. The index can be a unique one.

```
modelBuilder.Entity<Product>().HasIndex(p => p.Name).IsUnique(true);
modelBuilder.Entity<User>().HasIndex(u => new { u.FirstName, u.LastName });
```

### [**Ignore()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1.ignore)

When used with an [EntityTypeBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1) specifies one or more properties that are excluded from mapping.

```
modelBuilder.Entity<User>().Ignore(u => u.FullName);
```

### [**ComplexProperty()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1.complexproperty)

Specifies that a property of a class or structure type is a [Complex Type](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/whatsnew#value-objects-using-complex-types). 

Check the `ComplexTypeAttribute` presented earlier.

```
modelBuilder.Entity<SalesOrder>(so => {
		so.ComplexProperty(sa => sa.ShippingAddress, sa => { sa.IsRequired(); });
		so.ComplexProperty(ba => ba.BillingAddress, ba => { ba.IsRequired(); });
	});
```

### [**HasData()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1.hasdata)

Used in adding initial data to the database.

```
modelBuilder.Entity<Product>().HasData(new List<Product>()
{
    new Product(){ ... }
    ...
});
```

### [**ToView()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.relationalentitytypebuilderextensions.toview)

Specifies that an entity maps to a database view.

```
modelBuilder.Entity<ProductOrdersTotal>().ToView("v_product_salesorders_total");
```

### [**ToSqlQuery()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.relationalentitytypebuilderextensions.tosqlquery)

Maps an entity to a `SELECT` Sql statement.

```
modelBuilder.Entity<ProductOrdersTotal>(builder => { builder.ToSqlQuery("select * from v_product_salesorders_total"); });

...

// example usage
var Totals = MyDataContext.Set<ProductOrdersTotal>().ToList();
```

### [**HasQueryFilter()**](https://learn.microsoft.com/en-us/ef/core/querying/filters)

Specifies that the entity has a [global query filter](https://learn.microsoft.com/en-us/ef/core/querying/filters) that should be automatically applied to queries of this entity type.

```
modelBuilder.Entity<User>().HasQueryFilter(u => u.UserType == "ClientApplication");
```

### [**Property()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1.property). 

Returns a [PropertyBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1) which is use in configuring a property of the entity.

```
modelBuilder.Entity<User>().Property(u => u.Salt).HasColumnName("PasswordSalt");
```

## Configuration - Fluent API - Property Configuration

The following methods are members of the [PropertyBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1) class. 

The presented list is not exhaustive.

### [**HasConversion()**](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions)

Specifies a [ValueConverter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.storage.valueconversion.valueconverter) to be used in converting the value when reading or writing to the database.

```
modelBuilder.Entity<User>().Property(u => u.UserType)
    .HasConversion(
        v => v.ToString(),
        v => (UserTypeEnum)Enum.Parse(typeof(UserTypeEnum), v));
```

### [**HasColumnType()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.relationalpropertybuilderextensions.hascolumntype)	

Specifies a database provider specific data type of the column that maps the property.

```
modelBuilder.Entity<Product>().Property(p => p.Price)
    .HasColumnType("decimal(18, 4)");
```

### [**HasComputedColumnSql()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.relationalpropertybuilderextensions.hascomputedcolumnsql)

Specifies that the property maps to [computed column](https://en.wikipedia.org/wiki/Virtual_column) in the database.

`HasComputedColumnSql()` accepts a string parameter with the expression used to generate the computed value for the database column.

It also accepts a boolean parameter, as the second one, which specifies if the value should be stored in the database like a regular column.
 
```
modelBuilder.Entity<User>().Property(u => u.FullName)
    .HasComputedColumnSql("concat(FirstName, ' ', LastName)", false);
```


### [**HasDefaultValue()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.relationalpropertybuilderextensions.hasdefaultvalue)

Specifies a default value for the database column the property maps to.

```
modelBuilder.Entity<User>().Propery(u => u.IsActive)
    .HasDefaultValue(true);
```

 
### [**HasDefaultValueSql()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.relationalpropertybuilderextensions.hasdefaultvaluesql)

Specifies a Sql expression which generates a default value for the database column the property maps to.

```
modelBuilder.Entity<User>().Property(u => u.FullName)
    .HasDefaultValueSql("concat(FirstName, ' ', LastName)");
```
 

### [**HasField()**](https://learn.microsoft.com/en-us/ef/core/modeling/backing-field)

Specifies a [backing field](https://learn.microsoft.com/en-us/ef/core/modeling/backing-field) that should be used be `EF Core` when reading or writing, instead of the property.

```
public class User: BaseEntity
{
    bool blocked;

    public string UserName { get; set; }
    public string Password { get; set; }
    public string PasswordSalt { get; set; }
    public string Name { get; set; }
    public AppUserType UserType { get; set; }
    public bool IsBlocked 
    {
        get { return blocked; }
        set { blocked = value; }
    }
}

...

modelBuilder.Entity<User>().Property(u => u.IsBlocked)
    .HasField("blocked");
```
### [**HasMaxLength()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1.hasmaxlength)

Specifies the maximum length of data that can be stored in a string or array property.
 
```
modelBuilder.Entity<Product>().Property(p => p.Name)
    .HasMaxLength(96);
```

### [**HasPrecision()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1.hasprecision)

Specifies the precision and scale of a property. 

```
modelBuilder.Entity<Product>().Property(p => p.Price)
    .HasPrecision(14, 2);
```

### [**HasValueGenerator()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1.hasvaluegenerator)

Specifies a custom [ValueGenerator](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.valuegeneration.valuegenerator) class which provides values for a property.

```
public class SalesOrderCodeNoValueGenerator : ValueGenerator<string>
{        
    public override string Next(EntityEntry entry)
    {
        var Service = entry.Context.GetService<SalesOrderService>();
        return Service.NextCodeNoValue();
    }

    public override bool GeneratesTemporaryValues => false;
}

...

modelBuilder.Entity<SalesOrder>().Property(so => so.OrderNo)
    .HasValueGenerator<SalesOrderCodeNoValueGenerator>()
    .IsRequired();
```

### [**IsConcurrencyToken()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1.isconcurrencytoken)

Specifies that a property is a [concurrency token](https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=fluent-api#application-managed-concurrency-tokens). 

A property configured as a concurrency token is a kind of [record lock](https://en.wikipedia.org/wiki/Record_locking). 

The database column value, of the mapped property, is checked when the entity is updated or deleted, using the `DbContext.SaveChanges()` method. 

If the database column value has changed, since the entity was retrieved from the database, an exception is thrown and no changes are applied to the database.

```
public class SalesOrder
{
    public byte[] RowVersion { get; set; }
    ...
}

...

modelBuilder.Entity<SalesOrder>().Property(so => so.RowVersion)
    .IsConcurrencyToken();
```

### [**IsFixedLength()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.relationalpropertybuilderextensions.isfixedlength)

Specifies that the mapped column can store fixed-length data only, such as strings.

```
modelBuilder.Entity<Country>().Property(c => c.TwoDigitCode)
    .HasMaxLength(2)
    .IsFixedLength();
```
 
### [**IsRowVersion()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1.isrowversion)

Specifies that a property maps a column that is a [row version](https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=fluent-api#native-database-generated-concurrency-tokens).

Not all database providers support this feature.

```
public class SalesOrder
{
    public byte[] RowVersion { get; set; }
    ...
}

...

modelBuilder.Entity<SalesOrder>().Property(so => so.RowVersion)
    .IsRowVersion();
```

### [**IsRequired()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1.isrequired)	

Specifies whether a property is required to have a value and not be null.

```
modelBuilder.Entity<User>().Property(u => u.UserName)
    .IsRequired();

modelBuilder.Entity<User>().Property(u => u.FullName)
    .IsRequired(false);
```

### [**IsUnicode()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1.isunicode)

Specifies whether a string property maps to a unicode column.

```
modelBuilder.Entity<User>().Property(u => u.UserName)
    .IsUnicode(true);
```

### [**UseCollation()**](https://learn.microsoft.com/en-us/ef/core/miscellaneous/collations-and-case-sensitivity#column-collation)

Specifies a collation to be used with the column the property maps to.

```
modelBuilder.Entity<Customer>().Property(c => c.Name)
    .UseCollation("Greek_CI_AI");
```
 
### [**ValueGeneratedNever()**](https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=fluent-api#no-value-generation)

Specifies that no value should be generated by the database for the column the property maps to, when the entity is saved.
 

```
modelBuilder.Entity<Product>().Property(p => p.Id)
    .ValueGeneratedNever();
``` 
### [**ValueGeneratedOnAdd()**](https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=fluent-api#explicitly-configuring-value-generation)

Specifies that a value is generated by the database for the column the property maps to, when the entity is inserted.

```
modelBuilder.Entity<SalesOrder>().Property(o => o.DateCreated)
    .ValueGeneratedOnAdd();
```

### [**ValueGeneratedOnUpdate()**](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1.valuegeneratedonupdate)

Specifies that a value is generated by the database for the column the property maps to, when the entity is updated.

```
modelBuilder.Entity<SalesOrder>().Property(o => o.DateUpdated)
    .ValueGeneratedOnUpdate();
```

### [**ValueGeneratedOnAddOrUpdate()**](https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=fluent-api#explicitly-configuring-value-generation)

Specifies that a value is generated by the database for the column the property maps to, when the entity is inserted or updated.

```
modelBuilder.Entity<SalesOrder>().Property(o => o.DateUpdated)
    .ValueGeneratedOnAddOrUpdate();
```

## Relationships

`Relationship` is a term denoting two entities related to each other.

In the next example `Product` and `Category` form a relationship.

```
// Principal
public class Category 
{
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
}

// Dependent
public class Product
{
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public string CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; }
}
```

The `Product` **has** a `Category`.

A relationship can be expressed in the opposite direction too.

The `Category` **has** a collection of `Products`.

```
public class Category  
{
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }

    public ICollection<Product> Products { get; }
}
```

### Relationship Mapping

In `EF Core` relationship mapping is done by mapping primary keys and foreign keys in the underlying relational database.
 
To form a relationship the following are required

- a primary key property in one entity, the **Principal** entity
- a foreign key property in another entity, the **Dependent** entity
- a configuration that establishes the relationship between the two.

In the previous example both entities configure a primary key, using the `Key` attribute.

Also the `Product` entity configures a foreign key using the `ForeignKey` attribute. 

That relationship can be expressed using Fluent API too and omitting the attributes.

```
modelBuilder.Entity<Category>()
    .HasMany(c => c.Products)
    .WithOne(p => p.Category)
    .HasForeignKey(p => p.CategoryId)
    .HasPrincipalKey(c => c.Id);
```

### Relationship Types

The following relationship types are supported.

- **One-to-one**. A single entity is associated with another single entity.
- **One-to-many**. A single entity is associated with any number of other entities.
- **Many-to-many**. Any number of entities are associated with any number of other entities.

## One-to-one Relationship

A single entity is associated with another single entity.

In an one-to-one relationship  

- a primary key is required in the principal entity, e.g. `Driver.Id`
- a foreign key is required in the dependent entity, e.g. `Car.DriverId`
- optionally the principal entity may have a reference to the dependent entity, e.g. `Driver.Car`
- optionally the dependent entity may have a reference to the principal entity, e.g. `Car.Driver`

### The common case: both entities contain reference to each other
The most common case is 
- to have the dependent entity having a foreign key property, e.g. `Car.DriverId`
- to have the dependent entity having a reference to the principal entity, e.g. `Car.Driver`
- to optionally have the principal entity having a reference to the dependent entity, e.g. `Driver.Car`

Consider the next example.

```
// Principal
public class Driver 
{
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    public Car Car { get; set; }
}

// Dependent
public class Car  
{
    [Key]
    public string Id { get; set; }
    public string PlateNumber { get; set; }

    public string DriverId { get; set; }

    [ForeignKey(nameof(DriverId))]
    public Driver Driver { get; set; }
}

public class DriverEntityTypeConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        // nothing here
    }
}

public class CarEntityTypeConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        // nothing here
    }
}

```
`EF Core` will discover this relationship using configuration convention. It will discover the relationship even if the `Data Annotation` attributes are omitted.
 
This relationship can also be expressed using Fluent API in which case the data annotation attributes are not required.

```
// Principal
public class Driver 
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Car Car { get; set; }
}

// Dependent
public class Car  
{
    public string Id { get; set; }
    public string PlateNumber { get; set; }

    public string DriverId { get; set; }
    public Driver Driver { get; set; }
}
```

Starting configuration with the principal entity.

```
modelBuilder.Entity<Driver>()
    .HasOne(d => d.Car)
    .WithOne(c => c.Driver)
    .HasForeignKey<Car>(c => c.DriverId)
    .IsRequired();
```

Starting configuration with the dependent entity.

 ```
modelBuilder.Entity<Car>()
    .HasOne(c => c.Driver)
    .WithOne(d => d.Car)
    .HasForeignKey<Car>(c => c.DriverId)
    .IsRequired();
 ```

Both approaches result in exactly the same configuration.

The `Car.DriverId` is required to have a value.

As it is now, in the example, the `Car.DriverId` foreign key **is required to have a valid value**. It can be configured as optional using either `IsRequired(false)` or using a nullable string, e.g.  

 ```public string DriverId? { get; set; }```



### Principal without reference to dependent

A usual case is to omit the relational property in the principal entity.

```
// Principal
public class Driver
{
    public string Id { get; set; }
    public string Name { get; set; }
}

// Dependent
public class Car
{
    public string Id { get; set; }
    public string PlateNumber { get; set; }

    public string DriverId { get; set; }
    public Driver Driver { get; set; }
}
```

`EF Core` configuration convention logic will detect the above relationship even without the Fluent configuration.

The Fluent configuration for the above starts with the dependent entity and the `WithOne()` method is called without arguments indicating that there is no navigation in this direction, i.e. the `Driver` has no reference to a `Car`.

```
modelBuilder.Entity<Car>()
    .HasOne(c => c.Driver)
    .WithOne();
``` 

### No references at all

Another usual case is to omit the relational properties in both entities altogether. 

```
// Principal 
public class Driver
{
    public string Id { get; set; }
}

// Dependent  
public class Car
{
    public string Id { get; set; }
    public string DriverId { get; set; } // Required foreign key property
}
```

The above cannot be detected by configuration conventions. A Fluent configuration is required.

```
modelBuilder.Entity<Driver>()
    .HasOne<Car>()
    .WithOne();
```

### More one-to-one cases
 
Except of the most common cases as depicted above there are many other cases, actually variations, described in the [docs](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-one).

## One-to-many Relationship

A single entity is associated with any number of other entities.

In an one-to-many relationship  

- a primary key is required in the principal entity, e.g. `Driver.Id`
- a foreign key is required in the dependent entity, e.g. `Car.DriverId`
- optionally the principal entity may have a navigation property to a collection of dependent entities, e.g. `Driver.Cars`
- optionally the dependent entity may have a reference to the principal entity, e.g. `Car.Driver`

### The common case: both entities contain reference to each other

The most common case is 
- to have the dependent entity having a foreign key property, e.g. `Car.DriverId`
- to have the dependent entity having a reference to the principal entity, e.g. `Car.Driver`
- to optionally have the principal entity having a navigation property to a collection of dependent entities, e.g. `Driver.Cars`

Consider the next example.

```
// Principal
public class Driver 
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ICollection<Car> Cars { get; set; }
}

// Dependent
public class Car  
{
    public string Id { get; set; }
    public string PlateNumber { get; set; }

    public string DriverId { get; set; }
    public Driver Driver { get; set; }
}

public class DriverEntityTypeConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        // nothing here
    }
}

public class CarEntityTypeConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        // nothing here
    }
}
```

`EF Core` will discover this relationship using configuration convention.  

The relationship can be expressed using Fluent API.

Starting configuration with the principal entity.

```
modelBuilder.Entity<Driver>()
    .HasMany(d => d.Cars)
    .WithOne(c => c.Driver)
    .HasForeignKey(c => c.DriverId)
    .IsRequired();
```

Starting configuration with the dependent entity.

Watch the interchange of the `Has/With` methods in the two examples.

```
modelBuilder.Entity<Car>()
    .HasOne(c => c.Driver)
    .WithMany(d => d.Cars)
    .HasForeignKey(c => c.DriverId)
    .IsRequired();   
```
 
Both approaches result in exactly the same configuration.

The `Car.DriverId` is required to have a value.

As it is now, in the example, the `Car.DriverId` foreign key **is required to have a valid value**. It can be configured as optional using either `IsRequired(false)` or using a nullable string, e.g.  

 ```public string DriverId? { get; set; }``` 

### Principal without reference to dependent collection

A usual case is to omit the navigation property to the collection of dependent entities in the principal entity.

```
// Principal
public class Driver 
{
    public string Id { get; set; }
    public string Name { get; set; }
}

// Dependent
public class Car  
{
    public string Id { get; set; }
    public string PlateNumber { get; set; }

    public string DriverId { get; set; }
    public Driver Driver { get; set; }
}
```
`EF Core` configuration convention logic will detect the above relationship even without the Fluent configuration.

The Fluent configuration for the above starts with the dependent entity and the `WithMany()` method is called without arguments indicating that there is no navigation in this direction, i.e. the `Driver` has no reference to a `Cars` collection.

```
modelBuilder.Entity<Car>()
    .HasOne(c => c.Driver)
    .WithMany()
    .HasForeignKey(c => c.DriverId)
    .IsRequired();
```

### No references at all

Another usual case is to omit the relational properties in both entities altogether. 

```
// Principal
public class Driver 
{
    public string Id { get; set; }
    public string Name { get; set; }
}

// Dependent
public class Car  
{
    public string Id { get; set; }
    public string PlateNumber { get; set; }

    public string DriverId { get; set; }
}
```

The above cannot be detected by configuration conventions. A Fluent configuration is required.

```
modelBuilder.Entity<Driver>()
    .HasMany<Car>()
    .WithOne();
```

### Dependent without reference to principal

A usual case is to omit the relational property in the dependent entity.

```
// Principal
public class Driver 
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ICollection<Car> Cars { get; set; }
}

// Dependent
public class Car  
{
    public string Id { get; set; }
    public string PlateNumber { get; set; }

    public string DriverId { get; set; }
}
```

`EF Core` will discover this relationship using configuration convention.  

Here is the `Fluent API` configuration for the above case.

```
modelBuilder.Entity<Driver>()
    .HasMany(d => d.Cars)
    .WithOne()
    .HasForeignKey(c => c.DriverId)
    .IsRequired();
```

### More one-to-many cases
 
Except of the most common cases as depicted above there are many other cases, actually variations, described in the [docs](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-many).

## Many-to-many Relationship

Any number of entities are associated with any number of other entities.

A `many-to-many` relationship can be thought of as a pair of one-to-many relationships.

A `many-to-many` relationship is a lot different from an `one-to-one` and an `one-to-many` relationship. 

A `many-to-many` relationship between two entities requires a third entity, i.e. a third database table. That table or _entity-in-the-middle_ contains references, i.e. foreign keys, to both entities of the relationship.

That _table-in-the-middle_ is known with many names in the literature

- associative table
- join table
- junction table
- cross-reference table
- [correlation table](https://teonotebook.wordpress.com/2025/02/01/databases-remarks-and-thoughts/).

Here is such one relationship: a driver can be responsible for many cars and a car can have many drivers.

```
// Principal
public class Driver
{
    public string Id { get; set; }
    public string Name { get; set; }
}

// Principal
public class Car
{
    public string Id { get; set; }
    public string PlateNumber { get; set; }
}

// Dependent
public class DriverToCar
{
    public string DriverId { get; set; }
    public string CarId { get; set; }
}
```

The pair `DriverId, CarId` is a unique combination.

### The common case: without defining a join table

The most common case is to define the two principal entities with relational collections to each other and omit the `join` table altogether.

```
// Principal
public class Driver
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Car> Cars { get; } = [];
}

// Principal
public class Car
{
    public string Id { get; set; }
    public string PlateNumber { get; set; }
    public List<Driver> Drivers { get; } = [];
}
```

`EF Core` will discover this relationship using configuration convention.  

The relationship can also be expressed using Fluent API.

```
modelBuilder.Entity<Driver>()
    .HasMany(d => d.Cars)
    .WithMany(c => c.Drivers);
```

Even with the above explicit configuration `EF Core` configures a lot in the background for this relationship.

The following is the fully complete configuration, for learning purposes, and it is what actually the `EF Core` configures.

```
modelBuilder.Entity<Driver>()
    .HasMany(d => d.Cars)
    .WithMany(c => c.Drivers)
    .UsingEntity(
        "DriverCar",
        r => r.HasOne(typeof(Car)).WithMany().HasForeignKey("CarId").HasPrincipalKey(nameof(Car.Id)),
        l => l.HasOne(typeof(Driver)).WithMany().HasForeignKey("DriverId").HasPrincipalKey(nameof(Driver.Id)),
        j => j.HasKey("DriverId", "CarId"));
```

The result of all of the above three configurations is a database table, which is going to be used as the `join` table.  

The following is taken from an `Sqlite` database.

```
CREATE TABLE "DriverCar" (
    "DriverId" TEXT NOT NULL,
    "CarId" TEXT NOT NULL,
    CONSTRAINT "PK_DriverCar" PRIMARY KEY ("DriverId", "CarId"),
    CONSTRAINT "FK_DriverCar_Car_CarId" FOREIGN KEY ("CarId") REFERENCES "Car" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_DriverCar_Driver_DriverId" FOREIGN KEY ("DriverId") REFERENCES "Driver" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_DriverCar_CarId" ON "DriverCar" ("CarId");
```

The `DriverCar` name is given to the `join` table by convention. The developer can define this table name.

```
modelBuilder.Entity<Driver>()
    .HasMany(d => d.Cars)
    .WithMany(c => c.Drivers)
    .UsingEntity("DriverToCar");
```

### Defining a join table

A common case is to explicitly define the `join` table. Which is useful in cases where that `join` table is going to be referenced by application code.

```
// Principal
public class Driver
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Car> Cars { get; } = [];
}

// Principal
public class Car
{
    public string Id { get; set; }
    public string PlateNumber { get; set; }
    public List<Driver> Drivers { get; } = [];
}

// Dependent
public class DriverToCar
{
    public string DriverId { get; set; }
    public string CarId { get; set; }
}
```

That relationship is required to be expressed using Fluent API.

```
modelBuilder.Entity<Driver>()
    .HasMany(d => d.Cars)
    .WithMany(c => c.Drivers)
    .UsingEntity<DriverToCar>();
```

### More many-to-many cases
 
Except of the most common cases as depicted above there are many other cases, actually variations, described in the [docs](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many).


## Loading Related Data

`Related data` is a term referring to navigation properties of an entity.

Navigation properties of an entity [can be loaded](https://learn.microsoft.com/en-us/ef/core/querying/related-data/) along with the entity itself.

A navigation property can be configured to be loaded every time the container entity is loaded, using the `AutoInclude()` method in configuration.

In the next example the `SalesOrder.Customer` property is configured to be loaded when a `SalesOrder` is loaded.

```
modelBuilder.Entity<SalesOrder>().Navigation(so => so.Customer).AutoInclude();
```

Except of the `AutoInclude()` method, `EF Core` offers three ways in loading related data.

- **Eager Loading**. Related data is loaded at the same time and along with the container.
- **Explicit Loading**. Related data is loaded at a later time explicitly.
- **Lazy Loading**. Related data is loaded when the navigation property is accessed. 

> There are `Async()` versions of most of the methods used in the next examples.
 
### Eager Loading

In [Eager Loading](https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager) related data is loaded at the same time and along with the container entity using methods such as [`Include()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.include) and [`ThenInclude()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.theninclude).

The `Include()` method is used in loading related data directly attached to the container entity.

In the next example `Customer` and `OrderLines` are navigations of the `SalesOrder` entity.

```
using (var Context = new DataContext())
{
    SalesOrder SO = Context.SalesOrders                        
                    .Include(so => so.Customer)
                    .Include(so => so.OrderLines)
                    .Single(so => so.Code == "SO-0001");
}
```

The `ThenInclude()` method is used in loading related data attached to an entity belonging to a level just loaded with a previous `Include()` or `ThenInclude()` call.

In the next example `Product` is a navigational property of the `SalesOrderLine` entity while the `Category` is a navigational property of the `Product` entity.

```
using (var Context = new DataContext())
{
    SalesOrder SO = Context.SalesOrders                        
                    .Include(so => so.Customer)
                    .Include(so => so.OrderLines)
                    .ThenInclude(sol => sol.Product)
                    .ThenInclude(p => p.Category)
                    .Single(so => so.Code == "SO-0001");
}
```
 
### Explicit Loading

In [Explicit Loading](https://learn.microsoft.com/en-us/ef/core/querying/related-data/explicit) related data is loaded explicitly at a later time, after the container entity is loaded first, using methods such as [`Entry()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.entry), [`Reference()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.changetracking.entityentry-1.reference) and [`Collection()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.changetracking.entityentry-1.collection).

The `Entry()` method returns an [`EntityEntry<T>`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.changetracking.entityentry-1) which provides operations for the entity.

The next example is a transcription of the previous one.
 
```
using (var Context = new DataContext())
{
    SalesOrder SO = Context.SalesOrders
                    .Single(so => so.Code == "SO-0001");

    Context.Entry(SO)
        .Reference(so => so.Customer)
        .Load();

    Context.Entry(SO)
        .Collection(so => so.OrderLines)
        .Query()
        .Include(so => so.Product)
        .ThenInclude(p => p.Category)
        .Load();
}
```

The [Query()](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.changetracking.collectionentry-2.query) method returns an `IQueryable<T>` that can be to further load entities referenced by its navigation property.



### Lazy Loading

In [Lazy Loading](https://learn.microsoft.com/en-us/ef/core/querying/related-data/lazy) related data is loaded when the navigation property is accessed. 

There is a **warning** about `Lazy Loading` in [docs](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying#beware-of-lazy-loading). Please consult it carefully.

There are two ways to use `Lazy Loading`.

- using lazy loading proxies
- without using lazy loading proxies.

#### Lazy Loading using proxies

There are three requirements

- the [Microsoft.EntityFrameworkCore.Proxies](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Proxies/) package must be installed
- a configuration using the `UseLazyLoadingProxies()` method
- navigation properties should be declared as **virtual**.

The configuration can be done in the `DbContext.OnConfiguring()` method

```
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseLazyLoadingProxies()
        .UseSqlite(ConnectionString);
```

or in the `AddDbContext()` in the application's startup code.

```
builder.Services.AddDbContext<DataContext>(
    c => c.UseLazyLoadingProxies()
    .UseSqlite(ConnectionString)
);
```

Navigation properties must be **virtual**.

```
public class SalesOrder : BaseEntity
{
    public SalesOrder()
    {
    }

    // ...

    public string CustomerId { get; set; }
    public virtual Customer Customer { get; set; }

    public virtual ICollection<SalesOrderLine> OrderLines { get; set; }
}
```

That way navigation properties are loaded automatically just when they accessed.

 
```
using (var Context = new DataContext())
{
    SalesOrder SO = Context.SalesOrders
                    .Single(so => so.Code == "SO-0001");

    if (SO.Customer.Balance <= 500) // lazy loading Customer
    {
        foreach (var SOLine in SO.OrderLines) // lazy loading OrderLines
        {
            // ...
        }
    }
}
```
 
#### Lazy Loading without proxies 

This lazy loading method requires an [ILazyLoader](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.infrastructure.ilazyloader) service to be injected in the entity.

```
public class SalesOrder : BaseEntity
{
    ILazyLoader LazyLoader;
    ICollection<SalesOrderLine> orderLines;
    Customer customer;

    public SalesOrder()
    {
    }
    public SalesOrder(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }
    // ...

    public string CustomerId { get; set; }
    public virtual Customer Customer
    { 
        get => LazyLoader.Load(this, ref customer); 
        set => customer = value; 
    } 
    public virtual ICollection<SalesOrderLine> OrderLines 
    { 
        get => LazyLoader.Load(this, ref orderLines); 
        set => orderLines = value; 
    } 
    
}
```

## Migrations

`EF Core` provides the [Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations) feature as a way to keep application entities and database schema synchronized.

`Migrations` alter the database schema according to changes in application entities while preserving existing data in the database.

The [Microsoft.EntityFrameworkCore.Tools](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools) package is required.

`EF Core Migrations` provide a set of operations that can be used

- either in the `Package Manager Console` in Visual Studio `(Tools | NuGet Package Manager | Package Manager Console)`
- or in the [NET Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
 

### Visual Studio Migration Operations

Next are the most [usual operations](https://learn.microsoft.com/en-us/ef/core/cli/powershell).

- `Add-Migration MIGRATION_NAME`. Creates a migration under a specified name.
- `Update-Database`. Creates the database if not exists and updates database schema after an `Add-Migration`
- `Update-Database MIGRATION_NAME`. Reverts the schema of the underlying database into one migration previously applied by `Add-Migration`
- `Remove-Migration`. Removes the last migration applied by `Add-Migration`

### NET Core CLI Migration Operations

The corresponding operations using NET Core CLI are the following.

- `dotnet ef migrations add MIGRATION_NAME`
- `dotnet ef database update`
- `dotnet ef database update MIGRATION_NAME`
- `dotnet ef migrations remove`
 
### Migrations procedure

- the developer adds a `DbContext` and some entities in the application.
- executes the `Add-Migration MIGRATION_NAME`, e.g. `Add-Migration Initial_Migration`
- this creates a `Migrations` folder in the application root folder with two files: 
  - `<TimeStamp>_<Migration Name>.cs`. Contains the migration operations in the `Up()` and `Down()` methods.
  - `<ContextClassName>ModelSnapshot.cs`. Contains a snapshot of the current configuration of application entities.
- every next time the `Add-Migration MIGRATION_NAME` is executed a new pair of files is added to that `Migrations` folder.
- the developer calls `Update-Database` to create the database and apply the last migration.
- calling `Remove-Migration` removes the last applied migration. 

### Migration considerations

If the application seeds initial data to the database, using use `OnModelCreating()` and `HasData()` methods, may face some warnings or exceptions, especially if there are entities with `Id` of type string that changes in each execution, e.g. `Id = Guid.NewGuid().ToString()`.

Check this [reddit thread](https://www.reddit.com/r/dotnet/comments/1h92qst/ef_core_the_model_for_context/) about that problem.

It is preferable to not use `HasData()` in `OnModelCreating()` to seed data, in order to avoid the problem.

```
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>().HasData(ProductList);
}
```

Instead a static class can be used that checks if a table is empty and if it is empty, then it pumps the data.

```
static public void AddData()
{
    using (var Context = new DataContext())
    {
        if (Context.Products.Count() <= 0)
        {
            DbSet<Category> Categories = Context.Set<Category>();
            Categories.AddRange(CategoryList);

            DbSet<MeasureUnit> MeasureUnits = Context.Set<MeasureUnit>();
            MeasureUnits.AddRange(MeasureUnitList);

            DbSet<Product> Products = Context.Set<Product>();
            Products.AddRange(ProductList);

            DbSet<ProductMeasureUnit> ProductMeasureUnits = Context.Set<ProductMeasureUnit>();
            ProductMeasureUnits.AddRange(ProductMeasureUnitList);

            Context.SaveChanges();
        }
    }
}
```
### Easy way to explore Migrations with a Windows.Forms application

Here is the `.csproj` file.

```
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>WinExe</OutputType>
        <TargetFramework>net9.0-windows</TargetFramework>
        <UseWindowsForms>true</UseWindowsForms>
        <ImplicitUsings>enable</ImplicitUsings>
    </PropertyGroup>

    <ItemGroup>
        <FrameworkReference Include="Microsoft.AspNetCore.App" />
        <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.5" />
        <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.5" />
        <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.5">
          <PrivateAssets>all</PrivateAssets>
          <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageReference>
        <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.5" />
    </ItemGroup>

</Project>
```

And here is the `Program` class in the `Program.cs` file.

```
internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        IHostBuilder builder = Host.CreateDefaultBuilder();
        
        builder.ConfigureServices((context, services) => {
            services.AddTransient<DemoService>();
        });

        ServiceProvider = builder.Build().Services;

        Application.Run(new MainForm());
    }

    public static IServiceProvider ServiceProvider { get; private set; }
}
```

Migrations require a Dependency Injection container in order to function properly.

Consult the docs on how to [setup a host](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host?tabs=hostbuilder#set-up-a-host).

The `ServiceProvider` can be later used as

```
var Service = Program.ServiceProvider.GetService<DemoService>();

// call a service method here

```

## Further Reading

- [DbContext Pooling](https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics?tabs=with-di%2Cexpression-api-with-constant#dbcontext-pooling)
- [Keys](https://learn.microsoft.com/en-us/ef/core/modeling/keys)
- [Indexes](https://learn.microsoft.com/en-us/ef/core/modeling/indexes)
- [Generated Values](https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties)
- [Shadow and Indexer Properties](https://learn.microsoft.com/en-us/ef/core/modeling/shadow-properties)
- [Backing Fields](https://learn.microsoft.com/en-us/ef/core/modeling/backing-field)
- [Value Conversions](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions)
- [Value Comparers](https://learn.microsoft.com/en-us/ef/core/modeling/value-comparers)
- [Data Seeding](https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding)
- [Advanced Table Mapping](https://learn.microsoft.com/en-us/ef/core/modeling/table-splitting)
- [Owned Entity Types](https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities)
- [Logging, Events and Diagnostics](https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/)
- [Navigations](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/navigations)
- [Conventions for Relationship Discovery](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/conventions)
- [Keyless Entity Types](https://learn.microsoft.com/en-us/ef/core/modeling/keyless-entity-types)
- [Bulk Configuration](https://learn.microsoft.com/en-us/ef/core/modeling/bulk-configuration)