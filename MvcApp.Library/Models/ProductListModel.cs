namespace MvcApp.Models
{
    public class ProductListModel
    {
        public PagingInfo PagingInfo { get; set; }
        public List<ProductModel> Products { get; set; }
    }
}
