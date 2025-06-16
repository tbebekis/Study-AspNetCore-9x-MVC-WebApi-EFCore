

namespace Plugin.Test.Controllers
{
    public class TestController: ControllerMvcBase
    {
        [HttpGet("/plugin-test-no-layout", Name = "Plugin.Test.NoLayout"), AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/plugin-test-own-layout", Name = "Plugin.Test.OwnLayout"), AllowAnonymous]
        public IActionResult Index2()
        {
            return View();
        }

        [HttpGet("/plugin-test-host-layout", Name = "Plugin.Test.HostLayout"), AllowAnonymous]
        public IActionResult Index3()
        {
            return View();
        }


    }
}
