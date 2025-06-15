namespace StudyLib
{
    /// <summary>
    /// A <see cref="DataResult"/> response for lists of items.
    /// </summary>
    [Description("A list of requested objects.")]
    public class ListResult<T> : DataResult
    {
        /// <summary>
        /// The list of items
        /// </summary>
        [Description("A list of result objects."), JsonPropertyOrder(-800)]
        public List<T> List { get; set; } = new List<T>();
    }
}
