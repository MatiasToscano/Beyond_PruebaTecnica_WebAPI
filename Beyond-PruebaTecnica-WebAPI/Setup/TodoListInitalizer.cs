using Beyond_PruebaTecnica_ConsoleApp.Interfaces;
using Beyond_PruebaTecnica_WebAPI.DTOs;
using Beyond_PruebaTecnica_WebAPI.Services;

namespace Beyond_PruebaTecnica_WebAPI.Setup
{
    public static class TodoListInitalizer
    {
        public static ITodoList GetList(ITodoListRepository repository)
        {
            var todoList = new TodoListService(repository);

            var id1 = repository.GetNextId();
            todoList.AddItem(id1, "Complete Project Report", "Finish the final report for the project", "Work");
            todoList.RegisterProgression(id1, new DateTime(2025, 3, 18), 30);
            todoList.RegisterProgression(id1, new DateTime(2025, 3, 19), 50);
            todoList.RegisterProgression(id1, new DateTime(2025, 3, 20), 20);

            int id2 = repository.GetNextId();
            todoList.AddItem(id2, "Buy Groceries", "Pick up fruits and veggies", "Personal");
            todoList.RegisterProgression(id2, new DateTime(2025, 4, 1), 40);
            todoList.RegisterProgression(id2, new DateTime(2025, 4, 2), 30);

            int id3 = repository.GetNextId();
            todoList.AddItem(id3, "Paint Miniatures", "Finish painting the space marines squad", "Hobby");
            todoList.RegisterProgression(id3, new DateTime(2025, 4, 5), 25);
            todoList.RegisterProgression(id3, new DateTime(2025, 4, 6), 25);
            todoList.RegisterProgression(id3, new DateTime(2025, 4, 7), 50);

            int id4 = repository.GetNextId();
            todoList.AddItem(id4, "Read Book", "Finish reading 'Clean Code'", "Personal");
            todoList.RegisterProgression(id4, new DateTime(2025, 4, 10), 40);

            int id5 = repository.GetNextId();
            todoList.AddItem(id5, "Refactor Code", "Refactor the legacy module for maintainability", "Work");
            todoList.RegisterProgression(id5, new DateTime(2025, 4, 10), 30);
            todoList.RegisterProgression(id5, new DateTime(2025, 4, 11), 40);

            int id6 = repository.GetNextId();
            todoList.AddItem(id6, "Learn Guitar", "Practice chords and a new song", "Hobby");
            todoList.RegisterProgression(id6, new DateTime(2025, 4, 8), 20);
            todoList.RegisterProgression(id6, new DateTime(2025, 4, 9), 25);

            int id7 = repository.GetNextId();
            todoList.AddItem(id7, "Meditate", "Daily mindfulness session", "Personal");
            todoList.RegisterProgression(id7, new DateTime(2025, 4, 11), 10);

            int id8 = repository.GetNextId();
            todoList.AddItem(id8, "Fix Bug #421", "Resolve null reference exception in API", "Work");
            todoList.RegisterProgression(id8, new DateTime(2025, 4, 10), 50);
            todoList.RegisterProgression(id8, new DateTime(2025, 4, 11), 50);

            return todoList;
        }
    }
}
