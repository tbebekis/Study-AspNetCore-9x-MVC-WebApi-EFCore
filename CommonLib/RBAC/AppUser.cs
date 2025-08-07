namespace CommonLib.Entities
{

    /// <summary>
    /// A user in an MVC application.
    /// <para>A client in a WebApi application.</para>
    /// </summary>
    [Index(nameof(UserName), IsUnique = true)]
    [Table(nameof(AppUser))]
    public class AppUser: BaseEntity, IAppUser, IApiClient
    {
        List<AppRole> fRoles;
        List<AppPermission> fPermissions;

        public AppUser()
            : base() 
        {
        }
        public AppUser(AppUserType UserType, string UserName, string PlainTextPassword, string Name = "")
            : this()
        {
            this.SetId();
            this.UserType = UserType;
            this.UserName = UserName;
            this.PasswordSalt = Hasher.GenerateSalt(96);
            this.Password = Hasher.Hash(PlainTextPassword, this.PasswordSalt);
            this.Name = Name;
        }

        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(Name)? Name: UserName;
        }


        /// <summary>
        /// Required. 
        /// <para><strong>Unique.</strong></para>
        /// </summary> 
        [Required, MaxLength(96)]
        public string UserName { get; set; }
        /// <summary>
        /// The client secret encrypted
        /// <para><strong>Encrypted.</strong></para>
        /// </summary>
        [MaxLength(64)]
        public string Password { get; set; }
        /// <summary>
        /// The client secret salt
        /// </summary>
        [Column("Salt"), MaxLength(96), JsonIgnore]
        public string PasswordSalt { get; set; }
        /// <summary>
        /// Optional. The user/client name
        /// </summary> 
        [MaxLength(96)]
        public string Name { get; set; }
        public AppUserType UserType { get; set; }
        /// <summary>
        /// True when requestor is blocked by admins
        /// </summary>
        public bool IsBlocked { get; set; }

        [NotMapped]
        public List<AppRole> Roles
        {
            get
            {
                if (fRoles == null)
                    fRoles = RBAC.GetUserRoles(Id);
                return fRoles;
            }
        }
        [NotMapped]
        public List<AppPermission> Permissions
        {
            get
            {
                if (fPermissions == null)
                    fPermissions = RBAC.GetUserPermissions(Id);
                return fPermissions;
            }
        }

        /// <summary>
        /// The ClientId, admin generated
        /// </summary>
        [NotMapped]
        string IApiClient.ClientId 
        {
            get => UserName;
            set => UserName = value;
        }
    }

    public class ApiClientEntityTypeConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(u => u.UserType)
                .HasConversion(
                    v => v.ToString(),
                    v => (AppUserType)Enum.Parse(typeof(AppUserType), v));
        }
    }
}
