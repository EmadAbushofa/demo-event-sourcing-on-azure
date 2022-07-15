using Todo.Query.Entities;
using Todo.Query.QueryHandlers.Filter;

namespace Todo.Query.Abstractions
{
    public interface ITasksRepository
    {
        Task<FilterResult> FilterAsync(FilterQuery filter, CancellationToken cancellationToken);
        Task<TodoTask?> FindAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> HasSimilarTodoTaskAsync(string userId, string title, CancellationToken cancellationToken);
        Task<TodoTask?> GetSimilarTodoTaskAsync(string userId, string title, CancellationToken cancellationToken);
        Task AddAsync(TodoTask task);
        Task RemoveAsync(TodoTask task);
    }
}
