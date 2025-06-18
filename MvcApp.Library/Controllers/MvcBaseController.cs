namespace MvcApp.Library
{

    /// <summary>
    /// Base MVC controller
    /// </summary>  
    public class MvcBaseController : Controller
    {
        MvcUserContext fUserContext;

        // ● serialize
        /// <summary>
        /// Serializes a specified instance.
        /// </summary>
        static public string Serialize(object Instance)
        {
            return CommonLib.Json.Serialize(Instance);
        }
        /// <summary>
        /// Serializes a specified instance.
        /// </summary>
        static public string Serialize(object Instance, JsonSerializerOptions JsonOptions)
        {
            return CommonLib.Json.Serialize(Instance, JsonOptions);
        }

        /// <summary>
        /// Serializes a specified instance.
        /// </summary>
        static public string Serialize(object Instance, string[] ExcludeProperties)
        {
            return CommonLib.Json.Serialize(Instance, ExcludeProperties);
        }

        // ● deserialize
        /// <summary>
        /// Deserializes (creates) an object of a specified type by deserializing a specified json text.
        /// <para>If no options specified then it uses the <see cref="SerializerOptions"/> options</para> 
        /// </summary>
        static public T Deserialize<T>(string JsonText, JsonSerializerOptions JsonOptions = null)
        {
            return CommonLib.Json.Deserialize<T>(JsonText, JsonOptions);
        }
        /// <summary>
        /// Deserializes (creates) an object of a specified type by deserializing a specified json text.
        /// <para>If no options specified then it uses the <see cref="SerializerOptions"/> options</para> 
        /// </summary>
        static public object Deserialize(string JsonText, Type ReturnType, JsonSerializerOptions JsonOptions = null)
        {
            return CommonLib.Json.Deserialize(JsonText, ReturnType, JsonOptions);
        }

        /// <summary>
        /// Loads an object's properties from a specified json text.
        /// <para>If no options specified then it uses the <see cref="SerializerOptions"/> options</para> 
        /// </summary>
        static public void PopulateObject(object Instance, string JsonText, JsonSerializerOptions JsonOptions = null)
        {
            CommonLib.Json.PopulateObject(Instance, JsonText, JsonOptions);
        }

        // ●  overridables
        /// <summary>
        /// The context regarding the current HTTP request (current visitor, selected warehouse, etc.)
        /// </summary>
        protected virtual MvcUserContext UserContext => fUserContext ?? (fUserContext = Lib.GetService<MvcUserContext>());
        /// <summary>
        /// Returns a localized string based on a specified resource key, e.g. Customer, and the current (Session's) culture code, e.g. el-GR
        /// </summary>
        protected virtual string L(string Key, params object[] Args)
        {
            string S = Res.GetString(Key);
            if ((Args != null) && (Args.Length > 0))
                S = string.Format(S, Args);
            return S;
        }
 
        /// <summary>
        /// Validates a model and returns the result.
        /// <para>Returns true if the specified model is ok and has no errors.</para>
        /// <para>Returns false if the specified model has errors, collects the error messages and creates the ErrorList entry in the ViewData.</para>
        /// </summary>
        protected virtual bool ValidateModel(object Model, bool EnhanceModelState = false, Func<Dictionary<string, ModelStateEntry>> InvalidEntriesFunc = null)
        {
            if (this.GetErrorList(Model, out var ErrorList, EnhanceModelState, InvalidEntriesFunc))
            {
                Session.AddToErrorList(ErrorList);
                return false;
            }

            return true;
        }
        /// <summary>
        /// Makes the whole model invalid.
        /// </summary>
        protected virtual void SetModelStateToInvalid(string ErrorText)
        {
            foreach (var Entry in ModelState)
            {
                var ModelStateEntry = Entry.Value;
                ModelStateEntry.Errors.Add("Error");
                ModelStateEntry.ValidationState = ModelValidationState.Invalid;
            }
        }


        // ●  actions 
        /// <summary>
        /// Returns the "not found" view
        /// </summary>
        protected virtual IActionResult NotFoundInternal(string Text = "No Message")
        {
            // can be used by overrides as
            // return NotFoundInternal(SOME MESSAGE HERE); 
            return View("_NotFound", Text);
        }
        /// <summary>
        /// Returns the "not yet" view
        /// </summary>
        protected virtual IActionResult NotYetInternal(string Text = "No Message")
        {
            // can be used by overrides as
            // return NotYetInternal(SOME MESSAGE HERE); 
            return View("_NotYet", Text);
        }

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public MvcBaseController()
        {
        }

    }
}
