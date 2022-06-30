using CommandClient = Todo.ApiGateway.TodoProto.Command.Tasks.TasksClient;
using QueryClient = Todo.ApiGateway.TodoProto.Query.Tasks.TasksClient;


namespace Todo.ApiGateway.Services
{
    public static class GrpcClientRegistrationExtension
    {
        public static void AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
        {
            configuration = configuration.GetSection("ServicesUrls");

            services.AddGrpcClient<CommandClient>(nameof(CommandClient), o =>
            {
                o.Address = new Uri(configuration["TodoCommandUrl"]);
            });

            services.AddGrpcClient<QueryClient>(nameof(QueryClient), o =>
            {
                o.Address = new Uri(configuration["TodoQueryUrl"]);
            });
        }
    }
}
