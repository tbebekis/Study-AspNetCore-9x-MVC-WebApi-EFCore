namespace CommonLib 
{

    /// <summary>
    /// The developer may define one or more requirements (Permissions) in a comma delimited string.
    /// <para>A user or client application must own any of those requirements in order to access the protected resource.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PermissionAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthorizationRequirementData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PermissionAttribute(string Permissions)
        {
            this.Permissions = Permissions.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries); 
        }
 

        /// <summary>
        /// Returns <see cref="IAuthorizationRequirement"/> that should be satisfied for authorization.
        /// </summary>
        public IEnumerable<IAuthorizationRequirement> GetRequirements()
        {
            yield return this;
        }

        /// <summary>
        /// The list of permissions.
        /// <para>A user or client application must own any of those requirements (Permissions) in order to access the protected resource.</para>
        /// </summary>
        public string[] Permissions { get; set; }
    }
}
