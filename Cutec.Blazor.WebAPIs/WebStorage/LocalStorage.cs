using Microsoft.JSInterop;
using System.Text.Json;

namespace Cutec.Blazor.WebAPIs
{
    public class LocalStorage
    {
        private readonly IJSInProcessRuntime js;

        public LocalStorage(IJSInProcessRuntime js)
        {
            this.js = js;
        }

        public int Length
        {
            get
            {
                return js.Invoke<int>("localStorage.lenth");
            }
        }

        public string Key(int index)
        {
            return js.Invoke<string>("localStorage.key", index);
        }

        public string GetItem(string keyName)
        {
            return js.Invoke<string>("localStorage.getItem", keyName);
        }

        public void SetItem(string keyName, string keyValue)
        {
            js.InvokeVoid("localStorage.setItem", keyName, keyValue);
        }

        public void RemoveItem(string keyName)
        {
            js.InvokeVoid("localStorage.removeItem", keyName);
        }

        public void Clear()
        {
            js.InvokeVoid("localStorage.clear");
        }

        #region extensions

        public T GetItem<T>(string keyName) where T : class
        {
            var keyValue = js.Invoke<string>("localStorage.getItem", keyName);

            if (!string.IsNullOrEmpty(keyValue))
            {
                var item = JsonSerializer.Deserialize<T>(keyValue);
                return item;
            }

            return null;
        }

        public void SetItem<T>(string keyName, T item) where T : class
        {
            if (item == null)
            {
                return;
            }

            var keyValue = JsonSerializer.Serialize(item);
            js.InvokeVoid("localStorage.setItem", keyName, keyValue);
        }

        #endregion
    }
}
