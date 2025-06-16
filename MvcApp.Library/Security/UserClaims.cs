namespace MvcApp.Library
{
    static public class UserClaims
    {
        // ● constants
        /// <summary>
        /// A claim type for a private claim. 
        /// Designates the level of a user, i.e. Admin, User, Guest, Service, etc.
        /// </summary>
        public const string SUserLevelClaimType = "UserLevel";
        /// <summary>
        /// A claim type for a private claim.
        /// Designates the scheme used in authentication, i.e. Cookies, JWT, etc.
        /// </summary>
        public const string SAuthenticationSchemeClaimType = "AuthenticationScheme";
        /// <summary>
        /// A claim type for a private claim. 
        /// When true the user is impersonating another user
        /// </summary>
        public const string SIsImpersonationClaimType = "IsImpersonation";


        // ● get values from user claims
        /// <summary>
        /// Returns the user id from claims.
        /// <para>This is a database table Id.</para>
        /// </summary>
        static public string GetUserId(IEnumerable<Claim> Claims)
        {
            return ClaimHelper.GetClaimValue<string>(Claims, ClaimTypes.NameIdentifier);
        }
        /// <summary>
        /// Returns true if the requestor is impersonating another user
        /// </summary>
        static public bool GetIsUserImpersonation(IEnumerable<Claim> Claims)
        {
            return ClaimHelper.GetClaimValue<bool>(Claims, SIsImpersonationClaimType);
        }
        /// <summary>
        /// Returns the authentication scheme of the requestor
        /// </summary>
        static public string GetUserAuthenticationScheme(IEnumerable<Claim> Claims)
        {
            return ClaimHelper.GetClaimValue<string>(Claims, SAuthenticationSchemeClaimType);
        }

        /// <summary>
        /// Generates and returns a claim list regarding a <see cref="IAppUser"/>.
        /// <para>Claims are stored in the user cookie.</para>
        /// </summary>
        static public List<Claim> GenerateUserClaimList(IAppUser User, string AuthenticationScheme, bool IsImpersonation = false)
        {
            if (string.IsNullOrWhiteSpace(User.Id))
                throw new ApplicationException("Cannot produce claims. No Id");

            if (string.IsNullOrWhiteSpace(User.UserName))
                throw new ApplicationException("Cannot produce claims. No UserName");

            List<Claim> ClaimList = new List<Claim>();

            ClaimList.Add(new Claim(ClaimTypes.NameIdentifier, User.Id));
            ClaimList.Add(new Claim(ClaimTypes.Name, !string.IsNullOrWhiteSpace(User.Name) ? User.Name : "no name"));
 

            // private claims
            ClaimList.Add(new Claim(UserClaims.SAuthenticationSchemeClaimType, AuthenticationScheme));
            ClaimList.Add(new Claim(UserClaims.SIsImpersonationClaimType, IsImpersonation.ToString()));

            return ClaimList;
        }
        /// <summary>
        /// Creates and returns a <see cref="ClaimsPrincipal"/> along with a claim list for a specified <see cref="IAppUser"/>.
        /// </summary>
        static public ClaimsPrincipal CreateUserPrincipal(IAppUser User, string AuthenticationScheme, bool IsImpersonation = false)
        {
            // create claim list
            List<Claim> ClaimList = GenerateUserClaimList(User, AuthenticationScheme, IsImpersonation);

            // identity and principal
            // NOTE: setting the second parameter actually authenticates the identity (IsAuthenticated returns true)
            ClaimsIdentity Identity = new ClaimsIdentity(ClaimList, AuthenticationScheme);
            ClaimsPrincipal Principal = new ClaimsPrincipal(Identity);

            return Principal;
        }
    }
}
