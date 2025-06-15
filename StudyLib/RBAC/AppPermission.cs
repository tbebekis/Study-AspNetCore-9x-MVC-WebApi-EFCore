namespace StudyLib.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    [Table(nameof(AppPermission))]
    public class AppPermission : BaseEntity
    {
        public const string RbacAdmin = "RBAC.Admin"; 

        public AppPermission()
        {
        }
        public AppPermission(string Name)
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

    public class AppPermissionEntityTypeConfiguration : IEntityTypeConfiguration<AppPermission>
    {
        public void Configure(EntityTypeBuilder<AppPermission> builder)
        {
            //builder
            //    .Property(e => e.ClientId)
            //    .IsRequired();
        }
    }
}
