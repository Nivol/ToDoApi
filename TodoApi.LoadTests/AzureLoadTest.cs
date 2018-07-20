using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ToDoApi.Models;

namespace TodoApi.LoadTests
{
    [TestClass]
    public class AzureLoadTest
    {
        private int success;
        private int fail;

        [TestMethod]
        public async Task AddListsTest()
        {
            var initialCount = await GetListCount();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 200; i++)
            {
                int idx = i;
                tasks.Add(Task.Run(() => AddListToAzure(idx)));
            }
            Task.WaitAll(tasks.ToArray());
            var listCount = await GetListCount();
            Assert.AreEqual(success-fail, listCount-initialCount);
        }

        private async Task<int> GetListCount()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(@"http://test9087.azurewebsites.net")
            };
            var response = await client.GetAsync("api/Todo/lists");
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            var lists = JsonConvert.DeserializeObject<List<TodoList>>(jsonString);
            return lists.Count;
        }

        private async void AddListToAzure(int idx)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(@"http://test9087.azurewebsites.net")
            };

            var list = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"Test {idx}",
                Description = Thread.CurrentThread.ManagedThreadId.ToString()
            };

            var response = await client.PostAsJsonAsync("api/Todo/lists", list);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                Interlocked.Increment(ref success);
            }
            else
            {
                Interlocked.Increment(ref fail);
            }
        }
    }
}
