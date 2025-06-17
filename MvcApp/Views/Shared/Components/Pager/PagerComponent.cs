namespace MvcApp.Components
{
    public class Pager : ViewComponent
    {
        /// <summary>
        /// Invokes the component and returns a view
        /// <para>Example call:</para>
        /// <para><c>@await Component.InvokeAsync("Pager", new { Info = MyPagingInfo }) </c></para>
        /// </summary>
        public async Task<IViewComponentResult> InvokeAsync(PagingInfo Info)
        {
            await Task.CompletedTask;
            return View(Info);
        }
    }
}
