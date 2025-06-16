namespace MvcApp.Library
{
    /// <summary>
    /// Extensions
    /// </summary>
    static public class ControllerExtensions
    {
        /// <summary>
        /// Renders a view to a string. View can be a main or a partial view.
        /// <para>See AjaxController.MiniSearch() for an example.</para>
        /// </summary>
        static public string RenderViewToString(this ControllerBase Instance, string ViewName, object Model, bool IsMainView, IDictionary<string, object> PlusViewData = null)
        {
            IRazorViewEngine ViewEngine = Lib.GetService<IRazorViewEngine>();
            ActionContext ActionContext = new ActionContext(Instance.HttpContext, Instance.RouteData, Instance.ControllerContext.ActionDescriptor, Instance.ModelState);

            ViewDataDictionary ViewData = null;
            ITempDataDictionary TempData = null;

            if (Instance is Controller)
            {
                ViewData = (Instance as Controller).ViewData;
                TempData = (Instance as Controller).TempData;
            }
            else
            {
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                TempData = new TempDataDictionary(ActionContext.HttpContext, Lib.GetService<ITempDataProvider>());
            }

            // assign ViewData's model
            ViewData.Model = Model;

            // plus view data
            if (PlusViewData != null)
            {
                foreach (var Entry in PlusViewData)
                    ViewData[Entry.Key] = Entry.Value;
            }

            // if ViewName is empty, set it to action name
            if (string.IsNullOrWhiteSpace(ViewName))
                ViewName = Instance.ControllerContext.ActionDescriptor.ActionName;

            // is ViewName a view name?
            ViewEngineResult ViewResult = ViewEngine.FindView(ActionContext, ViewName, IsMainView);

            // is ViewName a path?
            if (ViewResult.View == null)
            {
                ViewResult = ViewEngine.GetView(null, ViewName, false);
                if (ViewResult.View == null)
                    throw new ApplicationException($"Cannot render a View to string. View not found: {ViewName}");
            }

            using (var SW = new StringWriter())
            {
                ViewContext ViewContext = new ViewContext(ActionContext, ViewResult.View, ViewData, TempData, SW, new HtmlHelperOptions());

                Task T = ViewResult.View.RenderAsync(ViewContext);
                T.Wait();

                return SW.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Renders a partial view to a string.
        /// <para>See AjaxController.MiniSearch() for an example.</para>
        /// </summary>
        static public string RenderPartialViewToString(this ControllerBase Instance, string ViewName, object Model, IDictionary<string, object> PlusViewData = null)
        {
            return RenderViewToString(Instance, ViewName, Model, false, PlusViewData);
        }
        /// <summary>
        /// Renders a partial view to a string.
        /// <para>See AjaxController.MiniSearch() for an example.</para>
        /// </summary>
        static public string RenderPartialViewToString(this ControllerBase Instance, string ViewName, IDictionary<string, object> PlusViewData = null)
        {
            return RenderViewToString(Instance, ViewName, null, false, PlusViewData);
        }
        /// <summary>
        /// Renders a partial view to a string. 
        /// <para>NOTE: ViewName is set it to current action name.</para>
        /// <para>See AjaxController.MiniSearch() for an example.</para>
        /// </summary>
        static public string RenderPartialViewToString(this ControllerBase Instance, IDictionary<string, object> PlusViewData = null)
        {
            return RenderViewToString(Instance, null, null, false, PlusViewData);
        }
    }
}
