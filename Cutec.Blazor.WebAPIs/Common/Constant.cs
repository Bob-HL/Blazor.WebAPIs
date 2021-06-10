using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cutec.Blazor.WebAPIs
{
    public class Constant
    {
        public const string LoadJsCssFile = "__loadJsCssFile";
        public readonly static string ScriptPrefix = $"_content/{typeof(Constant).Namespace}/";

        internal const string IndexedDbInitializer = "__cbw_idb.initIndexedDb";
        internal const string IndexDbLoaded = "__cbw_idb_loaded";
        internal const string IndexedDbAgentNamePrefix = "cbw_idb_";
    }
}
