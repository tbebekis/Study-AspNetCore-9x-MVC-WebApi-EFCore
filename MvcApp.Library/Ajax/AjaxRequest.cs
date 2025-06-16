namespace MvcApp.Library
{
    /// <summary>
    /// An AJAX request.
    /// <para>An AJAX request could be either a Ui or a Proc request. </para>
    /// <para>A Ui request may set the an <see cref="IsSingleInstance"/> flag indicating that the Ui may exist only once. </para>
    /// <para>A Proc request may or may not return a packet. </para>
    /// <para>A requester may optionally set the <see cref="CommandId"/> and/or <see cref="CommandName"/> properties. </para>
    /// <para></para>
    /// <para>This class serves a registry for <see cref="IAjaxRequestHandler"/> objects.</para>
    /// <para>A developer may choose instead of using <see cref="AjaxRequest"/> handler methods in the <see cref="ControllerMvcAjax"/> ajax controller, 
    /// to implement his own <see cref="IAjaxRequestHandler"/> objects as request handlers
    /// and have the </para>
    /// <para></para>
    /// </summary>
    public class AjaxRequest
    {
        static readonly System.Threading.Lock syncLock = new();
        static List<IAjaxRequestHandler> Handlers = new List<IAjaxRequestHandler>();


        /// <summary>
        /// Constructor
        /// </summary>
        public AjaxRequest()
        {
        }

        /* static */
        /// <summary>
        /// Registers a handler.
        /// </summary>
        static public void Register(IAjaxRequestHandler Handler)
        {
            lock (syncLock)
                Handlers.Insert(0, Handler);
        }
        /// <summary>
        /// Unregisters a handler.
        /// </summary>
        static public void UnRegister(IAjaxRequestHandler Handler)
        {
            lock (syncLock)
                Handlers.Remove(Handler);
        }

        /// <summary>
        /// Iterates through its registered handlers, finds the right one and processes a specified <see cref="AjaxRequest"/>.
        /// Returns an <see cref="AjaxRequestResult"/> on success. Else returns null.
        /// </summary>
        static public AjaxRequestResult Process(AjaxRequest Request)
        {
            lock (syncLock)
            {
                AjaxRequestResult Result = null;

                foreach (IAjaxRequestHandler Handler in Handlers)
                {
                    Result = Handler.Process(Request);
                    if (Result != null)
                        return Result;
                }

                return Result;
            }
        }

        /* public */
        /// <summary>
        /// Returns true when Params contains a specified key.
        /// </summary>
        public bool ParamsContainsKey(string Key)
        {
            return !string.IsNullOrWhiteSpace(Key) && Params != null && Params.ContainsKey(Key) && Params[Key] != null;
        }
        /// <summary>
        /// Returns a param under a specified key, if any, else null.
        /// </summary>
        public object GetParam(string Key)
        {
            return ParamsContainsKey(Key) ? Params[Key] : null;
        }

        /// <summary>
        /// Converts the <see cref="Params"/> dictionary to a packet object, i.e. an incoming Model.
        /// </summary>
        public T ParamsToPacket<T>()
        {
            string JsonText = Json.Serialize(this.Params);
            T Result = Json.Deserialize<T>(JsonText);
            return Result;
        }

        /* properties */
        /// <summary>
        /// Optional. The id of the request.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Required. The name of the operation to execute
        /// </summary>
        public string OperationName { get; set; }
        /// <summary>
        /// Optional. Any parameteres coming along with the request.
        /// </summary>
        public Dictionary<string, object> Params { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// The request type. Ui or Proc.
        /// A Ui request returns HTML.
        /// A Proc request may or may not return a packet.
        /// </summary>
        public AjaxRequestType Type { get; set; }
        /// <summary>
        /// True when this is a single instance Ui request.
        /// </summary>
        public bool IsSingleInstance { get; set; }

        /// <summary>
        /// A requester may optionally set the <see cref="CommandId"/> and/or <see cref="CommandName"/> properties.
        /// </summary>
        public string CommandId { get; set; }
        /// <summary>
        /// A requester may optionally set the <see cref="CommandId"/> and/or <see cref="CommandName"/> properties.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// The <see cref="IViewToStringConverter"/> to use when processing a Ui request by converting a razor view to HTML.
        /// <para>The <see cref="ControllerMvcAjax"/> class provides a default implementation, and assigns itself to this property.</para>
        /// </summary>
        [JsonIgnore]
        public IViewToStringConverter ViewToStringConverter { get; set; }
    }
}
