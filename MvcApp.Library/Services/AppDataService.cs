using System.Runtime.CompilerServices;

namespace MvcApp.Library
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

        public override async Task<List<SelectListItem>> GetSelectList(string SelectedId = "", bool AddDefaultItem = false)
        {
            string CultureCode = Lib.Culture.Name;
            string CacheKey = $"{typeof(T).Name}-{nameof(GetSelectList)}-{CultureCode}";

            /// get from cache
            /// This code is here just for demonstration purposes.
            /// NOTE: in a real-world application information such as products, most probably, is NOT cached. 
            /// Other types (tables) such as Countries, Currencies, Measure Units, etc. used in look-ups, may be a better fit.
            List<SelectListItem> ResultList = await Lib.Cache.Get(CacheKey, async () => {
                List<SelectListItem> InnerResultList = await base.GetSelectList(SelectedId, AddDefaultItem);
                CacheLoaderResult<List<SelectListItem>> CacheResult = new(InnerResultList, Lib.Settings.Defaults.CacheTimeoutMinutes);
                return CacheResult;
            });

            return ResultList;
        }
    }
}
