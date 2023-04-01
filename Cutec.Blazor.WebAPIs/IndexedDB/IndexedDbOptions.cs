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

        /// <summary>
        /// Set a higher Version if there are changes (store, key or index) required an upgrade. Set Version to 0 to open current version of DB. 
        /// </summary>
        public int Version { get; set; } = 0;

        [JsonIgnore]
        public List<ObjectStoreSchema> Schema { get; set; }
    }
}
