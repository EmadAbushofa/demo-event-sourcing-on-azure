using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Todo.Query.Infrastructure.Data;

namespace Todo.Query.Test.Live.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static void TruncateSqlTables(this IServiceCollection services)
        {
            services.AddHostedService<DbTruncate>();
        }

        private class DbTruncate : IHostedService
        {
            private readonly IServiceProvider _provider;

            public DbTruncate(IServiceProvider provider)
            {
                _provider = provider;
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                var scope = _provider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();
                context.RemoveRange(context.Tasks);
                return context.SaveChangesAsync(cancellationToken);
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}