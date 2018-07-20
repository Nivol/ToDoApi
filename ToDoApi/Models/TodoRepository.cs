using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ToDoApi.Models
{
    public class TodoRepository : ITodoRepository
    {
        private readonly ConcurrentDictionary<string, TodoList> todoLists = new ConcurrentDictionary<string, TodoList>();

        public bool AddTaskToTodoList(string todoListId, TodoTask task)
        {
            if (string.IsNullOrEmpty(todoListId))
            {
                return false;
            }
            if (task == null)
            {
                return false;
            }

            if (todoLists.TryGetValue(todoListId, out TodoList todoList))
            {
                if(!todoList.Tasks.Exists(t => t.Id == task.Id))
                {
                    todoList.Tasks.Add(task);
                    return true;
                }
                return false;
            }
            else
            {
                throw new KeyNotFoundException("");
            }
        }

        public bool AddTodoList(TodoList todoList)
        {
            if (todoList == null)
            {
                return false;
            }
            return todoLists.TryAdd(todoList.Id, todoList);
        }

        public bool CompleteTask(string todoListId, string taskId, bool status)
        {
            if (string.IsNullOrEmpty(todoListId))
            {
                return false;
            }
            if (string.IsNullOrEmpty(taskId))
            {
                return false;
            }

            if (todoLists.TryGetValue(todoListId, out TodoList todoList))
            {
                var task = todoList.Tasks.Find(t => t.Id == taskId);
                if (task != null)
                {
                    task.Completed = status;
                    return true;
                }
            }
            return false;
        }

        public TodoList FindTodoList(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            todoLists.TryGetValue(id, out TodoList todoList);
            return todoList;
        }

        public IEnumerable<TodoList> GetTodoLists(string searchString, int? skip, int? limit)
        {
            var query = todoLists.Values.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(l => l.Name.Contains(searchString) || l.Description.Contains(searchString));
            }
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }
            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }
            return query.AsEnumerable();
        }
    }
}
