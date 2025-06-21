using System.Runtime.CompilerServices;

namespace WebApiApp.Library
{
    public class AppDataService<T>:  EFDataService<T> where T : BaseEntity
    {
        protected override void HandleException(Exception Ex, DataResult DataResult, [CallerMemberName] string CallerName = "")
        {
            if (DataResult != null)
                DataResult.ExceptionResult(Ex);

            try
            {
                string Service = this.GetType().Name;

                StackTrace StackTrace = new StackTrace();
                string Method = StackTrace.GetFrame(1).GetMethod().Name;
                Method = !string.IsNullOrWhiteSpace(CallerName) ? CallerName : Method;

                ILogger Logger = Lib.CreateLogger(Service);
                Logger.LogError(Ex, "Error in Service: {Service}, Method: {Method} ", Service, Method);
            }
            catch
            {
            }
        }

        public AppDataService()
            : base(typeof(DataContext))
        {
        }
    }
}
