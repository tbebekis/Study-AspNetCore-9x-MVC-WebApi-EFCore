namespace CommonLib.Entities
{
    [Table(nameof(SalesOrder))]
    public class SalesOrder : BaseEntity
    {
        static int CodeNumber = 0;

        string fCode;

        public SalesOrder()
        { 
        }
        public SalesOrder(string CustomerId, DateTime EntryDate)
        {
            SetId();
            this.CustomerId = CustomerId;
            this.EntryDate = EntryDate;

            CodeNumber++;
            this.CodeNo = CodeNumber;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // not working with Sqlite // TODO: Sqlite sequence
        public int CodeNo { get; set; }
        [MaxLength(40)]
        public string Code //=> new string('0', 9 - CodeNo.ToString().Length) + CodeNo.ToString();
        {
            get => !string.IsNullOrWhiteSpace(fCode) ? fCode : new string('0', 9 - CodeNo.ToString().Length) + CodeNo.ToString();
            set => fCode = value;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public DateTime EntryDate { get; set; }
        [MaxLength(40), Required]
        public string CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }

        public virtual List<SalesOrderLine> Lines { get; set; }

    }

    public class SalesOrderEntityTypeConfiguration : IEntityTypeConfiguration<SalesOrder>
    {
        public void Configure(EntityTypeBuilder<SalesOrder> builder)
        {
        }
    }

}
