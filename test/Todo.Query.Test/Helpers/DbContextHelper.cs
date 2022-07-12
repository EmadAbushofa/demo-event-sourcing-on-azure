using Microsoft.Extensions.DependencyInjection;
using Todo.Query.Entities;
using Todo.Query.Infrastructure.Data;

namespace Todo.Query.Test.Helpers
{
    public class DbContextHelper
    {
        private readonly IServiceProvider _provider;

        public DbContextHelper(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<TResult> Query<TResult>(Func<TodoTasksDbContext, Task<TResult>> useInScope)
        {
            using var scope = _provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();
            return await useInScope(context);
        }

        public async ValueTask<TResult> Query<TResult>(Func<TodoTasksDbContext, ValueTask<TResult>> useInScope)
        {
            using var scope = _provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();
            return await useInScope(context);
        }

        public async Task<TodoTask> InsertAsync(TodoTask todoTask)
        {
            using var scope = _provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();
            await context.Tasks.AddAsync(todoTask);
            await context.SaveChangesAsync();
            return todoTask;
        }
    }
}