namespace StudyLib
{
    /// <summary>
    /// Indicates the allowable CRUD operations in a database table
    /// </summary>
    [Flags]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CRUDMode
    {
        /// <summary>
        /// Nothin allowed.
        /// <para>Alias: N</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// Allow INSERTs
        /// <para>Alias: I</para>
        /// </summary>
        Insert = 1,
        /// <summary>
        /// Allow UPDATEs
        /// <para>Alias: U</para>
        /// </summary>
        Update = 2,
        /// <summary>
        /// Allow DELETEs
        /// <para>Alias: D</para>
        /// </summary>
        Delete = 4,
        /// <summary>
        /// Allow GetById
        /// <para>Alias: G</para>
        /// </summary>
        GetById = 8,
        /// <summary>
        /// Allow GetAll
        /// <para>Alias: A</para>
        /// </summary>
        GetAll = 0x10,
        /// <summary>
        /// Allow GetByFilter
        /// <para>Alias: F</para>
        /// </summary>
        GetByFilter = 0x20,

        All = CRUDMode.Insert
            | CRUDMode.Update
            | CRUDMode.Delete
            | CRUDMode.GetById
            | CRUDMode.GetAll
            | CRUDMode.GetByFilter
    }
}
