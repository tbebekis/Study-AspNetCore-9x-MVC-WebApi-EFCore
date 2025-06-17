namespace MvcApp.Controllers
{


    /// <summary>
    /// Base MVC controller of this application
    /// <para><strong>NOTE</strong>: Requires authentication</para>
    /// </summary>
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class MvcAppController : MvcBaseControllerApp
    {
        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public MvcAppController()
        {
        }

    }
}
