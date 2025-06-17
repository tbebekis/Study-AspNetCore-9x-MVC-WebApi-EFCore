namespace StudyLib
{
    /// <summary>
    /// Helper
    /// </summary>
    static public class Caches
    {
        const int MinTimeoutMinutes = 10;
        static int fDefaultEvictionTimeoutMinutes;

        // ● public 
        /// <summary>
        /// Normalizes the specified timeout minutes.
        /// </summary>
        static public int GetTimeoutMinutes(int TimeoutMinutes)
        {
            return TimeoutMinutes >= MinTimeoutMinutes ? TimeoutMinutes : DefaultEvictionTimeoutMinutes;
        }

        // ● properties 
        /// <summary>
        /// The default eviction timeout of an entry from the cache, in minutes. 
        /// </summary>
        static public int DefaultEvictionTimeoutMinutes
        {
            get => fDefaultEvictionTimeoutMinutes >= MinTimeoutMinutes ? fDefaultEvictionTimeoutMinutes : MinTimeoutMinutes;
            set => fDefaultEvictionTimeoutMinutes = value;
        }
    }
}
