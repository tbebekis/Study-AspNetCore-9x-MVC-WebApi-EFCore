namespace CommonLib.Entities
{
    [Table(nameof(AppRolePermission))]
    [PrimaryKey(nameof(RoleId), nameof(PermissionId))]
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


        [ForeignKey(nameof(AppRole))]
        [Required, MaxLength(40)]
        public string RoleId { get; set; }
        [ForeignKey(nameof(AppPermission))]
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
