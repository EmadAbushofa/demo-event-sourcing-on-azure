using MediatR;
using Todo.Command.Abstraction;
using Todo.Command.Models;

namespace Todo.Command.Features.Create
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
    {
        private readonly IEventStore _eventStore;

        public CreateTaskCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Guid> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            var todoTask = TodoTask.Create(command);

            await _eventStore.AppendToStreamAsync(
                events: todoTask.GetUncommittedEvents(),
                aggregateId: todoTask.Id
            );

            return todoTask.Id;
        }
    }
}
