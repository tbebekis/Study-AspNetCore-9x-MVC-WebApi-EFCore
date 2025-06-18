namespace CommonLib
{
    /// <summary>
    /// Object mapper.
    /// <para>Maps between types are added using the <see cref="Add()"/> method.</para>
    /// <para>When all mappings are added the <see cref="Configure()"/> should be called.</para>
    /// <para>After the <see cref="Configure()"/> is called the mapper is ready to use, and calling <see cref="Add()"/> throws an exception.</para>
    /// <para>The default implementation of this interface uses the excellent Automapper library found at https://automapper.org/ </para>
    /// </summary>
    public interface IObjectMapper
    {
        // ● methods
        /// <summary>
        /// Adds a map item between two types, from a source type to a destination type, in an internal list of map items. 
        /// <para>A flag controls whether the mapping is a two-way one.</para>
        /// <para>The actual mappings are created when the <see cref="Configure()"/> is called. </para>
        /// <para>After the <see cref="Configure()"/> is called the <see cref="IsReady"/> returns true and calling <see cref="Add()"/> throws an exception.</para>
        /// <para>NOTE: Throws an exception if the <see cref="IsReady"/> is true. </para>
        /// </summary>
        void Add(Type Source, Type Dest, bool TwoWay = false);

        /// <summary>
        /// Creates the mappings based on the internal map list. 
        /// <para>NOTE: Throws an exception if the <see cref="Configure"/>() method is already called. </para>
        /// </summary>
        void Configure();

        /// <summary>
        /// Creates and returns a destination object, based on a specified type argument, and maps a specified source object to destination object.
        /// </summary>
        TDestination Map<TDestination>(object Source) where TDestination : class;
        /// <summary>
        /// Maps a source to a destination object.
        /// </summary>
        TDestination MapTo<TSource, TDestination>(TSource Source, TDestination Dest) where TSource : class where TDestination : class;

        // ● properties 
        /// <summary>
        /// True when the <see cref="Map()" /> and <see cref="MapTo()" /> can be called.
        /// <para>NOTE: Calling <see cref="Add()"/> when the <see cref="IsReady"/> is true throws an exception . </para>
        /// </summary>
        bool IsReady { get; }
    }
}
