namespace Todo.Query.Abstractions
{
    public interface ITasksRepository
    {
        Task AddAsync(TodoTask task);
    }
}
