using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cutec.Blazor.WebAPIs.WebAssemblyDemo.Pages
{
    public partial class IndexedDb : ComponentBase
    {
        [Inject] private DbContext db { get; set; }
        [Inject] private NavigationManager navigationManager { get; set; }

        private List<ToDo> toDos;
        private ToDo toDo = new ToDo();
        private int? toDoCount;
        private TaskItem task = new TaskItem();
        private List<TaskItem> tasks;

        private EditContext getKeyEditContext;
        private ToDo getKeyModel = new ToDo();
        private object existingKey;
        private bool displayExistingKey;
        private int startId;

        protected override async Task OnInitializedAsync()
        {
            if (!db.IsOpen)
            {
                await db.OpenAsync();
            }

            toDos = await db.ToDos.GetAllAsync();

            var tasksStore = db.Store<TaskItem>();

            if (tasksStore != null)
            {
                tasks = await tasksStore.GetAllAsync();
            }

            getKeyEditContext = new EditContext(getKeyModel);
            getKeyEditContext.OnFieldChanged += (s, e) => displayExistingKey = false;

            await base.OnInitializedAsync();
        }

        private async Task SaveToDoAsync()
        {
            await db.ToDos.PutAsync(toDo);
            Console.WriteLine($"Generated ID: {toDo.Id}");
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

        private async Task GetFirstKeyAsync()
        {
            existingKey = await db.ToDos.GetFirstKeyAsync(getKeyModel.Id);
            displayExistingKey = true;
        }

        private async Task DeleteDbAsync()
        {
            await db.DeleteAsync();
            navigationManager.NavigateTo("/");
        }

        #region Run Tests

        private async Task RunTestsAsync()
        {
            testItems = new List<TestItem>();

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
                var testTodo = new ToDo { Name = $"Item {i}", Priority = 10 - i, IsCompleted = (i == 3 || i == 5) ? 1 : 0 };
                await db.ToDos.PutAsync(testTodo);

                if (i == 1)
                {
                    startId = testTodo.Id;
                }
            }

            var passed = (await db.ToDos.CountAsync()) == 5;
            testItems.Add(new TestItem { Name = "PutAsync() & CountAsync()", Passed = passed });
            StateHasChanged();

            var testTodos = new List<ToDo>();

            for (var i = 6; i <= 10; i++)
            {
                testTodos.Add(new ToDo { Name = $"Item {i}", Priority = 10 - i });
            }

            await db.ToDos.PutListAsync(testTodos);
            passed = (await db.ToDos.CountAsync()) == 10;
            testItems.Add(new TestItem { Name = "PutListAsync() & CountAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetByKeyAsyncTests()
        {
            var retrievedItem = await db.ToDos.GetByKeyAsync(startId);
            var passed = retrievedItem?.Id == startId;

            if (passed)
            {
                retrievedItem = await db.ToDos.GetByKeyAsync(startId + 5);
                passed = retrievedItem?.Id == startId + 5;
            }

            testItems.Add(new TestItem { Name = "GetByKeyAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetFirstByKeyRangeAsyncTests()
        {
            var retrievedItem = await db.ToDos.GetFirstByKeyRangeAsync(startId);
            var passed = retrievedItem?.Id == startId;

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstByKeyRangeAsync(startId, true);
                passed = retrievedItem?.Id == startId + 1;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstByKeyRangeAsync(null, false, startId + 6);
                passed = retrievedItem?.Id == startId;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstByKeyRangeAsync(null, false, startId + 6, true);
                passed = retrievedItem?.Id == startId;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstByKeyRangeAsync(startId + 4, false, startId + 6);
                passed = retrievedItem?.Id == startId + 4;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstByKeyRangeAsync(startId + 4, true, startId + 6, true);
                passed = retrievedItem?.Id == startId + 5;
            }

            testItems.Add(new TestItem { Name = "GetFirstByKeyRangeAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetFirstFromIndexAsyncTests()
        {
            var retrievedItem = await db.ToDos.GetFirstFromIndexAsync(x => x.Priority, 3);
            var passed = retrievedItem?.Id == startId + 6;

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstFromIndexAsync(x => x.Priority, 3, true);
                passed = retrievedItem?.Id == startId + 5;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstFromIndexAsync(x => x.Priority, null, false, 7);
                passed = retrievedItem?.Id == startId + 9;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstFromIndexAsync(x => x.Priority, null, false, 7, true);
                passed = retrievedItem?.Id == startId + 9;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstFromIndexAsync(x => x.Priority, 4, false, 7);
                passed = retrievedItem?.Id == startId + 5;
            }

            if (passed)
            {
                retrievedItem = await db.ToDos.GetFirstFromIndexAsync(x => x.Priority, 4, true, 7);
                passed = retrievedItem?.Id == startId + 4;
            }

            testItems.Add(new TestItem { Name = "GetFirstFromIndexAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllAsyncTests()
        {
            var retrievedItems = await db.ToDos.GetAllAsync();
            var ids = new int[10];

            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = startId + i;
            }

            var passed = retrievedItems.Count == 10 && GotItemsWithIds(retrievedItems, ids);

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(startId + 7);
                var expectedIds = new int[3];
                Array.Copy(ids, 7, expectedIds, 0, expectedIds.Length);
                passed = retrievedItems.Count == 3 && GotItemsWithIds(retrievedItems, expectedIds);
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(startId + 7, true);
                var expectedIds = new int[2];
                Array.Copy(ids, 8, expectedIds, 0, expectedIds.Length);
                passed = retrievedItems.Count == 2 && GotItemsWithIds(retrievedItems, expectedIds);
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(null, false, startId + 3);
                var expectedIds = new int[4];
                Array.Copy(ids, 0, expectedIds, 0, expectedIds.Length);
                passed = retrievedItems.Count == 4 && GotItemsWithIds(retrievedItems, expectedIds);
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(null, false, startId + 3, true);
                var expectedIds = new int[3];
                Array.Copy(ids, 0, expectedIds, 0, expectedIds.Length);
                passed = retrievedItems.Count == 3 && GotItemsWithIds(retrievedItems, expectedIds);
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(startId + 2, false, startId + 7);
                var expectedIds = new int[6];
                Array.Copy(ids, 2, expectedIds, 0, expectedIds.Length);
                passed = retrievedItems.Count == 6 && GotItemsWithIds(retrievedItems, expectedIds);
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(startId + 2, true, startId + 7, true);
                var expectedIds = new int[4];
                Array.Copy(ids, 3, expectedIds, 0, expectedIds.Length);
                passed = retrievedItems.Count == 4 && GotItemsWithIds(retrievedItems, expectedIds);
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllAsync(startId + 2, false, startId + 7, false, 2);
                var expectedIds = new int[2];
                Array.Copy(ids, 1, expectedIds, 0, expectedIds.Length);
                passed = retrievedItems.Count == 2 && GotItemsWithIds(retrievedItems, expectedIds);
            }

            testItems.Add(new TestItem { Name = "GetAllAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllFromIndexAsyncTests()
        {
            //Priority >= 3, get id <= (10 - 3)
            var retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, 3);
            var passed = retrievedItems.Count == 7 && GotItemsWithIds(retrievedItems, new int[] { startId + 6, startId + 5, startId + 4, startId + 3, startId + 2, startId + 1, startId });

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, 3, true);
                passed = retrievedItems.Count == 6 && GotItemsWithIds(retrievedItems, new int[] { startId + 5, startId + 4, startId + 3, startId + 2, startId + 1, startId });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, null, false, 7);
                passed = retrievedItems.Count == 8 && GotItemsWithIds(retrievedItems, new int[] { startId + 9, startId + 8, startId + 7, startId + 6, startId + 5, startId + 4, startId + 3, startId + 2 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, null, false, 7, true);
                passed = retrievedItems.Count == 7 && GotItemsWithIds(retrievedItems, new int[] { startId + 9, startId + 8, startId + 7, startId + 6, startId + 5, startId + 4, startId + 3 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, 3, false, 6);
                passed = retrievedItems.Count == 4 && GotItemsWithIds(retrievedItems, new int[] { startId + 6, startId + 5, startId + 4, startId + 3 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, 3, true, 6, true);
                passed = retrievedItems.Count == 2 && GotItemsWithIds(retrievedItems, new int[] { startId + 5, startId + 4 });
            }

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllFromIndexAsync(x => x.Priority, 3, false, 7, false, 2);
                passed = retrievedItems.Count == 2 && GotItemsWithIds(retrievedItems, new int[] { startId + 6, startId + 5 });
            }

            testItems.Add(new TestItem { Name = "GetAllFromIndexAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllByIndexValueAsyncTests()
        {
            var retrievedItems = await db.ToDos.GetAllByIndexValueAsync(x => x.Priority, 4);
            var passed = retrievedItems.Count == 1 && retrievedItems[0].Id == startId + 5;

            if (passed)
            {
                retrievedItems = await db.ToDos.GetAllByIndexValueAsync(x => x.IsCompleted, 1);
                passed = retrievedItems.Count == 2 && GotItemsWithIds(retrievedItems, new int[] { startId + 2, startId + 4 });
            }

            testItems.Add(new TestItem { Name = "GetAllByIndexValueAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllKeysAsyncTest()
        {
            var ids = new int[10];

            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = startId + i;
            }

            var retrievedIds = await db.ToDos.GetAllKeysAsync<int>();
            var passed = retrievedIds.Count == 10 && GotAllIds(retrievedIds, ids);

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(startId + 3);
                var expectedIds = new int[7];
                Array.Copy(ids, 3, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 7 && GotAllIds(retrievedIds, expectedIds);
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(startId + 3, true);
                var expectedIds = new int[6];
                Array.Copy(ids, 4, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 6 && GotAllIds(retrievedIds, expectedIds);
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(null, false, startId + 5);
                var expectedIds = new int[6];
                Array.Copy(ids, 0, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 6 && GotAllIds(retrievedIds, expectedIds);
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(null, false, startId + 5, true);
                var expectedIds = new int[5];
                Array.Copy(ids, 0, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 5 && GotAllIds(retrievedIds, expectedIds);
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(startId + 3, false, startId + 5, false);
                var expectedIds = new int[3];
                Array.Copy(ids, 3, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 3 && GotAllIds(retrievedIds, expectedIds);
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(startId + 3, true, startId + 5, true);
                var expectedIds = new int[1];
                Array.Copy(ids, 4, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 1 && GotAllIds(retrievedIds, expectedIds);
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysAsync<int>(startId + 2, true, startId + 6, false, 2);
                var expectedIds = new int[2];
                Array.Copy(ids, 3, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 2 && GotAllIds(retrievedIds, expectedIds);
            }

            testItems.Add(new TestItem { Name = "GetAllKeysAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllKeysFromIndexAsyncTest()
        {
            var ids = new int[10];

            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = startId + i;
            }

            var retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority);
            var passed = retrievedIds.Count == 10 && GotAllIds(retrievedIds, ids);

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, 4);
                var expectedIds = new int[6];
                Array.Copy(ids, 0, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 6 && GotAllIds(retrievedIds, expectedIds);
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, 4, true);
                var expectedIds = new int[5];
                Array.Copy(ids, 0, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 5 && GotAllIds(retrievedIds, expectedIds);
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, null, false, 6);
                var expectedIds = new int[7];
                Array.Copy(ids, 3, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 7 && GotAllIds(retrievedIds, expectedIds);
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, null, false, 6, true);
                var expectedIds = new int[6];
                Array.Copy(ids, 4, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 6 && GotAllIds(retrievedIds, expectedIds);
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, 4, false, 6, false);
                var expectedIds = new int[3];
                Array.Copy(ids, 3, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 3 && GotAllIds(retrievedIds, expectedIds);
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, 4, true, 6, true);
                var expectedIds = new int[1];
                Array.Copy(ids, 4, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 1 && GotAllIds(retrievedIds, expectedIds);
            }

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysFromIndexAsync<int, int>(x => x.Priority, 3, true, 7, false, 2);
                var expectedIds = new int[2];
                Array.Copy(ids, 3, expectedIds, 0, expectedIds.Length);
                passed = retrievedIds.Count == 2 && GotAllIds(retrievedIds, expectedIds);
            }

            testItems.Add(new TestItem { Name = "GetAllKeysFromIndexAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllKeysByIndexValueAsyncTests()
        {
            var retrievedIds = await db.ToDos.GetAllKeysByIndexValueAsync<int, int>(x => x.Priority, 4);
            var passed = retrievedIds.Count == 1 && retrievedIds[0] == startId + 5;

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllKeysByIndexValueAsync<int, int>(x => x.IsCompleted, 1);
                passed = retrievedIds.Count == 2 && GotAllIds(retrievedIds, new int[] { startId + 2, startId + 4 });
            }

            testItems.Add(new TestItem { Name = "GetAllKeysByIndexValueAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunGetAllIndexValuesAsyncTests()
        {
            var ids = new int[10];

            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = 9 - i;
            }

            var retrievedIds = await db.ToDos.GetAllIndexValuesAsync(x => x.Priority);
            var passed = retrievedIds.Count == 10 && GotAllIds(retrievedIds, ids);

            if (passed)
            {
                retrievedIds = await db.ToDos.GetAllIndexValuesAsync(x => x.IsCompleted);
                passed = retrievedIds.Count == 2 && GotAllIds(retrievedIds, new int[] { 0, 1});
            }

            testItems.Add(new TestItem { Name = "GetAllIndexValuesAsync()", Passed = passed });
            StateHasChanged();
        }

        private async Task RunDeleteTests()
        {
            var todoStore = db.ToDos;
            await todoStore.DeleteByKeyAsync(startId);
            var passed = (await todoStore.CountAsync()) == 9 && (await todoStore.GetByKeyAsync(startId)) == null;
            testItems.Add(new TestItem { Name = "DeleteByKeyAsync()", Passed = passed });
            StateHasChanged();

            if (passed)
            {
                var ids = new List<int>() { startId + 4, startId + 5, startId + 6 };
                await todoStore.DeleteByKeyListAsync(ids);
                passed = (await todoStore.CountAsync()) == 6 && (await todoStore.GetByKeyAsync(startId + 4)) == null;
                testItems.Add(new TestItem { Name = "DeleteByKeyListAsync()", Passed = passed });
                StateHasChanged();
            }

            if (passed)
            {
                await todoStore.ClearAsync();
                passed = (await todoStore.CountAsync()) == 0 && (await todoStore.GetByKeyAsync(startId + 1)) == null;
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
