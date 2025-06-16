namespace MvcApp.Library
{
    /// <summary>
    /// A slightly better (I hope) <see cref="TagBuilder"/> that supports nested tags.
    /// </summary>
    public class HtmlTag : TagBuilder
    {
        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public HtmlTag(string TagName)
            : base(TagName)
        {
            if (string.Compare("input", TagName, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                this.TagRenderMode = TagRenderMode.SelfClosing;
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public HtmlTag(string TagName, string CssClasses)
            : this(TagName)
        {
            this.Class(CssClasses);
        }

        // ● public 
        /// <summary>
        /// Returns a string representation of this instance
        /// </summary>
        public override string ToString()
        {
            return this.TagName;
        }

        /// <summary>
        /// Adds a tag to the <see cref="InnerTags"/> and returns the newly added <see cref="HtmlTag"/>
        /// </summary>
        public HtmlTag Add(string TagName, string CssClasses = "")
        {
            HtmlTag Result = new HtmlTag(TagName, CssClasses);
            this.InnerTags.Add(Result);
            return Result;
        }
        /// <summary>
        /// Adds a tag to the <see cref="InnerTags"/> and returns the newly added <see cref="HtmlTag"/>
        /// </summary>
        public HtmlTag Add(HtmlTag Tag)
        {
            this.InnerTags.Add(Tag);
            return Tag;
        }
        /// <summary>
        /// Adds a tag to the <see cref="InnerTags"/> and returns the newly added <see cref="HtmlTag"/>
        /// </summary>
        public HtmlTag AddInput(string InputType, string CssClasses = "")
        {
            HtmlTag Result = Add("input", CssClasses);
            Result.Attribute("type", InputType);
            return Result;
        }
        /// <summary>
        /// Adds a new attribute or replaces an existing one, if the specified key is not null or empty. Returns this instance.
        /// </summary>
        public HtmlTag Attribute(string Key, string Value)
        {
            if (!string.IsNullOrWhiteSpace(Key))
                this.MergeAttribute(Key, Value, replaceExisting: true);
            return this;
        }
        /// <summary>
        /// Adds one or more class names, delimited by a space, to the tag's css classes, if the specified class names is not a null or empty string. Returns this instance.
        /// </summary>
        public HtmlTag Class(string ClassNames)
        {
            if (!string.IsNullOrWhiteSpace(ClassNames))
                this.AddCssClass(ClassNames);
            return this;
        }

        /// <summary>
        /// Writes the tag by encoding it with the specified encoder to the specified  writer.
        /// </summary>
        new public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }

            this.WriteTo(writer, encoder, TagRenderMode);
        }

        // ● static 
        /// <summary>
        /// Creates and returns a new <see cref="HtmlTag"/>
        /// </summary>
        static public HtmlTag Div(string CssClasses = "")
        {
            return new HtmlTag("div", CssClasses);
        }
        /// <summary>
        /// Creates and returns a new <see cref="HtmlTag"/>
        /// </summary>
        static public HtmlTag Button(string Text, string CssClasses = "")
        {
            HtmlTag Result = new HtmlTag("button", CssClasses);
            Result.Attribute("type", "button");
            Result.InnerHtml.Append(Text);
            return Result;
        }
        /// <summary>
        /// Creates and returns a new <see cref="HtmlTag"/>
        /// </summary>
        static public HtmlTag Submit(string Text = "Submit", string CssClasses = "")
        {
            HtmlTag Result = new HtmlTag("button", CssClasses);
            Result.Attribute("type", "submit");
            Result.InnerHtml.Append(Text);
            return Result;
        }
        /// <summary>
        /// Creates and returns a new <see cref="HtmlTag"/>
        /// </summary>
        static public HtmlTag Reset(string Text = "Reset", string CssClasses = "")
        {
            HtmlTag Result = new HtmlTag("button", CssClasses);
            Result.Attribute("type", "reset");
            Result.InnerHtml.Append(Text);
            return Result;
        }
        /// <summary>
        /// Creates and returns a new <see cref="HtmlTag"/>
        /// </summary>
        static public HtmlTag Span(string Text, string CssClasses = "")
        {
            HtmlTag Result = new HtmlTag("span", CssClasses);
            Result.InnerHtml.Append(Text);
            return Result;
        }
        /// <summary>
        /// Creates and returns a new <see cref="HtmlTag"/>
        /// </summary>
        static public HtmlTag Label(string Text, string CssClasses = "")
        {
            HtmlTag Result = new HtmlTag("label", CssClasses);
            Result.InnerHtml.Append(Text);
            return Result;
        }
        /// <summary>
        /// Creates and returns a new <see cref="HtmlTag"/>
        /// </summary>
        static public HtmlTag Hidden()
        {
            return new HtmlTag("input").Attribute("type", "hidden");
        }
        /// <summary>
        /// Creates and returns a new <see cref="HtmlTag"/>
        /// </summary>
        static public HtmlTag TextBox(string CssClasses = "")
        {
            return Input("checkbox", CssClasses);
        }
        /// <summary>
        /// Creates and returns an input tag
        /// </summary>
        static public HtmlTag Input(string InputType, string CssClasses = "")
        {
            HtmlTag Result = new HtmlTag("input", CssClasses);
            Result.Attribute("type", InputType);
            return Result;
        }

        // ● properties 
        /// <summary>
        /// The list of inner tags
        /// </summary>
        public List<HtmlTag> InnerTags { get; } = new List<HtmlTag>();

        static internal void Test()
        {
            HtmlTag Div = HtmlTag.Div();
            Div.Attribute("style", "background-color: red");
            Div.Attribute("title", "this is a hint");
            Div.Add(HtmlTag.Label("HtmlTag"));
            Div.Add(HtmlTag.TextBox().Class("class1 class2"));

            TagBuilder TextDiv = new TagBuilder("div");
            TagBuilder Label = new TagBuilder("label");
            Label.InnerHtml.SetContent("Hi there");
            TextDiv.InnerHtml.AppendHtml(Label);

            string Text = Div.ToHtml();
        }
    }
}
