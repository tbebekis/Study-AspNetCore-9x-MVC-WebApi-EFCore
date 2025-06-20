namespace CommonLib.Entities
{
    [Table("LogEntry")]
    public class LogEntryEntity : BaseEntity
    {
        public LogEntryEntity() { }
        public LogEntryEntity(LogEntry Source) 
        { 
            this.Id = Source.Id;
            this.UserName = Source.UserName;
            this.HostName = Source.HostName;
            this.EntryTimeUtc = Source.TimeStampUtc;
            this.Category = Source.Category;
            this.Level = Source.Level;
            this.Scope = Source.GetScopeText();
            this.EventId = Source.EventId.Id;
            this.ExceptionName = Source.Exception != null? Source.Exception.GetType().Name: string.Empty;
            this.ExceptionData = Source.ExceptionData;
            this.LogText = Source.Text;
            this.StateText = !string.IsNullOrWhiteSpace(Source.StateText)? Source.StateText: Source.GetPropertiesAsTextList(); 
        }


        /// <summary>
        /// User name of the person who is currently logged on to operating system.
        /// </summary>
        [MaxLength(32)]
        public string UserName { get; set; }
        /// <summary>
        /// Host name of local computer
        /// </summary>
        [MaxLength(32)]
        public string HostName { get; set; }
        /// <summary>
        /// Date and time, in UTC, of the creation time of this instance
        /// </summary>
        public DateTime EntryTimeUtc { get; set; }
        /// <summary>
        /// Category this instance belongs to.
        /// <para>The category is usually the fully qualified class name of a class asking for a logger, e.g. MyNamespace.MyClass </para>
        /// </summary>
        [MaxLength(96)]
        public string Category { get; set; }
        /// <summary>
        /// The log level of this information.
        /// </summary>
        public LogLevel Level { get; set; }
        /// <summary>
        /// The scopes currently in use, if any. The last scope is the current one.
        /// </summary>
        [MaxLength(96)]
        public string Scope { get; set; }
        /// <summary>
        /// The EventId of this information. 
        /// <para>An EventId with Id set to zero, usually means no EventId.</para>
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// The exception this information represents, if any, else null.
        /// </summary>
        [MaxLength(96)]
        public string ExceptionName { get; set; }
        /// <summary>
        /// The exception data, if this is a log regarding an exception
        /// </summary>
        [Column(TypeName = "text")]
        public string ExceptionData { get; set; }
        /// <summary>
        /// The message of this information
        /// </summary>
        [Column(TypeName = "text")]
        public string LogText { get; set; }
        /// <summary>
        /// A dictionary with State properties converted to string.
        /// <para>When the log message is a message template with format values, e.g. <code>Logger.LogInformation("Customer {CustomerId} order {OrderId} is completed", CustomerId, OrderId)</code>  </para>
        /// this dictionary contains entries gathered from the message in order to ease any Structured Logging providers.
        /// </summary>
        [Column(TypeName = "text")]
        public string StateText { get; set; }      

    }


    public class LogEntryEntityTypeConfiguration : IEntityTypeConfiguration<LogEntryEntity>
    {
        public void Configure(EntityTypeBuilder<LogEntryEntity> builder)
        {
            //builder.Navigation(e => e.Category).AutoInclude();
        }
    }
}
