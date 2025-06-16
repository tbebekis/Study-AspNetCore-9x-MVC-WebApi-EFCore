namespace MvcApp.Library
{
    public class ControllerMvc: ControllerMvcBase
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

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public ControllerMvc()
        {

        }
    }

   
}
