namespace CommonLib
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

        /// <summary>
        /// Returns a value found under a specified key.
        /// <para>If the key does not exist, it calls the specified loader call-back function </para>
        /// <para>The loader function should be defined as <c>Task&lt;CacheLoaderResult&lt;T&gt;&gt; LoaderFunc&lt;T&gt;().</c></para>
        /// <para>The loader function returns a <see cref="CacheLoaderResult&lt;T&gt;"/> with two properties: the eviction timeout and the result object.</para>
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        Task<T> Get<T>(string Key, Func<Task<CacheLoaderResult<T>>> LoaderFunc);
    }




}
