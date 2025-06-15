namespace WebApiApp.Library
{

    /// <summary>
    /// Default settings
    /// </summary>
    public class DefaultSettings
    {
        int fCacheTimeoutMinutes = 5;
        int fPageSize = 10;
        string fCultureCode;

        /// <summary>
        /// The eviction timeout of an entry from the cache, in minutes. 
        /// <para>Defaults to 0 which means "use the timeouts of the internal implementation".</para>
        /// </summary>
        public int CacheTimeoutMinutes
        {
            get => fCacheTimeoutMinutes >= 5 ? fCacheTimeoutMinutes : 5;
            set => fCacheTimeoutMinutes = value;
        }
        /// <summary>
        /// The number of items in a page when paging is involved.
        /// </summary>
        public int PageSize
        {
            get => fPageSize >= 10 ? fPageSize : 10;
            set => fPageSize = value;
        }
        /// <summary>
        /// The default culture, i.e. el-GR
        /// <para>NOTE: This setting is assigned initially by default to any new visitor.</para>
        /// </summary>
        public string CultureCode
        {
            get => !string.IsNullOrWhiteSpace(fCultureCode) ? fCultureCode : "en-US";           // "en-US";  
            set => fCultureCode = value;
        }
        /// <summary>
        /// List of supported cultures
        /// </summary>
        public List<string> SupportedCultures { get; set; } = new List<string>() { "en-US" };
    }
}
