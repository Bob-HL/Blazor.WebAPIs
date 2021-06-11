using System;

namespace Cutec.Blazor.WebAPIs
{
    public class PositionEventArgs : EventArgs
    {
        public PositionEventArgs(GeolocationPosition position)
        {
            Position = position;
        }

        public GeolocationPosition Position { get; set; }
    }
}
