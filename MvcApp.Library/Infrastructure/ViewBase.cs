namespace MvcApp.Library
{
    /// <summary>
    /// View page class used by this application
    /// </summary>
    public abstract class ViewBase<TModel> : RazorPage<TModel>
    {
        // ● public 
        /// <summary>
        /// Returns a localized string based on a specified resource key, e.g. Customer, and the current (Session's) culture code, e.g. el-GR
        /// </summary>
        public HtmlString L(string Key, params object[] Args)
        {
            string S = Res.GetString(Key);  
            if ((Args != null) && (Args.Length > 0))
                S = string.Format(S, Args);
            return new HtmlString(S);
        }  
        /// <summary>
        /// Returns a string as an <see cref="HtmlString"/>
        /// </summary>
        public HtmlString ToHtml(string S, params object[] Args)
        {
            if ((Args != null) && (Args.Length > 0))
                S = string.Format(S, Args);
            return new HtmlString(S);
        }

        // ● properties 
        /// <summary>
        /// Returns the absolute Url of this page, along with the Query String, encoded.
        /// </summary>
        public string AbsoluteUrlEncoded => this.Context.Request.GetEncodedUrl();
        /// <summary>
        /// Returns the relative Url of this page, along with the Query String, encoded.
        /// </summary>
        public string RelativeUrlEncoded => this.Context.Request.GetEncodedPathAndQuery();
        /// <summary>
        /// Returns the relative Url of this page, along with the Query String, encoded.
        /// </summary>
        public string RelativeRawUrlEncoded => Lib.GetRelativeRawUrlEncoded(this.Context.Request);

        /// <summary>
        /// Return the file path of the view
        /// </summary>
        public string ViewFilePath { get { return ViewContext.ExecutingFilePath; } }
        /// <summary>
        /// Returns the file name of the view
        /// </summary>
        public string ViewFileName { get { return System.IO.Path.GetFileName(ViewContext.ExecutingFilePath); } }

        /// <summary>
        /// Gets or sets the title of the page
        /// </summary>
        public string PageTitle
        {
            get { return ViewData["Title"] as string; }
            set { ViewData["Title"] = value; }
        }
        /// <summary>
        /// Gets or sets the description of the page
        /// </summary>
        public string PageDescription
        {
            get { return ViewData["Description"] as string; }
            set { ViewData["Description"] = value; }
        }
    }
}
