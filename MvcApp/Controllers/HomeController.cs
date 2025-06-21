namespace MvcApp.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : MvcBaseControllerApp
    {
        

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public HomeController()
        {
        }

        // ● Actions
        [HttpGet("/", Name = "Home")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/login", Name = "Login")]
        public IActionResult Login()
        {
            if (UserContext.IsAuthenticated)
                return RedirectToRoute("Home");

            CredentialsModel Model = new CredentialsModel();
 
            // throw new Exception("Test error message"); // for testing error page

            return View("Login", Model);
        }
        [HttpPost("/login", Name = "Login")]
        public async Task<IActionResult> Login(CredentialsModel Model, string ReturnUrl = "")
        {
            if (UserContext.IsAuthenticated)
                return RedirectToRoute("Home");

            if (ValidateModel(Model))
            {
                AppUserService Service = new();

                ItemResult<IAppUser> Response = await Service.ValidateAppUserCredentials(Model.UserName, Model.Password);
                IAppUser User = Response.Item;

                if (Response.Succeeded && User != null)
                {       
                    bool IsImpersonation = Service.GetIsUserImpersonation(Model.Password);

                    await UserContext.SignInAsync(User, Model.RememberMe, IsImpersonation);

                    if (!string.IsNullOrWhiteSpace(ReturnUrl))
                        return HandleReturnUrl(ReturnUrl);

                    return RedirectToRoute("Home");
                }
                else if (!string.IsNullOrWhiteSpace(Response.ErrorText))
                {
                    Session.AddToErrorList(L(Response.ErrorText));
                }
                else
                {
                    Session.AddToErrorList(L("LoginFailed"));
                }

            }

            return View("Login", Model); // something went wrong 
        }
        [Route("/logout", Name = "Logout"), Authorize]
        public async Task<IActionResult> Logout()
        {
            if (UserContext.IsAuthenticated)
                await UserContext.SignOutAsync();

            return RedirectToRoute("Home");
        }

        [Route("/set-language", Name = "SetLanguage")]
        public IActionResult SetLanguage(string CultureCode, string ReturnUrl = "")
        { 
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(CultureCode)),
                new CookieOptions 
                {
                    Secure = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                    HttpOnly = true,
                    IsEssential = true,
                    Expires = DateTimeOffset.UtcNow.AddYears(1) 
                }
            );

            if (!string.IsNullOrWhiteSpace(ReturnUrl))
                return HandleReturnUrl(ReturnUrl);

            return RedirectToRoute("Home");
        }

        [Route("/notfound", Name = "NotFound")]
        public IActionResult NotFoundPage()
        {
            return NotFoundInternal("");
        }
        [Route("/notyet", Name = "NotYet")]
        public IActionResult NotYetPage()
        {
            return NotYetInternal("");
        }
        [Route("/access-denied", Name = "AccessDenied")]
        public async Task<IActionResult> AccessDenied()
        {
            await Task.CompletedTask;
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // the Error view gathers all error information
            return View();
        }

        [HttpGet("/plugin-test", Name = "PluginTest")]
        public IActionResult PluginTest()
        {
            return View();
        }

        [HttpGet("/ajax-demos", Name = "AjaxDemos")]
        public IActionResult AjaxDemos()
        {
            return View();
        }
    }
}
