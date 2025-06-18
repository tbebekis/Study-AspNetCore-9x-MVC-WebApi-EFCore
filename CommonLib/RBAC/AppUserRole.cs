namespace CommonLib.Entities
{
    [Table(nameof(AppUserRole))]
    [Index(nameof(UserId), nameof(RoleId), IsUnique = true)]
    public class AppUserRole : BaseEntity
    {
 
        public AppUserRole() 
        { 
        }
        public AppUserRole(string UserId, string RoleId)
        {
            SetId();
            this.UserId = UserId;
            this.RoleId = RoleId;
        }
        
        [Required, MaxLength(40)]
        public string UserId { get; set; }
        [Required, MaxLength(40)]
        public string RoleId { get; set; }
    }

    public class AppUserRoleTypeConfiguration : IEntityTypeConfiguration<AppUserRole>
    {
        public void Configure(EntityTypeBuilder<AppUserRole> builder)
        {
            //builder
            //    .Property(e => e.ClientId)
            //    .IsRequired();

        }
    }
}
