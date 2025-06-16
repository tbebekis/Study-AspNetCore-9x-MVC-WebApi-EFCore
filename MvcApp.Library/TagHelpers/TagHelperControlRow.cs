namespace MvcApp.Library
{
 
    
    /// <summary>
    /// Renders an editor control, that is an editable control.
    /// </summary> 
    [HtmlTargetElement(SInput, Attributes = SItems)]
    [HtmlTargetElement(SInput, Attributes = SRowId)]
    [HtmlTargetElement(SInput, Attributes = SFor, TagStructure = TagStructure.WithoutEndTag)]
    public class TagHelperControlRow : TagHelperAspForBase
    {
        // TODO: TouchSpin see https://www.youtube.com/watch?v=ZjVmNKVgDsE

        /// <summary>
        /// The type of the editor (HTML control)
        /// </summary>
        [Flags]
        protected enum EditorType
        {
            /// <summary>
            /// Default
            /// </summary>
            Default = 0,
            /// <summary>
            /// CheckBox
            /// </summary>
            CheckBox = 1,
            /// <summary>
            /// TextArea
            /// </summary>
            TextArea = 2,
            /// <summary>
            /// ComboBox
            /// </summary>
            ComboBox = 4
        }

        // ● private 
        const string SInput = "tp-input";
        const string SRowId = "asp-row-id";
        const string SRowClass = "asp-row-class";
        const string SItems = "asp-items";
        const string STemplate = "asp-template";
        const string SDefaultItem = "asp-default-item";


        string DefaultRowClasses { get { return "tp-CtrlRow"; } }
        string DefaultCheckBoxRowClasses { get { return "tp-CheckBoxRow"; } }

        string TextDivClasses { get { return "tp-CText"; } }
        string ControlDivClasses { get { return "tp-Ctrl"; } }
        string RequiredMarkClasses { get { return "tp-RequiredMark"; } }
        string HasErrorsClasses { get { return "tp-HasErrors"; } }

        string CheckBoxClasses { get { return "tp-CheckBox"; } }
        string TextAreaClasses { get { return "tp-Memo"; } }
        string ComboBoxClasses { get { return "tp-HtmlComboBox"; } }
        string DefaultEditorClasses { get { return "tp-TextBox"; } }

        string GetInputClasses(EditorType Kind)
        {
            switch (Kind)
            {
                case EditorType.Default: return DefaultEditorClasses;
                case EditorType.TextArea: return TextAreaClasses;
                case EditorType.ComboBox: return ComboBoxClasses;
            }

            return string.Empty;
        }
        /// <summary>
        /// Returns the editor type of the current HTML tag
        /// </summary>
        protected EditorType GetEditorType()
        {
            if (IsCheckBox(PropertyType, PropertyAttributes))
                return EditorType.CheckBox;

            if (PropertyType == typeof(string))
            {
                if (OutputAttributes.ContainsKey("type") && OutputAttributes["type"] != null)
                {
                    if (OutputAttributes["type"].ToString() == "textarea")
                        return EditorType.TextArea;
                }                    

                if (PropertyAttributes.OfType<TextAreaAttribute>().FirstOrDefault() != null)
                    return EditorType.TextArea;
            }

            if (Items != null)
                return EditorType.ComboBox;
 
            return EditorType.Default;
        }

        /// <summary>
        /// Applies property annotation attributes to the specified html attributes dictionary.
        /// <para>Later on that html attributes dictionary entries are merged into the output control html attributes. </para>
        /// </summary>
        /// <param name="PropertyAttributes">A list of CSharp attributes marking the model property</param>
        /// <param name="HtmlAttributes">An html attributes dictionary. Later on that html attributes dictionary entries are merged into the output control html attributes. </param>
        static void ApplyPropertyAttributes(IReadOnlyList<object> PropertyAttributes, Dictionary<string, object> HtmlAttributes)
        {
            ReadOnlyAttribute A = PropertyAttributes.OfType<ReadOnlyAttribute>().FirstOrDefault();
            if (A != null && A.IsReadOnly)
                HtmlAttributes.Add("readonly", "readonly");

            if (PropertyAttributes.OfType<DisabledAttribute>().FirstOrDefault() != null)
                HtmlAttributes.Add("disabled", "disabled");

            PlaceholderAttribute A2 = PropertyAttributes.OfType<PlaceholderAttribute>().FirstOrDefault();
            if (A != null && !string.IsNullOrWhiteSpace(A2.Text))
                HtmlAttributes.Add("placeholder", A2.Text);
        }

        static bool IsBool(Type PropertyType)
        {
            return PropertyType == typeof(bool);
        }
        static bool IsIntBool(Type PropertyType, IReadOnlyList<object> PropertyAttributes)
        {
            return PropertyType.IsIntegerType() && PropertyAttributes.OfType<IntBoolAttribute>().FirstOrDefault() != null;
        }
        static bool IsCheckBox(Type PropertyType, IReadOnlyList<object> PropertyAttributes)
        {
            return IsBool(PropertyType) || IsIntBool(PropertyType, PropertyAttributes);
        }
 
        void RenderCheckBox()
        {
            /*   
            <div class="tp-CheckBoxRow tp-Object" id="tp-CheckBoxRow-Married-2000">
                <label class="tp-CheckBox tp-Object" id="tp-CheckBox-2000" tabindex="0">
                    <input name="SAME_NAME" type="checkbox" value="1">
                    <span class="tp-RequiredMark">*</span>
                    <span class="tp-Text">TEXT GOES HERE</span>
                    <span class="checkmark"></span> 
                </label>
                <input name="SAME_NAME" type="hidden" value="0">
            </div> 
            */

            HtmlTag SourceLabel = Generator.GenerateLabel(ViewContext, For.ModelExplorer, For.Name, null, null).Clone();
            string CheckBoxId = SourceLabel.Attributes["for"];
            string InnerText = SourceLabel.InnerHtmlToString();

            // label
            HtmlTag Label = MainTag.Add("label");
            //Label.Attribute("for", CheckBoxId);
            Label.Class(CheckBoxClasses);

            // checkbox
            HtmlTag Editor = Label.AddInput("checkbox");

            // required - so add the asterisk to the title text
            if (PropertyAttributes.OfType<RequiredAttribute>().FirstOrDefault() != null) // (For.Metadata.IsRequired)
            {
                HtmlTag RequiredMark = Label.Add("span", RequiredMarkClasses);
                RequiredMark.InnerHtml.Append("*");
            }

            // label text
            HtmlTag tagText = Label.Add("span", "tp-Text");
            tagText.InnerHtml.AppendHtml(InnerText);

            Editor.Attribute("id", CheckBoxId);
            Editor.Attribute("name", CheckBoxId);
            Editor.Attribute("value", IsBool(PropertyType) ? "true" : "1");
            if (Convert.ToBoolean(For.Model))
            {
                Editor.Attribute("checked", "checked");
            }

            HtmlAttributes.Clear();
            ApplyPropertyAttributes(PropertyAttributes, HtmlAttributes);
            Editor.MergeAttributes(HtmlAttributes, replaceExisting: true);
            Editor.MergeAttributes(OutputAttributes, replaceExisting: true);

            if (HasErrors)
                Editor.Class(HasErrorsClasses);

            HtmlTag Checkmark = Label.Add("span", "checkmark");

            // hidden
            HtmlTag Hidden = HtmlTag.Hidden(); // <input name="RememberMe" type="hidden" value="false" />
            Hidden.Attribute("name", CheckBoxId);
            Hidden.Attribute("value", IsBool(PropertyType) ? "false" : "0");
            MainTag.Add(Hidden);

        }
        void RenderInput(EditorType Kind)
        {

            /*
                <div class="tp-CtrlRow tp-Row" id="control_row_UserId-2001">
                    <div class="tp-CText">
                        <label for="UserId">UserId</label>
                        <span class="tp-RequiredMark">*</span>
                    </div>
                    <div class="tp-Ctrl">
                        <input class="tp-TextBox" id="UserId" name="UserId" type="text" value="">
                    </div>
                </div>
            */

            // text DIV
            HtmlTag TextDiv = MainTag.Add("div", TextDivClasses);

            //HtmlTag Label = HtmlHelper.Label(For.Name).Clone();
            HtmlTag Label = Generator.GenerateLabel(ViewContext, For.ModelExplorer, For.Name, null, null).Clone(); // new { @class = "control-label" }
            TextDiv.Add(Label);

            // is required, so add the asterisk to the title text
            if (PropertyAttributes.OfType<RequiredAttribute>().FirstOrDefault() != null) // (For.Metadata.IsRequired)
            {
                HtmlTag RequiredMark = TextDiv.Add("span", RequiredMarkClasses);
                RequiredMark.InnerHtml.Append("*");
            }

            // control DIV
            HtmlTag ControlDiv = MainTag.Add("div", ControlDivClasses);


            // input element
            HtmlTag Editor = null;
            switch (Kind)
            {
                case EditorType.TextArea:
                    TextAreaAttribute TAA = PropertyAttributes.OfType<TextAreaAttribute>().FirstOrDefault();
                    int Rows = TAA != null ? TAA.Rows : 4;
                    int Cols = TAA != null ? TAA.Cols : 16;
                    Editor = Generator.GenerateTextArea(ViewContext, For.ModelExplorer, For.Name, Rows, Cols, null).Clone();
                    break;
                case EditorType.ComboBox:
                    Editor = Generator.GenerateSelect(ViewContext, For.ModelExplorer, DefaultItem, For.Name, Items, false, null).Clone(); // "select an item"
                    break;
                case EditorType.Default:
                    Editor = (HtmlHelper.Editor(For.Name, null) as TagBuilder).Clone();
                    break;
            }

            Editor.RemoveClientDataValidationAttributes();
            Editor.RemoveCssClasses();
            Editor.Class(GetInputClasses(Kind));

            HtmlAttributes.Clear();
            ApplyPropertyAttributes(PropertyAttributes, HtmlAttributes);
            Editor.MergeAttributes(HtmlAttributes, replaceExisting: true);
            Editor.MergeAttributes(OutputAttributes, replaceExisting: true);

            if (HasErrors)
                Editor.Class(HasErrorsClasses);

            ControlDiv.Add(Editor);
        }

        string MainTagCssClasses
        {
            get
            {
                bool AsCheckBox = IsCheckBox(PropertyType, PropertyAttributes);
                string Result = AsCheckBox ? DefaultCheckBoxRowClasses : DefaultRowClasses;
                if (!string.IsNullOrWhiteSpace(RowClass))
                    Result += $" {RowClass}";

                return Result;
            }

        }


        // ● overrides  
        /// <summary>
        /// Creates and assigns the  the main <see cref="HtmlTag"/> containing all output markup.
        /// </summary>
        protected override void CreateMainTag()
        {
            this.MainTag = new HtmlTag("div");
        }

        // ● construction 
        /// <summary>
        /// Constructor
        /// </summary>
        public TagHelperControlRow(IHtmlGenerator generator, IHtmlHelper htmlHelper)
            : base(generator, htmlHelper)
        {
        }
 
        // ● public 
        /// <summary>
        /// Synchronously executes the <see cref="TagHelper"/> with the a specified context and output.
        /// </summary>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // main tag is a control row
            MainTag.Class(MainTagCssClasses);
            MainTag.Attribute("id", !string.IsNullOrWhiteSpace(RowId) ? RowId : NextId($"control_row_{For.Name}"));



            if (!string.IsNullOrWhiteSpace(Errors))
            {
                MainTag.Attribute("title", Errors);
            }

            EditorType Kind = GetEditorType();

            switch (Kind)
            {
                case EditorType.CheckBox:
                    RenderCheckBox();
                    break;
                default:
                    RenderInput(Kind);
                    break;
            } 

            // output
            string HtmlText = MainTag.ToHtml();            
            output.SuppressOutput();    // clear the output
            output.Content.SetHtmlContent(HtmlText);
        }

        // ● properties 
        /// <summary>
        /// Optional. Defaults to empty string. The Id of the control row.
        /// </summary>
        [HtmlAttributeName(SRowId)]
        public string RowId { get; set; }
        /// <summary>
        /// Optional. Defaults to empty string. The css class(es) of the control row.
        /// </summary>
        [HtmlAttributeName(SRowClass)]
        public string RowClass { get; set; }
        /// <summary>
        /// Optional. Defaults to empty string. When is not empty then this text is added as the default option item to a combo box editor
        /// </summary>
        [HtmlAttributeName(SDefaultItem)]
        public string DefaultItem { get; set; }
 
        /// <summary>
        /// A collection of <see cref="SelectListItem"/> objects used to populate the &lt;select&gt; element with
        /// &lt;optgroup&gt; and &lt;option&gt; elements.
        /// </summary>
        [HtmlAttributeName(SItems)]
        public IEnumerable<SelectListItem> Items { get; set; }
        /// <summary>
        /// The editor template
        /// </summary>
        [HtmlAttributeName(STemplate)]
        public string Template { set; get; }
    }

    /*

        <div class="tp-Row">
            <div class="tp-Col l-33 m-50 tp-Ctrls lc-75 mc-70 sc-70">
                <div class="tp-CtrlRow" data-setup="{Text: 'Id', Control: { TypeName: 'TextBox', Id: 'Code', DataField: 'Code', ReadOnly: true } }"></div>
                <div class="tp-CtrlRow" data-setup="{Text: 'Trader', Control: { TypeName: 'TextBox', Id: 'Name', DataField: 'Name' } }"></div>
            </div>
        </div> 


    --------------------------------------------------------------------------
        <div class="tp-CtrlRow" data-setup="{Text: 'UserId', Control: { TypeName: 'TextBox', Id: 'UserId' DataField: 'UserId', ReadOnly: false } }"></div>

        <div class="tp-CtrlRow tp-Row" id="control_row_UserId-2001">
            <div class="tp-CText">
                <label for="UserId">UserId</label>
                <span class="tp-RequiredMark">*</span>
            </div>
            <div class="tp-Ctrl">
                <input class="tp-TextBox" id="UserId" name="UserId" type="text" value="">
            </div>
        </div> 

    --------------------------------------------------------------------------
        <div class="tp-CheckBoxRow" data-setup="{Text: 'Married', Control: { Id: 'Married', DataField: 'Married' } }"></div>

        <div class="tp-CheckBoxRow tp-Object" id="tp-CheckBoxRow-Married-2000">
            <label class="tp-CheckBox tp-Object" id="tp-CheckBox-2000" tabindex="0">
                <input name="SAME_NAME" type="checkbox" value="1">
                <span class="tp-RequiredMark" style="">*</span>
                <span class="tp-Text">TEXT GOES HERE</span>
                <span class="checkmark"></span> 
            </label>
            <input name="SAME_NAME" type="hidden" value="0">
        </div> 

    --------------------------------------------------------------------------

        output.Attributes.SetAttribute("class", "m-1 p-1");
        TagBuilder title = new TagBuilder("h1");
        title.InnerHtml.Append(PrePost);

        TagBuilder container = new TagBuilder("div");
        container.Attributes["class"] = "bg-info m-1 p-1";
        container.InnerHtml.AppendHtml(title);

        if (AddHeader)
            output.PreElement.SetHtmlContent(container);
        if (AddFooter)
            output.PostElement.SetHtmlContent(container);
        */
}
