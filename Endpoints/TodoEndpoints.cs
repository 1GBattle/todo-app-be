using Microsoft.EntityFrameworkCore;

public static class Todo
{
    public static void RegisterTodoEndpoints(this IEndpointRouteBuilder routes)
    {
        var todos = routes.MapGroup("/api/todos");

        // Returns all TodoItems
        todos.MapGet("/todoItems", async (Tododb db) => await db.TodoItems.ToListAsync()).RequireAuthorization();

        // Returns a specific TodoItem
        todos.MapGet(
            "/todoItems/{id}",
            async (Tododb db, int id) =>
                await db.TodoItems.FindAsync(id) is TodoItem todoItem
                    ? Results.Ok(todoItem)
                    : Results.NotFound()
        ).RequireAuthorization();

        // Creates a new TodoItem
        todos.MapPost(
            "/todoItem",
            async (Tododb db, TodoItem todoItem) =>
            {
                db.TodoItems.Add(todoItem);
                await db.SaveChangesAsync();
                return Results.Created($"/todoItems/{todoItem.Id}", todoItem);
            }
        ).RequireAuthorization();

        // Updates a TodoItem
        todos.MapPut(
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
        ).RequireAuthorization();

        // Deletes a TodoItem
        todos.MapDelete(
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
        ).RequireAuthorization();
    }
}
