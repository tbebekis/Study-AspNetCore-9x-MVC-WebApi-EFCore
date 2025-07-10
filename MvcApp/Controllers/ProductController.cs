namespace MvcApp.Controllers
{ 
 
    [Route("product")]
    public class ProductController : MvcAppController
    {
        ProductService Service = new();

        protected void Throw()
        {
            throw new Exception("This is a test exception.");
        }
        protected void TestLogger()
        {
            int CustomerId = 123;
            int OrderId = 456;

            using (Logger.BeginScope("THIS IS A SCOPE"))
            {
                Logger.LogCritical("Customer {CustomerId} order {OrderId} is completed.", CustomerId, OrderId);
                Logger.LogWarning("Just a warning");                 
            }
        }

        // ● List
        [Permission("Product.View")]
        [HttpGet("list", Name = "Product.List")]
        public async Task<ActionResult> Index()
        {
            //TestLogger();
            //Throw();

            //var SOService = new AppDataService<SalesOrder>();
            //var ResultList = await SOService.GetListAsync();

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

        // ● Insert
        [Permission("Product.Insert")]
        [HttpGet("insert", Name = "Product.Insert")]
        public async Task<ActionResult> Insert()
        {
            ProductModel Model = new();
            Model.AvailableMeasureUnits = await new MeasureUnitService().GetSelectList(AddDefaultItem:true);
            Model.AvailableCategories = await new CategoryService().GetSelectList(AddDefaultItem: true);

            return View(Model);
        }

        // ● Edit
        [Permission("Product.Edit")]
        [HttpGet("edit/{Id}", Name = "Product.Edit")]
        public async Task<ActionResult> Edit(string Id)
        {
            ItemResult<Product> ItemResult = await Service.GetByIdAsync(Id);
            if (!ItemResult.Succeeded)
                throw new Exception(ItemResult.ErrorText);

            Product Entity = ItemResult.Item;
            ProductModel Model = Lib.ObjectMapper.Map<ProductModel>(Entity);
            Model.AvailableMeasureUnits = await new MeasureUnitService().GetSelectList(AddDefaultItem: true);
            Model.AvailableCategories = await new CategoryService().GetSelectList(AddDefaultItem: true);
            return View(Model);
        }

        // ● Delete
        [Permission("Product.Delete")]
        [HttpGet("delete/{Id}", Name = "Product.Delete")]
        public async Task<ActionResult> Delete(string Id)
        {
            ItemResult<Product> ItemResult = await Service.DeleteAsync(Id);
            if (!ItemResult.Succeeded)
                throw new Exception(ItemResult.ErrorText);

            return RedirectToAction(nameof(Index));
        }

        // ● Save
        [ValidateAntiForgeryToken]
        [Permission("Product.Edit")]
        [HttpPost(Name = "Product.Save")]
        public async Task<ActionResult> Save(ProductModel Model)
        {
            bool IsNew = false;
            string EntityId = null;

            // --------------------------------------------------
            RedirectToActionResult Redirect()
            {
                if (IsNew)
                    return RedirectToAction(nameof(Insert));
                else
                    return RedirectToAction(nameof(Edit), new { EntityId });
            }
            // --------------------------------------------------

            if (ValidateModel(Model))
            {
                Product Entity = Lib.ObjectMapper.Map<Product>(Model);

                IsNew = Entity.IsNew();
                ItemResult<Product> ItemResult = null;
                if (IsNew)
                {
                    Entity.SetId();
                    ItemResult = await Service.InsertAsync(Entity);
                }
                else
                {
                    EntityId = Entity.Id;
                    ItemResult = await Service.UpdateAsync(Entity);
                }

                if (!ItemResult.Succeeded)
                {
                    this.SetModelStateToInvalid(ItemResult.ErrorText);
                    Session.AddToErrorList(ItemResult.ErrorText); 

                    return Redirect();
                }
                else
                {
                    return RedirectToAction(nameof(Edit), new { Entity.Id });
                }
            }

            return Redirect();
        }

        // ● Search (with paging)
        [Permission("Product.View")]
        [HttpGet("search", Name = "Product.Search")]
        public async Task<ActionResult> Search(string Term = "", string CategoryId = "", bool IncludeSubCategories = false)
        {
            ProductListFilter Filter = new();
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

        // ● Paging
        [Permission("Product.View")]
        [HttpGet("paging", Name = "Product.Paging")]
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
    }

 
}

/*
[ValidateAntiForgeryToken]
[Permission("Product.Insert")]
[HttpPost("insert", Name = "Product.Insert")]
public async Task<ActionResult> Insert(ProductModel Model)
{
    if (ValidateModel(Model))
    {
        Product Entity = Lib.ObjectMapper.Map<Product>(Model);
        Entity.SetId();

        ItemResult<Product> ItemResult = await Service.InsertAsync(Entity);
        if (!ItemResult.Succeeded)
        {
            this.SetModelStateToInvalid(ItemResult.ErrorText);
            Session.AddToErrorList(ItemResult.ErrorText);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    Model.AvailableMeasureUnits = await new MeasureUnitService().GetSelectList(AddDefaultItem: true);
    Model.AvailableCategories = await new CategoryService().GetSelectList(AddDefaultItem: true);
    return View(Model);
}

[ValidateAntiForgeryToken]
[Permission("Product.Edit")]
[HttpPost("edit", Name = "Product.Edit")]
public async Task<ActionResult> Edit(ProductModel Model)
{
    if (ValidateModel(Model))
    {
        Product Entity = Lib.ObjectMapper.Map<Product>(Model);

        ItemResult<Product> ItemResult = await Service.UpdateAsync(Entity);
        if (!ItemResult.Succeeded)
        {
            this.SetModelStateToInvalid(ItemResult.ErrorText);
            Session.AddToErrorList(ItemResult.ErrorText);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    Model.AvailableMeasureUnits = await new MeasureUnitService().GetSelectList(AddDefaultItem: true);
    Model.AvailableCategories = await new CategoryService().GetSelectList(AddDefaultItem: true);
    return View(Model);
}
// */