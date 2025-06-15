namespace StudyLib.Entities
{
    [Description("A user in an MVC application.")]
    public interface IAppUser
    {
        /// <summary>
        /// Required. 
        /// <para><strong>Unique.</strong></para>
        /// <para>Database Id or something similar.</para>
        /// </summary>
        [Description("Primary Key. Unique. Id in the database table.")]
        string Id { get; set; }
        /// <summary>
        /// Required. 
        /// <para><strong>Unique.</strong></para>
        /// </summary> 
        [Description("The ClientId, admin generated"), Required]
        string UserName { get; set; }
        /// <summary>
        /// Optional. The requestor name
        /// </summary> 
        [Description("The client application name, admin generated")]
        string Name { get; set; }
        /// <summary>
        /// True when this identity is blocked by admins
        /// </summary>
        [Description("True when identity is blocked by admins")]
        public bool IsBlocked { get; set; }
    }
}
