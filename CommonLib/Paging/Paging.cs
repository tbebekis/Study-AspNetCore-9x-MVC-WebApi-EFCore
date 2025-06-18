namespace CommonLib
{

    /// <summary>
    /// Helper with paging information
    /// </summary>
    public class Paging: IPaging
    {

        public Paging()
        {
        }
        public Paging(int pageIndex, int pageSize)
        {
            PageSize = pageSize <= 0? 5: pageSize;
            PageIndex = pageIndex < 0 ? 0: pageIndex;
        }

        // ● public
        /// <summary>
        /// Sets the properties of this instance
        /// </summary>
        public void SetFrom(IPaging Source)
        {
            TotalItems = Source.TotalItems;
            PageIndex = Source.PageIndex;
            PageSize = Source.PageSize;
        }
        /// <summary>
        /// Returns the total pages given the total items and the page size.
        /// </summary>
        static public int GetTotalPages(int TotalItems, int PageSize)
        {
            if (TotalItems <= PageSize)
                return 1;

            int Result = TotalItems / PageSize;
            int Remainder = TotalItems % PageSize;
            if (Remainder > 0)
                Result++;

            return Result;
        }

        // ● properties 
        /*
        /// <summary>
        /// The number of total items when this is a paged response. 
        /// <para><strong>NOTE:</strong> This property is assigned by the code that executes the query. <strong>Not</strong> by the caller code.</para>
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// The number of total pages.
        /// </summary>
        public int TotalPages => GetTotalPages(TotalItems, PageSize);
        /// <summary>
        /// The number of items in a page.
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// The current page index. 0 based.
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// The number of the page. 1-based
        /// </summary>
        public int PageNumber => PageIndex + 1;
        */

        // ● properties
        /// <summary>
        /// The number of total items when this is a paged response. 
        /// <para><strong>NOTE:</strong> This property is assigned by the code that executes the query. <strong>Not</strong> by the caller code.</para>
        /// </summary>
        [Description("The number of total items when this is a paged response. Assigned by the code that executes the query, not the caller.")]
        [JsonPropertyOrder(-899)]
        public int TotalItems { get; set; }
        /// <summary>
        /// The number of total pages.
        /// </summary>
        [Description("The number of total pages.")]
        [JsonPropertyOrder(-888)]
        public int TotalPages => Paging.GetTotalPages(TotalItems, PageSize);
        /// <summary>
        /// The number of items in a page.
        /// </summary>
        [Description("The number of items in the page.")]
        [JsonPropertyOrder(-887)]
        public int PageSize { get; set; }
        /// <summary>
        /// The current page index. 0 based.
        /// </summary>
        [Description("The current page index. 0 based.")]
        [JsonPropertyOrder(-886)]
        public int PageIndex { get; set; }
        /// <summary>
        /// The number of the page. 1-based
        /// </summary>
        [Description("The number of the page. 1-based")]
        [JsonPropertyOrder(-885)]
        public int PageNumber => PageIndex + 1;
    }


}
