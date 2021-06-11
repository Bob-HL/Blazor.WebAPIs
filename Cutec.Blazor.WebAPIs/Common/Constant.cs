namespace Cutec.Blazor.WebAPIs
{
    internal class Constant
    {
        public readonly static string ScriptPrefix = $"_content/{typeof(Constant).Namespace}/";

        #region Hard coded names must be same as in the *.ts files.
        
        public const string Prefix = "__cbw_";
        public readonly static string JsAgent = $"{Prefix}js_";
        internal readonly static string IndexedDbInitializer = $"{Prefix}idb.initIndexedDb";
        internal readonly static string IndexDbLoaded = $"{Prefix}idb_loaded";
        public readonly static string GeoLocation = $"{Prefix}geo";
        
        #endregion

        internal readonly static string IndexedDbAgentNamePrefix = $"idb{Prefix}";

        //#region API flag

        //public const string Geolocation = nameof(Geolocation);
        //public const string IndexedDb = nameof(IndexedDb);

        //#endregion
    }
}
