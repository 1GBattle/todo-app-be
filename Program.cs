using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Tododb>(options => options.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "TodoApi";
    config.Title = "TodoApi v1";
    config.Version = "v1";
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

// Returns all TodoItems
app.MapGet("/todoItems", async (Tododb db) => await db.TodoItems.ToListAsync());

// Returns a specific TodoItem
app.MapGet(
    "/todoItems/{id}",
    async (Tododb db, int id) =>
        await db.TodoItems.FindAsync(id) is TodoItem todoItem
            ? Results.Ok(todoItem)
            : Results.NotFound()
);

// Creates a new TodoItem
app.MapPost(
    "/todoItem",
    async (Tododb db, TodoItem todoItem) =>
    {
        db.TodoItems.Add(todoItem);
        await db.SaveChangesAsync();
        return Results.Created($"/todoItems/{todoItem.Id}", todoItem);
    }
);

// Updates a TodoItem
app.MapPut(
    "/todoItems/{id}",
    async (int id, TodoItem todoUpdates, Tododb db) =>
    {
        var todo = await db.TodoItems.FindAsync(id);

        if (todo is null)
            return Results.NotFound();

        todo.Description = todoUpdates.Description;
        todo.Title = todoUpdates.Title;
        todo.IsComplete = todoUpdates.IsComplete;

        await db.SaveChangesAsync();
        return Results.Ok(todo);
    }
);

// Deletes a TodoItem
app.MapDelete(
    "/todoItems/{id}",
    async (int id, Tododb db) =>
    {
        var todo = await db.TodoItems.FindAsync(id);

        if (todo is null)
            return Results.NotFound();

        db.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
);

app.Run();
