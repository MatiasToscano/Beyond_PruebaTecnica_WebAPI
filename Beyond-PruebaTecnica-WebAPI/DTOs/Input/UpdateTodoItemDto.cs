using Beyond_PruebaTecnica_WebAPI.Models;

namespace Beyond_PruebaTecnica_WebAPI.DTOs.Input
{
    public class UpdateTodoItemDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public List<Progression>? Progressions { get; set; }
    }
}
