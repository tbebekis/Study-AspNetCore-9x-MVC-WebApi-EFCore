namespace StudyLib
{
    static public class RBAC
    {
        static Func<DbContext> GetDataContextFunc;

        static RBAC()
        {
        }

        static public void Initialize(Func<DbContext> GetDataContextFunc)
        {
            RBAC.GetDataContextFunc = GetDataContextFunc;
        }

        static public AppUser GetAppUserById(string Id)
        {
            AppUser Result = null;

            using (var DataContext = GetDataContextFunc())
            {
                DbSet<AppUser> Users = DataContext.Set<AppUser>();
                Result = Users.FirstOrDefault(x => x.Id == Id);
            }

            return Result;
        }
        static public List<AppRole> GetUserRoles(string Id)
        {
            List<AppRole> Result = new List<AppRole>();

            using (var DataContext = GetDataContextFunc())
            {
                DbSet<AppUser> Users = DataContext.Set<AppUser>();
                DbSet<AppRole> Roles = DataContext.Set<AppRole>();
                DbSet<AppUserRole> UserRoles = DataContext.Set<AppUserRole>();

                AppUser User = Users.FirstOrDefault(x => x.Id == Id);
                if (User != null)
                {
                    // get the Ids of the roles the specified client is member of
                    List<string> RoleIdList = UserRoles.Where(r => r.UserId == Id)
                                              .Select(x => x.RoleId)
                                              .ToList();

                    // get the role object for each role Id
                    Result = Roles.Where(r => RoleIdList.Contains(r.Id))
                                  .Select(r => r)
                                  .ToList();
                }
            }

            return Result;
        }

        static public List<AppPermission> GetUserPermissions(string Id)
        {
            List<AppPermission> Result = new List<AppPermission>();

            // get the roles the specified client is member of
            List<AppRole> ClientRoleList = GetUserRoles(Id);

            if (ClientRoleList.Count > 0)
            {
                using (var DataContext = GetDataContextFunc())
                {
                    DbSet<AppRolePermission> RolePermissions = DataContext.Set<AppRolePermission>();
                    DbSet<AppPermission> Permissions = DataContext.Set<AppPermission>();

                    // for each role the client is member of
                    foreach (var Role in ClientRoleList)
                    {
                        // get the Ids of the permissions of that role in a string list
                        List<string> PermissionIdList = RolePermissions
                                            .Where(p => p.RoleId == Role.Id)
                                            .Select(x => x.PermissionId)
                                            .ToList();

                        if (PermissionIdList.Count > 0)
                        {
                            // get the permission object for each permission Id
                            List<AppPermission> List = Permissions.Where(p => PermissionIdList.Contains(p.Id))
                                                .Select(p => p)
                                                .ToList();

                            // add the permission object to the result list, if not already there
                            foreach (var Permission in List)
                            {
                                if (Result.FirstOrDefault(x => x.Id == Permission.Id) == null)
                                    Result.Add(Permission);
                            }
                        }
                    }
                }
            }



            return Result;
        }
    
        static public List<string> GetUserPermissionListForWebApi(HttpContext HttpContext)
        {
            List<string> Result = new();
            JwtSecurityToken JwtToken = TokenHelper.ReadTokenFromRequestHeader(HttpContext);
 
            string Id = ClaimHelper.GetClaimValue(JwtToken.Claims, System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

            if (!string.IsNullOrWhiteSpace(Id))
            {
                List<AppPermission> Permissions = RBAC.GetUserPermissions(Id);

                foreach (var Permission in Permissions)
                    Result.Add(Permission.Name);
            }

            return Result;
        }
        static public List<string> GetUserPermissionListForMvc(HttpContext HttpContext)
        {
            List<string> Result = new();

            string Id = ClaimHelper.GetClaimValue(HttpContext.User.Claims, System.Security.Claims.ClaimTypes.NameIdentifier);

            if (!string.IsNullOrWhiteSpace(Id))
            {
                List<AppPermission> Permissions = RBAC.GetUserPermissions(Id);

                foreach (var Permission in Permissions)
                    Result.Add(Permission.Name);
            }

            return Result;
        }
    }
}
