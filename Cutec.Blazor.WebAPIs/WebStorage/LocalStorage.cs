using Microsoft.JSInterop;

namespace Cutec.Blazor.WebAPIs
{
    public class LocalStorage : Storage
    {
        public LocalStorage(IJSInProcessRuntime js) : base ("localStorage", js)
        {
        }
    }
}
