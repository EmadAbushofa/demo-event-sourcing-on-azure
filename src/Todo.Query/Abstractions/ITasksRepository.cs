using Todo.Query.Entities;

namespace Todo.Query.Abstractions
{
    public interface ITasksRepository
    {
        Task<TodoTask?> FindAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task AddAsync(TodoTask task);
    }
}
