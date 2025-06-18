namespace StudyLib.Entities
{
    [Description("Indicates the user type.")]
    public enum AppUserType
    {
        [Description("None")]
        None = 0,
        [Description("The user is a person.")]
        User = 1,
        [Description("The user is a client application.")]
        Client = 2,
        [Description("The user is either a person or a client application with the Admin role.")]
        Admin = 0xFFFF
    }
}
