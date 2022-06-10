using Todo.Command.Abstractions;
using Todo.Command.Infrastructure.Query;

namespace Todo.Command.Services
{
    public static class ExternalServicesRegistrationExtension
    {
        public static void AddTasksQuery(this IServiceCollection services)
        {
            services.AddTransient<ITodoQuery, TempTodoQuery>();
        }
    }
}
