namespace MvcApp.Library
{
    public class AppUserService : AppDataService<AppUser>
    {

        public AppUserService()
        {
        }

        /// <summary>
        /// Validates the specified user credentials and returns a <see cref="AppUser"/> on success, else null.
        /// </summary>
        public async Task<ItemResult<IAppUser>> ValidateAppUserCredentials(string UserName, string PlainTextSecret)
        {
            ItemResult<IAppUser> Result = new();
            ItemResult<AppUser> UserResult = await GetByProcAsync(c => c.UserName == UserName && (c.UserType == AppUserType.User || c.UserType == AppUserType.Admin));

            if (UserResult.Item == null || !UserResult.Succeeded)
            {
                Result.CopyErrors(UserResult);
            }
            else if (!Hasher.Validate(PlainTextSecret, UserResult.Item.Password, UserResult.Item.PasswordSalt))
            {
                Result.ErrorResult(ApiStatusCodes.InvalidCredentials);
            }
            else
            {
                Result.Item = UserResult.Item;
            }
                

            return Result;
        }


        /// <summary>
        /// Returns true if the user is impersonating another user, by using a super user password
        /// </summary>
        public bool GetIsUserImpersonation(string PlainTextPassword)
        {
            // TODO: GetIsImpersonation() - user should come from database
            // bool IsImpersonation = !string.IsNullOrWhiteSpace(Settings.SuperUserPassword) && Settings.SuperUserPassword == PlainTextPassword;
            return false;
        }
    }
}
