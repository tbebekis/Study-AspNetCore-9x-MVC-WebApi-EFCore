namespace MvcApp.Library
{
    /// <summary>
    /// The response following an <see cref="AjaxRequest"/>
    /// <para>A <see cref="IAjaxRequestHandler"/> handles the <see cref="AjaxRequest"/> 
    /// and adds entries in the <see cref="Properties"/> dictionary of this object, using the indexer property <c>this</c></para>
    /// </summary>
    public class AjaxRequestResult
    {
        Dictionary<string, object> Properties = new Dictionary<string, object>();

        /* construction */
        /// <summary>
        /// Constructor
        /// </summary>
        public AjaxRequestResult()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public AjaxRequestResult(string OperationName)
        {
            this.OperationName = OperationName;
        }

        /* public */
        /// <summary>
        /// Returns the packet object.
        /// <para>The packet object goes to <see cref="HttpActionResult.Packet"/> property and it is the actual information that is returned to the caller.</para>
        /// </summary>
        public object GetPacketObject()
        {
            JsonObject Result = new();

            if (!string.IsNullOrWhiteSpace(OperationName))
                Result["OperationName"] = OperationName;
 
            string JsonText;
            if (Properties != null && Properties.Count > 0)
            {
                foreach (var Entry in Properties)
                {
                    JsonText = Json.Serialize(Entry.Value);
                    Result[Entry.Key] = JsonNode.Parse(JsonText);
                }
            }

            return Result;
        }
        /// <summary>
        /// Returns true if the internal properties dictionary contains a specified key.
        /// </summary>
        public bool ContainsKey(string Key)
        {
            return Properties.ContainsKey(Key);
        }

        /* properties */
        /// <summary>
        /// Optional. The name of the request/response operation, if any, else null.
        /// </summary>
        public string OperationName { get; set; }
        /// <summary>
        /// Indexer. Get or sets the value of an entry in internal properties dictionary.
        /// <para>If the specified key not exists, returns null.</para>
        /// </summary>
        public object this[string Key]
        {
            get { return Properties.ContainsKey(Key) ? Properties[Key] : null; }
            set { Properties[Key] = value; }
        }
        /// <summary>
        /// True when the call succeeds business-logic-wise.
        /// </summary>
        public bool IsSuccess { get; set; } = false;
    }
}
