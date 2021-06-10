namespace Cutec.Blazor.WebAPIs
{
    public class IndexSchema
    {
        public IndexSchema(string keyPath, string name = null, bool? unique = null, bool? multiEntry = null)
        {
            KeyPath = keyPath;
            Name = name ?? keyPath;
            Unique = unique;
            MultiEntry = multiEntry;
        }

        public string Name { get; set; }
        public string KeyPath { get; set; }
        public bool? Unique { get; set; }
        public bool? MultiEntry { get; set; }
    }
}
