using Microsoft.EntityFrameworkCore;
using Todo.Query.Abstractions;
using Todo.Query.Infrastructure.Data;

namespace Todo.Query.Services
{
    public static class DatabaseExtension
    {
        public static void AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<TodoTasksDbContext>(
                options => options.UseSqlServer(connectionString)
            );

            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
