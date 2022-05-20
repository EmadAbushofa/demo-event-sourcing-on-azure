using Grpc.Core;
using Todo.Command.Abstraction;
using Todo.Command.Events;
using Todo.Command.Events.DataTypes;
using Todo.Command.TodoProto;

namespace Todo.Command.GrpcServices
{
    public class TasksService : Tasks.TasksBase
    {
        private readonly IEventStore _eventStore;
        private readonly ILogger<TasksService> _logger;
        public TasksService(IEventStore eventStore, ILogger<TasksService> logger)
        {
            _eventStore = eventStore;
            _logger = logger;
        }


        public override async Task<Response> Create(CreateRequest request, ServerCallContext context)
        {
            var @event = new TaskCreatedEvent(
                aggregateId: Guid.NewGuid(),
                sequence: 1,
                userId: request.UserId,
                data: new TaskCreatedData()
            );

            await _eventStore.AppendToStreamAsync(@event);

            return new Response()
            {
                Id = @event.AggregateId.ToString()
            };
        }
    }
}