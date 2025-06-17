namespace MvcApp.Library
{
    /// <summary>
    /// Product attribute value.
    /// <para>CATION: Used ONLY in filtering products</para>
    /// </summary>
    public class ProductAttributeValue
    {
        /* construction */
        /// <summary>
        /// Constructor
        /// </summary>
        public ProductAttributeValue()
        {
        }

        /* public */
        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        public override string ToString()
        {
            return $"{Id} - {Name}";
        }

        /* properties */
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// DisplayOrder
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// Products having this specific value
        /// </summary>
        public int ProductCount { get; set; }
    }
}
