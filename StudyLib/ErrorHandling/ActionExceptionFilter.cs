using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
            HandlerFunc?.Invoke(FilterContext);
            context.ExceptionHandled = true;
            // TODO: Logger.Error(ActionDescriptor.ControllerName, ActionDescriptor.ActionName, RequestId, context.Exception);
        }

        /* properties */
        /// <summary>
        /// A replacable static handler function for global exceptions. It offers a default error handling.
        /// </summary>
        static public Action<ActionExceptionFilterContext> HandlerFunc = (Context) =>
        {
            if (Context.IsWebApi) // it is a WebApi controller 
            {
                DataResult Result = new();
                Result.ExceptionResult(Context.ExceptionContext.Exception);

                // NO, we do NOT want an invalid HTTP StatusCode. It is a valid HTTP Response.
                // We just have an action result with errors, so any error should be recorded by our HttpActionResult and delivered to the client.
                // context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError; 
                Context.ExceptionContext.HttpContext.Response.ContentType = "application/json";
                Context.ExceptionContext.Result = new JsonResult(Result);
            }
            else if (Context.IsMvc) // IsMvc controller
            {
                /* SEE: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-3.1#exception-filters */
                var Result = new ViewResult();
                Result.ViewName = "Error";
                Result.ViewData = new ViewDataDictionary(Context.ModelMetadataProvider, Context.ExceptionContext.ModelState);
                Result.ViewData.Add("Exception", Context.ExceptionContext.Exception);
                Result.ViewData.Add("RequestId", Context.RequestId);
                Context.ExceptionContext.Result = Result;
            }
            else
            {
                Context.ExceptionContext.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError); // new BadRequestResult();
            }

        };
    }
}
