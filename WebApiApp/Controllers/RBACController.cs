namespace WebApiApp.Controllers
{
    [Permission("RBAC.Admin")]
    [Tags("Security")]
    [Route("rbac")]
    public class RBACController : WebApiController
    {
        // ● actions
        [EndpointDescription("Returns the list of registered clients.")]
        [HttpGet("client/list", Name = "Client.List"), Produces<ListResult<IApiClient>>]
        public async Task<ListResult<IApiClient>> List()
        {
            ListResult<IApiClient> Result = new();

            ApiClientService Service = new();
            ListResult<AppUser> DataList = await Service.GetListWithFilterAsync(
                                                            c => c.UserType == AppUserType.Client || c.UserType == AppUserType.Admin,       // filter
                                                            null                                                                            // order  
                                                            );
            string JsonText = JsonSerializer.Serialize(DataList);

            if (DataList.Succeeded)
            {
                List<IApiClient> List = DataList.List.Cast<AppUser>().Select(x => x as IApiClient).ToList();
                Result.List = List;
            }
            else
            {
                Result.CopyErrors(DataList);
            }

            return Result;
        }

        [EndpointDescription("Insert a new AppClient.")]
        [HttpPost("client", Name = "Client.Insert"), Produces<ItemResult<IApiClient>>]
        public async Task<ItemResult<IApiClient>> InsertClient(ApiClientModel Model)
        {
            // TODO: Validation

            //
            AppUser Client = new AppUser(AppUserType.Client, Model.ClientId, Model.Secret, Model.Name);
            
            ApiClientService Service = new();
            ItemResult<AppUser> ResultClient = await Service.InsertAsync(Client);

            ItemResult<IApiClient> Result = new();
            Result.CopyErrors(ResultClient);
            Result.Item = ResultClient.Item;
            return Result;
        }
    }
}
