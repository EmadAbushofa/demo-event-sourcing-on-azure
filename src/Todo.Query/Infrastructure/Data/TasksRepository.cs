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

        public Task<bool> HasSimilarTodoTaskAsync(string userId, string title)
        {
            title = title.Trim().ToUpper();

            return _context.Tasks.AnyAsync(
                            t =>
                                t.UserId == userId &&
                                t.NormalizedTitle == title &&
                                t.IsCompleted == false
                        );
        }

        public Task<TodoTask?> GetSimilarTodoTaskAsync(string userId, string title)
        {
            title = title.Trim().ToUpper();

            return _context.Tasks.FirstOrDefaultAsync(
                            t =>
                                t.UserId == userId &&
                                t.NormalizedTitle == title &&
                                t.IsCompleted == false
                        );
        }

        public Task AddAsync(TodoTask task) => _context.Tasks.AddAsync(task).AsTask();
        public Task RemoveAsync(TodoTask task)
        {
            _context.Tasks.Remove(task);
            return Task.CompletedTask;
        }
    }
}
