namespace CommonLib.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    [Table(nameof(Category))]
    public class Category : BaseEntity
    {
        public Category() { }
        public Category(string Name)
        {
            this.SetId();
            this.Name = Name;
        }
        public Category(string Name, string ParentId)
            : this(Name)
        {
            this.ParentId = ParentId;
        }

        [MaxLength(40)]
        public string ParentId { get; set; }
        [Required, MaxLength(128)]
        public string Name { get; set; }
    }

    public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            //
        }
    }
}
