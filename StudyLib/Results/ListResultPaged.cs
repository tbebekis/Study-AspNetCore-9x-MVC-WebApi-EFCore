namespace StudyLib
{

    [Description("A list of requested objects with pagination.")]
    public class ListResultPaged<T>: ListResult<T>, IPaging
    {
        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public ListResultPaged() 
        { 
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public ListResultPaged(IPaging SourcePaging, List<T> SourceList = null)
        {
            SetFrom(SourcePaging, SourceList);
        }

        // ● public
        /// <summary>
        /// Sets the properties of this instance
        /// </summary>
        public void SetFrom(IPaging SourcePaging, List<T> SourceList = null)
        {
            TotalItems = SourcePaging.TotalItems;
            PageIndex = SourcePaging.PageIndex;
            PageSize = SourcePaging.PageSize;

            List = SourceList;
        }

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
