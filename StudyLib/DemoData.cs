using System.Data;

namespace StudyLib.Entities
{
    static public class DemoData
    {

        // ● fields
        static List<AppUser> UserList;
        static List<AppRole> RoleList;
        static List<AppPermission> PermissionList;
        static List<AppUserRole> UserRoleList;
        static List<AppRolePermission> RolePermissionList;

        static List<Category> CategoryList;
        static List<MeasureUnit> MeasureUnitList;
        static List<Warehouse> WarehouseList;
        static List<Product> ProductList;
        static List<ProductWarehouse> ProductWarehouseList;
        static List<ProductMeasureUnit> ProductMeasureUnitList;

        static List<Customer> CustomerList;
        static List<SalesOrder> SalesOrderList;
        static List<SalesOrderLine> SalesOrderLineList;

        // ● private (RBAC)
        static public List<AppUser> GetUserList()
        {
            List<AppUser> List = new List<AppUser>
            {
                new AppUser("user0", "secret", "User-0"),
                new AppUser("user1", "secret", "User-0"),

                new AppUser("client0", "secret", "Client-0"),
                new AppUser("client1", "secret", "Client-1"),

                new AppUser("Admin", "secret", "Admin-0")
            };

            return List;
        }
        static public List<AppRole> GetRoleList()
        {
            List<AppRole> List = new List<AppRole>
            {
                new AppRole("Admin"  ),
                new AppRole("Manager"),
                new AppRole("User"   ),
            };

            return List;
        }
        static public List<AppPermission> GetPermissionList()
        {
            List<AppPermission> List = new List<AppPermission>
            {
                new AppPermission(AppPermission.RbacAdmin),
                new AppPermission("Product.Create"),
                new AppPermission("Product.View"  ),
                new AppPermission("Product.Edit"  ),
                new AppPermission("Product.Delete"),
            }; 

            return List;
        }

        static public List<AppUserRole> GetUserRoleList()
        {
            List<AppUserRole> List = new List<AppUserRole>()
            {
                // ● Admin users
               new AppUserRole(
                    UserList.FirstOrDefault(u => u.UserName == "Admin").Id,
                    RoleList.FirstOrDefault(r => r.Name == "Admin").Id
                    ),

               // ● Manager users
               new AppUserRole(                    
                    UserList.FirstOrDefault(u => u.UserName == "user0").Id,
                    RoleList.FirstOrDefault(r => r.Name == "Manager").Id
                    ),
               new AppUserRole(
                    UserList.FirstOrDefault(u => u.UserName == "client0").Id,
                    RoleList.FirstOrDefault(r => r.Name == "Manager").Id
                    ),
            };

            return List;
        }
        static public List<AppRolePermission> GetRolePermissionList()
        {
            List<AppRolePermission> List = new List<AppRolePermission>
            {
                // ● Admin
                new AppRolePermission(
                    RoleList.FirstOrDefault(r => r.Name == "Admin").Id,
                    PermissionList.FirstOrDefault(r => r.Name == AppPermission.RbacAdmin).Id
                    ),
                new AppRolePermission(
                    RoleList.FirstOrDefault(r => r.Name == "Admin").Id,
                    PermissionList.FirstOrDefault(r => r.Name == "Product.Create").Id
                    ),
                new AppRolePermission(
                    RoleList.FirstOrDefault(r => r.Name == "Admin").Id,
                    PermissionList.FirstOrDefault(r => r.Name == "Product.View").Id
                    ),
                new AppRolePermission(
                    RoleList.FirstOrDefault(r => r.Name == "Admin").Id,
                    PermissionList.FirstOrDefault(r => r.Name == "Product.Edit").Id
                    ),
                new AppRolePermission(
                    RoleList.FirstOrDefault(r => r.Name == "Admin").Id,
                    PermissionList.FirstOrDefault(r => r.Name == "Product.Delete").Id
                    ),

                // ● Manager
                new AppRolePermission(
                    RoleList.FirstOrDefault(r => r.Name == "Manager").Id,
                    PermissionList.FirstOrDefault(r => r.Name == "Product.View").Id
                    ),
                 new AppRolePermission(
                    RoleList.FirstOrDefault(r => r.Name == "Manager").Id,
                    PermissionList.FirstOrDefault(r => r.Name == "Product.Edit").Id
                    ),
            };

            return List;
        }

