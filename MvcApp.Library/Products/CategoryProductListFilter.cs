namespace MvcApp.Library
{
 
    /// <summary>
    /// Categegory Products filtering class.
    /// <para>Used by the Category Products page, when filtering Products of a Category by Product Attributes</para>
    /// </summary>
    public class CategoryProductListFilter
    {
        public CategoryProductListFilter()
        {
            this.PageIndex = PagingInfo.GetQueryStringPageIndex();
            this.PageSize = PagingInfo.GetQueryStringPageSize();
            this.ValueList = GetQueryStringListFilters();
        }

        /// <summary>
        /// Returns the "filter" parameter from the current query string
        /// <para>The "filter" parameter has the format A0|V0,A1|V1,AN|VN where A is the AttributeId and V is the ValueId</para>
        /// </summary>
        static public List<ProductAttributeValueFilterItem> GetQueryStringListFilters()
        {
            List<ProductAttributeValueFilterItem> Result = new List<ProductAttributeValueFilterItem>();

            string Default = "";
            string Filter = Lib.GetQueryValue("filter", Default);
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                string[] FilterItems = Filter.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] Parts;
                ProductAttributeValueFilterItem Item;
                if (FilterItems != null && FilterItems.Length > 0)
                {
                    foreach (string FilterItem in FilterItems)
                    {
                        Parts = FilterItem.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        if (Parts.Length == 2)
                        {
                            Item = new ProductAttributeValueFilterItem();
                            Item.AttrId = Convert.ToInt32(Parts[0]);
                            Item.ValueId = Convert.ToInt32(Parts[1]);
                            Result.Add(Item);
                        }
                    }
                }
            }

            return Result;
        }


        public string CategoryId { get; set; }

        public int PageIndex { get; }
        public int PageSize { get; }

        public List<ProductAttributeValueFilterItem> ValueList { get; }
    }


}
