namespace StudyLib.Entities
{

    [Table(nameof(ProductWarehouse))]
    [Index(nameof(ProductId), nameof(WarehouseId), IsUnique = true)]
    public class ProductWarehouse : BaseEntity
    {
        public ProductWarehouse() { }
        public ProductWarehouse(string ProductId, string WarehouseId, decimal AvailableQty)
        {
            this.SetId();
            this.ProductId = ProductId;
            this.WarehouseId = WarehouseId;
            this.AvailableQty = AvailableQty;
        }

        [MaxLength(40), Required]
        public string ProductId { get; set; }
        [MaxLength(40), Required]
        public string WarehouseId { get; set; }
        /// <summary>
        /// Available Qty, in this warehouse, in primary or base measure unit 
        /// </summary>
        [Precision(18, 4), Required]
        public decimal AvailableQty { get; set; }
    }


    public class ProductWarehouseEntityTypeConfiguration : IEntityTypeConfiguration<ProductWarehouse>
    {
        public void Configure(EntityTypeBuilder<ProductWarehouse> builder)
        {
             
        }
    }
}
