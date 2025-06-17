namespace StudyLib
{
    /// <summary>
    /// Global exception filter for controller actions. Use this instead of try-catch blocks inside action methods.    
    /// <para>
    /// Exception filters: <br />
    ///  ● Handle unhandled exceptions that occur in Razor Page or controller creation, model binding, action filters, or action methods. <br />
    ///  ● Do not catch exceptions that occur in resource filters, result filters, or MVC result execution.  
    /// </para>
    /// <para></para>
    /// <para>
    /// SEE: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-3.1#exception-filters
    /// </para>
    /// <para>To register</para>
    /// <para><code> services.AddControllersWithViews(o =&gt; { o.Filters.Add&lt;ActionExceptionFilter&gt;(); })
    /// </code></para>
    /// </summary>
    public class ActionExceptionFilter : IExceptionFilter
    {
        IWebHostEnvironment HostEnvironment;
        IModelMetadataProvider ModelMetadataProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        public ActionExceptionFilter(IWebHostEnvironment HostEnvironment, IModelMetadataProvider ModelMetadataProvider)
        {
            this.HostEnvironment = HostEnvironment;
            this.ModelMetadataProvider = ModelMetadataProvider;
        }


        /// <summary>
        /// Called after an action has thrown a <see cref="Exception"/> 
        /// </summary>
        public void OnException(ExceptionContext context)
        {
            ActionExceptionFilterContext FilterContext = new(context, ModelMetadataProvider, HostEnvironment.IsDevelopment());

            if (FilterContext.IsWebApi && WebApiHandlerFunc != null)
            {
                WebApiHandlerFunc(FilterContext);
            }
            else if (FilterContext.IsMvc && MvcHandlerFunc != null)
            {
                MvcHandlerFunc(FilterContext);
            }
            else if (FilterContext.IsAjax && AjaxHandlerFunc != null)
            {
                AjaxHandlerFunc(FilterContext);
            }
            else
            {
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
 
            context.ExceptionHandled = true;
            // TODO: Logger.Error(ActionDescriptor.ControllerName, ActionDescriptor.ActionName, RequestId, context.Exception);
        }

        /* properties */
        /// <summary>
        /// A replacable static handler function for global exceptions. It offers a default error handling.
        /// </summary>
        static public Action<ActionExceptionFilterContext> WebApiHandlerFunc { get; set; }
        /// <summary>
        /// A replacable static handler function for global exceptions. It offers a default error handling.
        /// </summary>
        static public Action<ActionExceptionFilterContext> MvcHandlerFunc { get; set; }
        /// <summary>
        /// A replacable static handler function for global exceptions. It offers a default error handling.
        /// </summary>
        static public Action<ActionExceptionFilterContext> AjaxHandlerFunc { get; set; }

    }
}
