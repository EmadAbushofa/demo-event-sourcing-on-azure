using Todo.Command.Abstractions;
using Todo.Command.Client.TodoQueryProto;
using Todo.Command.Infrastructure.Query;

namespace Todo.Command.Services
{
    public static class ExternalServicesRegistrationExtension
    {
        public static void AddTasksQuery(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ITodoQuery, TodoQueryGrpcClient>();

            services.AddGrpcClient<Tasks.TasksClient>(o =>
            {
                o.Address = new Uri(configuration["TodoQueryUrl"]);
            });
        }
    }
}
