namespace CommonLib
{
    /// <summary>
    /// The result of a cache loader callback function.
    /// </summary>
    public class CacheLoaderResult<T>
    {
 
        // ● construction 
        /// <summary>
        /// Constructor
        /// </summary>
        public CacheLoaderResult(T Value)
            : this(Value, Caches.DefaultEvictionTimeoutMinutes)
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public CacheLoaderResult(T Value, int TimeoutMinutes = 0)
        {
            this.Value = Value;
            this.TimeoutMinutes = Caches.GetTimeoutMinutes(TimeoutMinutes);
        }
 
        // ● properties 
        /// <summary>
        /// The value returned from cache.
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// The eviction timeout of an entry from the cache, in minutes. 
        /// </summary>
        public int TimeoutMinutes { get; set; }

    }

}
