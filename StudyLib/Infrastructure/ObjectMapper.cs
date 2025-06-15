namespace StudyLib
{
    /// <summary>
    /// Object mapper.
    /// <para>This mapper is the default implementation of the <see cref="IObjectMapper"/> interface.</para>
    /// <para>This implementation uses the excellent AutoMaper library.</para>
    /// <para>SEE: https://automapper.org/ </para>
    /// </summary>
    public class ObjectMapper : IObjectMapper
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


        // ● methods
        /// <summary>
        /// Adds a map item between two types, from a source type to a destination type, in an internal list of map items. 
        /// <para>A flag controls whether the mapping is a two-way one.</para>
        /// <para>The actual mappings are created when the <see cref="Configure()"/> is called. </para>
        /// <para>After the <see cref="Configure()"/> is called the <see cref="IsReady"/> returns true and calling <see cref="Add()"/> throws an exception.</para>
        /// <para>NOTE: Throws an exception if the <see cref="IsReady"/> is true. </para>
        /// </summary>
        public void Add(Type Source, Type Dest, bool TwoWay = false)
        {
            if (IsReady)
               throw new ApplicationException($"Can not add map configuration. {nameof(ObjectMapper)} is already configured.");

            MapList.Add(new MapItem(Source, Dest, TwoWay));
        }

        /// <summary>
        /// Creates the mappings based on the internal map list. 
        /// <para>NOTE: Throws an exception if the <see cref="Configure"/>() method is already called. </para>
        /// </summary>
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

        /// <summary>
        /// Creates and returns a destination object, based on a specified type argument, and maps a specified source object to destination object.
        /// </summary>
        public TDestination Map<TDestination>(object Source) where TDestination : class
        {
            if (!IsReady)
                throw new ApplicationException($"Can not map objects. {nameof(ObjectMapper)} is not configured.");

            if (Source == null)
                throw new ArgumentNullException(nameof(Source));

            return Mapper.Map<TDestination>(Source);
        }
        /// <summary>
        /// Maps a source to a destination object.
        /// </summary>
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

        // ● properties 
        /// <summary>
        /// True when the <see cref="Map()" /> and <see cref="MapTo()" /> can be called.
        /// <para>NOTE: Calling <see cref="Add()"/> when the <see cref="IsReady"/> is true throws an exception . </para>
        /// </summary>
        public bool IsReady => Mapper != null;
    }
}
