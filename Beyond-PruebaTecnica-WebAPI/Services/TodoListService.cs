using Beyond_PruebaTecnica_ConsoleApp.Interfaces;
using Beyond_PruebaTecnica_WebAPI.DTOs;
using Beyond_PruebaTecnica_WebAPI.DTOs.Output;
using Beyond_PruebaTecnica_WebAPI.Exceptions;
using Beyond_PruebaTecnica_WebAPI.Models;

namespace Beyond_PruebaTecnica_WebAPI.Services
{
    public class TodoListService : ITodoList
    {
        private readonly List<TodoItem> items = new();
        private readonly ITodoListRepository repository;
        private const string Separator = "-----------------------------------------------------------------------";

        public TodoListService(ITodoListRepository repository)
        {
            this.repository = repository;
        }

        public void AddItem(int id, string title, string description, string category)
        {
            if (!repository.GetAllCategories().Contains(category))
                throw new InvalidCategoryException();

            items.Add(new TodoItem(id, title, description, category));
        }

        public void UpdateItem(int id, string title, string description, string category)
        {
            var item = Find(id);

            if (title != item.Title)
                item.Title = title;

            if (description != item.Description)
                item.UpdateDescription(description);

            if (category != item.Category)
                item.Category = category;
        }

        public void RemoveItem(int id)
        {
            var item = Find(id);
            if (item.TotalProgress > 50)
                throw new RemoveNotAllowedException();

            items.Remove(item);
        }

        public void RegisterProgression(int id, DateTime dateTime, decimal percent)
        {
            var item = Find(id);
            item.AddProgression(dateTime, percent);           
        }

        public void PrintItems()
        {
            foreach (var item in items.OrderBy(i => i.Id))
            {
                Console.WriteLine(item.Print());
                Console.WriteLine(Separator);
            }
        }

        public TodoItem Find(int id)
        {
            var item = items.FirstOrDefault(i => i.Id == id);
            if (item == null)
                throw new ItemNotFoundException(id);

            return item;
        }

        public List<TodoItem> GetAllItems()
        {
            return items.OrderBy(i => i.Id).ToList();
        }

        public TodoItemDto MapToDto(TodoItem item)
        {
            return new TodoItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Category = item.Category,
                IsCompleted = item.IsCompleted,
                TotalProgress = item.TotalProgress,
                Progressions = item.Progressions.Select(p => new Progression(p.Date, p.Percent)).ToList()
            };
        }
    }
}