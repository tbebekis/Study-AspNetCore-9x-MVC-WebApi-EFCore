namespace StudyLib.Entities
{
    [PrimaryKey(nameof(Id))]
    public class BaseEntity 
    {
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


        // ● properties
        [Key, MaxLength(40), DefaultValue(null), JsonPropertyOrder(-1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
    }
}
