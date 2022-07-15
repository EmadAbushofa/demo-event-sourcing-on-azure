using Microsoft.EntityFrameworkCore;
using Todo.Query.Abstractions;
using Todo.Query.Entities;
using Todo.Query.QueryHandlers.Filter;

namespace Todo.Query.Infrastructure.Data
{
    public class TasksRepository : ITasksRepository
    {
        private readonly TodoTasksDbContext _context;

        public TasksRepository(TodoTasksDbContext context)
        {
            _context = context;
        }

        public async Task<FilterResult> FilterAsync(FilterQuery filter, CancellationToken cancellationToken)
        {
            var results = await _context.Tasks.ToListAsync(cancellationToken);
            return new FilterResult(
                Page: filter.Page,
                Size: filter.Size,
                Total: results.Count,
                Tasks: results
            );
        }

        public Task<TodoTask?> FindAsync(Guid id, CancellationToken cancellationToken) =>
            _context.Tasks.FindAsync(new object[] { id }, cancellationToken: cancellationToken).AsTask();

        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken) =>
            _context.Tasks.AnyAsync(t => t.Id == id, cancellationToken);

        public Task<bool> HasSimilarTodoTaskAsync(string userId, string title, CancellationToken cancellationToken)
        {
            title = title.Trim().ToUpper();

            return _context.Tasks.AnyAsync(
                            t =>
                                t.UserId == userId &&
                                t.NormalizedTitle == title &&
                                t.IsCompleted == false
                        , cancellationToken);
        }

        public Task<TodoTask?> GetSimilarTodoTaskAsync(string userId, string title, CancellationToken cancellationToken)
        {
            title = title.Trim().ToUpper();

            return _context.Tasks.FirstOrDefaultAsync(
                            t =>
                                t.UserId == userId &&
                                t.NormalizedTitle == title &&
                                t.IsCompleted == false
                        , cancellationToken);
        }

        public Task AddAsync(TodoTask task) => _context.Tasks.AddAsync(task).AsTask();
        public Task RemoveAsync(TodoTask task)
        {
            _context.Tasks.Remove(task);
            return Task.CompletedTask;
        }
    }
}
