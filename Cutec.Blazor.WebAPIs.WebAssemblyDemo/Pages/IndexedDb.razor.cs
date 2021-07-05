using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Cutec.Blazor.WebAPIs.WebAssemblyDemo.Pages
{
    public partial class IndexedDb : ComponentBase
    {
        [Inject] private DbContext db { get; set; }

        private List<ToDo> toDos;
        private ToDo toDo = new ToDo();
        private static int toDoId = 1;
        private int? toDoCount;
        private TaskItem task = new TaskItem();
        private List<TaskItem> tasks;

        protected override async Task OnInitializedAsync()
        {
            toDos = await db.ToDos.GetAllAsync();
            
            if (toDos.Count > 0)
            {
                toDoId = toDos.Max(x => x.Id) + 1;
            }

            var tasksStore = db.Store<TaskItem>();

            if (tasksStore != null)
            {
                tasks = await tasksStore.GetAllAsync();
            }

            await base.OnInitializedAsync();
        }

        private async Task SaveToDoAsync()
        {
            if (toDo.Id == 0)
            {
                toDo.Id = toDoId++;
            }

            await db.ToDos.PutAsync(toDo);
            toDo = new ToDo();
            toDos = await db.ToDos.GetAllAsync();
        }

        private async Task ViewToDoAsync(ToDo toDo)
        {
            this.toDo = await db.ToDos.GetByKeyAsync(toDo.Id);
        }

        private async Task DeleteToDoAsync(ToDo toDo)
        {
            await db.ToDos.DeleteByKeyAsync(toDo.Id);
            toDos = await db.ToDos.GetAllAsync();
        }

        private async Task MarkAsDoneAsync(ToDo toDo)
        {
            toDo.IsCompleted = 1;
            await db.ToDos.PutAsync(toDo);
            toDos = await db.ToDos.GetAllAsync();
        }

        private async Task GetToDoCountAsync()
        {
            toDoCount = await db.ToDos.CountAsync();
        }

        private async Task ClearToDosAsync()
        {
            await db.ToDos.ClearAsync();
            toDos = await db.ToDos.GetAllAsync();
        }

        private async Task GetPendingToDosAsync()
        {
            toDos = await db.ToDos.GetAllByIndexValueAsync(x => x.IsCompleted, 0);
        }

        private async Task AddTaskAsync()
        {
            var tasksStore = db.Store<TaskItem>();
            await tasksStore.PutAsync(new TaskItem { Name = task.Name });
            task.Name = null;
            tasks = await tasksStore.GetAllAsync();
        }

        private async Task GetAllInKeyRangeAsync()
        {
            toDos = await db.ToDos.GetAllAsync(null, false, 4, true, 2);
        }
    }
}
