using System.Collections.Generic;

namespace ToDoApi.Models
{
    public interface ITodoRepository
    {
        IEnumerable<TodoList> GetTodoLists(string searchString, int? skip, int? limit);
        bool AddTodoList(TodoList todoList);
        TodoList FindTodoList(string id);
        bool AddTaskToTodoList(string todoListId, TodoTask task);
        bool CompleteTask(string todoListId, string taskId, bool status);
    }
}
