namespace MvcApp.Library
{
    /// <summary>
    /// Marks a model property as disabled. Results in a disabled html attribute.
    /// <para>NOTE: Used by <see cref="TagHelperControlRow"/> custom tag helper.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DisabledAttribute: Attribute, IModelAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DisabledAttribute()
        {
        }

        /// <summary>
        /// The class name of the attribute.
        /// <para>NOTE: <see cref="IModelAttribute"/> implementation. </para>
        /// </summary>
        public string ClassName { get { return this.GetType().Name; } }
    }
}
