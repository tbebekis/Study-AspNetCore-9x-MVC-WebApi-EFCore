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
        public async Task<ItemResult<IAppClient>> ValidateApiClientCredentials(string ClientId, string PlainTextSecret)
        {
            ItemResult<IAppClient> Result = new();
            ItemResult<AppUser> ClientResult = await GetByProcAsync(c => c.UserName == ClientId);

            if (ClientResult.Item == null || !ClientResult.Succeeded)
            {
                Result.CopyErrors(ClientResult);
                return Result;
            }

            if (!Hasher.Validate(PlainTextSecret, ClientResult.Item.Password, ClientResult.Item.PasswordSalt))
            {
                Result.ErrorResult(ApiStatusCodes.InvalidCredentials);
                return Result;
            }

            Result.Item = ClientResult.Item;

            return Result;
        }
    }
}
