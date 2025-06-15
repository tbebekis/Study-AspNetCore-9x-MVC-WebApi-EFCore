namespace StudyLib
{
    static public class TokenHelper
    {
        public const string CachePrefix_Jti = "Jti";
        public const string CachePrefix_RefreshToken = "RefreshToken";

        // ● miscs
        /// <summary>
        /// Reads and returns an HTTP header from <see cref="HttpRequest.Headers"/>
        /// </summary>
        static public string GetHttpHeader(this HttpRequest Request, string Key)
        {
            Key = Key.ToLowerInvariant();
            return Request == null ? string.Empty : Request.Headers.FirstOrDefault(x => x.Key.ToLowerInvariant() == Key).Value.FirstOrDefault();
        }

        /// <summary>
        /// Constructs and returns a Cache Key
        /// </summary>
        static public string GetJtiCacheKey(string JtiValue)
        {
            string Result = $"{CachePrefix_Jti}+{JtiValue}";
            return Result;
        }
        /// <summary>
        /// Constructs and returns a Cache Key
        /// <para>The specified <c>Id</c> value is the value of an <c>Id</c> database table field, where Clients are stored. </para>
        /// </summary>
        static public string GetRefreshTokenCachKey(string Id)
        {
            string Result = $"{CachePrefix_RefreshToken}+{Id}";
            return Result;
        }

        /// <summary>
        /// Returns the value of the <see cref="System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Locale"/> claim of a specified <see cref="JwtSecurityToken"/>.
        /// <para>The <c>Locale</c> claim value is the value of the requested <see cref="CultureInfo.Name"/>, e.g. <c>en-US.</c></para>
        /// </summary>
        static public string GetCultureCode(JwtSecurityToken Token)
        {
            return ClaimHelper.GetClaimValue(Token.Claims, System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Locale);
        }
        /// <summary>
        /// Returns the value of the <see cref="JwtRegisteredClaimNames.Sub"/> claim of a specified <see cref="JwtSecurityToken"/>.
        /// <para>The <c>Sub</c> claim value is the value of an <c>Id</c> database table field, where Clients are stored. </para>
        /// </summary>
        static public string GetApiClientId(JwtSecurityToken Token)
        {
            return ClaimHelper.GetClaimValue(Token.Claims, System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
        }

        /// <summary>
        /// Reads a token by reading the <see cref="HeaderNames.Authorization"/> header 
        /// from <see cref="HttpRequest.Headers"/>, 
        /// and converting the token string to a <see cref="JwtSecurityToken"/>
        /// <para>The JWT token handler of Asp.Net Core, by default, maps inbound claims using a certain logic.</para>
        /// <para>This default mapping happens when MapInboundClaims = true; which is the default.</para>
        /// <para>By default the JWT token handler, maps, for example, the JwtRegisteredClaimNames.Sub 
        /// to http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier claim.</para>
        /// <para>Setting <see cref="JwtBearerOptions.MapInboundClaims"/> to false disables that default claim mapping.</para>
        /// <para>Another way to check the inbound claims, as they are, i.e. without any mapping applied,
        /// is to read the Token string from HTTP Authorization header
        /// as the Tokens.ReadTokenFromRequestHeader() does.</para>
        /// <para>SEE: https://stackoverflow.com/a/68253821/1779320</para>
        /// <para>SEE: https://stackoverflow.com/a/62477483/1779320</para>
        /// </summary>
        static public JwtSecurityToken ReadTokenFromRequestHeader(HttpContext HttpContext)
        {
            string AuthorizationHeaderValue = GetHttpHeader(HttpContext.Request, HeaderNames.Authorization);

            if (!string.IsNullOrWhiteSpace(AuthorizationHeaderValue))
            {
                AuthorizationHeaderValue = AuthorizationHeaderValue.Trim();
                if (AuthorizationHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    AuthorizationHeaderValue = AuthorizationHeaderValue.Replace("Bearer ", string.Empty);
                    JwtSecurityToken Token = new JwtSecurityTokenHandler().ReadJwtToken(AuthorizationHeaderValue);
                    return Token;
                }
            }

            return null;
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
    }
}
