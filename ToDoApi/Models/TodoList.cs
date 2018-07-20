using System.Collections.Generic;

namespace ToDoApi.Models
{
    public class TodoList
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TodoTask> Tasks { get; set; }

        public TodoList()
        {
            Tasks = new List<TodoTask>();
        }
    }
}
