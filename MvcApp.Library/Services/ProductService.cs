using System.Reflection.Metadata;

namespace MvcApp.Library
{
    public class ProductService : AppDataService<Product>
    {
        public ProductService()
        {
        }


        public async Task<ListResultPaged<Product>> GetProductsPagedAsync(ProductListFilter Filter)
        {

            ListResultPaged<Product> ResultList = new();
            try
            {             
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
            }
            catch (Exception Ex)
            {
                HandleException(Ex, ResultList);
            }

            return ResultList;
        }
 
        public async Task<ListResultPaged<Product>> GetAllProductsWithPaging(int PageIndex, int PageSize)
        {
            ListResultPaged<Product> ResultList = new();
            try
            {
                ResultList = await GetListPagedAsync(
                                    new Paging(PageIndex, PageSize),        // paging
                                    q => q.OrderBy(p => p.Name)             // order
                                    );

                
            }
            catch (Exception Ex)
            {
                HandleException(Ex, ResultList);
            }

            return ResultList;
        }


        public async Task<ListResult<Product>> GetAllProducts()
        {
            ListResult<Product> ResultList = new();
            try
            {                
                ResultList = await GetListAsync(q => q.OrderBy(p => p.Name));
            }
            catch (Exception Ex)
            {
                HandleException(Ex, ResultList);
            }

            return ResultList;
        }
    }
}
