namespace CommonLib 
{

    /// <summary>
    /// An authorization handler that need to be called for a a <see cref="PermissionAttribute"/>  requirement.
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAttribute>
    {
        IHttpContextAccessor HttpContextAccessor;

        /// <summary>
        /// Makes a decision if authorization is allowed based on a specific requirement, which in this case is a <see cref="PermissionAttribute"/> attribute.
        /// </summary>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAttribute Attr)
        {
            var User = context.User;
            bool IsAuthenticated = User != null && User.Identity != null ? User.Identity.IsAuthenticated : false;

            if (IsAuthenticated && GetUserPermissionsFunc != null)
            {
                HttpContext HttpContext = HttpContextAccessor.HttpContext;
                List<string> UserPermissionList = GetUserPermissionsFunc(HttpContext);

                foreach (var UserPermission in UserPermissionList)
                {
                    foreach (var Requirement in Attr.Permissions)
                    {
                        if (string.Compare(UserPermission, Requirement, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            context.Succeed(Attr);
                            break;
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public PermissionAuthorizationHandler(IHttpContextAccessor HttpContextAccessor)
        {
            this.HttpContextAccessor = HttpContextAccessor;
        }
 

        /// <summary>
        /// Callback returning the list of permission names assigned to the user/client of the current request.
        /// <para><strong>WARNING: </strong> This property should be properly assigned in the application startup.</para>
        /// </summary>
        static public Func<HttpContext, List<string>> GetUserPermissionsFunc { get; set; }
    }
}
