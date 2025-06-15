namespace WebApiApp.Library
{
    /// <summary>
    /// Api Token Data
    /// </summary>
    [Description("The ApiToken object.")]
    public class TokenResult
    {
        /// <summary>
        /// The access token value.
        /// </summary>
        [Description("The token string.")]
        public string Token { get; set; }
        /// <summary>
        /// UTC Expiration date and time.
        /// </summary>
        [Description("UTC Expiration date and time.")]
        public DateTime ExpiresOn { get; set; }
        /// <summary>
        /// Token LifeTime in minutes
        /// </summary>
        [Description("Token LifeTime in minutes.")]
        public int LifeTimeMinutes { get; set; }
        /// <summary>
        /// The refresh token value.
        /// </summary>
        [Description("The refresh token string.")]
        public string RefreshToken { get; set; }
        /// <summary>
        /// UTC Expiration date and time.
        /// </summary>
        [Description("Refresh token UTC Expiration date and time.")]
        public DateTime RefreshTokenExpiresOn { get; set; }
        /// <summary>
        /// Token LifeTime in minutes
        /// </summary>
        [Description("Refresh Token LifeTime in minutes.")]
        public int RefreshTokenLifeTimeMinutes { get; set; }


    }
}
