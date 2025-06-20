namespace MvcApp.Library
{
    public class AppDataService<T>:  EFDataService<T> where T : BaseEntity
    {
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
