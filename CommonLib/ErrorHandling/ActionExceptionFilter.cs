using Microsoft.AspNetCore.Mvc.Abstractions;

namespace CommonLib
{
    /// <summary>
    /// Global exception filter for controller actions. Use this instead of try-catch blocks inside action methods.    
    /// <para>Catches exceptions thrown inside of an action method. <strong>Not inside a view.</strong></para>
    /// <para>
    /// <strong>NOTE:</strong> Exception filters  <br />
    ///  ● Handle unhandled exceptions that occur in Razor Page or controller creation, model binding, action filters, or action methods. <br />
    ///  ● Do <strong>not</strong> catch exceptions that occur in resource filters, result filters, or MVC result execution.  
    /// </para>
    /// <para>To register</para>
    /// <code> builder.Services.AddControllersWithViews(options => { options.Filters.Add&lt;ActionExceptionFilter&gt;(); });</code>
    /// SEE: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#exception-filters
    /// </summary>
    public class ActionExceptionFilter : IExceptionFilter
    {
        IWebHostEnvironment HostEnvironment;
        IModelMetadataProvider ModelMetadataProvider;
        ILogger Logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public ActionExceptionFilter(IWebHostEnvironment HostEnvironment, IModelMetadataProvider ModelMetadataProvider, ILogger<ActionExceptionFilter> logger)
        {
            this.HostEnvironment = HostEnvironment;
            this.ModelMetadataProvider = ModelMetadataProvider;
            this.Logger = logger;
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

            string Controller = FilterContext.ActionDescriptor.ControllerName;
            string Action = FilterContext.ActionDescriptor.ActionName;

            Logger.LogError(context.Exception, "Exception in Controller: {Controller}, Action: {Action}", Controller, Action);
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
