using Microsoft.JSInterop;

namespace Cutec.Blazor.WebAPIs
{
    public class SessionStorage : Storage
    {
        public SessionStorage(IJSInProcessRuntime js) : base("sessionStorage", js)
        {
        }
    }
}
