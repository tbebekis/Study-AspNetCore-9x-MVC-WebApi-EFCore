namespace WebApiApp.Controllers
{
    [Permission("RBAC.Admin")]
    [Tags("Security")]
    [Route("rbac")]
    public class RBACController : WebApiController
    {
        // ● actions
        [EndpointDescription("Returns the list of registered clients.")]
        [HttpGet("client/list", Name = "Client.List"), Produces<ListResult<IAppClient>>]
        public async Task<ListResult<IAppClient>> List()
        {
            ListResult<IAppClient> Result = new();

            AppDataService<AppUser> Service = new();
            ListResult<AppUser> DataList = await Service.GetListAsync();
            string JsonText = JsonSerializer.Serialize(DataList);
            if (DataList.Succeeded)
            {
                List<IAppClient> List = DataList.List.Cast<AppUser>().Select(x => x as IAppClient).ToList();
                Result.List = List;
            }
            else
            {
                Result.CopyErrors(DataList);
            }

            return Result;
        }

        [EndpointDescription("Insert a new AppClient.")]
        [HttpPost("client", Name = "Client.Insert"), Produces<ItemResult<IAppClient>>]
        public async Task<ItemResult<IAppClient>> InsertClient(AppUser Model)
        {
            // TODO: Validation

            //
            AppUser Client = new AppUser(Model.UserName, Model.Password, Model.Name);
            AppDataService<AppUser> Service = new();
            ItemResult<AppUser> ResultClient = await Service.InsertAsync(Client);

            ItemResult<IAppClient> Result = new();
            Result.CopyErrors(ResultClient);
            Result.Item = ResultClient.Item;
            return Result;
        }
    }
}
