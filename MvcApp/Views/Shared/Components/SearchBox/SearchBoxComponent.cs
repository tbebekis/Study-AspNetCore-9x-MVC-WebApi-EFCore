namespace MvcApp.Components
{
    public class SearchBox : ViewComponent
    {
        static readonly System.Threading.Lock syncLock = new();
        static int Counter = 0;
        static void GetIds(out string BoxId, out string ButtonId)
        {
            lock (syncLock)
            {
                Counter++;
                BoxId = $"edtSearchBox{Counter}";
                ButtonId = $"btnSearch{Counter}";
            }
        }
         
        /// <summary>
        /// Invokes the component and returns a view
        /// <para>Example call:</para>
        /// <para><c>@await Component.InvokeAsync("Pager", new { Info = MyPagingInfo }) </c></para>
        /// </summary>
        public async Task<IViewComponentResult> InvokeAsync(PagingInfo Info)
        {
            string BoxId;
            string ButtonId;
            GetIds(out BoxId, out ButtonId);
            
            SearchBoxModel Model = new();
            Model.Url = "/product/search?term=";
            Model.BoxId = BoxId;
            Model.ButtonId = ButtonId;
 
            await Task.CompletedTask;
            return View(Model);
        }
    }
}


namespace MvcApp.Models
{
    public class SearchBoxModel
    {
        public string Url { get; set; }
        public string BoxId { get; set; }
        public string ButtonId { get; set; }
    }
}
