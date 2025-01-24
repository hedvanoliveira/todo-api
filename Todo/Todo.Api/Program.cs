using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Todo.Api.Data;
using Todo.Api.Models;
using Microsoft.AspNetCore.OpenApi;
using Todo.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TodoApiContextSqlServer>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapTodoItemEndpoints();

app.Run();


public static class TodoItemEndpoints
{
	public static void MapTodoItemEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/TodoItens").WithTags(nameof(TodoItem));

        group.MapGet("/", async (TodoApiContextSqlServer db) =>
        {
            return await db.TodoItens.ToListAsync();
        })
        .WithName("GetAllTodoItems")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<TodoItem>, NotFound>> (int id, TodoApiContextSqlServer db) =>
        {
            return await db.TodoItens.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is TodoItem model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetTodoItemById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, TodoItem todoItem, TodoApiContextSqlServer db) =>
        {
            var affected = await db.TodoItens
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  //.SetProperty(m => m.Id, todoItem.Id)
                  .SetProperty(m => m.Description, todoItem.Description)
                  //.SetProperty(m => m.Created, todoItem.Created)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateTodoItem")
        .WithOpenApi();

        group.MapPost("/", async (TodoItem todoItem, TodoApiContextSqlServer db) =>
        {
            db.TodoItens.Add(todoItem);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/TodoItens/{todoItem.Id}",todoItem);
        })
        .WithName("CreateTodoItem")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, TodoApiContextSqlServer db) =>
        {
            var affected = await db.TodoItens
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteTodoItem")
        .WithOpenApi();
    }
}