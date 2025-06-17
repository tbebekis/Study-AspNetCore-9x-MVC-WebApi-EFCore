namespace MvcApp.Library
{

    /// <summary>
    /// Product filtering class.
    /// <para>Used by the Header Search or the Product Search page, when filtering Products</para>
    /// <para><strong>CAUTION: </strong>When <see cref="CategoryId"/> is specified 
    /// then the searching and filtering is done against the Products of this Category.
    /// When a <see cref="Term"/> is specified then that term is used  in searching the filtering products by <see cref="Name"/> or <see cref="Id"/>.
    /// </para>
    /// </summary>
    public class ProductListFilter
    {
        public ProductListFilter()
        {
            this.PageIndex = PagingInfo.GetQueryStringPageIndex();
            this.PageSize = PagingInfo.GetQueryStringPageSize();
        }

        /// <summary>
        /// Search term. Could be Product.Id or part of Product.Name
        /// <para>NOTE: The Term could be null or empty. In such a case just Caterory's Products are returned.</para>
        /// </summary>
        public string Term { get; set; }
        /// <summary>
        /// When false, the default, only Id and Name is returned with Products, to be displayed in the Mini Search dropdown. 
        /// <para>When true, then enough info should be returned in order to render a Product Box or Product Row list.</para>
        /// </summary>
        public bool FullProductInfo { get; set; }
        /// <summary>
        /// If specified then the search and the filtering is done against the Products of this Category.
        /// </summary>
        public string CategoryId { get; set; }
        /// <summary>
        /// When true sub-Categories of the specified Category are taken into account.
        /// </summary>
        public bool IncludeSubCategories { get; set; }
        /// <summary>
        /// When true, the default, then only Published Categories are searched.
        /// </summary>
        public bool PublishedCategoriesOnly { get; set; } = true;

        /// <summary>
        /// Page index. Comes from the HTTP Request Query String
        /// </summary>
        public int PageIndex { get; }
        /// <summary>
        /// Page size. Comes from the HTTP Request Query String
        /// </summary>
        public int PageSize { get; }
    }
}
