namespace MvcApp.Controllers
{
    /*
    [Route("product")]
    public class ProductController : MvcAppController
    {

        [Permission("Product.View")]
        [HttpGet("search", Name = "Product.Search")]
        public ActionResult Search(string Term = "", string CategoryId = "", bool IncludeSubCategories = false)
        {
            ProductListFilter Filter = new ();
            Filter.FullProductInfo = true;
            Filter.Term = Term;
            Filter.CategoryId = CategoryId;                         // "376"; // CategoryId;
            Filter.IncludeSubCategories = IncludeSubCategories;     // true;  // IncludeSubCategories;

            // get the data
            ListDataResult<Product> ListResult = DataStore.GetProducts(Filter);

            if (ListResult.Succeeded)
            {
                // map entities to models
                List<ProductModel> ModelList = WLib.ObjectMapper.Map<List<ProductModel>>(ListResult.List);

                // create the model
                ProductListModel ListModel = new();
                ListModel.PagingInfo = new PagingInfo(ListResult.TotalItems);
                ListModel.Products = ModelList;

                return View(ListModel);
            }


            return RedirectToErrorPage(ListResult.ErrorText);
        }
 
        [Permission("Product.View")]
        [HttpGet("list", Name = "Product.List")]
        public ActionResult Index()
        {
            // get the data
            ListDataResult<Product> ListResult = DataStore.GetAllProducts();

            if (ListResult.Succeeded)
            {
                // map entities to models
                List<ProductModel> ModelList = WLib.ObjectMapper.Map<List<ProductModel>>(ListResult.List);

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
        public ActionResult Paging()
        {
            int PageIndex = PagingInfo.GetQueryStringPageIndex();
            int PageSize = PagingInfo.GetQueryStringPageSize();

            // get the data
            ListDataResult<Product> ListResult = DataStore.GetAllProductsWithPaging(PageIndex, PageSize);

            if (ListResult.Succeeded)
            {
                // map entities to models
                List<ProductModel> ModelList = WLib.ObjectMapper.Map<List<ProductModel>>(ListResult.List);

                // create the model
                ProductListModel ListModel = new();
                ListModel.PagingInfo = new PagingInfo(ListResult.TotalItems);
                ListModel.Products = ModelList;

                return View(ListModel);
            }

            return RedirectToErrorPage(ListResult.ErrorText);
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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
    */
}
