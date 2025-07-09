namespace MvcApp.Components
{
    public class MainMenu : ViewComponent
    {
        /// <summary>
        /// Invokes the component and returns a view
        /// <para>Example call:</para>
        /// <para><c>@await Component.InvokeAsync("MainMenu") </c></para>
        /// </summary>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.CompletedTask;

            MenuModel Model = GetMenu("MainMenu");            
            return View(Model);
        }

        public static MenuModel GetMenu(string MenuName)
        {
            // ● menu bar
            MenuModel MenuBar = new MenuModel();

            MenuModel mnuHome = MenuBar.Add("Home", "/");
            MenuModel mnuDemos = MenuBar.Add("Demos");
            MenuModel mnuProducts = MenuBar.Add("Products");

            // ● demos
            MenuModel mnuPluginTest = mnuDemos.Add("Plugin Test", "/plugin-test");
            MenuModel mnuAjaxDemos = mnuDemos.Add("Ajax Demos", "/ajax-demos");
            MenuModel mnuHealthChecks = mnuDemos.Add("Health Checks", "/health-check");
            MenuModel mnuEndPoints = mnuDemos.Add("End Points", "/end-points");

            // ● products
            MenuModel mnuProductList = mnuProducts.Add("Product List", "/product/list");
            MenuModel mnuProductListPaging = mnuProducts.Add("Product Lis with Paging", "/product/paging");

            return MenuBar;
        }
    }
}
