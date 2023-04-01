using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Cutec.Blazor.WebAPIs
{
    public class ObjectStoreSchema
    {
        public ObjectStoreSchema(string name, PropertyInfo keyProp, bool autoIncrement = false, List<IndexSchema> indexes = null)
        {
            Name = name;
            KeyPath = keyProp?.Name.ToCamelCase();
            AutoIncrement = autoIncrement;
            Indexes = indexes;

            if (keyProp != null && autoIncrement)
            {
                AutoKeyProp = keyProp;
            }
        }

        public string Name { get; set; }
        public string KeyPath { get; set; }
        public bool AutoIncrement { get; set; }

        /// <summary>
        /// The PropertyInfo of the object. It is null if AutoIncrement == false or KeyPath is null.
        /// </summary>
        [JsonIgnore]
        public PropertyInfo AutoKeyProp { get; set; }

        public List<IndexSchema> Indexes { get; set; }
    }
}
