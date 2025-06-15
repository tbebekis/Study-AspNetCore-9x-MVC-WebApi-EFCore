namespace StudyLib.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    [Table(nameof(AppRole))]    
    public class AppRole : BaseEntity
    {
 
        public AppRole()
        {
        }
        public AppRole(string Name)
        {
            SetId();
            this.Name = Name;
        }
        public override string ToString()
        {
            return Name;
        }
 
        [Required, MaxLength(96)]
        public string Name { get; set; }
    }

    public class AppRoleEntityTypeConfiguration : IEntityTypeConfiguration<AppRole>
    {
        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            //builder
            //    .Property(e => e.ClientId)
            //    .IsRequired();


        }
    }
}
