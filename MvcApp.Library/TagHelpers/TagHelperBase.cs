namespace MvcApp.Library
{

    /// <summary>
    /// A <see cref="TagHelper"/> descendant class
    /// </summary>
    public abstract class TagHelperBase : TagHelper
    {

        // ● overridables 
        /// <summary>
        /// Creates and assigns the <see cref="MainTag"/> that is the main <see cref="HtmlTag"/> containing all output markup.
        /// </summary>
        protected abstract void CreateMainTag();
        /// <summary>
        /// Initializes the helper.
        /// <para>CAUTION: Should be called by the first line in <c>Process()</c> or <c>ProcessAsync()</c> method </para>
        /// </summary>
        protected virtual void Initialize(TagHelperContext context, TagHelperOutput output)
        {
            // contextualize IHtmlHelper 
            (HtmlHelper as IViewContextAware).Contextualize(ViewContext);
            CreateMainTag();

            this.TagHelperContext = context;
            this.Output = output;

            // collect html attributes already defined by the user in the html markup 
            foreach (var Entry in Output.Attributes)
                OutputAttributes.Add(Entry.Name, Entry.Value);
        }

        /// <summary>
        /// Generates and returns a unique id for an HTML Element.
        /// <para>WARNING: HTML element id is case-sensitive.</para>
        /// </summary>
        protected string NextId(string Prefix = "")
        {
            return ViewContext.HttpContext.NextId(Prefix);
        }

        // ● construction 
        /// <summary>
        /// Constructor
        /// </summary>
        public TagHelperBase(IHtmlGenerator generator, IHtmlHelper htmlHelper)
        {
            this.Generator = generator;
            this.HtmlHelper = htmlHelper;
        }

        // ● public  
        /// <summary>
        /// Initializes the <see cref="ITagHelper"/> with the given <paramref name="context"/>. Additions to
        /// <see cref="TagHelperContext.Items"/> should be done within this method to ensure they're added prior to
        /// executing the children.
        /// <para>NOTE: This method is called automatically by .Net</para>
        /// </summary>
        public override void Init(TagHelperContext context)
        {
            base.Init(context);

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
        /// <summary>
        /// Executes the <see cref="TagHelper"/> a-synchronously.
        /// <para>NOTE: It calls <see cref="Initialize(TagHelperContext, TagHelperOutput)"/> too. </para>
        /// <para>The .Net (Asp.Net Core 3.0) framework calls the <see cref="TagHelper.ProcessAsync(TagHelperContext, TagHelperOutput)"/> which calls the synchronous <c>Process()</c> in turn </para>
        /// </summary>
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            Initialize(context, output);
            return base.ProcessAsync(context, output);
        }

        // ● properties  
        /// <summary>
        /// The main <see cref="HtmlTag"/> containing all output markup.
        /// </summary>
        [HtmlAttributeNotBound]
        public HtmlTag MainTag { get; protected set; }
        /// <summary>
        /// Html helper
        /// </summary>
        [HtmlAttributeNotBound]
        public IHtmlHelper HtmlHelper { get; }
        /// <summary>
        /// Helper. Generates html form elements and more.
        /// </summary>
        [HtmlAttributeNotBound]
        public IHtmlGenerator Generator { get; }
        /// <summary>
        /// A <see cref="TagHelperContext"/> instance as passed to the <see cref="TagHelper"/> <c>Process()</c> method.
        /// </summary>
        [HtmlAttributeNotBound]
        public TagHelperContext TagHelperContext { get; private set; }
        /// <summary>
        /// A <see cref="TagHelperOutput"/> instance as passed to the <see cref="TagHelper"/> <c>Process()</c> method.
        /// </summary>
        [HtmlAttributeNotBound]
        public TagHelperOutput Output { get; private set; }
        /// <summary>
        /// View context already contextualized.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// A dictionary of html attributes already defined by the user in the html markup
        /// </summary>
        [HtmlAttributeNotBound]
        public Dictionary<string, object> OutputAttributes { get; } = new Dictionary<string, object>();
        /// <summary>
        /// Custom parameteres.
        /// </summary>
        [HtmlAttributeNotBound]
        public Dictionary<string, object> Params { get; } = new Dictionary<string, object>();
    }
}
