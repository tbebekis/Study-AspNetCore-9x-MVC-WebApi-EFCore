namespace WebApiApp.Library
{
    public class AppSettings
    {
        public AppSettings()
        {
        }

        public DefaultSettings Defaults { get; set; } = new DefaultSettings();
        public JwtSettings Jwt { get; set; } = new JwtSettings();
    }
}
