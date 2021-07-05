namespace Cutec.Blazor.WebAPIs.WebAssemblyDemo
{
    public class ToDo
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        // Indexed DB doesn't support boolean data type index.
        [Index]
        public int IsCompleted { get; set; }
    }
}
