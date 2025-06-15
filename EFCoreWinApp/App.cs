namespace EFCoreWinApp
{
    static public partial class App
    {
        // ● public
        static public void Initialize()
        {
            if (DataContext.UseInMemoryDatabase)
                DemoData.AddDataInMemory(() => new DataContext());
            else 
                DataContext.EnsureDatabase(); 
        }


        //static public DataContext GetDataContext() => new DataContext();



 
    }
}
