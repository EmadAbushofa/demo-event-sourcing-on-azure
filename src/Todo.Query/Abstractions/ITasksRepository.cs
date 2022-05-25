namespace Todo.Query.Abstractions
{
    public interface ITasksRepository
    {
        Task<bool> ExistsAsync(Guid id);
        Task AddAsync(TodoTask task);
    }
}
