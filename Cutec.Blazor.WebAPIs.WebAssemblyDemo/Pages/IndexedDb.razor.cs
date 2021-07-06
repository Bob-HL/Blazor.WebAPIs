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
            //this.toDo = await db.ToDos.GetFirstFromIndexAsync(x => x.Priority, 3);
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

        #region Run Tests

        private async Task RunTestsAsync()
        {
            testItems = new List<TestItem>();

            toDoId = 1;
            await db.ToDos.ClearAsync();

            // prepare testing data
            await RunPutAndPutListAsyncTests();

            await RunGetByKeyAsyncTests();
            await RunGetFirstByKeyRangeAsyncTests();
            await RunGetFirstFromIndexAsyncTests();
            await RunGetAllAsyncTests();
            await RunGetAllFromIndexAsyncTests();
            await RunGetAllByIndexValueAsyncTests();
            await RunGetAllKeysAsyncTest();
            await RunGetAllKeysFromIndexAsyncTest();
            await RunGetAllKeysByIndexValueAsyncTests();
            await RunGetAllIndexValuesAsyncTests();
            await RunDeleteTests();

            toDos = await db.ToDos.GetAllAsync();
        }

        private async Task RunPutAndPutListAsyncTests()
        {
            for (var i = 1; i <= 5; i++)
            {
                var testTodo = new ToDo { Id = i, Name = $"Item {i}", Priority = 10 - i, IsCompleted = (i == 3 || i == 5) ? 1 : 0 };
                await db.ToDos.PutAsync(testTodo);
            }

            var passed = (await db.ToDos.CountAsync()) == 5;
            testItems.Add(new TestItem { Name = "PutAsync() & CountAsync()", Passed = passed });
            StateHasChanged();

            var testTodos = new List<ToDo>();

            for (var i = 6; i <= 10; i++)
            {
                testTodos.Add(new ToDo { Id = i, Name = $"Item {i}", Priority = 10 - i });
            }

            await db.ToDos.PutListAsync(testTodos);
            passed = (await db.ToDos.CountAsync()) == 10;
            testItems.Add(new TestItem { Name = "PutListAsync() & CountAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetByKeyAsyncTests()
        {
            var retrievedItem = await db.ToDos.GetByKeyAsync(2);
            var passed = retrievedItem?.Id == 2;

            if (passed)
            {
                retrievedItem = await db.ToDos.GetByKeyAsync(9);
                passed = retrievedItem?.Id == 9;
            }

            testItems.Add(new TestItem { Name = "GetByKeyAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetFirstByKeyRangeAsyncTests()
        {
            var retrievedItem = await db.ToDos.GetFirstByKeyRangeAsync(2);
            var passed = retrievedItem?.Id == 2;

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstByKeyRangeAsync(2, true);
                passed = retrievedItem?.Id == 3;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstByKeyRangeAsync(null, false, 7);
                passed = retrievedItem?.Id == 1;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstByKeyRangeAsync(null, false, 7, true);
                passed = retrievedItem?.Id == 1;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstByKeyRangeAsync(5, false, 7);
                passed = retrievedItem?.Id == 5;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstByKeyRangeAsync(5, true, 7, true);
                passed = retrievedItem?.Id == 6;
            }

            testItems.Add(new TestItem { Name = "GetFirstByKeyRangeAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetFirstFromIndexAsyncTests()
        {
            var retrievedItem = await db.ToDos.GetFirstFromIndexAsync(x => x.Priority, 3);
            var passed = retrievedItem?.Id == 7;

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstFromIndexAsync(x => x.Priority, 3, true);
                passed = retrievedItem?.Id == 6;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstFromIndexAsync(x => x.Priority, null, false, 7);
                passed = retrievedItem?.Id == 10;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstFromIndexAsync(x => x.Priority, null, false, 7, true);
                passed = retrievedItem?.Id == 10;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstFromIndexAsync(x => x.Priority, 4, false, 7);
                passed = retrievedItem?.Id == 6;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstFromIndexAsync(x => x.Priority, 4, true, 7);
                passed = retrievedItem?.Id == 5;
            }

            testItems.Add(new TestItem { Name = "GetFirstFromIndexAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllAsyncTests()
        {
            var retrievedItems = await db.ToDos.GetAllAsync();
            var passed = retrievedItems.Count == 10 && GotItemsWithIds(retrievedItems, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(8);
                passed = retrievedItems.Count == 3 && GotItemsWithIds(retrievedItems, new int[] { 8, 9, 10 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(8, true);
                passed = retrievedItems.Count == 2 && GotItemsWithIds(retrievedItems, new int[] { 9, 10 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(null, false, 4);
                passed = retrievedItems.Count == 4 && GotItemsWithIds(retrievedItems, new int[] { 1, 2, 3, 4 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(null, false, 4, true);
                passed = retrievedItems.Count == 3 && GotItemsWithIds(retrievedItems, new int[] { 1, 2, 3 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(3, false, 8);
                passed = retrievedItems.Count == 6 && GotItemsWithIds(retrievedItems, new int[] { 3, 4, 5, 6, 7, 8 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(3, true, 8, true);
                passed = retrievedItems.Count == 4 && GotItemsWithIds(retrievedItems, new int[] { 4, 5, 6, 7 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(3, false, 8, false, 2);
                passed = retrievedItems.Count == 2 && GotItemsWithIds(retrievedItems, new int[] { 2, 3 });
            }

            testItems.Add(new TestItem { Name = "GetAllAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllFromIndexAsyncTests()
        {
            //Priority >= 3, get id <= (10 - 3)
            var retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, 3);
            var passed = retrievedItems.Count == 7 && GotItemsWithIds(retrievedItems, new int[] { 7, 6, 5, 4, 3, 2, 1 });

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, 3, true);
                passed = retrievedItems.Count == 6 && GotItemsWithIds(retrievedItems, new int[] { 6, 5, 4, 3, 2, 1 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, null, false, 7);
                passed = retrievedItems.Count == 8 && GotItemsWithIds(retrievedItems, new int[] { 10, 9, 8, 7, 6, 5, 4, 3 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, null, false, 7, true);
                passed = retrievedItems.Count == 7 && GotItemsWithIds(retrievedItems, new int[] { 10, 9, 8, 7, 6, 5, 4 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, 3, false, 6);
                passed = retrievedItems.Count == 4 && GotItemsWithIds(retrievedItems, new int[] { 7, 6, 5, 4 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, 3, true, 6, true);
                passed = retrievedItems.Count == 2 && GotItemsWithIds(retrievedItems, new int[] { 6, 5 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, 3, false, 7, false, 2);
                passed = retrievedItems.Count == 2 && GotItemsWithIds(retrievedItems, new int[] { 7, 6 });
            }

            testItems.Add(new TestItem { Name = "GetAllFromIndexAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllByIndexValueAsyncTests()
        {
            var retrievedItems = await db.ToDos.GetAllByIndexValueAsync(x => x.Priority, 4);
            var passed = retrievedItems.Count == 1 && retrievedItems[0].Id == 6;

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllByIndexValueAsync(x => x.IsCompleted, 1);
                passed = retrievedItems.Count == 2 && GotItemsWithIds(retrievedItems, new int[] { 3, 5 });
            }

            testItems.Add(new TestItem { Name = "GetAllByIndexValueAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllKeysAsyncTest()
        {
            var retrievedIds = await db.ToDos.GetAllKeysAsync<int>();
            var passed = retrievedIds.Count == 10 && GotAllIds(retrievedIds, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(4);
                passed = retrievedIds.Count == 7 && GotAllIds(retrievedIds, new int[] { 4, 5, 6, 7, 8, 9, 10 });
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(4, true);
                passed = retrievedIds.Count == 6 && GotAllIds(retrievedIds, new int[] { 5, 6, 7, 8, 9, 10 });
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(null, false, 6);
                passed = retrievedIds.Count == 6 && GotAllIds(retrievedIds, new int[] { 1, 2, 3, 4, 5, 6 });
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(null, false, 6, true);
                passed = retrievedIds.Count == 5 && GotAllIds(retrievedIds, new int[] { 1, 2, 3, 4, 5 });
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(4, false, 6, false);
                passed = retrievedIds.Count == 3 && GotAllIds(retrievedIds, new int[] { 4, 5, 6 });
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(4, true, 6, true);
                passed = retrievedIds.Count == 1 && GotAllIds(retrievedIds, new int[] { 5 });
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(3, true, 7, false, 2);
                passed = retrievedIds.Count == 2 && GotAllIds(retrievedIds, new int[] { 4, 5 });
            }

            testItems.Add(new TestItem { Name = "GetAllKeysAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllKeysFromIndexAsyncTest()
        {
            var retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority);
            var passed = retrievedIds.Count == 10 && GotAllIds(retrievedIds, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, 4);
                passed = retrievedIds.Count == 6 && GotAllIds(retrievedIds, new int[] { 1, 2, 3, 4, 5, 6 });
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, 4, true);
                passed = retrievedIds.Count == 5 && GotAllIds(retrievedIds, new int[] { 1, 2, 3, 4, 5 });
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, null, false, 6);
                passed = retrievedIds.Count == 7 && GotAllIds(retrievedIds, new int[] { 4, 5, 6, 7, 8, 9, 10 });
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, null, false, 6, true);
                passed = retrievedIds.Count == 6 && GotAllIds(retrievedIds, new int[] { 5, 6, 7, 8, 9, 10 });
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, 4, false, 6, false);
                passed = retrievedIds.Count == 3 && GotAllIds(retrievedIds, new int[] { 4, 5, 6 });
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, 4, true, 6, true);
                passed = retrievedIds.Count == 1 && GotAllIds(retrievedIds, new int[] { 5 });
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, 3, true, 7, false, 2);
                passed = retrievedIds.Count == 2 && GotAllIds(retrievedIds, new int[] { 4, 5 });
            }

            testItems.Add(new TestItem { Name = "GetAllKeysFromIndexAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllKeysByIndexValueAsyncTests()
        {
            var retrievedIds = await db.ToDos.GetAllKeysByIndexValueAsync<int, int>(x => x.Priority, 4);
            var passed = retrievedIds.Count == 1 && retrievedIds[0] == 6;

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysByIndexValueAsync<int, int>(x => x.IsCompleted, 1);
                passed = retrievedIds.Count == 2 && GotAllIds(retrievedIds, new int[] { 3, 5 });
            }

            testItems.Add(new TestItem { Name = "GetAllKeysByIndexValueAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllIndexValuesAsyncTests()
        {
            var retrievedIds = await db.ToDos.GetAllIndexValuesAsync(x => x.Priority);
            var passed = retrievedIds.Count == 10 && GotAllIds(retrievedIds, new int[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 });

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllIndexValuesAsync(x => x.IsCompleted);
                passed = retrievedIds.Count == 2 && GotAllIds(retrievedIds, new int[] { 0, 1 });
            }

            testItems.Add(new TestItem { Name = "GetAllIndexValuesAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunDeleteTests()
        {
            var todoStore = db.ToDos;
            await todoStore.DeleteByKeyAsync(1);
            var passed = (await todoStore.CountAsync()) == 9 && (await todoStore.GetByKeyAsync(1)) == null;
            testItems.Add(new TestItem { Name = "DeleteByKeyAsync()", Passed = passed });
            StateHasChanged();

            if (passed)
            {
                var ids = new List<int>() { 5, 6, 7 };
                await todoStore.DeleteByKeyListAsync(ids);
                passed = (await todoStore.CountAsync()) == 6 && (await todoStore.GetByKeyAsync(5)) == null;
                testItems.Add(new TestItem { Name = "DeleteByKeyListAsync()", Passed = passed });
                StateHasChanged();
            }

            if (passed)
            {
                await todoStore.ClearAsync();
                passed = (await todoStore.CountAsync()) == 0 && (await todoStore.GetByKeyAsync(2)) == null;
                testItems.Add(new TestItem { Name = "ClearAsync()", Passed = passed });
                StateHasChanged();
            }
        }

        private bool GotItemsWithIds(List<ToDo> retrievedItems, int[] ids)
        {
            for (var i = 1; i < ids.Length; i++)
            {
                if (!retrievedItems.Exists(x => x.Id == ids[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool GotAllIds(List<int> retrievedIds, int[] ids)
        {
            for (var i = 1; i < ids.Length; i++)
            {
                if (!retrievedIds.Contains(ids[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private List<TestItem> testItems;

        private class TestItem
        {
            public string Name { get; set; }
            public bool Passed { get; set; }
        }

        #endregion
    }
}
