namespace CommonLib
{
    public interface IPaging
    {
        /// <summary>
        /// The number of total items when this is a paged response. 
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        int TotalItems { get; set; }
        /// <summary>
        /// The number of items in a page.
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// The current page index. 0 based.
        /// <para><strong>NOTE: </strong>Used only with paged responses.</para>
        /// </summary>
        int PageIndex { get; set; }
    }
}
