namespace EFCoreWinApp
{
    public partial class MainForm : Form
    {

        void FormInitialize()
        {
            LogBox.Initialize(edtLog);
 

            App.Initialize();

            // ● no service
            btnProductsNoService.Click += (s, e) => App.AllProductsNoService();

            // ● service
            btnProductsWithService.Click += async (s, e) => await App.ProductList(GetPageNo()); 
            btnProductsWithFilterFunc.Click += async (s, e) => await App.ProductListWithFilterProc(GetPageNo());
            btnProductsWithEFListParams.Click += async (s, e) => await App.ProductListWithEFListParams(GetPageNo());
            btnSalesOrders.Click += async (s, e) => await App.SalesOrders(GetPageNo());

            btnProductsWithSqlFilter.Click += async (s, e) => await App.ProductListWithSqlFilter(GetPageNo(), edtSqlFilter.Text.Trim());
            btnSingleProductById.Click += async (s, e) => await App.SingleProductById(edtProductId.Text.Trim());
        }
        int GetPageNo()
        {
            string S = edtPageNo.Text.Trim();
            return Convert.ToInt32(S);
        }


        protected override void OnShown(EventArgs e)
        {
            if (!DesignMode)
                FormInitialize();
            base.OnShown(e);
        }

        public MainForm()
        {
            InitializeComponent();
        }
    }
}
