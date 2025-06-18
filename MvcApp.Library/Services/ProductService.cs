namespace MvcApp.Library
{
    public class ProductService : AppDataService<Product>
    {
        public ProductService()
        {
        }


        public async Task<ListResultPaged<Product>> GetProductsPagedAsync(ProductListFilter Filter)
        {
            ListResultPaged<Product> ResultList = null;

            if (!string.IsNullOrWhiteSpace(Filter.CategoryId))
            {
                ResultList = await GetListWithFilterPagedAsync(
                    new Paging(Filter.PageIndex, Filter.PageSize),      // paging
                    p => p.CategoryId == Filter.CategoryId,             // filter
                    q => q.OrderBy(p => p.Name),                        // order
                    p => p.Category                                     // includes
                    );
            }
            else if (!string.IsNullOrWhiteSpace(Filter.Term))
            {
                ResultList = await GetListWithFilterPagedAsync(
                    new Paging(Filter.PageIndex, Filter.PageSize),
                    p => p.Name.Contains(Filter.Term) || p.Id == Filter.Term,                                       // filter
                    q => q.OrderBy(p => p.Name),                                                                    // order
                    p => p.Category                                                                                 // includes
                    );
            }
 
            return ResultList;
        }
 
        public async Task<ListResultPaged<Product>> GetAllProductsWithPaging(int PageIndex, int PageSize)
        {
            ListResultPaged<Product> ResultList = await GetListPagedAsync(
                                                new Paging(PageIndex, PageSize),        // paging
                                                q => q.OrderBy(p => p.Name)             // order
                                                );

            return ResultList;
        }


        public async Task<ListResult<Product>> GetAllProducts()
        {
            string CultureCode = Lib.Culture.Name;
            string CacheKey = $"{nameof(GetAllProducts)}-{CultureCode}";

            // get products from cache
            // NOTE: in a real-world application most probably information such as products is NOT cached. 
            // This code is here just for demonstration purposes
            ListResult<Product> ResultList = await Lib.Cache.Get(CacheKey, async () => {
                ListResult<Product> InnerResultList = await GetListAsync(q => q.OrderBy(p => p.Name));
                CacheLoaderResult<ListResult<Product>> CacheResult = new(InnerResultList, Lib.Settings.Defaults.CacheTimeoutMinutes);
                return CacheResult;
            });

            return ResultList;
        }
    }
}
