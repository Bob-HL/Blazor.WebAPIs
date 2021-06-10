namespace Cutec.Blazor.WebAPIs.WebAssemblyDemo
{
    public class DbContext : IndexedDb
    {
        public ObjectStore<ToDo> ToDos { get; set; }

        // uncomment the following line to test schema upgrade. Also need to increase the options.Version in Program
        public ObjectStore<TaskItem> TaskItems { get; set; }
    }
}
