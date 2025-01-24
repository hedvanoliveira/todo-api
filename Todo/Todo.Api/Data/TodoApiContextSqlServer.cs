using Microsoft.EntityFrameworkCore;
using Todo.Api.Models;

namespace Todo.Api.Data;

public class TodoApiContextSqlServer(DbContextOptions<TodoApiContextSqlServer> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItens { get; set; } = default!;
}
