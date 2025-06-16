namespace MvcApp.Library
{
    /// <summary>
    /// A <see cref="TagHelperBase"/> descendant class.
    /// <para>For use with asp-for tag helpers, i.e. tag helpers the render a model's property or a full model.</para>
    /// </summary>
    public abstract class TagHelperAspForBase: TagHelperBase
    {
        /// <summary>
        /// Constant
        /// </summary>
        protected const string SFor = "asp-for";

        /// <summary>
        /// container for additional attributes
        /// </summary>
        protected Dictionary<string, object> HtmlAttributes = new Dictionary<string, object>();

        // ● overrides 
        /// <summary>
        /// Initializes the helper.
        /// <para>CAUTION: Should be called by the first line in <c>Process()</c> or <c>ProcessAsync()</c> method </para>
        /// </summary>
        protected override void Initialize(TagHelperContext context, TagHelperOutput output)
        {
            base.Initialize(context, output);

            // In order to get all attributes, built-in and custom, typecast Metadata to DefaultModelMetadata.
            // For another technique of getting just the custom attributes marking the property see the TagHelperModelMetadataProvider class
            // see also: https://github.com/aspnet/Mvc/issues/4597
            PropertyMetadata = For.Metadata as DefaultModelMetadata;
            PropertyAttributes = PropertyMetadata.Attributes.PropertyAttributes;
            PropertyType = PropertyMetadata.ModelType;

            FullName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
            HasErrors = ViewContext.ViewData.ModelState.TryGetValue(FullName, out var entry) && entry.Errors.Count > 0;

            if (HasErrors)
            {
                StringBuilder SB = new StringBuilder();
                SB.Append($"{For.Metadata.DisplayName} has errors: ");
                foreach (var Error in entry.Errors)
                    SB.Append(Error.ErrorMessage);
                Errors = SB.ToString();
            }
        }


        // ● construction 
        /// <summary>
        /// Constructor
        /// </summary>
        public TagHelperAspForBase(IHtmlGenerator generator, IHtmlHelper htmlHelper)
            : base(generator, htmlHelper)
        {
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

            if (For == null)
            {
                throw new NullReferenceException("No model expression for a tag helper. asp-for is missing");
            }
 
            if (For.Metadata == null)
            {
                throw new NullReferenceException($"No metadata for a tag helper. Field: {For.Name}");
            }
        }

        // ● properties  
        /// <summary>
        /// The expression passed to the tag helper. Usually this is a model's property or the full model
        /// </summary>
        [HtmlAttributeName(SFor)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// The value of the property, this instance represents, in the current model.
        /// </summary>
        [HtmlAttributeNotBound]
        public object PropertyValue { get; protected set; }
        /// <summary>
        /// The <see cref="Type"/> of model's property
        /// </summary>
        [HtmlAttributeNotBound]
        public Type PropertyType { get; protected set; }
        /// <summary>
        /// Property metadata.
        /// <para> In order to get all attributes, built-in and custom, typecast Metadata to DefaultModelMetadata. </para>
        /// <para> For another technique of getting just the custom attributes marking the property see the <see cref="TagHelperModelMetadataProvider"/> class </para>
        /// <para> see also: https://github.com/aspnet/Mvc/issues/4597 </para>
        /// </summary>
        [HtmlAttributeNotBound]
        public DefaultModelMetadata PropertyMetadata { get; protected set; }
        /// <summary>
        /// The list of CSharp attributes marking the model property
        /// </summary>
        [HtmlAttributeNotBound]
        public IReadOnlyList<object> PropertyAttributes { get; protected set; }
        /// <summary>
        /// Fully-qualified expression name for the specified partialFieldName in the <see cref="For"/> <see cref="ModelExpression"/>.
        /// </summary>
        [HtmlAttributeNotBound]
        public string FullName { get; protected set; }

        /// <summary>
        /// True when the property value is invalid, after a POST
        /// </summary>
        [HtmlAttributeNotBound]
        public bool HasErrors { get; protected set; }
        /// <summary>
        /// Message regarding the errors or null.
        /// </summary>
        [HtmlAttributeNotBound]
        public string Errors { get; protected set; }
    }
}
