using MediatR;
using Todo.Command.Abstractions;
using Todo.Command.Exceptions;
using Todo.Command.Models;

namespace Todo.Command.CommandHandlers.Complete
{
    public class CompleteCommandHandler : IRequestHandler<CompleteCommand, Guid>
    {
        private readonly IEventStore _eventStore;

        public CompleteCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Guid> Handle(CompleteCommand command, CancellationToken cancellationToken)
        {
            var events = await _eventStore.GetStreamAsync(command.Id);

            if (events.Count == 0)
                throw new NotFoundException();

            var todoTask = TodoTask.LoadFromHistory(events);

            todoTask.Complete(command);

            await _eventStore.AppendToStreamAsync(todoTask);

            return todoTask.Id;
        }
    }
}
