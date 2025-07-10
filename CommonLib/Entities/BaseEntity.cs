namespace CommonLib.Entities
{

    /// <summary>
    /// <para><strong>Blob Types</strong></para>
    /// <code>
    /// ---------------------------------------------------------------------------------
    ///  Server         Blob                                    Text Blob
    /// ---------------------------------------------------------------------------------
    ///   MsSql         image                                   nvarchar(max)
    ///   FirebirdSql   BLOB SUB_TYPE 0 SEGMENT SIZE 80         BLOB SUB_TYPE TEXT SEGMENT SIZE 80
    ///   MySql         LONGBLOB                                LONGTEXT  CHARACTER SET UTF8
    ///   Oracle        blob                                    nclob
    ///   PostgreSql    bytea                                   text
    ///   Sqlite        blob                                    text 
    /// </code>
    /// </summary>
    [PrimaryKey(nameof(Id))]
    public class BaseEntity 
    {
        public const string SDefaultId = "00000000-0000-0000-0000-000000000000";

        public BaseEntity()
        {            
        }

        // ● miscs 
        /// <summary>
        /// Creates and returns a new Guid.
        /// <para>If UseBrackets is true, the new guid is surrounded by {}</para>
        /// </summary>
        static public string GenId(bool UseBrackets)
        {
            string format = UseBrackets ? "B" : "D";
            return Guid.NewGuid().ToString(format).ToUpper();
        }
        /// <summary>
        /// Creates and returns a new Guid WITHOUT surrounding brackets, i.e. {}
        /// </summary>
        static public string GenId()
        {
            return GenId(false);
        }

        // ● miscs 
        /// <summary>
        /// Generates and sets the <see cref="Id"/> property.
        /// </summary>
        public virtual void SetId()
        {
            this.Id = GenId();
        }
        /// <summary>
        /// Returns true when this is a new instance.
        /// </summary>
        public virtual bool IsNew()
        {
            return string.IsNullOrWhiteSpace(this.Id) || this.Id == SDefaultId;
        }

        // ● properties
        [Key, MaxLength(40), DefaultValue(null), JsonPropertyOrder(-1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
    }
}
