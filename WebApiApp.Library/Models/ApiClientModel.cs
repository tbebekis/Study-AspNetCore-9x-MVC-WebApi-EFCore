namespace WebApiApp.Models
{

    [Description("Used by an Admin to register a new client application")]
    public class ApiClientModel    
    {

        [Description("The ClientId. Admin generated"), Required]
        [MaxLength(96)]
        public string ClientId { get; set; }

        [Description("The Client Secret un-encrypted. Admin generated"), Required]
        [MaxLength(64)]
        public string Secret { get; set; }

        [Description("The Client Name. Admin generated")]
        [MaxLength(96)]
        public string Name { get; set; }
    }
}