        // ● private (Data)
        static List<Category> GetCategoryList()
        {
            List<Category> List = new List<Category>()
            {
                new Category("Foods"),
                new Category("Pharmacy"),
            };

            string GetCategoryId(string Name)
            {
                return List.FirstOrDefault(c => c.Name == Name).Id;
            }

            string FoodsId = GetCategoryId("Foods");
            string PharmacyId = GetCategoryId("Pharmacy");

            List.AddRange(new Category[]
            {
                new Category("Cotton", PharmacyId),
                new Category("Animal Foods", FoodsId),
                new Category("Spices", FoodsId),
                new Category("Nuts", FoodsId),
                new Category("Fruits", FoodsId),
                new Category("Dried Fruits", FoodsId),
                new Category("Vegetables", FoodsId),
                new Category("Cereals", FoodsId),
            });

            return List;
        }
        static List<MeasureUnit> GetMeasureUnitList()
        {
            List<MeasureUnit> List = new List<MeasureUnit>()
            {
                new MeasureUnit("Piece"),
                new MeasureUnit("Carton"),
                new MeasureUnit("Large Carton"),
                new MeasureUnit("Pallet"),
                new MeasureUnit("Sack"),
                new MeasureUnit("LargeSack"),
                new MeasureUnit("Box"),
                new MeasureUnit("Κilogram"),

            };

            return List;
        }
        static List<Warehouse> GetWarehouseList()
        {
            List<Warehouse> List = new List<Warehouse>()
            {
                new Warehouse("Location 1"),
                new Warehouse("Location 2"),
                new Warehouse("Location 3"),

            };

            return List;
        }
        static List<Product> GetProductList()
        {
            Random R = new Random();

            decimal GetPrice()
            {
                decimal Result = Convert.ToDecimal(R.NextDouble());
                Result = Math.Round(Result, 2);
                Result += R.Next(2, 40);
                return Result;
            }
            string GetCategoryId(string Name)
            {
                return CategoryList.FirstOrDefault(c => c.Name == Name).Id;
            }
            string GetUoMId(string Name)
            {
                return MeasureUnitList.FirstOrDefault(c => c.Name == Name).Id;
            }

            List<Product> List = new List<Product>()
            {
                new Product("Absorbent cotton", GetPrice(), GetCategoryId("Cotton"), GetUoMId("Box")),
                new Product("Alfalfa pellets", GetPrice(), GetCategoryId("Animal Foods"), GetUoMId("Sack")),
                new Product("Allspice", GetPrice(), GetCategoryId("Spices"), GetUoMId("Piece")),
                new Product("Almonds", GetPrice(), GetCategoryId("Nuts"), GetUoMId("Κilogram")),
                new Product("Aniseed", GetPrice(), GetCategoryId("Spices"), GetUoMId("Κilogram")),
                new Product("Apples", GetPrice(), GetCategoryId("Fruits"), GetUoMId("Κilogram")),
                new Product("Apples, dried", GetPrice(), GetCategoryId("Dried Fruits"), GetUoMId("Κilogram")),
                new Product("Apricot kernels", GetPrice(), GetCategoryId("Nuts"), GetUoMId("Κilogram")),
                new Product("Apricots, dried", GetPrice(), GetCategoryId("Dried Fruits"), GetUoMId("Κilogram")),
                new Product("Artichokes", GetPrice(), GetCategoryId("Vegetables"), GetUoMId("Κilogram")),
                new Product("Asparagus", GetPrice(), GetCategoryId("Vegetables"), GetUoMId("Κilogram")),
                new Product("Avocados", GetPrice(), GetCategoryId("Fruits"), GetUoMId("Κilogram")),
                new Product("Bananas", GetPrice(), GetCategoryId("Fruits"), GetUoMId("Κilogram")),
                new Product("Barley", GetPrice(), GetCategoryId("Cereals"), GetUoMId("Κilogram")),
                new Product("Bay leaves", GetPrice(), GetCategoryId("Spices"), GetUoMId("Box")),
            };

            return List;
        }
        static List<ProductWarehouse> GetProductWarehouseList()
        {
            Random R = new Random();

            decimal GetAvailableQty()
            {
                decimal Result = R.Next(50, 300);
                Result = Math.Round(Result, 2);
                return Result;
            }

            List<ProductWarehouse> List = new();

            ProductWarehouse Item;
            foreach (var W in WarehouseList)
            {
                foreach (var P in ProductList)
                {
                    Item = new ProductWarehouse(P.Id, W.Id, GetAvailableQty());
                    List.Add(Item);
                }
            }


            return List;
        }
        static public List<ProductMeasureUnit> GetProductMeasureUnitList()
        {
            List<ProductMeasureUnit> List = new();

            string GetProductId(string Name)
            {
                return ProductList.FirstOrDefault(p => p.Name == Name).Id;
            }
            string GetMeasureUnitId(string Name)
            {
                return MeasureUnitList.FirstOrDefault(p => p.Name == Name).Id;
            }


            List.Add(new ProductMeasureUnit(GetProductId("Absorbent cotton"), GetMeasureUnitId("Carton"), 12));
            List.Add(new ProductMeasureUnit(GetProductId("Bay leaves"), GetMeasureUnitId("Carton"), 32));
            List.Add(new ProductMeasureUnit(GetProductId("Allspice"), GetMeasureUnitId("Carton"), 24));

            return List;
        }

