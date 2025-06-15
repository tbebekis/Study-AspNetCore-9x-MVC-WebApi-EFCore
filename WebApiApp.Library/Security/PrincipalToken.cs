namespace WebApiApp.Library
{
    public class PrincipalToken
    {
        public PrincipalToken() { }

        public ClaimsPrincipal Principal { get; set; }
        public JwtSecurityToken JwtToken { get; set; }
    }
}
