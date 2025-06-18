namespace MvcApp.Models
{
    public class ProductModel
    {

        public override string ToString()
        {
            return Name;
        }

        public string Id { get; set; }

        [StringLength(128), RequiredEx]
        public string Name { get; set; }
        [Range(0, 1000000000), RequiredEx]
        public double Price { get; set; }
        [Title("Measure Unit"), RequiredEx]
        public string MeasureUnitId { get; set; }
        [Title("Category"), RequiredEx]
        public string CategoryId { get; set; }

        public IList<SelectListItem> AvailableMeasureUnits { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }
    }
}
