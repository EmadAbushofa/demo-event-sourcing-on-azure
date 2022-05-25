namespace Todo.Query.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        ITasksRepository Tasks { get; }
        Task CompleteAsync();
    }
}
