using MediatR;
using Todo.Command.Abstractions;
using Todo.Command.Exceptions;
using Todo.Command.Models;

namespace Todo.Command.CommandHandlers.UpdateInfo
{
    public class UpdateTaskInfoCommandHandler : IRequestHandler<UpdateTaskInfoCommand, Guid>
    {
        private readonly IEventStore _eventStore;
        private readonly ITodoQuery _query;

        public UpdateTaskInfoCommandHandler(IEventStore eventStore, ITodoQuery query)
        {
            _eventStore = eventStore;
            _query = query;
        }

        public async Task<Guid> Handle(UpdateTaskInfoCommand command, CancellationToken cancellationToken)
        {
            var events = await _eventStore.GetStreamAsync(command.Id);

            if (events.Count == 0)
                throw new NotFoundException();

            var todoTask = TodoTask.LoadFromHistory(events);

            todoTask.UpdateInfo(command);

            await _eventStore.AppendToStreamAsync(todoTask);

            return todoTask.Id;
        }
    }
}
