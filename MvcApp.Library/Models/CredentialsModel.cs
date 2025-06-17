namespace MvcApp.Models
{
    public class CredentialsModel
    {
        [RequiredEx]
        [Title("UserName")]
        public string UserName { get; set; }

        [RequiredEx]
        [DataType(DataType.Password)]
        [Title("Password")]
        public string Password { get; set; }

        [RequiredEx]
        [Title("RememberMe")]
        public bool RememberMe { get; set; }
    }
}
