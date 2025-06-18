namespace CommonLib.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    [Table(nameof(Customer))]
    public class Customer: BaseEntity
    {
        public Customer() { }
        public Customer(string Name)
        {
            this.SetId();
            this.Name = Name;
        }

        [Required, MaxLength(128)]
        public string Name { get; set; }
    }


    public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            //
        }
    }
}
