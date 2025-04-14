using Beyond_PruebaTecnica_ConsoleApp.Interfaces;
using Beyond_PruebaTecnica_WebAPI.DTOs;
using Beyond_PruebaTecnica_WebAPI.DTOs.Input;
using Beyond_PruebaTecnica_WebAPI.DTOs.Output;
using Microsoft.AspNetCore.Mvc;

namespace Beyond_PruebaTecnica_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoListController : ControllerBase
    {
        private readonly ITodoList todoList;
        private readonly ITodoListRepository repository;

        public TodoListController(ITodoList todoList, ITodoListRepository repository)
        {
            this.todoList = todoList;
            this.repository = repository;
        }

        [HttpGet()]
        public ActionResult<IEnumerable<TodoItemDto>> Get()
        {
            var result = todoList.GetAllItems().Select(item => todoList.MapToDto(item));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var item = todoList.Find(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] CreateTodoItemDto dto)
        {
            try
            {
                var id = repository.GetNextId();
                todoList.AddItem(id, dto.Title, dto.Description, dto.Category);

                foreach (var item in dto.Progressions)
                {
                    todoList.RegisterProgression(id, item.Date, item.Percent);
                }

                var addedItem = todoList.Find(id);
                var dtoToReturn = todoList.MapToDto(addedItem);

                return Ok(dtoToReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] CreateTodoItemDto dto)
        {
            try
            {
                var existingItem = todoList.Find(id);
                if (existingItem is null)
                    return NotFound($"El ítem con el id {id} no ha sido encontrado.");

                todoList.UpdateItem(id, dto.Title, dto.Description, dto.Category);
                existingItem.Progressions.Clear();

                foreach (var p in dto.Progressions)
                {
                    todoList.RegisterProgression(id, p.Date, p.Percent);
                }

                var updatedItem = todoList.Find(id);
                return Ok(todoList.MapToDto(updatedItem));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Este PUT es sólo para agregar una progresión a un ítem ya existente
        [HttpPut("{id}/add_progression")]
        public IActionResult Put_AddProgression(int id, [FromBody] AddProgressionDto dto)
        {
            try
            {
                var item = todoList.Find(id);
                if (item is null)
                    return NotFound($"El ítem con el id {id} no ha sido encontrado.");

                todoList.RegisterProgression(id, dto.Date, dto.Percent);

                var updatedItem = todoList.Find(id);
                return Ok(todoList.MapToDto(updatedItem));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] UpdateTodoItemDto dto)
        {
            try
            {
                var existingItem = todoList.Find(id);
                if (existingItem is null)
                    return NotFound($"El ítem con el id {id} no ha sido encontrado.");

                if (!string.IsNullOrWhiteSpace(dto.Title))
                    existingItem.UpdateTitle(dto.Title);

                if (!string.IsNullOrWhiteSpace(dto.Description))
                    existingItem.UpdateDescription(dto.Description);

                if (!string.IsNullOrWhiteSpace(dto.Category))
                    existingItem.UpdateCategory(dto.Category);

                if (dto.Progressions is not null)
                {
                    existingItem.Progressions.Clear();
                    foreach (var p in dto.Progressions)
                    {
                        todoList.RegisterProgression(id, p.Date, p.Percent);
                    }
                }

                var updatedItem = todoList.Find(id);
                return Ok(todoList.MapToDto(updatedItem));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                todoList.RemoveItem(id);
                return Ok($"El ítem con id {id} ha sido removido con éxito.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
