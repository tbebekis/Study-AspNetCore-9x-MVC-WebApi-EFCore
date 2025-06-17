namespace MvcApp.Library
{
    /// <summary>
    /// Paging information, for category etc. pages
    /// </summary>
    public class PagingInfo
    {

        int fButtonPlaces = 4;

        void Calculate()
        {
            if ((PageSize <= 0) || (TotalItems <= 0))
                this.TotalPages = 0;
            else if (TotalItems < PageSize)
                this.TotalPages = 1;
            else
            {
                this.TotalPages = TotalItems / PageSize;
                if (TotalItems % PageSize > 0)
                    TotalPages++;
            }
        }
        void PrepareButtons()
        {
            List<PagerButton> List = new List<PagerButton>();

            PagerButton Button;

            int StartIndex;
            if (PageIndex >= 0 && PageIndex <= ButtonPlaces - 1)
            {
                StartIndex = 0;
            }
            else if (PageIndex >= TotalPages - (ButtonPlaces + 1) && PageIndex <= TotalPages - 1)
            {
                StartIndex = TotalPages - (ButtonPlaces);
            }
            else
            {
                StartIndex = PageIndex;
            }


            for (int i = 0; i < ButtonPlaces; i++)
            {
                Button = new PagerButton();
                List.Add(Button);

                Button.Mode = PagerButtonMode.Page;
                Button.PageIndex = StartIndex + i;

                if (Button.PageIndex > TotalPages - 1)
                {
                    Button.PageIndex = -1;
                    Button.Mode = PagerButtonMode.Empty;
                }

                Button.IsCurrentPage = Button.PageIndex == PageIndex;
            }


            Buttons = List.ToArray();
        }

        /* construction */
        /// <summary>
        /// Constructor
        /// </summary>
        public PagingInfo(int TotalItems = 0)
        {
            this.ViewMode = GetQueryStringListViewMode();

            this.PageIndex = GetQueryStringPageIndex();
            this.PageSize = GetQueryStringPageSize();

            this.TotalItems = TotalItems <= 0 ? 0 : TotalItems;

            Calculate();
            PrepareButtons();
        }


        /// <summary>
        /// Returns the "pagenumber" parameter from the current query string and converts it to PageIndex.
        /// <para>NOTE: PageIndex is 0-based, where PageNumber is 1-based.</para>
        /// </summary>
        static public int GetQueryStringPageIndex()
        {
            int Default = 1;
            int Result = Lib.GetQueryValue("pagenumber", Default);

            // PageNumber is PageIndex + 1
            return Result > 0 ? Result - 1 : 0;
        }
        /// <summary>
        /// Returns the "pagesize" parameter from the current query string
        /// </summary>
        static public int GetQueryStringPageSize()
        {
            int Default = DefaultPageSize <= 0? 10: DefaultPageSize;
            return Lib.GetQueryValue("pagesize", Default);
        }
        /// <summary>
        /// Returns the "viewmode" parameter from the current query string
        /// </summary>
        static public PageViewMode GetQueryStringListViewMode()
        {
            int Default = (int)PageViewMode.Grid;
            return (PageViewMode)Lib.GetQueryValue("viewmode", Default);
        }


        /// <summary>
        /// The current page index. 0 based.
        /// </summary>
        public int PageIndex { get; }
        /// <summary>
        /// The number of the page. 1-based
        /// </summary>
        public int PageNumber => PageIndex + 1;
        /// <summary>
        /// The number of items in a page.
        /// </summary>
        public int PageSize { get; }
        /// <summary>
        /// The total items.
        /// </summary>
        public int TotalItems { get; }

        /// <summary>
        /// The total pages.
        /// </summary>
        public int TotalPages { get; private set; }
        /// <summary>
        /// The view mode of the page. Possible values: grid and list
        /// </summary>
        public PageViewMode ViewMode { get; }


        /// <summary>
        /// How many pager buttons to display on a page
        /// </summary>
        public int ButtonPlaces
        {
            get { return fButtonPlaces; }
            set
            {
                if (value != ButtonPlaces && value > 4)
                {
                    fButtonPlaces = value;
                    PrepareButtons();
                }
            }
        }
        /// <summary>
        /// The list of the buttons to display
        /// </summary>
        public PagerButton[] Buttons { get; private set; }


        public bool HasPreviousPage => PageIndex > 0 && PageIndex <= TotalPages - 1;
        public bool HasNextPage => PageIndex >= 0 && PageIndex < TotalPages - 1;

        public string PagePrevText => (PageNumber - 1).ToString();
        public string PageNextText => (PageNumber + 1).ToString();

        static public int DefaultPageSize { get; set; }


    }
}
