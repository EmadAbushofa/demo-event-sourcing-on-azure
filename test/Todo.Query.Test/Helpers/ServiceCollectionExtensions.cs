using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Todo.Query.Infrastructure.Data;

namespace Todo.Query.Test.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static void ReplaceWithInMemoryDatabase(this IServiceCollection services)
        {
            var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<TodoTasksDbContext>));

            services.Remove(descriptor);

            descriptor = services.Single(d => d.ImplementationType == typeof(DatabaseRunner));

            services.Remove(descriptor);

            var dbName = Guid.NewGuid().ToString();

            services.AddDbContext<TodoTasksDbContext>(options => options.UseInMemoryDatabase(dbName));
        }
    }
}