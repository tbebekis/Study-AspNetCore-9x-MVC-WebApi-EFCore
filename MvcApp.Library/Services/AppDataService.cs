namespace MvcApp.Library
{
    public class AppDataService<T>:  EFDataService<T> where T : BaseEntity
    {
        public AppDataService()
            : base(typeof(DataContext))
        {
        }
    }
}
