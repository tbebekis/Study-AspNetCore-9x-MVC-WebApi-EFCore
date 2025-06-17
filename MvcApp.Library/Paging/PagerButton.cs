namespace MvcApp.Library
{
    public class PagerButton
    {
        public PagerButtonMode Mode { get; set; } = PagerButtonMode.Empty;
        public int PageIndex { get; set; } = -1;
        public int PageNumber => PageIndex < 0 ? 0 : PageIndex + 1;
        public string DisplayText
        {
            get
            {
                if (Mode == PagerButtonMode.Page)
                    return PageNumber.ToString();
                if (Mode == PagerButtonMode.More)
                    return "...";
                return "";
            }
        }
        public bool IsCurrentPage { get; set; }
    }
}
