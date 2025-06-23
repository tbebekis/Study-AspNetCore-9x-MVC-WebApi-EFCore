namespace CommonLib
{

    [Description("A list of requested objects with pagination.")]
    public class ListResultPaged<T>: ListResult<T>, IPaging
    {
        int IPaging.TotalItems { get => Paging.TotalItems; set => Paging.TotalItems = value; }
        int IPaging.PageSize { get => Paging.PageSize; set => Paging.PageSize = value; }
        int IPaging.PageIndex { get => Paging.TotalItems; set => Paging.PageIndex = value; }

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
            Paging.TotalItems = SourcePaging.TotalItems;
            Paging.PageIndex = SourcePaging.PageIndex;
            Paging.PageSize = SourcePaging.PageSize;

            List = SourceList;
        }

        // ● properties
        [Description("Paging information.")]
        [JsonPropertyOrder(-899)]
        public Paging Paging { get; set; } = new(); 
    }
}
