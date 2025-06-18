namespace MvcApp.Models
{
    public class ProductModel
    {

        public override string ToString()
        {
            return Name;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string MeasureUnitId { get; set; }
        public string CategoryId { get; set; }

        public IList<SelectListItem> AvailableMeasureUnits { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }
    }
}
