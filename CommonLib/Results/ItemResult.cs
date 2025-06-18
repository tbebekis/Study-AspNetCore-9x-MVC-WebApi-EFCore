namespace CommonLib
{
    /// <summary>
    /// A <see cref="DataResult"/> response for a single item
    /// </summary>
    [Description("A requested object.")]
    public class ItemResult<T> : DataResult
    {
        /// <summary>
        /// The item
        /// </summary>
        [Description("The result object.")]
        public T Item { get; set; }
    }
 
}
