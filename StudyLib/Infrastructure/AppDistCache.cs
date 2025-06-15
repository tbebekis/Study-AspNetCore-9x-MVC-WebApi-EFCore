namespace StudyLib
{

    /// <summary>
    /// An <see cref="IDistributedCache"/> wrapper 
    /// </summary>
    public class AppDistCache: IAppCache
    {
        // ● private 
        IDistributedCache Cache;

        static JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        // ● construction 
        /// <summary>
        /// Constructor
        /// </summary>
        public AppDistCache(IDistributedCache Cache)
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
            T Value;
            return TryGetValue<T>(Key, out Value) ? Value : default(T);
        }
        /// <summary>
        /// Returns true if an entry exists under a specified key. Returns the value too as out parameter.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public bool TryGetValue<T>(string Key, out T Value)
        {
            byte[] Buffer = Cache.Get(Key);
            Value = default;

            if (Buffer == null)
                return false;

            Value = JsonSerializer.Deserialize<T>(Buffer, JsonOptions);
            return true;
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

            byte[] Buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Value, JsonOptions));

            if (TimeoutMinutes > 0)
            {
                var o = new DistributedCacheEntryOptions();
                o.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(TimeoutMinutes); // An absolute expiration means a cached item will be removed an an explicit date and time
                o.SlidingExpiration = TimeSpan.FromMinutes(TimeoutMinutes);     // Sliding expiration means a cached item will be removed it is remains idle (not accessed) for a certain amount of time.

                Cache.Set(Key, Buffer, o);
            }
            else
            {
                Cache.Set(Key, Buffer);
            }
        }

        /// <summary>
        /// Returns true if the key exists.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public bool ContainsKey(string Key)
        {
            return Cache.Get(Key) != null;
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
