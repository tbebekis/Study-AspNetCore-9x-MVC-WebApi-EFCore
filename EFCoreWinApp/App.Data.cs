namespace EFCoreWinApp
{
    static public partial class App
    {
        static void ShowMessage(string Text)
        {
            LogBox.Clear();
            LogBox.AppendLine(Text);
        }
        static void ShowData(object Data)
        {
            LogBox.Clear();
            string JsonText = Json.Serialize(Data);
            LogBox.AppendLine(JsonText);
        }

        // ● no service
        static public void AllProductsNoService()
        {
            using (var DataContext = new DataContext())
            {
                DbSet<Product> DbSet = DataContext.Set<Product>();
                var List = DbSet.ToList();
                ShowData(List);
            }
        }

        // ● service
        static public async Task ProductList(int PageNo = 0)
        {
            var Service = new EFDataService<Product>(typeof(DataContext));
            var Result = await Service.GetListPagedAsync(new Paging(PageNo, 5),         // paging
                                                         q => q.OrderBy(p => p.Name)    // order by
                                                         );
            ShowData(Result);
        }
        static public async Task ProductListWithSqlFilter(int PageNo = 0, string Filter = "Name like '%al%'")
        {
            if (string.IsNullOrWhiteSpace(Filter))
            {
                ShowMessage("Filter is required");
                return;
            }

            var Service = new EFDataService<Product>(typeof(DataContext));

            // SQLite does not support expressions of type 'decimal' in ORDER BY clauses
            var Result = await Service.GetListWithSqlFilterPagedAsync(new Paging(PageNo, 5),                // paging
                                                                      Filter,                               // SQL WHERE filter
                                                                      q => q.OrderBy(p => (double)p.Price)  // order by
                                                                      );
            ShowData(Result);
        }
        static public async Task ProductListWithFilterProc(int PageNo = 0)
        {
            var Service = new EFDataService<Product>(typeof(DataContext));

            var Result = await Service.GetListWithFilterPagedAsync(new Paging(PageNo, 5),       // paging
                                                                  p => p.Price > 10.5M,         // filter
                                                                  q => q.OrderBy(p => p.Name),  // order by
                                                                  p => p.Category               // include
                                                                  );

            ShowData(Result);
        }
 
        static public async Task SingleProductById(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                ShowMessage("Id is required");
                return;
            }

            var Service = new EFDataService<Product>(typeof(DataContext));
            var Result = await Service.GetByIdAsync(Id);
            ShowData(Result);
        }

        static public async Task SalesOrders(int PageNo = 0)
        {
            var Service = new EFDataService<SalesOrder>(typeof(DataContext));

            // with two Include() functions
            var Result = await Service.GetListPagedAsync(new Paging(PageNo, 5),         // paging
                                                         null,                          // order by
                                                         o => o.Customer,               // include 1
                                                         o => o.Lines                   // include 2
                                                         );

            ShowData(Result);
        }

        // ● extensions
        static public async Task ProductListWithEFListParams(int PageNo = 0)
        {
            using (var DataContext = new DataContext())
            {
                EFListParams<Product> Params = new EFListParams<Product>(p => p.Category)
                {
                    Paging = new Paging(PageNo, 5),                    
                };

                // or
                // Params.IncludeFuncs.Add(p => p.Category);
 
                var List = await DataContext.GetList<Product>(Params);

                ListResultPaged<Product> Result = new(Params.Paging, List);
                ShowData(Result);
            }
        }

 
    }
}
