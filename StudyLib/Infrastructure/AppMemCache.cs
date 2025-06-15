namespace StudyLib
{
    /// <summary>
    /// An <see cref="IMemoryCache"/> wrapper 
    /// </summary>
    public class AppMemCache : IAppCache
    {
        /* private */
        IMemoryCache Cache;

        // ● construction 
        /// <summary>
        /// Constructor
        /// </summary>
        public AppMemCache(IMemoryCache Cache)
        {
            this.Cache = Cache;
        }

        // ● public  
        /// <summary>
        /// Returns a value found under a specified key, if any, else returns the default value of the specified type argument.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public T Get<T>(string Key)
        {
            return Cache.Get<T>(Key);
        }
        /// <summary>
        /// Returns true if an entry exists under a specified key. Returns the value too as out parameter.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public bool TryGetValue<T>(string Key, out T Value)
        {
            return Cache.TryGetValue(Key, out Value);
        }
        /// <summary>
        /// Removes and returns a value found under a specified key, if any, else returns the default value of the specified type argument.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public T Pop<T>(string Key)
        {
            T Result = Get<T>(Key);
            Remove(Key);
            return Result;
        }

        /// <summary>
        /// Sets an entry under a specified key. Creates the entry if not already exists.
        /// <para>If is a new entry it will be removed from the cache after <see cref="DefaultEvictionTimeoutMinutes"/> minutes. </para>
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public void Set<T>(string Key, T Value)
        {
            Set(Key, Value, DefaultEvictionTimeoutMinutes);
        }
        /// <summary>
        /// Sets an entry under a specified key. Creates the entry if not already exists.
        /// <para>If is a new entry it will be removed from the cache after the specified timeout minutes. </para>
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public void Set<T>(string Key, T Value, int TimeoutMinutes)
        {

            Remove(Key);

            if (TimeoutMinutes > 0)
            {
                var o = new MemoryCacheEntryOptions();
                o.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(TimeoutMinutes); // An absolute expiration means a cached item will be removed an an explicit date and time
                o.SlidingExpiration = TimeSpan.FromMinutes(TimeoutMinutes);     // Sliding expiration means a cached item will be removed it is remains idle (not accessed) for a certain amount of time.

                Cache.Set(Key, Value, o);
            }
            else
            {
                Cache.Set(Key, Value);
            }
        }


        /// <summary>
        /// Returns true if the key exists.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public bool ContainsKey(string Key)
        {
            return Cache.TryGetValue(Key, out var Item);
        }
        /// <summary>
        /// Removes an entry by a specified key, if is in the cache.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public void Remove(string Key)
        {
            if (ContainsKey(Key))
                Cache.Remove(Key);
        }
 
        // ● properties 
        /// <summary>
        /// The default eviction timeout of an entry from the cache, in minutes. 
        /// <para>Defaults to 0 which means "use the timeouts of the internal implementation".</para>
        /// </summary>
        public int DefaultEvictionTimeoutMinutes { get; set; }  
    }
}
