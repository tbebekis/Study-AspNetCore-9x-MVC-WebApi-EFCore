namespace CommonLib
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
            Set(Key, Value, Caches.DefaultEvictionTimeoutMinutes);
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
                o.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(TimeoutMinutes); // An absolute expiration means a cached item will be removed at an explicit date and time
                o.SlidingExpiration = TimeSpan.FromMinutes(TimeoutMinutes);     // Sliding expiration means a cached item will be removed if is remains idle (not accessed) for a certain amount of time.

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

        /// <summary>
        /// Returns a value found under a specified key.
        /// <para>If the key does not exist, it calls the specified loader call-back function </para>
        /// <para>The loader function should be defined as <c>Task&lt;CacheLoaderResult&lt;T&gt;&gt; LoaderFunc&lt;T&gt;().</c></para>
        /// <para>The loader function returns a <see cref="CacheLoaderResult&lt;T&gt;"/> with two properties: the eviction timeout and the result object.</para>
        /// <para>NOTE: Key is case sensitive.</para>
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
}
