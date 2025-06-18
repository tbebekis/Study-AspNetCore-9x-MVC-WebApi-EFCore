namespace CommonLib.Entities
{

    [Table(nameof(ProductMeasureUnit))]
    [Index(nameof(ProductId), nameof(MeasureUnitId), IsUnique = true)]
    public class ProductMeasureUnit : BaseEntity
    {
        public ProductMeasureUnit() { }
        public ProductMeasureUnit(string ProductId, string MeasureUnitId, decimal QtyPerBaseUnit = 1)
        {
            this.SetId();
            this.ProductId = ProductId;
            this.MeasureUnitId = MeasureUnitId;
            this.QtyPerBaseUnit = QtyPerBaseUnit;
        }

        [MaxLength(40)]
        public string ProductId { get; set; }
        [MaxLength(40)]
        public string MeasureUnitId { get; set; }
        /// <summary>
        /// When this is the primary or base measure unit of a product, then it should be 1.
        /// <para>When this is a secondary measure unit, then it is the number of base units this unit contains. </para>
        /// </summary>
        public decimal QtyPerBaseUnit { get; set; }
 
    }

    public class ProductMeasureUnitEntityTypeConfiguration : IEntityTypeConfiguration<ProductMeasureUnit>
    {
        public void Configure(EntityTypeBuilder<ProductMeasureUnit> builder)
        {

        }
    }
}
