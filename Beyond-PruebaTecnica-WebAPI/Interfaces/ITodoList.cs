using Beyond_PruebaTecnica_WebAPI.DTOs.Output;
using Beyond_PruebaTecnica_WebAPI.Models;

namespace Beyond_PruebaTecnica_WebAPI.DTOs
{
    public interface ITodoList
    {
        void AddItem(int id, string title, string description, string category);
        void UpdateItem(int id, string title, string description, string category);
        void RemoveItem(int id);
        void RegisterProgression(int id, DateTime dateTime, decimal percent);
        void PrintItems();
        TodoItem Find(int id);
        List<TodoItem> GetAllItems();
        TodoItemDto MapToDto(TodoItem item);
    }
}
