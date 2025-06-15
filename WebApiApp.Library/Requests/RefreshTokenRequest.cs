namespace WebApiApp.Library
{
    [Description("Client Refresh Token request.")]
    public class RefreshTokenRequest
    {
        /// <summary>
        /// The access token value.
        /// </summary>
        [Description("The token string. Could be expired or not expired.")]
        public string Token { get; set; }
        /// <summary>
        /// The refresh token value.
        /// </summary>
        [Description("The refresh token string.")]
        public string RefreshToken { get; set; }
    }
}
