namespace MvcApp.Library
{
    /// <summary>
    /// The mode of a pager button
    /// </summary>
    public enum PagerButtonMode
    {
        /// <summary>
        /// Button is just an empty placeholder
        /// </summary>
        Empty = 0,
        /// <summary>
        /// Button displays a page number an contains a link to the page
        /// </summary>
        Page = 1,
        /// <summary>
        /// Button displays ... as a sign that there are more pages
        /// </summary>
        More = 2
    }
}
