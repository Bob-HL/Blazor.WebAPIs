using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Cutec.Blazor.WebAPIs
{
    public class Geolocation : IDisposable
    {
        private int? watchId;
        private readonly IJSRuntime js;

        public Geolocation(IJSRuntime js)
        {
            this.js = js;
        }

        public async Task<GeolocationPosition> GetCurrentPositionAsync(PositionOptions options = null)
        {
            var position = await js.InvokeAsync<GeolocationPosition>($"{Constant.GeoLocation}.getCurrentPosition", options);
            return position;
        }

        #region watch

        public async Task WatchPositionAsync(PositionOptions options = null)
        {
            if (watchId.HasValue)
            {
                await ClearWatchAsync();
            }

            watchId = await js.InvokeAsync<int>($"{Constant.GeoLocation}.watchPosition", options, DotNetObjectReference.Create(this));
        }

        [JSInvokable("GotPosition")]
        public void GotPosition(GeolocationPosition position)
        {
            var e = new PositionEventArgs(position);
            OnPositionReceived(e);
        }

        [JSInvokable("OnError")]
        public void OnError(GeolocationPositionError error)
        {
            //TODO:
            Console.Error.WriteLine(error);
        }

        public event EventHandler<PositionEventArgs> PositionReceived;

        protected virtual void OnPositionReceived(PositionEventArgs e)
        {
            var handler = PositionReceived;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        public async Task ClearWatchAsync()
        {
            if (watchId.HasValue)
            {
                await js.InvokeVoidAsync("navigator.geolocation.clearWatch", watchId);
                watchId = null;
            }
        }

        #region IDisposable

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                _ = Task.Run(() => ClearWatchAsync());
                disposedValue = true;
            }
        }

        ~Geolocation()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
