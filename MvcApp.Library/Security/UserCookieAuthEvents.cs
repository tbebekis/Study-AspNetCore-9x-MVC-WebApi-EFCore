namespace MvcApp.Library
{
    /// <summary>
    /// Allows subscribing to events raised during cookie authentication.
    /// <para>SEE: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie#react-to-back-end-changes</para>
    /// </summary>
    public class UserCookieAuthEvents : CookieAuthenticationEvents
    {
        ILogger Logger;

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public UserCookieAuthEvents(ILogger<UserCookieAuthEvents> Logger)
        {
            this.Logger = Logger;
        }

        // ● public overrides
        /// <summary>
        /// Invoked during sign in.
        /// </summary>
        public override Task SignedIn(CookieSignedInContext context)
        {
            string Id = ClaimHelper.GetClaimValue(context.Principal.Claims, ClaimTypes.NameIdentifier);  
            string UserName = ClaimHelper.GetClaimValue(context.Principal.Claims, ClaimTypes.Name);
            Logger.LogInformation("A user just signed-in. UserName: {UserName}, Id: {Id}", UserName, Id);
            return base.SignedIn(context);
        }
        /// <summary>
        /// Invoked on sign out.
        /// </summary>
        /// <param name="context">The <see cref="CookieSigningOutContext"/>.</param>
        public override Task SigningOut(CookieSigningOutContext context)
        {
            string Id = ClaimHelper.GetClaimValue(context.HttpContext.User.Claims, ClaimTypes.NameIdentifier);
            string UserName = ClaimHelper.GetClaimValue(context.HttpContext.User.Claims, ClaimTypes.Name);
            Logger.LogInformation("A user is signing out. UserName: {UserName}, Id: {Id}", UserName, Id);
            return base.SigningOut(context);
        }

        /// <summary>
        /// <para><strong>WARNING: </strong>here HttpContext.User.Identity.IsAuthenticated is yet false 
        /// because the authentication validation is not finished yet</para>
        /// https://stackoverflow.com/questions/73294941/how-to-update-user-claims-stored-in-authentication-cookie-in-net-core-6
        /// </summary>
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            try
            {
                if (context.Scheme.Name == CookieAuthenticationDefaults.AuthenticationScheme && context.Principal.Identity.IsAuthenticated)
                { 
                    string Id = ClaimHelper.GetClaimValue(context.Principal.Claims, ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrWhiteSpace(Id))
                    {
                        AppUser User = RBAC.GetAppUserById(Id); 
                        if (User == null)
                        {
                            context.RejectPrincipal();
                            return;
                        }
                        else if (User.IsBlocked)
                        {
                            context.RejectPrincipal();

                            // WARNING: here HttpContext.User.Identity.IsAuthenticated is yet false 
                            // because the authentication validation is not finished yet
                            MvcUserContext UserContext = Lib.GetService<MvcUserContext>();
                            await UserContext.SignOutAsync();
                            return;
                        }
                    } 
                }
            }
            catch
            {
                // do nothing
            }
        }

    }
}
