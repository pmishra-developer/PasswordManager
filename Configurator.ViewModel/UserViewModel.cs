namespace Configurator.ViewModel
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string BusinessName { get; set; }
        public string Phone { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpiryDate { get; set; }
        public string SerialNumber { get; set; }
        public int SubscriptionCount { get; set; }
        public bool ActiveSubscription { get; set; }
    }
}
