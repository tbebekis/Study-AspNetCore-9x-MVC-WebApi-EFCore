namespace CommonLib
{
    /// <summary>
    /// Indicates the CRUD operations allowed to an Entity.
    /// <para>NOTE: Setting just the attribute means all CRUD modes are allowed.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CRUDModeAttribute: Attribute
    {

        /// <summary>
        /// A mapping to CRUDMode values.
        /// </summary>
        static public Dictionary<char, CRUDMode> ModeDic = new Dictionary<char, CRUDMode>() {
            { 'N', CRUDMode.None },
            { 'I', CRUDMode.Insert },
            { 'U', CRUDMode.Update },
            { 'D', CRUDMode.Delete },
            { 'G', CRUDMode.GetById },
            { 'A', CRUDMode.GetAll },
            { 'F', CRUDMode.GetByFilter },
        };
        /// <summary>
        /// Converts a string Mode (e.g. "IUDG") to a CRUDMode value.
        /// </summary>
        static CRUDMode StrToMode(string Modes)
        {
            CRUDMode Result = CRUDMode.None;

            if (!string.IsNullOrWhiteSpace(Modes))
            {
                char C;
                foreach (char c in Modes)
                {
                    C = char.ToUpper(c);
                    if (ModeDic.ContainsKey(C))
                    {
                        Result |= ModeDic[C];
                    }
                }
            }

            return Result;
        }

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public CRUDModeAttribute()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public CRUDModeAttribute(string Modes)
        {
            this.Modes = StrToMode(Modes);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public CRUDModeAttribute(CRUDMode Modes)
        {
            this.Modes = Modes;
        }

        /// <summary>
        /// A bit-field indicating the CRUD operations allowed to an Entity
        /// </summary>
        public CRUDMode Modes { get; set; } = CRUDMode.All;
    }
}
