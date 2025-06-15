namespace StudyLib.Entities
{
    [Table(nameof(Product))]
    [Index(nameof(Name), IsUnique = true)] 
    public class Product : BaseEntity
    {

        public Product() { }
        public Product(string Name, decimal Price, string CategoryId, string MeasureUnitId)
        {
            this.SetId();
            this.Name = Name;
            this.Price = Price;
            this.CategoryId = CategoryId;
            this.MeasureUnitId = MeasureUnitId;
        }

        public override string ToString() => Name;
 
 
        [MaxLength(128), Required]
        public string Name { get; set; }
        [Precision(18, 4), Required]
        public decimal Price { get; set; }

        /// <summary>
        /// This is the primary or base measure unit
        /// </summary>
        [MaxLength(40), Required]
        public string MeasureUnitId { get; set; }

        [MaxLength(40), Required]
        public string CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }
    }


    public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            //builder.Navigation(e => e.Category).AutoInclude();
        }
    }
}
