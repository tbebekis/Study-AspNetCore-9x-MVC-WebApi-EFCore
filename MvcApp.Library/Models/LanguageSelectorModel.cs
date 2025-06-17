namespace MvcApp.Models
{
    public class LanguageSelectorModel
    {
        public LanguageSelectorModel()
        {
        }

        public List<CultureInfo> Languages { get; } = new List<CultureInfo>();
        public string SelectedLanguage { get; set; }
    }
}
