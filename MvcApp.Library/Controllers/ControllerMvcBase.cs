namespace MvcApp.Library
{

    /// <summary>
    /// Base MVC controller
    /// </summary>  
    public class ControllerMvcBase : Controller
    {
        MvcUserContext fUserContext;

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
        public ControllerMvcBase()
        {
        }

    }
}
