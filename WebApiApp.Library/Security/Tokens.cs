namespace WebApiApp.Library
{

    /// <summary>
    /// Helper for handling tokens
    /// </summary>
    static public class Tokens
    {
        /// <summary>
        /// Generates an Access Token based on a database table Id which identifies the client application and a requested locale.
        /// <para>The specified <c>Id</c> value is the value of an <c>Id</c> database table field, where Clients are stored. </para>
        /// </summary>
        static public ItemResult<TokenResult> CreateAccessToken(string Id, string CultureCode)
        {
            JwtSettings Jwt = Lib.Settings.Jwt;

            if (string.IsNullOrWhiteSpace(CultureCode))
                CultureCode = Lib.Settings.Defaults.CultureCode;

            // ● handle Jti claim
            /// NOTE: sub claim identifies the subject(client, user, account, etc.), and could be the value of ClientApp.Id primary key
            /// NOTE: jti claim identifies the token itself, preventing replay attacks, and should be a value other than the sub claim value
            /// 
            /// ● A jti claim is used to prevent replay attacks
            /// STEPS:  
            /// • add a jti claim when the Access Token is created
            /// • save jti claim value to a cache or database
            /// • on every request check the jti claim. This app uses ApiClientJwtBearerEvents.TokenValidated() method for that check
            /// • if the jti claim  is not included in the request's Access Token or it is not in the cache/database then the request should be rejected
            /// • when Access Token expires the jti claim should be deleted from cache/database
            string JtiValue = Lib.GenId();
            string JtiCacheKey = TokenHelper.GetJtiCacheKey(JtiValue);
            Lib.Cache.Set(JtiCacheKey, JtiValue, Jwt.TokenLifeTimeMinutes);

            // ● handle refresh token
            /// NOTE: A Refresh token is created when an Access Token is created and both tokens are sent to the client
            /// NOTE: A Refresh token should have a much longer LifeTime than the Access Token
            /// 
            ///  ● A Refresh token, when it is not expired, it is used in refreshing an expired Access Token
            ///  STEPS:
            ///  • in route such as /authenticate validate client credentials and if they are invalid then the request should be rejected
            ///  • if client credentials are valid then an Access token is created, along with a Refresh Token, and both tokens are sent to the client
            ///  • the newly created Refresh Token is stored in a cache/database
            ///  • when the Access Token expires the client calls /refresh passing both the expired Access Token and the not-yet-expired Refresh Token
            ///  • if any of the tokens is not present then the request should be rejected
            ///  • if the passed-in Refresh Token is different from the Refresh Token stored in a cache/database then the request should be rejected
            ///  • if the Refresh Token is expired then the request should be rejected
            ///  • if all is ok then the application issues a new pair of Access and Refresh tokens and both tokens are sent to the client
            string RefreshToken = CreateRefreshToken();
            string RefreshTokenCacheKey = TokenHelper.GetRefreshTokenCachKey(Id);
            Lib.Cache.Set(RefreshTokenCacheKey, RefreshToken, Jwt.RefreshTokenLifeTimeMinutes);

            // ● Claims
            /// NOTE: sub claim identifies the subject(client, user, account, etc.), and could be the value of ClientApp.Id primary key
            /// NOTE: jti claim identifies the token itself, preventing replay attacks, and should be a value other than the sub claim value
            /// 
            /// SEE: List of registered claims from different sources
            /// https://datatracker.ietf.org/doc/html/rfc7519#section-4
            /// http://openid.net/specs/openid-connect-core-1_0.html#IDToken
            /// https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
            List<Claim> ClaimList = new List<Claim>();
            ClaimList.Add(new Claim(JwtRegisteredClaimNames.Sub, Id));          // sub identifies the subject(client, user, account, etc.)
            ClaimList.Add(new Claim(JwtRegisteredClaimNames.Jti, JtiValue));    // jti identifies the token itself, preventing replay attacks
            ClaimList.Add(new Claim(JwtRegisteredClaimNames.Locale, CultureCode));

            // ● JWT token
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.EncryptionKey));
            var SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var ExpirationDateTime = DateTime.UtcNow.AddMinutes(Jwt.TokenLifeTimeMinutes);

            JwtSecurityToken JwtToken = new JwtSecurityToken(
                issuer: Jwt.Issuer,
                audience: Jwt.Audience,
                claims: ClaimList.ToArray(),
                expires: ExpirationDateTime,                                // exp claim, indicates the latest time at which the token can be used
                notBefore: DateTime.UtcNow,                                 // nbf claim, a timestamp before which the token is not valid
                signingCredentials: SigningCredentials                      // used by the .Net to sign the JwtSecurityToken
            );

            // ● Token
            TokenResult TokenResult = new TokenResult();
            TokenResult.Token = new JwtSecurityTokenHandler().WriteToken(JwtToken);
            TokenResult.ExpiresOn = JwtToken.ValidTo; //JwtToken.ValidTo.ToString("yyyy-MM-dd HH:mm");
            TokenResult.LifeTimeMinutes = Jwt.TokenLifeTimeMinutes;
            TokenResult.RefreshToken = RefreshToken;
            TokenResult.RefreshTokenExpiresOn = DateTime.UtcNow.AddMinutes(Jwt.RefreshTokenLifeTimeMinutes);
            TokenResult.RefreshTokenLifeTimeMinutes = Jwt.RefreshTokenLifeTimeMinutes;

            // ● Response
            ItemResult<TokenResult> Result = new();
            Result.Item = TokenResult;

            return Result;
        }

        /// <summary>
        /// Generates and returns a refresh token using <see cref="RandomNumberGenerator"/>
        /// </summary>
        static public string CreateRefreshToken()
        {
            byte[] Buffer = new byte[32];
            using (var RNG = RandomNumberGenerator.Create())
            {
                RNG.GetBytes(Buffer);
                return Convert.ToBase64String(Buffer);
            }
        }

        /// <summary>
        /// Returns the <see cref="ClaimsPrincipal"/> out of an Access Token text
        /// </summary>
        static public PrincipalToken GetPrincipalFromExpiredToken(string ExpiredTokenText)
        {
            JwtSettings Jwt = Lib.Settings.Jwt;
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.EncryptionKey));

            var ValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,               // no audience validation
                ValidateIssuer = false,                 // no issuer validation
                ValidateIssuerSigningKey = true,        // signing key validation
                IssuerSigningKey = SecurityKey,
                ValidateLifetime = false                // no expiration date validation
            };

            JwtSecurityTokenHandler TokenHandler = new();

            SecurityToken SecurityToken;
            ClaimsPrincipal Principal = TokenHandler.ValidateToken(ExpiredTokenText, ValidationParameters, out SecurityToken);
            JwtSecurityToken JwtSecurityToken = SecurityToken as JwtSecurityToken;

            if (JwtSecurityToken == null || !JwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return new PrincipalToken() { Principal = Principal, JwtToken = JwtSecurityToken };
        }
    }
}
