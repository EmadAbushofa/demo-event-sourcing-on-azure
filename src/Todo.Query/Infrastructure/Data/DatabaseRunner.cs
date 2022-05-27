using Microsoft.EntityFrameworkCore;

namespace Todo.Query.Infrastructure.Data
{
    public class DatabaseRunner : IHostedService
    {
        private readonly IServiceProvider _provider;

        public DatabaseRunner(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();
            await context.Database.MigrateAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
