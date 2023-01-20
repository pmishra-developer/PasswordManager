namespace Configurator.ViewModel
{
    public class SubscriptionViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string iCodeSerialNumber { get; set; }
        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
