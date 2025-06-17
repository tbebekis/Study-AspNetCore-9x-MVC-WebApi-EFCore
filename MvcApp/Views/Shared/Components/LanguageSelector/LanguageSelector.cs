namespace MvcApp.Components
{
    /// <summary>
    /// Language selector view component
    /// </summary>
    public class LanguageSelector : ViewComponent
    {
        /// <summary>
        /// Invokes the component and returns a view
        /// </summary>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            LanguageSelectorModel M = new LanguageSelectorModel();

            List<CultureInfo> List = Lib.GetSupportedCultures();

            M.Languages.AddRange(List); 
            M.SelectedLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToUpper();

            await Task.CompletedTask;
            
            return View(M);
        }
    }
}
