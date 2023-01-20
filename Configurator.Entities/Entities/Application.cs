namespace Configurator.Database.Entities
{
    public class Application : IdentityEntity
    {
        public int BootLoaderDeviceId { get; set; }
        public string UUID { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
