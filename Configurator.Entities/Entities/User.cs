namespace Configurator.Database.Entities
{
    public class User : IdentityEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string BusinessName { get; set; }
        public string Phone { get; set; }
        public List<Subscription> Subscriptions { get; set; }
    }
}
