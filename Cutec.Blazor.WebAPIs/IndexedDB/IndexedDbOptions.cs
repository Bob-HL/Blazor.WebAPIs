using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cutec.Blazor.WebAPIs
{
    public class IndexedDbOptions
    {
        public string Name
        {
            get { return name; }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }

                name = value;
                IndexedDbAgentName = Constant.IndexedDbAgentNamePrefix + name;
            }
        }

        private string name;

        public string IndexedDbAgentName { get; protected set; }

        public int Version { get; set; } = 1;

        [JsonIgnore]
        public List<ObjectStoreSchema> Schema { get; set; }
    }
}
