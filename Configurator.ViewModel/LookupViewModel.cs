namespace Configurator.ViewModel
{
    public class LookupViewModel
    {
        public int Id { get; set; }
        public int Offset { get; set; }
        public string LookupType { get; set; }
        public string Value { get; set; }

        public LookupViewModel(string lookupType, int offset, string value)
        {
            LookupType = lookupType;
            Offset = offset;
            Value = value;
        }
    }
}
