using Beyond_PruebaTecnica_WebAPI.Models;

namespace Beyond_PruebaTecnica_WebAPI.DTOs.Output
{
    public class TodoItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal TotalProgress { get; set; }
        public bool IsCompleted { get; set; }
        public List<Progression> Progressions { get; set; }
    }
}
