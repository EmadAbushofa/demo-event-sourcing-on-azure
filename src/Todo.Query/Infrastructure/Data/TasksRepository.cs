using Microsoft.EntityFrameworkCore;
using Todo.Query.Abstractions;
using Todo.Query.Entities;

namespace Todo.Query.Infrastructure.Data
{
    public class TasksRepository : ITasksRepository
    {
        private readonly TodoTasksDbContext _context;

        public TasksRepository(TodoTasksDbContext context)
        {
            _context = context;
        }

        public Task<TodoTask?> FindAsync(Guid id)
            => _context.Tasks.FindAsync(id).AsTask();

        public Task<bool> ExistsAsync(Guid id) => _context.Tasks.AnyAsync(t => t.Id == id);

        public Task AddAsync(TodoTask task) => _context.Tasks.AddAsync(task).AsTask();
    }
}
