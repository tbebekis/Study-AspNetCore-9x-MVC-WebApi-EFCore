namespace MvcApp.Library
{
    /// <summary>
    /// Helper. Builds html markup for items such as script and css files.
    /// <para>WARNING: This service should be registered as a Scoped service. Scoped : once per HTTP Request
    /// </para>
    /// </summary>
    public class PageBuilderService
    {
        /* private */
        class MetaPart
        {
            public MetaPart(string Text = "", int DisplayOrder = 0)
            {
                this.Text = Text;
                this.DisplayOrder = DisplayOrder;
            }

            public string Text { get; set; }
            public int DisplayOrder { get; set; }
        }
        /// <summary>
        /// A css or javascript file reference, or an inline script block, or a script line in the tp.Ready() function.
        /// </summary>
        class FileRef
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public FileRef(string Source, int DisplayOrder = 0, string FallBack = "", bool IsAsync = false, bool Defer = false)
            {
                this.Source = Source;
                this.DisplayOrder = DisplayOrder;
                this.FallBack = FallBack;
                this.IsAsync = IsAsync;
                this.Defer = Defer;
            }

            /// <summary>
            /// The url of the resource, e.g. https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css
            /// </summary>
            public string Source { get; set; }
            /// <summary>
            /// A local fall-back url used when a cdn url fails, e.g. /tp/css/font-awesome.min.css
            /// </summary>
            public string FallBack { get; set; }
            /// <summary>
            /// async attribute value. Used with javascripts only.
            /// </summary>
            public bool IsAsync { get; set; }
            /// <summary>
            /// defer attribute value. Used with javascripts only.
            /// </summary>
            public bool Defer { get; set; }

            public int DisplayOrder { get; set; }
        }


        System.Threading.Lock syncLock = new();

        IActionContextAccessor fActionContextAccessor;
        IUrlHelperFactory fUrlHelperFactory;



        List<MetaPart> TitleParts = new List<MetaPart>();
        List<MetaPart> MetaKeywordParts = new List<MetaPart>();
        string fMetaDescription = null;

        List<FileRef> HeadScriptFiles = new List<FileRef>();
        List<FileRef> FooterScriptFiles = new List<FileRef>();

        List<FileRef> HeadCssFiles = new List<FileRef>();
        List<FileRef> FooterCssFiles = new List<FileRef>();

        List<FileRef> HeadInlineScripts = new List<FileRef>();
        List<FileRef> FooterInlineScripts = new List<FileRef>();

        List<FileRef> ReadyScriptBlocks = new List<FileRef>();

        Dictionary<string, string> NamedTextDic = new Dictionary<string, string>();

        bool CanAdd(List<FileRef> List, string Source)
        {
            return !string.IsNullOrWhiteSpace(Source)
                && List.FirstOrDefault(item => item.Source.ToLowerInvariant() == Source.ToLowerInvariant()) == null;
        }

        string GenerateScriptFiles(List<FileRef> List)
        {
            if (List.Count > 0)
            {
                List.Sort((A, B) => { return A.DisplayOrder - B.DisplayOrder; });

                IUrlHelper UrlHelper = fUrlHelperFactory.GetUrlHelper(fActionContextAccessor.ActionContext);

                StringBuilder SB = new StringBuilder();
                string S;
                string Async;
                string Defer;
                string FallBack;
                int Counter = 0;
                foreach (var Item in List)
                {
                    Async = Item.IsAsync ? "async" : string.Empty;
                    Defer = Item.Defer ? "defer" : string.Empty;

                    FallBack = "";
                    if (!string.IsNullOrWhiteSpace(Item.FallBack))
                    {
                        FallBack = $" onerror='CssOrScriptFallBack(this)' data-fallback='{Item.FallBack}' ";
                    }
                    S = $"<script {Async} {Defer} src='{UrlHelper.Content(Item.Source)}' {FallBack}></script>";
                    SB.AppendLine(Counter > 0 ? "    " + S : S);

                    Counter++;
                }

                return SB.ToString();
            }

            return string.Empty;
        }
        string GenerateInlineScripts(List<FileRef> List)
        {
            if (List.Count > 0)
            {
                List.Sort((A, B) => { return A.DisplayOrder - B.DisplayOrder; });

                StringBuilder SB = new StringBuilder();
                foreach (var Item in List)
                {
                    SB.AppendLine("");

                    if (!Item.Source.Trim().StartsWith("<script>"))
                    {
                        SB.AppendLine("<script>");
                        SB.AppendLine(Item.Source);
                        SB.AppendLine("</script>");
                    }
                    else
                    {
                        SB.AppendLine(Item.Source);
                    }
                }

                return SB.ToString();
            }

            return string.Empty;
        }
        string GenerateCssFiles(List<FileRef> List)
        {
            if (List.Count > 0)
            {
                List.Sort((A, B) => { return A.DisplayOrder - B.DisplayOrder; });

                IUrlHelper UrlHelper = fUrlHelperFactory.GetUrlHelper(fActionContextAccessor.ActionContext);

                StringBuilder SB = new StringBuilder();
                string S;
                string FallBack;
                int Counter = 0;
                foreach (var Item in List)
                {
                    FallBack = "";
                    if (!string.IsNullOrWhiteSpace(Item.FallBack))
                    {
                        FallBack = $" onerror='CssOrScriptFallBack(this)' data-fallback='{Item.FallBack}' ";
                    }
                    S = $"<link href='{UrlHelper.Content(Item.Source)}' rel='stylesheet' {FallBack} />";
                    SB.AppendLine(Counter > 0 ? "    " + S : S);

                    Counter++;
                }

                return SB.ToString();
            }

            return string.Empty;
        }

        /* construction */
        /// <summary>
        /// Constructor
        /// </summary>
        public PageBuilderService(IActionContextAccessor ActionContextAccessor, IUrlHelperFactory UrlHelperFactory)
        {
            fActionContextAccessor = ActionContextAccessor;
            fUrlHelperFactory = UrlHelperFactory;
        }

        /* public */

        /* ● meta-tag parts (Title, MetaDescription, MetaKeywords) */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        public void AddPageTitlePart(string Text, int DisplayOrder = 0)
        {
            if (!string.IsNullOrWhiteSpace(Text))
            {
                lock (syncLock)
                {
                    TitleParts.Add(new MetaPart(Text, DisplayOrder));
                }
            }
        }
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        public void AddMetaKeywordsPart(string Text, int DisplayOrder = 0)
        {
            if (!string.IsNullOrWhiteSpace(Text))
            {
                lock (syncLock)
                {
                    MetaKeywordParts.Add(new MetaPart(Text, DisplayOrder));
                }
            }
        }
        /// <summary>
        /// Sets the description meta-tag value
        /// </summary>
        public void SetMetaDescription(string Text)
        {
            if (!string.IsNullOrWhiteSpace(Text))
                fMetaDescription = Text;
        }


        /* ● Head script files */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        public void AddScriptFileToHead(string Url, int DisplayOrder = 0, string FallBack = "", bool Async = false, bool Defer = false)
        {
            lock (syncLock)
            {
                if (CanAdd(HeadScriptFiles, Url))
                    HeadScriptFiles.Add(new FileRef(Url, DisplayOrder, FallBack, Async, Defer));
            }
        }


        /* ● Footer script files */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        public void AddScriptFileToFooter(string Url, int DisplayOrder = 0, string FallBack = "", bool Async = false, bool Defer = false)
        {
            lock (syncLock)
            {
                if (CanAdd(FooterScriptFiles, Url))
                    FooterScriptFiles.Add(new FileRef(Url, DisplayOrder, FallBack, Async, Defer));
            }
        }


        /* ● Head inline javascript */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        public void AddInlineScriptToHead(string Source, int DisplayOrder = 0)
        {
            lock (syncLock)
            {
                if (CanAdd(HeadInlineScripts, Source))
                    HeadInlineScripts.Add(new FileRef(Source, DisplayOrder));
            }
        }

        /* ● Footer inline javascript */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        public void AddInlineScriptToFooter(string Source, int DisplayOrder = 0)
        {
            lock (syncLock)
            {
                if (CanAdd(FooterInlineScripts, Source))
                    FooterInlineScripts.Add(new FileRef(Source, DisplayOrder));
            }
        }

        /* ● tp.Ready() javascript */
        /// <summary>
        /// Adds javascript source code to the tp.Ready() script block
        /// </summary>
        public void AddToReadyScriptBlock(string Source, int DisplayOrder = 0)
        {
            lock (syncLock)
            {
                if (!string.IsNullOrWhiteSpace(Source))
                    ReadyScriptBlocks.Add(new FileRef(Source.Trim(), DisplayOrder)); //sbReady.AppendLine(Source.Trim());
            }
        }

        /* ● Head Css files */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        public void AddCssFileToHead(string Url, int DisplayOrder = 0, string FallBack = "")
        {
            lock (syncLock)
            {
                if (CanAdd(HeadCssFiles, Url))
                    HeadCssFiles.Add(new FileRef(Url, DisplayOrder, FallBack));
            }
        }

        /* ● Footer Css files */
        /// <summary>
        /// Adds an item to the internal list
        /// </summary>
        public void AddCssFileToFooter(string Url, int DisplayOrder = 0, string FallBack = "")
        {
            lock (syncLock)
            {
                if (CanAdd(FooterCssFiles, Url))
                    FooterCssFiles.Add(new FileRef(Url, DisplayOrder, FallBack));
            }
        }

        /* ● Named Text */
        /// <summary>
        /// Adds a named text to an internal dictionary.
        /// <para>A named text could be a script part or css part or other text, under a unique name.</para>
        /// </summary>
        public void AddNamedText(string Name, string Text)
        {
            lock (syncLock)
            {
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    NamedTextDic[Name] = !string.IsNullOrWhiteSpace(Text) ? Text : string.Empty;
                }
            }

        }
        /// <summary>
        /// Returns a text under a name.
        /// <para>A named text could be a script part or css part or other text, under a unique name.</para>
        /// </summary>
        public string GetNamedText(string Name)
        {
            if (NamedTextDic.ContainsKey(Name))
                return NamedTextDic[Name];
            return string.Empty;
        }

        /* ● generate meta tags (Title, MetaDescription, MetaKeywords) */
        /// <summary>
        /// Generates and returns the html markup for the specified items
        /// </summary>
        public string GeneratePageTitle(string Text = "")
        {
            lock (syncLock)
            {
                string Result = "";
                string Separator = !string.IsNullOrWhiteSpace(Seo.TitleSeparator) ? Seo.TitleSeparator : " | ";

                if (!string.IsNullOrWhiteSpace(Text))
                {
                    TitleParts.Add(new MetaPart(Text, int.MaxValue));
                }

                TitleParts.Sort((A, B) => { return A.DisplayOrder - B.DisplayOrder; });

                if (!string.IsNullOrWhiteSpace(Seo.DefaultMetaTitle))
                {
                    if (Seo.AppendDefaultMetaTitle)
                        TitleParts.Add(new MetaPart(Seo.DefaultMetaTitle, int.MaxValue));
                    else
                        TitleParts.Insert(0, new MetaPart(Seo.DefaultMetaTitle, 0));
                }

                List<string> List = TitleParts.Aggregate(new List<string>(), (List, Item) => {
                    List.Add(Item.Text);
                    return List;
                });


                if (List != null && List.Count > 0)
                {
                    Result = $"<title>{string.Join(Separator, List)}</title>";
                }

                return Result;

            }
        }
        /// <summary>
        /// Generates and returns the html markup for the specified items
        /// </summary>
        public string GenerateMetaKeywords(string Text = "")
        {
            lock (syncLock)
            {
                string Result = "";

                if (!string.IsNullOrWhiteSpace(Text))
                {
                    MetaKeywordParts.Add(new MetaPart(Text, int.MaxValue));
                }

                MetaKeywordParts.Sort((A, B) => { return A.DisplayOrder - B.DisplayOrder; });

                if (!string.IsNullOrWhiteSpace(Seo.DefaultMetaKeywords))
                {
                    MetaKeywordParts.Add(new MetaPart(Seo.DefaultMetaKeywords, int.MaxValue));
                }


                List<string> List = MetaKeywordParts.Aggregate(new List<string>(), (List, Item) => {
                    List.Add(Item.Text);
                    return List;
                });

                if (List != null && List.Count > 0)
                {
                    StringBuilder SB = new StringBuilder();
                    string S;
                    for (int i = 0; i < List.Count; i++)
                    {
                        S = List[i].Trim();
                        if (i < List.Count - 1 && !S.EndsWith(','))
                            S += ",";
                        SB.Append(S);
                    }

                    Result = $"<meta name='keywords' content='{SB}' />";
                }

                return Result;

            }
        }
        /// <summary>
        /// Generates and returns the html markup for the specified items
        /// </summary>
        public string GenerateMetaDescription(string Text = "")
        {
            string Result = "";

            StringBuilder SB = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(Text))
            {
                if (SB.Length > 0)
                    SB.Append("; ");
                SB.Append(Text);
            }

            if (!string.IsNullOrWhiteSpace(fMetaDescription))
            {
                if (SB.Length > 0)
                    SB.Append("; ");
                SB.Append(fMetaDescription);
            }

            if (!string.IsNullOrWhiteSpace(Seo.DefaultMetaDescription))
            {
                if (SB.Length > 0)
                    SB.Append("; ");
                SB.Append(Seo.DefaultMetaDescription);
            }

            if (SB.Length > 0)
            {
                Result = $"<meta name='description' content='{SB}' />";
            }

            return Result;
        }

        /* ● generate script files */
        /// <summary>
        /// Generates and returns the html markup for the specified items
        /// </summary>
        public string GenerateHeadScriptFiles()
        {
            lock (syncLock)
            {
                return GenerateScriptFiles(HeadScriptFiles);
            }
        }
        /// <summary>
        /// Generates and returns the html markup for the specified items
        /// </summary>
        public string GenerateFooterScriptFiles()
        {
            lock (syncLock)
            {
                return GenerateScriptFiles(FooterScriptFiles);
            }
        }

        /* ● generate inline scripts */
        /// <summary>
        /// Generates and returns the html markup for the specified items
        /// </summary>
        public string GenerateHeadInlineScripts()
        {
            lock (syncLock)
            {
                return GenerateInlineScripts(HeadInlineScripts);
            }
        }
        /// <summary>
        /// Generates and returns the html markup for the specified items
        /// </summary>
        public string GenerateFooterInlineScripts()
        {
            lock (syncLock)
            {
                return GenerateInlineScripts(FooterInlineScripts);
            }
        }

        /* ● generate css files */
        /// <summary>
        /// Generates and returns the html markup for the specified items
        /// </summary>
        public string GenerateHeadCssFiles()
        {
            lock (syncLock)
            {
                return GenerateCssFiles(HeadCssFiles);
            }
        }
        /// <summary>
        /// Generates and returns the html markup for the specified items
        /// </summary>
        public string GenerateFooterCssFiles()
        {
            lock (syncLock)
            {
                return GenerateCssFiles(FooterCssFiles);
            }
        }

        /* ● generate tp.Ready() script block */
        /// <summary>
        /// Generates and returns the tp.Ready() script block, if any, else empty string.
        /// </summary>
        public string GenerateReadyScriptBlock()
        {
            lock (syncLock)
            {
                ReadyScriptBlocks.Sort((A, B) => { return A.DisplayOrder - B.DisplayOrder; });
                StringBuilder sbReady = new StringBuilder();
                foreach (var Item in ReadyScriptBlocks)
                    sbReady.AppendLine(Item.Source);


                if (sbReady.Length == 0)
                    return string.Empty;

                StringBuilder SB = new StringBuilder();
                SB.AppendLine();
                SB.AppendLine("<script>");
                SB.AppendLine("    tp.Ready(() => {");
                SB.AppendLine(sbReady.ToString());
                SB.AppendLine("    });");
                SB.AppendLine("</script>");
                SB.AppendLine("");
                SB.AppendLine();

                return SB.ToString();
            }
        }

        /// <summary>
        /// SEO related settings
        /// </summary>
        static public SeoSettings Seo => Lib.Settings.Seo;
    }
}
