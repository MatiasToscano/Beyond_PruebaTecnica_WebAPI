using Beyond_PruebaTecnica_WebAPI.Controllers;
using Beyond_PruebaTecnica_WebAPI.DTOs.Input;
using Beyond_PruebaTecnica_WebAPI.DTOs.Output;
using Beyond_PruebaTecnica_WebAPI.Models;
using Beyond_PruebaTecnica_ConsoleApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Beyond_PruebaTecnica_WebAPI.DTOs;
using Beyond_PruebaTecnica_WebAPI.Exceptions;

namespace Beyond.PruebaTecnica.Tests
{
    public class TodoListControllerTests
    {
        private readonly Mock<ITodoList> mockTodoList;
        private readonly Mock<ITodoListRepository> mockRepository;
        private readonly TodoListController controller;

        public TodoListControllerTests()
        {
            mockTodoList = new Mock<ITodoList>();
            mockRepository = new Mock<ITodoListRepository>();

            mockRepository.Setup(r => r.GetAllCategories()).Returns(new List<string> { "Work", "Personal", "Hobby" });

            controller = new TodoListController(mockTodoList.Object, mockRepository.Object);
        }

        [Fact]
        public void GetAll_WhenEmpty_ReturnsEmptyList()
        {
            mockTodoList.Setup(t => t.GetAllItems()).Returns(new List<TodoItem>());

            var result = controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<TodoItemDto>>(okResult.Value);
            Assert.Empty(list);
        }

        [Fact]
        public void GetAll_ReturnsAllItems()
        {
            var items = new List<TodoItem>
            {
                new(1, "Test", "Desc", "Work")
            };

            mockTodoList.Setup(t => t.GetAllItems()).Returns(items);
            mockTodoList.Setup(t => t.MapToDto(It.IsAny<TodoItem>())).Returns<TodoItem>(item => new TodoItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Category = item.Category,
                Progressions = item.Progressions,
                TotalProgress = item.Progressions.Sum(p => p.Percent),
                IsCompleted = item.Progressions.Sum(p => p.Percent) >= 100
            });

            var actionResult = controller.Get();
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var value = Assert.IsAssignableFrom<IEnumerable<TodoItemDto>>(okResult.Value);

            Assert.Single(value);
        }

        [Fact]
        public void GetById_ExistingItem_ReturnsItem()
        {
            var item = new TodoItem(1, "Test", "Desc", "Work");
            mockTodoList.Setup(t => t.Find(1)).Returns(item);
            mockTodoList.Setup(t => t.MapToDto(item)).Returns(new TodoItemDto { Id = 1, Title = "Test" });

            var result = controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<TodoItem>(okResult.Value);
            Assert.Equal(1, dto.Id);
        }

        [Fact]
        public void GetById_NonExistingItem_ReturnsNotFound()
        {
            mockTodoList.Setup(t => t.Find(999)).Throws(new ItemNotFoundException(999));

            var result = controller.GetById(999);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFoundResult.Value);
            
