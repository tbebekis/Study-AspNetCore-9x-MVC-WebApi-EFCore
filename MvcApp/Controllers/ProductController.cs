namespace MvcApp.Controllers
{
 
 
    [Route("product")]
    public class ProductController : MvcAppController
    {
        ProductService Service = new();

        [Permission("Product.View")]
        [HttpGet("search", Name = "Product.Search")]
        public async Task<ActionResult> Search(string Term = "", string CategoryId = "", bool IncludeSubCategories = false)
        {
            ProductListFilter Filter = new ();
            Filter.FullProductInfo = true;
            Filter.Term = Term;
            Filter.CategoryId = CategoryId;                          
            Filter.IncludeSubCategories = IncludeSubCategories;     // false - NOT USED here

            // get the data
            ListResultPaged<Product> ListResult = await Service.GetProductsPagedAsync(Filter);

            if (ListResult.Succeeded)
            {
                // map entities to models
                List<ProductModel> ModelList = Lib.ObjectMapper.Map<List<ProductModel>>(ListResult.List);

                // create the model
                ProductListModel ListModel = new();
                ListModel.PagingInfo = new PagingInfo(ListResult.Paging.TotalItems);
                ListModel.Products = ModelList;

                return View(ListModel);
            }

            return RedirectToErrorPage(ListResult.ErrorText);
        }
 
        [Permission("Product.View")]
        [HttpGet("list", Name = "Product.List")]
        public async Task<ActionResult> Index()
        {
            // get the data
            ListResult<Product> ListResult = await Service.GetAllProducts();

            if (ListResult.Succeeded)
            {
                // map entities to models
                List<ProductModel> ModelList = Lib.ObjectMapper.Map<List<ProductModel>>(ListResult.List);

                // create the model
                ProductListModel ListModel = new();
                // ListModel.PagingInfo = new PagingInfo(ListResult.TotalItems); // no paging in this view
                ListModel.Products = ModelList;
 
                return View(ListModel);
            }

            return RedirectToErrorPage(ListResult.ErrorText); 
        }

        [Permission("Product.View")]
        [HttpGet("paging")]
        public async Task<ActionResult> Paging()
        {
            int PageIndex = PagingInfo.GetQueryStringPageIndex();
            int PageSize = PagingInfo.GetQueryStringPageSize();

            // get the data
            ListResultPaged<Product> ListResult = await Service.GetAllProductsWithPaging(PageIndex, PageSize);

            if (ListResult.Succeeded)
            {
                // map entities to models
                List<ProductModel> ModelList = Lib.ObjectMapper.Map<List<ProductModel>>(ListResult.List);

                // create the model
                ProductListModel ListModel = new();
                ListModel.PagingInfo = new PagingInfo(ListResult.Paging.TotalItems);
                ListModel.Products = ModelList;

                return View(ListModel);
            }

            return RedirectToErrorPage(ListResult.ErrorText);
        }


        [Permission("Product.Insert")]
        [HttpGet("insert")]
        public async Task<ActionResult> Insert()
        {
            ProductModel Model = new();
            Model.AvailableMeasureUnits = await new AppDataService<MeasureUnit>().GetSelectList(AddDefaultItem:true);
            Model.AvailableCategories = await new AppDataService<Category>().GetSelectList(AddDefaultItem: true);

            return View(Model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        [Permission("Product.View")]
        [HttpGet("view/{Id}")]
        public ActionResult Details(string Id)
        {
            return View();
        }



        // [HttpGet("/blog/update/{blogpostid}", Name = "UpdateBlogPost")]

        // GET: ProductController/Edit/5

        [Permission("Product.Edit")]
        [HttpGet("edit/{Id}")]
        public ActionResult Edit(string Id)
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Permission("Product.Delete")]
        [HttpGet("delete/{Id}")]
        public ActionResult Delete(string Id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string Id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    
        
    }

 
}
