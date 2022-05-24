using MediatR;
using Todo.Query.Server.TodoProto;

namespace Todo.Query.GrpcServices
{
    public class TasksService : Tasks.TasksBase
    {
        private readonly IMediator _mediator;

        public TasksService(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}