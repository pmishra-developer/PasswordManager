namespace Configurator.Database.Entities
{
    public class Device : IdentityEntity
    {
        public string SerialNumber { get; set; }
        public string UUID { get; set; }
        public string TargetMarket { get; set; }
        public byte[] UnitRandomId { get; set; }
        public byte[] UnitRandomKey { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
