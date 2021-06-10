# Blazor.WebAPIs
This is a Blazor library for accessing APIs provided by web browser: IndexedDB, window.localStorage...(more to come)

## To use the library
1. Instal NuGet package: ```Cutec.Blazor.WebAPIs```.
2. Add script reference to ```index.html```: ```<script src="_content/Cutec.Blazor.WebAPIs/Common.js"></script>```
 
# IndexedDB APIs
It is based on [idb](https://github.com/jakearchibald/idb).

Inherite from ```IndexedDb''':
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
- GetByKeyAsync, GetAllAsync, GetAllByIndexValueAsync
- GetAllKeysByIndexValueAsync, GetAllKeysAsync
- CountAsync

**Update**
- PutAsync
- PutListAsync

**Delete**
- DeleteByKeyAsync
- ClearAsync

# LocalStorage