        static List<Customer> GetCustomerList()
        {
            List<Customer> List = new List<Customer>()
            {
                new Customer("Chuck Norris"),
                new Customer("Bruce Lee"),
                new Customer("Jason Statham"),
            };

            return List;
        }
        static List<SalesOrder> GetSalesOrderList()
        {

            List<SalesOrder> List = new();
            
            Random R = new Random();
            int OrderCount;

            DateTime DT = DateTime.Now;
            SalesOrder Item;
            foreach (var C in CustomerList)
            {
                OrderCount = R.Next(1, 5);
                for (int i = 0; i < OrderCount; i++)
                {
                    Item = new(C.Id, DT.AddDays(1));
                    List.Add(Item);
                }
            }

            return List;
        }
        static public List<SalesOrderLine> GetSalesOrderLineList()
        {
            List<SalesOrderLine> List = new();

            bool IsProductInList(string OrderId, string ProductId)
            {
                return List.FirstOrDefault(sol => sol.ProductId == ProductId && sol.OrderId == OrderId) != null;
            }

            Random R = new Random();
            int LineTries = (ProductList.Count - 1) * 5;
            int ProductIndex;
            Product P;
            SalesOrderLine Line;

            string OrderId; 
            string ProductId; 
            decimal Qty; 
            decimal Price; 
            decimal DiscountPercent; 
            decimal TaxPercent;

            foreach (var O in SalesOrderList)
            {
                for (int i = 0; i < LineTries; i++)
                {
                    ProductIndex = R.Next(0, ProductList.Count - 1);
                    P = ProductList[ProductIndex];
                    
                    OrderId = O.Id;
                    ProductId = P.Id;

                    if (!IsProductInList(OrderId, ProductId))
                    {
                        Qty = R.Next(1, 20);
                        Price = P.Price;
                        DiscountPercent = R.Next(1, 5) + Convert.ToDecimal(R.NextDouble());
                        TaxPercent = 24;

                        Line = new(OrderId, ProductId, Qty, Price, DiscountPercent, TaxPercent);
                        Line.Calculate();

                        List.Add(Line);
                    }
                }
            }

            return List;
        }


