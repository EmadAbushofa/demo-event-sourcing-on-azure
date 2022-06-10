using Microsoft.EntityFrameworkCore;
using Todo.Query.Abstractions;
using Todo.Query.Infrastructure.Data;

namespace Todo.Query.ServicesExtensions
{
    public static class EntityFrameworkExtension
    {
        public static void AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database");

            services.AddDbContext<TodoTasksDbContext>(
                options => options.UseSqlServer(connectionString)
            );

            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
