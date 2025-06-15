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

        /// <summary>
        /// Constructor
        /// </summary>
        public ActionExceptionFilterContext(ExceptionContext ExceptionContext, IModelMetadataProvider ModelMetadataProvider, bool IsDevelopment)
        {
            this.ExceptionContext = ExceptionContext;
            this.ModelMetadataProvider = ModelMetadataProvider;
            this.IsDevelopment = IsDevelopment;
        }

        /* properties */
        /// <summary>
        /// True means the exception thrown in an action of a Web Api controller, else in an Mvc controller.
        /// </summary>
        public bool IsWebApi => ControllerTypeInfo.IsSubclassOf(BaseControllerType) && !ControllerTypeInfo.IsSubclassOf(ControllerType);
        public bool IsMvc => ControllerTypeInfo.IsSubclassOf(BaseControllerType) && ControllerTypeInfo.IsSubclassOf(ControllerType);
        /// <summary>
        /// The exception context
        /// </summary>
        public ExceptionContext ExceptionContext { get; }
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
        public bool IsDevelopment { get; }
    }
}
