# Entity Framework Core

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

[Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) is an [Object Relational Mapping (ORM)](https://en.wikipedia.org/wiki/Object%E2%80%93relational_mapping) framework.

`EF Core` is open-source, lightweight, extensible and supports [a great number of databases](https://learn.microsoft.com/en-us/ef/core/providers/).

## The EF Core Model

In order to access data in a database with `EF Core` two elements are required

- an Entity. A mostly [POCO](https://en.wikipedia.org/wiki/Plain_old_CLR_object) class that maps to a database table.
- a Data Context. A [DbContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext) derived class which represents a session with the database.

In `EF Core` terminology these two elements, the Entities and the Data Context, are called [Model](https://learn.microsoft.com/en-us/ef/core/modeling/).

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
- The `ApplyConfigurationsFromAssembly()` call in the `OnModelCreating()` method forces `EF Core` to search a specified Assembly for entity classes and include them in its `Model`. This is done because our entity classes are accompanied by a `IEntityTypeConfiguration` interface implementation.
- The line `DemoData.AddData(modelBuilder)`, in the `OnModelCreating()` method, uses the static `DemoData` class to add initial data in the database.
- [DbContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext) is used for `CRUD` operations and is a generic repository too.
- [DbSet](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbset-1), like the `DbContext` is used for `CRUD` operations and is a generic repository too.
- `DbContext` and `DbSet` provide `CRUD` methods such as `Add()`, `Remove()` and `Update()`.
- `DbContext` provides the `SaveChanges()` method which [commits](https://en.wikipedia.org/wiki/Commit_(data_management)) the changes to the database.
- It is a good idea to review the documentation for both of these clasess and check the provided properties and methods. 
- `DbContext` is **not** thread safe.
- `DbContext` is [IDisposable](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose). Its lifetime should be as short as possible.
- `DbContext` represents a session with the database, i.e. a [Transaction](https://en.wikipedia.org/wiki/Database_transaction).
- In other words `DbContext` implements the [Unit of Work](https://en.wikipedia.org/wiki/Unit_of_work). An `EF Core` application may call many times `Add()`, `Remove()` or `Update()` methods, against one or more entities, and then, in the end, call the `DbContext.SaveChanges()` method in order to [commit](https://en.wikipedia.org/wiki/Commit_(data_management)) the changes to the database.
 