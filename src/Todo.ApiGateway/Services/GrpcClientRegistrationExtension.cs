using Todo.ApiGateway.TodoProto.Command;

namespace Todo.ApiGateway.Services
{
    public static class GrpcClientRegistrationExtension
    {
        public static void AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
        {
            configuration = configuration.GetSection("ServicesUrls");

            services.AddGrpcClient<Tasks.TasksClient>(o =>
            {
                o.Address = new Uri(configuration["TodoCommandUrl"]);
            });
        }
    }
}
