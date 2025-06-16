namespace MvcApp.Library
{
    /// <summary>
    /// Information about a razor view
    /// </summary>
    public class AjaxViewInfo
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public AjaxViewInfo()
        {
        }

        /* properties */
        /// <summary>
        /// The razor view name or the full path to a razor view file.
        /// </summary>
        public string RazorViewNameOrPath { get; set; }
        /// <summary>
        /// The model object, if any, else null.
        /// </summary>
        public object Model { get; set; }
        /// <summary>
        /// A view data dictionary to pass to the view.
        /// </summary>
        public Dictionary<string, object> ViewData { get; } = new Dictionary<string, object>();
    }
}
