namespace Cutec.Blazor.WebAPIs
{
    public class GeolocationPosition
    {
        /// <summary>
        /// A value defining the current location.
        /// </summary>
        public GeolocationCoordinates Coords { get; set; }

        /// <summary>
        /// A value indicating the time at which the location was retrieved.
        /// It is the milliseconds since 1/1/1970 UTC.
        /// </summary>
        public long Timestamp { get; set; }
    }
}
