namespace StudyLib.Entities
{
    /// <summary>
    /// Represents a Warehouse containing products.
    /// </summary>
    [Table(nameof(Warehouse))]
    [Index(nameof(Name), IsUnique = true)]
    public class Warehouse : BaseEntity
    {
        public Warehouse() { }
        public Warehouse(string Name)
        {
            this.SetId();
            this.Name = Name;
        }

        [Required, MaxLength(128)]
        public string Name { get; set; }
    }

    public class WarehouseEntityTypeConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            //
        }
    }
}
