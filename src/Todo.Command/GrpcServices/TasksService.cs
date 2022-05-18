using Grpc.Core;
using Todo.Command.TodoProto;

namespace Todo.Command.GrpcServices
{
    public class TasksService : Tasks.TasksBase
    {
        private readonly ILogger<TasksService> _logger;
        public TasksService(ILogger<TasksService> logger)
        {
            _logger = logger;
        }


        public override async Task<Response> Create(CreateRequest request, ServerCallContext context)
        {
            return new Response()
            {
                Id = Guid.NewGuid().ToString()
            };
        }
    }
}