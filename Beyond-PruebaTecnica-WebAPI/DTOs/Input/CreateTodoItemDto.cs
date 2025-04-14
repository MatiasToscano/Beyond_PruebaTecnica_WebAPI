using Beyond_PruebaTecnica_WebAPI.Models;

namespace Beyond_PruebaTecnica_WebAPI.DTOs.Input
{
    public class CreateTodoItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<Progression> Progressions { get; set; } = new();
    }
}
