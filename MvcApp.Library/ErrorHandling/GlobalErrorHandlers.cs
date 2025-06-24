namespace MvcApp.Library.ErrorHandling
{

    /// <summary>
    /// Helper containing global error handler methods used as call-backs.
    /// </summary>
    static public class GlobalErrorHandlers
    {
        /// <summary>
        /// Handles exceptions thrown in MVC Contoller actions.
        /// </summary>
        static public void MvcActionErrorHandler(ActionExceptionFilterContext Context)
        {
            ErrorViewModel Model = new ErrorViewModel();
            Model.Exception = Context.Exception;
            Model.ErrorMessage = Context.Exception.Message;
            Model.RequestId = Context.RequestId; 

            /* SEE: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#exception-filters */
            var Result = new ViewResult();
            Result.ViewName = "Error";
            Result.ViewData = new ViewDataDictionary(Context.ModelMetadataProvider, Context.ExceptionContext.ModelState);
            Result.ViewData.Model = Model;
            // Result.ViewData.Add("Exception", Context.ExceptionContext.Exception);
            // Result.ViewData.Add("RequestId", Context.RequestId);
            Context.ExceptionContext.Result = Result;
        }
        /// <summary>
        /// Handles exceptions thrown in MVC Contoller actions regarding AJAX calls.
        /// </summary>
        static public void AjaxActionErrorHandler(ActionExceptionFilterContext Context)
        {
            DataResult Result = new();
            Result.ExceptionResult(Context.ExceptionContext.Exception);

            /// NO, we do NOT want an invalid HTTP StatusCode. It is a valid HTTP Response.
            /// We just have an action result with errors, so any error should be recorded by our HttpActionResult and delivered to the client.
            /// context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError; 
            Context.ExceptionContext.HttpContext.Response.ContentType = "application/json";
            Context.ExceptionContext.Result = new JsonResult(Result);
        }
    }
}
