namespace MvcApp.Library
{
    /// <summary>
    /// Extensions
    /// </summary>
    static public class MvcExtensions
    {
        /// <summary>
        /// Returns a collection of Key/Value pairs where Key is the Property Name of the current Model and Value a Model error, if any.
        /// </summary>
        static public IEnumerable<KeyValuePair<string, ModelStateEntry>> GetModelInvalidStateEntries(this ControllerBase Controller)
        {
            var InvalidEntries = Controller.ModelState.Where(entry => entry.Value.ValidationState == ModelValidationState.Invalid).Select(entry => entry);
            return InvalidEntries;
        }

        /// <summary>
        /// Returns true if a specified model state is invalid, along with a string list of errors.
        /// <para>The returned list contains the display (localized) names of the invalid properties, along with property names.</para>
        /// <para>If specified then enhances the model state errors with the new error strings.</para>
        /// </summary>
        static public bool GetErrorList(this ControllerBase Controller, object Model, out List<string> List, bool EnhanceModelState = false, Func<Dictionary<string, ModelStateEntry>> InvalidEntriesFunc = null)
        {
            List = null;

            if (Model == null)
            {
                List = new List<string>();
                List.Add("Model is null!");
            }
            else if (!Controller.ModelState.IsValid)
            {
                // Entries is IEnumerable<KeyValuePair<string, ModelStateEntry>> that are invalid
                // var Entries = Controller.ModelState.Where(entry => entry.Value.ValidationState == ModelValidationState.Invalid).Select(entry => entry);
                var Entries = InvalidEntriesFunc != null ? InvalidEntriesFunc() : Controller.GetModelInvalidStateEntries();

                if (Entries != null && Entries.Count() > 0)
                {
                    List = new List<string>();
                    Type ModelType = Model.GetType();
                    var Properties = ModelType.GetProperties();
                    PropertyInfo PropInfo;
                    string PropName;

                    ModelMetadata ModelMetadata = Controller.MetadataProvider.GetMetadataForType(ModelType);
                    ModelMetadata PropMetadata;

                    ModelStateEntry Entry;
                    StringBuilder SB;

                    string DisplayName;
                    foreach (var Pair in Entries)
                    {
                        DisplayName = Pair.Key;
                        PropName = Pair.Key;

                        PropInfo = Properties.FirstOrDefault(p => p.Name.Equals(Pair.Key, StringComparison.InvariantCultureIgnoreCase));
                        if (PropInfo != null)
                        {
                            PropName = PropInfo.Name;
                            PropMetadata = ModelMetadata.GetMetadataForProperty(ModelType, PropInfo.Name);
                            DisplayName = PropMetadata.DisplayName;
                        }

                        SB = new StringBuilder();

                        Entry = Pair.Value;

                        SB.Append($"{DisplayName} ({PropName}) has errors: ");
                        foreach (var Error in Entry.Errors)
                            SB.Append(Error.ErrorMessage);
                        List.Add(SB.ToString());

                        if (EnhanceModelState)
                        {
                            Entry.Errors.Clear();
                            Entry.Errors.Add(SB.ToString());
                        }

                    }
                }

            }

            return List != null;
        }
        /// <summary>
        /// Returns true if a specified model state is invalid, along with a string list of errors
        /// </summary>
        static public bool GetErrorList(this ModelStateDictionary ModelState, out List<string> List)
        {
            List = null;

            if (!ModelState.IsValid)
            {
                List = ModelState.GetErrorList();
                return true;
            }

            return false;
        }
        /// <summary>
        /// Returns a string list of an invalid model state
        /// </summary>
        static public List<string> GetErrorList(this ModelStateDictionary ModelState)
        {
            List<string> Result = new List<string>();

            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> Errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (ModelError E in Errors)
                    Result.Add(E.ErrorMessage);
            }

            return Result;
        }
        /// <summary>
        /// Returns the errors of an invalid model state as a UL list
        /// </summary>
        static public string GetErrorsHtmlText(this ModelStateDictionary ModelState)
        {
            List<string> List = GetErrorList(ModelState);
            HtmlTag UL = new HtmlTag("ul");
            foreach (string Item in List)
            {
                UL.Add("li").InnerHtml.AppendLine(Item);
            }

            return UL.ToHtml();
        }
        /// <summary>
        /// Returns the errors of an invalid model state as plain text lines
        /// </summary>
        static public string GetErrorsText(this ModelStateDictionary ModelState)
        {
            StringBuilder SB = new StringBuilder();

            List<string> List = GetErrorList(ModelState);
            foreach (string S in List)
            {
                SB.AppendLine(S);
            }

            return SB.ToString();
        }

        /// <summary>
        /// Returns an <see cref="Attribute"/> instance that marks a Model's property, if there, else null.
        /// <para>NOTE: Assumes that <see cref="ModelExpression.Metadata"/> is a <see cref="Microsoft.AspNetCore.Mvc.ModelBinding.Metadata.DefaultModelMetadata"/> instance. </para>
        /// <para>The <c>DefaultModelMetadata.Attributes.Attributes</c> collection contains all <see cref="Attribute"/>s marking the property, either custom or built-in.</para>
        /// <para>NOTE 2: The <see cref="ModelExpression.Metadata"/>.ValidatorMetadata contains only validation <see cref="Attribute"/>s </para>
        /// <para>See Also: https://github.com/aspnet/Mvc/issues/4597 </para>
        /// </summary>
        static public T GetAttribute<T>(this ModelExpression For) where T : Attribute
        {
            Type type = typeof(T);

            if (For != null)
            {
                var DefaultMetadata = For.Metadata as Microsoft.AspNetCore.Mvc.ModelBinding.Metadata.DefaultModelMetadata;
                if (DefaultMetadata != null && DefaultMetadata.Attributes != null)
                {
                    return DefaultMetadata.Attributes.Attributes.FirstOrDefault(item => item.GetType() == type) as T;
                }
            }


            return default(T);
        }
 
        static HashSet<Type> IntegerTypeSet = new HashSet<Type>
        {
            typeof(Byte),
            typeof(SByte),
            typeof(Int16),
            typeof(Int32),
            typeof(Int64),
            typeof(UInt16),
            typeof(UInt32),
            typeof(UInt64)
        };


        /// <summary>
        /// Returns true if a specified type is an integer type, i.e. Byte, SByte, Int16, Int32, Int64, UInt16, UInt32, UInt64
        /// </summary>
        static public bool IsIntegerType(this Type T)
        {
            return IntegerTypeSet.Contains(T) || IntegerTypeSet.Contains(Nullable.GetUnderlyingType(T));
        }

        /// <summary>
        /// Generates and returns a unique id for an HTML Element.
        /// <para>WARNING: HTML element id is case-sensitive.</para>
        /// </summary>
        static public string NextId(this HttpContext Context, string Prefix = "")
        {
            return ElementIdGenerator.Next(Prefix);
        }

        /// <summary>
        /// Generates and returns a unique id for an HTML Element.
        /// <para>WARNING: HTML element id is case-sensitive.</para>
        /// </summary>
        static public string NextId(this IHtmlHelper Html, string Prefix = "")
        {
            return NextId(Html.ViewContext.HttpContext, Prefix);
        }



    }
}
