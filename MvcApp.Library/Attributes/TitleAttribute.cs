namespace MvcApp.Library
{
    /// <summary>
    /// A localizable <see cref="DisplayNameAttribute"/> attribute, to be used with model properties
    /// <para>NOTE: Used by <see cref="TagHelperControlRow"/> custom tag helper.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TitleAttribute: DisplayNameAttribute, IModelAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TitleAttribute(string Key)
        {
            this.Key = Key;
        }

        /// <summary>
        /// The resource string key
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// The localized text.
        /// </summary>
        public override string DisplayName => Lib.Localize(Key); 
        /// <summary>
        /// The class name of the attribute.
        /// <para>NOTE: <see cref="IModelAttribute"/> implementation. </para>
        /// </summary>
        public string ClassName { get { return this.GetType().Name; } }
    }



}
