using Beyond_PruebaTecnica_ConsoleApp.Interfaces;
using Beyond_PruebaTecnica_WebAPI.DTOs;
using Beyond_PruebaTecnica_WebAPI.Repositories;
using Beyond_PruebaTecnica_WebAPI.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITodoListRepository, InMemoryTodoListRepository>();
builder.Services.AddSingleton<ITodoList>(sp =>
{
    var repo = sp.GetRequiredService<ITodoListRepository>();
    return TodoListInitalizer.GetList(repo);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
