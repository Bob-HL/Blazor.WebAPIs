# Blazor.WebAPIs
This is a Blazor library for accessing APIs provided by web browser: IndexedDB, window.localStorage...(more to come)

## To use the library
1. Instal NuGet package: ```Cutec.Blazor.WebAPIs```.
2. Add script reference to ```index.html```: ```<script src="_content/Cutec.Blazor.WebAPIs/All.js"></script>```
3. Register services; ```builder.Services.AddBlazorWebAPIs();```. Alternatively, you can register the needed indivitial API, ex. ```builder.Services.AddWebStorage();```
 
# IndexedDB APIs
It is based on [idb](https://github.com/jakearchibald/idb).

Inherit from ```IndexedDb```:
```CSharp
public class DbContext : IndexedDb
{
    public ObjectStore<ToDo> ToDos { get; set; }
}    
```

In Program.cs
```CSharp
builder.Services.AddIndexedDB<DbContext>();

var host = builder.Build();

await host.Services.UseIndexedDbAsync<DbContext>(options =>
{
  options.Name = "MyDb";
  options.Version = 1;
});
```

Inject DbContent into Components:
```CSharp
[Inject] private DbContext db { get; set; }

...

var toDos = await db.ToDos.GetAllAsync();
```

**Retrieving**
- GetByKeyAsync, GetFirstByKeyRangeAsync, GetFirstFromIndexAsync
- GetAllAsync, GetAllFromIndexAsync, GetAllByIndexValueAsync
- GetAllKeysAsync, GetAllKeysFromIndexAsync, GetAllKeysByIndexValueAsync
- GetAllIndexValuesAsync
- CountAsync

**Update**
- PutAsync
- PutListAsync

**Delete**
- DeleteByKeyAsync
- DeleteByKeyListAsync
- ClearAsync

# LocalStorage
Inject ```LocalStorage```:
```CSharp
[Inject] private LocalStorage localStorage { get; set; }
```

**APIs**
```CSharp
int Length
string Key(int index)
string GetItem(string keyName)
void SetItem(string keyName, string keyValue)
void RemoveItem(string keyName)
void Clear()

T GetItem<T>(string keyName) where T : class
void SetItem<T>(string keyName, T item) where T : class
```
# Geolocation
Inject and use ```Geolocation```.