        static DemoData()
        {
            UserList = GetUserList();
            RoleList = GetRoleList();
            PermissionList = GetPermissionList();
            UserRoleList = GetUserRoleList();
            RolePermissionList = GetRolePermissionList();

            CategoryList = GetCategoryList();
            MeasureUnitList = GetMeasureUnitList();
            WarehouseList = GetWarehouseList();
            ProductList = GetProductList();

            ProductWarehouseList = GetProductWarehouseList();
            ProductMeasureUnitList = GetProductMeasureUnitList();

            CustomerList = GetCustomerList();
            SalesOrderList = GetSalesOrderList();
            SalesOrderLineList = GetSalesOrderLineList();
        }
 

        static public void AddDataInMemory(Func<DbContext> GetDataContextFunc)
        {
            using (var DataContext = GetDataContextFunc())
            {

                // ● RBAC
                DbSet<AppUser> UserSet = DataContext.Set<AppUser>();
                UserSet.AddRange(UserList);

                DbSet<AppRole> RoleSet = DataContext.Set<AppRole>();
                RoleSet.AddRange(RoleList);

                DbSet<AppPermission> PermissionSet = DataContext.Set<AppPermission>();
                PermissionSet.AddRange(PermissionList);

                DbSet<AppUserRole> ClientRoleSet = DataContext.Set<AppUserRole>();
                ClientRoleSet.AddRange(UserRoleList);

                DbSet<AppRolePermission> RolePermissionSet = DataContext.Set<AppRolePermission>();
                RolePermissionSet.AddRange(RolePermissionList);

                // ● Data
                DbSet<Category> Categories = DataContext.Set<Category>();
                Categories.AddRange(CategoryList);

                DbSet<MeasureUnit> MeasureUnits = DataContext.Set<MeasureUnit>();
                MeasureUnits.AddRange(MeasureUnitList);

                DbSet<Warehouse> Warehouses = DataContext.Set<Warehouse>();
                Warehouses.AddRange(WarehouseList);

                DbSet<Product> Products = DataContext.Set<Product>();
                Products.AddRange(ProductList);


                DbSet<ProductWarehouse> ProductWarehouses = DataContext.Set<ProductWarehouse>();
                ProductWarehouses.AddRange(ProductWarehouseList);

                DbSet<ProductMeasureUnit> ProductMeasureUnits = DataContext.Set<ProductMeasureUnit>();
                ProductMeasureUnits.AddRange(ProductMeasureUnitList);
 
                DbSet<Customer> Customers = DataContext.Set<Customer>();
                Customers.AddRange(CustomerList);

                // ● Trades
                DbSet<SalesOrder> SalesOrders = DataContext.Set<SalesOrder>();
                SalesOrders.AddRange(SalesOrderList);

                DbSet<SalesOrderLine> SalesOrderLines = DataContext.Set<SalesOrderLine>();
                SalesOrderLines.AddRange(SalesOrderLineList);
 
                DataContext.SaveChanges();
            }
        }
        static public void AddData(ModelBuilder modelBuilder)
        {
            // ● RBAC
            modelBuilder.Entity<AppUser>().HasData(UserList);
            modelBuilder.Entity<AppRole>().HasData(RoleList);
            modelBuilder.Entity<AppPermission>().HasData(PermissionList);
            modelBuilder.Entity<AppUserRole>().HasData(UserRoleList);
            modelBuilder.Entity<AppRolePermission>().HasData(RolePermissionList);

            // ● Data
            modelBuilder.Entity<Category>().HasData(CategoryList);
            modelBuilder.Entity<MeasureUnit>().HasData(MeasureUnitList);
            modelBuilder.Entity<Warehouse>().HasData(WarehouseList);
            modelBuilder.Entity<Product>().HasData(ProductList);

            modelBuilder.Entity<ProductWarehouse>().HasData(ProductWarehouseList);
            modelBuilder.Entity<ProductMeasureUnit>().HasData(ProductMeasureUnitList);

            modelBuilder.Entity<Customer>().HasData(CustomerList);

            // ● Trades
            modelBuilder.Entity<SalesOrder>().HasData(SalesOrderList);
            modelBuilder.Entity<SalesOrderLine>().HasData(SalesOrderLineList);
        }
    }
}
