namespace MvcApp.Library
{
    /// <summary>
    /// Helper just to demonstrate the use of <see cref="AjaxRequest"/> with <see cref="IAjaxRequestHandler"/>.
    /// </summary>
    static public class AjaxRequestHandlers
    {
        /// <summary>
        /// Represents an object that handles an <see cref="AjaxRequest"/> and returns an <see cref="AjaxRequestResult"/>
        /// </summary>
        public class AjaxHandler : IAjaxRequestHandler
        {

            /// <summary>
            /// Processes an <see cref="AjaxRequest"/> and if it handles the request returns an <see cref="AjaxRequestResult"/>. Else returns null.
            /// </summary>
            public AjaxRequestResult Process(AjaxRequest Request)
            {
                if (Request.Type == AjaxRequestType.Ui)  // Ajax call requests a MVC View 
                {
                    // find the view info
                    // NOTE: A AjaxViewInfoProvider along with a registry for such providers maybe used here
                    AjaxViewInfo ViewInfo = GetViewInfo(Request);

                    if (ViewInfo != null)
                    {
                        AjaxRequestResult Result = new AjaxRequestResult(Request.OperationName);

                        // set the HtmlText if empty
                        string HtmlText = Result["HtmlText"] as string;

                        if (string.IsNullOrWhiteSpace(HtmlText) && !string.IsNullOrWhiteSpace(ViewInfo.RazorViewNameOrPath))
                        {
                            HtmlText = Request.ViewToStringConverter.ViewToString(ViewInfo.RazorViewNameOrPath, ViewInfo.Model, ViewInfo.ViewData);
                            Result["HtmlText"] = HtmlText;
                        }

                        return Result;
                    }
                }
                else  // AjaxRequestType.Proc - Ajax call requests to call a method
                {
                    if (Lib.IsSameText(Request.OperationName, "AjaxRequest_OperationName"))
                        return AjaxRequest_OperationName(Request);

                    if (Lib.IsSameText(Request.OperationName, "AjaxRequest_Model"))
                        return AjaxRequest_Model(Request);
                }


                return null;
            }
        }

        static AjaxViewInfo GetViewInfo(AjaxRequest Request)
        {
            AjaxViewInfo ViewInfo = null;
            if (Lib.IsSameText(Request.OperationName, "AjaxDemoView"))
            {
                ViewInfo = new AjaxViewInfo();
                ViewInfo.RazorViewNameOrPath = "AjaxDemoView";
                ViewInfo.Model = new AjaxModel()
                {

                    Message = "This is a Asp.Net MVC View rendered from the server",
                    Value = 123.45M
                };
                ViewInfo.ViewData["Title"] = "Ajax Demo View";

                return ViewInfo;
            }

            return ViewInfo;
        }

        static AjaxRequestResult AjaxRequest_OperationName(AjaxRequest Request)
        {
            AjaxRequestResult Result = new AjaxRequestResult(Request.OperationName);
            Result["RequestId"] = Request.Id;
            Result["AppName"] = "MvcApp";
            Result["Html"] = false;
            Result["Number"] = 123.45;
            Result["Date"] = DateTime.Now;

            Result.IsSuccess = true;

            return Result;
        }
        static AjaxRequestResult AjaxRequest_Model(AjaxRequest Request)
        {
            AjaxModel M = Request.ParamsToPacket<AjaxModel>();
            M.Message = "The App just received this message: " + M.Message;
            M.Value = M.Value * 2;

            AjaxRequestResult Result = new AjaxRequestResult(Request.OperationName);
            Result["RequestId"] = Request.Id;
            Result["Model"] = M;
            Result.IsSuccess = true;

            return Result;
        }

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        static AjaxRequestHandlers()
        {
        }

        static public void Initialize()
        {
            AjaxRequest.Register(new AjaxHandler());
        }
    }


}
