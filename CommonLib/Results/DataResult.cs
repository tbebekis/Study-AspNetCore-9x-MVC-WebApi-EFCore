namespace CommonLib
{

    /// <summary>
    /// Base response
    /// </summary>
    [Description("Base data result.")]
    public class DataResult
    {
        protected const string SCheckErrorList = "Check the Error list";
        List<string> fErrors;

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public DataResult()
        {
        }

        // ● public
        /// <summary>
        /// Adds an error to the error list
        /// </summary>
        /// <param name="ErrorText"></param>
        public virtual void AddError(string ErrorText)
        {
            if (!string.IsNullOrWhiteSpace(ErrorText))
            {
                if (fErrors == null)
                    fErrors = new List<string>();

                if (!fErrors.Contains(ErrorText))
                    fErrors.Add(ErrorText);

                Message = SCheckErrorList;
            }
        }
        /// <summary>
        /// Clears the error list
        /// </summary>
        public virtual void ClearErrors()
        {
            if (fErrors != null)
                fErrors.Clear();
        }
        /// <summary>
        /// Copies errors and status from a specified source
        /// </summary>
        public virtual void CopyErrors(DataResult Source)
        {
            this.Status = Source.Status;
            foreach (var error in Source.Errors) 
                this.AddError(error);
        }
        /// <summary>
        /// Sets the result state of this instance along with message and error message.
        /// </summary>
        public virtual void SetResult(int Status = -1, string ErrorMessage = null, string Message = null)
        {
            if (Status >= StatusCodes.Status100Continue)
                this.Status = Status;

            if (!string.IsNullOrWhiteSpace(ErrorMessage))
                Errors.Add(ErrorMessage); 

            if (!string.IsNullOrWhiteSpace(Message))
               this.Message = Message;
        }

        // ● error results
        /// <summary>
        /// Error result
        /// </summary>
        public virtual void ErrorResult(int Status, string ErrorMessage = null)
        {
            if (!string.IsNullOrWhiteSpace(ErrorMessage))
                ErrorMessage = $"Error: {ErrorMessage}";
            else if (Status >= 400 && Status <= 599)
                ErrorMessage = Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(Status);
            else if (Status >= 1000)
                ErrorMessage = ApiStatusCodes.StatusCodeToMessage[Status];
            else
                ErrorMessage = $"Error: {Status}";

            SetResult(Status, ErrorMessage, SCheckErrorList);
        }
        /// <summary>
        /// Error result
        /// </summary>
        public virtual void NotAuthenticated()
        {
            string ErrorMessage = "Not Authenticated. Invalid Token or not Token at all. A valid Access Token is required.";
            ErrorResult(StatusCodes.Status401Unauthorized, ErrorMessage);
        }
        /// <summary>
        /// Error result
        /// </summary>
        public virtual void TokenExpired(string ErrorMessage)
        {
            ErrorMessage = !string.IsNullOrWhiteSpace(ErrorMessage) ? ErrorMessage : $"The Access Token is expired";
            ErrorResult(StatusCodes.Status401Unauthorized, ErrorMessage);
        }
        /// <summary>
        /// Error result
        /// </summary>
        public virtual void ExceptionResult(Exception Ex)
        {
            string ErrorMessage = Ex == null? "Unspecified Exception": $"Exception: {Ex.GetType().Name}";
            ErrorResult(ApiStatusCodes.Exception, ErrorMessage);

            if (Ex != null)
            {
                List<string> ExceptionErrorList = Ex.GetErrors();
                Errors.AddRange(ExceptionErrorList);
            }
 
        }
        /// <summary>
        /// Error result
        /// </summary>
        public virtual void BadRequest(string ErrorMessage = null)
        {
            ErrorMessage = !string.IsNullOrWhiteSpace(ErrorMessage) ? $"Bad request: {ErrorMessage}" : "Bad request";
            ErrorResult(StatusCodes.Status400BadRequest, ErrorMessage);
        }
        /// <summary>
        /// Error result
        /// </summary>
        public virtual void NoDataResult(string Message = null)
        {
            Message = !string.IsNullOrWhiteSpace(Message) ? $"No data: {Message}" : "No data.";
            SetResult(Status: ApiStatusCodes.NoData, Message: Message);
        }

        // ● properties
        /// <summary>
        /// True if there are no errors
        /// </summary>
        [JsonIgnore]
        public bool Succeeded => fErrors == null || fErrors.Count == 0;
        /// <summary>
        /// Returns the text of all errors.
        /// </summary>
        [JsonIgnore]
        public string ErrorText => Succeeded ? string.Empty : string.Join(Environment.NewLine, Errors.ToArray());
 
        [Description("A Status numeric code. Could be a standard HTTP Status Code or an Api Status Code.")]
        [DefaultValue(StatusCodes.Status200OK), JsonPropertyOrder(-1002)]
        public int Status { get; set; } = StatusCodes.Status200OK;
        [Description("A response message. Defaults to OK.")]
        [DefaultValue("OK"), JsonPropertyOrder(-1001)]
        public string Message { get; set; } = "OK";
        [Description("The list of errors, if any."), JsonPropertyOrder(-1000)]
        public List<string> Errors
        {
            get
            {
                if (fErrors == null)
                    fErrors = new List<string>();
                return fErrors;
            }
            set { fErrors = value; }
        }
    }
}
