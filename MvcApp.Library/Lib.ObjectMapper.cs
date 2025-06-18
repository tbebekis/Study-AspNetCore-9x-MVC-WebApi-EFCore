namespace MvcApp.Library
{
    static public partial class Lib
    {
        public static void AddObjectMappings()
        {
            // TODO: use the ObjectMapper property to configure object mappings here, e.g.
            //
            Lib.ObjectMapper.Add(typeof(Product), typeof(ProductModel), TwoWay: true);
        }
    }
}
