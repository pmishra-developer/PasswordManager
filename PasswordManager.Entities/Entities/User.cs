namespace PasswordManager.Database.Entities
{
    public class User : IdentityEntity
    {
        public string Name { get; set; } = string.Empty;
    }
}
