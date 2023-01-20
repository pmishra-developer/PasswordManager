namespace Configurator.Database.Entities
{
    public class Subscription : IdentityEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int BootloaderDeviceId { get; set; }
        public virtual Device BootloaderDevice { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
