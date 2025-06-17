namespace MvcApp.Library
{

    /// <summary>
    /// Represents a product attribute along with its value.
    /// <para>Used by the Category Products page, when filtering Products of a Category by Product Attributes</para>
    /// </summary>
    public class ProductAttributeValueFilterItem
    {
        public ProductAttributeValueFilterItem()
        {
        }

        public int AttrId { get; set; }
        public int ValueId { get; set; }
    }
}
