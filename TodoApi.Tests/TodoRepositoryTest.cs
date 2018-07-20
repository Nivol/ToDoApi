using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ToDoApi.Models;
using System.Collections.Generic;
using System;

namespace TodoApi.Tests
{
    [TestClass]
    public class TodoRepositoryTest
    {
        [TestMethod]
        public void AddTodoListTest()
        {
            var repo = new TodoRepository();
            var todoList = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            };
            var result = repo.AddTodoList(todoList);
            Assert.IsTrue(result);

            var repList = repo.FindTodoList(todoList.Id);
            Assert.IsNotNull(repList);
            Assert.AreEqual(todoList.Id, repList.Id);
            Assert.AreEqual(todoList.Name, repList.Name);
            Assert.AreEqual(todoList.Description, repList.Description);
        }

        [TestMethod]
        public void AddTaskToTodoListTest()
        {
            var repo = new TodoRepository();
            var todoList  = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            };
            var result = repo.AddTodoList(todoList);
            Assert.IsTrue(result);
            var todoTask = new TodoTask
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Task name",
                Completed = false
            };
            result = repo.AddTaskToTodoList(todoList.Id, todoTask);
            Assert.IsTrue(result);

            var repList = repo.FindTodoList(todoList.Id);
            Assert.IsNotNull(repList);

            var repTask = repList.Tasks.First();
            Assert.AreEqual(todoTask.Id, repTask.Id);
            Assert.AreEqual(todoTask.Name, repTask.Name);
            Assert.AreEqual(todoTask.Completed, repTask.Completed);
        }

        [TestMethod]
        public void AddDuplicateTaskToTodoListTest()
        {
            var repo = new TodoRepository();
            var todoList = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            };
            repo.AddTodoList(todoList);
            var todoTask = new TodoTask
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Task name",
                Completed = false
            };
            repo.AddTaskToTodoList(todoList.Id, todoTask);
            var result = repo.AddTaskToTodoList(todoList.Id, todoTask);
            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void AddTaskToWrongTodoListTest()
        {
            var repo = new TodoRepository();
            var todoList = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            };
            repo.AddTodoList(todoList);
            var todoTask = new TodoTask
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Task name",
                Completed = false
            };
            repo.AddTaskToTodoList("111", todoTask);
        }

        [TestMethod]
        public void CompleteTaskTest()
        {
            var repo = new TodoRepository();
            var todoList = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            };
            repo.AddTodoList(todoList);
            var todoTask = new TodoTask
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Task name",
                Completed = false
            };
            repo.AddTaskToTodoList(todoList.Id, todoTask);
            var result = repo.CompleteTask(todoList.Id, todoTask.Id, true);
            Assert.IsTrue(result);
            var repList = repo.FindTodoList(todoList.Id);
            Assert.IsNotNull(repList);
            var repTask = repList.Tasks.First();
            Assert.IsTrue(repTask.Completed);
        }

        [TestMethod]
        public void CompleteTaskWithWrongTodoListIdTest()
        {
            var repo = new TodoRepository();
            var todoList = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            };
            repo.AddTodoList(todoList);
            var todoTask = new TodoTask
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Task name",
                Completed = false
            };
            repo.AddTaskToTodoList(todoList.Id, todoTask);
            var result = repo.CompleteTask("1", todoTask.Id, true);
            Assert.IsFalse(result);

            var repList = repo.FindTodoList(todoList.Id);
            Assert.IsNotNull(repList);
            var repTask = repList.Tasks.First();
            Assert.IsFalse(repTask.Completed);
        }

        [TestMethod]
        public void CompleteTaskWithWrongTodoTaskIdTest()
        {
            var repo = new TodoRepository();
            var todoList = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            };
            repo.AddTodoList(todoList);
            var todoTask = new TodoTask
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Task name",
                Completed = false
            };
            repo.AddTaskToTodoList(todoList.Id, todoTask);
            var result = repo.CompleteTask(todoList.Id, "1", true);
            Assert.IsFalse(result);

            var repList = repo.FindTodoList(todoList.Id);
            Assert.IsNotNull(repList);
            var repTask = repList.Tasks.First();
            Assert.IsFalse(repTask.Completed);
        }

        [TestMethod]
        public void FindTodoListTest()
        {
            var repo = new TodoRepository();
            var todoList = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            };
            repo.AddTodoList(todoList);

            var repList = repo.FindTodoList(todoList.Id);
            Assert.IsNotNull(repList);
            Assert.AreEqual(todoList.Id, repList.Id);
        }

        [TestMethod]
        public void GetTodoListsWithoutParametrsTest()
        {
            var repo = new TodoRepository();
            repo.AddTodoList(new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            });
            repo.AddTodoList(new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            });

            var items = repo.GetTodoLists("", null, null);
            Assert.AreEqual(2, items.Count());
        }

        [TestMethod]
        public void GetTodoListsWithSearchTest()
        {
            var repo = new TodoRepository();
            var todoList = new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            };
            repo.AddTodoList(todoList);
            repo.AddTodoList(new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "List",
                Description = "Description"
            });

            var items = repo.GetTodoLists("Name", null, null);

            Assert.AreEqual(1, items.Count());
            Assert.AreEqual(todoList.Id, items.First().Id);
        }

        [TestMethod]
        public void GetTodoListWithSkipTest()
        {
            var repo = new TodoRepository();
            repo.AddTodoList(new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            });
            repo.AddTodoList(new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            });

            var items = repo.GetTodoLists("", 2, null);

            Assert.AreEqual(0, items.Count());
        }

        [TestMethod]
        public void GetTodoListWithLimitTest()
        {
            var repo = new TodoRepository();
            repo.AddTodoList(new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            });
            repo.AddTodoList(new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            });
            repo.AddTodoList(new TodoList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Description = "Description"
            });

            var items = repo.GetTodoLists("", null, 2);

            Assert.AreEqual(2, items.Count());
        }
    }
}
