namespace Cutec.Blazor.WebAPIs
{
    public class GeolocationPositionError
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public enum GeolocationPositionErrorCode
    {
        /// <summary>
        /// The acquisition of the geolocation information failed because the page didn't have the permission to do it.
        /// </summary>
        PermissionDenied = 1,

        /// <summary>
        /// The acquisition of the geolocation failed because at least one internal source of position returned an internal error.
        /// </summary>
        PositionUnavailable = 2,

        /// <summary>
        /// The time allowed to acquire the geolocation, defined by PositionOptions.timeout information was reached before the information was obtained.
        /// </summary>
        Timeout = 3
    }
}
