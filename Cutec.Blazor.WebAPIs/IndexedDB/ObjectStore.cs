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


        // Gets the value of the first record in a store matching the key range query. Refer GetAllAsync for key range explanation.
        public async Task<T> GetByKeyRangeAsync(object lowerKey = null, bool lowerOpen = false, object upperKey = null, bool upperOpen = false)
        {
            T data = await js.InvokeAsync<T>($"{indexedDbAgentName}.getByKeyRange", Name, lowerKey, lowerOpen, upperKey, upperOpen);
            return data;
        }

        // Gets the value of the first record in a store matching the index value range query. Refer GetAllAsync for range explanation.
        public async Task<T> GetFromIndexAsync<TIndex>(Expression<Func<T, TIndex>> indexSelector, object lowerKey = null, bool lowerOpen = false, object upperKey = null, bool upperOpen = false)
        {
            MemberExpression member = indexSelector.Body as MemberExpression;
            var indexName = member.Member.Name.ToCamelCase();
            T data = await js.InvokeAsync<T>($"{indexedDbAgentName}.getFromIndex", Name, indexName, lowerKey, lowerOpen, upperKey, upperOpen);
            return data;
        }

        /// <summary>
        /// Gets all values in a store that match the key range. Ref: IDBKeyRange - https://developer.mozilla.org/en-US/docs/Web/API/IDBKeyRange.
        /// </summary>
        /// <param name="lowerKey">The lower value of the key range, including undefined.</param>
        /// <param name="lowerOpen">Matches those with key equal to lowerKey if lowerOpen is false.</param>
        /// <param name="upperKey">The upper value of the key range.</param>
        /// <param name="upperOpen">Matches those with key equal to upperKey if upperOpen is false.</param>
        /// <param name="count">Maximum number of values to return.</param>
        /// <returns>All items in a store that match the query.</returns>
        public async Task<List<T>> GetAllAsync(object lowerKey = null, bool lowerOpen = false, object upperKey = null, bool upperOpen = false, int? count = null)
        {
            var data = await js.InvokeAsync<List<T>>($"{indexedDbAgentName}.getAll", Name, lowerKey, lowerOpen, upperKey, upperOpen, count);
            return data;
        }
        public async Task<List<T>> GetAllFromIndexAsync<TIndex>(Expression<Func<T, TIndex>> indexSelector, object lowerKey = null, bool lowerOpen = false, object upperKey = null, bool upperOpen = false, int? count = null)
        {
            MemberExpression member = indexSelector.Body as MemberExpression;
            var indexName = member.Member.Name.ToCamelCase();
            var data = await js.InvokeAsync<List<T>>($"{indexedDbAgentName}.getAllFromIndex", Name, indexName, lowerKey, lowerOpen, upperKey, upperOpen, count);
            return data;
        }

        public async Task<List<T>> GetAllByIndexValueAsync<TIndex>(Expression<Func<T, TIndex>> indexSelector, TIndex indexValue)
        {
            MemberExpression member = indexSelector.Body as MemberExpression;
            var indexName = member.Member.Name.ToCamelCase();
            var data = await js.InvokeAsync<List<T>>($"{indexedDbAgentName}.getAllByIndexValue", Name, indexName, indexValue);
            return data;
        }

        public async Task<List<TKey>> GetAllKeysAsync<TKey>(object lowerKey = null, bool lowerOpen = false, object upperKey = null, bool upperOpen = false, int? count = null)
        {
            var keys = await js.InvokeAsync<List<TKey>>($"{indexedDbAgentName}.getAllKeys", Name, lowerKey, lowerOpen, upperKey, upperOpen, count);
            return keys;
        }

        public async Task<List<TKey>> GetAllKeysFromIndexAsync<TIndex, TKey>(Expression<Func<T, TIndex>> indexSelector, object lowerKey = null, bool lowerOpen = false, object upperKey = null, bool upperOpen = false, int? count = null)
        {
            MemberExpression member = indexSelector.Body as MemberExpression;
            var indexName = member.Member.Name.ToCamelCase();
            var keys = await js.InvokeAsync<List<TKey>>($"{indexedDbAgentName}.getAllKeysFromIndex", Name, indexName, lowerKey, lowerOpen, upperKey, upperOpen, count);
            return keys;
        }

        public async Task<List<TKey>> GetAllKeysByIndexValueAsync<TIndex, TKey>(Expression<Func<T, TIndex>> indexSelector, TIndex indexValue)
        {
            MemberExpression member = indexSelector.Body as MemberExpression;
            var indexName = member.Member.Name.ToCamelCase();
            var keys = await js.InvokeAsync<List<TKey>>($"{indexedDbAgentName}.getAllKeysByIndexValue", Name, indexName, indexValue);
            return keys;
        }

        public async Task<List<TIndex>> GetAllIndexValuesAsync<TIndex>(Expression<Func<T, TIndex>> indexSelector)
        {
            MemberExpression member = indexSelector.Body as MemberExpression;
            var indexName = member.Member.Name.ToCamelCase();
            var keys = await js.InvokeAsync<List<TIndex>>($"{indexedDbAgentName}.getAllIndexValues", Name, indexName);
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

        public async Task DeleteByKeyListAsync<TKey>(List<TKey> keys)
        {
            await js.InvokeAsync<T>($"{indexedDbAgentName}.delete", Name, keys);
        }

        public async Task ClearAsync()
        {
            await js.InvokeVoidAsync($"{indexedDbAgentName}.clear", Name);
        }
    }
}
