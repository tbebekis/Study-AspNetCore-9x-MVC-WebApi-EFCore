namespace WebApiApp.Controllers
{

    [Tags("Products")]
    [Route("product")]
    public class ProductController : WebApiController
    {
        AppDataService<Product> Service = new();

        // ● construction
        public ProductController()
        {
        }

        // ● actions
        [EndpointDescription("Returns the list of all products.")]
        [HttpGet("list", Name = "Product.List"), Produces<ListResult<Product>>]
        public async Task<ListResult<Product>> List()
        {
            ListResult<Product> Result = await Service.GetListAsync();
            return Result;
        }

        [EndpointDescription("Returns the list of all products using pagination.")]
        [HttpPost("list/paged", Name = "Product.List.Paged"), Produces<ListResultPaged<Product>>]
        public async Task<ListResultPaged<Product>> PagedList(PagedRequest Model)
        {
            ListResultPaged<Product> Result = await Service.GetListPagedAsync(new Paging(Model.PageIndex, Model.PageSize), null);
            return Result;
        }


    }

}
