namespace WebApiApp.Controllers
{
    [ApiController]
    [Consumes("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WebApiController : ControllerBase
    {
        /*
        /// <summary>
        /// Creates a <see cref="Microsoft.AspNetCore.Mvc.JsonResult" /> object that serializes the specified data object to JSON.
        /// </summary>
        [NonAction]
        protected JsonResult Json(object data)
        {
            return new JsonResult(data);
        }
        */
    }
}
