namespace Configurator.Application
{
    public class AppSettings
	{
        public string DatabasePath { get; set; }
        public bool ConfiguratorMode { get; set; }
        public string BaseUrl { get; set; }
        public string ApplicationFilePath { get; set; }
        public string BootloaderFilePath { get; set; }
        public string TargetMarketOptions { get; set; }
        public string PemFileName { get; set; }
        public string LookUpSampleFile { get; set; }
        public string LoadLookupData { get; set; }
        public string ServiceUri { get; set; }
        public string StringSetting { get; set; }
        public int IntegerSetting { get; set; }
        public bool BooleanSetting { get; set; }
        public string MyName { get; set; }
        
    }
}
