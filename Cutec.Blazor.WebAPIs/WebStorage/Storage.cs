using Microsoft.JSInterop;
using System.Text.Json;

namespace Cutec.Blazor.WebAPIs
{
    public abstract class Storage
    {
        private string storage;

        private readonly IJSInProcessRuntime js;

        public Storage(string storage, IJSInProcessRuntime js)
        {
            this.storage = storage;
            this.js = js;
        }

        /// <summary>
        /// Returns an integer representing the number of data items stored in the Storage object.
        /// </summary>
        public int Length
        {
            get
            {
                return js.Invoke<int>($"{storage}.lenth");
            }
        }

        /// <summary>
        /// When passed a number n, this method will return the name of the nth key in the storage.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string Key(int index)
        {
            return js.Invoke<string>($"{storage}.key", index);
        }

        public string GetItem(string keyName)
        {
            return js.Invoke<string>($"{storage}.getItem", keyName);
        }

        public void SetItem(string keyName, string keyValue)
        {
            js.InvokeVoid($"{storage}.setItem", keyName, keyValue);
        }

        public void RemoveItem(string keyName)
        {
            js.InvokeVoid($"{storage}.removeItem", keyName);
        }

        public void Clear()
        {
            js.InvokeVoid($"{storage}.clear");
        }

        #region extensions

        public T GetItem<T>(string keyName) where T : class
        {
            var keyValue = js.Invoke<string>($"{storage}.getItem", keyName);

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
            js.InvokeVoid($"{storage}.setItem", keyName, keyValue);
        }

        #endregion
    }
}
