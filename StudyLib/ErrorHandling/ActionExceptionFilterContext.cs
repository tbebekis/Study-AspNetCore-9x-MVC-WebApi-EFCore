using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StudyLib
{
    /// <summary>
    /// Context for the <see cref="ActionExceptionFilter"/>
    /// </summary>
    public class ActionExceptionFilterContext
    {
        Type BaseControllerType = typeof(ControllerBase);
        Type ControllerType = typeof(Controller);

        string fRequestId;

        // ● Ajax
        /// <summary>
        /// Returns true when the request is an Ajax request
        /// </summary>
        static public bool IsAjaxRequest(HttpRequest R)
        {
            /*
            X-Requested-With -> XMLHttpRequest is invalid in cross-domain call
           
            Only the following headers are allowed across origins:
                Accept
                Accept-Language
                Content-Language
                Last-Event-ID
                Content-Type
            */


            return string.Equals(R.Query[HeaderNames.XRequestedWith], "XMLHttpRequest", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(R.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ActionExceptionFilterContext(ExceptionContext ExceptionContext, IModelMetadataProvider ModelMetadataProvider, bool IsDevelopment)
        {
            this.ExceptionContext = ExceptionContext;
            this.ModelMetadataProvider = ModelMetadataProvider;
            this.InDevMode = IsDevelopment;
            this.Exception = ExceptionContext.Exception;

            IsWebApi = ControllerTypeInfo.IsSubclassOf(BaseControllerType) && !ControllerTypeInfo.IsSubclassOf(ControllerType);
            if (!IsWebApi)
            {
                IsMvc = ControllerTypeInfo.IsSubclassOf(BaseControllerType) && ControllerTypeInfo.IsSubclassOf(ControllerType);
                if (IsMvc)
                {
                    IsAjax = IsAjaxRequest(ExceptionContext.HttpContext.Request);
                    if (IsAjax)
                        IsMvc = false;
                }
            } 
        }

        /* properties */
        /// <summary>
        /// True means the exception thrown in an action of a Web Api controller.
        /// </summary>
        public bool IsWebApi { get; }
        /// <summary>
        /// True means the exception thrown in an action of a MVC controller.
        /// </summary>
        public bool IsMvc { get; }
        /// <summary>
        /// True means the exception thrown in an action of a MVC controller controller with "XMLHttpRequest" in request headers.
        /// </summary>
        public bool IsAjax { get; }
        /// <summary>
        /// The exception context
        /// </summary>
        public ExceptionContext ExceptionContext { get; }
        /// <summary>
        /// The exception
        /// </summary>
        public Exception Exception { get; }
        /// <summary>
        /// The action descriptor
        /// </summary>
        public ControllerActionDescriptor ActionDescriptor => ExceptionContext.ActionDescriptor as ControllerActionDescriptor;
        /// <summary>
        /// The controller type info. <see cref="TypeInfo"/> is a descendant of the <see cref="Type"/> class.
        /// </summary>
        public TypeInfo ControllerTypeInfo => ActionDescriptor.ControllerTypeInfo;
        /// <summary>
        /// The model metadata provider
        /// </summary>
        public IModelMetadataProvider ModelMetadataProvider { get; }
        /// <summary>
        /// The Id of the current http request
        /// </summary>
        public string RequestId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fRequestId))
                {
                    fRequestId = Activity.Current?.Id ?? ExceptionContext.HttpContext.TraceIdentifier;
                    if (!string.IsNullOrWhiteSpace(fRequestId) && fRequestId.StartsWith('|'))
                        fRequestId = fRequestId.Remove(0, 1);
                }

                return fRequestId;
            }
        }
        /// <summary>
        /// True when in development environment
        /// </summary>
        public bool InDevMode { get; }
    }
}
