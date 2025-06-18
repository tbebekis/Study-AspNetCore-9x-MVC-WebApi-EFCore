namespace WebApiApp.Library
{
    public class ApiClientService : AppDataService<AppUser>
    {

        public ApiClientService()
        {
        }

        /// <summary>
        /// Validates the specified user credentials and returns a <see cref="IRequestor"/> on success, else null.
        /// </summary>
        public async Task<ItemResult<IApiClient>> ValidateApiClientCredentials(string ClientId, string PlainTextSecret)
        {
            ItemResult<IApiClient> Result = new();
            ItemResult<AppUser> ClientResult = await GetByProcAsync(c => c.UserName == ClientId && (c.UserType == AppUserType.Client || c.UserType == AppUserType.Admin));

            if (ClientResult.Item == null || !ClientResult.Succeeded)
            {
                Result.CopyErrors(ClientResult);                
            }
            else if (!Hasher.Validate(PlainTextSecret, ClientResult.Item.Password, ClientResult.Item.PasswordSalt))
            {
                Result.ErrorResult(ApiStatusCodes.InvalidCredentials);                
            }
            else
            {
                Result.Item = ClientResult.Item;
            }

            return Result;
        }
    }
}
