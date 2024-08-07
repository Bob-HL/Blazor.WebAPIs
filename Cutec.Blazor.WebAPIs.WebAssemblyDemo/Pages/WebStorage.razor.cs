using Microsoft.AspNetCore.Components;

namespace Cutec.Blazor.WebAPIs.WebAssemblyDemo.Pages
{
    public partial class WebStorage : ComponentBase
    {
        [Inject]
        private SessionStorage sessionStorage { get; set; }
        [Inject]
        private LocalStorage localStorage { get; set; }

        private string? sessionKey;
        private string? sessionValue;

        private string? localKey;
        private string? localValue;

        private void SaveSessionValue()
        {
            if (!string.IsNullOrWhiteSpace(sessionKey))
            {
                sessionStorage.SetItem(sessionKey, sessionValue);
            }
        }

        private void GetSessionValue()
        {
            if (!string.IsNullOrWhiteSpace(sessionKey))
            {
                sessionValue = sessionStorage.GetItem(sessionKey);
            }
        }
    }
}
