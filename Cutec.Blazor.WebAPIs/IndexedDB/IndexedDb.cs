using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Cutec.Blazor.WebAPIs
{
    public class IndexedDb
    {
        private readonly static Type objectStoreGenericType = typeof(ObjectStore<>);

        private IndexedDbOptions options;
        private IJSRuntime js;
        private ILookup<Type, dynamic> typeProperties;
        private bool isOpen;

        public IndexedDb()
        {
        }

        public int Version { get; protected set; }
        public bool IsOpen { get { return isOpen; } }

        public async Task InitializeAsync(IndexedDbOptions options, IJSRuntime js)
        {
            this.options = options;
            this.js = js;
            await OpenAsync();
        }

        public async Task OpenAsync()
        {
            if (!isOpen)
            {
                if (options == null || js == null)
                {
                    throw new InvalidOperationException("IndexedDb.InitializeAsync() must be called first.");
                }

                Initialize();

                var numberValue = await js.InvokeAsync<NumberValue>(Constant.IndexedDbInitializer, options, DotNetObjectReference.Create(this));
                Version = numberValue.Value;
                isOpen = true;
            }
        }

        [JSInvokable("GetSchema")]
        public List<ObjectStoreSchema> GetSchema(int oldVersion, int newVersion)
        {
            return options?.Schema;
        }

        [JSInvokable("OnDbEvent")]
        public virtual void OnDbEvent(IndexedDbEvent dbEvent, int currentVersion, int blockedVersion)
        {
        }

        public ObjectStore<T> Store<T>() where T: class
        {
            var entityType = typeof(T);
            var properties = typeProperties[entityType];

            var count = properties.Count();

            if (count == 1)
            {
                return properties.First();
            }
            
            if (count > 1)
            {
                throw new Exception($"There are multiple properties matching with type of 'ObjectStore<{entityType.Name}>'");
            }

            return null;
        }

        #region Initialization

        private void Initialize()
        {
            bool generateSchema = options.Schema == null;

            if (generateSchema)
            {
                options.Schema = new List<ObjectStoreSchema>();
            }

            var typePropertieMaps = new List<Tuple<Type, dynamic>>();
            var objectStoreGenericType = typeof(ObjectStore<>);
            var properties = GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == objectStoreGenericType)
                {
                    // Populate ObjectStore properties
                    var entityTypes = prop.PropertyType.GetGenericArguments();
                    var objectStoreType = objectStoreGenericType.MakeGenericType(entityTypes);

                    ObjectStoreSchema storeSchema;

                    if (generateSchema)
                    {
                        storeSchema = GetObjectStoreSchema(entityTypes[0], prop.Name);
                        options.Schema.Add(storeSchema);
                    }
                    else
                    {
                        storeSchema = options.Schema.Where(x => x.Name == prop.Name).FirstOrDefault();
                    }

                    var objectStore = Activator.CreateInstance(objectStoreType, prop.Name, js, options.IndexedDbAgentName, storeSchema?.AutoKeyProp);
                    prop.SetValue(this, objectStore);

                    typePropertieMaps.Add(Tuple.Create(entityTypes[0], objectStore));
                }
            }

            typeProperties = typePropertieMaps.ToLookup(x => x.Item1, x => x.Item2);
        }

        /// <summary>
        /// Gets the ObjectStore schema.
        /// </summary>
        /// <param name="objectStoreType">The type of ObjectStore.</param>
        /// <param name="name">The name of the ObjectStore.</param>
        /// <returns>The schema for the specify store.</returns>
        private ObjectStoreSchema GetObjectStoreSchema(Type objectStoreType, string name)
        {
            var autoIncrementAttributes = objectStoreType.GetCustomAttributes(typeof(AutoIncrementAttribute), false);
            bool autoIncrement = autoIncrementAttributes.Count() > 0;

            var properties = objectStoreType.GetProperties();
            PropertyInfo keyProp = null;
            List<IndexSchema> indexes = null;

            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];

                if (keyProp == null)
                {
                    var keyAttributes = property.GetCustomAttributes(typeof(KeyAttribute), false);

                    if (keyAttributes.Length > 0)
                    {
                        keyProp = property;
                    }
                }

                var indexAttributes = property.GetCustomAttributes(typeof(IndexAttribute), false);

                if (indexAttributes.Length > 0)
                {
                    if (indexes == null)
                    {
                        indexes = new List<IndexSchema>();
                    }

                    indexes.Add(new IndexSchema(property.Name.ToCamelCase()));
                }
            }

            if (keyProp == null && !autoIncrement)
            {
                throw new Exception($"[Key] or [AutoIncrement] not found in type: {objectStoreType.Name}.");
            }

            return new ObjectStoreSchema(name, keyProp, autoIncrement, indexes);
        }

        #endregion

        public async Task ClearDbAsync(string[] excludeStores = null)
        {
            var objectStoreGenericType = typeof(ObjectStore<>);
            var properties = GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == objectStoreGenericType && excludeStores?.Contains(prop.Name) != true)
                {
                    await js.InvokeVoidAsync($"{options.IndexedDbAgentName}.clear", prop.Name);
                }
            }
        }

        public async Task Close()
        {
            await js.InvokeVoidAsync($"{options.IndexedDbAgentName}.close");
            isOpen = false;
        }

        /// <summary>
        /// After deletion, a restart is needed to avoid this issue: https://github.com/learningequality/studio/issues/2094.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync()
        {
            if (isOpen)
            {
                await Close();
            }

            await js.InvokeVoidAsync(Constant.DeleteIndexedDb, options, DotNetObjectReference.Create(this));
        }

        private class NumberValue
        {
            public int Value { get; set; }
        }
    }
}
