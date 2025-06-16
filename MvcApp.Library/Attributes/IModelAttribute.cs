namespace MvcApp.Library
{
    /// <summary>
    /// A custom model attribute.
    /// <para>See <see cref="TagHelperModelMetadataProvider"/> for usage instructions. </para>
    /// </summary>
    public interface IModelAttribute
    {
        /// <summary>
        /// The class name of the attribute
        /// </summary>
        string ClassName { get; }
    }
}
