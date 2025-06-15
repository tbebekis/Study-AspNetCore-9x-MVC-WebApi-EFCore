namespace WebApiApp.Library
{

    /// <summary>
    /// Client Access Token request
    /// </summary>
    [Description("Client Access Token request.")]
    public class TokenRequest
    {
        string fLocale;

        /// <summary>
        /// The ClientId
        /// </summary>
        [Description("The ClientId")]
        public string ClientId { get; set; }
        /// <summary>
        /// The client secret
        /// </summary>
        [Description("The secret in plain text.")]
        public string Secret { get; set; }
        /// <summary>
        /// The locale code of the request.
        /// <para>For example <c>en-US</c></para>
        /// <para>SEE: https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims</para>
        /// </summary>
        [Description("The locale code of the request, e.g. en-US, or null.")]
        [DefaultValue(null)]
        public string Locale
        {
            get => !string.IsNullOrWhiteSpace(fLocale) ? fLocale : Lib.Settings.Defaults.CultureCode;
            set => fLocale = value;
        }
    }
}
