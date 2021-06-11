namespace Cutec.Blazor.WebAPIs
{
    public class PositionOptions
    {
        /// <summary>
        /// A value indicates the application would like to receive the best possible results. If true and if the device is able to provide a more accurate position, it will do so. Note that this can result in slower response times or increased power consumption.
        /// </summary>
        public bool EnableHighAccuracy { get; set; }

        /// <summary>
        /// A positive long value representing the maximum length of time (in milliseconds) the device is allowed to take in order to return a position.
        /// Infinity (null) meaning that getCurrentPosition() won't return until the position is available.
        /// </summary>
        public long? Timeout { get; set; }

        /// <summary>
        /// A positive long value indicating the maximum age in milliseconds of a possible cached position that is acceptable to return. If set to 0, it means that the device cannot use a cached position and must attempt to retrieve the real current position. If set to null (Infinity) the device must return a cached position regardless of its age.
        /// </summary>
        public long? MaximumAge { get; set; } = 0;
    }
}
