namespace CommonLib.Entities
{
    [Table(nameof(AppRolePermission))]
    [Index(nameof(RoleId), nameof(PermissionId), IsUnique = true)]
    public class AppRolePermission : BaseEntity
    {
        public AppRolePermission() 
        { 
        }
        public AppRolePermission(string RoleId, string PermissionId)
        {
            SetId();
            this.RoleId = RoleId;
            this.PermissionId = PermissionId;
        }

        [Required, MaxLength(40), ForeignKey()]
        public string RoleId { get; set; }
        [Required, MaxLength(40)]
        public string PermissionId { get; set; }
    }


    public class AppRolePermissionTypeConfiguration : IEntityTypeConfiguration<AppRolePermission>
    {
        public void Configure(EntityTypeBuilder<AppRolePermission> builder)
        {
            //builder
            //    .Property(e => e.ClientId)
            //    .IsRequired();
        }
    }
}
