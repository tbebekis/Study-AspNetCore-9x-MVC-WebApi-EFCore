namespace MvcApp.Library
{
    /// <summary>
    /// Helper. Builds html markup for items such as script and css files.
    /// </summary>
    static public class PageBuilder
    {
        public const int MasterLayoutDisplayOrder = 0;
        public const int PageLayoutDisplayOrder = MasterLayoutDisplayOrder + 100;
        public const int PageDisplayOrder = PageLayoutDisplayOrder + 100;
        public const int SubPageDisplayOrder = PageDisplayOrder + 100;
        public const int PartialDisplayOrder = SubPageDisplayOrder + 100;

        static PageBuilderService GetPageBuilderService()
        {
            return Lib.GetService<PageBuilderService>();
        }

        /* ● meta-tags (Title, MetaDescription, MetaKeywords) */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        static public void AddPageTitlePart(string Text, int DisplayOrder = MasterLayoutDisplayOrder)
        {
            var Builder = GetPageBuilderService();
            Builder.AddPageTitlePart(Text, DisplayOrder);
        }
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        static public void AddMetaKeywordsPart(string Text, int DisplayOrder = MasterLayoutDisplayOrder)
        {
            var Builder = GetPageBuilderService();
            Builder.AddMetaKeywordsPart(Text, DisplayOrder);
        }
        /// <summary>
        /// Sets the description meta-tag value
        /// </summary>
        static public void SetMetaDescription(string Text)
        {
            var Builder = GetPageBuilderService();
            Builder.SetMetaDescription(Text);
        }

        /* ● Head script files */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        static public void AddScriptFileToHead(string Url, int DisplayOrder = MasterLayoutDisplayOrder, string FallBack = "", bool Async = false, bool Defer = false)
        {
            var Builder = GetPageBuilderService();
            Builder.AddScriptFileToHead(Url, DisplayOrder, FallBack, Async, Defer);
        }


        /* ● Footer script files */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        static public void AddScriptFileToFooter(string Url, int DisplayOrder = MasterLayoutDisplayOrder, string FallBack = "", bool Async = false, bool Defer = false)
        {
            var Builder = GetPageBuilderService();
            Builder.AddScriptFileToFooter(Url, DisplayOrder, FallBack, Async, Defer);
        }


        /* ● Head inline javascript */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        static public void AddInlineScriptToHead(string SourceCode, int DisplayOrder = MasterLayoutDisplayOrder)
        {
            var Builder = GetPageBuilderService();
            Builder.AddInlineScriptToHead(SourceCode, DisplayOrder);
        }


        /* ● Footer inline javascript */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        static public void AddInlineScriptToFooter(string SourceCode, int DisplayOrder = MasterLayoutDisplayOrder)
        {
            var Builder = GetPageBuilderService();
            Builder.AddInlineScriptToFooter(SourceCode, DisplayOrder);
        }


        /* ● tp.Ready() javascript */
        /// <summary>
        /// Adds javascript source code to the tp.Ready() script block
        /// </summary>
        static public void AddToReadyScriptBlock(string SourceCode, int DisplayOrder = MasterLayoutDisplayOrder)
        {
            var Builder = GetPageBuilderService();
            Builder.AddToReadyScriptBlock(SourceCode, DisplayOrder);
        }

        /* ● Head Css files */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        static public void AddCssFileToHead(string Url, int DisplayOrder = MasterLayoutDisplayOrder, string FallBack = "")
        {
            var Builder = GetPageBuilderService();
            Builder.AddCssFileToHead(Url, DisplayOrder, FallBack);
        }


        /* ● Footer Css files */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        static public void AddCssFileToFooter(string Url, int DisplayOrder = MasterLayoutDisplayOrder, string FallBack = "")
        {
            var Builder = GetPageBuilderService();
            Builder.AddCssFileToFooter(Url, DisplayOrder, FallBack);
        }

        /* ● Named Text */
        /// <summary>
        /// Adds a named text to an internal dictionary.
        /// <para>A named text could be a script part or css part or other text, under a unique name.</para>
        /// </summary>
        static public void AddNamedText(string Name, string Text)
        {
            var Builder = GetPageBuilderService();
            Builder.AddNamedText(Name, Text);

        }
        /// <summary>
        /// Returns a text under a name.
        /// <para>A named text could be a script part or css part or other text, under a unique name.</para>
        /// </summary>
        static public IHtmlContent RenderNamedText(string Name)
        {
            var Builder = GetPageBuilderService();
            return new HtmlString(Builder.GetNamedText(Name));
        }


        /* ● render meta-tags (Title, MetaDescription, MetaKeywords) */
        /// <summary>
        /// Renders the html markup for the specified items
        /// </summary>
        static public IHtmlContent RenderPageTitle(string Text = "")
        {
            var Builder = GetPageBuilderService();
            return new HtmlString(Builder.GeneratePageTitle(Text));
        }
        /// <summary>
        /// Renders the html markup for the specified items
        /// </summary>
        static public IHtmlContent RenderMetaKeywords(string Text = "")
        {
            var Builder = GetPageBuilderService();
            return new HtmlString(Builder.GenerateMetaKeywords(Text));
        }
        /// <summary>
        /// Renders the html markup for the specified items
        /// </summary>
        static public IHtmlContent RenderMetaDescription(string Text = "")
        {
            var Builder = GetPageBuilderService();
            return new HtmlString(Builder.GenerateMetaDescription(Text));
        }

        /* ● render script files */
        /// <summary>
        /// Renders the html markup for the specified items
        /// </summary>
        static public IHtmlContent RenderHeadScriptFiles()
        {
            var Builder = GetPageBuilderService();
            return new HtmlString(Builder.GenerateHeadScriptFiles());
        }
        /// <summary>
        /// Renders the html markup for the specified items
        /// </summary>
        static public IHtmlContent RenderFooterScriptFiles()
        {
            var Builder = GetPageBuilderService();
            return new HtmlString(Builder.GenerateFooterScriptFiles());
        }

        /* ● render inline scripts */
        /// <summary>
        /// Renders the html markup for the specified items
        /// </summary>
        static public IHtmlContent RenderHeadInlineScripts()
        {
            var Builder = GetPageBuilderService();
            return new HtmlString(Builder.GenerateHeadInlineScripts());
        }
        /// <summary>
        /// Renders the html markup for the specified items
        /// </summary>
        static public IHtmlContent RenderFooterInlineScripts()
        {
            var Builder = GetPageBuilderService();
            return new HtmlString(Builder.GenerateFooterInlineScripts());
        }

        /* ● render css files */
        /// <summary>
        /// Renders the html markup for the specified items
        /// </summary>
        static public IHtmlContent RenderHeadCssFiles()
        {
            var Builder = GetPageBuilderService();
            return new HtmlString(Builder.GenerateHeadCssFiles());
        }
        /// <summary>
        /// Renders the html markup for the specified items
        /// </summary>
        static public IHtmlContent RenderFooterCssFiles()
        {
            var Builder = GetPageBuilderService();
            return new HtmlString(Builder.GenerateFooterCssFiles());
        }


        /* ● generate tp.Ready() script block */
        /// <summary>
        /// Generates and returns the tp.Ready() script block, if any, else empty string.
        /// </summary>
        static public IHtmlContent RenderReadyScriptBlock()
        {
            var Builder = GetPageBuilderService();
            return new HtmlString(Builder.GenerateReadyScriptBlock());
        }
    }
}