            Assert.Equal("No existe un item con Id = 999.", message);
        }

        [Fact]
        public void Post_ValidItem_ReturnsCreatedItem()
        {
            var dto = new CreateTodoItemDto
            {
                Title = "New",
                Description = "NewDesc",
                Category = "Work",
                Progressions = new List<Progression> { new(DateTime.Today, 50) }
            };

            mockRepository.Setup(r => r.GetNextId()).Returns(1);

            var item = new TodoItem(1, dto.Title, dto.Description, dto.Category);
            item.AddProgression(DateTime.Today, 50);

            mockTodoList.Setup(t => t.AddItem(1, dto.Title, dto.Description, dto.Category));
            mockTodoList.Setup(t => t.Find(1)).Returns(item);
            mockTodoList.Setup(t => t.RegisterProgression(1, DateTime.Today, 50));
            mockTodoList.Setup(t => t.MapToDto(item)).Returns(new TodoItemDto
            {
                Id = 1,
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                Progressions = item.Progressions,
                TotalProgress = 50,
                IsCompleted = false
            });

            var result = controller.Post(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<TodoItemDto>(okResult.Value);
            Assert.Equal(1, returned.Id);
            Assert.Equal(50, returned.TotalProgress);
        }

        [Fact]
        public void Post_InvalidData_ReturnsEmptyItemr()
        {
            var dto = new CreateTodoItemDto();

            mockRepository.Setup(r => r.GetNextId()).Returns(1);

            var item = new TodoItem(1, dto.Title, dto.Description, dto.Category);
            item.AddProgression(DateTime.Today, 50);

            mockTodoList.Setup(t => t.AddItem(1, dto.Title, dto.Description, dto.Category));
            mockTodoList.Setup(t => t.Find(1)).Returns(item);
            mockTodoList.Setup(t => t.RegisterProgression(1, DateTime.Today, 50));
            mockTodoList.Setup(t => t.MapToDto(item)).Returns(new TodoItemDto());

            var result = controller.Post(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<TodoItemDto>(okResult.Value);
            Assert.Null(returned.Category);
            Assert.Null(returned.Description);
            Assert.False(returned.IsCompleted);
            Assert.Null(returned.Progressions);
            Assert.Null(returned.Title);
            Assert.Equal(0, returned.TotalProgress);
        }

        [Fact]
        public void Put_NonExistingId_ReturnsNotFound()
        {
            mockTodoList.Setup(t => t.Find(999)).Returns((TodoItem)null);

            var result = controller.Put(999, new CreateTodoItemDto());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Put_ValidId_ReturnsUpdatedItem()
        {
            var existing = new TodoItem(1, "Old", "OldDesc", "Work");

            mockTodoList.Setup(t => t.Find(1)).Returns(existing);
            mockTodoList.Setup(t => t.UpdateItem(1, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            mockTodoList.Setup(t => t.MapToDto(existing)).Returns(new TodoItemDto
            {
                Id = 1,
                Title = "New",
                Description = "NewDesc",
                Category = "Work",
                Progressions = new List<Progression> { new(DateTime.Today, 100) },
                TotalProgress = 100,
                IsCompleted = true
            });

            var updateDto = new CreateTodoItemDto
            {
                Title = "New",
                Description = "NewDesc",
                Category = "Work",
                Progressions = new List<Progression> { new(DateTime.Today, 100) }
            };

            var result = controller.Put(1, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<TodoItemDto>(okResult.Value);
            Assert.Equal("New", dto.Title);
            Assert.Equal(100, dto.TotalProgress);
        }

        [Fact]
        public void Patch_ValidProgression_AddsProgression()
        {
            var item = new TodoItem(1, "PatchTask", "Patch", "Work");
            mockTodoList.Setup(t => t.Find(1)).Returns(item);
            mockTodoList.Setup(t => t.RegisterProgression(1, It.IsAny<DateTime>(), It.IsAny<decimal>()));
            mockTodoList.Setup(t => t.MapToDto(item)).Returns(new TodoItemDto
            {
                Id = 1,
                Title = "PatchTask",
                Progressions = new List<Progression> { new(DateTime.Today, 100) },
                TotalProgress = 100,
                IsCompleted = true
            });

            var patchDto = new UpdateTodoItemDto
            {
                Title = "Updated",
                Description = "Updated Desc",
                Category = "Hobby",
                Progressions = new List<Progression> { new(DateTime.Today, 100) }
            };

            var result = controller.Patch(1, patchDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<TodoItemDto>(okResult.Value);
            Assert.True(dto.IsCompleted);
            Assert.Equal(100, dto.TotalProgress);
        }

        [Fact]
        public void Patch_NonExistingItem_ReturnsNotFound()
        {
            mockTodoList.Setup(t => t.Find(123)).Throws(new ItemNotFoundException(123));

            var patchDto = new UpdateTodoItemDto
            {
                Title = "Updated",
                Description = "Updated Desc",
                Category = "Hobby",
                Progressions = new List<Progression> { new(DateTime.Today, 100) }
            };

            var result = controller.Patch(123, patchDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Delete_ExistingItem_ReturnsOk()
        {
            var item = new TodoItem(1, "Task", "Desc", "Work");
            mockTodoList.Setup(t => t.Find(1)).Returns(item);
            mockTodoList.Setup(t => t.RemoveItem(1));

            var result = controller.Delete(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("El ítem con id 1 ha sido removido con éxito.", okResult.Value);
        }

        [Fact]
        public void Delete_NonExistingItem_ThrowsItemNotFoundException_ReturnsBadRequest()
        {
            mockTodoList.Setup(t => t.RemoveItem(2)).Throws(new ItemNotFoundException(2));

            var result = controller.Delete(2);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No existe un item con Id = 2.", badRequest.Value);
        }
    }
}
