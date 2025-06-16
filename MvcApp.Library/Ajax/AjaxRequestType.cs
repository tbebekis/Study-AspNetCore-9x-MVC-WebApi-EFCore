namespace MvcApp.Library
{

    /// <summary>
    /// The request type.  Ui or Proc.  
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AjaxRequestType
    {
        /// <summary>
        /// Request the execution of a procedure. 
        /// </summary>
        Proc = 1,
        /// <summary>
        /// Requests a Ui. 
        /// </summary>
        Ui = 0,              
    }
}
