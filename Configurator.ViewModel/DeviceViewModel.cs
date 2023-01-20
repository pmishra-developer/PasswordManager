namespace Configurator.ViewModel
{
    public class DeviceViewModel
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string UUID { get; set; }
        public string TargetMarket { get; set; }
        public byte[] UnitRandomId { get; set; }
        public byte[] UnitRandomKey { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }

        public DeviceViewModel()
        {

        }

        public DeviceViewModel(int id, string serialNumber, string uuid, string targetMarket, byte[] randomId, byte[] randomKey)
        {
            Id = id;
            SerialNumber = serialNumber;
            UUID = uuid + "89ABCDEF";
            TargetMarket = targetMarket;
            UnitRandomId = randomId;
            UnitRandomKey = randomKey;
        }
    }
}
