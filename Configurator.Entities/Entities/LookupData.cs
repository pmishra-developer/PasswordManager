namespace Configurator.Database.Entities
{
    public class LookupData : IdentityEntity
    {
        public int Offset { get; set; }
        public LookupType LookupType { get; set; }
        public string Value { get; set; }
    }
}
