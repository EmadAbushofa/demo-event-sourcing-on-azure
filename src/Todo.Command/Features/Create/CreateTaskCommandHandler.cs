using MediatR;
using Todo.Command.Abstraction;
using Todo.Command.Events;
using Todo.Command.Events.DataTypes;

namespace Todo.Command.Features.Create
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
    {
        private readonly IEventStore _eventStore;
        private readonly ILogger<CreateTaskCommandHandler> _logger;
        public CreateTaskCommandHandler(IEventStore eventStore, ILogger<CreateTaskCommandHandler> logger)
        {
            _eventStore = eventStore;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            var @event = new TaskCreatedEvent(
                aggregateId: Guid.NewGuid(),
                sequence: 1,
                userId: command.UserId,
                data: new TaskCreatedData()
            );

            await _eventStore.AppendToStreamAsync(@event);

            return @event.AggregateId;
        }
    }
}
