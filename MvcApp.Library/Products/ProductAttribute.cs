namespace MvcApp.Library
{
    /// <summary>
    /// Product attribute.
    /// <para>CATION: Used ONLY in filtering products</para>
    /// </summary>
    public class ProductAttribute
    {
        /* construction */
        /// <summary>
        /// Constructor
        /// </summary>
        public ProductAttribute()
        {
        }

        /* public */
        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        public override string ToString()
        {
            return $"{Id} - {Name} - Values = {Values.Count}";
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
        /// The list of values owned by this attribute
        /// </summary>
        public List<ProductAttributeValue> Values { get; set; } = new List<ProductAttributeValue>();
    }

}
