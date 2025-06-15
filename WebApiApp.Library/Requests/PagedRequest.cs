namespace WebApiApp.Library
{
    [Description("A request for getting back paged data.")]
    public class PagedRequest
    {
        int fPageIndex;
        int fPageSize;

        [Description("The page index. Zero based."), DefaultValue(0)]
        public int PageIndex
        {
            get => fPageIndex <= 0? 0: fPageIndex;
            set => fPageIndex = value;
        }
        [Description("The size of the page."), DefaultValue(5)]
        public int PageSize
        {
            get => fPageSize <= 5? 5: fPageSize;
            set => fPageSize = value;
        }
    }
}
