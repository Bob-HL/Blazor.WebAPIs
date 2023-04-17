using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Cutec.Blazor.WebAPIs.WebAssemblyDemo.Pages
{
    public partial class GeolocationDemo : ComponentBase
    {
        [Inject] private Geolocation geolocation { get; set; }

        private GeolocationPosition currentPosition;
        private GeolocationPosition position;
        private bool watching;

        private async Task GetCurrentPositionAsync()
        {
            currentPosition = await geolocation.GetCurrentPositionAsync();
        }

        private async Task WatchPositionAsync()
        {
            geolocation.PositionReceived += Geolocation_PositionReceived;
            await geolocation.WatchPositionAsync();
            watching = true;
        }

        private void Geolocation_PositionReceived(object sender, PositionEventArgs e)
        {
            position = e.Position;
            StateHasChanged();
        }

        private async Task ClearWatchAsync()
        {
            await geolocation.ClearWatchAsync();
            position = null;
            geolocation.PositionReceived -= Geolocation_PositionReceived;
            watching = false;
        }
    }
}
