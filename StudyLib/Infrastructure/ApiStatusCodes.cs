namespace StudyLib
{
    /// <summary>
    /// API Status Codes.
    /// <para>Standard HTTP Status Codes</para>
    /// <list type="bullet">
    /// <item>100 – 199: Informational responses</item>
    /// <item>200 – 299: Successful responses   </item>
    /// <item>300 – 399: Redirection messages   </item>
    /// <item>400 – 499: Client error responses </item>
    /// <item>500 – 599: Server error responses </item>
    /// </list>
    /// <para>SEE: https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Status</para>
    /// <para>SEE: https://www.rfc-editor.org/rfc/rfc2616#section-6.1.1 </para>
    /// </summary>
    static public class ApiStatusCodes
    {
        // ● miscs
        public const int UnknownError = 1000;
        public const int Exception = 1001;
        public const int NoData = 1002;
        public const int LocaleNotSupported = 1003;

        // ● authentication
        public const int InvalidIdentity = 1101;
        public const int InvalidCredentials = 1102;
        public const int CredentialsRequired = 1103;       
        public const int TokenExpired = 1104;
        public const int RefreshTokenExpired = 1105;
        public const int TokenAndRefreshTokenRequired = 1106;
        public const int NoIdentityInfoInToken = 1107;

        // ● entities
        public const int EntityNotFound = 1200;
        public const int EntityIdNotSpecified = 1201;

        static public Dictionary<int, string> StatusCodeToMessage = new()
        {
            // ● miscs
            { UnknownError, "Unknown Error" },
            { Exception, "Exception Error" },
            { NoData, "No Data" },
            { LocaleNotSupported, "Requested Locale Not Supported" },

             // ● authentication
            { InvalidIdentity, "Invalid Client or User" },
            { InvalidCredentials, "Invalid Credentials" },
            { CredentialsRequired, "Credentials Required" },
            
            { TokenExpired, "Token has expired or is invalid. Please authenticate again." },
            { RefreshTokenExpired, "Refresh Token has expired or is invalid. Please authenticate again." },
            { TokenAndRefreshTokenRequired, "Request is not valid. Token and RefreshToken required." },
            { NoIdentityInfoInToken, "No Identity information in Token." },

            // ● entities
            { EntityNotFound, "Entity not found." },
            { EntityIdNotSpecified, "Entity Id not specified." },
        };

 
    }


}
