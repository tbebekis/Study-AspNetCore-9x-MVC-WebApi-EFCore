# Object Mapping

Different parts of applications use different data objects. 

- **Entity**. The term [entity](https://en.wikipedia.org/wiki/Entity) is used in conjuction with [Object Relation Mapping (ORM)](https://en.wikipedia.org/wiki/Object%E2%80%93relational_mapping) frameworks. In `ORMs` an entity class is a mapping to a table in a relational database and each entity instance is a data row in that table. An entity usually has an `Id` property.
- **Data Transfer Object (DTO)**. A [DTO](https://en.wikipedia.org/wiki/Data_transfer_object) is an object used in transfering data between layers, i.e. parts, of an application or between applications.
- **Model or View Model**. In the [MVC](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller) world a [model](https://en.wikipedia.org/wiki/Data_model) is an object used in transfering data between a controller and a view.

`Object Mapping` is the procedure of converting an object of certain type to an object of another type.

The main reason of using object mapping is security. For example entities coming from an ORM may contain sensitive  or internal information.

Another reason is that some objects are carriers of large chunks of information that may not required by the receiver end.

## Property Correspondence

`Object Mapping` is done by setting up a correspondence of properties of one type to properties of another type.

For this correspondence to be done automatically 

- source and destination properties must mach by name and type
- all destination properties are meant to be mapped
- destination properties not mapped to source properties are ignored.

When these conditions are not met then a custom mapping is required.
 
## Object Mapping libraries in .Net

There is number of Object Mapping libraries in the .Net ecosystem. 

Some of them are the following.

- [AutoMapper](https://github.com/LuckyPennySoftware/AutoMapper)
- [Mapster](https://github.com/MapsterMapper/Mapster)
- [Mapperly](https://github.com/riok/mapperly)
- [AgileMapper](https://github.com/agileobjects/AgileMapper)
- [TinyMapper](https://github.com/TinyMapper/TinyMapper)
- and more

## A JSON Object Mapper

[JsonSerializer](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to) offers a quick and dirty way to come up with an Object Mapper.

```
using System.Text.Json;
using System.Text.Json.Serialization;

static public class JsonMapper
{
    static JsonSerializerOptions JsonOptions = new()
    {
            PropertyNameCaseInsensitive = true,
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, 
            ReferenceHandler = null
    };

    static public TDest Map<TDest>(object Source) where TDest : new()
    {
        string JsonText = JsonSerializer.Serialize(Source, JsonOptions);
        TDest Result = JsonSerializer.Deserialize<TDest>(JsonText, JsonOptions);
        return Result;
    }
}
```

## A Object Mapper class with AutoMapper

Consider the following interface.

The `Add()` method adds a map item between two types, from a source type to a destination type, in an internal list of map items.  A flag controls whether the mapping is a two-way one.

The `Configure()` method creates the mappings based on the internal map list. After the `Configure()` is called the mapper is ready to use, and calling `Add()` throws an exception.

The `Map<TDestination>()` and `MapTo<TSource, TDestination>(TSource Source, TDestination Dest)` perform the actuall mapping between a source and a destination object.

```
public interface IObjectMapper
{
    void Add(Type Source, Type Dest, bool TwoWay = false);

    void Configure();

    TDestination Map<TDestination>(object Source) where TDestination : class;
    TDestination MapTo<TSource, TDestination>(TSource Source, TDestination Dest) where TSource : class where TDestination : class;

    bool IsReady { get; }
}
```

The following class is an implementation of the above interface using the excellent [AutoMapper](https://github.com/LuckyPennySoftware/AutoMapper) library.

```
internal class ObjectMapper : IObjectMapper
{
    class MapItem
    {
        public MapItem(Type Source, Type Dest, bool TwoWay)
        {
            this.Source = Source;
            this.Dest = Dest;
            this.TwoWay = TwoWay;
        }

        public Type Source { get; }
        public Type Dest { get; }
        public bool TwoWay { get; }
    }

    static List<MapItem> MapList = new List<MapItem>();

    static MapperConfiguration Configuration;
    static Mapper Mapper;
 
    public void Add(Type Source, Type Dest, bool TwoWay = false)
    {
        if (IsReady)
           throw new ApplicationException($"Can not add map configuration. {nameof(ObjectMapper)} is already configured.");

        MapList.Add(new MapItem(Source, Dest, TwoWay));
    }
 
    public void Configure()
    {
        if (IsReady)
            throw new ApplicationException($"{nameof(ObjectMapper)} is already configured.");

        Configuration = new MapperConfiguration(cfg => {

            cfg.AllowNullCollections = true;
            cfg.AddGlobalIgnore("Item");

            foreach (var Item in MapList)
            {
                cfg.CreateMap(Item.Source, Item.Dest);
                if (Item.TwoWay)
                    cfg.CreateMap(Item.Dest, Item.Source);
            }

            MapList.Clear();
        });

        Mapper = new Mapper(Configuration);
    }

    public TDestination Map<TDestination>(object Source) where TDestination : class
    {
        if (!IsReady)
            throw new ApplicationException($"Can not map objects. {nameof(ObjectMapper)} is not configured.");

        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        return Mapper.Map<TDestination>(Source);
    }

    public TDestination MapTo<TSource, TDestination>(TSource Source, TDestination Dest) where TSource : class where TDestination : class
    {
        if (!IsReady)
            throw new ApplicationException($"Can not map objects. {nameof(ObjectMapper)} is not configured.");

        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        if (Dest == null)
            throw new ArgumentNullException(nameof(Dest));

        return Mapper.Map(Source, Dest);
    }

    public bool IsReady => Mapper != null;
}

```