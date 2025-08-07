namespace CommonLib.Entities
{
    /// <summary>
    /// Represents a visitor of the site
    /// </summary>
    [Table(nameof(Visitor))]
    public class Visitor: BaseEntity
    {
        /// <summary>
        /// The visitor Id
        /// </summary>
        [NotMapped]
        public string Code
        {
            get => Id;
            set => Id = value;
        }

        /// <summary>
        /// Culture, i.e. el-GR
        /// </summary>
        [MaxLength(40)]
        public string CultureCode { get; set; } = "en-US";
        /// <summary>
        /// When a visitor is registered is associated to a user.
        /// </summary>
        [MaxLength(40)]
        public string UserId { get; set; }


        /// <summary>
        /// Last IP address
        /// </summary>
        [MaxLength(92)]
        public string IpAddress { get; set; }
        /// <summary>
        /// Last Access date-time
        /// </summary>
        public DateTime LastAccessDT { get; set; } = DateTime.MinValue;
        /// <summary>
        /// Last Login date-time
        /// </summary>
        public DateTime LastLoginDT { get; set; } = DateTime.MinValue;
    }

    public class VisitorEntityTypeConfiguration : IEntityTypeConfiguration<Visitor>
    {
        public void Configure(EntityTypeBuilder<Visitor> builder)
        {
            //
        }
    }
}
