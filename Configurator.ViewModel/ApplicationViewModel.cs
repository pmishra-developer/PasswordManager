
namespace Configurator.ViewModel
{
    public class ApplicationViewModel
    {
        public int Id { get; set; }
        public int BootLoaderDeviceId { get; set; }
        public string UUID { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string Options { get; set; }
        public string Created { get; set; }

        public ApplicationViewModel()
        {

        }

        public ApplicationViewModel(int id, string uuid, string fileName, byte[] fileContent)
        {
            Id = id;
            UUID = uuid;
            FileName = fileName;
            Content = fileContent;
        }
    }
}
