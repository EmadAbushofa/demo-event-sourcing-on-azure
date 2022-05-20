using Todo.Command.Abstraction;
using Todo.Command.Client.TodoQueryProto;

namespace Todo.Command.Infrastructure.Query
{
    public class TodoQueryGrpcClient : ITodoQuery
    {
        private readonly Tasks.TasksClient _queryClient;
        private readonly ILogger<TodoQueryGrpcClient> _logger;

        public TodoQueryGrpcClient(Tasks.TasksClient queryClient, ILogger<TodoQueryGrpcClient> logger)
        {
            _queryClient = queryClient;
            _logger = logger;
        }

        public async Task<bool> SimilarTitleExistsAsync(string title)
        {
            try
            {
                var response = await _queryClient.SimilarTitleExistsAsync(new SimilarTitleExistsRequest()
                {
                    Title = title,
                });

                return response.Exists;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Call for SimilarTitleExists exception.");
                return false;
            }
        }
    }
}
