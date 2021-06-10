using Cutec.Blazor.WebAPIs;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cutec.Blazor.WebAPIs
{
    public class ObjectStore<T> where T: class
    {
        private readonly IJSRuntime js;
        private readonly string indexedDbAgentName;

        public ObjectStore(string name, IJSRuntime js, string indexedDbAgentName)
        {
            Name = name;
            this.js = js;
            this.indexedDbAgentName = indexedDbAgentName;
        }

        public string Name { get; set; }

        public async Task<T> GetByKeyAsync(object key)
        {
            T data = await js.InvokeAsync<T>($"{indexedDbAgentName}.getByKey", Name, key);
            return data;
        }

        public async Task<List<T>> GetAllAsync()
        {
            var data = await js.InvokeAsync<List<T>>($"{indexedDbAgentName}.getAll", Name);
            return data;
        }

        public async Task<List<T>> GetAllByIndexValueAsync<TIndex>(Expression<Func<T, TIndex>> indexSelector, TIndex indexValue)
        {
            MemberExpression member = indexSelector.Body as MemberExpression;
            var indexName = member.Member.Name.ToCamelCase();
            var data = await js.InvokeAsync<List<T>>($"{indexedDbAgentName}.getAllByIndexValue", Name, indexName, indexValue);
            return data;
        }

        public async Task<List<TIndex>> GetAllKeysByIndexValueAsync<TIndex>(Expression<Func<T, TIndex>> indexSelector)
        {
            MemberExpression member = indexSelector.Body as MemberExpression;
            //await js.InvokeVoidAsync("console.log", indexSelector.Body.ToString());
            var indexName = member.Member.Name.ToCamelCase();
            var keys = await js.InvokeAsync<List<TIndex>>($"{indexedDbAgentName}.getAllKeysByIndexValue", Name, indexName);
            return keys;
        }

        public async Task<List<TKey>> GetAllKeysAsync<TKey>()
        {
            var keys = await js.InvokeAsync<List<TKey>>($"{indexedDbAgentName}.getAllKeys", Name);
            return keys;
        }

        public async Task<int> CountAsync()
        {
            var count = await js.InvokeAsync<int>($"{indexedDbAgentName}.count", Name);
            return count;
        }

        /// <summary>
        /// Update a record.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task PutAsync(T data)
        {
            if (data != null)
            {
                await js.InvokeAsync<T>($"{indexedDbAgentName}.put", Name, data);
            }
        }

        public async Task PutListAsync(List<T> data)
        {
            if (data != null && data.Count > 0)
            {
                await js.InvokeAsync<T>($"{indexedDbAgentName}.put", Name, data);
            }
        }

        /// <summary>
        /// Delete a record or a list of records.
        /// </summary>
        /// <param name="key">The primary key or a list of primary keys of the record(s).</param>
        /// <returns></returns>
        public async Task DeleteByKeyAsync(object key)
        {
            await js.InvokeAsync<T>($"{indexedDbAgentName}.delete", Name, key);
        }

        public async Task ClearAsync()
        {
            await js.InvokeVoidAsync($"{indexedDbAgentName}.clear", Name);
        }
    }
}
