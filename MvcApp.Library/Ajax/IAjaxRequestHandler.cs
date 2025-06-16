namespace MvcApp.Library
{
    /// <summary>
    /// Represents an object that processes an <see cref="AjaxRequest"/> and returns an <see cref="AjaxRequestResult"/>
    /// </summary>
    public interface IAjaxRequestHandler
    {
        /// <summary>
        /// Processes an <see cref="AjaxRequest"/> and if it handles the request returns an <see cref="AjaxRequestResult"/>. Else returns null.
        /// </summary>
        AjaxRequestResult Process(AjaxRequest Request);
    }
}
