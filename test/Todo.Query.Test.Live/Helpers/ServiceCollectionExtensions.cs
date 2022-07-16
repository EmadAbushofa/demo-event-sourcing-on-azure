using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Todo.Query.Infrastructure.Data;
using Todo.Query.Test.Live.EventBus;

namespace Todo.Query.Test.Live.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEventHandlingListener(this IServiceCollection services)
        {
            services.AddSingleton<TodoHandlingListener>();

            services.AddSingleton(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();

                var queryTopic = configuration["ServiceBus:QueryTopicName"];

                return new TodoHandlingListenerOptions()
                {
                    QueryTopicName = queryTopic,
                    QuerySubscriptionName = "todo-tasks-query-test",
                };
            });
        }

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