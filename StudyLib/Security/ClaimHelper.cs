namespace StudyLib
{

    /// <summary>
    /// Helper for handling claims
    /// </summary>
    static public class ClaimHelper
    {
        // ● Claims
        /// <summary>
        /// Returns true if a claim exists in a sequence of claims
        /// </summary>
        static public bool ContainsClaim(IEnumerable<Claim> Claims, string TokenType)
        {
            return FindClaim(Claims, TokenType) != null;
        }
        /// <summary>
        /// Finds and returns a claim, if a claim exists in a sequence of claims, else null.
        /// </summary>
        static public Claim FindClaim(IEnumerable<Claim> Claims, string TokenType)
        {
            TokenType = TokenType.ToLowerInvariant();
            return Claims.FirstOrDefault(item => item.Type.ToLowerInvariant() == TokenType);
        }

        /// <summary>
        /// Returns the value of a claim, if a claim exists in a sequence of claims, else null.
        /// </summary>
        static public string GetClaimValue(IEnumerable<Claim> Claims, string TokenType)
        {
            Claim Claim = FindClaim(Claims, TokenType);
            return Claim != null ? Claim.Value : null;
        }
        /// <summary>
        /// Returns the value of a claim, if any, or a default value.
        /// </summary>
        static public T GetClaimValue<T>(IEnumerable<Claim> Claims, string ClaimType, T DefaultValue = default(T))
        {
            Claim Claim = Claims.Where(c => c.Type == ClaimType).FirstOrDefault();
            return GetClaimValue(Claim, DefaultValue);
        }
        /// <summary>
        /// Returns the value of a claim, if not null, else returns a default value.
        /// </summary>
        static public T GetClaimValue<T>(Claim Claim, T DefaultValue = default(T))
        {
            return Claim != null ? (T)Convert.ChangeType(Claim.Value, typeof(T)) : DefaultValue;
        }

        /// <summary>
        /// Returns the list of claims of a specified token.
        /// <para>NOTE: The token class type passed to JwtBearerEvents events changed in Asp.Net 8.0 
        /// from JwtSecurityToken to JsonWebToken class</para>
        /// <para>SEE: https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/securitytoken-events</para>
        /// </summary>
        static public List<Claim> GetClaimList(SecurityToken SecToken)
        {
            if (SecToken == null)
                throw new ApplicationException($"Cannot get claim list. The specified parameter {nameof(SecToken)} is null");

            JwtSecurityToken JwtToken = SecToken as JwtSecurityToken;
            if (JwtToken != null)
                return JwtToken.Claims.ToList();

            /// NOTE: The token class type passed to JwtBearerEvents events changed in Asp.Net 8.0 
            /// from JwtSecurityToken to JsonWebToken class
            /// SEE: https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/securitytoken-events
            JsonWebToken WebToken = SecToken as JsonWebToken;
            if (WebToken != null)
                return WebToken.Claims.ToList();

            return new List<Claim>();

        }
    }
}
