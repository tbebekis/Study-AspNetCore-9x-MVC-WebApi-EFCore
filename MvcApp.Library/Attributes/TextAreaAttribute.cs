namespace MvcApp.Library
{
    /// <summary>
    /// Marks a model property as disable. Results in a disabled html attribute
    /// <para>NOTE: Used by <see cref="TagHelperControlRow"/> custom tag helper.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TextAreaAttribute : Attribute, IModelAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TextAreaAttribute()
        {
        }

        /// <summary>
        /// The rows
        /// </summary>
        public int Rows { get; set; } = 4;
        /// <summary>
        /// The columns
        /// </summary>
        public int Cols { get; set; } = 20;

        /// <summary>
        /// The class name of the attribute.
        /// <para>NOTE: <see cref="IModelAttribute"/> implementation. </para>
        /// </summary>
        public string ClassName { get { return this.GetType().Name; } }
 
    }
}
