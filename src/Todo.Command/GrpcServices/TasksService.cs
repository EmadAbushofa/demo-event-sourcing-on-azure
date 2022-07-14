using Grpc.Core;
using MediatR;
using Todo.Command.Extensions;
using Todo.Command.TodoProto;

namespace Todo.Command.GrpcServices
{
    public class TasksService : Tasks.TasksBase
    {
        private readonly IMediator _mediator;

        public TasksService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<Response> Create(CreateRequest request, ServerCallContext context)
        {
            var command = request.ToCommand();

            var id = await _mediator.Send(command);

            return new Response() { Id = id.ToString() };
        }

        public override async Task<Response> UpdateInfo(UpdateInfoRequest request, ServerCallContext context)
        {
            var command = request.ToCommand();

            var id = await _mediator.Send(command);

            return new Response() { Id = id.ToString() };
        }

        public override async Task<Response> ChangeDueDate(ChangeDueDateRequest request, ServerCallContext context)
        {
            var command = request.ToCommand();

            var id = await _mediator.Send(command);

            return new Response() { Id = id.ToString() };
        }
    }
}