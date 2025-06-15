namespace StudyLib
{
    /// <summary>
    /// Represents a Cache.
    /// <para>Could be a memory or distribute cache.</para>
    /// </summary>
    public interface IAppCache
    {
        // ● public  
        /// <summary>
        /// Returns a value found under a specified key, if any, else returns the default value of the specified type argument.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        T Get<T>(string Key);
        /// <summary>
        /// Returns true if an entry exists under a specified key. Returns the value too as out parameter.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        bool TryGetValue<T>(string Key, out T Value);
        /// <summary>
        /// Removes and returns a value found under a specified key, if any, else returns the default value of the specified type argument.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        T Pop<T>(string Key);

        /// <summary>
        /// Sets an entry under a specified key. Creates the entry if not already exists.
        /// <para>If is a new entry it will be removed from the cache after <see cref="DefaultEvictionTimeoutMinutes"/> minutes. </para>
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        void Set<T>(string Key, T Value);
        /// <summary>
        /// Sets an entry under a specified key. Creates the entry if not already exists.
        /// <para>If is a new entry it will be removed from the cache after the specified timeout minutes. </para>
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        void Set<T>(string Key, T Value, int TimeoutMinutes);

        /// <summary>
        /// Returns true if the key exists.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        bool ContainsKey(string Key);
        /// <summary>
        /// Removes an entry by a specified key, if is in the cache.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        void Remove(string Key);

        // ● properties 
        /// <summary>
        /// The default eviction timeout of an entry from the cache, in minutes. 
        /// <para>Defaults to 0 which means "use the timeouts of the internal implementation".</para>
        /// </summary>
        int DefaultEvictionTimeoutMinutes { get; set; }
    }
}
