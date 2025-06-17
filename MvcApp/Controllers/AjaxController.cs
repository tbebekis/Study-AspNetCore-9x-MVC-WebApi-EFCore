namespace MvcApp.Controllers
{
    [AuthorizeAjax]
    public class AjaxController: MvcBaseControllerAjax
    {
        [HttpPost("/Ajax/PlainAjaxCall")]
        public async Task<JsonResult> PlainAjaxCall([FromBody] AjaxModel M)
        {
            await Task.CompletedTask;

            M.Message = "The App just received this message: " + M.Message;
            M.Value = M.Value * 2;

            return Json(M);
        }

        [HttpPost("/Ajax/Ajax_PostModelAsync")]
        public async Task<JsonResult> PostModelAsync([FromBody] AjaxModel M)
        {
            await Task.CompletedTask;

            M.Message = "The App just received this message: " + M.Message;
            M.Value = M.Value * 2;

            return Json(M);
        }

        [HttpPost("/Ajax/Request")]
        public async Task<JsonResult> AjaxRequest([FromBody] AjaxRequest R)
        {
            return await ProcessRequestAsync(R);
        }



        /// <summary>
        /// Constructor
        /// </summary>
        public AjaxController()
        {
        }
    }
}
