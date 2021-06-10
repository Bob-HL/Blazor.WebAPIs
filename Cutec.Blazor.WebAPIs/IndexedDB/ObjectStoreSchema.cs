using System.Collections.Generic;

namespace Cutec.Blazor.WebAPIs
{
    public class ObjectStoreSchema
    {
        public ObjectStoreSchema(string name, string keyPath, bool autoIncrement = false, List<IndexSchema> indexes = null)
        {
            Name = name;
            KeyPath = keyPath;
            AutoIncrement = autoIncrement;
            Indexes = indexes;
        }

        public string Name { get; set; }
        public string KeyPath { get; set; }
        public bool AutoIncrement { get; set; }

        public List<IndexSchema> Indexes { get; set; }
    }
}
