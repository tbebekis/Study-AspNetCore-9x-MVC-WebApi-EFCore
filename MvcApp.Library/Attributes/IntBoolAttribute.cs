namespace MvcApp.Library
{
    /// <summary>
    /// Marks a model integer property to be used as a boolean and display an html checkbox when rendered.
    /// <para>NOTE: Used by <see cref="TagHelperControlRow"/> custom tag helper.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IntBoolAttribute: Attribute, IModelAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public IntBoolAttribute()
        {
        }

        /// <summary>
        /// The class name of the attribute.
        /// <para>NOTE: <see cref="IModelAttribute"/> implementation. </para>
        /// </summary>
        public string ClassName { get { return this.GetType().Name; } }
    }
}
