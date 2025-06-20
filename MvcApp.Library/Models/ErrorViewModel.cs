namespace MvcApp.Models
{
    public class ErrorViewModel
    {
        string GetFullStack(Exception Ex)
        {
            StringBuilder SB = new();

            Exception E = Ex;

            while (E != null)
            {
                SB.AppendLine(Ex.ToString());
                E = E.InnerException;
                if (E != null)
                {
                    SB.AppendLine(" ----------------------------------------------------------------");
                    SB.AppendLine(" ");
                }
            }

            return SB.ToString();
        }

        /// <summary>
        /// Updates this instance's properties.
        /// <para>It is called by the Error view.</para>
        /// </summary>
        public void Update(HttpContext HttpContext, ViewDataDictionary ViewData, IWebHostEnvironment HostEnvironment)
        {
            // ● RequestId
            if (string.IsNullOrWhiteSpace(RequestId)) 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            // ● Exception
            if (Exception == null)
            {
                Exception = ViewData.ContainsKey("Exception") ? ViewData["Exception"] as Exception : null;

                if (Exception == null)
                {
                    var ExceptionPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                    if (ExceptionPathFeature != null)
                        Exception = ExceptionPathFeature.Error;
                }
            }

            // ● ErrorMessage
            if (string.IsNullOrWhiteSpace(ErrorMessage))
                ErrorMessage = Exception != null ? Exception.Message : "Unknown Error";


            // ● StackTrace
            StackTrace = !HostEnvironment.IsDevelopment() ? null : (Exception != null ? GetFullStack(Exception) : null);
        }

        public Exception Exception { get; set; }
        public string ErrorMessage { get; set; }
        public string RequestId { get; set; }
        public string StackTrace { get; set; }
    }
}
