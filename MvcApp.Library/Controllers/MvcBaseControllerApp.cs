namespace MvcApp.Library
{

    /// <summary>
    /// <para>NOTE: Does NOT require authentication</para>
    /// </summary>
    public class MvcBaseControllerApp: MvcBaseController
    {

        // ● overridables
        /// <summary>
        /// Handles the ReturnUrl parameter
        /// </summary>
        protected virtual IActionResult HandleReturnUrl(string ReturnUrl = "")
        {
            // https://github.com/dotnet/aspnetcore/issues/4919
            // https://stackoverflow.com/questions/54979168/invalid-non-ascii-or-control-character-in-header-on-redirect

            // home  
            if (string.IsNullOrWhiteSpace(ReturnUrl))
                ReturnUrl = Url.RouteUrl("Home");

            bool IsEncoded = ReturnUrl.StartsWith('%');

            if (IsEncoded)
                ReturnUrl = ReturnUrl.UrlDecode();

            // prevent open redirection attack
            if (!Url.IsLocalUrl(ReturnUrl))
                ReturnUrl = Url.RouteUrl("Home");

            return Redirect(ReturnUrl);
        }

        protected virtual RedirectToActionResult RedirectToErrorPage(string ErrorMessage)
        {
            ErrorViewModel Model = new ErrorViewModel();
            Model.ErrorMessage = ErrorMessage;
            Model.RequestId = Lib.RequestId;

            string JsonText = Serialize(Model);

            TempData["ErrorModel"] = JsonText;

            return RedirectToAction("Error", "Home", new { });
        }

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public MvcBaseControllerApp()
        {

        }
    }

   
}
