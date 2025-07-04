using System.Reflection.Metadata;

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

        // not working with Sqlite
        // In Sqlite only the primary key can be configured as auto-increment
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]  
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
            // The requirement is to have an auto-increment field, the CodeNo,
            // which is used in constructing the Code property.
            // Sqlite supports auto-increment fields but only in primary keys.
            // Our primary key is a string.
            // So we use a computed column to read the rowid Sqlite value.
            // SEE: https://www.sqlite.org/lang_createtable.html#rowid
            builder.Property(u => u.CodeNo)
                .HasComputedColumnSql("rowid", false);
        }
    }

}
