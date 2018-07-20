using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ToDoApi;
using ToDoApi.Models;

namespace TodoApi.IntegrationTests
{
    [TestClass]
    public class IntegrationTest
    {
        private readonly TestServer server;
        private readonly HttpClient client;

        public IntegrationTest()
        {
            server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseEnvironment("Development"));
            client = server.CreateClient();
        }

        [TestMethod]
        public async Task AddListTest()
        {
            var list = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test",
                Description = "Test"
            };
            var response = await client.PostAsJsonAsync("api/Todo/lists", list);

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var addedList = JsonConvert.DeserializeObject<TodoList>(responseString);
            Assert.AreEqual(list.Id, addedList.Id);
            Assert.AreEqual(list.Name, addedList.Name);
            Assert.AreEqual(list.Description, addedList.Description);
        }

        [TestMethod]
        public async Task AddListAndTaskTest()
        {
            var list = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test",
                Description = "Test"
            };
            var response = await client.PostAsJsonAsync("api/Todo/lists", list);
            response.EnsureSuccessStatusCode();

            var task = new TodoTask
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test task",
                Completed = false
            };

            response = await client.PostAsJsonAsync($"api/Todo/list/{list.Id}/tasks", task);
            response.EnsureSuccessStatusCode();

            response = await client.GetAsync($"api/Todo/list/{list.Id}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var addedList = JsonConvert.DeserializeObject<TodoList>(responseString);
            Assert.AreEqual(list.Id, addedList.Id);
            Assert.AreEqual(1, addedList.Tasks.Count);
            var addedTask = addedList.Tasks.First();
            Assert.AreEqual(task.Id, addedTask.Id);
            Assert.AreEqual(task.Name, addedTask.Name);
            Assert.AreEqual(task.Completed, addedTask.Completed);
        }

        [TestMethod]
        public async Task AddListWithTaskAndSetCompletedTest()
        {
            var task = new TodoTask
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test task",
                Completed = false
            };

            var list = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test",
                Description = "Test",
                Tasks = new List<TodoTask>
                {
                    task
                }
            };
            var response = await client.PostAsJsonAsync("api/Todo/lists", list);
            response.EnsureSuccessStatusCode();

            var comleted = new Complete { Completed = true };
            response = await client.PostAsJsonAsync($"api/Todo/list/{list.Id}/task/{task.Id}/complete", comleted);
            response.EnsureSuccessStatusCode();

            response = await client.GetAsync($"api/Todo/list/{list.Id}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var addedList = JsonConvert.DeserializeObject<TodoList>(responseString);
            var addedTask = addedList.Tasks.First();
            Assert.AreEqual(list.Id, addedList.Id);
            Assert.AreEqual(1, addedList.Tasks.Count);
            Assert.AreEqual(task.Id, addedTask.Id);
            Assert.IsTrue(addedTask.Completed);
        }

        [TestMethod]
        public async Task AddDuplicatedListTest()
        {
            var list = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test",
                Description = "Test"
            };
            var response = await client.PostAsJsonAsync("api/Todo/lists", list);
            response.EnsureSuccessStatusCode();

            response = await client.PostAsJsonAsync("api/Todo/lists", list);
            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        }

        [TestMethod]
        public async Task AddTaskToWrongListTest()
        {
            var task = new TodoTask
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test task",
                Completed = false
            };

            var response = await client.PostAsJsonAsync($"api/Todo/list/1/tasks", task);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task AddWrongListTest()
        {
            var content = new StringContent(JsonConvert.SerializeObject(null), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"api/Todo/lists", content);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task GetListWithWrongIdTest()
        {
            var response = await client.GetAsync($"api/Todo/list/1");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task AddDuplicatedTaskTest()
        {
            var task = new TodoTask
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test",
                Completed = false
            };

            var list = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test",
                Description = "Test",
                Tasks = new List<TodoTask> { task }
            };
            var response = await client.PostAsJsonAsync("api/Todo/lists", list);
            response.EnsureSuccessStatusCode();

            response = await client.PostAsJsonAsync($"api/Todo/list/{list.Id}/tasks", task);
            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        }

        [TestMethod]
        public async Task GetListsTest()
        {
            var list = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test 1",
                Description = "Test 1"
            };
            var list2 = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test 2",
                Description = "Test 2"
            };
            var list3 = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            };

            var response = await client.PostAsJsonAsync("api/Todo/lists", list);
            response.EnsureSuccessStatusCode();
            response = await client.PostAsJsonAsync("api/Todo/lists", list2);
            response.EnsureSuccessStatusCode();

            response = await client.GetAsync("api/Todo/lists?searchString=Test");
            var responseString = await response.Content.ReadAsStringAsync();
            var lists = JsonConvert.DeserializeObject<List<TodoList>>(responseString);
            Assert.AreEqual(2, lists.Count);
        }
    }
}
