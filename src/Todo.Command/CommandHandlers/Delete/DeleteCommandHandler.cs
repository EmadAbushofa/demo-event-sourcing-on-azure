using MediatR;
using Todo.Command.Abstractions.Persistence;
using Todo.Command.Domain;
using Todo.Command.Exceptions;

namespace Todo.Command.CommandHandlers.Delete
{
    public class DeleteCommandHandler : IRequestHandler<DeleteCommand, Guid>
    {
        private readonly IEventStore _eventStore;

        public DeleteCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Guid> Handle(DeleteCommand command, CancellationToken cancellationToken)
        {
            var events = await _eventStore.GetStreamAsync(command.Id);

            if (events.Count == 0)
                throw new NotFoundException();

            var todoTask = TodoTask.LoadFromHistory(events);

            todoTask.Delete(command);

            await _eventStore.AppendToStreamAsync(todoTask);

            return todoTask.Id;
        }
    }
}
