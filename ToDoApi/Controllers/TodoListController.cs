using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ToDoApi.Models;

namespace ToDoApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Todo")]
    public class TodoListController : Controller
    {
        private ITodoRepository repository;

        public TodoListController(ITodoRepository toDoRepository)
        {
            repository = toDoRepository;
        }

        [HttpGet("{id}", Name = "GetTodo")]
        [Route("list/{id}")]
        public IActionResult GetById(string id)
        {
            var item = repository.FindTodoList(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [Route("lists")]
        public IEnumerable<TodoList> GetAll(string searchString, int? skip, int? limit)
        {
            return repository.GetTodoLists(searchString, skip, limit);
        }

        [HttpPost]
        [Route("lists")]
        public IActionResult Create([FromBody] TodoList  todoList)
        {
            if (todoList == null)
            {
                return BadRequest();
            }
            if(repository.AddTodoList(todoList))
            {
                return CreatedAtRoute("GetTodo", new { id = todoList.Id }, todoList);
            }
            else
            {
                return new StatusCodeResult(409);
            }
        }

        [HttpPost]
        [Route("list/{id}/tasks")]
        public IActionResult AddTask(string id, [FromBody] TodoTask task)
        {
            if(task == null)
            {
                return BadRequest();
            }
            try
            {
                if (repository.AddTaskToTodoList(id, task))
                {
                    return new StatusCodeResult(201);
                }
                else
                {
                    return new StatusCodeResult(409);
                }
            }
            catch (KeyNotFoundException)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("list/{id}/task/{taskId}/complete")]
        public IActionResult UpdateComplete(string id, string taskId, [FromBody]Complete value)
        {
            if (repository.CompleteTask(id, taskId, value.Completed))
            {
                return new StatusCodeResult(201);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}