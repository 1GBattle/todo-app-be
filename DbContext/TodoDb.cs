using Microsoft.EntityFrameworkCore;

class Tododb(DbContextOptions<Tododb> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems { get; set; }
}
