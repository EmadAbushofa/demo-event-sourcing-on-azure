using MediatR;
using Todo.Command.Abstractions;
using Todo.Command.Exceptions;
using Todo.Command.Models;

namespace Todo.Command.CommandHandlers.ChangeDueDate
{
    public class ChangeDueDateCommandHandler : IRequestHandler<ChangeDueDateCommand, Guid>
    {
        private readonly IEventStore _eventStore;

        public ChangeDueDateCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Guid> Handle(ChangeDueDateCommand command, CancellationToken cancellationToken)
        {
            var events = await _eventStore.GetStreamAsync(command.Id);

            if (events.Count == 0)
                throw new NotFoundException();

            var todoTask = TodoTask.LoadFromHistory(events);

            todoTask.ChangeDueDate(command);

            await _eventStore.AppendToStreamAsync(todoTask);

            return todoTask.Id;
        }
    }
}
