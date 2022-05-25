using Microsoft.EntityFrameworkCore;
using Todo.Query.Abstractions;
using Todo.Query.Infrastructure.Data.Configurations;

namespace Todo.Query.Infrastructure.Data
{
    public class TodoTasksDbContext : DbContext
    {
        public TodoTasksDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TodoTaskConfiguration());
        }

        public DbSet<TodoTask> Tasks { get; private set; } = null!;
    }
}
