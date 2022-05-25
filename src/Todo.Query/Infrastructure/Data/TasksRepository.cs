using Todo.Query.Abstractions;

namespace Todo.Query.Infrastructure.Data
{
    public class TasksRepository : ITasksRepository
    {
        private readonly TodoTasksDbContext _context;

        public TasksRepository(TodoTasksDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(TodoTask task)
        {
            return _context.Tasks.AddAsync(task).AsTask();
        }
    }
}
