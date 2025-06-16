namespace MvcApp.Library
{
    /// <summary>
    /// SEO related settings
    /// </summary>
    public class SeoSettings
    {
        /* construction */
        /// <summary>
        /// Constructor
        /// </summary>
        public SeoSettings()
        {
        }

        /// <summary>
        /// Default title. Rendered only when it is not empty.
        /// </summary>
        public string DefaultMetaTitle { get; set; }
        /// <summary>
        /// Default meta keywords
        /// </summary>
        public string DefaultMetaKeywords { get; set; }
        /// <summary>
        /// Default meta description
        /// </summary>
        public string DefaultMetaDescription { get; set; }

        /// <summary>
        /// Page title separator
        /// </summary>
        public string TitleSeparator { get; set; }
        /// <summary>
        /// True appends the default meta-title to the whole title. False, the default, pre-pends the default meta-title.
        /// </summary>
        public bool AppendDefaultMetaTitle { get; set; }

    }
}
