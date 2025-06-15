namespace StudyLib.Entities
{

    [Table(nameof(SalesOrderLine))]
    public class SalesOrderLine : BaseEntity
    {
        public SalesOrderLine()
        {
        }
        public SalesOrderLine(string OrderId, string ProductId, decimal Qty, decimal Price, decimal DiscountPercent, decimal TaxPercent)
        {
            SetId();

            this.OrderId = OrderId;
            this.ProductId = ProductId;
            this.Qty = Qty;
            this.Price = Price;
            this.DiscountPercent = DiscountPercent;
            this.TaxPercent = TaxPercent;

            Calculate();
        }

        public void Calculate()
        {
            BaseAmount = Qty * Price;
            NetAmount = 0;
            DiscountAmount = 0;
            TaxAmount = 0;
            LineAmount = 0;

            if (BaseAmount > 0 && DiscountPercent > 0)
            {
                DiscountAmount = (BaseAmount / 100) * DiscountPercent;
                NetAmount = BaseAmount - DiscountAmount;
            }

            if (NetAmount > 0 && TaxPercent > 0)
            {
                TaxAmount = (NetAmount / 100) * TaxPercent;
                LineAmount = NetAmount + TaxAmount;
            }

        }

        [MaxLength(40), Required]
        public string OrderId { get; set; }
        [ForeignKey(nameof(OrderId)), JsonIgnore]
        public virtual SalesOrder Order { get; set; }

        [MaxLength(40), Required]
        public string ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }

        [Precision(18, 4), Required]
        public decimal Qty { get; set; }
        [Precision(18, 4), Required]
        public decimal Price { get; set; }
        [Precision(18, 4), Required]
        public decimal BaseAmount { get; set; }
        [Precision(18, 4), Required]
        public decimal DiscountPercent { get; set; }
        [Precision(18, 4), Required]
        public decimal DiscountAmount { get; set; }
        [Precision(18, 4), Required]
        public decimal NetAmount { get; set; }
        [Precision(18, 4), Required]
        public decimal TaxPercent { get; set; }
        [Precision(18, 4), Required]
        public decimal TaxAmount { get; set; }
        [Precision(18, 4), Required]
        public decimal LineAmount { get; set; }
    }

    public class SalesOrderLineEntityTypeConfiguration : IEntityTypeConfiguration<SalesOrderLine>
    {
        public void Configure(EntityTypeBuilder<SalesOrderLine> builder)
        {
            builder.Navigation(e => e.Product).AutoInclude();
        }
    }
}
