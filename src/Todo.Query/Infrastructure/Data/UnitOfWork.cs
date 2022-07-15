using Todo.Query.Abstractions;

namespace Todo.Query.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TodoTasksDbContext _context;

        public UnitOfWork(TodoTasksDbContext context)
        {
            _context = context;
            Tasks = new TasksRepository(context);
        }

        public ITasksRepository Tasks { get; }

        public Task CompleteAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);

        public void Dispose() => _context.Dispose();
    }
}
