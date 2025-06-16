namespace MvcApp.Library
{

    /// <summary>
    /// Base Ajax controller.
    /// </summary>
    public class ControllerMvcAjax : ControllerMvc, IViewToStringConverter
    { 
        /// <summary>
        /// Renders a partial view to a string.
        /// <para>See AjaxController.MiniSearch() for an example.</para>
        /// </summary>
        string IViewToStringConverter.ViewToString(string ViewName, object Model, IDictionary<string, object> PlusViewData)
        {
            return this.RenderPartialViewToString(ViewName, Model, PlusViewData);
        }
        /// <summary>
        /// Renders a partial view to a string.
        /// <para>See AjaxController.MiniSearch() for an example.</para>
        /// </summary>
        string IViewToStringConverter.ViewToString(string ViewName, IDictionary<string, object> PlusViewData)
        {
            return this.RenderPartialViewToString(ViewName, PlusViewData);
        }

        /// <summary>
        /// Returns the exception text
        /// </summary>
        protected virtual string GetExceptionText(Exception e)
        {
            string Result = Lib.InDevMode ? e.ToString() : e.Message;
            return Result;
        }

        /// <summary>
        /// Processes the request.
        /// <para>Sets itself, i.e. this Controller, as the <see cref="AjaxRequest.ViewToStringConverter"/> which is used when processing a Ui request by converting a razor view to HTML.</para>
        /// <para>Calls the static method <see cref="AjaxRequest.Process(AjaxRequest)"/> 
        /// which processes the request by calling all registered <see cref="IAjaxRequestHandler"/> handlers.</para>
        /// <para>And finally assigns the result to the <see cref="AjaxPacketResult"/>.</para>
        /// </summary>
        protected virtual async Task<JsonResult> ProcessRequestAsync(AjaxRequest R)
        {
            await Task.CompletedTask;

            R.ViewToStringConverter = this;

            AjaxPacketResult Result = new ();
            try
            {
                AjaxRequestResult RequestResult = AjaxRequest.Process(R);

                Result = AjaxPacketResult.SetPacket(RequestResult.GetPacketObject(), true);  
                Result.IsSuccess = RequestResult.IsSuccess;
            }
            catch (Exception e)
            {
                Result.ErrorText = GetExceptionText(e);
            }

            return Json(Result);
        }
    }

}
